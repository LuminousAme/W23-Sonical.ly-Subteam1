using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGameSlots
{
    public class SlotsManager : MonoBehaviour
    {
        [System.Serializable]
        public class SlotOption
        {
            public int tokens;
            public int targetAccuracy;

            public SlotOption(int tokens, int targetAccuracy)
            {
                this.tokens = tokens;
                this.targetAccuracy = targetAccuracy;
            }
        }

        public List<SlotOption> slotOptions = new List<SlotOption>();

        int[] indices = { 0, 0, 0 };
        public int[] Indices { get => indices; }

        public SlotOption Randomize()
        {
            indices[0] = Random.Range(0, slotOptions.Count);
            indices[1] = Random.Range(0, slotOptions.Count);
            indices[2] = Random.Range(0, slotOptions.Count);

            int averageTokens = (slotOptions[indices[0]].tokens
                + slotOptions[indices[1]].tokens
                + slotOptions[indices[2]].tokens) / 3;

            //round to the nearest interval of 500 
            averageTokens = (int)System.Math.Round((double)averageTokens / 500.0, 0, System.MidpointRounding.AwayFromZero) * 500;

            int averageAccuracy = (slotOptions[indices[0]].targetAccuracy
                + slotOptions[indices[1]].targetAccuracy
                + slotOptions[indices[2]].targetAccuracy) / 3;

            //round to the nearest interval of 5
            averageAccuracy = (int)System.Math.Round((double)averageAccuracy / 5.0, 0, System.MidpointRounding.AwayFromZero) * 5;

            return new SlotOption(averageTokens, averageAccuracy);
        }
    }
}