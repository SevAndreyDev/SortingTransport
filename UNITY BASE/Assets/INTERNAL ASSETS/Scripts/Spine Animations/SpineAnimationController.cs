using System;
using UnityEngine;
using Spine.Unity;
using Spine;

namespace EnglishKids.SortingTransport
{
    public class SpineAnimationController : MonoBehaviour
    {
        [Serializable]
        public class AnimationModule
        {
            public AnimationReferenceAsset animation;
            public float speed = 1f;
            public bool loop;
        }

        //==================================================
        // Fields
        //==================================================

        [SerializeField] private SkeletonGraphic _animation;

        protected string _currentAnimationName;
        private Action<string> OnAnimationCompleteHandler;

        //==================================================
        // Properties
        //==================================================

        public float Width { get { return _animation.Skeleton.Data.Width; } }
        public float Height { get { return _animation.Skeleton.Data.Height; } }
        public float Scale { get { return _animation.skeletonDataAsset.scale; } }
        public bool IsPlaying { get; protected set; }
        public bool IsLooping { get; protected set; }
        
        //public bool IsPlaying { get { return !string.IsNullOrEmpty(_currentAnimation) && _animation.AnimationState.GetCurrent(0).IsComplete; } }

        //==================================================
        // Methods
        //==================================================

        protected void PlayAnimation(AnimationReferenceAsset targetAnimation, bool loop, float animationSpeed)
        {
            if (!targetAnimation.name.Equals(_currentAnimationName))
            {                
                this.IsPlaying = true;
                this.IsLooping = loop;

                //_animation.Skeleton.SetToSetupPose();
                _animation.AnimationState.ClearTrack(0);
                _animation.AnimationState.SetAnimation(0, targetAnimation, loop).TimeScale = animationSpeed;

                if (!loop)
                {
                    _animation.AnimationState.Complete -= OnAnimationComplete;
                    _animation.AnimationState.Complete += OnAnimationComplete;
                }

                _currentAnimationName = targetAnimation.name;
            }                        
        }

        protected void PlayAnimation(AnimationModule animationModule)
        {
            PlayAnimation(animationModule.animation, animationModule.loop, animationModule.speed);
        }

        public void SetStartPose()
        {
            _animation.Skeleton.SetToSetupPose();
        }
                
        #region Events
        protected virtual void OnAnimationComplete(TrackEntry trackEntry)
        {
            _animation.AnimationState.Complete -= OnAnimationComplete;
            _animation.AnimationState.ClearTrack(0);
            _currentAnimationName = string.Empty;

            this.IsPlaying = false;
        }
        #endregion
    }
}