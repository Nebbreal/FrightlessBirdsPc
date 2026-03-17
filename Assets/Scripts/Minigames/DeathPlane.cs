using System;
using Minigames.FrozenFeathers;
using UnityEngine;

namespace Minigames
{
    public class DeathPlane : MonoBehaviour
    {
        [SerializeField]
        private MinigameManager minigameManager;

        public void OnTriggerEnter(Collider other)
        {
            minigameManager.EliminatePlayer(other.gameObject);
        }
    }
}