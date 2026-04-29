using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    public class EASAToolClientPetMotionMakerPanel : EASAToolPetMotionMakerBase
    {
        #region Public variables
        public UIScrollView motionListScrollView;
        public UIScrollView motionTimeBoxScrollView;
        public UIProgressBar motionTimelineProgress;

        public UIButton playButton, pauseButton;

        public EASAToolClientPetMotionItem motionItemPrefab;

        public float maxTime = 90.0f;
        #endregion

        #region Properties
        private System.WeakReference _mainPanel = null;
        public EASAToolClientPetMotionPanel mainPanel
        {
            get
            {
                if (_mainPanel == null)
                {
                    return null;
                }
                return _mainPanel.Target as EASAToolClientPetMotionPanel;
            }
            set
            {
                if (_mainPanel == null)
                {
                    _mainPanel = new System.WeakReference(value);
                }
                else
                {
                    _mainPanel.Target = value;
                }
            }
        }

        private bool _playing = false;
        public bool playing
        {
            get
            {
                return _playing;
            }
        }
        #endregion

        public override void BeginShow()
        {
            base.BeginShow();

            // Initialize the motion list
            UIGrid motionListGrid = motionListScrollView.GetComponentInChildren<UIGrid>();
            if (motionListGrid.GetChildList().Count == 0)
            {
                for (int i = 0; i < anims.Length; i++)
                {
                    if (anims[i] != null)
                    {
                        EASAToolClientPetMotionItem item = NGUITools.AddChild(motionListGrid.gameObject, motionItemPrefab.gameObject).GetComponent<EASAToolClientPetMotionItem>();
                        item.mainPanel = mainPanel;
                        item.motionName = anims[i].name;
                        item.Refresh();
                    }
                }
                motionListGrid.Reposition();
                motionListScrollView.ResetPosition();
            }

            // Initialize the motion time box
            motionTimeBoxScrollView.onDragFinished = () =>
            {
                motionTimeBoxScrollView.SetDragAmount(0, 0, true);
            };

            UIGrid motionTimeBoxGrid = motionTimeBoxScrollView.GetComponentInChildren<UIGrid>();
            List<Transform> list = motionTimeBoxGrid.GetChildList();
            foreach (Transform item in list)
            {
                NGUITools.Destroy(item.gameObject);
            }
            list.Clear();

            // Set Timeline progress value to 0
            motionTimelineProgress.value = 0;

            // show the play button, and hide the pause button.
            NGUITools.SetActive(playButton.gameObject, true);
            NGUITools.SetActive(pauseButton.gameObject, false);

            // if the animation has finished, call the stop action.
            animationControl.onFinished = () =>
            {
                NGUITools.SetActive(playButton.gameObject, true);
                NGUITools.SetActive(pauseButton.gameObject, false);

                _playing = false;
            };

            _playing = false;
        }

        public override void Active()
        {
            base.Active();

            playButton.isEnabled = false;
        }

        public override void Update()
        {
            if (connected)
            {
                EASPacket packet = socket.Receive(EASPacket.kTypePetMotion, false);
                if (packet != null)
                {
                    if (packet.GetBool("data/command/next_step"))
                    {
                        _PerformNextStep();

                        socket.Receive(EASPacket.kTypePetMotion);
                    }
                }
            }

            base.Update();
        }

        public void NextStep()
        {
            if (connected)
            {
                EASClientManager.ShowLoading();

                EASPacket packet = new EASPacket();
                packet.type = EASPacket.kTypePetMotion;
                packet.Set("data/command/next_step", true);
                List<object> list = new List<object>();
                foreach (var item in GetMotionItemsInMotionTimeBox())
                {
                    list.Add(item.motionName);
                }
                packet.Set("data/anim_list", list);
                socket.Send(packet);
            }
            else
            {
                _PerformNextStep();
            }
        }

        private void _PerformNextStep()
        {
            EASClientManager.HideLoading();

            mainPanel.NextStep();
        }

        public void RefreshTimeline()
        {
            float value = 1.0f / maxTime;
            float time = 0.0f;

            var items = GetMotionItemsInMotionTimeBox();
            foreach (var item in items)
            {
                var anim = GetClip(item.motionName);
                if (anim != null)
                {
                    time += anim.length;
                }
            }

            value = value * time;

            motionTimelineProgress.value = value;

            playButton.isEnabled = time > 0.0f;
        }

        public EASAToolClientPetMotionItem[] GetMotionItemsInMotionTimeBox()
        {
            List<EASAToolClientPetMotionItem> list = new List<EASAToolClientPetMotionItem>();

            UIGrid motionTimeBoxGrid = motionTimeBoxScrollView.GetComponentInChildren<UIGrid>();
            List<Transform> childs = motionTimeBoxGrid.GetChildList();
            foreach (Transform t in childs)
            {
                EASAToolClientPetMotionItem item = t.GetComponent<EASAToolClientPetMotionItem>();
                if (item != null)
                {
                    list.Add(item);
                }
            }

            return list.ToArray();
        }

        public AnimationClip[] GetClipsInMotionTimeBox()
        {
            List<AnimationClip> list = new List<AnimationClip>();

            var items = GetMotionItemsInMotionTimeBox();

            foreach (var item in items)
            {
                var anim = GetClip(item.motionName);
                if (anim != null)
                {
                    list.Add(anim);
                }
            }

            return list.ToArray();
        }

        public void AddMotionItemInMotionTimeBox(string motionName)
        {
            UIGrid grid = motionTimeBoxScrollView.GetComponentInChildren<UIGrid>();
            if (grid != null)
            {
                EASAToolClientPetMotionItem item = NGUITools.AddChild(grid.gameObject, motionItemPrefab.gameObject).GetComponent<EASAToolClientPetMotionItem>();
                item.mainPanel = mainPanel;
                item.motionName = motionName;
                item.placedInMotionMaker = true;
                item.cloneOnDrag = false;
                item.restriction = UIDragDropItem.Restriction.Vertical;
                item.Refresh();

                grid.Reposition();
                motionTimeBoxScrollView.ResetPosition();
                mainPanel.motionMaker.RefreshTimeline();
            }
        }

        public void Action_Play()
        {
            NGUITools.SetActive(playButton.gameObject, false);
            NGUITools.SetActive(pauseButton.gameObject, true);

            if (_playing)
            {
                Resume();

                EASPacket packet = new EASPacket();
                packet.type = EASPacket.kTypePetMotion;
                packet.Set("data/resume", true);
                socket.Send(packet);
            }
            else
            {
                var clips = GetClipsInMotionTimeBox();
                Play(clips);

                // send packet
                EASPacket packet = new EASPacket();
                packet.type = EASPacket.kTypePetMotion;
                List<object> list = new List<object>();
                foreach (var c in clips)
                    list.Add(c.name);
                packet.Set("data/anim_list", list);
                socket.Send(packet);

                _playing = true;
            }

            Debug.Log("MotionMaker - Play");
        }

        public void Action_Pause()
        {
            NGUITools.SetActive(playButton.gameObject, true);
            NGUITools.SetActive(pauseButton.gameObject, false);

            Pause();

            // send packet
            EASPacket packet = new EASPacket();
            packet.type = EASPacket.kTypePetMotion;
            packet.Set("data/pause", true);
            socket.Send(packet);

            Debug.Log("MotionMaker - Pause");
        }

        public void Action_Stop()
        {
            NGUITools.SetActive(playButton.gameObject, true);
            NGUITools.SetActive(pauseButton.gameObject, false);

            _playing = false;

            Stop();

            // send packet
            EASPacket packet = new EASPacket();
            packet.type = EASPacket.kTypePetMotion;
            packet.Set("data/stop", true);
            socket.Send(packet);

            Debug.Log("MotionMaker - Stop");
        }
    }
}