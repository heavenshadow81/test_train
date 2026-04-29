using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    public class EASAToolClientPetMotionItem : UIDragDropItem
    {
        // main panel
        public EASAToolClientPetMotionPanel mainPanel;

        // UI
        public UISprite sprite;
        public UIButton button;
        public UILabel nameLabel;

        // Properties
        public bool isDragOk = true;
        public bool placedInMotionMaker = false;
        public string motionName = "";

        // Private variables
        private float _prevClickTime = 0.0f;

        protected override void Start()
        {
            base.Start();

            if (sprite == null)
                sprite = GetComponent<UISprite>();
        }


        public void Refresh()
        {
            nameLabel.text = motionName;

            if (cloneOnDrag)
            {
                sprite.spriteName = "motion box";
                sprite.MakePixelPerfect();

                if (button != null) button.normalSprite = "motion box";

                nameLabel.color = Color.black;
            }
            else
            {
                sprite.spriteName = "motion box 2";
                sprite.MakePixelPerfect();

                if (button != null) button.normalSprite = "motion box 2";

                nameLabel.color = Color.white;
            }
        }

        public void Click()
        {
            if ((Time.time - _prevClickTime) < 0.4f)
                DoubleClick();
            else
            {
                if (!placedInMotionMaker)
                {
                    Preview();

                    if (mainPanel != null && mainPanel.motionMaker != null &&
                        mainPanel.motionMaker.GetMotionItemsInMotionTimeBox().Length < 4)
                    {
                        mainPanel.motionMaker.AddMotionItemInMotionTimeBox(motionName);
                    }
                }
            }

            _prevClickTime = Time.time;
        }

        public void DoubleClick()
        {
            if (placedInMotionMaker)
            {
                if (mainPanel != null && mainPanel.motionMaker != null)
                {
                    NGUITools.Destroy(gameObject);
                    mainPanel.motionMaker.motionTimeBoxScrollView.ResetPosition();
                    if (mainPanel.motionMaker.motionTimeBoxScrollView.GetComponentInChildren<UIGrid>() != null)
                    {
                        mainPanel.motionMaker.motionTimeBoxScrollView.GetComponentInChildren<UIGrid>().Reposition();
                    }
                    mainPanel.motionMaker.RefreshTimeline();
                }
            }
        }

        public void Preview()
        {
            if (!mainPanel.motionMaker.playing)
            {
                mainPanel.motionMaker.Play(new string[] { motionName });

                // send packet
                EASPacket packet = new EASPacket();
                packet.type = EASPacket.kTypePetMotion;
                List<object> list = new List<object>();
                list.Add(motionName);
                packet.Set("data/anim_list", list);
                mainPanel.socket.Send(packet);
            }

            Debug.Log("MotionItem - Preview");
        }

        protected override void OnDragDropRelease(GameObject surface)
        {
            if (surface != null)
            {
                if ((surface.name.Equals("MotionTimeBoxBackground") ||
                    (surface.GetComponent<EASAToolClientPetMotionItem>() != null &&
                 surface.GetComponentInParent<UIDragDropContainer>() != null &&
                    surface.GetComponentInParent<UIDragDropContainer>().name.Equals("MotionTimeBoxBackground"))) &&
                    mainPanel.motionMaker.GetMotionItemsInMotionTimeBox().Length < 4)
                {
                    cloneOnDrag = false;
                    placedInMotionMaker = true;
                    restriction = Restriction.Vertical;
                    if (mDragScrollView != null)
                        mDragScrollView.scrollView = null;
                }
                else
                {
                    cloneOnDrag = true;
                }
            }
            else
            {
                cloneOnDrag = true;
            }

            base.OnDragDropRelease(surface);

            Refresh();

            if (mainPanel != null && mainPanel.motionMaker != null)
            {
                mainPanel.motionMaker.RefreshTimeline();

                if (mainPanel.motionMaker.GetMotionItemsInMotionTimeBox().Length == 1)
                {
                    StartCoroutine(SnapToTop());
                }
            }
        }

        public IEnumerator SnapToTop()
        {
            yield return null;

            UIScrollView sv = mainPanel.motionMaker.motionTimeBoxScrollView;
            UIGrid grid = sv.GetComponent<UIGrid>();
            if (grid != null) grid.Reposition();
            sv.ResetPosition();
        }
    }
}