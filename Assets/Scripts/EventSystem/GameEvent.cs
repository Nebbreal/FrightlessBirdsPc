using System;
using System.Collections.Generic;
using UnityEngine;

namespace EventSystem
{
    [CreateAssetMenu(fileName = "GameEvent", menuName = "EventSystem/GameEvent")]
    public class GameEvent : ScriptableObject
    {
        public List<GameEventListener> listeners = new();
        
        public void Raise(Component sender, params object[] data)
        {
            //Trigger all listeners
            foreach (GameEventListener listener in listeners.ToArray())
            {
                try
                {
                    listener.OnEventRaised(sender, data);
                }
                catch (Exception ex)
                {
                    Exception exceptionToThrow = new Exception("GameEvent: " + this.name + " threw exception with message: " + ex.Message, ex);
                    Debug.LogException(exceptionToThrow);
                }
            }
        }

        #region ListenerManagement
        public void RegisterListener(GameEventListener listener)
        {
            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
            }
        }

        public void UnregisterListener(GameEventListener listener)
        {
            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);
            }
        }

        #endregion
    }
}
