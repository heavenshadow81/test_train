using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace ML.PlaywallKids.Aquarium
{
    public class Interaction : MonoBehaviour
    {
        //private int touchCnt;
        private int motionCnt;
        private ButtonUI btnUI;

        //ksy debug
        //private Vector3 lastPosition;

        void DestroyFade()
        {
            iTween.CameraFadeDestroy();
        }

        void Start()
        {
            btnUI = gameObject.GetComponent<ButtonUI>();
        }
        
        void Update()
        {
            //multi-touch input process
            TouchInfo[] touches = CustomInput.touches;
            for (int TouchIndex = 0; TouchIndex < touches.Length; TouchIndex++)
            {
                if (touches[TouchIndex].phase != TouchInfo.Phase.Begin)
                    continue;

                //transmit to buttonUI
                btnUI.TouchButton(touches[TouchIndex].position);

                if (ButtonUI.GetState() != 1)    // 저작모드가 아닐경우에만
                {
                    //게임오브젝트 탐색
                    Ray ray = Camera.main.ScreenPointToRay(touches[TouchIndex].position);

                    RaycastHit hit = new RaycastHit();
                    if (!Physics.Raycast(ray, out hit))
                        continue;

                    //Create A Tool Fishes Action
                    string strCollTag = hit.collider.gameObject.tag;
                    
                    PathExample path = hit.collider.GetComponentInParent<PathExample>();
                    if (path != null)
                    {
                        Vector3 tpos = ray.GetPoint(150);

                        int cmd = Random.Range(1, 5);
                        //cmd = 1;

                        switch (cmd)
                        {
                            case 1:
                                path.FoundIt("RightHand", tpos, strCollTag);
                                break;
                            case 2:
                                path.FoundIt("Heart", tpos, strCollTag);
                                break;
                            case 3:
                                path.FoundIt("TwoHand", tpos, strCollTag);
                                break;
                                /*
                            case 4:
                                path.FoundIt("LeftHand", tpos, strCollTag);
                                break;
                                */
                        }

                        //path.GoPosition(tpos + (Random.insideUnitSphere * 20), "Shake");
                        //path.Come(tpos);
                    }
                }
            }
            
            if (ButtonUI.GetState() == 2)    // 플레이모드일때만
            {
                //모션입력 처리루틴
                //motionCnt = extInput.GetMotionCount();
                motionCnt = 0;
                for (int i = 0; i < motionCnt; i++)
                {
                    MotionInfo motion = new MotionInfo();
                    //motion = CustomInput.GetMotionSeq(i);

                    //Debug.Log("motion=" + motion.motionType + "  " + motion.axisX + "," + motion.axisY);

                    //motion process code here!!!
                    Ray ray = Camera.main.ScreenPointToRay(new Vector3(motion.axisX, motion.axisY, 0));

                    //ksy debug
                    /*
                    Collider[] colliders;
                    Vector3 spherePos = Camera.main.ScreenToWorldPoint(new Vector3(lastPosition.x, lastPosition.y, Camera.main.nearClipPlane+450));
                    colliders = Physics.OverlapSphere(spherePos, 60);
                    */

                    //ksy debug
                    RaycastHit[] hits = Physics.SphereCastAll(ray, 130, 1000);
                    Debug.DrawRay(ray.origin, ray.direction * 700, Color.green, 2, true);
                    //Debug.Log("detect Count=" + hits.Length);
                    foreach (RaycastHit hp in hits)
                    {
                        PathExample path = hp.collider.transform.parent.GetComponent<PathExample>();
                        if (path)
                        {
                            //Debug.Log("detect object=" + hp.collider.name);
                            string strCollTag = hp.collider.gameObject.tag;

                            Vector3 tpos = ray.GetPoint(300);

                            switch (motion.motionType)
                            {
                                case 1:
                                    path.FoundIt("RightHand", tpos, strCollTag);
                                    break;
                                case 2:
                                    path.FoundIt("LeftHand", tpos, strCollTag);
                                    break;
                                case 3:
                                    path.FoundIt("Heart", tpos, strCollTag);
                                    break;
                                case 4:
                                    path.FoundIt("TwoHand", tpos, strCollTag);
                                    break;
                            }

                            StartCoroutine("Back", path);
                        }
                    }
                }
            }
        }

        void OnDrawGizmos()
        {
            //Vector3 p = Camera.main.ScreenToWorldPoint(new Vector3(lastPosition.x, lastPosition.y, Camera.main.nearClipPlane+450));
            //Gizmos.color = Color.red;
            //Gizmos.DrawSphere(p, 60.0F);
            //Gizmos.DrawWireSphere(p, 60.0F);
            //Gizmos.DrawCube(p, new Vector3(70, 70, 250));
            //Gizmos.DrawWireCube(p, new Vector3(70, 70, 250));
        }
    }
}