using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    public class FreeDrawingCarBone : FreeDrawingObjectBone
    {
        #region Public variables
        public GameObject goSeats;
        #endregion

        #region Properties
        public override FreeDrawingObjectType objectType
        {
            get
            {
                return FreeDrawingObjectType.Car;
            }
        }
        #endregion

        #region Constants
        public const string kBodyMomentumBone = "body_momentum";
        public const string kWheelFLBone = "wheel_fl";
        public const string kWheelFRBone = "wheel_fr";
        public const string kWheelRLBone = "wheel_rl";
        public const string kWheelRRBone = "wheel_rr";
        #endregion

        public override void Start()
        {
            base.Start();
            goSeats.SetActive(false);
        }

        public void SetDragon(int userID)
        {
            FreeDrawingAnimationControl freeDrawingControl = GetComponent<FreeDrawingAnimationControl>();
            if (freeDrawingControl != null)
                freeDrawingControl.movesAlongPath = false;

            DragonAnimationControl dragon = CharacterManager.GetMakeObject(userID);
            DragonComeToFront come = dragon.GetComponent<DragonComeToFront>();

            if (come == null)
                come = dragon.gameObject.AddComponent<DragonComeToFront>();

            come.userId = userID;
            come.Come(DragonComeToFront.ComeReason.Ride, RideDragon);
        }

        public void RideDragon(DragonAnimationControl control)
        {
            goSeats.SetActive(true);

            // move false
            control.usesNavMesh = false;

            // look dummy false
            DragonComeToFront come = control.GetComponent<DragonComeToFront>();
            if (come != null)
                come.enabled = false;

            // look path false
            control.enabled = false;

            StartCoroutine(Init(control));
        }

        IEnumerator Init(DragonAnimationControl control)
        {
            yield return new WaitForEndOfFrame();

            control.transform.parent = goSeats.transform;
            control.transform.localPosition = Vector3.zero;
            control.transform.localScale = Vector3.one;
            control.transform.localRotation = Quaternion.identity;

            control.Sit();

            MeshRenderer mesh = null;
            MeshRenderer[] listMesh = transform.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < listMesh.Length; i++)
                if (listMesh[i].name.Equals(kBodyMomentumBone))
                    mesh = listMesh[i];

            if (mesh != null)
            {
                Destroy(GetComponent<BoxCollider>());
                mesh.gameObject.AddComponent<MeshCollider>();
                goSeats.transform.position = new Vector3(goSeats.transform.position.x, 10, goSeats.transform.position.z);

                while (true)
                {
                    Vector3 from = goSeats.transform.position;
                    RaycastHit hitInfo;

                    if (Physics.Raycast(from, Physics.gravity, out hitInfo))
                    {
                        Vector3 to = hitInfo.point;

                        Vector3 dist = Vector3.Min(Vector3.down * Time.deltaTime, (to - from) * 0.5f * Time.deltaTime);

                        goSeats.transform.position += dist;

                        if ((from - to).sqrMagnitude < 0.01f)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(1.5f);

            FreeDrawingAnimationControl freeDrawingControl = GetComponent<FreeDrawingAnimationControl>();
            if (freeDrawingControl != null)
                freeDrawingControl.comeToFront.Back();

            yield return new WaitForSeconds(30f);

            TweenScale.Begin(gameObject, 0.15f, Vector3.zero);

            control.effect.Pop();
            Destroy(gameObject, 1f);
        }

        protected override Transform _GetRootBone()
        {
            Transform rootBone = transform;
            return rootBone;
        }

        protected override Dictionary<string, Transform> _GetBoneDict()
        {
            Dictionary<string, Transform> boneDict = new Dictionary<string, Transform>();

            Transform[] hierarchy = _GetRootBone().GetComponentsInChildren<Transform>();

            foreach (Transform t in hierarchy)
            {
                string boneName = "";

                switch (t.name)
                {
                    case "Dummy_Body_Momentum":
                        boneName = kBodyMomentumBone;
                        break;
                    case "Dummy_Wheel_FL":
                        boneName = kWheelFLBone;
                        break;
                    case "Dummy_Wheel_FR":
                        boneName = kWheelFRBone;
                        break;
                    case "Dummy_Wheel_RL":
                        boneName = kWheelRLBone;
                        break;
                    case "Dummy_Wheel_RR":
                        boneName = kWheelRRBone;
                        break;
                    case "Dummy_Car_Eyes_L":
                        boneName = kEyeLBone;
                        break;
                    case "Dummy_Car_Eyes_R":
                        boneName = kEyeRBone;
                        break;
                }

                if (!string.IsNullOrEmpty(boneName))
                {
                    boneDict[boneName] = t;
                }
            }

            return boneDict;
        }
    }
}