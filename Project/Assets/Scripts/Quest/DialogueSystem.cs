using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private Canvas dialogueCanvas;  // Referencia al Canvas de diálogo
    [SerializeField] private Text dialogueText;
    [SerializeField] private GameObject optionsContainer;  // Contenedor para las opciones de respuesta
    [SerializeField] private Button optionPrefab;  // Prefab para los botones de respuesta
    private GameObject player;  // Referencia al jugador
    [SerializeField] private float interactionDistance = 3.0f;  // Distancia de interacción
    [SerializeField] private float buttonSpacing = 60.0f;  // Distancia entre botones
    [SerializeField] private string characterMovementScriptName = "CharacterMovement";
    [SerializeField] DialogueSystem dialogue2;
    [SerializeField] DialogueSystem dialogueIdle;

    private Animator conversationAnimator;  // Referencia al Animator del NPC
    [SerializeField] private string conversationAnimationTrigger = "StartConversation";  // Nombre del trigger de la animación

    [SerializeField] public int dialogueID;

    private SceneTransition sceneTransition;

    public delegate void OnDialogueEvent(int idDialogue);
    public static event OnDialogueEvent OnDialogueEnd;

    private GameObject freelookCamera;

    private AudioSource typingSound;  // AudioSource para reproducir los sonidos
    [SerializeField] private AudioClip[] typingClips;   // Array de sonidos de tipeo
    [SerializeField] private float typingSpeed = 0.05f;   // Velocidad a la que se escribe cada letra
    // Se ha eliminado la velocidad acelerada al mantener el clic

    private SceneDependentToggle sceneDependentToggle;

    [System.Serializable]
    public class DialogueNode
    {
        public string dialogueText;
        public string[] responses;  // Opciones de respuesta
        public int[] nextDialogueIndices;  // Índices para los siguientes diálogos según la respuesta
        public bool requiresCondition;  // Si este nodo requiere completar algo
        public string conditionKey;  // Clave para verificar la condición
    }

    [SerializeField] private DialogueNode[] dialogueNodes;  // Nodos del diálogo
    [SerializeField] private string questKey;  // Clave para verificar si la misión está completa
    [SerializeField] public bool isMissionComplete = false;  // Estado de la misión

    private int currentDialogueIndex = 0;
    private bool isDialogueActive = false;
    private int selectedOptionIndex = 0;  // Opción actualmente seleccionada
    private bool hasTalkedBefore = false;

    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private string currentFullText = "";
    private DialogueNode currentDialogueNode;

    private void Start()
    {
        isMissionComplete = false;
        dialogueCanvas.gameObject.SetActive(false);
        optionsContainer.SetActive(false);  // Desactiva el contenedor de opciones al inicio

        if (freelookCamera == null)
        {
            freelookCamera = GameObject.FindWithTag("FreeLookCamera");
        }

        if (conversationAnimator == null)
        {
            conversationAnimator = GetComponent<Animator>();
        }

        // Buscar el objeto Player y, en sus hijos, el AudioSource para el sonido de tipeo
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj;
            typingSound = playerObj.GetComponentInChildren<AudioSource>();
        }

        SceneDependentToggle sceneDependentToggle = FindObjectOfType<SceneDependentToggle>(true);
        if (sceneDependentToggle == null)
        {
            Debug.LogWarning("No se encontró un objeto con SceneDependentToggle en la escena.");
        }
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            return;
        }

        if (freelookCamera == null)
        {
            freelookCamera = GameObject.FindWithTag("FreeLookCamera");
        }

        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance <= interactionDistance && Input.GetKeyDown(KeyCode.E) && !isDialogueActive)
        {
            StartDialogue();
        }

        if (isDialogueActive)
        {
            HandleOptionNavigation();
        }
    }

    public void StartDialogue()
    {
        Debug.Log("Iniciando diálogo");

        if (isMissionComplete)
        {
            if (dialogue2 != null)
            {
                dialogue2.enabled = true;
                this.enabled = false; // Desactivar este script
                return;
            }
        }

        isDialogueActive = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (freelookCamera != null)
        {
            freelookCamera.SetActive(false);
        }


        // Si ya has hablado antes y no hay una misión pendiente, muestra el último nodo
        if (hasTalkedBefore)
        {
            currentDialogueIndex = dialogueNodes.Length - 1;  // Último nodo
        }
        else
        {
            currentDialogueIndex = 0;
        }

        // Desactivar el movimiento del jugador
        MonoBehaviour movementScript = player.GetComponent(characterMovementScriptName) as MonoBehaviour;
        if (movementScript != null) movementScript.enabled = false;

        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("walking", false);
            playerAnimator.SetTrigger(conversationAnimationTrigger);
        }
        else
        {
            Debug.LogWarning("El jugador no tiene un Animator.");
        }

        dialogueCanvas.gameObject.SetActive(true);  // Activa el Canvas
        ShowDialogue();

        if (conversationAnimator != null)
        {
            conversationAnimator.SetTrigger(conversationAnimationTrigger);
        }
    }

    private void ShowDialogue()
    {
        DialogueNode currentNode = dialogueNodes[currentDialogueIndex];

        if (currentNode.requiresCondition && !CheckCondition(currentNode.conditionKey))
        {
            Debug.Log("No se cumplen las condiciones para esta conversación.");
            EndDialogue();
            return;
        }

        hasTalkedBefore = true;
        currentDialogueNode = currentNode;
        currentFullText = currentNode.dialogueText;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeDialogue(currentFullText, currentDialogueNode));
        isTyping = true;
    }

    private IEnumerator TypeDialogue(string fullText, DialogueNode currentNode)
    {
        dialogueText.text = "";
        for (int i = 0; i < fullText.Length; i++)
        {
            dialogueText.text += fullText[i];

            if (typingSound != null && fullText[i] != ' ' && typingClips != null && typingClips.Length > 0)
            {
                int randomIndex = Random.Range(0, typingClips.Length);
                AudioClip clip = typingClips[randomIndex];
                typingSound.PlayOneShot(clip);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        GenerateResponseOptions(currentNode);
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        dialogueText.text = currentFullText;
        isTyping = false;
        GenerateResponseOptions(currentDialogueNode);
    }

    private void GenerateResponseOptions(DialogueNode currentNode)
    {
        // Limpiar las opciones anteriores
        foreach (Transform child in optionsContainer.transform)
        {
            Destroy(child.gameObject);
        }

        optionsContainer.SetActive(true);

        for (int i = 0; i < currentNode.responses.Length; i++)
        {
            string responseText = currentNode.responses[i];
            int nextIndex = currentNode.nextDialogueIndices[i];

            Button optionButton = Instantiate(optionPrefab, optionsContainer.transform);
            optionButton.GetComponentInChildren<TMP_Text>().text = responseText;

            RectTransform rectTransform = optionButton.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, -i * buttonSpacing);

            int index = i;  // Capturar el índice correctamente
            optionButton.onClick.AddListener(() => OnResponseSelected(nextIndex));
        }

        selectedOptionIndex = 0;
        HighlightSelectedOption();
    }

    private void HandleOptionNavigation()
    {
        // Usar SPACE para saltar el tipeo
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                SkipTyping();
                return;
            }
        }

        // Usar ENTER para seleccionar la opción
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isTyping && optionsContainer.activeSelf && optionsContainer.transform.childCount > 0)
            {
                Transform selectedButton = optionsContainer.transform.GetChild(selectedOptionIndex);
                selectedButton.GetComponent<Button>().onClick.Invoke();
            }
        }

        // Navegar opciones con W y S
        if (Input.GetKeyDown(KeyCode.W))
        {
            selectedOptionIndex = Mathf.Max(0, selectedOptionIndex - 1);
            HighlightSelectedOption();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            selectedOptionIndex = Mathf.Min(optionsContainer.transform.childCount - 1, selectedOptionIndex + 1);
            HighlightSelectedOption();
        }
    }

    private void HighlightSelectedOption()
    {
        for (int i = 0; i < optionsContainer.transform.childCount; i++)
        {
            Transform option = optionsContainer.transform.GetChild(i);
            TMP_Text text = option.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                text.color = (i == selectedOptionIndex) ? Color.yellow : Color.white;
            }
        }
    }

    private void OnResponseSelected(int nextIndex)
    {
        if (nextIndex == -1)
        {
            EndDialogue();
        }
        else
        {
            currentDialogueIndex = nextIndex;
            ShowDialogue();
        }
    }

    private void EndDialogue()
    {
        int npcID = GetComponent<SetID>()?.ID ?? -1;
        OnDialogueEnd?.Invoke(npcID);

        Debug.Log("Diálogo finalizado");

        if (conversationAnimator != null)
        {
            conversationAnimator.SetTrigger("EndConversation");
        }

        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("EndConversation");
        }
        else
        {
            Debug.LogWarning("El jugador no tiene un Animator.");
        }

        if (gameObject.CompareTag("Ship"))
        {
            if (sceneTransition == null)
            {
                sceneTransition = FindFirstObjectByType<SceneTransition>();
                if (sceneTransition == null)
                {
                    Debug.LogError("No se encontró SceneTransition en la escena.");
                    return;
                }
            }
            sceneTransition.LoadLevelForest();
        }
        if (gameObject.CompareTag("Forest"))
        {
            if (sceneTransition == null)
            {
                sceneTransition = FindFirstObjectByType<SceneTransition>();
                if (sceneTransition == null)
                {
                    Debug.LogError("No se encontró SceneTransition en la escena.");
                    return;
                }
            }
            sceneTransition.LoadLevelForestExit();
        }
        if (gameObject.CompareTag("ShipEnd"))
        {
            if (sceneTransition == null)
            {
                sceneTransition = FindFirstObjectByType<SceneTransition>();
                if (sceneTransition == null)
                {
                    Debug.LogError("No se encontró SceneTransition en la escena.");
                    return;
                }
            }
            sceneTransition.LoadLevelFinal();
        }
        if (sceneDependentToggle == null)
        {
            sceneDependentToggle = FindObjectOfType<SceneDependentToggle>(true);
            if (sceneDependentToggle == null)
            {
                Debug.LogError("No se encontró SceneDependentToggle en la escena.");
            }
        }
        if (gameObject.CompareTag("Pulpo"))
        {
            PulpilloMovement pulpillo = GetComponent<PulpilloMovement>();
            if (pulpillo != null)
            {
                pulpillo.enabled = true;
                PlayerPrefs.SetInt("pulpilloTalked", 1);
                PlayerPrefs.Save();
            }
            else
            {
                Debug.LogWarning("No se encontró el componente PulpilloMovement en el objeto.");
            }
        }

        isDialogueActive = false;
        dialogueCanvas.gameObject.SetActive(false);
        optionsContainer.SetActive(false);

        MonoBehaviour movementScript = player.GetComponent(characterMovementScriptName) as MonoBehaviour;
        if (movementScript != null) movementScript.enabled = true;

        if (dialogueIdle != null)
        {
            if (dialogue2 != null && isMissionComplete == false)
            {
                dialogueIdle.enabled = true;
            }
            else if (dialogue2 != null && isMissionComplete == true)
            {
                dialogue2.enabled = true;
                dialogueIdle.enabled = false;
            }
        }

        if (dialogueID < 2)
        {
            Destroy(this);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        freelookCamera.SetActive(true);
    }

    private bool CheckCondition(string key)
    {
        return PlayerPrefs.GetInt(key, 0) == 1;
    }
}
