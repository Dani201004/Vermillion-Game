using UnityEngine;
using UnityEngine.UI;

public class MovementTutorial : MonoBehaviour
{
    private const string MovementTutorialKey = "MovementTutorialCompleted"; // Clave en PlayerPrefs

    [SerializeField] private Button closeButton; // Asigna el botón de cierre en el Inspector

    private void Start()
    {
        // Si el tutorial ya fue visto, se oculta y se destruye
        if (PlayerPrefs.GetInt(MovementTutorialKey, 0) == 1)
        {
            gameObject.SetActive(false);
            return;
        }

        // Asegurar que el botón cierre el tutorial
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseTutorial);
        }
    }

    public void CloseTutorial()
    {
        // Guarda en PlayerPrefs que el tutorial ha sido completado
        PlayerPrefs.SetInt(MovementTutorialKey, 1);
        PlayerPrefs.Save();

        // Oculta el tutorial
        gameObject.SetActive(false);
    }
}
