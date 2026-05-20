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
        public float PanelOffset = 300f;

        private void OnEnable()
        {
            transform.localPosition = new Vector3(0f, MathF.Sign(target.transform.localPosition.y)*PanelOffset, 0f);
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
            float easedT = Ease.InOutBack(t);

            Vector3 offset = target.transform.localPosition.normalized * offsetStrength;
            transform.position = Vector3.Slerp(_startPosition, target.transform.position - offset, easedT);
            transform.rotation = Quaternion.Lerp(_startRotation, _targetRotation, Ease.OutExpo(t));

            if (t >= 1f)
            {
                updateText.Raise(this);
            }
        }
    }
}
