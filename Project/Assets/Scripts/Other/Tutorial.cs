using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private Text[] page;
    private int actualPage = 0;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    private const string TutorialKey = "TutorialCompleted"; // Clave en PlayerPrefs

    private void Start()
    {
        // Si el tutorial ya fue visto, se oculta y se destruye
        if (PlayerPrefs.GetInt(TutorialKey, 0) == 1)
        {
            gameObject.SetActive(false);
            return;
        }

        // Muestra la primera página
        for (int i = 0; i < page.Length; i++)
        {
            page[i].gameObject.SetActive(i == 0);
        }

        UpdateButtons();
    }

    public void NextPage()
    {
        if (actualPage < page.Length - 1)
        {
            page[actualPage].gameObject.SetActive(false);
            actualPage++;
            page[actualPage].gameObject.SetActive(true);
        }

        UpdateButtons();
    }

    public void PreviousPage()
    {
        if (actualPage > 0)
        {
            page[actualPage].gameObject.SetActive(false);
            actualPage--;
            page[actualPage].gameObject.SetActive(true);
        }

        UpdateButtons();
    }

    private void UpdateButtons()
    {
        nextButton.gameObject.SetActive(actualPage < page.Length - 1);
        previousButton.gameObject.SetActive(actualPage > 0);
    }

    public void CloseTutorial()
    {
        // Guarda en PlayerPrefs que el tutorial ha sido completado
        PlayerPrefs.SetInt(TutorialKey, 1);
        PlayerPrefs.Save();

        // Oculta el tutorial
        gameObject.SetActive(false);
    }
}

