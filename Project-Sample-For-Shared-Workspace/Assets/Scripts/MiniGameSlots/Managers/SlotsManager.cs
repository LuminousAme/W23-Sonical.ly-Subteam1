using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGameSlots
{
    public class SlotsManager : MonoBehaviour
    {
        //a single permutation of a slot
        [System.Serializable]
        public class SlotOption
        {
            public int tokens;
            public Sprite icon;

            public SlotOption(int tokens)
            {
                this.tokens = tokens;
            }
        }

        //the values for all of the possible slot permutations
        public List<SlotOption> slotOptions = new List<SlotOption>();
        //the final result
        SlotOption result;
        public SlotOption Result
        {
            get {return result;}
            set { result = value; }
        }

        //text objects to update
        public TextValueAnimator jackpotText;
        public TextValueAnimator accuracyText;
        public TextValueAnimator timeText;

        //slot icons
        public SlotIconAnimator icon1;
        public SlotIconAnimator icon2;
        public SlotIconAnimator icon3;

        int[] indices = { 0, 0, 0 };
        public int[] Indices { get => indices; }

        public SlotOption Randomize()
        {
            indices[0] = Random.Range(0, slotOptions.Count);
            //randomize which case is triggered
            switch (Random.Range(0, 4))
            {
                //all 3 slots are the same
                case 0:
                    indices[1] = indices[0];
                    indices[2] = indices[0];
                    break;

                //the first two slots are the same, third is random
                case 1:
                    indices[1] = indices[0];
                    indices[2] = Random.Range(0, slotOptions.Count);
                    break;

                //first and third are the same, second is random
                case 2:
                    indices[1] = Random.Range(0, slotOptions.Count);
                    indices[2] = indices[0];
                    break;

                //all 3 are random
                default:
                    indices[1] = Random.Range(0, slotOptions.Count);
                    indices[2] = Random.Range(0, slotOptions.Count);
                    break;
            }


            //Tokens
            int averageTokens = (slotOptions[indices[0]].tokens
                + slotOptions[indices[1]].tokens
                + slotOptions[indices[2]].tokens) / 3;

            //round to the nearest interval of 500 
            averageTokens = (int)System.Math.Round((double)averageTokens / 500.0, 0, System.MidpointRounding.AwayFromZero) * 500;

            return new SlotOption(averageTokens);
        }

        public void RandomizeUI()
        {
            //handle updating the UI
            result = Randomize();

            icon1.Spin(indices[0]);
            icon2.Spin(indices[1]);
            icon3.Spin(indices[2], UpdateText);
        }

        void UpdateText()
        {
            int tokens = result.tokens;
            int accuracy = GetTargetAccuracy(tokens);
            int timeToBeat = GetTimeToBeat(tokens);

            jackpotText.AnimateValue(tokens, 1f);
            accuracyText.AnimateValue(accuracy, 1f);
            timeText.AnimateValue(timeToBeat, 1f);
        }

        public int GetTargetAccuracy(int tokens)
        {
            float min = slotOptions[0].tokens;
            float max = slotOptions[slotOptions.Count - 1].tokens;

            if (tokens <= min) return 50;
            else if (tokens >= max) return 99;

            float t = Mathf.InverseLerp(min, max, tokens);

            float value = Mathf.Lerp(50, 100, t);

            //round to the nearest interval of 5
            return (int)System.Math.Round((double)value / 5.0, 0, System.MidpointRounding.AwayFromZero) * 5;
        }

        public int GetTimeToBeat(int tokens)
        {
            float min = slotOptions[0].tokens;
            float max = slotOptions[slotOptions.Count - 1].tokens;

            if (tokens <= min) return 45;
            else if (tokens >= max) return 10;

            float t = Mathf.InverseLerp(min, max, tokens);

            float value = Mathf.Lerp(45, 10, t);

            //round to the nearest interval of 5
            return (int)System.Math.Round((double)value / 5.0, 0, System.MidpointRounding.AwayFromZero) * 5;
        }
    }
}