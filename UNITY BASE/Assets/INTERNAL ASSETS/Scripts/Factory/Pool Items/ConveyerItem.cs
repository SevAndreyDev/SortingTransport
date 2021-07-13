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

        public void Activate(ColorBlock data, ColorBlock.TransportElement transport)
        {
            Subscribe(EventManager.Subscribes.Subscribe);
            _dragElement.Activate(data, transport);
        }
                
        public void Deactivate()
        {
            Subscribe(EventManager.Subscribes.Unscribe);
            _dragElement.Deactivate();

            this.Pool.Put(this.gameObject);
        }

        protected override void Subscribe(EventManager.Subscribes subscribe)
        {
            base.Subscribe(subscribe);

            _eventManager.RefreshEventListener(GameEvents.ResetGameSceneObjects.ToString(), OnResetGame, subscribe);
        }

        #region Events
        private void OnResetGame(object[] args)
        {             
            Deactivate();
        }
        #endregion
    }
}