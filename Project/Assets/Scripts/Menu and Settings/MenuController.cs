using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private SceneTransition sceneTransition;
    private bool isCooldown = false;

    private void Start()
    {
        sceneTransition = FindFirstObjectByType<SceneTransition>();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void NewGame()
    {
        if (isCooldown)
            return; // Si está en cooldown, no hace nada

        isCooldown = true; // Activamos el cooldown

        if (sceneTransition != null)
        {
            sceneTransition.LoadLevelNormal();
        }
        else
        {
            Debug.LogWarning("SceneTransition no encontrado en la escena.");
        }
        StartCoroutine(ResetCooldown());
    }

    private IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(10);
        isCooldown = false;
    }

    public void Continue()
    {
        SceneManager.LoadScene(1);
    }

    public void Bestiary()
    {
        SceneManager.LoadScene(3);
    }

    public void Settings()
    {
        SceneManager.LoadScene(2);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
