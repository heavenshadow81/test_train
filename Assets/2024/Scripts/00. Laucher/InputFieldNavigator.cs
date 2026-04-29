using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class InputFieldNavigator : MonoBehaviour
{
    [SerializeField] private TMP_InputField[] inputFields;
    private int currentFieldIndex = -1;

    private void Start()
    {
        // 각 InputField에 onSelect 이벤트 추가
        for (int i = 0; i < inputFields.Length; i++)
        {
            int index = i; // 로컬 변수로 복사하여 람다 캡처 이슈 방지
            inputFields[i].onSelect.AddListener((string text) => OnInputFieldSelected(index));
        }
    }

    private void Update()
    {
        // Tab 키나 Enter 키 입력 체크
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Return))
        {
            MoveToNextInputField();
        }
    }

    private void OnInputFieldSelected(int index)
    {
        currentFieldIndex = index; // 현재 선택된 필드 인덱스 업데이트
    }

    private void MoveToNextInputField()
    {
        // 현재 입력 필드에서 포커스를 해제
        if(currentFieldIndex != -1) inputFields[currentFieldIndex].DeactivateInputField();

        // 마지막 필드인지 확인
        if (currentFieldIndex == inputFields.Length - 1)
        {
            // 전체 선택 해제
            EventSystem.current.SetSelectedGameObject(null);
            currentFieldIndex = -1; // -1로 초기화하여 다음 호출에서 처음으로 이동
            return;
        }

        // 다음 필드 인덱스로 이동
        currentFieldIndex++;
        EventSystem.current.SetSelectedGameObject(inputFields[currentFieldIndex].gameObject);
        inputFields[currentFieldIndex].ActivateInputField();
    }
}
