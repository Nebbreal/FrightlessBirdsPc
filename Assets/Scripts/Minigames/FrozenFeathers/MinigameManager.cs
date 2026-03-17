using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Minigames.FrozenFeathers
{
    public class MinigameManager : MonoBehaviour, IMinigameManager
    {
        
        public UnityEvent winEvent = new();
        [SerializeField]
        private GameObject shrinkingPlatform;
        [SerializeField]
        private float shrinkingPlatformTargetScale = 0.3f;

        [SerializeField] 
        private GameObject respawnPoint;
        //Config
        private GameObject _gameManager;
        private GameConfig _gameConfig;
        private MockPlayerManager _playerManager;
        private List<GameObject> _players;
        
        private float _roundTimer;
        private float _startScale;

        public void Start()
        {
            #region MinigameConfig
                _gameManager = GameObject.FindWithTag("GameManager");
                _gameConfig = _gameManager.GetComponent<GameConfig>();
                
                _playerManager = _gameManager.GetComponent<MockPlayerManager>();
                _players = _playerManager.players;
                
                _playerManager.SetPlayerLives();
            #endregion
            
            _roundTimer = _gameConfig.roundLength;
            _startScale = shrinkingPlatform.transform.localScale.x;
        }

        public void Update()
        {
            if (_roundTimer > 0)
            {
                _roundTimer -= Time.deltaTime;
                ShrinkPlatform();
            }
        }

        private void ShrinkPlatform()
        {
            float currentPlatRadius = shrinkingPlatformTargetScale + (_startScale - shrinkingPlatformTargetScale) * (_roundTimer / _gameConfig.roundLength);
            shrinkingPlatform.transform.localScale = new Vector3(currentPlatRadius,  shrinkingPlatform.transform.transform.localScale.y, currentPlatRadius);
        }

        public void EliminatePlayer(GameObject entity)
        {
            if (!_players.Contains(entity)) return;
           
            _playerManager.DeductLife(entity);

            if (_playerManager.PlayerLives[entity.name] > 0)
            {
                GameObject playerObject = entity.gameObject;
                Rigidbody playerRigidbody = playerObject.GetComponent<Rigidbody>();
                playerRigidbody.linearVelocity = Vector3.zero;
                playerObject.transform.position = respawnPoint.transform.position;
            }
            

            if (_playerManager.GetAlivePlayerAmount() <= 1)
            {
                OnWin();
            }
        }

        private void OnWin()
        {
            Time.timeScale = 0;
            winEvent.Invoke();
        }
    }
}
