using UnityEngine;
using System.Collections.Generic;

namespace EnglishKids.SortingTransport
{
	public class ObjectPool 
	{
        //==================================================
        // Fields
        //==================================================

        private GameObject _prefab = null;
		private List<GameObject> _objectsList = null;
        private Transform _container = null;

        //==================================================
        // Properties
        //==================================================

        public int Count
		{
			get { return _objectsList.Count; }
		}

        public bool HasPrefab
        {
            get { return (_prefab == null) ? false : true; }
        }

        public bool HasContainer
        {
            get { return (_container == null) ? false : true; }
        }

        //==================================================
        // Constructors
        //==================================================

        public ObjectPool()
		{
            _objectsList = new List<GameObject>();
			_prefab = null;
		}

        public ObjectPool(GameObject prefab)
        {
            _objectsList = new List<GameObject>();
            _prefab = prefab;
        }

        public ObjectPool(string prefabAssetsPath)
        {
            _objectsList = new List<GameObject>();
            _prefab = Resources.Load(prefabAssetsPath) as GameObject;
        }

        //==================================================
        // Methods
        //==================================================

        public void SetPrefab(GameObject prefab)
		{
			_objectsList.Clear();
            _prefab = prefab;
		}

        public void SetPrefab(string prefabAssetsPath)
        {
            _objectsList.Clear();
            _prefab = Resources.Load(prefabAssetsPath) as GameObject;
        }

        public void SetContainer(Transform parent)
        {
			_container = parent;

            for (int i = 0; i < _objectsList.Count; i++)
                _objectsList[i].transform.SetParent(parent, true);
        }

        public void GenerateObjectsInPool(int count)
		{
			if (_prefab == null)
            {
				Debug.LogError ("Prefab is null");
				return;
			}
            
			for (int i = 0; i < count; i++) 
			{
				GameObject newObject = GameObject.Instantiate(_prefab, _container) as GameObject;
				newObject.SetActive(false);
                _objectsList.Add (newObject);
			}
		}

        public GameObject Get()
        {
            if (_prefab == null)
            {
                Debug.LogError("Prefab is null");
                return null;
            }

            GameObject resultObject = null;

            if (_objectsList.Count == 0)
            {
                resultObject = GameObject.Instantiate(_prefab) as GameObject;
                resultObject.SetActive(false);
            }
            else
            {
                resultObject = _objectsList[_objectsList.Count - 1];
                _objectsList.RemoveAt(_objectsList.Count - 1);
            }

			resultObject.transform.SetParent(null);

            return resultObject;
        }

        public void Put(GameObject objectForPool, bool clearParent = false) 
		{
			objectForPool.SetActive(false);

            if (clearParent)
                objectForPool.transform.SetParent(null);
            else if (_container != null)
                objectForPool.transform.SetParent(_container);

			_objectsList.Add(objectForPool);
        }

        public void Clear()
		{
			for (int i = _objectsList.Count-1; i >= 0; i--)
				GameObject.Destroy(_objectsList [i]);
			_objectsList.Clear();
		}        
	}
}
