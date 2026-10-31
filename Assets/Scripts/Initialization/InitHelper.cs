using UnityEngine;
using UnityEngine.SceneManagement;

namespace Initialization
{
    public class InitHelper : MonoBehaviour
    {
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
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}
