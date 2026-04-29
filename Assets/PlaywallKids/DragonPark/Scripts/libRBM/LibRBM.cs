using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibRBM {
    /// <summary>
    /// ***********************************************************************************************************
    /// [Class for rbm File ] 
    ///   - 
    /// ***********************************************************************************************************
    /// </summary>
    public class RBMHeader {
        public byte VersionInfo { set; get; } // 버전정보	    1	BYTE	모션파일 버전(=2)	모션파일 구조 변경의 여지
        public UInt16 SNLength { set; get; } // SN길이	        2	WORD	SN길이	
        public string SNCode { set; get; } // SN코드        	SN길이	STRING	Serial Number	저작권 보호의 목적
        public string Producer { set; get; } // 만든이	        10	STRING	만든 사람 이름 또는 ID	저작권 보호의 목적
        public string ProducerAddress { set; get; } // 연락처	20	STRING	만든 사람의 이메일, 주소, 전화번호 등등의 연락처	저작권 보호의 목적
        public UInt16 MotionNameLength { set; get; } // 모션이름길이	2	WORD	모션이름길이	
        public string MotionName { set; get; } // 모션이름	    모션이름길이	STRING	모션이름(모션빌더에서  255글자 이내로 제한)	모션의 이해를 돕기 위함
        public UInt32 MotionAbsoluteIndex { set; get; } // Reserved	    4	DWORD		
        public byte PlatForm { set; get; } // 플랫폼	        1	BYTE	"1:Creator Huno, 2:Creator Dino, 3:Creator Dogy, 4:Creator 비표준,…(플랫폼 리스트 참조)"	표준 플랫폼은 모션 파일의 호환성을 높이기 위한 것으로서 로봇의 조립 상태를 참고하기 위한 값이다
        public UInt32 SceneCount { set; get; } // 프레임수	    4	DWORD	프레임(Frame) 개수	
        public string Reserved1 { set; get; } // Reserved	    2	BYTE		
        public UInt16 wCKCount { set; get; } // wCK(SAM) 수	2	WORD	wCK(SAM) 수	
        public string Reserved2 { set; get; } // Reserved	    2	BYTE		
        public List<RBMwCKSetting> wCKSettingList { set; get; }

        public RBMHeader()
        {
            wCKSettingList = new List<RBMwCKSetting>();
        }

        public bool ParseRBM(ref string[] tokens, ref int count)
        {
            try
            {
                VersionInfo = byte.Parse(tokens[count++]);
                SNLength = UInt16.Parse(tokens[count++]);
                SNCode = tokens[count++];
                Producer = tokens[count++];
                ProducerAddress = tokens[count++];
                MotionNameLength = UInt16.Parse(tokens[count++]);
                MotionName = tokens[count++];
                MotionAbsoluteIndex = UInt32.Parse(tokens[count++]);
                PlatForm = byte.Parse(tokens[count++]);
                SceneCount = UInt32.Parse(tokens[count++]);
                Reserved1 = tokens[count++];
                wCKCount = UInt16.Parse(tokens[count++]);
                Reserved2 = tokens[count++];

                for (int i = 0; i < wCKCount; i++)
                {
                    RBMwCKSetting wCKSetting = new RBMwCKSetting();
                    if (!wCKSetting.ParseRBM(ref tokens, ref count)) return false;
                    wCKSettingList.Add(wCKSetting);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public string GetReportString()
        {
            string report = "";
            report += string.Format("	byte	VersionInfo	 : {0}\r\n", VersionInfo);
            report += string.Format("	UInt16	SNLength	 : {0}\r\n", SNLength);
            report += string.Format("	string	SNCode	 : {0}\r\n", SNCode);
            report += string.Format("	string	Producer	 : {0}\r\n", Producer);
            report += string.Format("	string	ProducerAddress	 : {0}\r\n", ProducerAddress);
            report += string.Format("	UInt16	MotionNameLength	 : {0}\r\n", MotionNameLength);
            report += string.Format("	string	MotionName	 : {0}\r\n", MotionName);
            report += string.Format("	UInt16	MotionAbsoluteIndex	 : {0}\r\n", MotionAbsoluteIndex);
            report += string.Format("	byte	PlatForm	 : {0}\r\n", PlatForm);
            report += string.Format("	UInt32	FrameCount	 : {0}\r\n", SceneCount);
            report += string.Format("	UInt16	Reserved1	 : {0}\r\n", Reserved1);
            report += string.Format("	UInt16	wCKCount	 : {0}\r\n", wCKCount);
            report += string.Format("	UInt32	Reserved2	 : {0}\r\n", Reserved2);

            for (int i = 0; i < wCKCount; i++)
            {
                report += wCKSettingList[i].GetReportString();
            }
            return report;
        }
    }

    public class RBMwCKSetting {
        public byte wCKID { set; get; }                     //		wCK(SAM)의 ID   1	BYTE	wCK의 ID	
        public byte TPGain { set; get; }                    //		TPGAIN	        1	BYTE	임시 P이득	
        public byte TDGain { set; get; }                    //		TDGAIN	        1	BYTE	임시 D이득	
        public byte TIGain { set; get; }                    //		TIGAIN	        1	BYTE	임시 I이득	
        public byte PortMode { set; get; }                  //		PORT MODE	    1	BYTE	확장포트 사용유무(0:사용안함, 1:사용함)	
        public byte FileZeroPos { set; get; }               //		FILE ZERO POS	1   BYTE	모션파일을 만들 때 사용된 로봇의 영점 위치정보

        public bool ParseRBM(ref string[] tokens, ref int count)
        {
            try
            {
                wCKID = byte.Parse(tokens[count++]);
                TPGain = byte.Parse(tokens[count++]);
                TDGain = byte.Parse(tokens[count++]);
                TIGain = byte.Parse(tokens[count++]);
                PortMode = byte.Parse(tokens[count++]);
                FileZeroPos = byte.Parse(tokens[count++]);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public string GetReportString()
        {
            string report = "";
            report += string.Format("	byte	wCKID	 : {0}\r\n", wCKID);
            report += string.Format("	byte	TPGain	 : {0}\r\n", TPGain);
            report += string.Format("	byte	TDGain	 : {0}\r\n", TDGain);
            report += string.Format("	byte	TIGain	 : {0}\r\n", TIGain);
            report += string.Format("	byte	PortMode	 : {0}\r\n", PortMode);
            report += string.Format("	byte	FileZeroPos	 : {0}\r\n", FileZeroPos);

            return report;
        }
    }

    public class RBMScene {
        public UInt16 SceneIndex { set; get; }              //		씬인덱스(0~65535)	2	WORD	프레임 인덱스(0~65535)	
        public UInt16 SceneNameLength { set; get; }         //		씬 이름길이	2	WORD	프레임 이름길이	
        public string SceneName { set; get; }               //		씬 이름	    프레임 이름 길이	STRING	프레임 이름(모션빌더에서 255글자 이내로 제한)	프레임 내용 설명
        public UInt16 FrameCount { set; get; }              //      프레임수
        public UInt16 SceneTime { set; get; }               //      씬 실행시간 
        public List<RBMwCKInfo> wCKInfoList { set; get; }

        public RBMScene()
        {
            wCKInfoList = new List<RBMwCKInfo>();
        }

        public bool ParseRBM(ref string[] tokens, ref int count, int wCKCount)
        {
            try
            {
                SceneIndex = UInt16.Parse(tokens[count++]);
                SceneNameLength = UInt16.Parse(tokens[count++]);
                SceneName = tokens[count++];
                FrameCount = UInt16.Parse(tokens[count++]);
                SceneTime = UInt16.Parse(tokens[count++]);

                for (int i = 0; i < wCKCount; i++)
                {
                    RBMwCKInfo wCKinfo = new RBMwCKInfo();
                    if (!wCKinfo.ParseRBM(ref tokens, ref count)) return false;
                    wCKInfoList.Add(wCKinfo);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public string GetReportString()
        {
            string report = "";
            report += string.Format("	UInt16	SceneIndex	 : {0}\r\n", SceneIndex);
            report += string.Format("	UInt16	SceneNameLength	 : {0}\r\n", SceneNameLength);
            report += string.Format("	string	SceneName	 : {0}\r\n", SceneName);
            report += string.Format("	UInt16 FrameCount	 : {0}\r\n", FrameCount);
            report += string.Format("	UInt16 SceneTime	 : {0}\r\n", SceneTime);

            foreach (RBMwCKInfo wCKinfo in wCKInfoList)
            {
                report += wCKinfo.GetReportString();
            }
            return report;
        }
    }

    public class RBMwCKInfo {
        public string Reserved { set; get; }                  //		Reserved	    1	BYTE	
        public byte wCKID { set; get; }                     //		wCK(SAM)의 ID	1	BYTE	wCK(SAM)의 ID
        public byte SPOS { set; get; }                      //		SPOS	        1	BYTE
        public byte DPOS { set; get; }                      //		DPOS	        1	BYTE
        public byte Torque { set; get; }                    //		TORQUE	        1	BYTE	토크
        public byte PortData { set; get; }                  //		PORT DATA	    1	BYTE	확장포트 출력값


        public bool ParseRBM(ref string[] tokens, ref int count)
        {
            try
            {
                Reserved = tokens[count++];
                wCKID = byte.Parse(tokens[count++]);
                SPOS = byte.Parse(tokens[count++]);
                DPOS = byte.Parse(tokens[count++]);
                Torque = byte.Parse(tokens[count++]);
                PortData = byte.Parse(tokens[count++]);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public string GetReportString()
        {
            string report = "";
            report += string.Format("	string Reserved	 : {0}\r\n", Reserved);
            report += string.Format("	byte	wCKID	 : {0}\r\n", wCKID);
            report += string.Format("	byte	SPOS	 : {0}\r\n", SPOS);
            report += string.Format("	byte	DPOS	 : {0}\r\n", DPOS);
            report += string.Format("	byte	Torque	 : {0}\r\n", Torque);
            report += string.Format("	byte	PortData	 : {0}\r\n", PortData);

            return report;
        }
    }

    public class RBMFile {
        public RBMHeader Header { set; get; }
        public List<RBMScene> Frames { set; get; }

        public RBMFile()
        {
            Header = new RBMHeader();
            Frames = new List<RBMScene>();
        }

        public bool ParseRBM(string RBMString)
        {
            string[] tokens = RBMString.Split(':');

            int count = 0;
            bool bResult = Header.ParseRBM(ref tokens, ref count);
            if (!bResult) return false;

            //for (int i = 0; i < Header.wCKCount; i++)
            //LWY Debugging..
            for (int i = 0; i < Header.SceneCount; i++)
            {
                RBMScene frame = new RBMScene();
                bResult = frame.ParseRBM(ref tokens, ref count, Header.wCKCount);
                if (!bResult) return false;
                Frames.Add(frame);
            }

            return true;
        }

        public string GetReportString()
        {
            string report = "";
            report += Header.GetReportString();
            foreach (RBMScene frame in Frames)
            {
                report += frame.GetReportString();
            }

            return report;
        }
    }
}