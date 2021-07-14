using UnityEngine.UI;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace EnglishKids.SortingTransport
{
    public class SpeachButton : ViewObject
    {
        //==================================================
        // Fields
        //==================================================

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Orientations _orientation;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private SpeachButtonsData _data;

        [Header("Tween Animation")]
        [SerializeField] private TweenAnimation _fadeTween;

        private ColorKinds _colorKind;
        private Speach _currentSpeach;
        private bool _isActive;

        //==================================================
        // Methods
        //==================================================

        private void Awake()
        {
            Initialize();
        }

        public void Activate()
        {
            ColorBlock block = _orientation == Orientations.Left ? _manager.LeftDataBlock : _manager.RightDataBlock;
            _colorKind = block.Kind;
                        
            SetText(_data.GetText(block.ColorSpeach));
            _currentSpeach = block.ColorSpeach;

            Subscribe(EventManager.Subscribes.Subscribe);

            // Tween
            float duration = Mathf.Abs(1f - _canvasGroup.alpha) * _fadeTween.Duration;

            var secuance = DOTween.Sequence();
            secuance.Append(_canvasGroup.DOFade(1f, duration)).SetEase(_fadeTween.Ease);

            _isActive = true;
        }

        public void Deactivate()
        {
            Subscribe(EventManager.Subscribes.Subscribe);

            float duration = _canvasGroup.alpha * _fadeTween.Duration;

            var secuance = DOTween.Sequence();
            secuance.Append(_canvasGroup.DOFade(0f, duration)).SetEase(_fadeTween.Ease);

            _isActive = false;
        }

        protected override void Subscribe(EventManager.Subscribes subscribe)
        {
            base.Subscribe(subscribe);

            _eventManager.RefreshEventListener(GameEvents.RefreshSpeachButton.ToString(), OnRefreshSpeachButton, subscribe);
        }

        private void SetText(string text)
        {
            _label.text = text;
        }

        #region Events
        private void OnRefreshSpeachButton(object[] args)
        {
            ColorKinds kind = (ColorKinds)args[0];

            if (_colorKind == kind)
            {
                Speach speach = (Speach)args[1];
                SetText(_data.GetText(speach));
                
                _currentSpeach = speach;
            }
        }

        public void OnButtonClick()
        {
            if (_isActive)
            {
                _audio.PlaySpeach(_currentSpeach);
            }
        }
        #endregion
    }
}