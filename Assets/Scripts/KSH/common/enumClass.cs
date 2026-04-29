public enum GameState
{
    None = 0,
    GameIntro,
    GameWait,
    GamePlay,
    GameResult
}
public enum GameResult
{
    none, Success, Fail
}

/// <summary>
/// 사파리 9종
/// 아쿠아 8종
/// </summary>
public enum SafariAnimal
{ 
    S1, S2, S3, S4, S5, S6, S7, S8,
    A1, A2, A3, A4, A5, A6, A7, A8
}
public enum SafariBodyKind
{ 
    Head , Body
}
public enum FoodKind
{ 
    Apple =0,
    Kiwi,
    Lemon,
    Watermelon ,
    MAX
}

public enum HumanColor
{
    BROWN, BLACK, WHITE, PINK, PURPLE, YELLOW
}

/// <summary>
/// 동물- 파일이름 동일
/// </summary>
public enum FindStuff
{
    Dolphin1,
    Dolphin2,
    Dolphin3,
    Duck1,
    Duck2,
    Duck3,
    Fish1,Fish2,Fish3,Fish4,Fish5,Fish6,Fish7,Fish8,Fish9,
    Giraffe1,Giraffe2,Giraffe3,
    Hippo1,Hippo2,Hippo3,
    Lion1,Lion2,Lion3,
    Monkey1, Monkey2, Monkey3,
    Penguin1, Penguin2, Penguin3,
    Pig1, Pig2, Pig3,
    Rabbit1, Rabbit2, Rabbit3,
    Shark1, Shark2, Shark3,
    Sheep1, Sheep2, Sheep3,
    Snake1, Snake2, Snake3,
    Whale1, Whale2, Whale3
}

public enum DirType 
{
    Left, 
    Right 
}
public enum DisplayArrowTYPE
{
    LEFT,
    BOTTOM,
    TOP,
    RIGHT,
}
public enum ArrowTYPE
{
    LEFTDOWN,
    BOTTOMDOWN,
    TOPDOWN,
    RIGHTDOWN,
    LEFTUP,
    BOTTOMUP,
    TOPUP,
    RIGHTUP,
}
public enum CONCEPT { Magicland, Zootopia, Attraction }
public enum SceneChangeState
{
    Main, Title, MagicLand, Zootopia, Attraction
}

public enum IntroChange
{
    Main, Title, Game
}

[System.Serializable]
public class EnumClass
{
    public GameState state = GameState.None;    // 현재 상태
    public GameResult resultState = GameResult.none; // 승패 관련
}
