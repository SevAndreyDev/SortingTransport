using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public class CutSceneController : MonoBehaviour
    {
        //==================================================
        // Fields
        //==================================================

        [SerializeField] private Conveyer _conveyer;

        //==================================================
        // Properties
        //==================================================

        //==================================================
        // Methods
        //==================================================

        private void Awake()
        {
            GameManager.OnStartCutScene -= OnStartCutScene;
            GameManager.OnStartCutScene += OnStartCutScene;

            _conveyer.Initialize();
        }

        #region Events
        private void OnStartCutScene()
        {            
            _conveyer.BuildCutCell();
        }
        #endregion
    }
}