using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    using Common;

    /// <summary>
    /// 아쿠아리움 물고기 사용자 메뉴
    /// </summary>
    public class AToolFishMenuPanel : MonoBehaviour
    {
        public UIButton printButton;
        private int _userId;
        public int userId
        {
            get { return _userId; }
            set { _userId = value; Refresh(); }
        }

        public void Refresh()
        {
            printButton.isEnabled = CommonSettings.printerEnabled && !AquariumPrinter.IsPrinted(userId, createFishesObj.Instance().GetRecentFishIdentifier(userId));
        }
        
        public void Release()
        {
            GameObject fish = createFishesObj.Instance().GetRecentFish(userId);
            if (fish != null)
            {
                fish.SendMessage("release");
                fish.AddComponent<PathExample>();
            }

            Destroy(gameObject);
            if (AquariumMenuControl.sharedInstance != null)
                AquariumMenuControl.sharedInstance.Activate(userId);
        }

        public void Print()
        {
            if (AquariumMenuControl.sharedInstance != null)
            {
                gameObject.SetActive(false);
                AquariumMenuControl.sharedInstance.ShowFishPrint(userId);
            }
        }
    }
}