using System;
using System.Collections.Generic;
using System.Linq;
using Minigames;
using UnityEngine;
using UnityEngine.Serialization;

namespace MinigameFramework.UIElements
{
    public class WinningPlayerModelInstantiator : MonoBehaviour
    {
        [FormerlySerializedAs("pivot")] [FormerlySerializedAs("pivotPosition")] [SerializeField] 
        private GameObject centerPivot;
        [SerializeField] 
        private Vector3 modelPivotOffset;
        [SerializeField]
        private float multipleModelGap = 5f;

        private List<GameObject> _instantiatedPivots = new List<GameObject>();
        
        public void OnUpdateWinUIModel(Component sender, object[] modelsObj)
        {
            if (modelsObj.First() is not List<GameObject> winnerObjects) return;
            if (winnerObjects.Count == 0) return;

            if (winnerObjects.Count == 1)
            {
                InstantiateModel(winnerObjects[0], 0f);
            }
            else
            {
                //Calculate the starting position and offset the models will need to be evenly distributed
                //around the center (Based on the multipleModelGap)
                float centerX = centerPivot.transform.localPosition.x;
                float totalWidth = multipleModelGap * (winnerObjects.Count - 1);
                float startPoint = centerX - (totalWidth / 2f);
                
                for (int i = 0; i < winnerObjects.Count; i++)
                {
                    float modelXPos = startPoint + (i * multipleModelGap);
                    
                    InstantiateModel(winnerObjects[i], modelXPos);
                }
            }
        }

        private void InstantiateModel(GameObject winnerModelObject, float xPos)
        {
            GameObject pivot = Instantiate(centerPivot, gameObject.transform);
            pivot.GetComponent<WinningPlayerModelSpin>().isSpinning = true;
            pivot.transform.localPosition = new Vector3(xPos, 0f, 0f);
            GameObject spinningModel = Instantiate(winnerModelObject, pivot.transform);

            PlayerModel spinningModelScript = spinningModel.GetComponent<PlayerModel>();
            GameObject winnerObject =  winnerModelObject.GetComponentInParent<PlayerController>().gameObject;
            GameManager gameManager = GameManager.Instance;
            PlayerManager playerManager = gameManager.PlayerManager;
            if (playerManager.Teams.Count > 0)
            {
                List<Team> teams = playerManager.Teams;
                Team playerTeam = teams.FirstOrDefault(team => team.Players.Contains(winnerObject));
                int playerTeamIndex = teams.IndexOf(playerTeam);
                Color playerColor = gameManager.CurrentMinigame.teamOutlineColors[playerTeamIndex];
                spinningModelScript.ChangeOutlineColor(playerColor);
            }
            else
            {
                int playerID = winnerModelObject.GetComponentInParent<PlayerController>().ID;
                spinningModelScript.ChangeOutlineColor(playerManager.PlayerColors[playerID]);
            }
            
            _instantiatedPivots.Add(pivot);
            spinningModel.transform.localPosition = modelPivotOffset;
            SetObjectLayer(spinningModel, LayerMask.NameToLayer("UI"));
        }
        
        private void SetObjectLayer(GameObject model, int layer)
        {
            model.layer = layer;
            Transform[] children = model.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                child.gameObject.layer = layer;
            }
        }

        public void OnCleanWinUI(Component sender, object[] data)
        {
            foreach (GameObject pivot in _instantiatedPivots)
            {
                if (pivot == centerPivot) continue;
              
                Destroy(pivot);
            }
        }
    }
}