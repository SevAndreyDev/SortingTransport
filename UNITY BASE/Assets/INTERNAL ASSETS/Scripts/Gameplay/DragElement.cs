using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace EnglishKids.SortingTransport
{
    public class DragElement : ViewObject, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private enum States
        {
            InConveyer,
            Drag,
            MoveToConveyer,
            MoveToSlot,
            InSlot,
            TransportSound
        }

        public static event Action<TransportKinds, ColorKinds, Action<TransportPivot>> OnElementWasActivated;        
        public static event Action OnElementWasUsed;
        
        //==================================================
        // Fields
        //==================================================

        [Space]
        [SerializeField] private RectTransform _conveyerPanel;
        [SerializeField] private Image _image;
        [SerializeField] private float _canvasScaleFactor;

        [Header("Tween Settings")]
        [SerializeField] TweenAnimation _toConveyer;
        [SerializeField] TweenAnimation _toSlot;
        [SerializeField] TweenAnimation _conveyerScale;
        [SerializeField] TweenAnimation _slotScale;

        private ColorBlock _data;
        private ColorBlock.TransportElement _transportData;

        private TransportPivot _targetPivot;
        private float _referenceDistance;
        
        private float _possibleTargetOffset;
        private Vector3 _conveyerPosition;
        private Vector3 _startPosition;        
        private Audio _speachKind;

        private Sequence _scaleUpSequence;
        private States _state;

        //==================================================
        // Properties
        //==================================================

        public bool InSlot { get { return _state == States.InSlot || _state == States.TransportSound || _state == States.MoveToSlot; } }

        //==================================================
        // Methods
        //==================================================

        public static void ClearEvents()
        {
            OnElementWasActivated = null;
            OnElementWasUsed = null;
        }

        protected override void Init()
        {
            base.Init();

            _referenceDistance = _manager.ScaledScreenWidth;            
            _startPosition = this.CachedTransform.localPosition;
        }

        public void Activate(ColorBlock data, ColorBlock.TransportElement transport)
        {
            Initialize();
            
            _state = States.InConveyer;

            _targetPivot = null;
            _data = data;
            _transportData = transport;
                        
            _image.sprite = transport.sprite;
            _image.SetNativeSize();
            _image.raycastTarget = true;

            this.CachedTransform.localScale = Vector3.one * _transportData.conveyerScale;

            OnElementWasActivated?.Invoke(_transportData.kind, _data.Kind, OnElementsWasActivatedCallback);
        }

        public void Deactivate()
        {
            this.CachedTransform.SetParent(_conveyerPanel);
            this.CachedTransform.localPosition = _startPosition;
        }

        private void Update()
        {
            switch (_state)
            {
                case States.TransportSound:
                    _audio.PlaySingleSound(_targetPivot.TransportSound);
                    break;
            }
        }

        public void ConnectToPivot()
        {
            this.CachedTransform.SetParent(_targetPivot.CachedTransform, true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _eventManager.InvokeEvent(GameEvents.Action.ToString());

            switch (_state)
            {
                case States.InConveyer:
                    {
                        _conveyerPosition = this.CachedTransform.position;
                        this.CachedTransform.SetParent(_manager.DragField);
                        _state = States.Drag;

                        _eventManager.InvokeEvent(GameEvents.RefreshSpeachButton.ToString(), _data.Kind, _transportData.speach);

                        var sequance = DOTween.Sequence();
                        sequance.Append(this.CachedTransform.DOScale(_data.DragScaleFactor, _conveyerScale.Duration)).SetEase(_conveyerScale.Ease);

                        _scaleUpSequence = sequance;

                        sequance.OnComplete(() =>
                        {
                            _scaleUpSequence = null;
                        });
                    }
                    break;

                case States.InSlot:
                    _eventManager.InvokeEvent(GameEvents.RefreshSpeachButton.ToString(), _data.Kind, _transportData.speach);
                    _state = States.TransportSound;
                    break;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _eventManager.InvokeEvent(GameEvents.Action.ToString());

            switch (_state)
            {
                case States.Drag:
                    StopDrag();
                    break;

                case States.TransportSound:
                    _state = States.InSlot;
                    break;
            }
        }
                
        public void OnDrag(PointerEventData eventData)
        {
            _eventManager.InvokeEvent(GameEvents.Action.ToString());

            if (_state == States.Drag)
            {
                this.CachedTransform.localPosition += new Vector3(eventData.delta.x, eventData.delta.y, 0f);
            }
        }

        private bool IsCorrectAnswer()
        {
            if (this.CachedTransform.position.x <= _manager.LeftAnswerAnchor.position.x && _manager.LeftDataBlock.Kind == _data.Kind)
                return true;

            if (this.CachedTransform.position.x >= _manager.RightAnswerAnchor.position.x && _manager.RightDataBlock.Kind == _data.Kind)
                return true;

            return false;
        }

        private void StopDrag()
        {
            var sequance = DOTween.Sequence();

            if (IsCorrectAnswer())
            {
                _state = States.MoveToSlot;
                OnElementWasUsed?.Invoke();

                _audio.PlaySpeach(_transportData.speach, _data.ColorSpeach);

                float duration = CalculateDuration(_targetPivot.CachedTransform.position, _toSlot.Duration);
                sequance.Append(this.CachedTransform.DOMove(_targetPivot.CachedTransform.position, duration)).SetEase(_toSlot.Ease);
                sequance.Insert(0f, this.CachedTransform.DOScale(_transportData.scale, Mathf.Min(duration, _slotScale.Duration))).SetEase(_slotScale.Ease);

                sequance.OnComplete(() =>
                {
                    this.CachedTransform.SetParent(_targetPivot.BackgroundsPanel);
                    _state = States.InSlot;
                    _eventManager.InvokeEvent(GameEvents.RefreshSpeachButton.ToString(), _data.Kind, _data.ColorSpeach);

                    StarsEffectItem star = Spawner.Instance.Get(PoolObjectKinds.StarEffect) as StarsEffectItem;
                    star.Play(this.CachedTransform);
                    _audio.PlaySound(Audio.CorrectAnswer);
                });
            }
            else
            {
                _state = States.MoveToConveyer;
                this.CachedTransform.SetParent(_conveyerPanel);

                if (_scaleUpSequence != null)
                    _scaleUpSequence.Kill();

                float duration = CalculateDuration(_conveyerPosition, _toConveyer.Duration);
                sequance.Append(this.CachedTransform.DOLocalMove(_startPosition, duration)).SetEase(_toConveyer.Ease);
                sequance.Insert(0f, this.CachedTransform.DOScale(_transportData.conveyerScale, Mathf.Min(_conveyerScale.Duration, duration))).SetEase(_conveyerScale.Ease);

                sequance.OnComplete(() =>
                {
                    _state = States.InConveyer;

                    if (duration > 0f)
                        _audio.PlaySound(Audio.WrongAnswer);
                });
            }
        }

        private float CalculateDuration(Vector3 target, float baseDuration)
        {
            float distance = Vector3.Distance(this.CachedTransform.position, target);
            float duration = Mathf.Clamp01(distance / _referenceDistance) * baseDuration;

            //print(distance / _canvasScaleFactor);

            return duration / _canvasScaleFactor;
        }

        #region Events
        private void OnElementsWasActivatedCallback(TransportPivot pivot)
        {
            _targetPivot = pivot;
        }
        #endregion
    }
}