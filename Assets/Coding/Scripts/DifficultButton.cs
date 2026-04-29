using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultButton : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Dropdown difficult;
    private void OnEnable()
    {
        difficult.value = (int)ContentsOptions.GetDifficult();
    }
}
