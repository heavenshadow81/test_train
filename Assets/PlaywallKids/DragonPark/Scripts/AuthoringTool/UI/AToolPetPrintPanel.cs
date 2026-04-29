using System.Collections;
using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// Print preview panel of Dragon Park.
    /// </summary>
    public class AToolPetPrintPanel : MonoBehaviour
    {
        public UIPanel target;
        public UITexture previewImage;
        public GameObject buttons;
        public DragonPrinter dragonPrinter;

        public int userId
        {
            get { return _userId; }
            set { Set(value); }
        }
        private int _userId;

        public void Set(int newUserId)
        {
            Cleanup();
            _userId = newUserId;
            StopCoroutine("_LoadPreviewImage");
            StartCoroutine("_LoadPreviewImage");
        }

        public void Close()
        {
            Cleanup();
            MenuControl.sharedInstance.HidePetPrint(userId);
            GameObject go = SimpleInstantiatedTemplateControl.GetCurrentTemplate(userId);
            var bone = go.GetComponent<BoneObject>();
            if (bone is FreeDrawingObjectBone)
                MenuControl.sharedInstance.ShowFreeDrawingMenu(userId);
            else
                MenuControl.sharedInstance.ShowPetMenu(userId);
        }

        public void Cleanup()
        {
            previewImage.mainTexture = null;
        }

        private IEnumerator _LoadPreviewImage()
        {
            buttons.SetActive(false);

            dragonPrinter.Set(userId);
            while (dragonPrinter.isCapturing)
                yield return null;

            if (dragonPrinter.previewTexture != null)
            {
                previewImage.mainTexture = dragonPrinter.previewTexture;
                previewImage.mainTexture.wrapMode = TextureWrapMode.Clamp;
                previewImage.fixedAspect = true;
            }

            buttons.SetActive(true);
        }

        public void Print()
        {
            StartCoroutine(_Print());
        }

        private IEnumerator _Print()
        {
            buttons.SetActive(false);
            dragonPrinter.Print();
            while (dragonPrinter.isPrinting)
                yield return null;
            Close();
        }
    }
}