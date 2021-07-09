using System;
using Spine;
using UnityEngine;

namespace EnglishKids.SortingTransport
{    
    public class Robot : SpineAnimationController
    {
        public enum AnimationKinds
        {
            Start,
            WorkUploadYellow,
            WorkBackIndicator,
            WorkSteam,
            AfterWorkIdle,
            LookAtCar
        }

        [Serializable]
        public class AnimationBlock
        {
            public AnimationKinds kind;
            public AnimationModule animation;
        }

        //==================================================
        // Fields
        //==================================================

        [Header("Spine Animations")]
        [SerializeField] private AnimationBlock[] _animationBlocks;

        private AnimationBlock _currentAnimation;
        
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
                    _currentAnimation = item;
                    return;
                }
            }

            Debug.LogError(string.Format("Couldn't find actual animation: {0}", kind));
        }

        #region Events
        protected override void OnAnimationComplete(TrackEntry trackEntry)
        {
            base.OnAnimationComplete(trackEntry);

            if (_currentAnimation != null && _currentAnimation.kind == AnimationKinds.LookAtCar)
            {
                Play(AnimationKinds.Start);
            }
        }
        #endregion
    }
}