using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.EAS
{
    public class EASRemoteKinectPanel : EASAnimatablePanel
    {
        public AvatarSkeleton avatar1Prefab;
        public AvatarSkeleton avatar2Prefab;

        private AvatarSkeleton _avatar1, _avatar2;

        public override void BeginShow()
        {
            base.BeginShow();

            avatar1Prefab.gameObject.SetActive(false);
            avatar2Prefab.gameObject.SetActive(false);

            _avatar1 = (AvatarSkeleton)Instantiate(avatar1Prefab, avatar1Prefab.transform.position, avatar1Prefab.transform.rotation);
            _avatar1.gameObject.SetActive(true);
            _avatar1.transform.parent = avatar1Prefab.transform.parent;
            _avatar1.transform.localScale = avatar1Prefab.transform.localScale;
            _avatar2 = (AvatarSkeleton)Instantiate(avatar2Prefab, avatar2Prefab.transform.position, avatar2Prefab.transform.rotation);
            _avatar2.gameObject.SetActive(true);
            _avatar2.transform.parent = avatar2Prefab.transform.parent;
            _avatar2.transform.localScale = avatar2Prefab.transform.localScale;

            var playwall = EASServerPlaywallManager.sharedInstance;
            playwall.user1Skeleton = _avatar1;
            playwall.user2Skeleton = _avatar2;
        }

        public override void Deactive()
        {
            if (_avatar1 != null)
            {
                Destroy(_avatar1.gameObject);
                _avatar1 = null;
            }

            if (_avatar2 != null)
            {
                Destroy(_avatar2.gameObject);
                _avatar2 = null;
            }

            var playwall = EASServerPlaywallManager.sharedInstance;
            playwall.user1Skeleton = null;
            playwall.user2Skeleton = null;

            base.Deactive();
        }
    }
}