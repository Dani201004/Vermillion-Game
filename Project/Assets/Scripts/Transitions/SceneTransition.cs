using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    public int returnSceneIndex; // Valor por defecto en caso de no asignarse desde otro script

    [SerializeField] private Animator transitionAnim;

    private AsyncOperation operation;

    private bool isSubscribed = false; // Bandera para evitar múltiples llamadas a SuscribeEvent
    private bool isSubscribedBattle = false;

    private Canvas childCanvas;

    private Travel travel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        childCanvas = GetComponentInChildren<Canvas>();
        if (childCanvas != null)
        {
            childCanvas.gameObject.SetActive(false);
        }
    }

    public void LoadLevel(string sceneName)
    {
        if (childCanvas != null)
        {
            childCanvas.gameObject.SetActive(true);
        }
        StartCoroutine(LoadSceneSave(sceneName));
    }
    private IEnumerator LoadSceneSave(string sceneName)
    {
        transitionAnim.SetTrigger("End");

        // Esperar un pequeño tiempo para la animación de salida
        yield return new WaitForSeconds(1f);

        // Cargar la escena 9 inmediatamente
        SceneManager.LoadScene(9);

        // Esperar a que la escena 9 se cargue completamente
        yield return new WaitForSeconds(1f);

        // Cargar la escena de forma asíncrona usando el nombre pasado
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        yield return new WaitForSeconds(6f);

        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        // La escena se ha cargado; obtenemos la escena activa.
        Scene activeScene = SceneManager.GetActiveScene();
        Debug.Log("Escena activa: " + activeScene.name);

        // Verificar si los datos se han cargado; si no, cargar la partida.
        if (SaveGameManager.Instance.CurrentGameData == null)
        {
            Debug.Log("currentGameData es null; cargando datos...");
            SaveGameManager.Instance.LoadGameAndApply();
            yield return null;
        }

        // Restaura al jugador en la posición guardada.
        SaveGameManager.Instance.RestorePlayer();

        transitionAnim.SetTrigger("Start");
        isSubscribed = false;
    }
    public void LoadLevelNormal()
    {
        if (childCanvas != null)
        {
            childCanvas.gameObject.SetActive(true);
        }
        StartCoroutine(LoadSceneNormal());
    }
    private IEnumerator LoadSceneNormal()
    {
        transitionAnim.SetTrigger("End");

        // Esperar un pequeño tiempo para la animación de salida
        yield return new WaitForSeconds(1f);

        // Cargar la escena 9 inmediatamente
        SceneManager.LoadScene(9);

        // Esperar a que la escena 9 se cargue completamente
        yield return new WaitForSeconds(1f);

        // Cargar la siguiente escena de manera asíncrona sin activarla aún
        operation = SceneManager.LoadSceneAsync(4);
        operation.allowSceneActivation = false;

        yield return new WaitForSeconds(6f);

        // Esperar hasta que la escena esté casi completamente cargada
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f) // Cuando la escena está lista
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        // Activar animación de entrada
        transitionAnim.SetTrigger("Start");
    }
    public void LoadLevelMenu()
    {
        if (childCanvas != null)
        {
            childCanvas.gameObject.SetActive(true);
        }
        StartCoroutine(LoadSceneMenu());
    }
    private IEnumerator LoadSceneMenu()
    {
        transitionAnim.SetTrigger("End");

        // Esperar un pequeño tiempo para la animación de salida
        yield return new WaitForSeconds(1f);

        // Cargar la escena 9 inmediatamente
        SceneManager.LoadScene(9);

        // Esperar a que la escena 9 se cargue completamente
        yield return new WaitForSeconds(1f);

        // Cargar la siguiente escena de manera asíncrona sin activarla aún
        operation = SceneManager.LoadSceneAsync(0);
        operation.allowSceneActivation = false;

        yield return new WaitForSeconds(3f);

        // Esperar hasta que la escena esté casi completamente cargada
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f) // Cuando la escena está lista
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        // Activar animación de entrada
        transitionAnim.SetTrigger("Start");
    }
    public void LoadLevelCinematic()
    {
        if (childCanvas != null)
        {
            childCanvas.gameObject.SetActive(true);
        }
        StartCoroutine(LoadSceneCinematic());
    }
    private IEnumerator LoadSceneCinematic()
    {
        transitionAnim.SetTrigger("End");

        // Esperar un pequeño tiempo para la animación de salida
        yield return new WaitForSeconds(1f);

        // Cargar la escena 9 inmediatamente
        SceneManager.LoadScene(9);

        // Esperar a que la escena 9 se cargue completamente
        yield return new WaitForSeconds(1f);

        // Cargar la siguiente escena de manera asíncrona sin activarla aún
        operation = SceneManager.LoadSceneAsync(5);
        operation.allowSceneActivation = false;

        yield return new WaitForSeconds(3f);

        // Esperar hasta que la escena esté casi completamente cargada
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f) // Cuando la escena está lista
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        // Activar animación de entrada
        transitionAnim.SetTrigger("Start");
    }
    public void LoadLevelTavern()
    {
        if (childCanvas != null)
        {
            childCanvas.gameObject.SetActive(true);
        }
        StartCoroutine(LoadSceneTavern());
    }
    private IEnumerator LoadSceneTavern()
    {
        transitionAnim.SetTrigger("End");

        // Esperar un pequeño tiempo para la animación de salida
        yield return new WaitForSeconds(1f);

        // Cargar la escena 9 inmediatamente
        SceneManager.LoadScene(9);

        // Esperar a que la escena 9 se cargue completamente
        yield return new WaitForSeconds(1f);

        // Cargar la siguiente escena de manera asíncrona sin activarla aún
        operation = SceneManager.LoadSceneAsync(6);
        operation.allowSceneActivation = false;

        yield return new WaitForSeconds(6f);

        // Esperar hasta que la escena esté casi completamente cargada
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f) // Cuando la escena está lista
            {
                operation.allowSceneActivation = true;
                SoundManager.Instance?.ResumeMusicAfterCombat();
            }
            yield return null;
        }

        // Activar animación de entrada
        transitionAnim.SetTrigger("Start");
    }

    public void LoadLevelForest()
    {
        if (childCanvas != null)
        {
            childCanvas.gameObject.SetActive(true);
        }
        StartCoroutine(LoadSceneForest());
    }
    private IEnumerator LoadSceneForest()
    {
        transitionAnim.SetTrigger("End");

        // Esperar un pequeño tiempo para la animación de salida
        yield return new WaitForSeconds(1f);

        transitionAnim.SetTrigger("Start");

        travel = FindFirstObjectByType<Travel>();

        travel.ForestTravel();

        yield return new WaitForSeconds(9f);

        transitionAnim.SetTrigger("End");

        // Cargar la escena 9 inmediatamente
        SceneManager.LoadScene(9);

        // Esperar a que la escena 9 se cargue completamente
        yield return new WaitForSeconds(1f);

        // Cargar la siguiente escena de manera asíncrona sin activarla aún
        operation = SceneManager.LoadSceneAsync(7);
        operation.allowSceneActivation = false;

        yield return new WaitForSeconds(6f);

        // Esperar hasta que la escena esté casi completamente cargada
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f) // Cuando la escena está lista
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        // Activar animación de entrada
        transitionAnim.SetTrigger("Start");
    }

    public void LoadLevelForestExit()
    {
        if (childCanvas != null)
        {
            childCanvas.gameObject.SetActive(true);
        }
        StartCoroutine(LoadSceneForestExit());
    }
    private IEnumerator LoadSceneForestExit()
    {
        transitionAnim.SetTrigger("End");

        // Esperar un pequeño tiempo para la animación de salida
        yield return new WaitForSeconds(1f);

        // Cargar la escena 9 inmediatamente
        SceneManager.LoadScene(9);

        // Esperar a que la escena 9 se cargue completamente
        yield return new WaitForSeconds(1f);

        // Cargar la siguiente escena de manera asíncrona sin activarla aún
        operation = SceneManager.LoadSceneAsync(6);
        operation.allowSceneActivation = false;

        yield return new WaitForSeconds(6f);

        // Esperar hasta que la escena esté casi completamente cargada
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f) // Cuando la escena está lista
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        // Activar animación de entrada
        transitionAnim.SetTrigger("Start");
    }

    public void LoadLevelFinal()
    {
        if (childCanvas != null)
        {
            childCanvas.gameObject.SetActive(true);
        }
        StartCoroutine(LoadSceneFinal());
    }
    private IEnumerator LoadSceneFinal()
    {
        transitionAnim.SetTrigger("End");

        // Esperar un pequeño tiempo para la animación de salida
        yield return new WaitForSeconds(1f);

        transitionAnim.SetTrigger("Start");

        travel = FindFirstObjectByType<Travel>();

        travel.FinalTravel();

        yield return new WaitForSeconds(9f);

        transitionAnim.SetTrigger("End");

        // Cargar la escena 9 inmediatamente
        SceneManager.LoadScene(9);

        // Esperar a que la escena 9 se cargue completamente
        yield return new WaitForSeconds(1f);

        // Cargar la siguiente escena de manera asíncrona sin activarla aún
        operation = SceneManager.LoadSceneAsync(10);
        operation.allowSceneActivation = false;

        yield return new WaitForSeconds(3f);

        // Esperar hasta que la escena esté casi completamente cargada
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f) // Cuando la escena está lista
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        // Activar animación de entrada
        transitionAnim.SetTrigger("Start");

        yield return new WaitForSeconds(6f);

        Application.Quit();
    }

    public void LoadLevelBattle()
    {
        if (childCanvas != null)
        {
            childCanvas.gameObject.SetActive(true);
        }
        StartCoroutine(LoadSceneBattle());
    }
    private IEnumerator LoadSceneBattle()
    {
        transitionAnim.SetTrigger("End");

        // Esperar un pequeño tiempo para la animación de salida
        yield return new WaitForSeconds(1f);

        // Cargar la escena 9 inmediatamente
        SceneManager.LoadScene(9);

        // Esperar a que la escena 9 se cargue completamente
        yield return new WaitForSeconds(1f);

        // Cargar la siguiente escena de manera asíncrona sin activarla aún
        operation = SceneManager.LoadSceneAsync(8);
        operation.allowSceneActivation = false;

        yield return new WaitForSeconds(6f);

        // Esperar hasta que la escena esté casi completamente cargada
        while (!operation.isDone)
        {
            if (!isSubscribedBattle && operation.progress >= 0.9f) // Cuando la escena está lista
            {
                operation.allowSceneActivation = true;

                if (!isSubscribed && operation.allowSceneActivation == true)
                {
                    isSubscribedBattle = true;
                }
            }
            yield return null;
        }

        // Activar animación de entrada
        transitionAnim.SetTrigger("Start");

        isSubscribedBattle = false;
    }
    public void LoadLevelBoss()
    {
        if (childCanvas != null)
        {
            childCanvas.gameObject.SetActive(true);
        }
        StartCoroutine(LoadSceneBoss());
    }
    private IEnumerator LoadSceneBoss()
    {
        transitionAnim.SetTrigger("End");

        // Esperar un pequeño tiempo para la animación de salida
        yield return new WaitForSeconds(1f);

        // Cargar la escena 9 inmediatamente
        SceneManager.LoadScene(9);

        // Esperar a que la escena 9 se cargue completamente
        yield return new WaitForSeconds(1f);

        // Cargar la siguiente escena de manera asíncrona sin activarla aún
        operation = SceneManager.LoadSceneAsync(8);
        operation.allowSceneActivation = false;

        yield return new WaitForSeconds(6f);

        // Esperar hasta que la escena esté casi completamente cargada
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f) // Cuando la escena está lista
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        // Activar animación de entrada
        transitionAnim.SetTrigger("Start");
    }
    public void LoadLevelExitBattle()
    {
        if (childCanvas != null)
        {
            childCanvas.gameObject.SetActive(true);
        }
        StartCoroutine(LoadSceneExitBattle());
    }

    private IEnumerator LoadSceneExitBattle()
    {
        transitionAnim.SetTrigger("End");

        // Espera un poco para que se reproduzca la animación de salida
        yield return new WaitForSeconds(1f);

        // Carga la escena intermedia (por ejemplo, la escena 9)
        SceneManager.LoadScene(9);

        yield return new WaitForSeconds(1f);

        // Cargar la escena de salida de batalla usando el índice almacenado
        operation = SceneManager.LoadSceneAsync(returnSceneIndex);
        operation.allowSceneActivation = false;

        yield return new WaitForSeconds(6f);

        // Esperar hasta que la escena esté lista para activarse
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        // Activar la animación de entrada
        transitionAnim.SetTrigger("Start");
    }
}
