using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] cards;

    // Start is called before the first frame update
    void Start()
    {
        CardSpawn();
    }

    public void CardSpawn()
    {
        GameObject card = Instantiate(cards[Random.Range(0, cards.Length)], gameObject.transform);
        card.transform.DOLocalMoveY(600, 0.5f);
    }
}