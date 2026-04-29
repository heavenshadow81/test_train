using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    public class DragonFruit : MonoBehaviour
    {
        #region Public variables
        public Dragon dragon;
        public AudioClip poopSound;
        public int userId = 0;
        public bool showMenuAfterEat;
        #endregion

        #region Private variables
        private static Dictionary<int, DragonFruit> _currentFruitDict = new Dictionary<int, DragonFruit>();
        private DragonComeToFront _comeToFront = null;
        private bool _needToCome = false;
        private bool _eat = false;

        private const int JUMP_MAX = 3;
        private int _jumpCnt = 0;
        #endregion

        #region Unity methods
        void Start()
        {
            _jumpCnt = 0;
            _currentFruitDict[userId] = this;

            if (GetComponent<Rigidbody>() == null)
            {
                gameObject.AddComponent<Rigidbody>();
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
                GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }

            Collider fruitCollider = GetComponent<Collider>();
            if (fruitCollider == null)
            {
                SphereCollider c = gameObject.AddComponent<SphereCollider>();
                c.radius = 0.15f;
                fruitCollider = c;
            }

            if (dragon != null)
            {
                _comeToFront = dragon.GetComponent<DragonComeToFront>();
                if (_comeToFront == null)
                {
                    _comeToFront = dragon.gameObject.AddComponent<DragonComeToFront>();
                    _comeToFront.userId = userId;
                }

                transform.position = _GetFruitPosition() + Vector3.up * 8.0f;
                transform.localRotation = Quaternion.Euler(0, 270, 0);
                Vector3 localScale = transform.localScale;
                localScale *= 0.75f;
                transform.localScale = localScale;

                // ignore the dragon's collider.
                var dragonColliders = dragon.GetComponentsInChildren<Collider>();
                foreach (var dc in dragonColliders)
                    Physics.IgnoreCollision(fruitCollider, dc, true);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        void Update()
        {
            if (_needToCome && _comeToFront != null)
            {
                if (_comeToFront.isComing == false && _comeToFront.came == false)
                {
                    _comeToFront.Come(DragonComeToFront.ComeReason.Eat);
                }
                else if (_comeToFront.came)
                {
                    if (_eat == false)
                    {
                        _eat = true;
                        dragon.dragonAnimation.Eat(gameObject);
                    }
                }
            }
        }

        private Vector3 _GetFruitPosition()
        {
            // default position
            Vector3 pos = new Vector3(19.5f, 0.0f, -4.5f + userId * 2.0f);
            if (_comeToFront != null)
            {
                pos = DragonComeToFront.GetDummyPosition("fruit", userId);
                if (pos == Vector3.zero)
                {
                    pos = _comeToFront.GetFrontPosition() + new Vector3(0.5f, 0.0f, 0.0f);
                }
            }
            else
            {
                pos = new Vector3(19.5f, 0.0f, -4.5f + userId * 2.0f);
            }

            return pos;
        }

        public void OnCollisionEnter(Collision collision)
        {
            // On Landing on Ground
            //collider.enabled = false;
            //rigidbody.useGravity = false;
            //rigidbody.isKinematic = true;

            if (poopSound != null)
            {
                AudioSource.PlayClipAtPoint(poopSound, Camera.main.transform.position);
            }

            if (_jumpCnt < JUMP_MAX)
            {
                GetComponent<Rigidbody>().AddForce(Vector3.up * (JUMP_MAX - _jumpCnt) * 75f);
                _jumpCnt++;

            }
            else
            {
                _needToCome = true;
            }
        }

        public void OnDestroy()
        {
            if (_comeToFront != null)
            {
                if (showMenuAfterEat)
                {
                    if (MenuControl.sharedInstance != null)
                        MenuControl.sharedInstance.ShowPetMenu(userId);
                    else
                        _comeToFront.Back();
                }
            }
            if (_currentFruitDict.ContainsKey(userId))
                _currentFruitDict.Remove(userId);
        }
        #endregion

        #region -
        public static DragonFruit GetFruit(int userId)
        {
            DragonFruit fruit = null;
            if (_currentFruitDict.ContainsKey(userId))
                fruit = _currentFruitDict[userId];
            return fruit;
        }
        #endregion
    }
}