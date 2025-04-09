public class TalkWithPartyGoal : Goal
{
    public int AllyID { get; set; }

    public TalkWithPartyGoal(QuestBehaviour quest, int ally, string description, bool completed, int currentAmount, int requiredAmount)
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
        DialogueSystem.OnDialogueEnd += TalkedWithAllies;
    }

    void TalkedWithAllies(int idDialogue)
    {
        if (idDialogue == 1 /*ally.ID == this.AllyID*/)
        {
            this.CurrentAmount = this.CurrentAmount + 1;
            Check();
        }

        if (CurrentAmount >= RequiredAmount)
        {
            DialogueSystem.OnDialogueEnd -= TalkedWithAllies;
        }
    }
}
