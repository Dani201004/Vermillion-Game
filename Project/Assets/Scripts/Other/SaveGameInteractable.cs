using System.Collections;
using UnityEngine;

public class SaveGameInteractable : MonoBehaviour
{
    private RayCast rayCastScript; // Referencia al script RayCast
    private Camera playerCamera; // C�mara del jugador (asignar en el inspector)

    private Transform lastInteractedObject; // �ltimo objeto interactuado para evitar duplicados
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

        // Buscar autom�ticamente el script RayCast si no est� asignado
        if (rayCastScript == null)
        {
            rayCastScript = FindFirstObjectByType<RayCast>();

            if (rayCastScript == null)
            {
                Debug.LogError("No se encontr� el script RayCast en la escena.");
            }
        }

        // Buscar autom�ticamente la c�mara del jugador si no est� asignada
        if (playerCamera == null)
        {
            playerCamera = Camera.main;

            if (playerCamera == null)
            {
                Debug.LogError("No se encontr� una c�mara principal en la escena.");
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
        // Si ya se interactu� con este mismo objeto, se puede omitir la acci�n (opcional)
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
                Debug.LogError("No se encontr� SaveGameManager. Aseg�rate de que est� en la escena.");
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
