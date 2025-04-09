using UnityEngine;

public class RayCast : MonoBehaviour
{
    [SerializeField] private float maxDistance = 5f; // Distancia m�xima del Raycast
    [SerializeField] private LayerMask npcLayer; // Capa de los NPCs
    [SerializeField] private LayerMask savePointLayer; // Capa de los puntos de guardado
    [SerializeField] private LayerMask goblinBossLayer; // Capa del Boss goblin
    [SerializeField] private LayerMask slimeBossLayer; // Capa del Boss slime
    [SerializeField] private LayerMask mushroomBossLayer; // Capa del Boss mushroom
    [SerializeField] private GameObject npcInteractionTextPrefab; // Prefab del texto que aparecer� sobre el NPC
    [SerializeField] private GameObject savePointTextPrefab; // Prefab del texto que aparecer� sobre el punto de guardado
    [SerializeField] private Transform player; // Transform del personaje (deber�a estar asignado en el Inspector)
    [SerializeField] private Camera playerCamera; // C�mara del jugador (asignar en el inspector)
    [SerializeField] private KeyCode interactKey = KeyCode.E; // Tecla para interactuar

    private GameObject currentText; // Referencia al texto activo
    private SaveGameManager saveGameManager; // Referencia al SaveGameManager

    private RaycastHit? currentHit; // �ltimo objeto impactado por el Raycast

    // Variables para el cooldown de interacci�n con los bosses
    private float bossInteractionCooldownDuration = 10f;
    private float lastBossInteractionTime = -10f; // Iniciado en -cooldown para permitir la primera interacci�n

    private void Start()
    {
        // Intentar encontrar el SaveGameManager en la escena
        saveGameManager = GameObject.FindFirstObjectByType<SaveGameManager>();
    }

    private void Update()
    {
        PerformRaycast(); // Actualiza el objeto impactado
        HandleInteraction(); // Maneja la interacci�n con los objetos

        // Si hay texto activo, haz que siempre mire hacia el jugador
        if (currentText != null)
        {
            RotateTextToFacePlayer();
        }
    }

    // Realiza un Raycast desde la posici�n y direcci�n del jugador.
    private void PerformRaycast()
    {
        Vector3 origin = player.position; // Origen del Raycast
        Vector3 direction = player.forward; // Direcci�n del Raycast

        // Realizamos el Raycast con las capas "NPC", "SavePoint" y bosses
        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, npcLayer | savePointLayer | goblinBossLayer | slimeBossLayer | mushroomBossLayer))
        {
            currentHit = hit; // Almacena el objeto impactado
            HandleTextDisplay(hit); // Maneja la visualizaci�n del texto
        }
        else
        {
            // Si no hay impacto, elimina el texto
            currentHit = null;
            DestroyCurrentText();
        }

        // Dibuja el Raycast en la escena para depuraci�n
        Debug.DrawRay(origin, direction * maxDistance, Color.red);
    }

    // Maneja la visualizaci�n del texto seg�n el objeto impactado.
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

            // Actualiza la posici�n del texto
            currentText.transform.position = hitObject.position + Vector3.up * 3.3f;
        }
        // Si impactamos un punto de guardado
        else if (((1 << hit.collider.gameObject.layer) & savePointLayer) != 0)
        {
            if (currentText == null)
            {
                ShowSavePointText(hitObject);
            }

            // Actualiza la posici�n del texto
            currentText.transform.position = hitObject.position + Vector3.up * 3.3f;
        }
        else
        {
            DestroyCurrentText();
        }
    }

    // Maneja la interacci�n con el objeto impactado.
    private void HandleInteraction()
    {
        if (currentHit.HasValue && Input.GetKeyDown(interactKey))
        {
            RaycastHit hit = currentHit.Value;

            // Interacci�n con NPC (sin cooldown)
            if (((1 << hit.collider.gameObject.layer) & npcLayer) != 0)
            {
                // Aqu� puedes a�adir la l�gica de interacci�n con NPC
                // Debug.Log("Interaccionando con NPC");
            }
            // Interacci�n con bosses (con cooldown de 10 segundos)
            else if ((((1 << hit.collider.gameObject.layer) & goblinBossLayer) != 0) ||
                     (((1 << hit.collider.gameObject.layer) & slimeBossLayer) != 0) ||
                     (((1 << hit.collider.gameObject.layer) & mushroomBossLayer) != 0))
            {
                // Verifica si se cumpli� el cooldown
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

                    // Actualiza el tiempo de la �ltima interacci�n con boss
                    lastBossInteractionTime = Time.time;
                }
            }
        }
    }

    private void ShowTextAboveNPC(Transform npc)
    {
        // Instancia el texto sobre el NPC
        currentText = Instantiate(npcInteractionTextPrefab, npc.position + Vector3.up * 2f, Quaternion.identity);
        currentText.transform.SetParent(npc); // Opcional: Aseg�rate de que el texto siga al NPC
    }

    private void ShowSavePointText(Transform savePoint)
    {
        // Instancia el texto sobre el objeto de guardado
        currentText = Instantiate(savePointTextPrefab, savePoint.position + Vector3.up * 2f, Quaternion.identity);
        currentText.transform.SetParent(savePoint); // Opcional: Aseg�rate de que el texto siga al objeto
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
        // Aseg�rate de que el texto apunte hacia la c�mara del jugador
        Vector3 directionToCamera = (playerCamera.transform.position - currentText.transform.position).normalized;

        // Mant�n el texto plano en el eje Y
        directionToCamera.y = 0;

        // Ajusta la rotaci�n del texto
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
