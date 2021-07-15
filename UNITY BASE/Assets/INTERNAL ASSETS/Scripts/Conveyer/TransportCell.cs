using System;

namespace EnglishKids.SortingTransport
{
    public class TransportCell : ViewObject
    {
        public event Action OnCellIsEmpty;

        //==================================================
        // Properties
        //==================================================

        public int ContentItemsCount { get; private set; }
                                
        //==================================================
        // Methods
        //==================================================
                        
        public void Configure(int contentCount)
        {
            this.ContentItemsCount = contentCount;
            OnCellIsEmpty = null;
        }

        protected override void Subscribe(EventManager.Subscribes subscribe)
        {
            base.Subscribe(subscribe);

            DragElement.OnElementWasUsed -= OnElementWasUsed;
            if (subscribe == EventManager.Subscribes.Subscribe)
            {
                DragElement.OnElementWasUsed += OnElementWasUsed;
            }

        }
        
        #region Events
        private void OnElementWasUsed()
        {
            this.ContentItemsCount--;

            if (this.ContentItemsCount <= 0)
            {
                Deactivate();
                OnCellIsEmpty?.Invoke();
            }
        }
        #endregion
    }
}