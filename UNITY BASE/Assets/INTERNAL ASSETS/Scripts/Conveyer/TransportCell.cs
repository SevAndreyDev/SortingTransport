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

        public void Activate()
        {
            DragElement.OnElementWasUsed -= OnElementWasUsed;
            DragElement.OnElementWasUsed += OnElementWasUsed;
        }

        public void Deactivate()
        {
            DragElement.OnElementWasUsed -= OnElementWasUsed;
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