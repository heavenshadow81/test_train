using UnityEngine;

public abstract class UITimeDisplay : MonoBehaviour
{
    public bool Active
    {
        set
        {
            gameObject.SetActive(value);
        }

        get
        {
            return gameObject.activeInHierarchy;
        }
    }
    public abstract void InitTime(int _iTime);
    public abstract void ChangeTime(int _iTime);
}

public abstract class ComboDisplay : MonoBehaviour
{
    public bool Active
    {
        get
        {
            return gameObject.activeInHierarchy;
        }
        set
        {
            gameObject.SetActive(value);
        }
    }
    public abstract void DisplayScore(Vector3 pos, int num); 
}