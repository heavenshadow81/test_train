using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Settings;

namespace LGM
{
    namespace Drum
    {
        public class GameManager : MonoBehaviour
        {
            public EnumClass stateClass;
            public GameUI gameUI;
            public ScreenProsess screenProsess;

            public ZoZoBasePatton<GameManager> zozo;
            void Awake()
            {
                stateClass = new EnumClass();


                #region 공용 스테이트 패턴 

                ActionProcess.Enter_StateListener(null, null, null, null);

                zozo = new ZoZoBasePatton<GameManager>();
                zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
                #endregion

            }
        }
    }
}

