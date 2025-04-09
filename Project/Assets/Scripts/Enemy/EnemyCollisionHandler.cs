using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyCollisionHandler : MonoBehaviour
{
    private SceneTransitionManager sceneTransitionManager;
    private bool isSubscribed = false;

    private void Start()
    {
        // Busca el SceneTransitionManager en la escena
        sceneTransitionManager = FindFirstObjectByType<SceneTransitionManager>();
        if (sceneTransitionManager == null)
        {
            Debug.LogError("SceneTransitionManager no encontrado en la escena.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isSubscribed)
        {
            // Captura el índice de la escena actual y lo guarda para volver después del combate
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneTransition.Instance.returnSceneIndex = currentSceneIndex;

            if (sceneTransitionManager != null)
            {
                // Envía el nombre del GameObject enemigo al SceneTransitionManager
                sceneTransitionManager.StartBattle(transform.position, gameObject.name);
                isSubscribed = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                BossLoader.Instance.LoadRandomBattle();
                Invoke(nameof(Unsubscribe), 1f);
            }
        }
    }

    private void Unsubscribe()
    {
        isSubscribed = false;
    }

    /// <summary>
    /// Método que será llamado por el SceneTransitionManager tras finalizar el combate.
    /// Si winnedReturn es true, destruye este enemigo.
    /// </summary>
    public void HandleBattleOutcome()
    {
        Debug.Log("HandleBattleOutcome llamado en enemigo: " + gameObject.name);
        if (SceneTransitionManager.winnedReturn)
        {
            Debug.Log("Combate ganado, destruyendo enemigo: " + gameObject.name);
            SceneTransitionManager.winnedReturn = false;
            Destroy(gameObject);
        }
    }
}

