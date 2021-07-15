using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public class CutCell : ViewObject
    {
        //==================================================
        // Fields
        //==================================================

        [SerializeField] private GameObject _content;

        //==================================================
        // Methods
        //==================================================
        
        protected override void Init(params object[] args)
        {
            base.Init();

            _content.transform.localScale = Vector3.one * GameManager.Instance.ScaleFactor;

            GameManager.OnStartGame -= OnStartGame;
            GameManager.OnStartGame += OnStartGame;
            GameManager.OnStartResetGame -= OnResetGame;
            GameManager.OnStartResetGame += OnResetGame;
        }

        #region Events
        private void OnStartGame()
        {
            _content.SetActive(false);
        }

        private void OnResetGame()
        {
            _content.SetActive(true);
        }
        #endregion
    }
}