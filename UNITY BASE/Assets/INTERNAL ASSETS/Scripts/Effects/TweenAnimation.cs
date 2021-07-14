using System;
using UnityEngine;
using DG.Tweening;

namespace EnglishKids.SortingTransport
{
    [Serializable]
    public class TweenAnimation
    {
        //==================================================
        // Fields
        //==================================================

        [SerializeField] private float _duration = 1f;
        [SerializeField] private Ease _ease;
        
        //==================================================
        // Properties
        //==================================================

        public float Duration { get { return _duration; } }
        public Ease Ease { get { return _ease; } }
    }
}