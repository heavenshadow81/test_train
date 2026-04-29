using GuessNumber;
using System.Security.Cryptography;
using System.Text;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] TMP_InputField usernameField;
    [SerializeField] TMP_InputField passwordField;
    [SerializeField] Toggle autoLoginToggle;
    [SerializeField] Button loginButton;
    [SerializeField] MessageUIManager messageManager;
    [SerializeField] GameObject agencyUI;
    private AuthManager authManager;

    [Header("암호화 키")]
    [SerializeField] EncryptionKeyConfig keyConfig;
    private string EncryptionKey => keyConfig.EncryptionKey;

    private void Start()
    {
        authManager = FindFirstObjectByType<AuthManager>();

        loginButton.onClick.AddListener(OnLoginButtonClicked);

        AutoLoginCheck();
    }

    private void AutoLoginCheck()
    {
        if (PlayerPrefs.HasKey("BAXLoginID") && PlayerPrefs.HasKey("BAXPassword"))
        {
            string encryptedLoginID = PlayerPrefs.GetString("BAXLoginID");
            string encryptedPassword = PlayerPrefs.GetString("BAXPassword");

            string savedLoginID = Decrypt(encryptedLoginID, EncryptionKey);
            string savedPassword = Decrypt(encryptedPassword, EncryptionKey);

            StartCoroutine(authManager.Login(savedLoginID, savedPassword, (success, message, roleName) =>
            {
                if (success)
                {
                    if (roleName == "ROLE_KINDER")
                    {
                        LoadNextSceneAsync();
                    }
                    else if (roleName == "ROLE_AGENCY")
                    {
                        agencyUI.SetActive(true);
                        gameObject.SetActive(false);
                    }
                }
                else
                {
                    messageManager.ShowMessage(message);
                }
            }));
        }
    }

    private void OnLoginButtonClicked()
    {
        string loginID = usernameField.text;
        string password = passwordField.text;

        if (string.IsNullOrEmpty(loginID))
        {
            messageManager.ShowMessage("아이디를 입력하세요.");
            return;
        }
        else if (string.IsNullOrEmpty(password))
        {
            messageManager.ShowMessage("비밀번호를 입력하세요.");
            return;
        }

        // 로그인 요청
        StartCoroutine(authManager.Login(loginID, password, (success, message, roleName) =>
        {
            if (success)
            {
                SaveLoginInfo(loginID, password);

                if (roleName == "ROLE_KINDER")
                {
                    LoadNextSceneAsync();
                }
                else if (roleName == "ROLE_AGENCY")
                {
                    agencyUI.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            else
            {
                messageManager.ShowMessage("로그인 실패: " + message);
            }
        }));
    }

    private void SaveLoginInfo(string loginID, string password)
    {
        if (autoLoginToggle.isOn)
        {
            string encryptedLoginID = Encrypt(loginID, EncryptionKey);
            string encryptedPassword = Encrypt(password, EncryptionKey);

            PlayerPrefs.SetString("BAXLoginID", encryptedLoginID);
            PlayerPrefs.SetString("BAXPassword", encryptedPassword);
        }
        else
        {
            PlayerPrefs.DeleteKey("BAXLoginID");
            PlayerPrefs.DeleteKey("BAXPassword");
        }
        PlayerPrefs.Save();
    }

    private void LoadNextSceneAsync()
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    private System.Collections.IEnumerator LoadSceneCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private string Encrypt(string plainText, string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        Array.Resize(ref keyBytes, 16); // AES-128
        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.GenerateIV(); // 랜덤 IV 생성
            byte[] iv = aes.IV;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, iv);

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            // IV와 암호화 데이터를 함께 반환
            byte[] result = new byte[iv.Length + encryptedBytes.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(encryptedBytes, 0, result, iv.Length, encryptedBytes.Length);

            return Convert.ToBase64String(result);
        }
    }

    private string Decrypt(string encryptedText, string key)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        Array.Resize(ref keyBytes, 16); // AES-128
        byte[] encryptedBytesWithIV = Convert.FromBase64String(encryptedText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;

            // IV를 추출
            byte[] iv = new byte[16];
            byte[] encryptedBytes = new byte[encryptedBytesWithIV.Length - 16];
            Buffer.BlockCopy(encryptedBytesWithIV, 0, iv, 0, 16);
            Buffer.BlockCopy(encryptedBytesWithIV, 16, encryptedBytes, 0, encryptedBytes.Length);

            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            byte[] plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
