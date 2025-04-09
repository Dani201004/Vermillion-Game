using System;
using System.Collections;
using UnityEngine;

public class Chest : MonoBehaviour
{
    // El item que se añadirá al inventario al abrir el cofre.
    public Item itemToAdd;

    // Referencia al MessageBoxController
    public MessageBoxController messageBox;

    // Flags para controlar si el cofre ya se abrió y si el jugador está en rango.
    public bool isOpened = false;
    public bool playerInRange = false;

    public delegate void OnChestEvent(bool chestOpened);
    public static event Action<bool> OnChestOpened;

    [SerializeField] private Animator chestAnimator;
    [SerializeField] private ParticleSystem chestParticle; // Sistema de partículas único

    // AudioSources para los sonidos en lugar de AudioClips.
    [SerializeField] private AudioSource openAudioSource;   // AudioSource para el sonido de apertura
    [SerializeField] private AudioSource rewardAudioSource; // AudioSource para el sonido de recompensa

    private void Start()
    {
        if (chestAnimator == null)
        {
            chestAnimator = GetComponent<Animator>();
        }

        if (chestParticle == null)
        {
            chestParticle = GetComponentInChildren<ParticleSystem>();
        }
    }

    void Update()
    {
        // Si el jugador está en rango, el cofre no se ha abierto y se presiona la tecla E...
        if (playerInRange && !isOpened && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    /// <summary>
    /// Abre el cofre, añade el item al inventario, reproduce la animación, el sistema de partículas y los sonidos.
    /// </summary>
    private void OpenChest()
    {
        // Intenta agregar el item al inventario.
        if (InventoryManager.AddItem(itemToAdd))
        {
            Debug.Log("Item agregado al inventario: " + itemToAdd.Name);
            OnChestOpened?.Invoke(true);
            // Mostrar mensaje en el MessageBoxController
            if (messageBox != null)
            {
                messageBox.ShowMessage("Obtuviste: " + itemToAdd.Name);
            }
        }
        else
        {
            Debug.Log("No se pudo agregar el item. ¿Inventario lleno?");
        }

        isOpened = true;

        // Ejecutar la animación del cofre
        if (chestAnimator != null)
        {
            chestAnimator.SetTrigger("Abrirse");
        }

        // Reproducir el sistema de partículas
        if (chestParticle != null)
        {
            chestParticle.Play();
        }

        // Reproducir el sonido de apertura y, al terminar, el de recompensa.
        if (openAudioSource != null)
        {
            openAudioSource.Play();
            // Inicia una coroutine que espera la duración del sonido de apertura para reproducir el de recompensa.
            StartCoroutine(PlayRewardSoundAfterDelay(openAudioSource.clip.length));
        }
        else if (rewardAudioSource != null)
        {
            // Si no hay AudioSource de apertura, reproducir directamente el de recompensa.
            rewardAudioSource.Play();
        }
    }

    private IEnumerator PlayRewardSoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (rewardAudioSource != null)
        {
            rewardAudioSource.Play();
        }
    }

    // Detecta cuando el jugador entra en el área de colisión del cofre.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Presiona E para abrir el cofre");
        }
    }

    // Detecta cuando el jugador sale del área de colisión.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
