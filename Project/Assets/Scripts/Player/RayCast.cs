using UnityEngine;

public class RayCast : MonoBehaviour
{
    [SerializeField] private float maxDistance = 5f; // Distancia máxima del Raycast
    [SerializeField] private LayerMask npcLayer; // Capa de los NPCs
    [SerializeField] private LayerMask savePointLayer; // Capa de los puntos de guardado
    [SerializeField] private LayerMask goblinBossLayer; // Capa del Boss goblin
    [SerializeField] private LayerMask slimeBossLayer; // Capa del Boss slime
    [SerializeField] private LayerMask mushroomBossLayer; // Capa del Boss mushroom
    [SerializeField] private GameObject npcInteractionTextPrefab; // Prefab del texto que aparecerá sobre el NPC
    [SerializeField] private GameObject savePointTextPrefab; // Prefab del texto que aparecerá sobre el punto de guardado
    [SerializeField] private Transform player; // Transform del personaje (debería estar asignado en el Inspector)
    [SerializeField] private Camera playerCamera; // Cámara del jugador (asignar en el inspector)
    [SerializeField] private KeyCode interactKey = KeyCode.E; // Tecla para interactuar

    private GameObject currentText; // Referencia al texto activo
    private SaveGameManager saveGameManager; // Referencia al SaveGameManager

    private RaycastHit? currentHit; // Último objeto impactado por el Raycast

    // Variables para el cooldown de interacción con los bosses
    private float bossInteractionCooldownDuration = 10f;
    private float lastBossInteractionTime = -10f; // Iniciado en -cooldown para permitir la primera interacción

    private void Start()
    {
        // Intentar encontrar el SaveGameManager en la escena
        saveGameManager = GameObject.FindFirstObjectByType<SaveGameManager>();
    }

    private void Update()
    {
        PerformRaycast(); // Actualiza el objeto impactado
        HandleInteraction(); // Maneja la interacción con los objetos

        // Si hay texto activo, haz que siempre mire hacia el jugador
        if (currentText != null)
        {
            RotateTextToFacePlayer();
        }
    }

    // Realiza un Raycast desde la posición y dirección del jugador.
    private void PerformRaycast()
    {
        Vector3 origin = player.position; // Origen del Raycast
        Vector3 direction = player.forward; // Dirección del Raycast

        // Realizamos el Raycast con las capas "NPC", "SavePoint" y bosses
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, npcLayer | savePointLayer | goblinBossLayer | slimeBossLayer | mushroomBossLayer))
        {
            currentHit = hit; // Almacena el objeto impactado
            HandleTextDisplay(hit); // Maneja la visualización del texto
        }
        else
        {
            // Si no hay impacto, elimina el texto
            currentHit = null;
            DestroyCurrentText();
        }

        // Dibuja el Raycast en la escena para depuración
        Debug.DrawRay(origin, direction * maxDistance, Color.red);
    }

    // Maneja la visualización del texto según el objeto impactado.
    private void HandleTextDisplay(RaycastHit hit)
    {
        Transform hitObject = hit.transform;

        // Si impactamos un NPC
        if (((1 << hit.collider.gameObject.layer) & npcLayer) != 0)
        {
            if (currentText == null)
            {
                ShowTextAboveNPC(hitObject);
            }

            // Actualiza la posición del texto
            currentText.transform.position = hitObject.position + Vector3.up * 3.3f;
        }
        // Si impactamos un punto de guardado
        else if (((1 << hit.collider.gameObject.layer) & savePointLayer) != 0)
        {
            if (currentText == null)
            {
                ShowSavePointText(hitObject);
            }

            // Actualiza la posición del texto
            currentText.transform.position = hitObject.position + Vector3.up * 3.3f;
        }
        else
        {
            DestroyCurrentText();
        }
    }

    // Maneja la interacción con el objeto impactado.
    private void HandleInteraction()
    {
        if (currentHit.HasValue && Input.GetKeyDown(interactKey))
        {
            RaycastHit hit = currentHit.Value;

            // Interacción con NPC (sin cooldown)
            if (((1 << hit.collider.gameObject.layer) & npcLayer) != 0)
            {
                // Aquí puedes añadir la lógica de interacción con NPC
                // Debug.Log("Interaccionando con NPC");
            }
            // Interacción con bosses (con cooldown de 10 segundos)
            else if ((((1 << hit.collider.gameObject.layer) & goblinBossLayer) != 0) ||
                     (((1 << hit.collider.gameObject.layer) & slimeBossLayer) != 0) ||
                     (((1 << hit.collider.gameObject.layer) & mushroomBossLayer) != 0))
            {
                // Verifica si se cumplió el cooldown
                if (Time.time - lastBossInteractionTime < bossInteractionCooldownDuration)
                {
                    Debug.Log("Cooldown activo. Espera 10 segundos para interactuar nuevamente con el boss.");
                    return;
                }

                BossBehaviour boss = hit.collider.GetComponent<BossBehaviour>();
                if (boss != null)
                {
                    if (((1 << hit.collider.gameObject.layer) & goblinBossLayer) != 0)
                    {
                        boss.InteractWithBossGoblin();
                    }
                    else if (((1 << hit.collider.gameObject.layer) & slimeBossLayer) != 0)
                    {
                        boss.InteractWithBossSlime();
                    }
                    else if (((1 << hit.collider.gameObject.layer) & mushroomBossLayer) != 0)
                    {
                        boss.InteractWithBossMushroom();
                    }

                    // Actualiza el tiempo de la última interacción con boss
                    lastBossInteractionTime = Time.time;
                }
            }
        }
    }

    private void ShowTextAboveNPC(Transform npc)
    {
        // Instancia el texto sobre el NPC
        currentText = Instantiate(npcInteractionTextPrefab, npc.position + Vector3.up * 2f, Quaternion.identity);
        currentText.transform.SetParent(npc); // Opcional: Asegúrate de que el texto siga al NPC
    }

    private void ShowSavePointText(Transform savePoint)
    {
        // Instancia el texto sobre el objeto de guardado
        currentText = Instantiate(savePointTextPrefab, savePoint.position + Vector3.up * 2f, Quaternion.identity);
        currentText.transform.SetParent(savePoint); // Opcional: Asegúrate de que el texto siga al objeto
    }

    private void DestroyCurrentText()
    {
        if (currentText != null)
        {
            Destroy(currentText);
            currentText = null;
        }
    }

    private void RotateTextToFacePlayer()
    {
        // Asegúrate de que el texto apunte hacia la cámara del jugador
        Vector3 directionToCamera = (playerCamera.transform.position - currentText.transform.position).normalized;

        // Mantén el texto plano en el eje Y
        directionToCamera.y = 0;

        // Ajusta la rotación del texto
        currentText.transform.rotation = Quaternion.LookRotation(-directionToCamera);
    }

    // Devuelve el objeto impactado por el Raycast.
    public RaycastHit? GetRaycastHit()
    {
        return currentHit;
    }

    // Devuelve la capa asignada a los puntos de guardado.
    public LayerMask GetSavePointLayer()
    {
        return savePointLayer;
    }
}
