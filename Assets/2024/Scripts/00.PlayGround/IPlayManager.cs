using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayManager
{
    void HandleInput(Vector2 inputPosition);
    void CorrectAnswer(GameObject touched);
    void WrongAnswer(GameObject touched);
}
