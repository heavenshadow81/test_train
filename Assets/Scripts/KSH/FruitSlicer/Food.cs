using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Unity.VisualScripting;

namespace Bax.P0.Client.UnityWorld.SlicerGame
{
    public class Food : MonoBehaviour
    {
        //���������� ��� �ִ� ������
        public FoodKind kind;
        
        public bool isDown = false;

        //������
        public SpriteRenderer spriteRenderer;
        //���ϸ��� ũ�Ⱑ �������̶� �ö��̴��� ������  (4��)
        public PolygonCollider2D[] polygonColliders;

        //Ǯ�� �������̽�
        public IObjectPool<Food> Ipool;

        //������ ��ġ
        [NonSerialized] public Vector2 createPos;

        //�̵����ǵ�
        public float moveSpeed;

        private void colliderActive(int idx)
        {
            //��� �ö��̴��� ��Ȱ��ȭ
            foreach (var item in polygonColliders) item.gameObject.SetActive(false);
            //�ش��ϴ� ���� ����
            kind = (FoodKind)idx;
            //�ش��ϴ� ������ �ö��̴� Ȱ��ȭ
            polygonColliders[idx].gameObject.SetActive(true);
        }
        public void SpriteSetting(int idx)
        {
            //�ѹ��� �ش��ϴ� ����ũ�⸸ŭ�� �ö��̴� Ȱ��ȭ
            colliderActive(idx);
            //�ش��ϴ� ���� �̹��� �ε�
            foodSprite(spriteRenderer, string.Empty).Forget();
        }
        //�����̹��� �ε�
        private async UniTask foodSprite(SpriteRenderer renderer, string piece = "Piece")
        {
            //�ش��ϴ� �����̸� + piece
            //�Ķ���Ͱ� Empty = ��������
            //�Ķ���Ͱ� Piece = �ɰ�������
            string name = kind.ToString() + piece;
            //�̹��� �ε�
            await SlicerMgr.instance.loadSprite.LoadSpriteData(name, renderer);
            await UniTask.Yield(SlicerMgr.instance._sources.Token);
        }

        //������ �ɰ��ٸ� 
        public void AddScoreText(float score)
        {
            //�ؽ�Ʈ �α� ����
            var log = SlicerMgr.instance.logManager.GetLog();
            //�ؽ�Ʈ ũ�� ���� ����
            RectTransform rect = (RectTransform)log.LogText.transform;
            rect.rect.Set(0, 0, 100, 100);

            //�ɰ� ���Ͽ� ���� ���;��� �ؽ�Ʈ ��ġ ����
            switch (kind)
            {
                case FoodKind.Apple:
                    log.transform.position = SlicerMgr.instance.appleText.transform.position;
                    break;
                case FoodKind.Kiwi:
                    log.transform.position = SlicerMgr.instance.kiwiText.transform.position;
                    break;
                case FoodKind.Lemon:
                    log.transform.position = SlicerMgr.instance.orangeText.transform.position;
                    break;
                case FoodKind.Watermelon:
                    log.transform.position = SlicerMgr.instance.watermelonText.transform.position;
                    break;
            }
            //��Ʈ ũ��
            log.LogText.fontSize = 60f;
            //��Ʈ ����
            log.LogText.color = new Color(1, 1, 1, 1);
            //�ؽ�Ʈ 
            log.LogText.text = $"+{score}";


            //���� �� 0.6��ŭ Ʈ����Ű��
            log.transform.DOMove(log.transform.position + Vector3.up * 0.6f, 0.5f).
            OnComplete(() =>
            {
                //���Ž�Ŵ
                SlicerMgr.instance.logManager.SetLog(log);
            }).WithCancellation(cancellationToken: SlicerMgr.instance._sources.Token);
        }

       

        //�ɰ��ٸ� �ɰ� �̹��� ����
        public async void CreateClone(bool Flip, Vector2 dir, float AddForcePower)
        {
            //�̵��� ���߰�
            moveSpeed = 0;
            //����ӿ�����Ʈ ����
            var clone = new GameObject();
            //������Ʈ �߰�
            var foodClone = clone.AddComponent<FoodClone>();
            //������Ʈ�ȿ� ������ ĳ��
            foodClone.spriteRenderer = foodClone.AddComponent<SpriteRenderer>();
            foodClone.rid2D = foodClone.AddComponent<Rigidbody2D>();

            //��ġ ���� (������ġ)
            foodClone.transform.position = transform.position;
            foodClone.transform.rotation = transform.rotation;

            //�ɰ� ���� �̹��� �ε�
            foodSprite(foodClone.spriteRenderer).Forget();
            //�̹��� ����
            foodClone.spriteRenderer.flipX = Flip;
            //�̹��� ����ũ ����
            foodClone.spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            //������ �������� �ֵ��� 1�� ����
            foodClone.rid2D.gravityScale = 1;
            //gravity �� �����ϰ� 0���� ���� ��
            foodClone.rid2D.velocity = Vector2.zero;
            //�Ķ���Ϳ��� ���޹��� �������� ����
            foodClone.rid2D.AddForce(dir * AddForcePower, ForceMode2D.Impulse);

            await UniTask.Yield(SlicerMgr.instance._sources.Token);
            //foodPlaceList.Add(clone);
        }

        //���� ���
        public float Dir(Vector2 pos)
        {
            Vector2 dir = pos - (Vector2)transform.position;
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }


        private void Update()
        {
            if (SlicerMgr.instance.stateClass.state == GameState.GamePlay && gameObject.activeSelf)
            {
                //�̵�
                transform.position += transform.right * Time.deltaTime * moveSpeed;

                //������ ��ġ���� �Ÿ� ��� 100�� �Ѵ´ٸ� (�Ϻη� ũ������)
                if ((Vector2.Distance(transform.position, createPos) >= 100f))
                {
                    ReturnObject();
                }
            }
        }


        /// <summary>
        /// ��ũ��ȭ�� ������ �������� ���� �ؼ� Pulling �� �������.
        /// </summary>
        public void ReturnObject()
        {
            SlicerMgr.instance.foodPulling.Release(this);
            SlicerMgr.instance.foodList.Remove(this);
            spriteRenderer.sprite = null;
        }
    }
}
