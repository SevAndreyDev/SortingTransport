using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace EnglishKids.SortingTransport
{
    public class DragElement : ViewObject, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public static event Action<TransportKinds, ColorKinds, Action<TransportPivot>> OnElementWasActivated;        
        public static event Action OnElementWasUsed;
        

        //==================================================
        // Fields
        //==================================================

        [Space]
        [SerializeField] private RectTransform _conveyerPanel;
        [SerializeField] private Image _image;        
        [SerializeField] private float _returnMovementDuration = 2f;
        [SerializeField] private float _putInSlotDuration = 0.3f;

        private GameManager _manager;
        private AudioManager _audio;

        private TransportKinds _transportKind;
        private ColorKinds _colorKind;

        private RectTransform _targetPanel;



        private float _possibleTargetOffset;
        private Vector3 _startPosition;
        private Audio _speachKind;

        private bool _wasInitialized;

        //==================================================
        // Methods
        //==================================================

        public static void ClearEvents()
        {
            OnElementWasActivated = null;
            OnElementWasUsed = null;
        }

        private void Initialize()
        {
            if (!_wasInitialized)
            {
                _manager = GameManager.Instance;
                _audio = AudioManager.Instance;

                _startPosition = this.CachedTransform.localPosition;
                
                _wasInitialized = true;
            }
        }

        public void Activate(ColorBlock data, ColorBlock.TransportElement transport)
        {
            Initialize();
                        
            _colorKind = data.Kind;
            _transportKind = transport.kind;

            _image.sprite = transport.sprite;
            _image.SetNativeSize();
            _image.transform.localScale = Vector3.one * transport.scale;
            _image.raycastTarget = true;

            OnElementWasActivated?.Invoke(_transportKind, _colorKind, OnElementsWasActivatedCallback);
                                                
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

        public void OnPointerDown(PointerEventData eventData)
        {
            this.CachedTransform.SetParent(_manager.DragField);

            //this.CachedTransform.SetParent(_manager.DragField);

            //_audio.PlaySound(Audio.Pick);

            //var sequance = DOTween.Sequence();
            //sequance.Append(this.CachedTransform.DORotate(Vector3.zero, _rotationDuration));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
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
        }
                
        public void OnDrag(PointerEventData eventData)
        {
            this.CachedTransform.position += new Vector3(eventData.delta.x, eventData.delta.y, 0f);
        }

        private bool IsCorrectAnswer()
        {
            if (this.CachedTransform.position.x <= _manager.LeftAnswerAnchor.position.x && _manager.LeftDataBlock.Kind == _colorKind)
                return true;

            if (this.CachedTransform.position.x >= _manager.RightAnswerAnchor.position.x && _manager.RightDataBlock.Kind == _colorKind)
                return true;

            return false;
        }

        #region Events
        private void OnElementsWasActivatedCallback(TransportPivot pivot)
        {
            _targetPanel = pivot.CachedTransform;
        }
        #endregion
    }
}