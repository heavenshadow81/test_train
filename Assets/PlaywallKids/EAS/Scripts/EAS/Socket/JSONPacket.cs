using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    /// <summary>
    /// base class of json-based packet class.
    /// </summary>
    public class JSONPacket
    {
        private Dictionary<string, object> _dict = new Dictionary<string, object>();

        private static char[] _pathSeparators = new char[] { '/', '\\', '.' };

        public JSONPacket()
        {
        }

        public JSONPacket(string jsonStr)
        {
            try
            {
                _dict = MiniJSON.Json.Deserialize(jsonStr) as Dictionary<string, object>;
            }
            catch (System.Exception e)
            {
                Debug.LogError("JSONPacket() : invalid json format.");
                Debug.LogException(e);

                _dict = null;
            }

            if (_dict == null)
            {
                _dict = new Dictionary<string, object>();
            }
        }

        public object Get(string path)
        {
            object value = null;

            if (!string.IsNullOrEmpty(path))
            {
                string[] tokens = path.Split(_pathSeparators, System.StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length > 0)
                {
                    Dictionary<string, object> d = _dict;
                    List<object> l = null;

                    for (int i = 0, cnt = tokens.Length; i < cnt; i++)
                    {
                        object obj = null;

                        // parse dictionary
                        if (d != null)
                        {
                            d.TryGetValue(tokens[i], out obj);
                        }
                        // parse list
                        else if (l != null)
                        {
                            string token = tokens[i];
                            int idx = 0;

                            // length of the list
                            if (token.Equals("length") || token.Equals("count") || token.Equals("size"))
                            {
                                obj = l.Count;
                            }
                            // idx
                            else if (int.TryParse(token, out idx) && idx < l.Count)
                            {
                                obj = l[idx];
                            }
                        }
                        else
                        {
                            break;
                        }

                        if (obj != null)
                        {
                            if (i + 1 == cnt)
                            {
                                value = obj;
                            }
                            else
                            {
                                d = obj as Dictionary<string, object>;
                                l = obj as List<object>;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return value;
        }

        public void Set(string path, object value)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string[] tokens = path.Split(_pathSeparators, System.StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length > 0)
                {
                    Dictionary<string, object> d = _dict;
                    List<object> l = null;
                    for (int i = 0, cnt = tokens.Length; i < cnt; i++)
                    {
                        if (d == null && l == null)
                            break;
                        if (i + 1 == cnt)
                        {
                            if (l != null)
                            {
                                int idx = 0;

                                if (int.TryParse(tokens[i], out idx) && idx < l.Count)
                                    l[idx] = value;
                            }
                            else
                                d[tokens[i]] = value;
                        }
                        else
                        {
                            object c = null;

                            if (l != null)
                            {
                                int idx = 0;

                                if (int.TryParse(tokens[i], out idx) && idx < l.Count)
                                {
                                    c = l[idx];
                                }
                            }
                            else
                            {
                                d.TryGetValue(tokens[i], out c);
                            }

                            if (c == null || (c as Dictionary<string, object> == null && c as List<object> == null))
                            {
                                Dictionary<string, object> field = new Dictionary<string, object>();
                                if (d != null)
                                {
                                    d[tokens[i]] = field;
                                    d = field;
                                    l = null;
                                }
                                else
                                {
                                    l = null;
                                    d = null;
                                }
                            }
                            else if (c as Dictionary<string, object> != null)
                            {
                                d = c as Dictionary<string, object>;
                                l = null;
                            }
                            else if (c as List<object> != null)
                            {
                                l = c as List<object>;
                                d = null;
                            }
                        }
                    }
                }
            }
        }

        public int GetInt(string path)
        {
            int value = 0;

            object obj = Get(path);
            if (obj != null)
            {
                int.TryParse(obj.ToString(), out value);
            }

            return value;
        }

        public float GetFloat(string path)
        {
            float value = 0;

            object obj = Get(path);
            if (obj != null)
            {
                float.TryParse(obj.ToString(), out value);
            }

            return value;
        }

        public bool GetBool(string path)
        {
            bool value = false;

            object obj = Get(path);
            if (obj != null)
            {
                bool.TryParse(obj.ToString(), out value);
            }

            return value;
        }

        public string GetString(string path)
        {
            string str = "";

            object obj = Get(path);
            if (obj != null)
            {
                str = obj.ToString();
            }

            return str;
        }

        public List<object> GetList(string path)
        {
            List<object> list = null;

            object obj = Get(path);
            if (obj != null)
            {
                list = obj as List<object>;
            }

            return list;
        }

        public List<T> GetList<T>(string path)
        {
            List<T> list = new List<T>();

            List<object> objList = null;
            object obj = Get(path);
            if (obj != null)
            {
                objList = obj as List<object>;
            }

            if (objList != null)
            {
                for (int i = 0; i < objList.Count; i++)
                {
                    list.Add((T)objList[i]);
                }
            }

            return list;
        }

        public void MakeNewField(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string[] tokens = path.Split(_pathSeparators, System.StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length > 0)
                {
                    Dictionary<string, object> d = _dict;
                    for (int i = 0, cnt = tokens.Length; i < cnt; i++)
                    {
                        object c = null;

                        d.TryGetValue(tokens[i], out c);

                        if (c == null)
                        {
                            c = new Dictionary<string, object>();
                            d[tokens[i]] = c;
                            d = c as Dictionary<string, object>;
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            return MiniJSON.Json.Serialize(_dict);
        }

        public virtual byte[] ToByteArray()
        {
            List<byte> byteList = new List<byte>(128);

            // get json string from this class
            string jsonString = ToString();

            // convert string to bytes (using UTF-8 encoding)
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);

            // json length
            byteList.AddRange(System.BitConverter.GetBytes(jsonBytes.Length));

            // json bytes
            byteList.AddRange(jsonBytes);

            return byteList.ToArray();
        }
    }
}