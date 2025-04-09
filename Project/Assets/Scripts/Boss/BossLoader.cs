using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossLoader : MonoBehaviour
{

    public static BossLoader Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Este objeto se mantendr� entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Este m�todo se llama cuando otro Collider entra en el trigger de este objeto
    private void OnTriggerEnter(Collider other)
    {
        // Solo se interact�a si el objeto que colisiona tiene la etiqueta "Player"
        if (other.CompareTag("Player"))
        {
            // Aqu� puedes decidir qu� acci�n realizar, por ejemplo:
            LoadBossSceneAndSpawn("Goblin");
            // O tambi�n podr�as llamar a LoadRandomBattle() seg�n la l�gica de tu juego
        }
    }

    public void LoadBossSceneAndSpawn(string bossType)
    {
        StartCoroutine(LoadSceneAndSpawnBossCoroutine(bossType));
    }

    private IEnumerator LoadSceneAndSpawnBossCoroutine(string bossType)
    {

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // Llama a la transici�n (suponiendo que tienes un SceneTransition)
        SceneTransition sceneTransition = FindFirstObjectByType<SceneTransition>();
        if (sceneTransition != null)
        {
            sceneTransition.LoadLevelBoss();
        }
        else
        {
            Debug.LogWarning("No se encontr� SceneTransition");
        }

        // Esperar hasta que la escena 8 est� cargada
        var scene8 = SceneManager.GetSceneByBuildIndex(8);
        while (!scene8.isLoaded)
        {
            yield return null;
            scene8 = SceneManager.GetSceneByBuildIndex(8);
        }

        // Opcional: Forzar que la escena 8 sea la activa
        SceneManager.SetActiveScene(scene8);

        // Esperar un poco m�s para asegurarnos de que el EnemyManager se inicializ�
        yield return new WaitForSeconds(0.5f);

        // Buscar el EnemyManager en la nueva escena
        EnemyManager newEnemyManager = FindFirstObjectByType<EnemyManager>();
        if (newEnemyManager != null)
        {
            newEnemyManager.spawnRandomEnemies = false;
            newEnemyManager.isBossFight = true;

            switch (bossType)
            {
                case "Goblin":
                    newEnemyManager.SpawnGoblinBoss();
                    break;
                case "Slime":
                    newEnemyManager.SpawnSlimeBoss();
                    break;
                case "Mushroom":
                    newEnemyManager.SpawnMushroomBoss();
                    break;
                default:
                    Debug.LogWarning("Tipo de boss desconocido: " + bossType);
                    break;
            }
        }
        else
        {
            Debug.LogError("EnemyManager no encontrado en la escena del boss.");
        }
    }

    public void LoadRandomBattle()
    {
        StartCoroutine(LoadRandomBattleCoroutine());
    }
    private IEnumerator LoadRandomBattleCoroutine()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneTransition sceneTransition = FindFirstObjectByType<SceneTransition>();
        if (sceneTransition != null)
        {
            sceneTransition.LoadLevelBattle();
        }
        else
        {
            Debug.LogWarning("No se encontr� SceneTransition");
        }

        var battleScene = SceneManager.GetSceneByBuildIndex(8);
        while (!battleScene.isLoaded)
        {
            yield return null;
            battleScene = SceneManager.GetSceneByBuildIndex(8);
        }

        SceneManager.SetActiveScene(battleScene);
        yield return new WaitForSeconds(0.5f);

        EnemyManager newEnemyManager = FindFirstObjectByType<EnemyManager>();
        if (newEnemyManager != null)
        {
            newEnemyManager.spawnRandomEnemies = true;
            newEnemyManager.isBossFight = false;
            newEnemyManager.ReinitializeEnemies();
        }
        else
        {
            Debug.LogError("EnemyManager no encontrado en la escena de batalla.");
        }
    }
}
