using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

namespace EnglishKids.SortingTransport
{
    public class Conveyer : ViewObject
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
        [SerializeField] private float _referenceScreenHeight;

        [Header("Prefabs")]
        [SerializeField] private ViewObject _emptyCell;
        [SerializeField] private ViewObject _startCell;

        [Header("Tween Animations")]
        [SerializeField] private TweenAnimation _cutSceneTween;

        private Queue<float> _movingDistanceQueue;
        private Vector2 _startPosition;
        
        //==================================================
        // Properties
        //==================================================

        public bool IsMoving { get; private set; }
        
        //==================================================
        // Methods
        //==================================================

        public void Initialize()
        {
            _movingDistanceQueue = new Queue<float>();
            _startPosition = this.CachedTransform.anchoredPosition;
        }

        public void BuildStartCell()
        {            
            _movingDistanceQueue.Clear();

            if (_startPosition != this.CachedTransform.anchoredPosition)
            {
                this.CachedTransform.anchoredPosition = _startPosition;
            }
            else
            {
                ViewObject cell = Instantiate(_emptyCell, this.CachedTransform);
                cell.CachedTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _referenceScreenHeight);
                cell.SetActive(true);

                cell = Instantiate(_startCell, this.CachedTransform);
                cell.SetActive(true);
            }
        }

        public void BuildTransportCell()
        {

        }

        public void Move()
        {
            switch (GameManager.Instance.State)
            {
                case GameStates.CutScene:
                    {
                        const float HALF_FACTOR = 0.5f;
                        float height = (_startCell.CachedTransform.rect.height + _referenceScreenHeight) * HALF_FACTOR;

                        Move(_cutSceneTween, height);
                    }
                    break;
            }
        }

        private void Move(TweenAnimation animation, float distance)
        {
            this.IsMoving = true;

            Vector3 position = this.CachedTransform.localPosition;
            Vector3 target = position + Vector3.down * distance;

            var secuance = DOTween.Sequence();
            secuance.Append(this.CachedTransform.DOLocalMove(target, animation.duration)).SetEase(animation.ease);

            secuance.OnComplete(() =>
            {
                this.IsMoving = false;                                    
            });            
        }
    }
}