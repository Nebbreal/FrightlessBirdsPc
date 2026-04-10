using System.Collections.Generic;
using UnityEngine;

namespace MinigameFramework
{
    public class Team
    {
        public int Lives;
        public int Size;
        public List<GameObject> Players = new();

        public Team(int size)
        {
            Size = size;
        }
    }
}