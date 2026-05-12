using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using System.Collections;

public class TableWare_LT : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform originTransform;
    [SerializeField] private Transform foodTransform;

    private float moveDuration = 0.3f;
    private Tweener moveTween;

    public Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;
    private RectTransform rect;

    [SerializeField] private PlayManager_LT playManager;

    private Image tableWareImg;
    [SerializeField] private Sprite[] tableWares = null;
    public Sprite selectedSprite = null;

    [SerializeField] Food_LT food = null;

    public bool hasCollision = false;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        tableWareImg = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        rigidBody.gravityScale = 0;
        rigidBody.velocity = Vector2.zero; // Ȥ�� �������� �̵� �ӵ��� �ʱ�ȭ
        rigidBody.simulated = true; // Rigidbody2D�� Ȱ��ȭ
        boxCollider.enabled = true; // Collider2D�� Ȱ��ȭ
        food.OnFoodChange += Init;
        food.OnFoodRotate += MoveTargetPos;
        Init();
    }

    private void OnDisable()
    {
        rigidBody.simulated = false; // Rigidbody2D ��Ȱ��ȭ
        boxCollider.enabled = false; // Collider2D ��Ȱ��ȭ
        food.OnFoodChange -= Init;
        food.OnFoodRotate -= MoveTargetPos;
    }

    private void Init()
    {
        ResetPos();
        SettingImage();
        MoveTargetPos();
    }

    private void SettingImage()
    {
        int randomNum = UnityEngine.Random.Range(0, tableWares.Length);
        selectedSprite = tableWares[randomNum];

        tableWareImg.sprite = selectedSprite;
    }

    public void MoveTargetPos()
    {
        transform.DOLocalMoveY(targetTransform.localPosition.y, 1f).OnComplete(() =>
        {
            playManager.SetTouchable();
        });
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasCollision) return;

        if (collision.gameObject.CompareTag("Keeper"))
        {
            hasCollision = true;
            StopShootTableWare();
            PlayBounceAnimation(collision);
        }
    }

    public void ShootTableWare()
    {
        moveTween = transform.DOMoveY(foodTransform.position.y, moveDuration)
                            .SetEase(Ease.Linear);
    }

    public void StopShootTableWare()
    {
        if (moveTween != null && moveTween.IsPlaying())
        {
            moveTween.Kill();
        }
    }

    private void PlayBounceAnimation(Collision2D collision)
    {
        if (tableWareImg.sprite.name.Contains("Chopsticks"))
        {
            SoundMGR.Instance.SoundPlay("PlayGround_WrongAnswer");
        }
        else
        {
            SoundMGR.Instance.SoundPlay("PlayGround_Clinking");
        }

        transform.DOLocalMoveY(transform.localPosition.y - rect.rect.height, moveDuration / 2)
                 .SetEase(Ease.OutQuad);

        float rotationAngle = 360f;
        transform.DORotate(new Vector3(0, 0, rotationAngle), moveDuration, RotateMode.FastBeyond360)
                 .SetEase(Ease.OutQuad);

        tableWareImg.DOFade(0, moveDuration);

        DOVirtual.DelayedCall(moveDuration, () =>
        {
            ResetPos();
            MoveTargetPos();
        });
    }

    public void ResetPos()
    {
        if (moveTween != null && moveTween.IsPlaying())
        {
            moveTween.Kill();
        }

        transform.position = originTransform.position; // ���� ��ġ�� ����
        Color color = tableWareImg.color;
        color.a = 1f; // ���� ���� 1�� ����
        tableWareImg.color = color; // ������ ���� ���� �ٽ� ����
        hasCollision = false;
    }
}
