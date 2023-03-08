using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MiniGameWheel
{
    public class SwipeDetector : MonoBehaviour
    {
        Vector2 fingerTouchPos;
        Vector2 fingerReleasePos;

        public float minDistanceForSwipe = 20f;

        public static event Action<float> OnSwipe = delegate { };

        // Update is called once per frame
        void Update()
        {
            for(int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if(touch.phase == TouchPhase.Began)
                {
                    fingerTouchPos = touch.position;
                }

                if(touch.phase == TouchPhase.Ended)
                {
                    fingerReleasePos = touch.position;
                    float distance = Vector2.Distance(fingerTouchPos, fingerReleasePos);
                    if (distance >= minDistanceForSwipe) OnSwipe.Invoke(distance);
                    fingerTouchPos = fingerReleasePos;
                }
            }
        }
    }
}
