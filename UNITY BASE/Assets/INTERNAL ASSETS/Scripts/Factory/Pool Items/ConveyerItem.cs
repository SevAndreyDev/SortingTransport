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
        // Properties
        //==================================================

        public bool InSlot { get { return _dragElement.InSlot; } }

        //==================================================
        // Methods
        //==================================================

        public override void Activate(params object[] args)
        {
            base.Activate(args);

            ColorBlock data = (ColorBlock)args[0];
            ColorBlock.TransportElement transport = (ColorBlock.TransportElement)args[1];
                        
            _dragElement.Activate(data, transport);
        }

        public override void Deactivate()
        {
            base.Deactivate();
            
            _dragElement.Deactivate();
            this.Pool.Put(this.gameObject);
        }

        protected override void Subscribe(EventManager.Subscribes subscribe)
        {
            base.Subscribe(subscribe);

            _eventsManager.RefreshEventListener(GameEvents.PrepareToResetGame.ToString(), OnPrepareToResetGame, subscribe);
            _eventsManager.RefreshEventListener(GameEvents.ResetGameSceneObjects.ToString(), OnResetGame, subscribe);
        }

        #region Events
        private void OnPrepareToResetGame(object[] args)
        {
            _dragElement.ConnectToPivot();
        }

        private void OnResetGame(object[] args)
        {             
            Deactivate();
        }
        #endregion
    }
}