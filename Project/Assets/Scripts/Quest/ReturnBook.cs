using UnityEngine;

public class ReturnBook : QuestBehaviour
{
    //item de la mision
    BookItem book;
    GameObject bookitem;
    //id de mision
    //public int missionId = 2;
    public bool uiCanBeShown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        book = FindObjectOfType<BookItem>(true); // Busca aunque esté desactivado
        if (book != null)
        {
            bookitem = book.gameObject;
            bookitem.SetActive(true);
        }
        Debug.Log("Misión: Devolver libro comenzada");
        QuestName = "Encuentra el libro";
        Description = "La bruja me ha pedido buscar su grimorio, el cual debería estar en alguna parte de la taberna.";
        Goals.Add(new ReturnBookGoal(this, "Encuentra el libro", false, 0, 1));
        Goals.ForEach(g => g.Init());

        book = GameObject.FindGameObjectWithTag("BookKeyItem").GetComponent<BookItem>();
    }

    private void Update()
    {
        if (this.enabled)
        {
            uiCanBeShown = true;
        }
        else if (!this.enabled)
        {
            uiCanBeShown = false;
        }

        if (book.hasBook == true)
        {
            Goals[0].CurrentAmount = 1;
            Goals[0].Check();

        }
    }

}
