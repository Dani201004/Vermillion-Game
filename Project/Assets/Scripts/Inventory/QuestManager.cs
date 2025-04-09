using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    [SerializeField] GameObject questHUD;

    // Variable estática para almacenar la instancia única
    private static QuestManager instance;

    void Awake()
    {
        // Si ya existe una instancia, destruye esta para que solo quede una.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Si no existe, la asignamos y marcamos este objeto para que no se destruya al cargar nuevas escenas.
        instance = this;
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(questHUD);
    }

    void Update()
    {
        // Activamos o desactivamos el questHUD según la escena activa.
        if (SceneManager.GetActiveScene().buildIndex == 6)
        {
            questHUD.SetActive(true);
        }
        else
        {
            questHUD.SetActive(false);
        }
    }

    // Método que destruye todas las instancias de QuestManager
    public static void DestroyAllInstances()
    {
        QuestManager[] managers = FindObjectsOfType<QuestManager>();
        foreach (QuestManager qm in managers)
        {
            Destroy(qm.gameObject);
        }
        // Reiniciamos la instancia estática.
        instance = null;
    }
}