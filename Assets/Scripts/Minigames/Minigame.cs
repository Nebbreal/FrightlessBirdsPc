using System.Collections.Generic;
using UnityEngine;

namespace Minigames
{
    [CreateAssetMenu(fileName = "Minigame", menuName = "MinigameFramework/Minigame")]
    public class Minigame : ScriptableObject
    {
        public SceneField sceneToLoad;
        public string gameName;
        public string gameDescription;
        [Range(1, 20)]
        public int maxLives;
        [Range(1, 120)] 
        public int respawnTime;
        [Range(2, 4)] 
        public List<int> intendedPlayerCounts;
        [Range(1, 4)] 
        public List<int> teamSizes;
    }
}
