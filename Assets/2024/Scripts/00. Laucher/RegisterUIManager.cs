using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField confirmPasswordField;
    [SerializeField] private TMP_InputField companyNameField;
    [SerializeField] private TMP_InputField phoneNumberField;
    [SerializeField] private TMP_InputField adressField;
    [SerializeField] private Button registerButton;

    [Header("뒤로가기")]
    [SerializeField] Button backButton;
    [SerializeField] GameObject agencyUI;

    [Header("ID중복확인")]
    [SerializeField] private Button checkIdButton;

    private AuthManager authManager;
    [SerializeField] MessageUIManager messageManager;

    private void Start()
    {
        authManager = FindFirstObjectByType<AuthManager>();

        registerButton.onClick.AddListener(OnRegisterButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        checkIdButton.onClick.AddListener(OnCheckIdButtonClicked);
    }

    private void OnBackButtonClicked()
    {
        agencyUI.SetActive(true);
        gameObject.SetActive(false);
    }

    private void OnRegisterButtonClicked()
    {
        string loginId = usernameField.text.Trim();
        string password = passwordField.text.Trim();
        string confirmPassword = confirmPasswordField.text.Trim();
        string telNo = phoneNumberField.text.Trim();
        string kinderName = companyNameField.text.Trim();
        string address = adressField.text.Trim();
        string macAddress = GetMacAddress();

        // 필수 입력 필드 확인
        if (string.IsNullOrEmpty(loginId) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) ||
            string.IsNullOrEmpty(telNo) || string.IsNullOrEmpty(kinderName))
        {
            messageManager.ShowMessage("필수입력 항목을 입력하세요.");
            return;
        }

        //// 비밀번호 유효성 검사
        //if (!IsPasswordValid(password, out string validationMessage))
        //{
        //    errorMessage.text = validationMessage;
        //    return;
        //}

        // 비밀번호 확인
        if (password != confirmPassword)
        {
            messageManager.ShowMessage("비밀번호가 일치하지 않습니다.");
            return;
        }

        // KinderRegData 객체 생성
        KinderRegistData registrationData = new KinderRegistData
        {
            LoginId = loginId,
            Password = password,
            KinderName = kinderName,
            KinderTel = telNo,
            KinderUniqueNo = "666",
            MacAddress = macAddress,
            KinderAddress = address,
            ActivateContentsGrpList = new List<ActivateContentsGrp>
            {
                 new ActivateContentsGrp
                 {
                     ContentGrpId = "",
                     LicenseEndDate = ""
                 }
            },
            AgencyId = "lmj"
        };

        // 회원가입 요청
        StartCoroutine(authManager.Register(registrationData, (success, message) =>
        {
            if (success)
            {
                //Debug.Log("유치원 등록 성공: " + message);
                messageManager.ShowMessage($"유치원 등록 성공.");
            }
            else
            {
                //Debug.LogError("유치원 등록 실패: " + message);
                messageManager.ShowMessage($"유치원 등록 실패: {message}");
            }
        }));
    }

    private void OnCheckIdButtonClicked()
    {
        string loginId = usernameField.text.Trim();

        if (string.IsNullOrEmpty(loginId))
        {
            messageManager.ShowMessage("아이디를 입력하세요.");
            return;
        }

        // 아이디 중복 확인 요청
        StartCoroutine(authManager.CheckIdAvailability(loginId, (isAvailable, message) =>
        {
            if (isAvailable)
            {
                messageManager.ShowMessage("사용 가능한 아이디입니다.");
            }
            else
            {
                messageManager.ShowMessage("사용할 수 없는 아이디입니다.");
            }
        }));
    }

    /// <summary>
    /// 비밀번호 유효성을 검사합니다.
    /// 최소 8자 이상이며, 대문자, 소문자, 숫자, 특수문자를 포함해야 합니다.
    /// </summary>
    /// <param name="password">검사할 비밀번호</param>
    /// <param name="errorMessage">유효하지 않을 경우 실패 이유를 반환</param>
    /// <returns>유효하면 true, 그렇지 않으면 false</returns>
    private bool IsPasswordValid(string password, out string errorMessage)
    {
        const string specialCharacters = "!@#$%^&*()-_=+[]{}|;:'\",.<>?/`~\\";

        if (password.Length < 8)
        {
            errorMessage = "비밀번호는 최소 8자 이상이어야 합니다.";
            return false;
        }

        bool hasUpperCase = password.Any(char.IsUpper);
        bool hasLowerCase = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecialCharacter = password.Any(c => specialCharacters.Contains(c));

        if (!hasUpperCase)
        {
            errorMessage = "비밀번호에 대문자가 포함되어야 합니다.";
            return false;
        }
        if (!hasLowerCase)
        {
            errorMessage = "비밀번호에 소문자가 포함되어야 합니다.";
            return false;
        }
        if (!hasDigit)
        {
            errorMessage = "비밀번호에 숫자가 포함되어야 합니다.";
            return false;
        }
        if (!hasSpecialCharacter)
        {
            errorMessage = "비밀번호에 특수문자가 포함되어야 합니다.";
            return false;
        }

        errorMessage = null; // 성공 시 에러 메시지가 필요 없음
        return true;
    }

    public string GetMacAddress()
    {
#if UNITY_ANDROID
        // 안드로이드 네이티브 방식으로 MAC 주소 가져오기
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using (var wifiManager = context.Call<AndroidJavaObject>("getSystemService", "wifi"))
                    {
                        using (var wifiInfo = wifiManager.Call<AndroidJavaObject>("getConnectionInfo"))
                        {
                            string macAddress = wifiInfo.Call<string>("getMacAddress");
                            return macAddress ?? "No MAC Address Found";
                        }
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error fetching MAC Address (Android): {ex.Message}");
            return "Error fetching MAC Address";
        }
#else
        try
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var adapter in networkInterfaces)
            {
                if (adapter.OperationalStatus == OperationalStatus.Up &&
                    (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                     adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                {
                    var macAddress = adapter.GetPhysicalAddress();
                    if (macAddress != null && macAddress.GetAddressBytes().Length > 0)
                    {
                        return string.Join("-", macAddress.GetAddressBytes().Select(b => b.ToString("X2")));
                    }
                }
            }

            return "No MAC Address Found";
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error fetching MAC Address (Non-Android): {ex.Message}");
            return "Error fetching MAC Address";
        }
#endif
    }
}
