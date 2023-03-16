using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

namespace MiniGameWheel
{
    [ExecuteAlways]
    public class WheelManager : MonoBehaviour
    {
        [System.Serializable]
        public class SpinWheelResult
        {
            public int tokens;

            public SpinWheelResult(int tokens)
            {
                this.tokens = tokens;
            }
        }

        [Tooltip("How full the wheel is, the lower the more space between options on the wheel, 1 is no space")]
        [Range(0.0f, 1.0f)]
        public float fillAmount = 0.95f;

        [Tooltip("How far away from the centre the text should be")]
        public float textDistance = 250.0f;

        public GameObject wheelOptionsPrefab;
        public Transform wheelHolderReference;
        public GameObject instructionText;

        //the values for all of the possible wheel permutations
        public List<SpinWheelResult> possibleResults = new List<SpinWheelResult>();
        SpinWheelResult result;

        int index = 0;
        public int Index
        {
            get => index;
        }

        //animator for the wheel
        public WheelSpinAnimator WheelSpinAnimator;

        //text objects to update
        public GameObject textBackground;
        public TextValueAnimator jackpotText;
        public TextValueAnimator accuracyText;
        public TextValueAnimator timeText;

        bool needsToPopulate = false;
        bool waitAFrame = true;

        private void OnEnable()
        {
            SwipeDetector.OnSwipe += RandomizeUI;
        }

        private void OnDisable()
        {
            SwipeDetector.OnSwipe -= RandomizeUI;
        }

        void Start()
        {
            needsToPopulate = true;
        }

        private void OnValidate()
        {
            needsToPopulate = true;
        }

        

        void Update()
        {
            if (!waitAFrame && needsToPopulate)
            {
                Populate();
                needsToPopulate = false;
            }
            else if (waitAFrame) waitAFrame = false;
        }

        //based on this video: https://youtu.be/uONqTBSTkJ8 
        public void Populate()
        {
            //if the parent of the wheel options, or the wheel options prefab is null in the list just return
            if (!wheelHolderReference || !wheelOptionsPrefab) return;

            //destroy any current children the holder may have
            while (wheelHolderReference.childCount > 0) DestroyImmediate(wheelHolderReference.GetChild(0).gameObject);

            //if there are no options in the list just return
            if (possibleResults.Count == 0) return;

            //otherwise begin populating the wheel
            for (int i = 0; i < possibleResults.Count; i++)
            {
                GameObject newOption;
#if UNITY_EDITOR
                newOption = Instantiate(wheelOptionsPrefab);
                GameObjectUtility.SetParentAndAlign(newOption, wheelHolderReference.gameObject);
#else
                newOption = Instantiate(wheelOptionsPrefab, wheelHolderReference);
#endif
                //puts it in the correct place around the wheel
                newOption.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, i * -360.0f / possibleResults.Count));
                //fills the correct portion of the wheel
                newOption.GetComponent<Image>().fillAmount = fillAmount / possibleResults.Count;

                //change the text to the correct tokens amount
                TMP_Text text = newOption.GetComponentInChildren<TMP_Text>();
                text.text = possibleResults[i].tokens.ToString();

                Vector3 up = newOption.transform.up;
                newOption.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (i+1) * -360.0f / possibleResults.Count));
                Vector3 nextUp = newOption.transform.up;
                newOption.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, i * -360.0f / possibleResults.Count));

                Vector3 direction = ((nextUp + up) * 0.5f).normalized;
                direction = new Vector3(direction.x * text.transform.lossyScale.x, direction.y * text.transform.lossyScale.y, direction.z * text.transform.lossyScale.z);

                text.transform.position = wheelHolderReference.position + (direction * textDistance);
                float textangle = -(360f * (fillAmount / possibleResults.Count / 2));
                text.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, textangle));
            }
            //rotates the wheel so the first option is at the top
            wheelHolderReference.rotation = Quaternion.Euler(new Vector3(0, 0, 360f * (fillAmount / possibleResults.Count / 2)));
        }

        public void RandomizeUI(float distance)
        {
            instructionText.SetActive(false);
            textBackground.SetActive(false);
            jackpotText.valueNull = true;
            accuracyText.valueNull = true;
            timeText.valueNull = true;

            //handle updating the UI
            int numberOfSpins = (int)System.Math.Round(distance / 10.0f);
            index = numberOfSpins % possibleResults.Count;
            result = new SpinWheelResult(possibleResults[index].tokens);

            WheelSpinAnimator.Spin(numberOfSpins, UpdateText);
        }

        void UpdateText()
        {
            int tokens = result.tokens;
            int accuracy = GetTargetAccuracy(tokens);
            int timeToBeat = GetTimeToBeat(tokens);

            textBackground.SetActive(true);
            jackpotText.AnimateValue(tokens, 1f);
            accuracyText.AnimateValue(accuracy, 1f);
            timeText.AnimateValue(timeToBeat, 1f);
        }

        public int GetTargetAccuracy(int tokens)
        {
            float min = possibleResults[0].tokens;
            float max = possibleResults[possibleResults.Count - 1].tokens;

            if (tokens <= min) return 50;
            else if (tokens >= max) return 99;

            float t = Mathf.InverseLerp(min, max, tokens);

            float value = Mathf.Lerp(50, 100, t);

            //round to the nearest interval of 5
            return (int)System.Math.Round((double)value / 5.0, 0, System.MidpointRounding.AwayFromZero) * 5;
        }

        public int GetTimeToBeat(int tokens)
        {
            float min = possibleResults[0].tokens;
            float max = possibleResults[possibleResults.Count - 1].tokens;

            if (tokens <= min) return 45;
            else if (tokens >= max) return 10;

            float t = Mathf.InverseLerp(min, max, tokens);

            float value = Mathf.Lerp(45, 10, t);

            //round to the nearest interval of 5
            return (int)System.Math.Round((double)value / 5.0, 0, System.MidpointRounding.AwayFromZero) * 5;
        }

        public void RotateTo(int fromIndex, int toIndex, float t)
        {
            if (fromIndex < 0) fromIndex = possibleResults.Count - 1;
            if (toIndex > possibleResults.Count - 1) toIndex = 0;

            Quaternion baseRot = Quaternion.Euler(new Vector3(0.0f, 0.0f, -1.0f * (fromIndex * -360f / possibleResults.Count) +
                (360f * (fillAmount / possibleResults.Count / 2))));

            Quaternion desiredRot = Quaternion.Euler(new Vector3(0.0f, 0.0f, -1.0f * (toIndex * -360f / possibleResults.Count) +
                (360f * (fillAmount / possibleResults.Count / 2))));

            wheelHolderReference.rotation = Quaternion.Slerp(baseRot, desiredRot, t);
        }
    }
}
