using System;
using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public class TransportPivot : ViewObject
    {
        //==================================================
        // Fields
        //==================================================

        [SerializeField] RectTransform _backgroundsPanel;
        [SerializeField] TransportKinds _kind;
        [SerializeField] private Audio _transportSound;
        
        //==================================================
        // Properties
        //==================================================
        
        public RectTransform BackgroundsPanel { get { return _backgroundsPanel; } }
        public TransportKinds TransportKind { get { return _kind; } }
        public ColorKinds ColorKind { get; private set; }
        public Audio TransportSound { get { return _transportSound; } }

        //==================================================
        // Methods
        //==================================================

        public override void Activate(params object[] args)
        {
            base.Activate(args);

            this.ColorKind = (ColorKinds)args[0];

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