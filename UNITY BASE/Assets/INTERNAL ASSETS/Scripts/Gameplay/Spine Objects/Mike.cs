using System;
using UnityEngine;
using DG.Tweening;

namespace EnglishKids.SortingTransport
{
    public class Mike : SpineAnimationController
    {
        public enum AnimationKinds
        {
            Idle,
            Speach,
            AfterSpeach,
            Jump
        }

        [Serializable]
        public class AnimationBlock
        {
            public AnimationKinds kind;
            public AnimationModule animation;
        }

        [Serializable]
        public class TweenAnimation
        {
            public float duration;
            public Ease ease;
        }

        //==================================================
        // Fields
        //==================================================

        [Header("Pivot Settings")]
        [SerializeField] private RectTransform _cachedTransform;
        [SerializeField] private RectTransform _hidePoint;
        [SerializeField] private RectTransform _showPoint;
        [SerializeField] private TweenAnimation _hideTween;
        [SerializeField] private TweenAnimation _showTween;

        [Header("Spine Animations")]
        [SerializeField] private AnimationBlock[] _animationBlocks;

        //==================================================
        // Properties
        //==================================================

        public bool IsTweenPlaying { get; private set; }

        //==================================================
        // Methods
        //==================================================

        public void Play(AnimationKinds kind)
        {
            foreach (AnimationBlock item in _animationBlocks)
            {
                if (item.kind == kind)
                {
                    PlayAnimation(item.animation);
                    return;
                }
            }

            Debug.LogError(string.Format("Couldn't find actual animation: {0}", kind));
        }

        public void Show(bool immediately = false)
        {
            if (!this.IsTweenPlaying)
                PlayTweenAnimation(_showPoint.localPosition, _showTween, immediately);
        }

        public void Hide(bool immediately = false)
        {
            if (!this.IsTweenPlaying)
                PlayTweenAnimation(_hidePoint.localPosition, _showTween, immediately);
        }

        private void PlayTweenAnimation(Vector3 targetPoint, TweenAnimation tween, bool immediately = false)
        {
            if (immediately)
            {
                _cachedTransform.localPosition = targetPoint;
                return;
            }

            this.IsTweenPlaying = true;

            var secuance = DOTween.Sequence();
            secuance.Append(_cachedTransform.DOLocalMove(targetPoint, tween.duration)).SetEase(tween.ease);

            secuance.OnComplete(() =>
            {
                this.IsTweenPlaying = false;
            });
        }
    }
}