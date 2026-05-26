using UnityEngine;

namespace MinigameFramework.UIElements
{
    public class WinningPlayerModelSpin : MonoBehaviour
    {
        public bool isSpinning = true;
        [SerializeField]
        private float angularSpeed = 30f;
        [SerializeField]
        private float transitionDurationMin = 2f;
        [SerializeField]
        private float transitionDurationMax = 6f;
        [SerializeField]
        private bool useLocalSpace = true;

        private Vector3 _startDir;
        private Vector3 _targetDir;
        private float _transitionDuration;
        private float _timer;

        private void Start()
        {
            _startDir = Random.onUnitSphere.normalized;
            _targetDir = Random.onUnitSphere.normalized;
            _transitionDuration = Random.Range(transitionDurationMin, transitionDurationMax);
            _timer = 0f;
        }

        private void Update()
        {
            if (!isSpinning) return;
            
            _timer += Time.deltaTime;
            float t = Mathf.Clamp01(_timer / Mathf.Max(0.0001f, _transitionDuration));
            float smoothStep = Mathf.SmoothStep(0f, 1f, t);

            Vector3 axis = Vector3.Slerp(_startDir, _targetDir, smoothStep).normalized;

            // rotate by a constant angle this frame (angle = angular speed * dt)
            float angleThisFrame = angularSpeed * Time.deltaTime; // degrees
            Quaternion delta = Quaternion.AngleAxis(angleThisFrame, axis);

            if (useLocalSpace)
                transform.localRotation = delta * transform.localRotation;
            else
                transform.rotation = delta * transform.rotation;

            if (t >= 1f - Mathf.Epsilon)
            {
                _startDir = _targetDir;
                do
                {
                    _targetDir = Random.onUnitSphere.normalized;
                } while (Vector3.Dot(_startDir, _targetDir) > 0.999f);

                _transitionDuration = Random.Range(transitionDurationMin, transitionDurationMax);
                _timer = 0f;
            }
        }
    }
}