using System;
using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public class TransportPivot : ViewObject
    {
        //==================================================
        // Fields
        //==================================================

        [SerializeField] TransportKinds _kind;
        
        //==================================================
        // Properties
        //==================================================
        
        public TransportKinds TransportKind { get { return _kind; } }
        public ColorKinds ColorKind { get; private set; }
        
        //==================================================
        // Methods
        //==================================================

        public void Activate(ColorKinds kind)
        {
            this.ColorKind = kind;

            DragElement.OnElementWasActivated -= OnElementWasActivated;
            DragElement.OnElementWasActivated += OnElementWasActivated;
        }

        #region Events
        private void OnElementWasActivated(TransportKinds transport, ColorKinds color, Action<TransportPivot> callback)
        {
            if (_kind == transport && this.ColorKind == color)
            {
                callback?.Invoke(this);
            }
        }
        #endregion
    }
}