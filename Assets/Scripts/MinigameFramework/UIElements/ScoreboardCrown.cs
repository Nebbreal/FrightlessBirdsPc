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
        public float offsetStrength;
        public GameEvent updateText;

        private void OnEnable()
        {
            transform.localPosition = new Vector3(0f, MathF.Sign(target.transform.localPosition.y)*300f, 0f);
            transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.localPosition.normalized);
            _startPosition = transform.position;
            _startRotation = transform.rotation;
            _targetRotation = Quaternion.LookRotation(Vector3.forward, -target.transform.localPosition.normalized);
        }
        private void Update(){
            if (_animationTime >= animationDuration)
            {
                return;
            }
            _animationTime += Time.deltaTime;
            float t = _animationTime / animationDuration;
            float easedT = easeInOutBack(t);

            Vector3 offset = target.transform.localPosition.normalized * offsetStrength;
            transform.position = Vector3.Slerp(_startPosition, target.transform.position - offset, easedT);
            transform.rotation = Quaternion.Lerp(_startRotation, _targetRotation, easeOutExpo(t));

            if (t >= 1f)
            {
                updateText.Raise(this);
            }
        }

        private float easeInOutBack(float x) {
            // Easing function values from Easings.net
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

        private float easeOutExpo(float x)
        {
            return x >= 1f ? 1f : 1f - Mathf.Pow(2f, -10f * x);
        }
    }
}
