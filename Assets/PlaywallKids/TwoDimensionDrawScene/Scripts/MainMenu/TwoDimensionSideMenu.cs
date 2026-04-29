using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class TwoDimensionSideMenu : MonoBehaviour
    {
        public enum EState { NONE, OPEN, OPENNING, CLOSING, CLOSE, DONE }

        [Range(1f, 10f)]
        public float speed;

        EState mState;
        public EState currentState
        {
            get
            {
                return mState;
            }
            private set
            {
                if (mState != value)
                {
                    switch (value)
                    {
                        case EState.CLOSE:
                            break;
                        case EState.CLOSING:

                            break;
                        case EState.DONE:
                            break;
                        case EState.OPEN:
                            break;
                        case EState.OPENNING:
                            gameObject.SetActive(true);
                            break;
                    }
                    mState = value;
                }
            }
        }

        Vector3 originPos;
        Vector3 targetPos;

        void Awake()
        {
            float width = gameObject.GetComponent<UIWidget>().width * gameObject.transform.localScale.x * 2f;
            originPos = new Vector3(-1 * (UtilityScript.width / 2 + width * 2), -100, 0);
            targetPos = new Vector3(-1 * UtilityScript.width / 2, -100, 0);
            if (speed == 0) speed = 1.5f;
            currentState = EState.CLOSE;
        }

        void OnEnable()
        { gameObject.transform.localPosition = originPos; }


        void Update()
        {
            Vector3 pos = Vector3.zero;
            switch (currentState)
            {
                case EState.CLOSE:
                    break;
                case EState.CLOSING:

                    pos = originPos - gameObject.transform.localPosition;
                    gameObject.transform.localPosition += pos * (Time.deltaTime * speed);
                    if ((pos.x * -1) < 1f)
                    {
                        gameObject.transform.localPosition = originPos;
                        gameObject.SetActive(false);

                        currentState = EState.CLOSE;
                    }


                    break;
                case EState.DONE:
                    break;
                case EState.OPEN:
                    break;
                case EState.OPENNING:
                    pos = targetPos - gameObject.transform.localPosition;
                    gameObject.transform.localPosition += pos * Time.deltaTime * speed;

                    if (pos.x < 1f)
                    {
                        gameObject.transform.localPosition = targetPos;
                        currentState = EState.OPEN;
                    }
                    break;
            }
        }

        public void Open()
        {
            if (currentState == EState.OPEN || currentState == EState.CLOSING) return;
            currentState = EState.OPENNING;
            /*
            if (currentState != EState.OPEN)
            {
                gameObject.SetActive(true);
                if (currentState == EState.DONE || currentState == EState.CLOSE)
                {
                    currentState = EState.OPEN;
                    StartCoroutine(OpenProcess());
                }
            }*/
        }

        public void Close()
        {
            if (currentState == EState.CLOSE || currentState == EState.OPENNING) return;
            currentState = EState.CLOSING;
            /*
            if (currentState != EState.CLOSE)
            {
                if (EState.OPENNING == currentState)
                {  }
                else if(currentState == EState.OPEN || currentState ==EState.DONE)
                {
                    currentState = EState.CLOSE;
                    StartCoroutine(CloseProcess()); 
                }
            }*/
        }

        IEnumerator OpenProcess()
        {
            currentState = EState.OPENNING;
            Vector3 pos;
            do
            {
                pos = targetPos - gameObject.transform.localPosition;
                gameObject.transform.localPosition += pos * Time.deltaTime * speed;
                yield return new WaitForEndOfFrame();
            } while (pos.x > 1f);
            gameObject.transform.localPosition = targetPos;

            currentState = EState.DONE;
        }

        IEnumerator CloseProcess()
        {
            currentState = EState.CLOSING;
            Vector3 pos;
            do
            {
                pos = originPos - gameObject.transform.localPosition;
                gameObject.transform.localPosition += pos * (Time.deltaTime * speed);
                yield return new WaitForEndOfFrame();
            } while (pos.x * -1 > 1f);
            gameObject.transform.localPosition = originPos;
            gameObject.SetActive(false);
            currentState = EState.DONE;
        }
    }
}