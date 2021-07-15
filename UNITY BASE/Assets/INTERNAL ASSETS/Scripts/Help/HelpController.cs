using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace EnglishKids.SortingTransport
{
    public class HelpController : ViewObject
    {
        //==================================================
        // Fields
        //==================================================

        [Space]
        [SerializeField] private RectTransform _cursorTransform;
        [SerializeField] private CanvasGroup _cursorCanvas;
        [SerializeField] private float _cursorYOffset;
        [SerializeField] private RectTransform _leftPivot;
        [SerializeField] private RectTransform _rightPivot;

        [Header("Animation Settings")]
        [SerializeField] private float _timeToStart;
        [SerializeField] private TweenAnimation _movingTween;
        [SerializeField] private TweenAnimation _fadeTween;
        [SerializeField] private float _delayInterval = 0.1f;
        
        private Sequence _sequence;
        private Conveyer _conveyer;
        private bool _checkHelp;
        private float _startDuration;

        //==================================================
        // Methods
        //==================================================

        public override void Activate(params object[] args)
        {
            base.Activate(args);

            _conveyer = (Conveyer)args[0];
            _startDuration = _timeToStart;
            _checkHelp = true;
        }

        public override void Deactivate()
        {
            base.Deactivate();

            _checkHelp = false;

            if (_sequence != null)
                _sequence.Kill();

            _cursorCanvas.alpha = 0f;
        }

        protected override void Subscribe(EventManager.Subscribes subscribe)
        {
            base.Subscribe(subscribe);
            _eventsManager.RefreshEventListener(GameEvents.Action.ToString(), OnActionWasInvoke, subscribe);
        }

        private void Update()
        {
            if (_checkHelp)
            {
                _startDuration -= Time.deltaTime;

                if (_startDuration <= 0f)
                {
                    PlayHelpAnimation(_conveyer.GetActualConveyerItemsList());
                    _startDuration = _timeToStart;
                }
            }
        }

        private void PlayHelpAnimation(List<ConveyerItem> list)
        {
            ConveyerItem target = FindActualItem(list);

            if (target == null)
                return;

            PrepareCursor(target);

            if (_sequence != null)
                _sequence.Kill();

            _sequence = CreateSequence(_leftPivot, _delayInterval);

            _sequence.OnComplete(() =>
            {
                _startDuration = _timeToStart;

                PrepareCursor(target);
                _sequence = CreateSequence(_rightPivot, 0f);

                _sequence.OnComplete(() =>
                {
                    _startDuration = _timeToStart;
                }
                );
            });
        }

        private void PrepareCursor(ConveyerItem target)
        {
            _cursorTransform.localPosition = target.CachedTransform.localPosition + Vector3.up * _cursorYOffset;
            _cursorCanvas.alpha = 0f;            
        }

        private ConveyerItem FindActualItem(List<ConveyerItem> list)
        {
            int middle = list.Count / 2;

            if (middle > 0 && middle < list.Count)
            {
                if (!list[middle].InSlot)
                    return list[middle];
            }

            foreach (ConveyerItem item in list)
            {
                if (!item.InSlot)
                    return item;
            }

            return null;
        }

        private Sequence CreateSequence(RectTransform pivot, float delay)
        {
            float insertTime = Mathf.Clamp(_movingTween.Duration - _fadeTween.Duration, 0f, _movingTween.Duration);

            var sequence = DOTween.Sequence();

            sequence.Append(_cursorTransform.DOLocalMove(pivot.localPosition + Vector3.up * _cursorYOffset, _movingTween.Duration)).SetEase(_movingTween.Ease);
            sequence.Insert(0f, _cursorCanvas.DOFade(1f, _fadeTween.Duration)).SetEase(_fadeTween.Ease);
            sequence.Insert(insertTime, _cursorCanvas.DOFade(0f, _fadeTween.Duration)).SetEase(_fadeTween.Ease);

            if (delay > 0f)
                sequence.AppendInterval(delay);

            return sequence;
        }

        #region Events
        private void OnActionWasInvoke(object[] args)
        {
            _startDuration = _timeToStart;

            if (_sequence != null)
                _sequence.Kill();

            _cursorCanvas.alpha = 0f;
        }
        #endregion
    }
}