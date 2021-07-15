using System.Collections;
using System;
using UnityEngine.EventSystems;
using UnityEngine;
using DG.Tweening;

namespace EnglishKids.SortingTransport
{
    public class Star : PoolItem, IPointerDownHandler
    {
        private enum States
        {
            Idle,
            Moving,
            InSlot
        }

        [Serializable]
        private class TweenAnimation
        {
            public float duration;
            public Ease ease;
        }

        //==================================================
        // Fields
        //==================================================

        [Header("Base Settings")]
        [SerializeField] private TweenAnimation _moveTween;
        [SerializeField] private TweenAnimation _scaleTween;
        [SerializeField] private float _toScaleFactor;

        [Header("Particles Settings")]
        [SerializeField] private ParticleSystem _particles;
        [SerializeField] private float _particlesDuration = 2.5f;
        [SerializeField] private float _separateDuration = 0.25f;

        private RectTransform _target;

        private bool _checkParticles;
        private bool _resetParticles;
        private float _duration;

        private States _state;
        
        //==================================================
        // Properties
        //==================================================

        //==================================================
        // Methods
        //==================================================

        public void Activate(StarsView uiTarget)
        {
            _target = uiTarget.CachedTransform;

            _resetParticles = false;
            _duration = _particlesDuration;
            _checkParticles = true;

            _state = States.Idle;

            _particles.Play(true);
        }
                
        public void Deactivate()
        {
            _state = States.Idle;
            _checkParticles = false;

            this.Pool.Put(this.gameObject);
        }

        private void Update()
        {
            if (!_checkParticles)
                return;
            
            switch (_state)
            {
                case States.Idle:
                case States.Moving:
                    if (_resetParticles)
                    {
                        if (_particles.isPlaying)
                            return;

                        if (_duration > 0f)
                        {
                            _duration -= Time.deltaTime;
                            return;
                        }                        

                        _particles.Play(true);
                        _duration = _particlesDuration;
                        _resetParticles = false;
                    }
                    else
                    {
                        _duration -= Time.deltaTime;

                        if (_duration <= 0f)
                        {
                            _duration = _separateDuration;
                            _particles.Stop(true);
                            _resetParticles = true;
                        }
                    }
                    break;

                case States.InSlot:
                    //if (_particles.isPlaying)
                    //    return;

                    Deactivate();
                    break;
            }
        }
                               
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_state == States.Idle)
            {
                _state = States.Moving;

                this.CachedTransform.SetParent(_manager.DragField);

                var sequance = DOTween.Sequence();
                sequance.Append(this.CachedTransform.DOMove(_target.position, _moveTween.duration)).SetEase(_moveTween.ease);
                sequance.Insert(0f, this.CachedTransform.DOScale(_toScaleFactor, _scaleTween.duration)).SetEase(_scaleTween.ease);

                sequance.OnComplete(() =>
                {
                    _manager.Stars++;
                    _state = States.InSlot;
                    //_particles.Stop(true);
                    //Deactivate();
                });
            }
        }
    }
}