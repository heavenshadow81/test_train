using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CookingGame
{
    public class CameraMove : MonoBehaviour
    {
        [SerializeField] Transform[] cameraTransforms = null;
        public void MoveCam(int index = 1, Action onComplete = null)
        {
            transform.position = cameraTransforms[index].position;
            transform.rotation = cameraTransforms[index].rotation;

            onComplete?.Invoke();
        }
    }
}
