

public class ReturnBookGoal : Goal
{
    DialogueSystem dialogue;

    public ReturnBookGoal(QuestBehaviour quest, string description, bool completed, int currentAmount, int requiredAmount)
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

}