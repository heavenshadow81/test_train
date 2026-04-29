using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager_CA : MonoBehaviour
{
    public static GameManager_CA instance;

    public int clear;
    public TextMeshProUGUI clearText;

    public int correct;
    public TextMeshProUGUI correctText;

    public List<int> input;
    public bool result;

    public TextMeshProUGUI arithmeticText;

    private void Awake()
    {
        if (instance == null)
        instance = this;
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        clear = 10;
        clearText.text = clear.ToString();

        correct = Random.Range(2, 19);
        correctText.text = correct.ToString();
    }

    public void CheckResult()
    {
        if (input.Count==2)
        {
            if (input[0] + input[1] == correct)
            {
                result = true;
                clear--;
                print(result);
            }
            else
            {          
                result = false;
                print(result);
            }
        }
    }

    public void lnit()
    {      
        correctText.transform.DOScale(0, 0.3f).OnComplete(() =>
        {
            correct = Random.Range(2, 19);
            correctText.text = correct.ToString();
            correctText.transform.DOScale(1, 0.3f);
        });
        
        input.Clear();
    }
}
