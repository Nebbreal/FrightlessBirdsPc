using System;
using EventSystem;
using UnityEngine;

namespace MinigameFramework.UIElements
{
    public class ScoreboardCrown : MonoBehaviour
    {
        [NonSerialized] public GameObject target;
        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private Quaternion _targetRotation;
        public float animationDuration;
        private float _animationTime = 0f;
        public GameEvent updateText;

        private void Awake(){
            _startPosition = transform.position;
            _startRotation = transform.rotation;
            _targetRotation = Quaternion.LookRotation(target.transform.position, Vector3.forward);
        }
        private void Update(){
            _animationTime += Time.deltaTime;
            float t = _animationTime / animationDuration;
            float easedT = easeInOutBack(t);

            transform.position = Vector3.Slerp(_startPosition, target.transform.position, easedT);
            transform.rotation = Quaternion.Slerp(_startRotation, _targetRotation, t);

            if (t >= 1f)
            {
                updateText.Raise(this);
            }
        }

        private float easeInOutBack(float x) {
            float c1 = 1.70158f;
            float c2 = c1 * 1.525f;

            if (x >= 1f)
            {
                return 1f;
            }

            return x < 0.5
            ? (Mathf.Pow(2f * x, 2f) * ((c2 + 1) * 2f * x - c2)) / 2f
            : (Mathf.Pow(2f * x - 2f, 2f) * ((c2 + 1f) * (x * 2f - 2f) + c2) + 2f) / 2f;
        }
    }
}
