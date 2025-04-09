using UnityEngine;

public class SkillTreeCanvas : MonoBehaviour
{
    private HUDController hUDController;
    public static SkillTreeCanvas instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Si deseas que persista entre escenas
            this.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        // Busca el HudManager en la escena (incluso en objetos hijos activos)
        HUDController hUDController = FindObjectOfType<HUDController>();

        if (hUDController != null)
        {
            Debug.Log("hUDController encontrado en la escena: " + hUDController.gameObject.name);
            // Aquí puedes guardar la referencia o realizar acciones adicionales con el HudManager
        }
        else
        {
            Debug.LogWarning("No se encontró el hUDController en la escena.");
        }
    }
    public void ResumeGameCanvas()
    {
        hUDController.ResumeGame();
    }
}