using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using ML.PlaywallKids.DragonPark;

public class PlayWallServer : MonoBehaviour {
	#region Public variables
	public string IP = "127.0.0.1";
	public int port = 5000;

	public int[] userSeqs;
	
	public AvatarSkeleton user1Skeleton = null;
	public AvatarSkeleton user2Skeleton = null;
	
	public bool recognizedSkeletons = false;
	public bool activatedSkeletons = false;
	public bool receivingSkeletionJoints = false;
	public bool readyToPlayAvatarGame = false;

	public byte currentMotion = 0x0;
	public bool user1MotionFlag = false;
	public bool user2MotionFlag = false;

	public Vector3 user1CursorPos = Vector3.zero;
	public Vector3 user2CursorPos = Vector3.zero;
	#endregion
	
	#region Properties
	private static PlayWallServer __sharedInstance = null;
	public static PlayWallServer sharedInstance {
		get {
			if(__sharedInstance == null) {
				__sharedInstance = FindObjectOfType<PlayWallServer>();
				if(__sharedInstance == null) {
					GameObject go = new GameObject("PlayWallServer");
					__sharedInstance = go.AddComponent<PlayWallServer>();
					DontDestroyOnLoad(go);
				}
			}
			return __sharedInstance;
		}
	}
	#endregion
	
	#region Private variables
	private Socket _socket;
	private Socket _client;
	
	private byte[] _buffer = new byte[kBufferSize];
	private List<byte> _bufferList = new List<byte>(kBufferSize * 4);

	private MenuControl.Menu _prevMenu = MenuControl.Menu.None;
	private bool _mirroringAvatar = false;
	#endregion
	
	#region Constants
	public const int kBufferSize = 256;
	#endregion
	
	#region Unity Methods
	void Start () {
		_InitSocket();
	}
	
	void Update() {
		_ParsePackets();
		_SendRequiredPackets();
		_CheckMenus();
	}

