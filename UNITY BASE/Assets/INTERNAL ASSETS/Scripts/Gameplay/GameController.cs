using System.Collections;
using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public class GameController : MonoBehaviour
    {
        private const float SHORT_DELAY = 0.4f;

        //==================================================
        // Fields
        //==================================================
                
        [Header("Characters")]
        [SerializeField] private Robot _robot;
        [SerializeField] private Mike _mike;

        [Header("Base Settings")]
        [SerializeField] private HelpController _helpController;
        [SerializeField] private StarsView _starsView;
        [SerializeField] private GameStatePositionTween _backgroundTween;
        [SerializeField] private GameStatePositionTween _robotTween;
        [SerializeField] private GameField _leftField;
        [SerializeField] private GameField _rightField;

        [Header("Effects")]
        [SerializeField] private CloudController _cloudController;

        [Header("Speach Buttons")]
        [SerializeField] private SpeachButton _leftButton;
        [SerializeField] private SpeachButton _rightButton;
        
        [Space]
        [SerializeField] private Conveyer _conveyer;

        private GameManager _manager;
        private EventManager _eventManager;
        private AudioManager _audio;

        //==================================================
        // Properties
        //==================================================

        //==================================================
        // Methods
        //==================================================

        private void Awake()
        {
            _manager = GameManager.Instance;
            _eventManager = EventManager.Instance;
            _audio = AudioManager.Instance;
            
            GameManager.OnStartCutScene -= OnStartCutScene;
            GameManager.OnStartCutScene += OnStartCutScene;
            GameManager.OnStartGame -= OnStartGame;
            GameManager.OnStartGame += OnStartGame;
            GameManager.OnStartStarMode -= OnStartStarMode;
            GameManager.OnStartStarMode += OnStartStarMode;
            GameManager.OnStartResetGame -= OnStartResetGame;
            GameManager.OnStartResetGame += OnStartResetGame;

            _conveyer.Initialize();

            _backgroundTween.Initialize();
            _robotTween.Initialize(_robot);

            _cloudController.Initialize();
            _helpController.Initialize();

            _leftField.Initialize();
            _rightField.Initialize();
        }

        private IEnumerator CutSceneProcess()
        {
            _backgroundTween.MoveToCutScenePosition(true);
            _robotTween.MoveToCutScenePosition(true);
            _mike.Hide(true);
            _mike.Play(Mike.AnimationKinds.Idle);
            _robot.Play(Robot.AnimationKinds.Start);            
            _conveyer.BuildStartCell();

            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(SHORT_DELAY);

            _conveyer.Move();
            _robot.Play(Robot.AnimationKinds.LookAtCar);
            yield return new WaitWhile(() => _conveyer.IsMoving);
                        
            // Robot animations block
            _robot.Play(Robot.AnimationKinds.WorkUploadYellow);
            while (_robot.IsPlaying)
            {
                _audio.PlaySingleSound(Audio.UploadYellowPaint);
                yield return null;
            }
            _audio.StopSound(Audio.UploadYellowPaint);
            
            _robot.Play(Robot.AnimationKinds.WorkBackIndicator);
            while (_robot.IsPlaying)
            {
                _audio.PlaySingleSound(Audio.RobotWork);
                yield return null;
            }
            _audio.StopSound(Audio.RobotWork);
            //yield return new WaitWhile(() => _robot.IsPlaying);
            
            _robot.Play(Robot.AnimationKinds.WorkSteam);
            _audio.PlaySound(Audio.Steam);
            yield return new WaitWhile(() => _robot.IsPlaying);
            _mike.Show();
            yield return new WaitForSeconds(SHORT_DELAY);
            _robot.Play(Robot.AnimationKinds.AfterWorkIdle);
                                    
            yield return new WaitWhile(() => _mike.IsTweenPlaying);
            _mike.Play(Mike.AnimationKinds.Speach);
            AudioManager.Instance.PlaySpeach(Speach.SortByColor);
            yield return new WaitWhile(() => _mike.IsPlaying);
            _mike.Play(Mike.AnimationKinds.AfterSpeach);      // Bad final frame, record it with pause animation            
            yield return new WaitWhile(() => _mike.IsPlaying);
            _mike.Play(Mike.AnimationKinds.Idle);

            yield return new WaitForSeconds(SHORT_DELAY);

            _mike.Hide();
            yield return new WaitWhile(() => _mike.IsTweenPlaying);

            _manager.State = GameStates.Game;
        }

        private IEnumerator GameProcess()
        {
            _leftField.Activate();
            _rightField.Activate();

            yield return new WaitForSeconds(SHORT_DELAY);
            _backgroundTween.MoveToGameScenePosition();
            _robotTween.MoveToGameScenePosition();

            yield return new WaitWhile(() => _backgroundTween.IsTweenPlaying || _robotTween.IsTweenPlaying);

            _leftButton.Activate();
            _rightButton.Activate();

            _cloudController.Activate();

            _conveyer.BuildTransportCell();
            yield return new WaitForEndOfFrame();
            _conveyer.Move();

            yield return new WaitWhile(() => _conveyer.IsMoving);

            _helpController.Activate(_conveyer);
        }
                
        private IEnumerator StarModeProcess()
        {
            _leftButton.Deactivate();
            _rightButton.Deactivate();
            _helpController.Deactivate();

            _conveyer.BuildStarCell(_starsView);
            yield return new WaitForSeconds(SHORT_DELAY);
            _conveyer.Move();

            _starsView.Show();            
        }

        private IEnumerator ResetGameProcess()
        {
            yield return new WaitForSeconds(SHORT_DELAY);

            _cloudController.Deactivate();
            _starsView.Hide();

            yield return new WaitWhile(() => _cloudController.IsFading);
            yield return new WaitForSeconds(SHORT_DELAY);

            _eventManager.InvokeEvent(GameEvents.PrepareToResetGame.ToString());
            
            _backgroundTween.MoveToCutScenePosition();
            _robotTween.MoveToCutScenePosition();

            yield return new WaitWhile(() => _backgroundTween.IsTweenPlaying || _robotTween.IsTweenPlaying);
            
            _eventManager.InvokeEvent(GameEvents.ResetGameSceneObjects.ToString());

            _manager.State = GameStates.CutScene;
        }

        #region Events
        private void OnStartCutScene()
        {
            StartCoroutine(CutSceneProcess());
        }

        private void OnStartGame()
        {
            StartCoroutine(GameProcess());
        }

        private void OnStartStarMode()
        {
            StartCoroutine(StarModeProcess());
        }

        private void OnStartResetGame()
        {
            StartCoroutine(ResetGameProcess());
        }
        #endregion
    }
}