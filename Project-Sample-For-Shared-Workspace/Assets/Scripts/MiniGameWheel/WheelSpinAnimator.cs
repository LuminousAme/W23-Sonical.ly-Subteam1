using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MiniGameWheel
{
    public class WheelSpinAnimator : MonoBehaviour
    {
        public WheelManager wheelManager;
        public float spinTime = 0.25f;

        float timeElapsed = 0f;
        int spinNum = 0;
        int targetSpins = 0;
        float acutalSpinTime = 0f;
        int currentIndex = 0;
        bool spinning = false;
        bool finishing = false;
        Action callback;

        public void Spin(int numberOfSpins, Action callback = null)
        {
            this.callback = callback;
            targetSpins = numberOfSpins;
            currentIndex = 0;
            spinNum = 0;
            spinning = true;
        }

        void Update()
        {
            //determine the spin time, slowing down on the last 5 spins
            if (spinNum > targetSpins - 5) acutalSpinTime = spinTime * (spinNum - (targetSpins - 5));
            else acutalSpinTime = spinTime;

            //if it's spinning
            if (spinning)
            {
                //rotate the wheel
                float t = timeElapsed / acutalSpinTime;
                if (t < 0.0f) t = 0.0f;
                else if (t > 1.0f) t = 1.0f;
                wheelManager.RotateTo(currentIndex - 1, currentIndex, t);

                //update the elapsed time
                timeElapsed += Time.deltaTime;

                if (timeElapsed > acutalSpinTime)
                {
                    //update every time the index changes
                    timeElapsed = 0f;
                    spinNum++;
                    currentIndex++;
                    if (currentIndex >= wheelManager.possibleResults.Count) currentIndex = 0;

                    //when we've hit the last spin stop spinning and call the callback
                    if (spinNum == targetSpins)
                    {
                        spinning = false;
                        finishing = true;
                    }
                }
            }

            if (finishing)
            {
                //update the elapsed time
                timeElapsed += Time.deltaTime;
                if (timeElapsed > acutalSpinTime)
                {
                    timeElapsed = 0f;
                    spinning = false;
                    callback?.Invoke();
                    finishing = false;
                }
            }
        }
    }
}