	private void _SendRequiredPackets() {
		if(recognizedSkeletons && !activatedSkeletons) {
			activatedSkeletons = true;

			SyncInterfacePacket packet = new SyncInterfacePacket();
			packet.PID = SyncInterfacePacket.PID_PLAYWALL;
			packet.CMD = SyncInterfacePacket.CMD_AVATAR;
			packet.data = new byte[] { SyncInterfacePacket.DATA_AVATAR_CODE_ACTIVE };
			byte[] bytes = packet.ToByteArray();
			if(_client != null) {
				_client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new System.AsyncCallback(OnSend), _client);
			}
		}
	}
	
	void OnDestroy() {
		if(_client != null) {
			_client.Close();
			_client = null;
		}
		
		if(_socket != null) {
			_socket.BeginDisconnect(false, new System.AsyncCallback(OnEndHostComplete), _socket);
		}
	}
	#endregion
	
	private void _InitSocket() {
		port = SettingsManager.port;

		Debug.Log ("Hosting on port " + port);
		
		try {
			IPEndPoint myEndPoint = new IPEndPoint(IPAddress.Any, port);
			//_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			//_socket.Bind(new IPEndPoint(IPAddress.Any, port));
			_socket = new Socket(myEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_socket.Bind(myEndPoint);
			_socket.Listen(5);
			_socket.BeginAccept(new System.AsyncCallback(OnClientConnect), _socket);
		}
		catch (System.Exception e)
		{
			Debug.LogError ("Exception when attempting to host (" + port + "): " + e);
			if(_socket != null) {
				_socket.BeginDisconnect (false, new System.AsyncCallback (OnEndHostComplete), _socket);
			}
		}
	}
	
	void OnEndHostComplete (System.IAsyncResult result) {
		Socket socket = (Socket)result.AsyncState;

		if(socket != null) {
			socket.EndDisconnect(result);
			socket.Close();
			socket = null;
		}
	}
	
	void OnClientConnect (System.IAsyncResult result)
	{
		Debug.Log ("Handling client connecting");
		if(_socket == null) return;
		try
		{
			Socket client = _socket.EndAccept(result);
			IPEndPoint endPoint = (IPEndPoint)client.RemoteEndPoint;
			string address = endPoint.Address.ToString();

			Debug.Log ("Client connected : " + address);

			_client = client;
			
			_socket.BeginAccept (new System.AsyncCallback (OnClientConnect), _socket);
			
			client.BeginReceive(_buffer, 0, kBufferSize, SocketFlags.None, new System.AsyncCallback(OnReceive), client);
		}
		catch (System.Exception e)
		{
			Debug.LogError ("Exception when accepting incoming connection: " + e);
		}
	}
	
	void OnSend(System.IAsyncResult result) {
		Socket client = result.AsyncState as Socket;
		if(client != null) {
			client.EndSend(result);
		}
	}
	
	void OnReceive(System.IAsyncResult result) {
		Socket client = result.AsyncState as Socket;
		if(client != null) {
			int len = client.EndReceive(result);
			for(int i = 0; i < len; i++) {
				_bufferList.Add(_buffer[i]);
			}
			
			client.BeginReceive(_buffer, 0, kBufferSize, SocketFlags.None, new System.AsyncCallback(OnReceive), client);
		}
	}

	private void _CheckMenus() {
		MenuControl menuControl = MenuControl.sharedInstance;
		if(menuControl != null) {
			if(menuControl.menu != _prevMenu) {
					
				if(_client != null) {
					SendSelectedMenu(menuControl.menu);
				}
			}
			_prevMenu = menuControl.menu;
		}
	}
	
	private void _ParsePackets() {
		SyncInterfacePacket packet = SyncInterfacePacket.Parse(_bufferList);
		if(packet != null) {
			_ProcessPacket(packet);
		}
	}

	private void _ProcessPacket(SyncInterfacePacket packet) {
		switch(packet.CMD) {
		case SyncInterfacePacket.CMD_DRAGON:
			_ProcessDragonPacket(packet);
			break;
		case SyncInterfacePacket.CMD_AVATAR:
			_ProcessAvatarPacket(packet);
			break;
		case SyncInterfacePacket.CMD_MOTION:
			_ProcessMotionPacket(packet);
			break;
		}
	}

	private void _ProcessDragonPacket(SyncInterfacePacket packet) {
		// parse
		int offset = 0;
		byte code = packet.data[offset++];

		if(code == 0x41) {
			// user count
			int userCount = System.BitConverter.ToInt32(packet.data, offset);
			offset += 4;

			// user sequences
			userSeqs = new int[userCount];
			for(int i = 0; i < userCount; i++) {
				int userSeq = System.BitConverter.ToInt32(packet.data, offset);
				offset += 4;
				userSeqs[i] = userSeq;
			}

			// set menu control values
			MenuControl.userCount = userCount;
			MenuControl.userSeqs = userSeqs;
			MenuControl.sharedInstance.menu = MenuControl.Menu.Dragon;
		}
		else if(code == 0x42) {
		}
	}

	private void _ProcessAvatarPacket(SyncInterfacePacket packet) {
		int offset = 0;
		byte code = packet.data[offset++];
		if(code == SyncInterfacePacket.DATA_AVATAR_CODE_RECOGNITION) {
			recognizedSkeletons = true;
		}
		else if(code == SyncInterfacePacket.DATA_AVATAR_CODE_COORD && packet.data.Length > 1) {
			int user1 = System.BitConverter.ToInt32(packet.data, offset);
			offset += 4;

			int user2 = System.BitConverter.ToInt32(packet.data, offset);
			offset += 4;
			
			Vector3[] user1Positions = new Vector3[AvatarSkeleton.jointCount];
			Vector3[] user2Positions = new Vector3[AvatarSkeleton.jointCount];

			// user 1 positions
			for(int i = 0; i < AvatarSkeleton.jointCount; i++) {
				// x
				int x = 1920 - System.BitConverter.ToInt32(packet.data, offset);
				offset += 4;
				
				// y
				int y = 1440 - System.BitConverter.ToInt32(packet.data, offset);
				offset += 4;
				
				// z
				int z = 4000 - System.BitConverter.ToInt32(packet.data, offset);
				offset += 4;
				
				user1Positions[i] = new Vector3(x, y, z);
			}
			
			// user 2 positions
			for(int i = 0; i < AvatarSkeleton.jointCount; i++) {
				// x
				int x = 1920 - (System.BitConverter.ToInt32(packet.data, offset) - 1920);
				offset += 4;
				
				// y
				int y = 1440 - System.BitConverter.ToInt32(packet.data, offset);
				offset += 4;
				
				// z
				int z = 4000 - System.BitConverter.ToInt32(packet.data, offset);
				offset += 4;
				
				user2Positions[i] = new Vector3(x, y, z);
			}

			if(user1 > 0 && user1Skeleton != null) {
				user1Skeleton.MoveJoints(user1Positions);
			}
			if(user2 > 0 && user2Skeleton != null) {
				user2Skeleton.MoveJoints(user2Positions);
			}
			receivingSkeletionJoints = true;
		}
		else if(code == SyncInterfacePacket.DATA_AVATAR_CODE_JUMP || (code == SyncInterfacePacket.DATA_AVATAR_CODE_COORD && packet.data.Length == 1)) {
			readyToPlayAvatarGame = true;
		}
	}

	private void _ProcessMotionPacket(SyncInterfacePacket packet) {
		int offset = 0;
		byte code = packet.data[offset++];

		// motion recognition
		if(code == 0x01) {
			byte player = packet.data[offset++];
			byte motion = packet.data[offset++];

			if(player == 0x00) {
				Debug.Log (string.Format("Player 0 Motion (motion : {0}) -> Attempt to accept both players!", motion));
				user1MotionFlag = user2MotionFlag = true;
			}
			else {
				if(player == 0x01) {
					Debug.Log (string.Format("Player 1 Motion (motion : {0})", motion));
					user1MotionFlag = true;
				}
				else if(player == 0x02) {
					Debug.Log (string.Format("Player 2 Motion (motion : {0})", motion));
					user2MotionFlag = true;
				}
			}
			currentMotion = motion;
		}
		else if(code == 0x02) {
			byte user1 = packet.data[offset++];
			byte user2 = packet.data[offset++];

			int user1CursorX = System.BitConverter.ToInt32(packet.data, offset);
			offset += 4;
			int user1CursorY = System.BitConverter.ToInt32(packet.data, offset);
			offset += 4;
			int user2CursorX = System.BitConverter.ToInt32(packet.data, offset);
			offset += 4;
			int user2CursorY = System.BitConverter.ToInt32(packet.data, offset);
			offset += 4;

			if(user1 > 0) {
				user1CursorPos = new Vector3(user1CursorX, 1440-user1CursorY);
			}
			if(user2 > 0) {
				user2CursorPos = new Vector3(user2CursorX, 1440-user2CursorY);
			}
		}
	}

	public void SendMotionDone() {
		if(currentMotion > 0) {
			var packet = new SyncInterfacePacket();
			packet.PID = SyncInterfacePacket.PID_PLAYWALL;
			packet.CMD = SyncInterfacePacket.CMD_MOTION;
			packet.data = new byte[] { 0x11, 0x00, currentMotion };
			byte[] bytes = packet.ToByteArray();
			
			currentMotion = 0;
			user1MotionFlag = user2MotionFlag = false;

			if(_client != null) {
				_client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new System.AsyncCallback(OnSend), _client);
				Debug.Log(string.Format("Packet(0x11, 0x00, 0x{0:X}) --> SyncInterface", packet.data[2]));
			}
			else {
				Debug.Log("Send failed (0x23), client is null.");
			}
		}
		Debug.Log ("current motion is 0");
	}

	public void StartAvatarTracking() {
		if(_client != null) {
			SyncInterfacePacket packet = new SyncInterfacePacket();
			packet.PID = SyncInterfacePacket.PID_PLAYWALL;
			packet.CMD = SyncInterfacePacket.CMD_AVATAR;
			packet.data = new byte[] { SyncInterfacePacket.DATA_AVATAR_CODE_ACTIVE };

			byte[] bytes = packet.ToByteArray();
			_client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new System.AsyncCallback(OnSend), _client);
		}
	}

	public void SendReset() {
		SendSelectedMenu(SyncInterfacePacket.DATA_MENU_RESET);
	}

	public void SendSelectedMenu(byte code) {
		user1Skeleton = null;
		user2Skeleton = null;
		recognizedSkeletons = false;
		activatedSkeletons = false;
		receivingSkeletionJoints = false;
		readyToPlayAvatarGame = false;
		currentMotion = 0;
		user1MotionFlag = user2MotionFlag = false;
		user1CursorPos = user2CursorPos = Vector3.zero;

		if(code == SyncInterfacePacket.DATA_MENU_RESET) {
			userSeqs = null;
		}

		if(_client != null) {
			SyncInterfacePacket packet = new SyncInterfacePacket();
			packet.PID = SyncInterfacePacket.PID_PLAYWALL;
			packet.CMD = SyncInterfacePacket.CMD_MENU;

			byte data = code;

			packet.data = new byte[1] { data };
			
			byte[] bytes = packet.ToByteArray();
			_client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new System.AsyncCallback(OnSend), _client);

			Debug.Log (string.Format ("Menu {0} --> SyncInterface", code));
		}
	}

	public void SendSelectedMenu(MenuControl.Menu menu) {
		byte data = 0x0;
		switch(menu) {
		case MenuControl.Menu.None:
			data = SyncInterfacePacket.DATA_MENU_NONE;
			break;
		case MenuControl.Menu.Dragon:
			data = SyncInterfacePacket.DATA_MENU_DRAGON;
			break;
		case MenuControl.Menu.Avatar:
			data = SyncInterfacePacket.DATA_MENU_AVATAR;
			break;
		case MenuControl.Menu.MotionGame:
			data = SyncInterfacePacket.DATA_MENU_MOTION;
			break;
		case MenuControl.Menu.Drawing:
			data = SyncInterfacePacket.DATA_MENU_DRAWING;
			break;
		case MenuControl.Menu.Reset:
			data = SyncInterfacePacket.DATA_MENU_RESET;
			break;
		default:
			data = SyncInterfacePacket.DATA_MENU_RESET;
			break;
		}

		SendSelectedMenu(data);
	}
}
