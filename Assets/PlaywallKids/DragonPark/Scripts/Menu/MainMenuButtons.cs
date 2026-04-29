using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// Simple main menu class.
    /// Send messages to MenuControl.
    /// </summary>
    public class MainMenuButtons : MonoBehaviour
    {
        public AudioClip sound;

        public void Drawing()
        {
            AudioSource.PlayClipAtPoint(sound, Vector3.zero);
            MenuControl.sharedInstance.ShowCanvas();
        }

        public void Character()
        {
            AudioSource.PlayClipAtPoint(sound, Vector3.zero);
            MenuControl.sharedInstance.ShowDragon();
        }

        public void Avatar()
        {
            AudioSource.PlayClipAtPoint(sound, Vector3.zero);
            MenuControl.sharedInstance.ShowAvatar();
        }

        public void Motion()
        {
            AudioSource.PlayClipAtPoint(sound, Vector3.zero);
            MenuControl.sharedInstance.ShowMotion();
        }

        public void PetMotion()
        {
            AudioSource.PlayClipAtPoint(sound, Vector3.zero);
            MenuControl.sharedInstance.ShowPetMotion();
        }


        public void FreeDrawing()
        {
            AudioSource.PlayClipAtPoint(sound, Vector3.zero);
            MenuControl.sharedInstance.ShowFreeDrawing();
        }
    }
}