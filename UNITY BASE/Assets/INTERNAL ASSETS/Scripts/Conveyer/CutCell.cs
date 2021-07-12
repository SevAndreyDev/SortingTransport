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

        private void Awake()
        {
            GameManager.OnStartGame -= OnStartGame;
            GameManager.OnStartGame += OnStartGame;
        }

        #region Events
        private void OnStartGame()
        {
            _content.SetActive(false);
        }
        #endregion
    }
}