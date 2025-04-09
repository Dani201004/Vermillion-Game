using System.Collections;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    private SceneTransition sceneTransition;
    private bool isDetected;
    private bool isOnCooldown = false; // Flag para el cooldown

    private void Start()
    {
        // Buscar SceneTransition en la escena, incluso si está en DontDestroyOnLoad
        sceneTransition = FindFirstObjectByType<SceneTransition>();

        if (sceneTransition == null)
        {
            Debug.LogError("No se encontró SceneTransition en la escena.");
        }
    }

    private void Update()
    {
        if (isDetected)
        {
            if (Input.GetKeyDown(KeyCode.E) && !isOnCooldown)
            {
                sceneTransition.LoadLevelTavern();
                isOnCooldown = true;
                StartCoroutine(ResetCooldown());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isDetected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isDetected = false;
        }
    }

    private IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(10);
        isOnCooldown = false;
    }
}