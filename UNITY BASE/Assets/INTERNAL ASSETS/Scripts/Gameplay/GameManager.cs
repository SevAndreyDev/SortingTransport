using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using DG.Tweening;

namespace EnglishKids.SortingTransport
{
    public enum GameStates
    {
        Loading,
        CutScene,
        Game,
        StarMode,
        Reset
    }
    
    public class GameManager : MonoSingleton<GameManager>
    {
        [Serializable]
        private class ResolutionSettings
        {
            public float screenSizeFactor;
            public float scale;
        }

        public static event Action OnStartCutScene;
        public static event Action OnStartGame;
        public static event Action OnStartStarMode;
        public static event Action OnStartResetGame;        
        public static event Action<int> OnChangeStarsCount;

        //==================================================
        // Fields
        //==================================================

        [Header("Screen Size Settings")]
        [SerializeField] private Vector2 _referenceSize;
        [SerializeField] private ResolutionSettings _rectangleRatio;
        [SerializeField] private ResolutionSettings _squareRatio;

        [Header("Base Settings")]
        [SerializeField] private float _topRobotBarOffset;
        [SerializeField] private float _conveyerWidth;
        [SerializeField] private RectTransform _dragField;
        [SerializeField] private RectTransform _leftAnswerAnchor;
        [SerializeField] private RectTransform _rightAnswerAnchor;
        [SerializeField] private int _starsPerLevel;

        [Header("Configuration Data")]
        [SerializeField] private BlocksAssembly _blocksAssembly;
        [SerializeField] private ColorBlock[] _colorBlocks;

        private Vector2 _canvasToScreenRation;
        private int _stars;
        private GameStates _state;
                        
        //==================================================
        // Properties
        //==================================================

        public GameStates State
        {
            get { return _state; }

            set
            {
                _state = value;

                switch (_state)
                {
                    case GameStates.Loading:
                        break;

                    case GameStates.CutScene:
                        OnStartCutScene?.Invoke();
                        break;

                    case GameStates.Game:
                        ConfigureNewGame();
                        OnStartGame?.Invoke();
                        break;

                    case GameStates.StarMode:
                        OnStartStarMode?.Invoke();
                        break;

                    case GameStates.Reset:
                        OnStartResetGame?.Invoke();
                        break;
                }
            }
        }

        public int StarsPerLevel { get { return _starsPerLevel; } }
        public int Stars { get { return _stars; } set { _stars = value; OnChangeStarsCount?.Invoke(_stars); } }

        public float CanvasHeight { get { return _referenceSize.y; } }
        public float CanvasWidth { get { return (((float)Screen.width) / Screen.height) * _referenceSize.y; } }

        public float TopRobotBarOffset { get { return _topRobotBarOffset; } }
        public float ConveyerWidth { get { return _conveyerWidth; } }
        public float ScaleFactor { get; private set; }

        public RectTransform DragField { get { return _dragField; } }
        public RectTransform LeftAnswerAnchor { get { return _leftAnswerAnchor; } }
        public RectTransform RightAnswerAnchor { get { return _rightAnswerAnchor; } }

        public ColorBlock LeftDataBlock { get; private set; }
        public ColorBlock RightDataBlock { get; private set; }                

        //==================================================
        // Methods
        //==================================================

        protected override void Init()
        {
            base.Init();
                        
            const float BASE_SCALE = 1f;
            
            float currentRatio = ((float)Screen.width) / Screen.height;
            float targetRatio = _referenceSize.x / _referenceSize.y;

            if (currentRatio > targetRatio)
            {
                float size = Mathf.Abs(_rectangleRatio.screenSizeFactor - targetRatio);
                float progress = Mathf.Clamp01((currentRatio - targetRatio) / size);

                this.ScaleFactor = Mathf.Lerp(BASE_SCALE, _rectangleRatio.scale, progress);
            }
            else if (currentRatio < targetRatio)
            {
                float size = Mathf.Abs(targetRatio - _squareRatio.screenSizeFactor);
                float progress = Mathf.Clamp01((currentRatio - _squareRatio.screenSizeFactor) / size);

                this.ScaleFactor = Mathf.Lerp(_squareRatio.scale, BASE_SCALE, progress);
            }
            else
            {
                this.ScaleFactor = BASE_SCALE;
            }

            _canvasToScreenRation = new Vector2(this.CanvasWidth / Screen.width, this.CanvasHeight / Screen.height);
                        
            this.State = GameStates.Loading;
        }

        private void Start()
        {
            this.State = GameStates.CutScene;
        }

        public Vector3 GetCanvasPoint(Vector3 screenPosition)
        {
            Vector3 position = new Vector3(_canvasToScreenRation.x * screenPosition.x, _canvasToScreenRation.y * screenPosition.y, 1f);
            position.x -= this.CanvasWidth * GameConstants.HALF_FACTOR;
            position.y -= this.CanvasHeight * GameConstants.HALF_FACTOR;
            return position;
        }
        
        private void ConfigureNewGame()
        {
            ColorBlock left, right;
            _blocksAssembly.ConfigureColorBlocks(out left, out right);

            this.LeftDataBlock = left;
            this.RightDataBlock = right;
        }

        private void ClearEvents()
        {
            // Clear static events
            OnStartCutScene = null;
            OnStartGame = null;
            OnStartStarMode = null;
            OnStartResetGame = null;            
            OnChangeStarsCount = null;

            EventManager.Instance.Clear();

            DragElement.ClearEvents();
        }

        #region Events
        public void OnCloseGame(int targetSceneIndex)
        {
            ClearEvents();
            DOTween.Clear();

            // Load target scene
            SceneManager.LoadScene(targetSceneIndex);
        }
        #endregion
    }
}