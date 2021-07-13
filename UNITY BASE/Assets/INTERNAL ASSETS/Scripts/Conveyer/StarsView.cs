using System;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace EnglishKids.SortingTransport
{
    public class StarsView : ViewObject
    {
        [Serializable]
        private class TweenAnimation
        {
            public float duration;
            public Ease ease;
        }

        //==================================================
        // Fields
        //==================================================

        [Space]
        [SerializeField] CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _starsLabel;

        [Header("Tween Animation")]
        [SerializeField] private TweenAnimation _fadeTween;
        
        //private GameManager _manager;
        private int _leftStars;

        //==================================================
        // Methods
        //==================================================

        private void Awake()
        {
            Initialize();
            
            GameManager.OnChangeStarsCount -= OnChangeStarsCount;
            GameManager.OnChangeStarsCount += OnChangeStarsCount;
        }
        
        public void Show()
        {
            _starsLabel.text = _manager.Stars.ToString();
            _leftStars = _manager.StarsPerLevel;

            float duration = Mathf.Abs(1f - _canvasGroup.alpha) * _fadeTween.duration;

            var secuance = DOTween.Sequence();
            secuance.Append(_canvasGroup.DOFade(1f, duration)).SetEase(_fadeTween.ease);
        }

        public void Hide()
        {
            float duration = _canvasGroup.alpha * _fadeTween.duration;

            var secuance = DOTween.Sequence();
            secuance.Append(_canvasGroup.DOFade(0f, duration)).SetEase(_fadeTween.ease);
        }

        #region Events
        private void OnChangeStarsCount(int count)
        {
            _starsLabel.text = count.ToString();
            _leftStars--;

            if (_leftStars <= 0)
                _manager.State = GameStates.Reset;
        }
        #endregion
    }
}