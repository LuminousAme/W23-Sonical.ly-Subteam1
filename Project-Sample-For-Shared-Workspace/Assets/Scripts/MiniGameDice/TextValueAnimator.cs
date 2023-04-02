using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace MinigameDice
{
    public class TextValueAnimator : MonoBehaviour
    {
        public TMP_Text text;
        public string prevalue = "";
        public string postvalue = "";
        public string nullString = "";
        public bool valueNull = true;

        private int value = 0;


        Action callback = null;
        float time = 0f;
        float elapsedTime = 0f;
        bool done = false;

        private void Update()
        {
            if (valueNull) text.text = nullString;
            else if (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.InverseLerp(0f, time, elapsedTime);
                float fval = Mathf.Lerp(0f, value, t);
                int ival = (int)fval;
                if (ival > value) ival = value;

                text.text = prevalue + ival.ToString() + postvalue;
            }
            else if (!done)
            {
                done = true;
                callback?.Invoke();
            }
        }

        public void AnimateValue(int value, float time, Action callback = null)
        {
            this.value = value;
            this.time = time;
            this.callback = callback;
            elapsedTime = 0f;
            valueNull = false;
            done = false;
        }
    }
}