using UnityEngine;

public class OpenChestGoal : Goal
{
    public bool IsChestOpened { get; set; }
    Chest chest { get; set; }

    public OpenChestGoal(QuestBehaviour quest, bool chestOpened, string description, bool completed, int currentAmount, int requiredAmount)
    {
        this.Quest = quest;
        this.IsChestOpened = chestOpened;
        this.Description = description;
        this.Completed = completed;
        this.CurrentAmount = currentAmount;
        this.RequiredAmount = requiredAmount;
    }

    public override void Init()
    {
        base.Init();
        chest = GameObject.FindGameObjectWithTag("ChestQuest").GetComponent<Chest>();
        Debug.Log(" " + chest.gameObject.name);
        if (chest != null)
        {
            Chest.OnChestOpened += ChestOpened;
        }
        else
        {
            Debug.LogError("Chest no encontrado");
        }
    }

    void ChestOpened(bool chestOpened)
    {
        chest.isOpened = chestOpened;
        if (chest.playerInRange) //&& Input.GetKey(KeyCode.E))
        {
            chestOpened = true;
            this.CurrentAmount = this.RequiredAmount;
            Check();
            Debug.Log("CofreAbierto" + this.CurrentAmount);

        }
        else
        {
            Debug.Log("No se ha abierto el cofre");
        }

        if (CurrentAmount >= RequiredAmount)
        {
            Chest.OnChestOpened -= ChestOpened;
        }

    }
}
