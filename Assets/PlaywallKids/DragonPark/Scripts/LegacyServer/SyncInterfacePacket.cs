using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SyncInterfacePacket {
	public byte PID;
	public byte CMD;

	public byte[] data = new byte[0];

	public const byte STX = 0x02;
	public const byte ETX = 0x03;

	public const byte PID_PLAYWALL = 0x12;
	public const byte PID_SYNCINTERFACE = 0x11;

	public const byte CMD_MENU = 0x20;
	public const byte CMD_DRAGON = 0x21;
	public const byte CMD_AVATAR = 0x22;
	public const byte CMD_MOTION = 0x23;

	// CMD_MENU
	public const byte DATA_MENU_DRAGON = 0x31;
	public const byte DATA_MENU_AVATAR = 0x32;
    public const byte DATA_MENU_MOTION = 0x33;
    public const byte DATA_MENU_DRAWING = 0x34;
    public const byte DATA_MENU_FREEDRAWING = 0x35;
    public const byte DATA_MENU_ROBOT = 0x36;
	public const byte DATA_MENU_NONE = 0x40;
	public const byte DATA_MENU_RESET = 0x50;

	// CMD_AVATAR
	public const byte DATA_AVATAR_CODE_RECOGNITION = 0x01;
	public const byte DATA_AVATAR_CODE_ACTIVE = 0x01;
	public const byte DATA_AVATAR_CODE_COORD = 0x02;
	public const byte DATA_AVATAR_CODE_JUMP = 0x03;

	/// <summary>
	/// Parse the buffer and generate the packet instance.
	/// </summary>
	public static SyncInterfacePacket Parse(List<byte> buffer) {
		return Parse (buffer, true);
	}

	public static SyncInterfacePacket Parse(List<byte> buffer, bool removesParsedArea) {
		SyncInterfacePacket packet = null;

		int stxIndex = buffer.IndexOf(STX);

		if(stxIndex > -1) {
			int etxIndex = -1;
			if(buffer.Count > stxIndex + 3) {
				byte PID = buffer[stxIndex + 1];
				byte CMD = buffer[stxIndex + 2];
				byte CODE = buffer[stxIndex + 3];
				if(PID == PID_SYNCINTERFACE && CMD == CMD_AVATAR && CODE == DATA_AVATAR_CODE_COORD) {
					if(buffer.Count >= stxIndex + 492) {
						etxIndex = stxIndex + 492;
					}
					else if(buffer.Count >= stxIndex + 4 && buffer[stxIndex + 4] == ETX) {
						etxIndex = stxIndex + 4;
					}
					else {
						etxIndex = -1;
					}
				}
				else if(PID == PID_SYNCINTERFACE && CMD == CMD_DRAGON && CODE == 0x41) {
					if(buffer.Count >= stxIndex + 28) {
						etxIndex = stxIndex + 28;
					}
					else {
						etxIndex = -1;
					}
				}
				else if(PID == PID_SYNCINTERFACE && CMD == CMD_MOTION) {
					if(CODE == 0x01) {
						etxIndex = stxIndex + 6;
					}
					else if(CODE == 0x02) {
						etxIndex = stxIndex + 22;
					}
					else {
						etxIndex = buffer.IndexOf(ETX);
					}
				}
				else {
					etxIndex = buffer.IndexOf(ETX);
				}
			}

			int size = etxIndex - stxIndex + 1;
			if(size > 3) {
				int offset = stxIndex + 1;

				// default infos
				packet = new SyncInterfacePacket();
				packet.PID = buffer[offset++];
				packet.CMD = buffer[offset++];

				// additional data area
				if(offset < etxIndex) {
					int dataLength = etxIndex - offset;
					packet.data = buffer.GetRange(offset, etxIndex - offset).ToArray();
					offset += dataLength + 1;
				}

				// remove parsed buffer range
				if(removesParsedArea) {
					buffer.RemoveRange(0, offset);
				}
				
				DebugUtil.Log(string.Format("SyncInterfacePacket.Parse() : Parsed packet. STX at {0}, ETX at {1}, length : {2}", stxIndex, etxIndex, offset));
			}
			else if(etxIndex > -1) {
				DebugUtil.Log(string.Format("SyncInterfacePacket.Parse() : Invalid packet. STX at {0}, ETX at {1}", stxIndex, etxIndex));
				buffer.RemoveRange(0, etxIndex + 1);
			}
		}

		return packet;
	}

	public List<byte> ToByteList() {
		List<byte> byteList = new List<byte>();

		// STX
		byteList.Add(STX);

		// PID
		byteList.Add(PID);

		// CMD
		byteList.Add(CMD);

		// Data
		byteList.AddRange(data);

		// ETX
		byteList.Add(ETX);

		return byteList;
	}

	public byte[] ToByteArray() {
		return ToByteList().ToArray();
	}
}
