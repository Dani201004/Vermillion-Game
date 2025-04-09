using UnityEngine;
using UnityEngine.SceneManagement;

public class BossBehaviour : MonoBehaviour
{
    // Este script se utiliza únicamente para detectar la interacción con un boss.
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
        // Captura el índice de la escena actual y lo guarda para volver después del combate
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneTransition.Instance.returnSceneIndex = currentSceneIndex;

        //Debug.Log("¡Has interactuado con el Boss Goblin!");
        BossLoader.Instance.LoadBossSceneAndSpawn("Goblin");

        // Envía el nombre del GameObject enemigo al SceneTransitionManager
        sceneTransitionManager.StartBattle(transform.position, gameObject.name);
    }

    public void InteractWithBossSlime()
    {
        // Captura el índice de la escena actual y lo guarda para volver después del combate
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneTransition.Instance.returnSceneIndex = currentSceneIndex;

        //Debug.Log("¡Has interactuado con el Boss Slime!");
        BossLoader.Instance.LoadBossSceneAndSpawn("Slime");

        // Envía el nombre del GameObject enemigo al SceneTransitionManager
        sceneTransitionManager.StartBattle(transform.position, gameObject.name);
    }

    public void InteractWithBossMushroom()
    {
        // Captura el índice de la escena actual y lo guarda para volver después del combate
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneTransition.Instance.returnSceneIndex = currentSceneIndex;

        //Debug.Log("¡Has interactuado con el Boss Mushroom!");
        BossLoader.Instance.LoadBossSceneAndSpawn("Mushroom");

        // Envía el nombre del GameObject enemigo al SceneTransitionManager
        sceneTransitionManager.StartBattle(transform.position, gameObject.name);
    }

}