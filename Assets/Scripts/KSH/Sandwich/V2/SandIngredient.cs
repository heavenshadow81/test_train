using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SandIngredient : MonoBehaviour
{
    //샌드위치 부속품 타입
    public SandKind kind;

    //터치
    public async void SandDown()
    {
        if (SandWichManager.instance.isDown == false && SandWichManager.instance.IsGame)
        {
            //사운드
            SandWichManager.instance.soundMgr.Btn();
            //스폰갯수가 7이하일때만 
            if (SandWichManager.instance.Rspawner.sandCnt < 7)
            {
                SandWichManager.instance.isDown = true;
                //Sand
                await SandWichManager.instance.Rspawner.SandSetting(kind);
                SandWichManager.instance.isDown = false;
            }

            if (SandWichManager.instance.Rspawner.sandCnt >= 7)
            {
                SandWichManager.instance.IsGame = false;
                await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));

                var LeftList = SandWichManager.instance.Lspawner.LaiseSandList;
                var RightList = SandWichManager.instance.Rspawner.LaiseSandList;
                SandWichManager.instance.resultList = new List<bool>();
                for (int i = 0; i < 7; i++)
                {
                    if (LeftList[i].SnadKind == RightList[i].SnadKind)
                    {
                        SandWichManager.instance.resultList.Add(true);
                    }
                    else
                    {
                        SandWichManager.instance.resultList.Add(false);
                    }
                }

                if (SandWichManager.instance.resultList.Contains(false))
                {
                    //실패
                    Debug.Log("실패");
                    SandWichManager.instance.UnClearEfx.Play();
                    SandWichManager.instance.soundMgr.FalseSound();
                    await SandWichManager.instance.Rspawner.SandDelete();
                    SandWichManager.instance.IsGame = true;
                }
                else
                {
                    //성공
                    Debug.Log("성공");
                    SandWichManager.instance.ClearEfx.Play();
                    SandWichManager.instance.soundMgr.TrueSound();
                    SandWichManager.instance.Money += 2000;
                    SandWichManager.instance.soundMgr.Raining();
                    ActionProcess.SandDeletes?.Invoke();
                }





                //  ActionProcess.SandAllRaise?.Invoke();
            }
        }
    }
}
