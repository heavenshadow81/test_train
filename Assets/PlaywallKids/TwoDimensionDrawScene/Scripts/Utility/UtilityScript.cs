using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class UtilityScript
    {
        private static Camera _cam;
        public static Camera NGUICamera
        {
            get
            {
                if (null == _cam)
                {
                    _cam = UICamera.currentCamera;
                    if (_cam == null)
                    {
                        if (UICamera.list[0] != null)
                        { _cam = UICamera.list[0].cachedCamera; }
                    }

                    if (_cam == null) Debug.LogError("NGUI Camera not found");
                }
                return _cam;
            }
        }


        private static float _fWidth;
        public static float width
        {
            get
            {
                if (_fWidth == 0)
                {
                    float ratioH = height / (float)Screen.height;
                    _fWidth = Screen.width * ratioH;
                }
                return _fWidth;
            }
        }


        private static float _fHeight;
        public static float height
        {
            get
            {
                if (_fHeight == 0)
                {
                    if (UIRoot.list.Count > 0)
                    {
                        _fHeight = UIRoot.list[0].activeHeight;
                    }
                    else
                    {
                        UIRoot root = null;
                        root = GameObject.FindObjectOfType<UIRoot>();
                        if (root != null)
                            _fHeight = root.activeHeight;
                    }
                }
                return _fHeight;
            }
        }

        private static float _ratio;
        public static float ratio
        {
            get
            {
                if (_ratio == 0)
                {
                    _ratio = Screen.width / Screen.height;
                }
                return _ratio;
            }
        }

        private static float _scaleRatio;
        public static float scaleRatio
        {
            get
            {
                if (_scaleRatio == 0)
                {
                    _scaleRatio = (float)UIRoot.list[0].activeHeight / (float)Screen.height;
                }
                return _scaleRatio;
            }
        }

        public static Vector3 RandomPostion(Vector2 min, Vector2 max)
        {
            if (UtilityScript.NGUICamera == null) return Vector3.zero;
            float w = UtilityScript.width;
            float h = UtilityScript.height;

            float x = UnityEngine.Random.Range(Mathf.Clamp01(min.x), Mathf.Clamp01(max.x));
            float y = UnityEngine.Random.Range(Mathf.Clamp01(min.y), Mathf.Clamp01(max.y));

            x = (x * w) - (w * 0.5f);
            y = (y * h) - (h * 0.5f);

            return new Vector3(x, y, 0);
        }

        public static Transform AddChild(Transform _parent, Transform _prefab)
        {
            _prefab.parent = _parent;
            _prefab.localPosition = Vector3.zero;
            _prefab.localRotation = Quaternion.identity;
            _prefab.localScale = Vector3.one;
            _prefab.gameObject.layer = _parent.gameObject.layer;

            return _prefab;
        }

        public static float SqrtDistance(Vector3 _p1, Vector3 _p2)
        {
            float _x = _p1.x - _p2.x;
            float _y = _p1.y - _p2.y;
            return _x * _x + _y * _y;
        }

        public static float Distance(Vector3 _p1, Vector3 _p2)
        {
            float _x = _p1.x - _p2.x;
            float _y = _p1.y - _p2.y;
            float _val = _x * _x + _y * _y;
            return Mathf.Sqrt(_val);
        }

        public static Vector3 WindowToNGUI(Vector3 _input)
        {
            _input.x = _input.x * scaleRatio - width * 0.5f;
            _input.y = _input.y * scaleRatio - height * 0.5f;
            return _input;
        }

        public static Vector2 WorldToNGUI(Camera _nguiCam, Vector3 _viewPostion)
        {
            Vector2 _nguiPosition = _nguiCam.ViewportToScreenPoint(_viewPostion);
            _nguiPosition *= UtilityScript.scaleRatio;
            _nguiPosition.x -= UtilityScript.width * 0.5f;
            _nguiPosition.y -= UtilityScript.height * 0.5f;
            return _nguiPosition;
        }

        public static void ShowLog(string szLog, bool bWarning)
        {
#if UNITY_EDITOR
            if (bWarning)
                Debug.LogWarning(szLog);
            else
                Debug.Log(szLog);
#endif
        }

        public static Vector3 GetRandomTarget(Vector3 _pos)
        {

            float x = UnityEngine.Random.Range(_pos.x - 500f, _pos.x + 500f);
            float y = UnityEngine.Random.Range(_pos.y - 70f, _pos.y + 70f);
            if (x >= UtilityScript.width / 2)
            {
                x = UtilityScript.width / 2 - 100f;
            }
            else if (x <= UtilityScript.width / 2 * -1)
            {
                x = UtilityScript.width / 2 * -1 + 100;
            }

            if (y >= UtilityScript.height / 2)
            {
                y = UtilityScript.height / 2 - 30f;
            }
            else if (y <= UtilityScript.height / 2 * -1)
            {
                y = UtilityScript.height / 2 * -1 + 30f;
            }

            return new Vector3(x, y, _pos.z);
        }

        public static byte[] GetBytes(string _str)
        {
            if (string.IsNullOrEmpty(_str)) return null;

            byte[] _copy = new byte[_str.Length * sizeof(char)];
            Buffer.BlockCopy(_str.ToCharArray(), 0, _copy, 0, _copy.Length);
            return _copy;
        }

        public static string GetString(byte[] _bytes)
        {
            if (_bytes == null || _bytes.Length == 0) return null;

            char[] _chars = new char[_bytes.Length / sizeof(char)];
            Buffer.BlockCopy(_bytes, 0, _chars, 0, _bytes.Length);
            return new string(_chars);
        }

        public static byte[] ToByteArray(string sStr)
        {
            if (sStr == null) return new byte[0];
            List<byte> byteList = new List<byte>(256);

            // get json string from this class
            string jsonString = sStr;

            //Debug.Log(sStr);

            // convert string to bytes (using UTF-8 encoding)
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);

            // json length
            byteList.AddRange(BitConverter.GetBytes(jsonBytes.Length));

            // json bytes
            byteList.AddRange(jsonBytes);

            return byteList.ToArray();
        }

        public static string ByteArryToStr(byte[] byteData, int len)
        {
            string strData = Encoding.UTF8.GetString(byteData, 4, len);
            return strData;
        }

        public static byte[] GetBytes(int _value)
        {
            byte[] _bytes = new byte[4];

            _bytes[0] = (byte)(0xff & (_value >> 24));
            _bytes[1] = (byte)(0xff & (_value >> 16));
            _bytes[2] = (byte)(0xff & (_value >> 8));
            _bytes[3] = (byte)(0xff & (_value >> 0));

            return _bytes;
        }
    }
}