using UnityEngine;

public class CombatGoal : Goal
{
    DialogueSystem dialogue;
    private int enemyRequirements = 1;

    public CombatGoal(QuestBehaviour quest, string description, bool completed, int currentAmount, int requiredAmount)
    {
        this.Quest = quest;
        this.Description = description;
        this.Completed = completed;
        this.CurrentAmount = currentAmount;
        this.RequiredAmount = requiredAmount;
    }


    public override void Init()
    {
        base.Init();
        dialogue = new DialogueSystem();
        dialogue.isMissionComplete = false;
    }

    void GoblinKill(GameObject enemy)
    {
        if (enemy = GameObject.FindGameObjectWithTag("Goblin"))
        {
            this.CurrentAmount = this.CurrentAmount + 1;
            Check();
        }
    }
}
