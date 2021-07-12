using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public class PoolItem : ViewObject
    {
        //==================================================
        // Fields
        //==================================================

        [SerializeField] private PoolObjectKinds _objectKind;

        //==================================================
        // Properties
        //==================================================

        public string Key { get { return _objectKind.ToString(); } }
        public ObjectPool Pool { get; set; }                
    }
}