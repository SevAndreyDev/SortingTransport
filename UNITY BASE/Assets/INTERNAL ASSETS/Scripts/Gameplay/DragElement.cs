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

        [Header("Tween Settings")]
        [SerializeField] TweenAnimation _toConveyer;
        [SerializeField] TweenAnimation _toSlot;

        private ColorBlock _data;
        private ColorBlock.TransportElement _transportData;
        //private Audio _transportSound;

        private TransportPivot _targetPivot;
        //private RectTransform _targetPanel;
        private float _referenceDistance;
        
        private float _possibleTargetOffset;
        private Vector3 _conveyerPosition;
        private Vector3 _startPosition;        
        private Audio _speachKind;

        private States _state;

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

            _targetPivot = null;

            _state = States.InConveyer;

            _data = data;
            _transportData = transport;

            //_colorKind = data.Kind;
            //_transportKind = transport.kind;
            //_speach = transport.speach;

            _image.sprite = transport.sprite;
            _image.SetNativeSize();
            _image.transform.localScale = Vector3.one * transport.scale;
            _image.raycastTarget = true;

            OnElementWasActivated?.Invoke(_transportData.kind, _data.Kind, OnElementsWasActivatedCallback);
                                                
            //Robot targetRobot = _manager.LeftRobot.ColorKind == configItem.ColorBlockSettings.Kind ? _manager.LeftRobot : _manager.RightRobot;
            //_target = targetRobot.FindBodyPart(configItem.robotPart);
            //_possibleTargetOffset = configItem.maxDragOffset;

            //_speachKind = configItem.ColorBlockSettings.SpeachKind;
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

        public void OnPointerDown(PointerEventData eventData)
        {
            switch (_state)
            {
                case States.InConveyer:                    
                    {
                        _conveyerPosition = this.CachedTransform.position;
                        this.CachedTransform.SetParent(_manager.DragField);
                        _state = States.Drag;

                        _eventManager.InvokeEvent(GameEvents.RefreshSpeachButton.ToString(), _data.Kind, _transportData.speach);


                        //_audio.PlaySound(Audio.Pick);

                        //var sequance = DOTween.Sequence();
                        //sequance.Append(this.CachedTransform.DORotate(Vector3.zero, _rotationDuration));
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
            switch (_state)
            {
                case States.Drag:
                    StopDrag();
                    break;

                case States.TransportSound:
                    _state = States.InSlot;
                    break;
            }
            /*
            var sequance = DOTween.Sequence();

            //if (Vector3.Distance(_cachedTransform.position, _target.CachedTransform.position) <= _possibleTargetOffset)
            if (IsCorrectAnswer())
            {
                _image.raycastTarget = false;
                OnElementWasUsed?.Invoke();

                //StarController star = _manager.Effests.GetFreeStarEffect();
                //star.CachedTransform.position = _cachedTransform.position;
                //star.Play();

                //_audio.PlaySound(Audio.CorrectSlot);
                //_audio.PlaySpeach(_speachKind);

                //_cachedTransform.SetParent(_target.CachedTransform);

                sequance.Append(this.CachedTransform.DOMove(_targetPanel.position, _putInSlotDuration));

                sequance.OnComplete(() =>
                {
                    this.CachedTransform.SetParent(_targetPanel);
                });
            }
            else
            {
                this.CachedTransform.SetParent(_conveyerPanel);

                //sequance.Append(this.CachedTransform.DORotate(_startRotation, _rotationDuration));

                if (this.CachedTransform.localPosition != _startPosition)
                {
                    _image.raycastTarget = false;

                    //_audio.PlaySound(Audio.WrongSlot);
                    sequance.Append(this.CachedTransform.DOLocalMove(_startPosition, _returnMovementDuration));
                    //sequance.Insert(0f, this.CachedTransform.DOLocalMove(_startPosition, _returnMovementDuration));

                    

                    sequance.OnComplete(() =>
                    {
                        _image.raycastTarget = true;
                    });
                }
            }
            */
        }
                
        public void OnDrag(PointerEventData eventData)
        {
            if (_state == States.Drag)
            {
                this.CachedTransform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0f);
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

                //StarController star = _manager.Effests.GetFreeStarEffect();
                //star.CachedTransform.position = _cachedTransform.position;
                //star.Play();

                //_audio.PlaySound(Audio.CorrectSlot);
                //_audio.PlaySpeach(_speachKind);

                _audio.PlaySpeach(_transportData.speach);
                //_audio.PlaySpeach(_data.ColorSpeach);

                //_cachedTransform.SetParent(_target.CachedTransform);

                float duration = CalculateDuration(_targetPivot.CachedTransform.position, _toSlot.Duration);
                sequance.Append(this.CachedTransform.DOMove(_targetPivot.CachedTransform.position, duration)).SetEase(_toSlot.Ease);

                sequance.OnComplete(() =>
                {
                    this.CachedTransform.SetParent(_targetPivot.BackgroundsPanel);
                    _state = States.InSlot;
                    _eventManager.InvokeEvent(GameEvents.RefreshSpeachButton.ToString(), _data.Kind, _data.ColorSpeach);
                });
            }
            else
            {
                _state = States.MoveToConveyer;
                this.CachedTransform.SetParent(_conveyerPanel);

                //sequance.Append(this.CachedTransform.DORotate(_startRotation, _rotationDuration));

                //if (this.CachedTransform.localPosition != _startPosition)
                {
                    //_image.raycastTarget = false;

                    //_audio.PlaySound(Audio.WrongSlot);
                    float duration = CalculateDuration(_conveyerPosition, _toConveyer.Duration);
                    sequance.Append(this.CachedTransform.DOLocalMove(_startPosition, duration)).SetEase(_toConveyer.Ease);
                    //sequance.Insert(0f, this.CachedTransform.DOLocalMove(_startPosition, _returnMovementDuration));



                    sequance.OnComplete(() =>
                    {
                        _state = States.InConveyer;
                        //_image.raycastTarget = true;
                    });
                }
            }
        }

        private float CalculateDuration(Vector3 target, float baseDuration)
        {
            float distance = Vector3.Distance(this.CachedTransform.position, target);
            return Mathf.Clamp01(distance / _referenceDistance) * baseDuration;
        }

        #region Events
        private void OnElementsWasActivatedCallback(TransportPivot pivot)
        {
            //_targetPanel = pivot.CachedTransform;
            _targetPivot = pivot;
            //_transportSound = pivot.TransportSound;
        }
        #endregion
    }
}