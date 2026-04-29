using System.Threading;

namespace ML.PlaywallKids.Aquarium
{
    public class SeaStoryServerThread
    {
        private Thread _thread;

        public SeaStoryServerThread()
        {
            ThreadStart ts = new ThreadStart(Run);
            _thread = new Thread(ts);
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Abort()
        {
            _thread.Abort();
        }

        public void Run()
        {
        }
    }
}