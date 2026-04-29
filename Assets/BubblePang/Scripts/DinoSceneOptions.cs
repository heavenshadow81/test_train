using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DinoSceneOptions
{
    static Theme theme;
    //ƒ‹≈Ÿ√˜ ≈◊∏∂
    public static Theme GetTheme() => theme;

    static BubblePang.ContentsType type;
    //ƒ‹≈Ÿ√˜ ≈∏¿‘
    public static BubblePang.ContentsType GetContentsType() => type;
    //∆ƒ∂ÛπÃ≈Õ ¡ˆ¡§...

    static Difficulty difficulty;
    // ƒ‹≈Ÿ√˜ ≥≠¿Ãµµ
    public static Difficulty GetDifficulty() => difficulty;

    static int persons;
    // ƒ‹≈Ÿ√˜ ¿Œø¯ºˆ

    public static int GetPersons() => persons;

    public static void SetParameter(int tpe)
    {
        type = (BubblePang.ContentsType)tpe;
    }

    public static void SetTheme(int them)
    {
        theme = (Theme)them;
    }

    // ø…º« º±≈√ »ƒ »Æ¿Œ ¥©∏¶∂ß 
    public static void RetrySaveParameter(BubblePang.ContentsType tpe, Difficulty diff, int person, Theme them)
    {
        type = tpe;

        difficulty = diff;

        persons = person;

        theme = them;
    }
}
