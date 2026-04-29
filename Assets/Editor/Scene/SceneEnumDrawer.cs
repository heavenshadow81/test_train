using UnityEngine;
using UnityEditor;

// SceneEnum 클래스에 대한 커스텀 PropertyDrawer 정의
[CustomPropertyDrawer(typeof(SceneEnum))]
public class SceneEnumDrawer : PropertyDrawer
{
    private Vector2 scrollPosition; // 스크롤 위치를 저장할 변수

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var sceneList = Resources.Load<SceneListScriptableObject>("SceneList");

        if (sceneList == null)
        {
            EditorGUI.LabelField(position, label.text, "씬리스트 찾을 수 없음");
            return;
        }

        SerializedProperty sceneNameProperty = property.FindPropertyRelative("sceneName");

        if (sceneList.scenes.Count == 0)
        {
            EditorGUI.LabelField(position, label.text, "빌트세팅에서 씬을 찾을 수 없음");
            return;
        }

        // 인스펙터 레이아웃 시작
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField(label.text);

        // Scroll View 시작
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150)); // 스크롤뷰 높이 설정

        int selectedIndex = Mathf.Max(0, sceneList.scenes.IndexOf(sceneNameProperty.stringValue));
        for (int i = 0; i < sceneList.scenes.Count; i++)
        {
            // 선택된 항목을 시각적으로 구분하기 위해 배경색을 변경
            if (i == selectedIndex)
            {
                GUI.backgroundColor = Color.magenta; // 선택된 항목의 배경색 설정
            }
            else
            {
                GUI.backgroundColor = Color.white; // 선택되지 않은 항목의 배경색 설정
            }

            if (GUILayout.Button(sceneList.scenes[i], GUILayout.ExpandWidth(true)))
            {
                selectedIndex = i;
                sceneNameProperty.stringValue = sceneList.scenes[selectedIndex];
            }
        }

        // 배경색을 기본값으로 되돌림
        GUI.backgroundColor = Color.white;

        // Scroll View 종료
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}
