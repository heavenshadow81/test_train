using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InTheHand.Net.Bluetooth;
using System.Threading;
using RobotControl;

namespace ML.PlaywallKids.EAS
{
    public class EASServerKAvatarManager : MonoBehaviour
    {
        #region Public variables
        public int robotCount = 1;
        #endregion

        #region Properties
        private static EASServerKAvatarManager _sharedInstance;
        public static EASServerKAvatarManager sharedInstance
        {
            get
            {
                if (_sharedInstance == null)
                {
                    _sharedInstance = FindObjectOfType<EASServerKAvatarManager>();
                }
                return _sharedInstance;
            }
        }
        #endregion

        #region Private variables;
        private class RobotInfo
        {
            public Robot robot;
            public bool connected;
            public bool discoveryEnd;
            public bool cannotDiscovery;

            public RobotInfo()
            {
                robot = new Robot();
                cannotDiscovery = false;
                connected = false;
                discoveryEnd = false;
            }
        }

        private List<RobotInfo> _robots = new List<RobotInfo>();
        #endregion

        public void Init(System.Action<bool> handler)
        {
            Loom.Initialize();

            if (_robots.Count == 0)
            {
                for (int i = 0; i < robotCount; i++)
                {
                    RobotInfo r = new RobotInfo();
                    _robots.Add(r);
                }
            }

            for (int i = 0; i < robotCount; i++)
            {
                RobotInfo r = _robots[i];
                if (!r.connected)
                {
                    r.cannotDiscovery = false;
                    r.discoveryEnd = false;
                    new Thread(new ParameterizedThreadStart(Thread_Discovery)).Start(r);
                }
            }

            new Thread(new ParameterizedThreadStart(Thread_ConnectRobot)).Start(handler);
        }

        public void Play()
        {
            foreach (RobotInfo r in _robots)
            {
                if (r.connected)
                {
                    r.robot.BluetoothWrite(0x14, 0x01, 1, Packet.BTN_1);
                }
            }
        }

        public void Stop()
        {
            foreach (RobotInfo r in _robots)
            {
                if (r.connected)
                {
                    r.robot.BluetoothWrite(0x14, 0x01, 1, Packet.BTN_C);
                }
            }
        }

        public void OnDestroy()
        {
            foreach (RobotInfo r in _robots)
            {
                if (r.connected)
                {
                    r.robot.DisConnect();
                    r.connected = false;
                }
            }
        }

        public void Thread_Discovery(object obj)
        {
            RobotInfo info = obj as RobotInfo;
            if (info != null)
            {
                if (!info.connected)
                {
                    try
                    {
                        info.cannotDiscovery = false;
                        info.robot.DiscoveryDevices();
                        info.discoveryEnd = true;
                    }
                    catch (System.Exception e)
                    {
                        Loom.QueueOnMainThread(() =>
                        {
                            Debug.LogException(e);
                        });

                        info.cannotDiscovery = true;
                        info.discoveryEnd = false;
                    }
                }
            }
        }

        public void Thread_ConnectRobot(object obj)
        {
            System.Action<bool> handler = obj as System.Action<bool>;

            // check discovery error
            while (true)
            {
                bool discoveryEnd = true;

                for (int i = 0; i < robotCount; i++)
                {
                    RobotInfo r = _robots[i];
                    if (r.cannotDiscovery)
                    {
                        if (handler != null)
                        {
                            Loom.QueueOnMainThread(() =>
                            {
                                handler(false);
                            });

                            return;
                        }
                    }
                    else
                    {
                        discoveryEnd = discoveryEnd && r.discoveryEnd;
                    }
                }

                Thread.Sleep(250);
            }

            for (int i = 0; i < robotCount; i++)
            {
                RobotInfo r = _robots[i];

                if (!r.connected)
                {
                    try
                    {
                        for (int j = 0; j < r.robot._bluetoothDeviceInfo.Length; j++)
                        {
                            var bluetoothInfo = r.robot._bluetoothDeviceInfo[j];
                            if (bluetoothInfo.DeviceName.Contains("RB_"))
                            {
                                r.robot.Connect(j);
                                r.connected = true;
                            }
                        }

                        if (handler != null)
                        {
                            Loom.QueueOnMainThread(() =>
                            {
                                handler(true);
                            });
                        }
                    }
                    catch (System.Exception e)
                    {
                        if (handler != null)
                        {
                            Loom.QueueOnMainThread(() =>
                            {
                                handler(false);
                            });
                        }
                    }
                }
            }
        }
    }
}