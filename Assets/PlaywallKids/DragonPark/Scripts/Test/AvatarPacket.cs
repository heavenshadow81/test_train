using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark.Test
{
    public class AvatarPacket
    {
        public int userCount = 0;

        public Vector3[] user1Positions = new Vector3[AvatarSkeleton.jointCount];
        public Vector3[] user2Positions = new Vector3[AvatarSkeleton.jointCount];

        public const int kPacketSize = 484;

        private static byte[] _cachedIntegerBytes = new byte[4];

        public static AvatarPacket Parse(List<byte> buffer)
        {
            if (buffer.Count < kPacketSize)
            {
                return null;
            }

            AvatarPacket packet = new AvatarPacket();

            int offset = 0;

            // player 1, 2
            buffer.CopyTo(offset, _cachedIntegerBytes, 0, _cachedIntegerBytes.Length);
            offset += _cachedIntegerBytes.Length;
            packet.userCount = System.BitConverter.ToInt32(_cachedIntegerBytes, 0);

            // user 1 positions
            for (int i = 0; i < AvatarSkeleton.jointCount; i++)
            {
                // x
                buffer.CopyTo(offset, _cachedIntegerBytes, 0, _cachedIntegerBytes.Length);
                offset += _cachedIntegerBytes.Length;
                int x = System.BitConverter.ToInt32(_cachedIntegerBytes, 0);
                x = 1920 - x;

                // y
                buffer.CopyTo(offset, _cachedIntegerBytes, 0, _cachedIntegerBytes.Length);
                offset += _cachedIntegerBytes.Length;
                int y = System.BitConverter.ToInt32(_cachedIntegerBytes, 0);

                // In my implementation, y axis is up side.
                y = 1440 - y;

                // z
                buffer.CopyTo(offset, _cachedIntegerBytes, 0, _cachedIntegerBytes.Length);
                offset += _cachedIntegerBytes.Length;
                int z = System.BitConverter.ToInt32(_cachedIntegerBytes, 0);

                z = 4000 - z;

                packet.user1Positions[i] = new Vector3(x, y, z);
            }

            // user 2 positions
            for (int i = 0; i < AvatarSkeleton.jointCount; i++)
            {
                // x
                buffer.CopyTo(offset, _cachedIntegerBytes, 0, _cachedIntegerBytes.Length);
                offset += _cachedIntegerBytes.Length;
                int x = System.BitConverter.ToInt32(_cachedIntegerBytes, 0);

                x = 1920 + (3840 - x);

                // y
                buffer.CopyTo(offset, _cachedIntegerBytes, 0, _cachedIntegerBytes.Length);
                offset += _cachedIntegerBytes.Length;
                int y = System.BitConverter.ToInt32(_cachedIntegerBytes, 0);

                // In my implementation, y axis is up side.
                y = 1440 - y;

                // z
                buffer.CopyTo(offset, _cachedIntegerBytes, 0, _cachedIntegerBytes.Length);
                offset += _cachedIntegerBytes.Length;
                int z = System.BitConverter.ToInt32(_cachedIntegerBytes, 0);

                z = 4000 - z;

                packet.user2Positions[i] = new Vector3(x, y, z);
            }

            return packet;
        }

        public byte[] ToByteArray()
        {
            List<byte> bytes = new List<byte>(kPacketSize);
            bytes.AddRange(System.BitConverter.GetBytes(userCount));
            foreach (Vector3 pos in user1Positions)
            {
                bytes.AddRange(System.BitConverter.GetBytes((int)pos.x));
                bytes.AddRange(System.BitConverter.GetBytes((int)pos.y));
                bytes.AddRange(System.BitConverter.GetBytes((int)pos.z));
            }
            foreach (Vector3 pos in user2Positions)
            {
                bytes.AddRange(System.BitConverter.GetBytes((int)pos.x));
                bytes.AddRange(System.BitConverter.GetBytes((int)pos.y));
                bytes.AddRange(System.BitConverter.GetBytes((int)pos.z));
            }
            return bytes.ToArray();
        }
    }
}