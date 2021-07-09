using System;
using UnityEngine;
using DG.Tweening;

namespace EnglishKids.SortingTransport
{
    public class GameStatePositionTween : MonoBehaviour
    {
        [Serializable]
        public class TweenAnimation
        {
            public float duration;
            public Ease ease;
        }

        //==================================================
        // Fields
        //==================================================

        [Header("Tween Settings")]
        [SerializeField] private RectTransform _target;

        //==================================================
        // Properties
        //==================================================

        //==================================================
        // Methods
        //==================================================
    }
}