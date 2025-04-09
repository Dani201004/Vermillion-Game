using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDependentToggle : MonoBehaviour
{
    [Header("Array de objetos a encender/apagar")]
    public GameObject[] objectsToToggle; // Estos objetos se encenderán o apagarán según el estado

    private const string SaveKey = "ToggleState"; // Clave para guardar el estado
    public bool toggleState; // Variable que determina el estado (true/false)
    public bool pulpilloEnabled;

    void Start()
    {
        // Cargar el estado guardado, por defecto 1 (true)
        toggleState = PlayerPrefs.GetInt(SaveKey, 0) == 1;
        pulpilloEnabled = PlayerPrefs.GetInt("pulpilloTalked", 0) == 1;

        // 1. Encender o apagar los objetos del array
        SetObjectsActive(objectsToToggle, toggleState);

        // 2. Acciones según la escena
        SetActive();
    }

    private void Update()
    {
        SetActive();
    }

    private void SetActive()
    {
        SetObjectsActive(objectsToToggle, toggleState);
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Game" || sceneName == "Forest")
        {
            PartyController partyController = FindObjectOfType<PartyController>();
            if (partyController != null)
            {
                partyController.enabled = toggleState;
            }

            if (pulpilloEnabled)
            {
                // Buscar a Pulpillo
                PulpilloMovement pulpillo = FindAnyObjectByType<PulpilloMovement>() ?? FindObjectOfType<PulpilloMovement>();
                if (pulpillo != null)
                {
                    pulpillo.enabled = toggleState;

                    // Buscar el sistema de diálogo en el mismo objeto o en sus hijos
                    DialogueSystem dialogueSystem = pulpillo.GetComponent<DialogueSystem>() ?? pulpillo.GetComponentInChildren<DialogueSystem>();
                    if (dialogueSystem != null)
                    {
                        dialogueSystem.enabled = !(toggleState && pulpilloEnabled); // Desactivar si ambas variables son true
                        //Debug.Log($"DialogueSystem en {pulpillo.name} se ha {(dialogueSystem.enabled ? "activado" : "desactivado")}");
                    }
                    else
                    {
                        Debug.LogWarning("No se encontró el componente DialogueSystem en Pulpillo.");
                    }
                }
                else
                {
                    Debug.LogWarning("No se encontró PulpilloMovement en la escena.");
                }
            }

            // Buscar otro objeto por nombre y desactivar su BoxCollider y su DialogueSystem si ambas condiciones son true
            if (toggleState)
            {
                GameObject githyanki = GameObject.Find("Githyanki"); // Asegúrate de que el nombre es correcto en la jerarquía de Unity
                if (githyanki != null)
                {
                    // 🔹 Desactivar su BoxCollider
                    BoxCollider targetCollider = githyanki.GetComponent<BoxCollider>();
                    if (targetCollider != null)
                    {
                        targetCollider.enabled = false;
                        //Debug.Log($"BoxCollider en {githyanki.name} se ha desactivado.");
                    }
                    else
                    {
                        Debug.LogWarning($"El objeto {githyanki.name} no tiene un BoxCollider adjunto.");
                    }

                    // Desactivar su DialogueSystem
                    DialogueSystem githyankiDialogue = githyanki.GetComponent<DialogueSystem>() ?? githyanki.GetComponentInChildren<DialogueSystem>();
                    if (githyankiDialogue != null)
                    {
                        githyankiDialogue.enabled = false;
                        //Debug.Log($"DialogueSystem en {githyanki.name} se ha desactivado.");
                    }
                    else
                    {
                        Debug.LogWarning($"El objeto {githyanki.name} no tiene un DialogueSystem adjunto.");
                    }
                }
                else
                {
                    Debug.LogWarning("No se encontró el objeto 'Githyanki' en la escena.");
                }

                // NUEVO: Buscar y desactivar el objeto "Quest 5" si ambas condiciones son true
                GameObject quest5 = GameObject.Find("Quest 5");
                if (quest5 != null)
                {
                    quest5.SetActive(false);
                    Debug.Log("El objeto 'Quest 5' ha sido desactivado.");
                }
                else
                {
                    Debug.LogWarning("No se encontró el objeto 'Quest 5' en la escena.");
                }
            }
        }
        else if (sceneName == "battleScene")
        {
            PlayerStats[] players = FindObjectsOfType<PlayerStats>();
            foreach (PlayerStats ps in players)
            {
                if (ps.characterName != "Thief")
                {
                    ps.gameObject.SetActive(toggleState);
                }
            }
        }
    }

    public void ToggleState()
    {
        toggleState = true; // Alternar el estado

        PlayerPrefs.SetInt(SaveKey, toggleState ? 1 : 0);
        PlayerPrefs.Save();

        SetActive();
    }

    private void SetObjectsActive(GameObject[] objects, bool state)
    {
        if (objects == null) return;
        foreach (GameObject obj in objects)
        {
            if (obj != null)
                obj.SetActive(state);
        }
    }
}
