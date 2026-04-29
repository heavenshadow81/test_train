using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// The type of free drawing object.
    /// </summary>
    public enum FreeDrawingObjectType
    {
        None,
        Car,
        Robot,
        Airplane
    }

    /// <summary>
    /// The base class of ALL free drawing objects.
    /// </summary>
    public abstract class FreeDrawingObjectBone : BoneObject
    {
        #region Public variables
        public GameObject[] eyePrefabs;
        public Texture2D[] eyeNormalTextures;
        public Texture2D[] eyeCryTextures;
        public int color = 0;
        #endregion

        #region Properties
        /// <summary>
        /// The type of object.
        /// </summary>
        public abstract FreeDrawingObjectType objectType { get; }
        #endregion

        #region Protected variables
        protected GameObject _leftEye, _rightEye;
        #endregion

        #region Constants
        public const string kEyeLBone = "eye_l";
        public const string kEyeRBone = "eye_r";
        #endregion

        public virtual void PrepareDefaultAccessories()
        {
            _PrepareDefaultEyeAccessories();
        }

        protected void _PrepareDefaultEyeAccessories()
        {
            // Checks bone existence
            if (GetBone(kEyeLBone) == null || GetBone(kEyeRBone) == null)
                return;

            // Checks prefab existence
            if (eyePrefabs == null || eyePrefabs.Length == 0)
                return;

            // Select eye prefab
            GameObject prefab = eyePrefabs[Random.Range(0, eyePrefabs.Length)];

            // Instantiate eyes on the 
            GameObject leftEye = Instantiate(prefab);
            SetAccessory(kEyeLBone, leftEye, true);
            leftEye.transform.localRotation = Quaternion.Euler(90, 0, 0);
            _leftEye = leftEye;
            GameObject rightEye = Instantiate(prefab);
            SetAccessory(kEyeRBone, rightEye, true);
            rightEye.transform.localRotation = Quaternion.Euler(90, 0, 0);
            _rightEye = rightEye;

            // Set textures
            if (eyeNormalTextures != null && eyeCryTextures != null && eyeNormalTextures.Length + eyeCryTextures.Length > 0)
            {
                int textureRandomIdx = Random.Range(0, eyeNormalTextures.Length + eyeCryTextures.Length);
                Texture2D texture = null;

                if (textureRandomIdx < eyeNormalTextures.Length)
                {
                    color = textureRandomIdx;
                    texture = eyeNormalTextures[textureRandomIdx];
                }
                else
                {
                    color = textureRandomIdx - eyeNormalTextures.Length;
                    texture = eyeCryTextures[textureRandomIdx - eyeNormalTextures.Length];
                }

                if (texture != null)
                {
                    MeshRenderer mr;
                    SkinnedMeshRenderer smr;
                    GameObject[] eyes = { leftEye, rightEye };
                    foreach (GameObject eye in eyes)
                    {
                        if (eye != null)
                        {
                            mr = eye.GetComponentInChildren<MeshRenderer>();
                            if (mr != null)
                            {
                                Material material = mr.material;
                                material.mainTexture = texture;
                                mr.material = material;
                            }
                            else
                            {
                                smr = eye.GetComponentInChildren<SkinnedMeshRenderer>();
                                if (smr != null)
                                {
                                    Material material = smr.material;
                                    material.mainTexture = texture;
                                    smr.material = material;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}