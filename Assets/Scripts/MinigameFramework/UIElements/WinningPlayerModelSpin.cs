using UnityEngine;

namespace MinigameFramework.UIElements
{
    /// <summary>
    /// Rotates the GameObject at a constant angular speed while smoothly drifting
    /// the rotation axis to random directions over time.
    /// Attach this to your model and tune the inspector fields.
    /// </summary>
    public class WinningPlayerModelSpin : MonoBehaviour
    {
        public float angularSpeed = 30f;
        public float transitionDurationMin = 2f;
        public float transitionDurationMax = 6f;
        public bool useLocalSpace = true;

        private Vector3 _startDir;
        private Vector3 _targetDir;
        private float _transitionDuration;
        private float _timer;

        void Start()
        {
            _startDir = Random.onUnitSphere.normalized;
            _targetDir = Random.onUnitSphere.normalized;
            _transitionDuration = Random.Range(transitionDurationMin, transitionDurationMax);
            _timer = 0f;
        }

        void Update()
        {
            // advance transition timer
            _timer += Time.deltaTime;
            float t = Mathf.Clamp01(_timer / Mathf.Max(0.0001f, _transitionDuration));
            // smooth easing for a more natural drift
            float s = Mathf.SmoothStep(0f, 1f, t);

            // smoothly interpolate the axis on the unit sphere
            Vector3 axis = Vector3.Slerp(_startDir, _targetDir, s).normalized;

            // rotate by a constant angle this frame (angle = angular speed * dt)
            float angleThisFrame = angularSpeed * Time.deltaTime; // degrees
            Quaternion delta = Quaternion.AngleAxis(angleThisFrame, axis);

            if (useLocalSpace)
                transform.localRotation = delta * transform.localRotation;
            else
                transform.rotation = delta * transform.rotation;

            // when transition completes: start a new one
            if (t >= 1f - Mathf.Epsilon)
            {
                _startDir = _targetDir;
                // ensure new target is not almost identical to start (avoid no-op)
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