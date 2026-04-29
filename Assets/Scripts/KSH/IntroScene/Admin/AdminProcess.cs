using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminProcess : IntroProcess
{
    public override void Awake()
    {
        base.Awake();
        //Cursor.visible = false;
    }

    public override void Start()
    {
        base.Start();

        MultyDisplay();
    }

    public void MultyDisplay()
    {
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate(Screen.width, Screen.height, new RefreshRate
            {
                numerator = 60,
                denominator = 1
            });
        }
    }

    public override void Prev()
    {
        base.Prev();
    }


    public override void Next()
    {
        base.Next();
    }

    public override void TitleJoinButton(int idx)
    {
        base.TitleJoinButton(idx);
    }
}
