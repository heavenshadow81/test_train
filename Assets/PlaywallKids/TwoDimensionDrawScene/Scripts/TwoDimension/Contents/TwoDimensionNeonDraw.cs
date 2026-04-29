using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class TwoDimensionNeonDraw : TwoDimensionBase
    {
        public GameObject[] goNeonLayers;

        public UIButton clearButton;


        void OnEnable()
        {
            for (int i = 0; i < clearButton.onClick.Count; i++)
                clearButton.onClick[i].Execute();
        }

        void Start()
        {
            if (goNeonLayers != null)
            {
                for (int i = 0; i < goNeonLayers.Length; i++)
                    goNeonLayers[i].SetLayerRecursively("Neon");
            }
        }

        public override bool PlayStart()
        {
            return true;
        }
    
        public override bool Play()
        {
            return false;
        }

        public override bool PlayEnd()
        {
            return true;
        }
    }
}