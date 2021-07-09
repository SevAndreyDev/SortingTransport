using System;
using UnityEngine.SceneManagement;

namespace EnglishKids.SortingTransport
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public static event Action OnStartCutScene;
        public static event Action OnStartGame;
        public static event Action OnStartStarMode;

        //==================================================
        // Fields
        //==================================================

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
                        OnStartGame?.Invoke();
                        break;

                    case GameStates.StarMode:
                        OnStartStarMode?.Invoke();
                        break;
                }
            }
        }

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

        public void OnCloseGame(int targetSceneIndex)
        {
            // Clear static events
            OnStartCutScene = null;
            OnStartGame = null;
            OnStartStarMode = null;

            // Load target scene
            SceneManager.LoadScene(targetSceneIndex);
        }
    }
}