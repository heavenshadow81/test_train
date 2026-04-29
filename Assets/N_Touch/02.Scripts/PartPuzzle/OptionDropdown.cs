using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
public enum Property
{
    Difficult,
    Player
}
public class OptionDropdown : MonoBehaviour
{
    public Property Prop;
    [SerializeField]
    Dropdown drop;
    void OnEnable()
    {
        print(Prop);
        switch (Prop)
        {
            case Property.Difficult:
                print((int)TouchContentsManger.Instance.Difficulty);
                
                drop.value = (int)TouchContentsManger.Instance.Difficulty;
                
                break;
            case Property.Player:
                print(TouchContentsManger.Instance.Players);
                drop.value = TouchContentsManger.Instance.Players-1;
                
                break;
        }
        print(drop.value);
    }
}
