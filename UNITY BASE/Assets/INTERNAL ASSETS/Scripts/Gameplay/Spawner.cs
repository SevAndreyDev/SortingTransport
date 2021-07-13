using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public class Spawner : MonoSingleton<Spawner>
    {
        //==================================================
        // Fields
        //==================================================

        [SerializeField] private GameObject _defaultContainer;
        [SerializeField] private Factory _factory;

        //==================================================
        // Methods
        //==================================================

        protected override void Init()
        {
            base.Init();

            _factory.Clear();
            _factory.Init(_defaultContainer);
        }

        public PoolItem Get(PoolObjectKinds kind, RectTransform parent = null)
        {
            PoolItem target = _factory.GenerateItem(kind.ToString(), parent);
            target.CachedTransform.localScale = Vector3.one;

            return target;
        }
    }
}