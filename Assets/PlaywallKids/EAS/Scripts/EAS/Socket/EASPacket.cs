using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    /// <summary>
    /// EAS-version packet.
    /// </summary>
    public class EASPacket : JSONPacket
    {
        public string type
        {
            get
            {
                object val = Get("type");
                if (val == null)
                {
                    Set("type", kTypeNone);
                    val = Get("type");
                }
                return val.ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    value = kTypeNone;

                Set("type", value);
            }
        }

        public Dictionary<string, object> data
        {
            get
            {
                Dictionary<string, object> data = Get("data") as Dictionary<string, object>;
                if (data == null)
                {
                    data = new Dictionary<string, object>();
                    Set("data", data);
                }
                return data;
            }
        }

        public const string kTypeRoot = "root";
        public const string kTypeCheckConnection = "check_connection";
        public const string kTypeDisconnect = "disconnect";
        public const string kTypeMenu = "menu";
        public const string kTypeSketch = "sketch";
        public const string kTypePet = "pet";
        public const string kType3D = "3d";
        public const string kTypeKinect = "kinect";
        public const string kTypeRobot = "robot";
        public const string kTypePetMotion = "pet_motion";
        public const string kType2D = "2d";
        public const string kTypeNone = "none";

        public EASPacket()
            : base()
        {
            if (Get("type") == null)
            {
                Set("type", kTypeNone);
            }

            MakeNewField("data");
        }

        public EASPacket(string jsonStr)
            : base(jsonStr)
        {
            if (Get("type") == null)
            {
                Set("type", kTypeNone);
            }

            MakeNewField("data");
        }
    }
}