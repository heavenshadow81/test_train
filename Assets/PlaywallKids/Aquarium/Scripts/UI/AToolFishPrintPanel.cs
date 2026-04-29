using System.Collections;
using System.IO;
using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    /// <summary>
    /// 아쿠아리움 물고기 인쇄 미리보기 창
    /// </summary>
    public class AToolFishPrintPanel : MonoBehaviour
    {
        public UIPanel target;
        public UITexture previewImage;
        public GameObject printButton;

        public int userId { get; private set; }
        public string identifier { get; private set; }

        public void Set(int newUserId)
        {
            Cleanup();
            userId = newUserId;
            identifier = createFishesObj.Instance().GetRecentFishIdentifier(userId);
            StopCoroutine("_LoadPreviewImage");
            StartCoroutine("_LoadPreviewImage");
        }

        public void Close()
        {
            Cleanup();
            Destroy(gameObject);
            if (AquariumMenuControl.sharedInstance != null)
                AquariumMenuControl.sharedInstance.ShowFishMenu(userId);
        }

        public void Cleanup()
        {
            if (previewImage.mainTexture != null)
            {
                Destroy(previewImage.mainTexture);
                previewImage.mainTexture = null;
            }
        }

        private IEnumerator _LoadPreviewImage()
        {
            printButton.SetActive(false);
            string path = ResourceManager.GetResourcePath();
            path = Path.Combine(path, string.Format("Aquarium/print/{0}.png", userId)).Replace("\\", "/");
            WWW www = new WWW(string.Concat("file:///", path));
            yield return www;
            printButton.SetActive(true);
            Texture tex = www.texture;
            if (tex != null)
            {
                previewImage.mainTexture = tex;
                previewImage.mainTexture.wrapMode = TextureWrapMode.Clamp;
                previewImage.fixedAspect = true;
            }
        }

        public void Print()
        {
            StartCoroutine(_Print());
        }

        IEnumerator _Print()
        {
            printButton.SetActive(false);
            var printer = AquariumMenuControl.sharedInstance.GetPrinter(userId);
            printer.Set(userId, identifier, previewImage.mainTexture);
            printer.Print();
            while (printer.isPrinting)
                yield return null;
            Close();
        }
    }
}