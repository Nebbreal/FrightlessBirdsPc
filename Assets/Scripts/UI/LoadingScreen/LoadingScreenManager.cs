using System;
using System.Collections;
using EventSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utility;

namespace UI.LoadingScreen
{
    public class LoadingScreenManager : MonoBehaviour
    {
        private readonly int _planeFlyState = Animator.StringToHash("FlyState");
        private float _fadeTimeElapsed = 0f;
        private float _currentAlpha = 0f;
        private float _fadeTarget = 1f;
        private float _fadeStartAlpha = 0f;
        
        [SerializeField]
        private string neutralState = "Plane|PlaneFly";
        [SerializeField] 
        private Animator planeAnimator;
        [SerializeField] 
        private Image backgroundImage;

        [Header("Configuration")] 
        [SerializeField]
        private float minWaitTime = 1f;
        [SerializeField] 
        private float bgFadeTime = 2f;

        [Header("GameEvents")] 
        [SerializeField]
        private GameEvent onLoadingScreenFinished;
        [SerializeField] 
        private GameEvent onLoadingScreenEnabled;

        public void Update()
        {
            if (_fadeTimeElapsed < bgFadeTime)
            {
                FadeBackGround();
            }
        }

        public void OnEnableLoadingScreen(Component sender, object[] _)
        {
            StartCoroutine(EnableLoadingScreenRoutine());
        }

        public void OnDisableLoadingScreen(Component sender, object[] _)
        {
            StartCoroutine(DisableLoadingScreenRoutine());
        }

        private void StartBackgroundFade(float targetOpacity)
        {
            _fadeTimeElapsed = 0f;
            _fadeStartAlpha = _currentAlpha;
            _fadeTarget = targetOpacity;            
        }
        
        private void FadeBackGround()
        {
            if (_fadeTimeElapsed < bgFadeTime)
            {
                _fadeTimeElapsed += Time.deltaTime;
                float t = Mathf.Clamp01(_fadeTimeElapsed / bgFadeTime);
                _currentAlpha = Mathf.Lerp(_fadeStartAlpha, _fadeTarget, t);
                
                Color newColor = backgroundImage.color;
                newColor.a = _currentAlpha;
                backgroundImage.color = newColor;
            }
        }

        private IEnumerator EnableLoadingScreenRoutine()
        {
            StartBackgroundFade(1f);
            planeAnimator.SetInteger(_planeFlyState, 0);
            
            yield return WaitForLoadingScreenTransitionFinished();
            
            onLoadingScreenEnabled.Raise(this, null);
        }

        private IEnumerator DisableLoadingScreenRoutine()
        {
            yield return WaitForPlaneNeutralState();
            planeAnimator.SetInteger(_planeFlyState, 1);
            
            yield return WaitForPlaneNeutralState();
            StartBackgroundFade(0f);
            
            yield return WaitForLoadingScreenTransitionFinished();
            onLoadingScreenFinished.Raise(this, null);
        }
        
        private IEnumerator WaitForPlaneNeutralState()
        {
            while (AnimatorUtilities.IsPlaying(planeAnimator, neutralState))
            {
                yield return null;
            }
        }

        private IEnumerator WaitForLoadingScreenTransitionFinished()
        {
            while (_fadeTimeElapsed < bgFadeTime)
            {
                yield return null;
            }
        }
    }
}
