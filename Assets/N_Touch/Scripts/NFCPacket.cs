using System;
using System.Text;

namespace ML.NFCPlugin
{
    public class Packet
    {
        public enum PacketType { CardAdded, CardRemoved }
        public PacketType Type;
        public int Length { get { return 8 + Data.Length; } }
        public IPacketData Data;

        public Packet() { }

        public Packet(byte[] bytes)
        {
            Array.Reverse(bytes, 0, 4);
            Array.Reverse(bytes, 4, 4);
            Type = (PacketType)BitConverter.ToInt32(bytes, 0);
            switch (Type)
            {
                case PacketType.CardAdded:
                    CardAddedPacketData Packet = new CardAddedPacketData(bytes, 8);
                    Data = Packet;
                    break;
            }
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[Length];
            byte[] typeBytes = BitConverter.GetBytes((int)Type);
            Array.Reverse(typeBytes);
            Array.Copy(typeBytes, 0, bytes, 0, 4);
            byte[] lengthBytes = BitConverter.GetBytes((int)Length);
            Array.Reverse(lengthBytes);
            Array.Copy(lengthBytes, 0, bytes, 4, 4);
            Array.Copy(Data.GetBytes(), 0, bytes, 8, Data.Length);
            return bytes;
        }

        public interface IPacketData
        {
            int Length { get; }
            byte[] GetBytes();
        }

        public class CardAddedPacketData : IPacketData
        {
            public int DeviceClass;
            public string CardName;
            public string ATR;
            public string UID;
            public int ReaderId;

            public int Length { get { return 112; } }

            public CardAddedPacketData() { }

            public CardAddedPacketData(byte[] bytes, int index)
            {
                Array.Reverse(bytes, index, 4);
                DeviceClass = BitConverter.ToInt32(bytes, index);
                CardName = Encoding.UTF8.GetString(bytes, index + 4, 32).Replace("\0", "");
                ATR = Encoding.UTF8.GetString(bytes, index + 36, 32).Replace("\0", "");
                UID = Encoding.UTF8.GetString(bytes, index + 68, 40).Replace("\0", "");
                Array.Reverse(bytes, index + 108, 4);
                ReaderId = BitConverter.ToInt32(bytes, index + 108);
            }

            public byte[] GetBytes()
            {
                byte[] bytes = new byte[Length];
                byte[] deviceClassBytes = BitConverter.GetBytes(DeviceClass);
                Array.Reverse(deviceClassBytes);
                Array.Copy(deviceClassBytes, 0, bytes, 0, 4);

                byte[] cardNameBytes = Encoding.UTF8.GetBytes(CardName);
                if (cardNameBytes.Length > 32) cardNameBytes[31] = 0;
                Array.Copy(cardNameBytes, 0, bytes, 4, Math.Min(32, cardNameBytes.Length));

                byte[] atrBytes = Encoding.UTF8.GetBytes(ATR);
                if (atrBytes.Length > 32) atrBytes[31] = 0;
                Array.Copy(atrBytes, 0, bytes, 36, Math.Min(32, cardNameBytes.Length));

                byte[] uidBytes = Encoding.UTF8.GetBytes(UID);
                if (uidBytes.Length > 40) uidBytes[39] = 0;
                Array.Copy(uidBytes, 0, bytes, 68, Math.Min(40, cardNameBytes.Length));

                byte[] readerIdBytes = BitConverter.GetBytes(ReaderId);
                Array.Reverse(readerIdBytes);
                Array.Copy(readerIdBytes, 0, bytes, 108, 4);

                return bytes;
            }
        }
    }
}