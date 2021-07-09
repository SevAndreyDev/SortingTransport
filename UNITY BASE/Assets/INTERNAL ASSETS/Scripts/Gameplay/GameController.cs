using System.Collections;
using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public class GameController : MonoBehaviour
    {
        //==================================================
        // Fields
        //==================================================

        [Space]
        [SerializeField] private Robot _robot;
        [SerializeField] private Mike _mike;
        [SerializeField] private Conveyer _conveyer;

        private GameManager _manager;

        //==================================================
        // Properties
        //==================================================

        //==================================================
        // Methods
        //==================================================

        private void Awake()
        {
            _manager = GameManager.Instance;
            
            GameManager.OnStartCutScene -= OnStartCutScene;
            GameManager.OnStartCutScene += OnStartCutScene;
            GameManager.OnStartGame -= OnStartGame;
            GameManager.OnStartGame += OnStartGame;

            _conveyer.Initialize();
        }

        private IEnumerator CutSceneProcess()
        {
            const float SHORT_DELAY = 0.4f;

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
            yield return new WaitWhile(() => _robot.IsPlaying);
            _mike.Show();
            yield return new WaitForSeconds(SHORT_DELAY);
            _robot.Play(Robot.AnimationKinds.AfterWorkIdle);
                        
            yield return new WaitWhile(() => _mike.IsTweenPlaying);
            _mike.Play(Mike.AnimationKinds.Speach);
            AudioManager.Instance.PlaySpeach(Audio.Speach);
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
            print("Game");
            yield return null;
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
        #endregion
    }
}