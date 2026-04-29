using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.Interaction
{
    public class InteractionPaintEffect : MonoBehaviour
    {
        public List<SpriteRenderer> listSprite;

        bool bUsed = false;
        bool bFadeOut = false;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

            if (bFadeOut)
            {
                for (int i = 0; i < listSprite.Count; i++)
                    listSprite[i].color -= new Color(0f, 0f, 0f, Time.deltaTime);
                if (listSprite[0].color.a <= 0)
                    UnUsed();
            }
        }

        public void FadeOut()
        {
            bFadeOut = true;
            // gameObject.SetActive(false);
        }

        public void UnUsed()
        {
            bFadeOut = false;
            gameObject.SetActive(false);
        }

        public void SetUsed(bool value)
        {
            bUsed = value;
        }

        public bool IsUseable()
        {
            return bUsed == false;
        }

        public void Setting(InteractionPaintBall paintBall, InteractionPaintBallWall[] arrayWall, int layer)
        {
            bUsed = true;
            gameObject.SetActive(true);
            for (int i = 0; i < listSprite.Count; i++)
                listSprite[i].transform.parent.gameObject.SetActive(false);

            transform.localPosition = paintBall.transform.localPosition;

            int randomIndex = Random.Range(0, 4);
            Sprite createSprite = Sprite.Create(listSprite[0].sprite.texture, _GetRect(randomIndex), Vector2.one * 0.5f);
            for (int i = 0; i < arrayWall.Length; i++)
            {
                Vector vector = _GetWallColliderDistance(arrayWall[i]);
                if (vector != null)
                {
                    SpriteRenderer sprite = _GetSprite();
                    if (sprite != null)
                    {
                        sprite.transform.parent.gameObject.SetActive(true);

                        sprite.sprite = createSprite;
                        sprite.color = paintBall.GetColor();

                        sprite.sortingOrder = layer;

                        sprite.transform.parent.localPosition = vector.GetVector3();
                        sprite.transform.parent.localRotation = arrayWall[i].transform.localRotation;

                        UVTextureAnimator effect = sprite.GetComponentInChildren<UVTextureAnimator>();
                        effect.GetComponent<Renderer>().sortingOrder = sprite.sortingOrder + 2;
                    }
                }
            }
        }

        Vector _GetWallColliderDistance(InteractionPaintBallWall wall)
        {
            float SPRITE_SIZE = listSprite[0].transform.localScale.x * 0.5f;

            Vector3 checkPosition = transform.position + (wall.GetNavigate() * SPRITE_SIZE);
            if (wall.GetComponent<Collider>().bounds.Contains(checkPosition))
            {
                Vector3 thisPos = transform.position;
                Vector3 wallPos = wall.transform.position;

                float wallWeight = wall.transform.localScale.z * 0.5f;
                switch (wall.type)
                {
                    case InteractionPaintBallWall.WallType.LEFT: return new Vector((wallPos.x + wallWeight) - thisPos.x, 0, 0);
                    case InteractionPaintBallWall.WallType.RIGHT: return new Vector((wallPos.x - wallWeight) - thisPos.x, 0, 0);
                    case InteractionPaintBallWall.WallType.FRONT: return new Vector(0, 0, (wallPos.z - wallWeight) - thisPos.z);
                    case InteractionPaintBallWall.WallType.BOTTOM: return new Vector(0, (wallPos.y + wallWeight) - thisPos.y, 0);
                }
            }
            return null;
        }

        SpriteRenderer _GetSprite()
        {
            for (int i = 0; i < listSprite.Count; i++)
                if (listSprite[i].gameObject.activeInHierarchy == false)
                    return listSprite[i];

            Debug.Log("Empty Sprite");
            return null;
        }

        Rect _GetRect(int index)
        {
            switch (index)
            {
                case 0: return new Rect(0, 0, 256, 256);
                case 1: return new Rect(0, 256, 256, 256);
                case 2: return new Rect(256, 0, 256, 256);
                case 3: return new Rect(256, 256, 256, 256);
            }
            return new Rect();
        }

        // Is Vector3 NullAble Class
        class Vector
        {
            Vector3 vec;

            public Vector(float x, float y, float z)
            {
                int powNum = 3;
                vec = new Vector3(GetRound(x, powNum), GetRound(y, powNum), GetRound(z, powNum));
            }

            public float x
            {
                get
                {
                    return vec.x;
                }
            }
            public float y
            {
                get
                {
                    return vec.y;
                }
            }
            public float z
            {
                get
                {
                    return vec.z;
                }
            }

            public Vector3 GetVector3()
            {
                // *0.1f Because since the overlapping walls and sprites
                return vec - (vec.normalized * 0.1f);
            }

            public float GetDistance()
            {
                return GetVector3().magnitude;
            }

            float GetRound(float round, int pow)
            {
                int powNum = (int)Mathf.Pow(10, pow);
                round = round * powNum;
                return round / powNum;
            }
        }
    }
}