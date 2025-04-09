using UnityEngine;
using UnityEngine.SceneManagement;

public class BossBehaviour : MonoBehaviour
{
    // Este script se utiliza �nicamente para detectar la interacci�n con un boss.
    // Al interactuar, llama al BossLoader (que es persistente) para cargar la escena del boss
    // y generar el jefe correspondiente.
    private SceneTransitionManager sceneTransitionManager;

    private void Start()
    {
        // Busca el SceneTransitionManager en la escena
        sceneTransitionManager = FindFirstObjectByType<SceneTransitionManager>();
        if (sceneTransitionManager == null)
        {
            Debug.LogError("SceneTransitionManager no encontrado en la escena.");
        }
    }

    public void InteractWithBossGoblin()
    {
        // Captura el �ndice de la escena actual y lo guarda para volver despu�s del combate
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneTransition.Instance.returnSceneIndex = currentSceneIndex;

        //Debug.Log("�Has interactuado con el Boss Goblin!");
        BossLoader.Instance.LoadBossSceneAndSpawn("Goblin");

        // Env�a el nombre del GameObject enemigo al SceneTransitionManager
        sceneTransitionManager.StartBattle(transform.position, gameObject.name);
    }

    public void InteractWithBossSlime()
    {
        // Captura el �ndice de la escena actual y lo guarda para volver despu�s del combate
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneTransition.Instance.returnSceneIndex = currentSceneIndex;

        //Debug.Log("�Has interactuado con el Boss Slime!");
        BossLoader.Instance.LoadBossSceneAndSpawn("Slime");

        // Env�a el nombre del GameObject enemigo al SceneTransitionManager
        sceneTransitionManager.StartBattle(transform.position, gameObject.name);
    }

    public void InteractWithBossMushroom()
    {
        // Captura el �ndice de la escena actual y lo guarda para volver despu�s del combate
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneTransition.Instance.returnSceneIndex = currentSceneIndex;

        //Debug.Log("�Has interactuado con el Boss Mushroom!");
        BossLoader.Instance.LoadBossSceneAndSpawn("Mushroom");

        // Env�a el nombre del GameObject enemigo al SceneTransitionManager
        sceneTransitionManager.StartBattle(transform.position, gameObject.name);
    }

}