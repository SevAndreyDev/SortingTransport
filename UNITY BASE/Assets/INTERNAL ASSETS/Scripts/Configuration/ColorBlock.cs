using System;
using UnityEngine;

namespace EnglishKids.SortingTransport
{
    [CreateAssetMenu(fileName = "Color Block", menuName = "Sorting Transport/Color Block", order = 51)]
    public class ColorBlock : ScriptableObject
    {
        [Serializable]
        public class TransportElement
        {
            public TransportKinds kind;
            public Sprite sprite;
            public float scale = 1f;
            public Speach speach;
        }

        //==================================================
        // Fields
        //==================================================

        [Space]
        [SerializeField] private ColorKinds _kind;
        [SerializeField] private Speach _speach;

        [Header("Base View Settings")]
        [SerializeField] private Sprite _background;
        [SerializeField] private float _scaleFactor = 1f;

        [Header("Transport")]
        [SerializeField] private TransportElement[] _transportElements;

        //==================================================
        // Properties
        //==================================================

        public ColorKinds Kind { get { return _kind; } }
        public Speach ColorSpeach { get { return _speach; } }
        public Sprite Background { get { return _background; } }
        public float ScaleFactor { get { return _scaleFactor; } }
        public TransportElement[] TransportElements { get { return _transportElements; } }
    }
}