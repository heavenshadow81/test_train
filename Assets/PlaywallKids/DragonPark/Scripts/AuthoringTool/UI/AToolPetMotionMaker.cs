using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    public class AToolPetMotionMaker : AnimatablePanel
    {
        #region Public variables
        public Transform characterPos;

        public UIScrollView motionListScrollView;
        public UIScrollView motionTimeBoxScrollView;
        public UIProgressBar motionTimelineProgress;

        public UIButton playButton, pauseButton;

        public AToolPetMotionItem motionItemPrefab;

        public float maxTime = 90.0f;
        #endregion

        #region Properties
        private System.WeakReference _mainPanel = null;
        public AToolPetMotionPanel mainPanel
        {
            get
            {
                if (_mainPanel == null)
                {
                    return null;
                }
                return _mainPanel.Target as AToolPetMotionPanel;
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

        private Dragon _dragon = null;
        public Dragon dragon
        {
            get
            {
                return _dragon;
            }
            set
            {
                if (_dragon != null)
                {
                    Destroy(_dragon.gameObject);
                    _dragon = null;
                }

                _dragon = value;
                _dragon.transform.parent = characterPos;
                _dragon.transform.localPosition = Vector3.zero;
                _dragon.transform.localRotation = Quaternion.identity;
                _dragon.transform.localScale = Vector3.one;
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
                for (int i = 0; i < 4; i++)
                {
                    AToolPetMotionItem item = NGUITools.AddChild(motionListGrid.gameObject, motionItemPrefab.gameObject).GetComponent<AToolPetMotionItem>();
                    item.mainPanel = mainPanel;
                    item.motionName = LocalizationManager.GetData((LocalizationKey)(12105 + i));    // 12105 : 발레, ...
                    item.motionNumber = i + 1;
                    item.Refresh();
                }
                motionListGrid.repositionNow = true;
                motionListGrid.onReposition = () =>
                {
                    motionListScrollView.ResetPosition();
                };
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

            _playing = false;
        }

        public override void Active()
        {
            base.Active();

            playButton.isEnabled = false;
        }

        public void Play(List<int> motions = null, System.Action onFinished = null)
        {
            DragonMotionList motionList = dragon.GetComponent<DragonMotionList>();
            if (motionList == null)
                motionList = dragon.gameObject.AddComponent<DragonMotionList>();
            motionList.Set(motions);
            motionList.onFinished = onFinished;
            motionList.Play();
        }

        public void Pause()
        {
            var animator = dragon.GetComponent<Animator>();
            animator.speed = 0;
        }

        public void Resume()
        {
            var animator = dragon.GetComponent<Animator>();
            animator.speed = 1;
        }

        public void Stop()
        {
            DragonMotionList motionList = dragon.GetComponent<DragonMotionList>();
            if (motionList != null)
            {
                motionList.Stop();
                motionList.onFinished = null;
            }
        }

        public void NextStep()
        {
            mainPanel.NextStep();
        }

        public void RefreshTimeline()
        {
            float value = 1.0f / maxTime;
            float time = 0.0f;

            var items = GetMotionItemsInMotionTimeBox();
            foreach (var item in items)
            {
                time += 20;
            }

            value = value * time;

            motionTimelineProgress.value = value;

            playButton.isEnabled = time > 0.0f;
        }

        public AToolPetMotionItem[] GetMotionItemsInMotionTimeBox()
        {
            List<AToolPetMotionItem> list = new List<AToolPetMotionItem>();

            UIGrid motionTimeBoxGrid = motionTimeBoxScrollView.GetComponentInChildren<UIGrid>();
            List<Transform> childs = motionTimeBoxGrid.GetChildList();
            foreach (Transform t in childs)
            {
                AToolPetMotionItem item = t.GetComponent<AToolPetMotionItem>();
                if (item != null)
                {
                    list.Add(item);
                }
            }

            return list.ToArray();
        }

        public List<int> GetMotionsInMotionTimeBox()
        {
            List<int> motions = new List<int>();
            var items = GetMotionItemsInMotionTimeBox();

            foreach (var item in items)
            {
                motions.Add(item.motionNumber);
            }

            return motions;
        }

        public void AddMotionItemInMotionTimeBox(int motionNumber, string animationName)
        {
            UIGrid grid = motionTimeBoxScrollView.GetComponentInChildren<UIGrid>();
            if (grid != null)
            {
                AToolPetMotionItem item = NGUITools.AddChild(grid.gameObject, motionItemPrefab.gameObject).GetComponent<AToolPetMotionItem>();
                item.mainPanel = mainPanel;
                item.motionName = animationName;
                item.motionNumber = motionNumber;
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
            }
            else
            {
                var list = GetMotionsInMotionTimeBox();
                Play(list, () =>
                {
                    NGUITools.SetActive(playButton.gameObject, true);
                    NGUITools.SetActive(pauseButton.gameObject, false);
                    _playing = false;
                });

                _playing = true;
            }

            Debug.Log("MotionMaker - Play");
        }

        public void Action_Pause()
        {
            NGUITools.SetActive(playButton.gameObject, true);
            NGUITools.SetActive(pauseButton.gameObject, false);

            Pause();

            Debug.Log("MotionMaker - Pause");
        }

        public void Action_Stop()
        {
            NGUITools.SetActive(playButton.gameObject, true);
            NGUITools.SetActive(pauseButton.gameObject, false);

            _playing = false;

            Stop();

            Debug.Log("MotionMaker - Stop");
        }
    }
}