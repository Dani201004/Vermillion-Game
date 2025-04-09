using UnityEngine;

public class ReturnItem : QuestBehaviour
{
    SwordItem sword;
    GameObject swordItem;

    public bool uiCanBeShown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sword = FindObjectOfType<SwordItem>(true); // Busca aunque esté desactivado
        if (sword != null)
        {
            swordItem = sword.gameObject;
            swordItem.SetActive(true);
        }
        Debug.Log("Misión: Devolver espada comenzada");
        QuestName = "Encuentra la espada";
        Description = "La aldeana me ha pedido encontrar su espada y devolvérsela.";
        Goals.Add(new ReturnItemGoal(this, "Encuentra la espada", false, 0, 1));
        Goals.ForEach(g => g.Init());

        sword = GameObject.FindGameObjectWithTag("SwordKeyItem").GetComponent<SwordItem>();
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

        if (sword.hasSword == true)
        {
            Goals[0].CurrentAmount = 1;
            Goals[0].Check();
        }
    }
}
