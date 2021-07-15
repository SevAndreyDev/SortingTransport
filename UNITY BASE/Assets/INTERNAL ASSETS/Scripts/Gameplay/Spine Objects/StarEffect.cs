using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public class StarEffect : SpineAnimationController
    {
        //==================================================
        // Fields
        //==================================================

        [SerializeField] private AnimationModule _effect;
                
        //==================================================
        // Methods
        //==================================================

        public void Play()
        {
            PlayAnimation(_effect);            
        }
    }
}