using System;
using System.Collections.Generic;
using System.Linq;
using EventSystem;
using Minigames;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScoreboardManager : MonoBehaviour
{
    [NonSerialized] public Dictionary<string, int> PlayerScores = new();

    public void GainPoint(GameObject player)
    {
        PlayerScores[player.name]++;
    }

    public void OnUpdateScoreboard(Component sender, object[] modelsObj)
    {
        
    }
}
