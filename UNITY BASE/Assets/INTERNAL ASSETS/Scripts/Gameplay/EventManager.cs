﻿using System.Collections.Generic;

namespace EnglishKids.SortingTransport
{
    public class EventManager : MonoSingleton<EventManager>
    {
        public enum Subscribes
        {
            Subscribe,
            Unscribe
        }

        public delegate void OnEvent(params object[] args);

        //==================================================
        // Fields
        //==================================================

        private Dictionary<string, List<OnEvent>> _listeners;

        //==================================================
        // Properties
        //==================================================

        public int ListenersCount { get { return _listeners.Count; } }

        //==================================================
        // Methods
        //==================================================

        protected override void Init()
        {
            base.Init();
        
            if (_listeners == null)
                _listeners = new Dictionary<string, List<OnEvent>>();
            else
                _listeners.Clear();
        }

        public void RefreshEventListener(string eventName, OnEvent listener, Subscribes state)
        {
            if (state == Subscribes.Subscribe)
                AddListener(eventName, listener);
            else
                RemoveListener(eventName, listener);
        }

        public void AddListener(string eventName, OnEvent listener)
        {
            List<OnEvent> listenList = null;

            if (_listeners.TryGetValue(eventName, out listenList))
            {
                if (!listenList.Contains(listener))
                    listenList.Add(listener);
            }
            else
            {
                listenList = new List<OnEvent> { listener };
                _listeners.Add(eventName, listenList);
            }
        }

        public void RemoveListener(string eventName, OnEvent listener)
        {
            List<OnEvent> listenList = null;

            if (_listeners.TryGetValue(eventName, out listenList))
            {
                listenList.Remove(listener);
            }
        }

        public void RemoveEvent(string eventName)
        {
            _listeners.Remove(eventName);
        }

        public void InvokeEvent(string eventName, params object[] args)
        {
            List<OnEvent> listenList = null;

            if (_listeners.TryGetValue(eventName, out listenList))
            {
                List<OnEvent> targetList = new List<OnEvent>(listenList);

                for (int i = 0; i < targetList.Count; i++)
                {
                    if (!targetList[i].Equals(null))
                    {
                        targetList[i](args);                        
                    }
                }
            }
        }

        public void Clear()
        {
            _listeners.Clear();
        }

        public void RemoveNullTargets()
        {
            var tempListeners = new Dictionary<string, List<OnEvent>>();

            foreach (KeyValuePair<string, List<OnEvent>> item in _listeners)
            {
                for (int i = item.Value.Count - 1; i >= 0; --i)
                {
                    if (item.Value[i].Target.Equals(null))
                    {
                        item.Value.RemoveAt(i);
                    }
                }

                if (item.Value.Count > 0)
                {
                    tempListeners.Add(item.Key, item.Value);
                }
            }

            _listeners = tempListeners;
        }
    }
}