using System.Collections;
using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public class StarsEffectItem : PoolItem
    {
        //==================================================
        // Fields
        //==================================================

        [SerializeField] private StarEffect _starEffect;

        //==================================================
        // Methods
        //==================================================

        public void Play(RectTransform target)
        {
            this.CachedTransform.SetParent(target);
            this.CachedTransform.localScale = Vector3.one;
            this.CachedTransform.localPosition = Vector3.zero;

            _starEffect.gameObject.SetActive(true);
            _starEffect.Play();

            StartCoroutine(PlayProcess());
        }

        private IEnumerator PlayProcess()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitWhile(() => _starEffect.IsPlaying);

            _starEffect.gameObject.SetActive(false);
            this.Pool.Put(this.gameObject);
        }
    }
}