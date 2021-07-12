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
        [SerializeField] private TweenAnimation _cutSceneTween;
        [SerializeField] private TweenAnimation _gameSceneTween;
        [SerializeField] private float _gameSceneOffset;
        [SerializeField] private bool _calculateRobotOffset;

        private GameManager _manager;
        private Vector3 _cutScenePosition;
        private float _robotSize;
        
        //==================================================
        // Properties
        //==================================================

        public bool IsTweenPlaying { get; private set; }

        //==================================================
        // Methods
        //==================================================

        public void Initialize(SpineAnimationController robot = null)
        {
            _manager = GameManager.Instance;
            _cutScenePosition = _target.localPosition;
            if (_calculateRobotOffset && robot != null)
            {
                _robotSize = _gameSceneOffset;
            }
        }

        public void MoveToCutScenePosition(bool immediately = false)
        {
            if (!this.IsTweenPlaying)
                PlayTweenAnimation(_cutScenePosition, _cutSceneTween, immediately);
        }

        public void MoveToGameScenePosition(bool immediately = false)
        {
            if (!this.IsTweenPlaying)
            {
                float offset = _calculateRobotOffset ? ((_manager.ReferenceScreenHeight + _gameSceneOffset) * GameConstants.HALF_FACTOR - _manager.TopRobotBarOffset) : _manager.ReferenceScreenHeight;
                Vector3 targetPoint = _cutScenePosition + Vector3.up * offset;

                PlayTweenAnimation(targetPoint, _gameSceneTween, immediately);
            }
        }               
        
        private void PlayTweenAnimation(Vector3 targetPoint, TweenAnimation tween, bool immediately)
        {
            if (immediately)
            {
                _target.localPosition = targetPoint;
                return;
            }

            this.IsTweenPlaying = true;

            var secuance = DOTween.Sequence();
            secuance.Append(_target.DOLocalMove(targetPoint, tween.duration)).SetEase(tween.ease);

            secuance.OnComplete(() =>
            {
                this.IsTweenPlaying = false;
            });
        }
    }
}