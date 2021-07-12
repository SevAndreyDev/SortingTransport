using System.Collections.Generic;
using UnityEngine;

namespace EnglishKids.SortingTransport
{
    [CreateAssetMenu(fileName = "Factory", menuName = "Sorting Transport/Factory", order = 50)]
    public class Factory : ScriptableObject
    {
        //==================================================
        // Fields
        //==================================================

        [SerializeField] private int _prewarmCount;
        [SerializeField] private List<PoolItem> _prefabs;

        public Dictionary<string, ObjectPool> _gamePools;

        //==================================================
        // Methods
        //==================================================

        public void Init(GameObject objectsContainer)
        {
            if (_gamePools == null)
                _gamePools = new Dictionary<string, ObjectPool>();

            foreach (PoolItem item in _prefabs)
            {
                string key = item.Key;

                if (string.IsNullOrEmpty(key))
                    continue;

                if (_gamePools.ContainsKey(key))
                {
                    _gamePools[key].SetContainer(objectsContainer.transform);
                }
                else
                {
                    ObjectPool pool = new ObjectPool(item.gameObject);
                    pool.SetContainer(objectsContainer.transform);
                    if (_prewarmCount > 0)
                        pool.GenerateObjectsInPool(_prewarmCount);

                    _gamePools.Add(key, pool);
                }
            }
        }

        public PoolItem GenerateItem(string key, Transform container = null)
        {
            PoolItem target = _gamePools.ContainsKey(key) ? _gamePools[key].Get().GetComponent<PoolItem>() : null;

            if (target != null)
            {
                target.Pool = _gamePools[key];
                if (container != null)
                    target.CachedTransform.SetParent(container);
            }

            return target;
        }

        public void Clear()
        {
            foreach (KeyValuePair<string, ObjectPool> item in _gamePools)
            {
                item.Value.Clear();
            }

            _gamePools.Clear();
        }
    }
}