using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoad : MonoBehaviour
{
    public string newScene;
    private ScreenFader screenFader;

    private void Start()
    {
        screenFader = FindObjectOfType<ScreenFader>();
        if (screenFader == null)
        {
            Debug.LogError("[SceneLoad] No se encontró ScreenFader en la escena!" + SceneManager.GetActiveScene().name);
            return;
        }
        if (screenFader.fadeCanvasGroup.alpha >= 1f)
        {
            screenFader.FadeIn(3f);
        }
    }

    private IEnumerator WaitAndChange()
    {
        yield return new WaitForSeconds(3f);
        Change();
    }

    public void Change()
    {
        Debug.Log("Cambiando a " + newScene);

        // Verificar si la escena existe en Build Settings
        if (!SceneExistsInBuildSettings(newScene))
        {
            Debug.LogError($"[SceneLoad] La escena '{newScene}' no existe en Build Settings. Añádela en File > Build Settings.");
            return;
        }

        Time.timeScale = 1; // Asegurarse de que el tiempo esté normalizado al cambiar de escena
        StartCoroutine(ChangeSceneWithFade());
    }

    /// <summary>
    /// Verifica si una escena existe en Build Settings
    /// </summary>
    private bool SceneExistsInBuildSettings(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[SceneLoad] El nombre de la escena está vacío.");
            return false;
        }

        // Verificar por nombre o por path
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (sceneNameInBuild.Equals(sceneName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator ChangeSceneWithFade()
    {
        yield return screenFader.FadeOut(3f);
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(newScene);
        yield return loadOperation;
    }
}
