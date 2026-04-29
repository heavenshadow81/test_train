using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DeviceTracker : MonoBehaviour
{
    private const string BASE_URL = "https://smartservice.co.kr/mystory"; // 서버 기본 URL

    /// <summary>
    /// 디바이스 정보를 서버에 등록합니다.
    /// </summary>
    /// <param name="companyName">영업소 이름</param>
    /// <param name="callback">성공/실패 콜백</param>
    public IEnumerator RegisterDevice(System.Action<bool, string> callback = null)
    {
        // JSON 데이터 준비
        var jsonData = new
        {
            device_id = SystemInfo.deviceUniqueIdentifier, // 기기 고유 ID
            device_model = SystemInfo.deviceModel,         // 기기 모델
            os_version = SystemInfo.operatingSystem,       // 운영체제 버전
        };

        string json = JsonUtility.ToJson(jsonData);

        // UnityWebRequest 설정
        using (UnityWebRequest request = new UnityWebRequest($"{BASE_URL}/register-device", "POST"))
        {
            // JSON 데이터를 UTF-8로 변환하여 업로드 핸들러에 설정
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();

            // Content-Type 헤더를 JSON으로 설정
            request.SetRequestHeader("Content-Type", "application/json");

            // 요청 보내기
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("기기 등록 성공: " + request.downloadHandler.text);
                callback?.Invoke(true, request.downloadHandler.text); // 성공 콜백 호출
            }
            else
            {
                Debug.LogError("기기 등록 실패: " + request.error);
                callback?.Invoke(false, request.error); // 실패 콜백 호출
            }
        }

    }
}
