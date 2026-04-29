using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace LibAP3
{

    public struct AP3Info{
        public string AP3Identifier;
        public byte versionInfo;
        public UInt16 length_sec;
        public UInt32 WAVsamples;
        public UInt32 AP3BodySize;
        public UInt32 keyframeSize; //  0: No keyframe information
        public UInt16 AP3FrameNum;
        public UInt32 keyframeIdx;
    }
    public struct WAVInfo
    {
        public string ChunkID;
        public UInt32 ChunkSize;
        public string Format;
        

        public string Subchunk1ID;
        public UInt32 Subchunk1Size;

        public UInt16 AudioFormat;
        public UInt16 NumChannels;
        public UInt32 SampleRate;
        public UInt32 ByteRate;
        public UInt16 BlockAlign;
        public UInt16 BitsPerSample;

        public string Subchunk2ID;
        public UInt32 Subchunk2Size;
        
        // 44 Bytes

        public bool ID3tag; 
    }


	public class AP3
	{
        public AP3Info AP3Header;
        public AP2 AP2Info;
        public WAVInfo WAVHeader;
        public Int16[] left;
        public Int16[] right;
        private bool loadWAV;
        private bool loadMotion;
        private bool loadKeyframe;

        
        public bool openWAVFile(string fn)
        {
            
            byte[] wav = File.ReadAllBytes(fn);

            if (openWAVStream(wav))
            {
                return true;

            }
                        
            return false;
        }

        public bool openWAVStream(byte[] wavStream)
        {
            if (!openWAVHeaderStream(wavStream))
                return false;

            int pos = 44;

            if(left != null)
                left.Initialize();
            if (right != null)
                right.Initialize();
            
            // Allocate memory (right will be null if only mono sound)
            left = new Int16[AP3Header.WAVsamples];
            if (WAVHeader.NumChannels == 2) right = new Int16[AP3Header.WAVsamples];
            else right = null;

            // Write to double array/s:
            int i = 0;
            uint dataByte = 44 + WAVHeader.Subchunk2Size;
            while (pos < dataByte)
            {
                left[i] = BitConverter.ToInt16(wavStream, pos);
                pos += 2;
                if (WAVHeader.NumChannels == 2)
                {
                    right[i] = BitConverter.ToInt16(wavStream, pos);
                    pos += 2;
                }
                i++;
            }

            loadWAV = true;
            return true;
        }

        private bool openWAVHeaderStream(byte[] wavStream)
        {
            if (wavStream[0] == 'R' && wavStream[1] == 'I' && wavStream[2] == 'F' && wavStream[3] == 'F')
                WAVHeader.ChunkID = "RIFF";
            else
                return false;

            if (wavStream[8] == 'W' && wavStream[9] == 'A' && wavStream[10] == 'V' && wavStream[11] == 'E')
                WAVHeader.Format = "WAVE";
            else
                return false;

            if (wavStream[12] == 'f' && wavStream[13] == 'm' && wavStream[14] == 't' && wavStream[15] == ' ')
                WAVHeader.Subchunk1ID = "fmt ";
            else
                return false;

            if (wavStream[36] == 'd' && wavStream[37] == 'a' && wavStream[38] == 't' && wavStream[39] == 'a')
                WAVHeader.Subchunk2ID = "data";
            else
                return false;

            if (BitConverter.ToUInt32(wavStream, 16) == 16)   // Check for PCM
                WAVHeader.Subchunk1Size = 16;
            else
                return false;

            if (BitConverter.ToUInt16(wavStream, 20) == 1)   // Check for PCM
                WAVHeader.AudioFormat = 1;
            else
                return false;


            // Determine if mono or stereo
            WAVHeader.NumChannels = wavStream[22];

            // Sampling Rate
            WAVHeader.SampleRate = BitConverter.ToUInt32(wavStream, 24);

            // Bits per Sample
            WAVHeader.BitsPerSample = BitConverter.ToUInt16(wavStream, 34);

            // BlockAlign
            WAVHeader.BlockAlign = BitConverter.ToUInt16(wavStream, 32);

            // ByteRate
            WAVHeader.ByteRate = BitConverter.ToUInt32(wavStream, 28);

            // Subchunk2Size
            WAVHeader.Subchunk2Size = BitConverter.ToUInt32(wavStream, 40);

            // ChunkSize
            WAVHeader.ChunkSize = BitConverter.ToUInt32(wavStream, 4);

            int pos = 12;   // First Subchunk ID from 12 to 16

            // Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
            while (!(wavStream[pos] == 100 && wavStream[pos + 1] == 97 && wavStream[pos + 2] == 116 && wavStream[pos + 3] == 97))
            {
                pos += 4;
                WAVHeader.Subchunk1Size = BitConverter.ToUInt32(wavStream, pos);
                pos += 4 + System.Convert.ToInt32(WAVHeader.Subchunk1Size);
            }
            pos += 8;

            // Pos is now positioned to start of actual sound data.

            AP3Header.WAVsamples = WAVHeader.Subchunk2Size / WAVHeader.NumChannels / WAVHeader.BitsPerSample * 8;


            if (WAVHeader.ChunkSize == 36 + WAVHeader.Subchunk2Size)
                WAVHeader.ID3tag = false;
            else if (WAVHeader.ChunkSize > 36 + WAVHeader.Subchunk2Size)
            {
                WAVHeader.ChunkSize = 36 + WAVHeader.Subchunk2Size;
                WAVHeader.ID3tag = true;
            }
            else
                return false;

            if (WAVHeader.ByteRate != WAVHeader.SampleRate * WAVHeader.NumChannels * WAVHeader.BitsPerSample / 8)
                return false;

            if (WAVHeader.BlockAlign != WAVHeader.NumChannels * WAVHeader.BitsPerSample / 8)
                return false;


            return true;
        }

        public bool saveWAVFile(string fn)
        {
            
            byte[] wav = saveWAVStream();

            System.IO.FileStream fStreamWAV = new System.IO.FileStream(fn, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            fStreamWAV.Write(wav, 0, wav.Length);
            fStreamWAV.Close();
            return true;
        }

        public byte[] saveWAVStream()
        {
            var wav = new List<byte>();

            saveWAVHeaderStream(ref wav);


            int pos = 0;
            while (pos < AP3Header.WAVsamples)
            {
                wav.AddRange(BitConverter.GetBytes(left[pos]));
                if (WAVHeader.NumChannels == 2)
                {
                    wav.AddRange(BitConverter.GetBytes(right[pos]));
                }
                pos++;
            }

            return wav.ToArray();
        }
        private void saveWAVHeaderStream(ref List<byte> wav)
        {
            wav.AddRange(Encoding.UTF8.GetBytes(WAVHeader.ChunkID));
            wav.AddRange(BitConverter.GetBytes(WAVHeader.ChunkSize));
            wav.AddRange(Encoding.UTF8.GetBytes(WAVHeader.Format));
            wav.AddRange(Encoding.UTF8.GetBytes(WAVHeader.Subchunk1ID));
            wav.AddRange(BitConverter.GetBytes(WAVHeader.Subchunk1Size));
            wav.AddRange(BitConverter.GetBytes(WAVHeader.AudioFormat));
            wav.AddRange(BitConverter.GetBytes(WAVHeader.NumChannels));
            wav.AddRange(BitConverter.GetBytes(WAVHeader.SampleRate));
            wav.AddRange(BitConverter.GetBytes(WAVHeader.ByteRate));
            wav.AddRange(BitConverter.GetBytes(WAVHeader.BlockAlign));
            wav.AddRange(BitConverter.GetBytes(WAVHeader.BitsPerSample));
            wav.AddRange(Encoding.UTF8.GetBytes(WAVHeader.Subchunk2ID));
            wav.AddRange(BitConverter.GetBytes(WAVHeader.Subchunk2Size));
        }
        
        public bool openAP2File(string fn)
        {

            AP2Info.openFile(fn);
            loadMotion = true;
            return true;

        }
        public bool saveAP2File(string fn)
        {

            if (loadMotion)
                AP2Info.saveAP2File(fn);
            else
                return false;

            return true;

        }

        public bool openMotion(Motion.Motion motionInfo)
        {
            AP2Info.openMotion(motionInfo);

            return true;
        }

        public void encodingAP3File(string fn)
        {

            System.IO.FileStream fStream = new System.IO.FileStream(fn, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            
            saveAP3Header(fStream);
            saveAP3Data(fStream);
                        
            fStream.Close();

        }

        private void saveAP3Header(System.IO.FileStream fStream)
        {
            System.IO.BinaryWriter fw = new System.IO.BinaryWriter(fStream);
            
            setAP3Header();

            UInt32 data_4;

            // AP3 Identifier (3 Bytes)
            write_string(fw, AP3Header.AP3Identifier, 3);

            // AP3 Version (1 Byte)
            write_1byte(fw, AP3Header.versionInfo);

            // 파일 길이 (2 Bytes)
            write_2byte(fw, AP3Header.length_sec);

            // WAV Sample 수 크기 (4 Bytes)
            write_4byte(fw, AP3Header.WAVsamples);

            // AP3 Body Data 크기 (4 Bytes)
            write_4byte(fw, AP3Header.AP3BodySize);

            // Key Frame Data 크기 (4 Bytes)
            write_4byte(fw, AP3Header.keyframeSize);

            // AP3 Frame 수 (2 Bytes)
            write_2byte(fw, AP3Header.AP3FrameNum);

            // KeyFrame Byte Index (4 Bytes)
            write_4byte(fw, AP3Header.keyframeIdx);

            // Reserved (4 Bytes)
            data_4 = System.Convert.ToUInt32(0);
            write_4byte(fw, data_4);

            // Reserved (4 Bytes)
            data_4 = System.Convert.ToUInt32(0);
            write_4byte(fw, data_4);


            // AP2 Header
            AP2Info.saveAP2Header(fw);

            // WAV Header
            var wav = new List<byte>();
            saveWAVHeaderStream(ref wav);
            fw.Write(wav.ToArray());
        
        }

        private void setAP3Header() {
            AP3Header.AP3Identifier = "AP3";
            AP3Header.versionInfo = 2;
            
            
            double length_sec_wav = AP3Header.WAVsamples/WAVHeader.SampleRate;
            double length_sec_motion = AP2Info.MotionData.framecount/AP2Info.MotionData.sampliing;

            if(length_sec_wav>length_sec_motion)
                AP3Header.length_sec = System.Convert.ToUInt16(Math.Ceiling(length_sec_wav));
            else
                AP3Header.length_sec = System.Convert.ToUInt16(Math.Ceiling(length_sec_motion));

            AP3Header.AP3BodySize = 0;  // 수정
            AP3Header.keyframeSize = 0; // 수정
            AP3Header.keyframeIdx = 32 + 44 + AP2Info.getHeaderBytes() + AP3Header.AP3BodySize;
            
        }

        private void saveAP3Data(System.IO.FileStream fStream)
        {
            int i;


            for (i = 0; i < AP3Header.length_sec; i++){
                saveAP3Frame(fStream, i);
            }


        }
        private void saveAP3Frame(System.IO.FileStream fStream, int idx)
        {
            System.IO.BinaryWriter fw = new System.IO.BinaryWriter(fStream);
            int i;
            int offset;
            uint temp;
            if (idx != AP3Header.length_sec - 1)
            {

                offset = idx * System.Convert.ToInt32(WAVHeader.SampleRate);
                for (i = 0; i < WAVHeader.SampleRate; i++)
                {
                    if(offset+i < AP3Header.WAVsamples)
                        fw.Write(left[offset + i]);
                    else
                        fw.Write(System.Convert.ToInt16(0));
                }
                if (WAVHeader.NumChannels == 2)
                {
                    for (i = 0; i < WAVHeader.SampleRate; i++)
                    {
                        if (offset + i < AP3Header.WAVsamples)
                            fw.Write(right[offset + i]);
                        else
                            fw.Write(System.Convert.ToInt16(0));
                    }
                }

                offset = idx * System.Convert.ToInt32(AP2Info.MotionData.sampliing);
                for (i = 0; i < AP2Info.MotionData.sampliing; i++)
                {
                    if (offset + i < AP2Info.MotionData.framecount)
                        AP2Info.saveAP2Frame(fw, offset + i);
                    else
                        AP2Info.saveAP2NullFrame(fw);

                }

            }
            else
            {
                double length_sec_wav = AP3Header.WAVsamples/WAVHeader.SampleRate;
                double length_sec_motion = AP2Info.MotionData.framecount/AP2Info.MotionData.sampliing;

                if (length_sec_wav > length_sec_motion)
                {
                    offset = idx * System.Convert.ToInt32(WAVHeader.SampleRate);
                    temp = AP3Header.WAVsamples % WAVHeader.SampleRate;

                    for (i = 0; i < WAVHeader.SampleRate; i++)
                    {
                        if (i < temp)
                            fw.Write(left[offset + i]);
                        else
                            fw.Write(System.Convert.ToInt16(0));
                    }
                    if (WAVHeader.NumChannels == 2)
                    {
                        for (i = 0; i < WAVHeader.SampleRate; i++)
                        {
                            if (i < temp)
                                fw.Write(right[offset + i]);
                            else
                                fw.Write(System.Convert.ToInt16(0));
                        }
                    }

                    offset = idx * System.Convert.ToInt32(AP2Info.MotionData.sampliing);
                    for (i = 0; i < AP2Info.MotionData.sampliing; i++)
                    {
                        if (offset + i < AP2Info.MotionData.framecount)
                            AP2Info.saveAP2Frame(fw, offset + i);
                        else
                            AP2Info.saveAP2NullFrame(fw);

                    }

                }
                else
                {
                    offset = idx * System.Convert.ToInt32(WAVHeader.SampleRate);
                    for (i = 0; i < WAVHeader.SampleRate; i++)
                    {
                        if (offset + i < AP3Header.WAVsamples)
                            fw.Write(left[offset + i]);
                        else
                            fw.Write(System.Convert.ToInt16(0));
                    }
                    if (WAVHeader.NumChannels == 2)
                    {
                        for (i = 0; i < WAVHeader.SampleRate; i++)
                        {
                            if (offset + i < AP3Header.WAVsamples)
                                fw.Write(right[offset + i]);
                            else
                                fw.Write(System.Convert.ToInt16(0));
                        }
                    }


                    offset = idx * System.Convert.ToInt32(AP2Info.MotionData.sampliing);
                    temp = AP2Info.MotionData.framecount % AP2Info.MotionData.sampliing;
                    for (i = 0; i < AP2Info.MotionData.sampliing; i++)
                    {
                        if (offset + i < temp)
                            AP2Info.saveAP2Frame(fw, offset + i);
                        else
                            AP2Info.saveAP2NullFrame(fw);

                    }

                }
                    


            }
            
        }

        public void openAP3File(string fn)
        {

            System.IO.FileStream fStream = new System.IO.FileStream(fn, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            loadAP3Header(fStream);
            loadAP3Data(fStream);

            fStream.Close();

            loadMotion = true;
            loadWAV = true;


        }

        private void loadAP3Header(System.IO.FileStream fStream)
        {
            System.IO.BinaryReader fr = new System.IO.BinaryReader(fStream);

            UInt32 data_4 = new UInt32();

            // AP3 Identifier (3 Bytes)
            read_string(fr, ref AP3Header.AP3Identifier, 3);
            
            // AP3 Version (1 Byte)
            read_1byte(fr, ref AP3Header.versionInfo);

            // 파일 길이 (2 Bytes)
            read_2byte(fr, ref AP3Header.length_sec);

            // WAV Sample 수 크기 (4 Bytes)
            read_4byte(fr, ref AP3Header.WAVsamples);

            // AP3 Body Data 크기 (4 Bytes)
            read_4byte(fr, ref AP3Header.AP3BodySize);

            // Key Frame Data 크기 (4 Bytes)
            read_4byte(fr, ref AP3Header.keyframeSize);

            // AP3 Frame 수 (2 Bytes)
            read_2byte(fr, ref AP3Header.AP3FrameNum);

            // KeyFrame Byte Index (4 Bytes)
            read_4byte(fr, ref AP3Header.keyframeIdx);

            // Reserved (4 Bytes)
            read_4byte(fr, ref data_4);

            // Reserved (4 Bytes)
            read_4byte(fr, ref data_4);


            // AP2 Header
            AP2Info.loadAP2Header(fr);

            // WAV Header
            byte[] wav = new byte[44];

            fr.Read(wav,0,44);

            openWAVHeaderStream(wav);
            
        }

        private void loadAP3Data(System.IO.FileStream fStream)
        {
            int i;


            if (left != null)
                left.Initialize();
            if (right != null)
                right.Initialize();

            // Allocate memory (right will be null if only mono sound)
            left = new Int16[AP3Header.WAVsamples];
            if (WAVHeader.NumChannels == 2) right = new Int16[AP3Header.WAVsamples];
            else right = null;



            for (i = 0; i < AP3Header.length_sec; i++)
            {
                loadAP3Frame(fStream, i);
            }


        }
        private void loadAP3Frame(System.IO.FileStream fStream, int idx)
        {
            System.IO.BinaryReader fr = new System.IO.BinaryReader(fStream);
            int i;
            int offset;
            Int16 temp;

            offset = idx * System.Convert.ToInt32(WAVHeader.SampleRate);
            for (i = 0; i < WAVHeader.SampleRate; i++)
            {
                if (offset + i < AP3Header.WAVsamples)
                    left[offset+i] = fr.ReadInt16();
                else
                    temp = fr.ReadInt16();
            }
            if (WAVHeader.NumChannels == 2)
            {
                for (i = 0; i < WAVHeader.SampleRate; i++)
                {
                    if (offset + i < AP3Header.WAVsamples)
                        right[offset+i] = fr.ReadInt16();
                    else
                        temp = fr.ReadInt16();
                }
            }

            offset = idx * System.Convert.ToInt32(AP2Info.MotionData.sampliing);
            for (i = 0; i < AP2Info.MotionData.sampliing; i++)
            {
                if (offset + i < AP2Info.MotionData.framecount)
                {
                    AP2Info.MotionData.frame.Add(new Motion.Frame(AP2Info.MotionData.motorcount));
                    AP2Info.loadAP2Frame(fr, offset + i);

                }
                else
                    AP2Info.loadAP2NullFrame(fr);

            }

         
        }

        
        private void write_string(System.IO.BinaryWriter fw, string data, int length)
        {
            int temp;

            byte[] buffer = null;
            string data_string = null;
            data_string = data;
            temp = data_string.Length;
            if (temp < length)
            {
                for (int i = 0; i < length - temp; i++)
                    data_string += " ";
            }
            buffer = Encoding.UTF8.GetBytes(data_string);

            fw.Write(buffer, 0, length);

        }

        private void write_1byte(System.IO.BinaryWriter fw, byte data)
        {
            fw.Write(data);

        }
        private void write_2byte(System.IO.BinaryWriter fw, UInt16 data)
        {
            fw.Write(data);

        }
        private void write_4byte(System.IO.BinaryWriter fw, UInt32 data)
        {
            fw.Write(data);

        }

        private void write_double(System.IO.BinaryWriter fw, double data)
        {
            fw.Write(data);
        }


        private void read_1byte(System.IO.BinaryReader fr, ref byte data)
        {
            data = fr.ReadByte();

        }
        private void read_2byte(System.IO.BinaryReader fr, ref UInt16 data)
        {
            data = fr.ReadUInt16();

        }
        private void read_4byte(System.IO.BinaryReader fr, ref UInt32 data)
        {
            data = fr.ReadUInt32();

        }
        private void read_double(System.IO.BinaryReader fr, ref double data)
        {
            data = fr.ReadDouble();
        }
        private void read_string(System.IO.BinaryReader fr, ref string data, int length)
        {
            //data = fr.ReadChars(length).ToString();

            byte[] temp = new byte[256];

            fr.Read(temp, 0, length);
            data = Encoding.UTF8.GetString(temp, 0, length);

        }

    

        public AP3()
        {
            
            AP2Info = new AP2();
            AP3Header= new AP3Info();
            WAVHeader = new WAVInfo();

            loadWAV=false;
            loadMotion=false;
            loadKeyframe=false;

            AP3Header.keyframeSize = 0;


        }



	}
}
