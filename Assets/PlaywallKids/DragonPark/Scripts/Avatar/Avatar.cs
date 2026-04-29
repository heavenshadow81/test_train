using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    public class Avatar : BoneObject
    {
        private AvatarSkeleton _skeleton = null;
        public AvatarSkeleton skeleton
        {
            get
            {
                if (_skeleton == null)
                {
                    _skeleton = GetComponent<AvatarSkeleton>();
                    if (_skeleton == null)
                    {
                        _skeleton = gameObject.AddComponent<AvatarSkeleton>();
                    }
                }
                return _skeleton;
            }
        }

        public override void OnDestroy()
        {
            //Texture2D headImage = GetHeadImage();
            //SetHeadImage(null);

            //if(headImage != null) {
            //	Destroy (headImage);
            //}

            base.OnDestroy();
        }

        public Texture2D GetHeadImage()
        {
            Texture2D texture = null;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.name.ToLower().Contains("hair") ||
                   child.name.ToLower().Contains("head"))
                {
                    MeshRenderer[] mrs = child.GetComponentsInChildren<MeshRenderer>(true);
                    if (mrs != null && mrs.Length > 0)
                    {
                        if (mrs[0].material != null)
                        {
                            texture = (Texture2D)mrs[0].material.mainTexture;
                        }
                    }
                    else
                    {
                        SkinnedMeshRenderer[] smrs = child.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                        if (smrs != null && smrs.Length > 0)
                        {
                            if (smrs[0].material != null)
                            {
                                texture = (Texture2D)smrs[0].material.mainTexture;
                            }
                        }
                    }

                    if (texture != null)
                    {
                        break;
                    }
                }
            }
            return texture;
        }

        public void SetHeadImage(Texture2D texture)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.name.ToLower().Contains("hair") ||
                   child.name.ToLower().Contains("head"))
                {
                    MeshRenderer[] mrs = child.GetComponentsInChildren<MeshRenderer>(true);
                    if (mrs != null && mrs.Length > 0)
                    {
                        if (mrs[0].material != null)
                        {
                            mrs[0].material.mainTexture = texture;
                            break;
                        }
                    }
                    else
                    {
                        SkinnedMeshRenderer[] smrs = child.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                        if (smrs != null && smrs.Length > 0)
                        {
                            if (smrs[0].material != null)
                            {
                                smrs[0].material.mainTexture = texture;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}