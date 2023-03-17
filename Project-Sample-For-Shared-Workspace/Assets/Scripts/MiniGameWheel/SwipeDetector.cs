using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MiniGameWheel
{
    public class SwipeDetector : MonoBehaviour
    {
        public enum SwipeMode
        {
            NONE = 0,
            MOBILE = 1,
            MOUSE = 2
        }

        SwipeMode swipeMode = SwipeMode.NONE;
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

                if(touch.phase == TouchPhase.Began && swipeMode == SwipeMode.NONE)
                {
                    fingerTouchPos = touch.position;
                    swipeMode = SwipeMode.MOBILE;
                }

                if(touch.phase == TouchPhase.Ended && swipeMode == SwipeMode.MOBILE)
                {
                    fingerReleasePos = touch.position;
                    float distance = Vector2.Distance(fingerTouchPos, fingerReleasePos);
                    if (distance >= minDistanceForSwipe) OnSwipe.Invoke(distance);
                    fingerTouchPos = fingerReleasePos;
                    swipeMode = SwipeMode.NONE;
                }
            }

            if(Input.GetMouseButtonDown(0) && swipeMode == SwipeMode.NONE)
            {
                fingerTouchPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                swipeMode = SwipeMode.MOUSE;
            }

            if(Input.GetMouseButtonUp(0) && swipeMode == SwipeMode.MOUSE)
            {
                fingerReleasePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                float distance = Vector2.Distance(fingerTouchPos, fingerReleasePos);
                if (distance >= minDistanceForSwipe) OnSwipe.Invoke(distance);
                fingerTouchPos = fingerReleasePos;
                swipeMode = SwipeMode.NONE;
            }
        }
    }
}
