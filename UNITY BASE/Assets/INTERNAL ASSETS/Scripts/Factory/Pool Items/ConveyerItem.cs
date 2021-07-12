using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public class ConveyerItem : PoolItem
    {
        //==================================================
        // Fields
        //==================================================
                
        [SerializeField] private DragElement _dragElement;

        //==================================================
        // Methods
        //==================================================

        public void Configure(ColorBlock data, ColorBlock.TransportElement transport)
        {
            _dragElement.Activate(data, transport);
        }

        public void Deactivate()
        {
            _dragElement.Deactivate();
            this.Pool.Put(this.gameObject);
        }
    }
}