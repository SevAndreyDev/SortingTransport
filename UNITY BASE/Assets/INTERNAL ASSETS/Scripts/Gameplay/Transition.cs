using UnityEngine.SceneManagement;
using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public class Transition : MonoBehaviour
    {
        //==================================================
        // Methods
        //==================================================

        public void MakeTransitionByIndex(int sceneId)
        {
            SceneManager.LoadScene(sceneId);
        }

        public void MakeTransitionByName(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}