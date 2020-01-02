using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup pauseGroup;

    // TODO: replace with atom event
    public void OnBackButton()
    {
        UnPause();
    }

    public void OnQuitButton()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

    public void Pause()
    {
        Time.timeScale = 0.0f;
        pauseGroup.alpha = 0f;
        pauseGroup.gameObject.SetActive(true);
        pauseGroup.DOFade(1.0f, 0.25f).SetUpdate(true);
    }

    public void UnPause()
    {
        pauseGroup.DOFade(0.0f, 0.25f).SetUpdate(true).OnComplete(() => { pauseGroup.gameObject.SetActive(false); Time.timeScale = 1.0f; });
    }
}