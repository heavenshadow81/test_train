using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FC_BuildingAnimation : MonoBehaviour
{
    [SerializeField] FC_GameManager gameManager;
    [SerializeField] GameObject spawner;
    [SerializeField] GameObject building;

    public void AnimOff()
    {
        gameManager.TouchOn();
    }

    public void SpawnOn()
    {
        spawner.SetActive(true);
    }

    public void BuildingOff()
    {
        building.SetActive(false);
    }
}
