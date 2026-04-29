using UnityEngine;
using UnityEngine.UI;

namespace ML.PlaywallKids.DragonPark.Test
{
    public class CommandLineArgumentsTest : MonoBehaviour
    {
        public Text text;

        void Start()
        {
            string[] args = System.Environment.GetCommandLineArgs();
            string combined = "";

            string curCommand = "";
            bool checkCommandValue = false;

            for (int i = 1; i < args.Length; i++)
            {
                string arg = args[i];
                combined += arg + " ";

                if (arg[0] == '-')
                {
                    curCommand = arg;
                    if (curCommand.ToLower().Equals("-bigboard-fullscreen"))
                        checkCommandValue = true;
                }
                else
                {
                    if (checkCommandValue)
                    {
                        if (curCommand.ToLower().Equals("-bigboard-fullscreen"))
                        {
                            int fullscreeen = Screen.fullScreen ? 1 : 0;
                            if (int.TryParse(arg, out fullscreeen))
                            {
                                Screen.fullScreen = fullscreeen != 0;
                            }

                        }
                    }

                    curCommand = "";
                    checkCommandValue = false;
                }
            }

            text.text = combined;
        }
    }
}