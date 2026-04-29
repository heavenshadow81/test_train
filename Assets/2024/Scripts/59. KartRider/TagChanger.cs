using UnityEngine;

public class TagChanger : MonoBehaviour
{
    [SerializeField] private string newTag = "Track"; // 변경할 태그 이름

    private void Start()
    {
        // 현재 오브젝트와 모든 자식의 태그 변경
        ChangeTagRecursively(gameObject, newTag);
    }

    private void ChangeTagRecursively(GameObject parent, string tag)
    {
        // 부모의 태그 변경
        parent.tag = tag;

        // 모든 자식 순회하며 태그 변경
        foreach (Transform child in parent.transform)
        {
            ChangeTagRecursively(child.gameObject, tag);
        }
    }
}
