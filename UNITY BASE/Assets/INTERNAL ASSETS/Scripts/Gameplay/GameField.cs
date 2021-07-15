using UnityEngine.UI;
using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public class GameField : ViewObject
    {
        //==================================================
        // Fields
        //==================================================

        [SerializeField] private Image _background;
        [SerializeField] private Orientations _orientation;        
        [SerializeField] private float _shiftOffset;

        [Header("Pivots List")]
        [SerializeField] private TransportPivot[] _pivots;
                
        //==================================================
        // Properties
        //==================================================

        public Image Background { get { return _background; } }
        public Orientations Orientation { get { return _orientation; } }
        public ColorBlock ColorBlockData { get; private set; }

        //==================================================
        // Methods
        //==================================================

        protected override void Init(params object[] args)
        {
            base.Init();
            
            Vector3 direction = _orientation == Orientations.Left ? Vector3.left : Vector3.right;
            
            float width = this.CachedTransform.rect.size.x;
            float offset = (width + _manager.ConveyerWidth * _manager.ScaleFactor) * GameConstants.HALF_FACTOR - _shiftOffset;

            Vector3 position = this.CachedTransform.localPosition;
            position.x = direction.x * offset;

            this.CachedTransform.localPosition = position;
        }

        public override void Activate(params object[] args)
        {
            base.Activate(args);
        
            this.ColorBlockData = _orientation == Orientations.Left ? _manager.LeftDataBlock : _manager.RightDataBlock;
            _background.sprite = this.ColorBlockData.Background;

            foreach (TransportPivot item in _pivots)
            {
                item.Activate(this.ColorBlockData.Kind);
            }
        }

        #region Events
        public void OnBackgroundClick()
        {
            _audio.PlaySpeach(this.ColorBlockData.ColorSpeach);
            _eventsManager.InvokeEvent(GameEvents.RefreshSpeachButton.ToString(), this.ColorBlockData.Kind, this.ColorBlockData.ColorSpeach);
            _eventsManager.InvokeEvent(GameEvents.Action.ToString());
        }
        #endregion
    }
}