using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace MiniGameSlots
{
    public class SlotIconAnimator : MonoBehaviour
    {
        public SlotsManager slotsManager;
        public Image image;
        public GameObject backGround;
        public int minSpins = 50;
        public float spinTime = 0.25f;

        float timeElapsed = 0f;
        int spinNum = 0;
        float acutalSpinTime = 0f;
        int currentIndex = 0;
        int targetIndex = 0;
        bool spinning = false;
        bool finishing = false;
        Action callback;

        //begin cycling through the icons
        public void Spin(int targetIndex, Action callback = null)
        {
            this.targetIndex = targetIndex;
            this.callback = callback;
            backGround.SetActive(false);
            spinNum = 0;
            spinning = true;
        }

        void Update()
        {
            //determine the spin time, slowing down on the last 5 spins
            if (spinNum > minSpins + targetIndex - 5) acutalSpinTime = spinTime * (spinNum - (minSpins + targetIndex - 5));
            else acutalSpinTime = spinTime;

            //if it's spinning
            if (spinning)
            {
                //update the elapsed time
                timeElapsed += Time.deltaTime;
                if(timeElapsed > acutalSpinTime)
                {
                    //update every time the index changes
                    timeElapsed = 0f;
                    spinNum++;
                    currentIndex++;
                    if (currentIndex >= slotsManager.slotOptions.Count) currentIndex = 0;

                    //set the icon
                    image.sprite = slotsManager.slotOptions[currentIndex].icon;

                    //when we've hit the last spin stop spinning and call the callback
                    if(spinNum == minSpins + targetIndex)
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
                    backGround.SetActive(true);
                    callback?.Invoke();
                    finishing = false;
                }
            }
        }
    }

}
