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
        [SerializeField] private GameStatePositionTween _backgroundTween;
        [SerializeField] private GameStatePositionTween _robotTween;
        [SerializeField] private GameField _leftField;
        [SerializeField] private GameField _rightField;
        
        [Space]
        [SerializeField] private Conveyer _conveyer;

        private GameManager _manager;
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
            _audio = AudioManager.Instance;
            
            GameManager.OnStartCutScene -= OnStartCutScene;
            GameManager.OnStartCutScene += OnStartCutScene;
            GameManager.OnStartGame -= OnStartGame;
            GameManager.OnStartGame += OnStartGame;
            GameManager.OnStartStarMode -= OnStartStarMode;
            GameManager.OnStartStarMode += OnStartStarMode;

            _conveyer.Initialize();

            _backgroundTween.Initialize();
            _robotTween.Initialize(_robot);

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
            yield return new WaitWhile(() => _robot.IsPlaying);
            _robot.Play(Robot.AnimationKinds.WorkBackIndicator);
            yield return new WaitWhile(() => _robot.IsPlaying);
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

            _conveyer.BuildTransportCell();
            yield return new WaitForEndOfFrame();
            _conveyer.Move();
        }

        private IEnumerator StarModeProcess()
        {
            _conveyer.BuildStarCell();
            yield return new WaitForSeconds(SHORT_DELAY);
            _conveyer.Move();
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
        #endregion
    }
}