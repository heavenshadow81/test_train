using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public class FontAssetReplacer : EditorWindow
{
    [SerializeField] private TMP_FontAsset targetFontAsset; // 변경 대상 폰트 에셋
    [SerializeField] private TMP_FontAsset replacementFontAsset; // 대체 폰트 에셋

    [MenuItem("Tools/TextMeshPro Font Replacer (All)")]
    public static void ShowWindow()
    {
        GetWindow<FontAssetReplacer>("Font Replacer (All)");
    }

    private void OnGUI()
    {
        GUILayout.Label("Replace TextMeshPro Fonts in Project", EditorStyles.boldLabel);
        targetFontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField("Target Font Asset", targetFontAsset, typeof(TMP_FontAsset), false);
        replacementFontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField("Replacement Font Asset", replacementFontAsset, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Replace Fonts in Project"))
        {
            if (targetFontAsset == null || replacementFontAsset == null)
            {
                Debug.LogError("Please assign both the Target Font Asset and the Replacement Font Asset.");
                return;
            }

            ReplaceFontsInProject();
        }
    }

    private void ReplaceFontsInProject()
    {
        int replacedCount = 0;

        // Replace in all scenes
        //replacedCount += ReplaceFontsInBuildScenes();

        // Replace in all prefabs
        //replacedCount += ReplaceFontsInAllPrefabs();

        ReplaceFontsInScene();

        Debug.Log($"Replaced {replacedCount} instances of '{targetFontAsset.name}' with '{replacementFontAsset.name}' across the project.");
    }

    private int ReplaceFontsInAllScenes()
    {
        int replacedCount = 0;
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene"); // 모든 씬 검색

        // 현재 열려 있는 씬 저장
        string currentScenePath = EditorSceneManager.GetActiveScene().path;

        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            var scene = EditorSceneManager.OpenScene(scenePath);

            List<TextMeshProUGUI> textObjects = new List<TextMeshProUGUI>();
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                textObjects.AddRange(root.GetComponentsInChildren<TextMeshProUGUI>(true));
            }

            bool sceneModified = false;
            foreach (var textObj in textObjects)
            {
                if (textObj.font == targetFontAsset)
                {
                    Undo.RecordObject(textObj, "Replace Font Asset");
                    textObj.font = replacementFontAsset;
                    replacedCount++;
                    sceneModified = true;
                }
            }

            if (sceneModified)
            {
                EditorSceneManager.MarkSceneDirty(scene); // 씬에 변경 사항 표시
                EditorSceneManager.SaveScene(scene); // 씬 저장
            }
        }

        // 원래 열려 있던 씬으로 복귀
        if (!string.IsNullOrEmpty(currentScenePath))
        {
            EditorSceneManager.OpenScene(currentScenePath);
        }

        return replacedCount;
    }

    private int ReplaceFontsInBuildScenes()
    {
        int replacedCount = 0;

        // 빌드 세팅에 포함된 씬 목록 가져오기
        var buildScenes = EditorBuildSettings.scenes;

        // 현재 열려 있는 씬 저장
        string currentScenePath = EditorSceneManager.GetActiveScene().path;

        foreach (var scene in buildScenes)
        {
            if (!scene.enabled) continue; // 빌드에 포함되지 않은 씬은 건너뜀

            string scenePath = scene.path;
            var loadedScene = EditorSceneManager.OpenScene(scenePath);

            // 루트 오브젝트 확인
            var rootObjects = loadedScene.GetRootGameObjects();
            Debug.Log($"Loaded Scene: {scenePath}, Root Objects Count: {rootObjects.Length}");

            List<TextMeshProUGUI> textObjects = new List<TextMeshProUGUI>();
            foreach (GameObject root in rootObjects)
            {
                textObjects.AddRange(root.GetComponentsInChildren<TextMeshProUGUI>(true));
            }

            Debug.Log($"Found {textObjects.Count} TextMeshProUGUI objects in scene: {scenePath}");

            bool sceneModified = false;
            foreach (var textObj in textObjects)
            {
                if (textObj.font == targetFontAsset)
                {
                    Debug.Log($"Matched Font in Object: {textObj.name}");
                    Undo.RecordObject(textObj, "Replace Font Asset");
                    textObj.font = replacementFontAsset;
                    replacedCount++;
                    sceneModified = true;
                }
            }

            if (sceneModified)
            {
                Debug.Log($"Scene modified: {scenePath}");
                EditorSceneManager.MarkSceneDirty(loadedScene);
                EditorSceneManager.SaveScene(loadedScene);
            }
            else
            {
                Debug.Log($"No changes made to scene: {scenePath}");
            }
        }

        // 원래 열려 있던 씬으로 복귀
        if (!string.IsNullOrEmpty(currentScenePath))
        {
            EditorSceneManager.OpenScene(currentScenePath);
        }

        return replacedCount;
    }

    private void ReplaceFontsInScene()
    {
        int replacedCount = 0;
        var textObjects = FindObjectsOfType<TextMeshProUGUI>(true);

        foreach (var textObj in textObjects)
        {
            if (textObj.font == targetFontAsset)
            {
                Undo.RecordObject(textObj, "Replace Font Asset");
                textObj.font = replacementFontAsset;
                replacedCount++;
            }
        }

        Debug.Log($"Replaced {replacedCount} instances of '{targetFontAsset.name}' with '{replacementFontAsset.name}'.");
    }

    private int ReplaceFontsInAllPrefabs()
    {
        int replacedCount = 0;
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab"); // 모든 프리팹 검색

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null) continue;

            TextMeshProUGUI[] textObjects = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);

            bool prefabModified = false;
            foreach (var textObj in textObjects)
            {
                if (textObj.font == targetFontAsset)
                {
                    Undo.RecordObject(textObj, "Replace Font Asset");
                    textObj.font = replacementFontAsset;
                    replacedCount++;
                    prefabModified = true;
                }
            }

            if (prefabModified)
            {
                EditorUtility.SetDirty(prefab);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return replacedCount;
    }
}
