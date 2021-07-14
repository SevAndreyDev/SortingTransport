using System;
using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public class SpeachButtonsData : MonoBehaviour
    {
        [Serializable]
        private class DataItem
        {
            public Speach speach;
            public string text;
        }

        //==================================================
        // Fields
        //==================================================

        [Space]
        [SerializeField] private DataItem[] _data;
                        
        //==================================================
        // Methods
        //==================================================

        public string GetText(Speach speach)
        {
            foreach (DataItem item in _data)
            {
                if (item.speach == speach)
                    return item.text;
            }

            return string.Empty;
        }
    }
}