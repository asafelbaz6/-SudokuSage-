using Levels;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace UI
{
    public class MenuButtons : MonoBehaviour
    {
        [ShowInInspector] private Fader _fader;

        [Inject]
        private void Construct(Fader fader)
        {
            _fader = fader;
        }
        public void StartGame()
        { 
            SceneManager.LoadSceneAsync(sceneBuildIndex: SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void BackToPreviousScene()
        {
            Debug.Log("Back To Previous Scene");
            if (_fader != null)
            {
                _fader.FadeOutBetweenLevels(LevelChange.Previous);
            }
        }

        public void QuitGame()
        {
            Debug.Log("Quit Game");
            Application.Quit();
        }
    }
}
