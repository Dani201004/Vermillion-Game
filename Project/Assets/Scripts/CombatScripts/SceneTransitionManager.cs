using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private float transitionDuration = 0.5f;
    private Vector3 playerPosition;
    SaveGameManager saveGameManager;

    // Bandera que indica si se gan� el combate.
    public static bool winnedReturn = false;

    private SceneTransition sceneTransition;
    private static SceneTransitionManager instance;
    // Se almacenar� el nombre del enemigo que inici� el combate
    private string currentEnemyName;

    private SceneDependentToggle sceneDependentToggle;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }

    private void Start()
    {
        sceneTransition = FindFirstObjectByType<SceneTransition>();
        if (sceneTransition == null)
        {
            Debug.LogError("SceneTransition no encontrado en la escena.");
        }

        // Buscar el objeto que tiene SceneDependentToggle
        SceneDependentToggle sceneDependentToggle = FindObjectOfType<SceneDependentToggle>(true);
        if (sceneDependentToggle == null)
        {
            Debug.LogWarning("No se encontr� un objeto con SceneDependentToggle en la escena.");
        }
    }

    /// <summary>
    /// Inicia la transici�n al combate.
    /// Se recibe la posici�n del jugador y el nombre del enemigo que colision�.
    /// </summary>
    public void StartBattle(Vector3 position, string enemyName)
    {
        playerPosition = position;
        currentEnemyName = enemyName;
        Debug.Log("StartBattle llamado. Enemigo: " + enemyName);
        FindFirstObjectByType<CameraTransition>().TransitionToScene();
        SoundManager.Instance?.StopMusicForCombat();
    }

    private void EndBattle()
    {
        Invoke(nameof(LoadExplorationScene), transitionDuration);
    }

    public void LoadExplorationScene()
    {
        // Suscribirse al evento para detectar la carga de la escena de exploraci�n
        SceneManager.sceneLoaded += OnSceneLoaded;
        sceneTransition.LoadLevelExitBattle();
        SoundManager.Instance?.ResumeMusicAfterCombat();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Se comprueba que la escena cargada es la correcta (la de exploraci�n)
        if (scene.buildIndex == SceneTransition.Instance.returnSceneIndex)
        {
            StartCoroutine(RestorePlayerPositionAndHandleEnemy());
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private IEnumerator RestorePlayerPositionAndHandleEnemy()
    {
        yield return new WaitForEndOfFrame();
        if (sceneDependentToggle == null)
        {
            sceneDependentToggle = FindObjectOfType<SceneDependentToggle>(true);
            if (sceneDependentToggle == null)
            {
                Debug.LogError("No se encontr� SceneDependentToggle en la escena.");
            }
        }

        // Si se encontr�, se llama a ToggleState
        sceneDependentToggle?.ToggleState();

        GameObject player = null;
        // Espera a que el jugador est� presente en la escena
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }

        // Restaura la posici�n del jugador y guarda el juego
        player.transform.position = playerPosition;
        saveGameManager?.SaveGame();

        Debug.Log("Restaurando posici�n del jugador. winnedReturn: " + winnedReturn);
        // Si se gan� el combate y se registr� el nombre del enemigo, se busca y se procesa su destrucci�n
        if (winnedReturn && !string.IsNullOrEmpty(currentEnemyName))
        {
            GameObject enemyObj = GameObject.Find(currentEnemyName);
            if (enemyObj != null)
            {
                EnemyCollisionHandler handler = enemyObj.GetComponent<EnemyCollisionHandler>();
                if (handler != null)
                {
                    Debug.Log("Llamando a HandleBattleOutcome en enemigo: " + enemyObj.name);
                    handler.HandleBattleOutcome();
                }
                else
                {
                    Debug.LogWarning("El GameObject encontrado no tiene el componente EnemyCollisionHandler.");
                }
            }
            else
            {
                Debug.LogWarning("No se encontr� el enemigo con nombre: " + currentEnemyName);
            }
            currentEnemyName = null;
        }
    }
}

