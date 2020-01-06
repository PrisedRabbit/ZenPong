using DG.Tweening;
using UnityAtoms;
using UnityAtoms.SceneMgmt;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PongGame
{
    public sealed class PauseMenuManager : MonoBehaviour
    {
        #pragma warning disable 0649
        
        [SerializeField] private ChangeScene quitToMainMenuAction;
        [SerializeField] private BoolEvent pauseGameEvent;
        [SerializeField] private CanvasGroup pauseGroup;

        // TODO: replace with atom event
        public void OnBackButton()
        {
            UnPause();
        }

        public void OnQuitButton()
        {
            Time.timeScale = 1.0f;
            quitToMainMenuAction.Do();
        }

        public void Pause()
        {
            Time.timeScale = 0.0f;
            pauseGroup.alpha = 0f;
            pauseGroup.gameObject.SetActive(true);
            pauseGroup.DOFade(1.0f, 0.25f).SetUpdate(true);
            pauseGameEvent.Raise(true);
        }

        public void UnPause()
        {
            pauseGroup.DOFade(0.0f, 0.25f).SetUpdate(true).OnComplete(() =>
            {
                pauseGroup.gameObject.SetActive(false);
                Time.timeScale = 1.0f;
                pauseGameEvent.Raise(false);
            });
        }
    }
}