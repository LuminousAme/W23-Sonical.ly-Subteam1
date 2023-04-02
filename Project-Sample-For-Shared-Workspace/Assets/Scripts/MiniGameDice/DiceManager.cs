using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinigameDice
{
    public class DiceManager : MonoBehaviour
    {
        [System.Serializable]
        public class DieFace
        {
            public enum direction
            {
                UP = 0, 
                DOWN,
                FORWARD,
                BACK,
                RIGHT,
                LEFT
            }

            public direction m_direction;
            public int m_value;
        }
        [SerializeField] List<DieFace> dieFaces = new List<DieFace>();
        [SerializeField] Vector3 upDirection = new Vector3(0.0f, 0.0f, -1.0f);


        [Space]
        [SerializeField] List<Rigidbody> dice = new List<Rigidbody>();
        [SerializeField] Vector3 alternateGravity = new Vector3(0.0f, 0.0f, 1.0f);
        [SerializeField] float randomizedScaleMin = 80f, randomizedScaleMax = 100f;
        [SerializeField] float randomUpMin = 8f, randomUpMax = 12f;
        [SerializeField] float timeStillToConfirmResult = 1f;
        [SerializeField] int resultMultiplier = 1000;

        [Space]
        //text objects to update
        [SerializeField] GameObject instructionText;
        [SerializeField] GameObject textBackground;
        [SerializeField] TextValueAnimator jackpotText;
        [SerializeField] TextValueAnimator accuracyText;
        [SerializeField] TextValueAnimator timeText;

        bool waitingForResult = false;
        List<float> timeStill = new List<float>();



        private void Start()
        {
            waitingForResult = false;
            timeStill.Clear();
            for (int i = 0; i < dice.Count; i++) timeStill.Add(0.0f);
        }

        private void OnEnable()
        {
            SwipeDetector.OnSwipeVec += RollDice;
        }

        private void OnDisable()
        {
            SwipeDetector.OnSwipeVec -= RollDice;
        }

        private void Update()
        {
            //don't bother with the rest of the function if we aren't waiting for results
            if (!waitingForResult) return;

            //check if all of the dice are still and have been for long enough that we're confident they're done moving
            bool allDiceStill = true;
            for(int i = 0; i < dice.Count; i++)
            {
                if (timeStill[i] < timeStillToConfirmResult) allDiceStill = false;

                if (dice[i].velocity.magnitude < 0.1f && dice[i].angularVelocity.magnitude < 0.1f)
                {
                    timeStill[i] += Time.deltaTime;
                }
                else
                {
                    timeStill[i] = 0.0f;
                }
            }

            //if they, fill out the results
            if (allDiceStill) DetermineResult();

        }

        private void FixedUpdate()
        {
            for (int i = 0; i < dice.Count; i++) dice[i].AddForce(alternateGravity);
        }

        //perform the acutal dice roll, based on the direction of the swipe with a little added extra randomness
        private void RollDice(Vector2 swipe)
        {
            instructionText.SetActive(false);
            textBackground.SetActive(false);
            jackpotText.valueNull = true;
            accuracyText.valueNull = true;
            timeText.valueNull = true;

            for (int i = 0; i < dice.Count; i++)
            {
                float scale = Random.Range(randomizedScaleMin, randomizedScaleMax);
                float z = -Random.Range(randomUpMin, randomUpMax);
                Vector3 force = new Vector3(swipe.x, swipe.y, z) * scale;
                dice[i].AddForce(force);
                dice[i].AddTorque(force * scale);

                timeStill[i] = 0.0f;
            }

            waitingForResult = true;
        }

        //determine the overall result of the dice roll
        private void DetermineResult()
        {
            //add up the value of each face
            int summedResult = 0;
            for(int i = 0; i < dice.Count; i++)
            {
                summedResult += DetermineUpFaceValue(dice[i]);
            }

            //update the text with the results
            UpdateText(summedResult * resultMultiplier);

            //stop waiting for new results
            waitingForResult = false;
        }

        //determine the value of an indivual face
        private int DetermineUpFaceValue(Rigidbody die)
        {
            int bestIndex = 0;
            float bestDotProduct = float.MinValue;
               
            //compare the direction of each face to the overall up direction to determine which one is closet to up
            for(int i = 0; i < dieFaces.Count; i++)
            {
                float dotProduct = float.MinValue;

                switch(dieFaces[i].m_direction)
                {
                    case DieFace.direction.UP:
                        dotProduct = Vector3.Dot(upDirection, die.transform.up);
                        break;
                    case DieFace.direction.DOWN:
                        dotProduct = Vector3.Dot(upDirection, -1f * die.transform.up);
                        break;
                    case DieFace.direction.FORWARD:
                        dotProduct = Vector3.Dot(upDirection, die.transform.forward);
                        break;
                    case DieFace.direction.BACK:
                        dotProduct = Vector3.Dot(upDirection, -1f * die.transform.forward);
                        break;
                    case DieFace.direction.RIGHT:
                        dotProduct = Vector3.Dot(upDirection, die.transform.right);
                        break;
                    case DieFace.direction.LEFT:
                        dotProduct = Vector3.Dot(upDirection, -1f * die.transform.right);
                        break;
                }

                //determine which dot product is closest to 1f, by subtracting 1f from each and checking the larger, as we know niether will ever be greater than 1f
                //the face with the dot prouct closest is the face facing up
                if((dotProduct - 1f) > (bestDotProduct - 1f))
                {
                    bestDotProduct = dotProduct;
                    bestIndex = i;
                }
            }

            //return the value of that face
            return dieFaces[bestIndex].m_value;
        }

        void UpdateText(int tokens)
        {
            int accuracy = GetTargetAccuracy(tokens);
            int timeToBeat = GetTimeToBeat(tokens);

            textBackground.SetActive(true);
            jackpotText.AnimateValue(tokens, 1f);
            accuracyText.AnimateValue(accuracy, 1f);
            timeText.AnimateValue(timeToBeat, 1f);
        }

        public int GetTargetAccuracy(int tokens)
        {
            float min = dieFaces[0].m_value * dice.Count * resultMultiplier;
            float max = dieFaces[dieFaces.Count - 1].m_value * dice.Count * resultMultiplier;

            if (tokens <= min) return 50;
            else if (tokens >= max) return 99;

            float t = Mathf.InverseLerp(min, max, tokens);

            float value = Mathf.Lerp(50, 100, t);

            //round to the nearest interval of 5
            return (int)System.Math.Round((double)value / 5.0, 0, System.MidpointRounding.AwayFromZero) * 5;
        }

        public int GetTimeToBeat(int tokens)
        {
            float min = dieFaces[0].m_value * dice.Count * resultMultiplier;
            float max = dieFaces[dieFaces.Count - 1].m_value * dice.Count * resultMultiplier;

            if (tokens <= min) return 45;
            else if (tokens >= max) return 10;

            float t = Mathf.InverseLerp(min, max, tokens);

            float value = Mathf.Lerp(45, 10, t);

            //round to the nearest interval of 5
            return (int)System.Math.Round((double)value / 5.0, 0, System.MidpointRounding.AwayFromZero) * 5;
        }
    }
}