using UnityEngine;
using UnityEngine.UI;

public class QuestGiver : MonoBehaviour
{
    public bool AssignedQuest { get; set; }
    public bool Done { get; set; }
    private GameObject quests;
    [SerializeField] string questType;
    QuestBehaviour Quest;
    [SerializeField] Collider missionZone;
    bool isDetected;
    [SerializeField] string questTextObjectName; // Nombre del objeto en la escena
    private Text questText;
    [SerializeField] Button questMenu;
    [SerializeField] DialogueSystem dialogue;
    [SerializeField] DialogueSystem dialogueidle;
    [SerializeField] DialogueSystem dialogue2;
    [SerializeField] GameObject item;


    private void Start()
    {
        Done = false;
        quests = GameObject.FindGameObjectWithTag("QuestsInstance");

        // Buscar el objeto questText en la escena (aunque esté inactivo o en DontDestroyOnLoad)
        questText = FindQuestTextObject();

        if (questText == null)
        {
            Debug.LogWarning($"QuestGiver: No se encontró el objeto de texto con el nombre '{questTextObjectName}'. Asegúrate de que el nombre sea correcto.");
        }
        CheckDialogueStatus();
    }
    private void Update()
    {
        // Buscar la referencia de QuestBehaviour si aún no se ha asignado
        if (Quest == null)
        {
            GameObject questObject = GameObject.FindGameObjectWithTag("QuestManager");
            if (questObject != null)
            {
                Quest = questObject.GetComponent<QuestBehaviour>();
            }
        }

        QuestStateCheck();
        CheckDialogueStatus();
    }

    void QuestStateCheck()
    {
        if (isDetected == true && Input.GetKeyDown(KeyCode.E))
        {
            if (!AssignedQuest)
            {
                AssignQuest();
                Debug.Log("Quest started.");
            }
            else if (AssignedQuest && !Done)
            {
                CheckQuest();
            }
        }
    }

    private void OnTriggerEnter(Collider missionZone)
    {
        if (missionZone.gameObject.tag == "Player")
        {
            isDetected = true;
        }
    }

    private void OnTriggerExit(Collider missionZone)
    {
        isDetected = false;
    }

    void AssignQuest()
    {
        AssignedQuest = true;
        Quest = (QuestBehaviour)quests.AddComponent(System.Type.GetType(questType));
        questText.gameObject.SetActive(true);
        if (questMenu != null)
        {
            questMenu.gameObject.SetActive(true);
        }

    }

    void CheckQuest()
    {
        if (Quest.Completed)
        {
            dialogue = this.gameObject.GetComponent<DialogueSystem>();
            Debug.Log("Completed the quest");
            questText.gameObject.SetActive(false);
            Quest.GiveReward();
            Done = true;

            // Marcar la misión completada en el componente actual.
            dialogue.isMissionComplete = true;
            Debug.Log("dialogue.isMissionComplete: " + dialogue.isMissionComplete);
            dialogue.StartDialogue();
            // Buscar y activar el otro componente DialogueSystem que esté en el mismo GameObject.
            if (Quest != null)
            {
                // Obtén el tipo a partir del string questType.
                System.Type targetType = System.Type.GetType(questType);
                if (targetType != null)
                {
                    // Busca en Quest el componente del tipo indicado.
                    QuestBehaviour specificQuest = Quest.GetComponent(targetType) as QuestBehaviour;
                    if (specificQuest != null)
                    {
                        specificQuest.realCompleted = true;
                        Debug.Log("Se ha establecido realCompleted a true en la misión: " + questType);
                    }
                }
                else
                {
                    Debug.LogWarning("No se pudo obtener el tipo a partir de questType: " + questType);
                }
            }
        }
    }
    void CheckDialogueStatus()
    {
        if (Quest != null)
        {
            // Obtén el tipo a partir del string questType
            System.Type targetType = System.Type.GetType(questType);
            if (targetType != null)
            {
                // Busca en Quest el componente del tipo indicado
                QuestBehaviour specificQuest = Quest.GetComponent(targetType) as QuestBehaviour;
                if (specificQuest != null && specificQuest.Completed)
                {
                    Debug.Log("Eliminando el DialogueSystem y DialogueIdle para la misión: " + questType);
                    if (dialogue != null)
                    {
                        Destroy(dialogue);
                        dialogue = null;
                    }

                }
                if (specificQuest != null && specificQuest.realCompleted)
                {
                    missionZone.enabled = false;
                    Destroy(dialogueidle);
                    dialogueidle = null;
                    dialogue2.enabled = true;
                    questText.gameObject.SetActive(false);
                    item.SetActive(false);
                    if (questMenu != null)
                    {
                        questMenu.gameObject.SetActive(false);
                    }
                    Debug.Log("dialogos eliminados");

                }
            }
            else
            {
                Debug.LogWarning("No se pudo obtener el tipo a partir de questType: " + questType);
            }
        }
    }
    private Text FindQuestTextObject()
    {
        // Esto devuelve TODOS los GameObjects de las escenas cargadas (incluyendo los inactivos)
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(true);
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == questTextObjectName) // Comparamos el nombre definido en el inspector
            {
                Text txt = obj.GetComponent<Text>();
                if (txt != null)
                {
                    return txt;
                }
            }
        }
        return null;
    }
}
