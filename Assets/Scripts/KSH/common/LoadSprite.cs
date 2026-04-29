using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

// By 김상현
public class LoadSprite 
{
    private string folderName;

    // 사용자가 작업을 진행하여 연산이 완료되면 결과를 가져올 수 있도록 하는 핸들을 반환
    private AsyncOperationHandle<Sprite> handle;   

    /// <summary>
    /// StreamingAssets 폴더의 접근할 폴더이름
    /// </summary>
    /// <param name="_folderName"></param>
    public LoadSprite(string _folderName)
    { 
        folderName = _folderName;   // 생성 시 사용할 폴더 주소 저장
    }

    // 이미지 주소로 불러오기 (UniTask : 코루틴 상위 버전 (에셋))
    public async UniTask LoadSpriteData(string spriteName , SpriteRenderer render)
    {
        // 매개변수의 이미지 불러오기
        // Completed 완료 시 실행할 함수를 담아둘 Action 변수
        Addressables.LoadAssetAsync<Sprite>($"Assets/Image/{folderName}/{spriteName}.png").Completed += 
            (obj) => 
            {
                if (obj.Status == AsyncOperationStatus.Succeeded)
                {
                    handle = obj;   // 성공 시 handle에 불러온 오브젝트(obj) 저장
                    render.sprite = obj.Result; // 불러온 결과 값(<Sprite>) (obj.Status : 열거형으로 성공했는지 실패했는지 반환.)
                }
                else
                {
                    // 실패
                    Debug.LogError("로딩 안됨");
                }
            };

        await UniTask.Yield();  // 1프레임 대기 후 반환 (yield return null;)
        #region streamingAssets 방식(지원님 쓰던 방식(PC에서만 사용 가능))

        //string  Path = $"file://{Application.streamingAssetsPath}";
        //string imagePath = $"{Path}/{folderName}/{spriteName}.png";
        //UnityWebRequest www = UnityWebRequestTexture.GetTexture(imagePath);
        //await www.SendWebRequest();

        //Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        //Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        //return newSprite;
        #endregion
    }

    // 이미지 버전
    public async UniTask LoadSpriteData(string spriteName, Image render)
    {
        Addressables. LoadAssetAsync<Sprite>($"Assets/Image/{folderName}/{spriteName}.png").Completed +=
            (obj) =>
            {
                if (obj.Status == AsyncOperationStatus.Succeeded)
                {
                    handle = obj;
                    render.sprite = obj.Result;
                }
                else
                {
                    Debug.LogError("로딩 안됨");
                }
            };

        await UniTask.Yield();
    }

    GameObject efx;
    // 어드레스불을 이용하여 바로 생성
    public async UniTask LoadEffect(AssetReference asset)
    {
        // 어드레스불 이용하여 생성 성공 시 loadSprite_Completed 함수로 이동
        Addressables.InstantiateAsync(asset).Completed += loadSprite_Completed;
    }

    //  생성한 object를 obj매개변수로 넘어와 실행
    private void loadSprite_Completed(AsyncOperationHandle<GameObject> obj)
    {
        // 성공적으로 생성 시 
        if (obj.Status == AsyncOperationStatus.Succeeded)
        { 
           efx = obj.Result;    // 안 넣어도 되는데 그냥 넣음... (by 김상현)
        }
    }

    // Disable에서 호출
    public void MemoryRerease()
    {
        //결과 존재하면 메모리 해제
        if(handle.Result != null)
            Addressables.Release(handle);

        // efx 메모리 해제
        Addressables.Release(efx);
    }
}