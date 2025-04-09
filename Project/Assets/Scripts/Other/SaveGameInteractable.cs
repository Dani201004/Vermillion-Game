using System.Collections;
using UnityEngine;

public class SaveGameInteractable : MonoBehaviour
{
    private RayCast rayCastScript; // Referencia al script RayCast
    private Camera playerCamera; // Cámara del jugador (asignar en el inspector)

    private Transform lastInteractedObject; // Último objeto interactuado para evitar duplicados
    private SaveGameManager saveGameManager; // Referencia al SaveGameManager

    [SerializeField] private float interactionCooldown = 1.0f; // Tiempo de espera entre interacciones
    private bool isInteractionOnCooldown = false;

    private void Update()
    {
        // Usa el RayCastGenerator del script RayCast
        if (rayCastScript != null)
        {
            HandleInteraction(rayCastScript.GetRaycastHit());
        }
        // Intentar encontrar el SaveGameManager en la escena
        saveGameManager = FindFirstObjectByType<SaveGameManager>();

        // Buscar automáticamente el script RayCast si no está asignado
        if (rayCastScript == null)
        {
            rayCastScript = FindFirstObjectByType<RayCast>();

            if (rayCastScript == null)
            {
                Debug.LogError("No se encontró el script RayCast en la escena.");
            }
        }

        // Buscar automáticamente la cámara del jugador si no está asignada
        if (playerCamera == null)
        {
            playerCamera = Camera.main;

            if (playerCamera == null)
            {
                Debug.LogError("No se encontró una cámara principal en la escena.");
            }
        }
    }

    private void HandleInteraction(RaycastHit? hit)
    {
        if (hit.HasValue)
        {
            Transform hitObject = hit.Value.transform;

            // Si el Raycast impacta un punto de guardado
            if (((1 << hit.Value.collider.gameObject.layer) & rayCastScript.GetSavePointLayer()) != 0)
            {
                HandleSavePointInteraction(hitObject);
            }
        }
    }

    private void HandleSavePointInteraction(Transform savePoint)
    {
        // Si ya se interactuó con este mismo objeto, se puede omitir la acción (opcional)
        if (lastInteractedObject == savePoint && isInteractionOnCooldown)
            return;

        lastInteractedObject = savePoint;

        if (Input.GetKeyDown(KeyCode.E) && !isInteractionOnCooldown)
        {
            if (saveGameManager != null)
            {
                saveGameManager.SaveGame();
                StartCoroutine(InteractionCooldown());
            }
            else
            {
                Debug.LogError("No se encontró SaveGameManager. Asegúrate de que esté en la escena.");
            }
        }
    }

    private IEnumerator InteractionCooldown()
    {
        isInteractionOnCooldown = true;
        yield return new WaitForSeconds(interactionCooldown);
        isInteractionOnCooldown = false;
    }
}
