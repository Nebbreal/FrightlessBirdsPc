using UnityEngine;

namespace Minigames.FrozenFeathers
{
    public class ShrinkingPlatform : MonoBehaviour
    {
        public GameObject gameManager;

        private float _roundTimer;
        //Config
        private GameConfig _gameConfig;
        [SerializeField]
        private float targetScale = 0.3f;

        private float _startScale;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            gameManager = GameObject.Find("GameManager");
            _gameConfig = gameManager.GetComponent<GameConfig>();
            _roundTimer = _gameConfig.roundLength;
            _startScale = transform.localScale.x;
        }

        // Update is called once per frame
        void Update()
        {
            if (_roundTimer > 0)
            {
                _roundTimer -= Time.deltaTime;
                float currentPlatRadius = targetScale + (_startScale - targetScale) * (_roundTimer / _gameConfig.roundLength);
                transform.localScale = new Vector3(currentPlatRadius, transform.localScale.y, currentPlatRadius);
            }
        }
    }
}
