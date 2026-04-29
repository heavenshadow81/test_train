//콘텐츠 모드 설정...버튼 같은 곳에 사용됨<>UIButtonPrefab이나 버튼을 호출하는 함수 등등에 사용되는 enum
public enum BigboardContentMode : int
{
    None = 0,

    Drawing2D_Aircap = 1,
    Drawing2D_Note = 2,
    Drawing2D_SandDraw = 3,
    Drawing2D_SandPrint = 4,
    Drawing2D_WindowDraw = 5,
    Drawing2D_HandPrint = 6,
    Drawing2D_FruitPrint = 7,
    Drawing2D_NeonDraw = 8,

    Drawing3D_SketchBook = 9,
    Drawing3D_Dragon = 10,
    Drawing3D_FreeDrawing = 11,
    Drawing3D_PetMotion = 12,
    //Drawing3D_Balloon = 13,

    Touch_Slime = 14,
    Touch_AlphabetGame = 15,

    Interaction_Heart = 16,
    Interaction_Lamplight = 17,
    Interaction_Dish = 18,
    Interaction_Paints = 19,
    Interaction_Ball = 20,

    Motion_JumpGame = 21,
    Motion_Weightlessness = 22,

    Aquarium_Fish = 23,
    Aquarium_Interaction = 24,

    Interaction_SkatingFlag = 25,
    Interaction_SkatingLandmark = 26,
    Interaction_SkatingCloth = 27,
    Interaction_Travel = 28,

    Touch_Soccer = 29,
    Stadium = 30,
    Touch_Basketball = 31,
    Touch_Volleyball = 32,
    Touch_Handball = 33,
    Touch_Run = 34,
    Touch_Jump = 35,
    Touch_Shot = 36,
    Touch_Dodgeball = 37,
    Touch_Archery = 38
}