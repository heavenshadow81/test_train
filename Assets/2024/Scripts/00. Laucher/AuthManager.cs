using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;

public class AuthManager : MonoBehaviour
{
    private const string BASE_URL = "https://smartservice.co.kr/mystory"; // 서버 기본 URL
    private DeviceTracker deviceTracker;
    [SerializeField] MessageUIManager messageManager;

    private void Start()
    {
        //deviceTracker = GetComponent<DeviceTracker>();
    }

    public IEnumerator Login(string loginID, string password, Action<bool, string, string> callback)
    {
        //// 디바이스 등록 작업 실행
        //if (deviceTracker != null)
        //{
        //    yield return StartCoroutine(deviceTracker.RegisterDevice((success, message) =>
        //    {
        //        if (!success)
        //        {
        //            Debug.LogWarning("디바이스 등록 실패: " + message);
        //        }
        //    }));
        //}

        // 서버가 Query String 방식으로 로그인 처리
        string url = $"{BASE_URL}/api/member/login?loginId={UnityWebRequest.EscapeURL(loginID)}&password={UnityWebRequest.EscapeURL(password)}";

        // UnityWebRequest 설정
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            // 빈 Body 설정 (POST이지만 데이터는 Query String에 전달됨)
            request.uploadHandler = new UploadHandlerRaw(new byte[0]);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 요청 보내기
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    // 서버 응답 JSON 파싱
                    var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);

                    if (response.ContainsKey("result") && response["result"].ToString() == "SUCCESS")
                    {
                        string roleName = response.ContainsKey("roleName") ? response["roleName"].ToString() : "";
                        callback(true, response["msg"].ToString(), roleName);
                    }
                    else
                    {
                        Debug.LogError("로그인 실패: " + response["msg"]);
                        messageManager.ShowMessage($"로그인 실패: {response["msg"]}");
                        callback(false, response["msg"].ToString(), null);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("서버 응답 파싱 실패: " + e.Message);
                    messageManager.ShowMessage($"서버 응답 파싱 실패: {e.Message}");
                    callback(false, "서버 응답 처리 중 오류가 발생했습니다.", null);
                }
            }
            else
            {
                Debug.LogError($"네트워크 오류: {request.error} ({request.responseCode})");
                messageManager.ShowMessage($"네트워크 오류: {request.error} ({request.responseCode})");
                callback(false, $"네트워크 오류: {request.responseCode}", null);
            }
        }
    }

    public IEnumerator Register(KinderRegistData registrationData, Action<bool, string> callback)
    {
        // JSON 직렬화
        string json = JsonConvert.SerializeObject(registrationData);
        Debug.Log("Request Body: " + json);
        Debug.Log("Request URL: " + $"{BASE_URL}/api/registerKinderByAgency");

        // UnityWebRequest 설정
        using (UnityWebRequest request = new UnityWebRequest($"{BASE_URL}/api/registerKinderByAgency", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 요청 보내기
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("서버 응답: " + request.downloadHandler.text);

                try
                {
                    // JSON 응답을 Dictionary로 파싱
                    var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);

                    // result 값 확인
                    if (response.ContainsKey("result") && response["result"].ToString() == "SUCCESS")
                    {
                        //Debug.Log("유치원 등록 성공: " + response["msg"]);
                        callback(true, response["msg"].ToString()); // 성공 콜백 호출
                    }
                    else
                    {
                        string errorMsg = response.ContainsKey("msg") ? response["msg"].ToString() : "알 수 없는 오류";
                        Debug.LogError("유치원 등록 실패: " + errorMsg);
                        messageManager.ShowMessage($"유치원 등록 실패: /n{errorMsg}");
                        callback(false, errorMsg); // 실패 콜백 호출
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("응답 처리 중 오류 발생: " + ex.Message);
                    messageManager.ShowMessage($"응답 처리 중 오류 발생: {ex.Message}");
                    callback(false, "서버 응답 처리 중 오류 발생");
                }
            }
            else
            {
                Debug.LogError($"네트워크 오류: {request.error} ({request.responseCode})");
                Debug.LogError($"서버 응답 내용: {request.downloadHandler.text}");
                messageManager.ShowMessage($"네트워크 오류: {request.error} ({request.responseCode})");
                messageManager.ShowMessage($"서버 응답 내용: {request.downloadHandler.text}");
                callback(false, "네트워크 오류 발생");
            }
        }
    }

    public IEnumerator CheckIdAvailability(string loginId, Action<bool, string> callback)
    {
        // 요청 URL 생성 (Query String 사용)
        string url = $"{BASE_URL}/api/member/checkIdDuplicated?loginId={UnityWebRequest.EscapeURL(loginId)}";

        // UnityWebRequest 설정
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            // 빈 Body 설정 (POST이지만 Query String에 데이터를 전달)
            request.uploadHandler = new UploadHandlerRaw(new byte[0]);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 요청 보내기
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    // 서버 응답 JSON 파싱
                    var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);

                    // 응답 결과 확인
                    if (response.ContainsKey("result") && response["result"].ToString() == "SUCCESS")
                    {
                        //Debug.Log("아이디 사용 가능: " + response["msg"]);
                        callback(true, response["msg"].ToString());
                    }
                    else
                    {
                        string errorMsg = response.ContainsKey("msg") ? response["msg"].ToString() : "아이디 중복 확인 실패";
                        //Debug.LogError("아이디 중복: " + errorMsg);
                        messageManager.ShowMessage($"이미 사용 중인 ID 입니다.");
                        callback(false, errorMsg);
                    }
                }
                catch (Exception e)
                {
                    //Debug.LogError("서버 응답 파싱 실패: " + e.Message);
                    messageManager.ShowMessage($"서버 응답 파싱 실패: {e.Message}");
                    callback(false, "서버 응답 처리 중 오류가 발생했습니다.");
                }
            }
            else
            {
                //Debug.LogError($"네트워크 오류: {request.error} ({request.responseCode})");
                messageManager.ShowMessage($"네트워크 오류: {request.error} ({request.responseCode})");
                callback(false, $"네트워크 오류: {request.responseCode}");
            }
        }
    }


    public IEnumerator RegisterDevice(string companyName, Action<bool, string> callback)
    {
        if (deviceTracker != null)
        {
            yield return StartCoroutine(deviceTracker.RegisterDevice(callback));
        }
    }
}
