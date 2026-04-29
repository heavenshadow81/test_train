using UnityEngine;

[CreateAssetMenu(fileName = "EncryptionKeyConfig", menuName = "EncryptionKeyConfig")]
public class EncryptionKeyConfig : ScriptableObject
{
    [SerializeField]
    private string encryptionKey; // 에디터에서 직접 설정

    public string EncryptionKey => encryptionKey;
}
