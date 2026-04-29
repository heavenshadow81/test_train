//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Windows.Kinect;

//namespace ML.MLBKids
//{
//    public class KinectTest : MonoBehaviour
//    {
//        private KinectSensor _sensor;
//        private MultiSourceFrameReader _multiReader;

//        void Start()
//        {
//            Init();
//        }

//        private void OnDestroy()
//        {
//            Cleanup();
//        }

//        public bool Init()
//        {
//            _sensor = KinectSensor.GetDefault();
//            if (_sensor == null)
//                return false;

//            _sensor.Open();
//            _multiReader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.BodyIndex);
            
//            return true;
//        }

//        public void Cleanup()
//        {
//            if(_multiReader != null)
//            {
//                _multiReader.Dispose();
//                _multiReader = null;
//            }

//            if(_sensor != null && _sensor.IsOpen)
//            {
//                _sensor.Close();
//            }
//        }
//    }
//}