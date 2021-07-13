using System.Collections.Generic;
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
        public static event Action OnStartCutScene;
        public static event Action OnStartGame;
        public static event Action OnStartStarMode;
        public static event Action OnStartResetGame;        
        public static event Action<int> OnChangeStarsCount;

        //==================================================
        // Fields
        //==================================================

        [Header("Base Settings")]
        [SerializeField] private float _referenceScreenHeight;
        [SerializeField] private float _topRobotBarOffset;
        [SerializeField] private float _conveyerWidth;
        [SerializeField] private RectTransform _dragField;
        [SerializeField] private RectTransform _leftAnswerAnchor;
        [SerializeField] private RectTransform _rightAnswerAnchor;
        [SerializeField] private int _starsPerLevel;

        [Header("Configuration Data")]
        [SerializeField] private ColorBlock[] _colorBlocks;

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

        public float ReferenceScreenHeight { get { return _referenceScreenHeight; } }        
        public float TopRobotBarOffset { get { return _topRobotBarOffset; } }
        public float ConveyerWidth { get { return _conveyerWidth; } }
        public float ScaleFactor { get { return 1f; } }

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
            this.State = GameStates.Loading;
        }

        private void Start()
        {
            this.State = GameStates.CutScene;
        }
        
        private void ConfigureNewGame()
        {
            List<ColorBlock> blocks = new List<ColorBlock>(_colorBlocks);
            this.LeftDataBlock = ExtractRandomColorBlock(blocks);
            this.RightDataBlock = ExtractRandomColorBlock(blocks);
        }

        private ColorBlock ExtractRandomColorBlock(List<ColorBlock> blocks)
        {
            int index = UnityEngine.Random.Range(0, blocks.Count);
            ColorBlock target = blocks[index];
            blocks.RemoveAt(index);

            return target;
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