using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// Custom Post-processor for imorting models.
    /// </summary>
    public class TemplateModelPostprocessor : AssetPostprocessor
    {
        private static List<string> _characterNames = null;
        public static List<string> characterNames
        {
            get
            {
                if (_characterNames == null)
                {
                    _characterNames = new List<string>(Dragon.characterNames);
                }
                return _characterNames;
            }

        }

        public void OnPreprocessModel()
        {
            // asset path
            string assetPath = assetImporter.assetPath;

            // file name
            string fileName = Path.GetFileName(assetPath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(assetPath);

            // Log
            Debug.Log("TemplateModelPostprocessor.OnPreprocessModel() : Preprocessing Asset at " + assetPath);

            // Dragon animations
            if (fileName.ToLower().EndsWith(".fbx") && assetPath.Contains("Models/Dragons/"))
            {
                ModelImporter modelImporter = assetImporter as ModelImporter;

                if (modelImporter != null)
                {
                    // model import settings
                    if (modelImporter.globalScale < 1.0f)
                    {
                        modelImporter.globalScale = 1.0f;
                    }
                    modelImporter.importAnimation = true;
                    modelImporter.optimizeMeshPolygons = false;
                    modelImporter.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
                    

                    // animation clip import settings
                    ModelImporterClipAnimation[] importerClips = modelImporter.clipAnimations;

                    // if importer clips is empty, try to get take infos directly in model importer.
                    if (importerClips.Length == 0)
                    {
                        PropertyInfo propertyInfo = typeof(ModelImporter).GetProperty("importedTakeInfos", BindingFlags.NonPublic | BindingFlags.Instance);
                        TakeInfo[] takeInfos = (TakeInfo[])propertyInfo.GetValue(modelImporter, null);

                        List<ModelImporterClipAnimation> importerClipList = new List<ModelImporterClipAnimation>();
                        foreach (TakeInfo takeInfo in takeInfos)
                        {
                            ModelImporterClipAnimation importerClip = new ModelImporterClipAnimation();

                            importerClip.name = takeInfo.name;
                            importerClip.takeName = takeInfo.name;
                            importerClip.firstFrame = Mathf.Round(takeInfo.bakeStartTime * takeInfo.sampleRate);
                            importerClip.lastFrame = Mathf.Round(takeInfo.bakeStopTime * takeInfo.sampleRate);
                            importerClip.maskType = ClipAnimationMaskType.CreateFromThisModel;
                            importerClip.keepOriginalPositionY = true;

                            importerClipList.Add(importerClip);
                        }

                        importerClips = importerClipList.ToArray();
                    }

                    // process each clips
                    foreach (ModelImporterClipAnimation importerClip in importerClips)
                    {
                        if (importerClip.takeName.Equals("Take 001"))
                        {
                            // Needs to repeat
                            if (fileName.Contains("Idle") ||
                               fileName.Contains("Stand") ||
                               fileName.Contains("Run") ||
                               fileName.Contains("Walk") ||
                               fileName.Contains("Dash") ||
                               fileName.Contains("Swim") ||
                               fileName.Contains("Fly") ||
                               fileName.Contains("Continue"))
                            {
                                importerClip.loopTime = true;
                            }

                            // Rename Take 001 to File Name without Character Name
                            // For Better Identifying
                            foreach (string character in characterNames)
                            {
                                string prefix = string.Format("{0}_", character);
                                if (fileNameWithoutExtension.StartsWith(prefix))
                                {
                                    importerClip.name = fileNameWithoutExtension;
                                }
                            }
                        }
                    }

                    // Reset!
                    modelImporter.clipAnimations = importerClips;
                }
            }
        }

        public void OnPostprocessModel(GameObject go)
        {
            // Remove Scale data
            _RemoveScaleDataOnAnimations(go);

            // Remove Root transform data
            _RemoveRootTransformDataOnAnimations(go);
        }

        private AnimationClip[] _GetAnimationClips(GameObject go)
        {
            /**
             * Find All Animation Clips (Unity 3.x)
             * */
            AnimationClip[] clips = AnimationUtility.GetAnimationClips(go);

            /**
             * Find All Animation Clips (Unity 4.x or higher)
             * */
            if (clips.Length == 0)
            {
                clips = UnityEngine.Object.FindObjectsOfType<AnimationClip>();
            }

            return clips;
        }

        private void _RemoveScaleDataOnAnimations(GameObject go)
        {
            // Find all animation clips
            AnimationClip[] clips = _GetAnimationClips(go);

            // Loop
            if (clips != null)
            {
                foreach (AnimationClip clip in clips)
                {
                    _RemoveScaleDataOnClip(clip);
                }
            }
        }

        private void _RemoveRootTransformDataOnAnimations(GameObject go)
        {
            // Find all animation clips
            AnimationClip[] clips = _GetAnimationClips(go);

            // Loop
            if (clips != null)
            {
                foreach (AnimationClip clip in clips)
                {
                    _RemoveRootTransformDataOnClip(clip);
                }
            }
        }

        private void _RemoveScaleDataOnClip(AnimationClip clip)
        {
            if (clip != null)
            {
                Debug.Log("TemplateModelPostprocessor._RemoveScaleDataOnClip() : Removing scale data on clip (" + clip.name + ").");

                AnimationClipCurveData[] curveDataArr = AnimationUtility.GetAllCurves(clip, false);

                /**
                 * In order to support custom bone scaling, 
                 * We'll remove scale value in all animations.
                 * */
                foreach (AnimationClipCurveData curveData in curveDataArr)
                {
                    //Debug.Log ("Curve Data Name : " + curveData.propertyName + ", target : " + curveData.target + ", path : " + curveData.path);
                    if (curveData.propertyName.ToLower().Contains("scale"))
                    {
                        clip.SetCurve(curveData.path, typeof(Transform), "localScale", null);
                    }
                }
            }
        }

        private void _RemoveRootTransformDataOnClip(AnimationClip clip)
        {
            if (clip != null)
            {
                Debug.Log("TemplateModelPostprocessor._RemoveScaleDataOnClip() : Removing root transform data on clip (" + clip.name + ").");

                clip.SetCurve("", typeof(Transform), "localPosition", null);
                clip.SetCurve("", typeof(Transform), "localRotation", null);

                /*
                AnimationClipCurveData[] curveDataArr = AnimationUtility.GetAllCurves(clip, false);

                foreach (AnimationClipCurveData curveData in curveDataArr)
                {
                    //Debug.Log ("Curve Data Name : " + curveData.propertyName + ", type : " + curveData.type + ", path : " + curveData.path);
                    if (string.IsNullOrEmpty(curveData.propertyName))
                    {
                        clip.SetCurve(curveData.path, typeof(Transform), "localPosition", null);
                        clip.SetCurve(curveData.path, typeof(Transform), "localRotation", null);
                    }
                }
                */
            }
        }
    }
}