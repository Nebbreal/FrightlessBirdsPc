using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace MinigameFramework.UIElements
{
    public class WinningPlayerModelInstantiator : MonoBehaviour
    {
        [SerializeField] 
        private GameObject pivot;

        public void OnUpdateWinUIModel(Component sender, object[] modelsObj)
        {
            if (modelsObj.First() is not List<GameObject> winnerModels) return;
            foreach (GameObject winnerModel in winnerModels)
            {
                GameObject spinningModel = Instantiate(winnerModel, pivot.transform);
                spinningModel.transform.localPosition = Vector3.zero;
                SetObjectLayer(spinningModel, LayerMask.NameToLayer("UI"));
            }
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

        private void ClearChildren()//TODO: trigger this when leaving win screen
        {
            foreach (Transform child in pivot.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}