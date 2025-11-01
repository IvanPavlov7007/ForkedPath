using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Pixelplacement.Singleton<GameManager>
{
    private IEnumerator CloseScene()
    {
        TransitionUI.Instance.FadeIn();
        yield return new WaitForSeconds(0.6f);
    }

    bool resettingLevel = false;
    void resetLevel()
    {
        if(resettingLevel) return;

        resettingLevel = true;
        IEnumerator ResetLevelCoroutine()
        {
            yield return CloseScene();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        StartCoroutine(ResetLevelCoroutine());
    }


    public static void RestartLevel()
    {
        Instance.resetLevel();
    }
}