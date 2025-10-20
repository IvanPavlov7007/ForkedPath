using System.Collections;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;

public sealed class InputSelectScene : MonoBehaviour
{
    public SceneAsset nextScene;
    AsyncOperation asyncLoad;

    private IEnumerator Start()
    {
        if (nextScene != null)
        {
            asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextScene.name);
            asyncLoad.allowSceneActivation = false;
        }
        else
        {
            Debug.LogWarning("Next scene is not assigned in InputSelectScene.");
        }
        yield return null;
    }

    public void SelectMobileInput()
    {
        MobileUIManager.mobileUIActive = true;
        Run.After(0.2f, () => {
            loadNextScene();
        });
    }

    public void SelectKeyboardInput()
    {
        MobileUIManager.mobileUIActive = false;
        Run.After(0.2f, () => {
            loadNextScene();
        });
    }

    void loadNextScene()
    {
        if (asyncLoad != null)
        {
            asyncLoad.allowSceneActivation = true;
        }
        else
        {
            Debug.LogWarning("Async load operation is not initialized.");
        }
    }
}
