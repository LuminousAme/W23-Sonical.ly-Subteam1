using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinigameDice
{
    public class DiceManager : MonoBehaviour
    {
        [SerializeField] List<Rigidbody> dice = new List<Rigidbody>();
        [SerializeField] Vector3 alternateGravity = new Vector3(0.0f, 0.0f, 1.0f);
        [SerializeField] float randomizedScaleMin = 80f, randomizedScaleMax = 100f;
        [SerializeField] float randomUpMin = 8f, randomUpMax = 12f;
        bool waitingForResult = false;

        private void Start()
        {
            waitingForResult = false;
        }

        private void OnEnable()
        {
            SwipeDetector.OnSwipeVec += RollDice;
        }

        private void OnDisable()
        {
            SwipeDetector.OnSwipeVec -= RollDice;
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < dice.Count; i++) dice[i].AddForce(alternateGravity);
        }

        private void RollDice(Vector2 swipe)
        {
            for(int i = 0; i < dice.Count; i++)
            {
                float scale = Random.Range(randomizedScaleMin, randomizedScaleMax);
                float z = -Random.Range(randomUpMin, randomUpMax);
                Vector3 force = new Vector3(swipe.x, swipe.y, z) * scale;
                dice[i].AddForce(force);
                dice[i].AddTorque(force * scale);
            }

            waitingForResult = true;
        }
    }
}