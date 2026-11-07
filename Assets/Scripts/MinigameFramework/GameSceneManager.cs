using System;
using EventSystem;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MinigameFramework
{
    public class GameSceneManager : MonoBehaviour
    {
        [Header("GameEvents")]
        [SerializeField] 
        private GameEvent onEnableLoadingScreen;
        [SerializeField] 
        private GameEvent onDisableLoadingScreen;
        
        private string _sceneToLoad = "";
        
        public void SetSceneToLoad(string sceneToLoad)
        {
            if (String.IsNullOrEmpty(sceneToLoad)) return;
            
            _sceneToLoad = sceneToLoad;
            onEnableLoadingScreen.Raise(this, null);
        }

        public void OnLoadingScreenEnabled(Component sender, object[] _)
        {
            SceneManager.LoadScene(_sceneToLoad);
            onDisableLoadingScreen.Raise(this, null);
            _sceneToLoad = "";
        }
    }
}
