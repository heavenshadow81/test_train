using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class CameraWork : MonoBehaviour
    {
        public Transform IntroCamPosition;
        public Transform IntroLookTarget;
        public Transform EndingCamPosition;
        public Transform EndingLookTarget;
        public Transform PodiumCamPositoin;
        public Transform PodiumLookTarget;
        public Transform _myTr;
        public float _speed;
        public float _rotspeed;

        public bool Cammoving;
        void Awake()
        {
            Cammoving = false;
            _myTr = this.transform;
        }
        void FixedUpdate()
        {
            if (Cammoving)
            {                
                if (KinectSkateManager.instance.playstate == PlayState.Ending)
                {
                    _myTr.position = Vector3.Lerp(_myTr.position, EndingCamPosition.position, _speed * Time.deltaTime);
                   // _myTr.LookAt(EndingCamPosition);
                    _myTr.rotation = Quaternion.Lerp(_myTr.rotation, Quaternion.LookRotation(EndingLookTarget.position - _myTr.position), _rotspeed * Time.deltaTime);
                }
                else// if(KinectSkateManager.instance.playstate == PlayState.Ready)
                {
                    _myTr.position = Vector3.Lerp(_myTr.position, IntroCamPosition.position, _speed * Time.deltaTime);
                 //   _myTr.LookAt(IntroLookTarget);
                    _myTr.rotation = Quaternion.Lerp(_myTr.rotation, Quaternion.LookRotation(IntroLookTarget.position - _myTr.position), _rotspeed * Time.deltaTime);
                }
                /*if (KinectSkateManager.instance.playstate == PlayState.Play)
                {
                    _myTr.position = IntroCamPosition.position;
                    _myTr.rotation = Quaternion.Lerp(_myTr.rotation, Quaternion.LookRotation(IntroLookTarget.position - _myTr.position), _rotspeed * Time.deltaTime);          
                }*/
            }            
        }
        public void SetCameraPodium()
        {
            _myTr.gameObject.SetActive(false);
         /*   Debug.Log("지금 엔딩위치로!!");
            Cammoving = false;
            _myTr.position = PodiumCamPositoin.position;
            _myTr.rotation = PodiumCamPositoin.rotation;*/
        }

        public void CameraMovingStart()
        {
            _myTr.parent = null;
            Cammoving = true;
        }
        public void CameraMoveInit()
        {
            _myTr.parent = null;
            Cammoving = false;
        }
    }
}

