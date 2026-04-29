using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

namespace ShapeMatching
{
    public class GameManager : TouchManager_3DTouch
    {
        public GameObject endImage;
        public GameObject notice;
        public GameObject[] shapes;
        bool selectShape;
        int shapeNumber;

        GameObject currentShape;
        bool checkBox;
        bool shape;

        // Start is called before the first frame update
        void OnEnable()
        {
            Physics.gravity = new Vector3(0, -40.0f, 0);
            selectShape = false;
            checkBox = false;  
        }


        private void FixedUpdate()
        {
            if (CheckBox.end)
            {
                Invoke("ShowEnd", 1);
            }
        }

        public override void HandleInput(Vector2 pos)
        {
            isTouchable = true;

            Ray ray = Camera.main.ScreenPointToRay(pos); // 2D 화면 좌표를 3D Ray로 변환
            RaycastHit hit;

            // Raycast로 오브젝트 감지
            if (Physics.Raycast(ray, out hit))
            {
                if (!selectShape)
                {
                    // 클릭된 오브젝트의 이름 가져오기
                    if (hit.transform.tag == "Shape")
                    {
                        shape = true;
                        selectShape = true;

                        SoundMGR.Instance.SoundPlay("ShapeMatching_도형선택");

                        currentShape = hit.transform.gameObject;
                        Vector3 currentPostion = hit.transform.position;

                        //print(hit.transform.name);

                        hit.transform.DOMoveY(50, 2).OnComplete(() =>
                        {
                            shape = false;
                            currentShape.SetActive(false);       
                            currentShape.transform.position = currentPostion;

                            ShowShape();
                        });
                        
                        shapeNumber = int.Parse(hit.transform.name) - 1;
                        notice.transform.DOScale(7, 1);
                    }
                }
                else if (selectShape)
                {
                    if (hit.transform.name == "CheckBox")
                    {
                        if (!checkBox)
                        {
                            shape = true;

                            //print(hit.transform.parent);
                            GameObject newShape = Instantiate(shapes[shapeNumber], hit.transform.parent);
                            newShape.transform.position = hit.transform.parent.position;
                            checkBox = true;
                            notice.transform.DOScale(0, 1);
             
                            //Invoke("ShowShape", 2);
                        }
                    }
                    
                    if(hit.transform.tag == "Shape")
                    {
                        if (!shape)
                        {
                            shape = true;

                            currentShape.SetActive(true);
                            GameObject newShape = Instantiate(effect[0], currentShape.transform);
                            newShape.transform.position = currentShape.transform.position;
                            Destroy(newShape, 2f);

                            Vector3 currentPostion = hit.transform.position;

                            SoundMGR.Instance.SoundPlay("ShapeMatching_도형선택");
                            hit.transform.DOMoveY(50, 2).OnComplete(() =>
                            {
                                hit.transform.gameObject.SetActive(false);
                                hit.transform.position = currentPostion;
                                shape = false;
                            });

                            shapeNumber = int.Parse(hit.transform.name) - 1;
                            currentShape = hit.transform.gameObject;
                        }
                    }
                }
            }
            else
            {
                print("오브젝트 감지 안됨");
                isTouchable = true;
            }     
        }

        void ShowEnd()
        {
            endImage.SetActive(true);
        }

        void ShowShape()
        {
            SoundMGR.Instance.SoundPlay("ShapeMatching_도형생성");

            GameObject newShape = Instantiate(effect[0], currentShape.transform);
            newShape.transform.position = currentShape.transform.position;
            Destroy(newShape, 2f);
            currentShape.SetActive(true);
            selectShape = false;
            checkBox = false;
        }
    }
}