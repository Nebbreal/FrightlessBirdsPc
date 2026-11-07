using MinigameFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Initialization
{
    public class InitHelper : MonoBehaviour
    {
        [SerializeField]
        private GameSceneManager gameSceneManager;
        [SerializeField] 
        private SceneField sceneToLoad;
    
        public void Start()
        {
            if (sceneToLoad == null)
            {
                Debug.LogError("No scene to load!");
            }
            else
            {
                gameSceneManager.SetSceneToLoad(sceneToLoad);
            }
        }
    }
}
