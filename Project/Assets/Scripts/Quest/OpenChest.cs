using UnityEngine;

public class OpenChest : QuestBehaviour
{
    ChestItem chest;
    GameObject chestItem;

    public bool uiCanBeShown;
    void Start()
    {
        chest = FindObjectOfType<ChestItem>(true); // Busca aunque est� desactivado
        if (chest != null)
        {
            chestItem = chest.gameObject;
            chestItem.SetActive(true);
        }
        Debug.Log("Misi�n: Abrir un cofre comenzada");
        QuestName = "Abrir un cofre";
        Description = "Un aldeano me ha dicho que cerca de Vermillion hay un cofre, deber�a ir r�pidamente a ver su contenido";

        Goals.Add(new OpenChestGoal(this, false, "Abrir un cofre", false, 0, 1));
        Goals.ForEach(g => g.Init());

        chest = GameObject.FindGameObjectWithTag("ChestQuest").GetComponent<ChestItem>();
    }
    private void Update()
    {
        if (this.enabled)
        {
            uiCanBeShown = true;
        }
        else
        {
            uiCanBeShown = false;
        }

        if (chest.hasChest == true)
        {
            Goals[0].CurrentAmount = 1;
            Goals[0].Check();

        }
    }
}
