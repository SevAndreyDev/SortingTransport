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

        private GameManager _manager;

        //==================================================
        // Properties
        //==================================================

        public Image Background { get { return _background; } }
        public Orientations Orientation { get { return _orientation; } }
        public ColorBlock ColorBlockData { get; private set; }
        
        //==================================================
        // Methods
        //==================================================

        public void Initialize()
        {
            _manager = GameManager.Instance;

            Vector3 direction = _orientation == Orientations.Left ? Vector3.left : Vector3.right;
            
            float width = this.CachedTransform.rect.size.x;
            float offset = (width + _manager.ConveyerWidth * _manager.ScaleFactor) * GameConstants.HALF_FACTOR - _shiftOffset;

            Vector3 position = this.CachedTransform.localPosition;
            position.x = direction.x * offset;

            this.CachedTransform.localPosition = position;
        }

        public void Activate()
        {
            this.ColorBlockData = _orientation == Orientations.Left ? _manager.LeftDataBlock : _manager.RightDataBlock;
            _background.sprite = this.ColorBlockData.Background;

            foreach (TransportPivot item in _pivots)
            {
                item.Activate(this.ColorBlockData.Kind);
            }
        }
    }
}