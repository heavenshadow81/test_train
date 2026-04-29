using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpriteData" , menuName = "Scriptable/SpriteData")]
public class spriteData : ScriptableObject
{
    [Header("그림맞추기 sprites 완성본")]
    public List<Sprite> safariSpriteList = new List<Sprite>();

    [Header("풍선 Sprites")]
    public List<Sprite> balloonSpriteList = new List<Sprite>();

    [Header("풍선 화살통 잔여 Sprite")]
    public Sprite ArrowLeft;
    public Sprite ArrowDonLeft;

    [Header("발판 Down")]
    public Sprite footDown;
    [Header("발판 Down")]
    public Sprite footUp;





    [Header("MerryGoSprites")]
    public Sprite LowerHorse;
    public Sprite UpperHorse;

    public Sprite ForHorse;
    public Sprite LeftRotHorse;
    public Sprite RightRotHorse;
    public Sprite BackHorse;
}
