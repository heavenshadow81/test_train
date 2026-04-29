using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.EAS
{
    public class EASClientKAvatarUI : EASAnimatablePanel
    {
        public void Play()
        {
            if (connected)
            {
                EASPacket packet = new EASPacket();
                packet.type = EASPacket.kTypeRobot;
                packet.Set("data/play", true);
                socket.Send(packet);
            }
        }

        public void Stop()
        {
            if (connected)
            {
                EASPacket packet = new EASPacket();
                packet.type = EASPacket.kTypeRobot;
                packet.Set("data/stop", true);
                socket.Send(packet);
            }
        }
    }
}