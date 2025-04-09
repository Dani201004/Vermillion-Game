
using UnityEngine;

public class GetPartyGoal : Goal
{
    public int AllyID { get; set; }
    DialogueSystem dialogue;

    public GetPartyGoal(QuestBehaviour quest, int ally, string description, bool completed, int currentAmount, int requiredAmount)
    {
        this.Quest = quest;
        this.AllyID = ally;
        this.Description = description;
        this.Completed = completed;
        this.CurrentAmount = currentAmount;
        this.RequiredAmount = requiredAmount;
    }

    public override void Init()
    {
        base.Init();
        DialogueSystem.OnDialogueEnd += AllyAcquired;
    }

    void AllyAcquired(int id)
    {
        Debug.Log($"AllyAcquired() ejecutado - ID Recibido: {id}, ID Esperado: {this.AllyID}");

        if (id == this.AllyID)
        {
            this.CurrentAmount = this.CurrentAmount + 1;
            Check();

            Debug.Log($"Aliado agregado - Total: {this.CurrentAmount}/{this.RequiredAmount}");
        }

        if (CurrentAmount == RequiredAmount)
        {

            DialogueSystem.OnDialogueEnd -= AllyAcquired;
            Debug.Log("Se alcanzó la cantidad requerida. Desuscribiendo evento.");
        }
    }
}
