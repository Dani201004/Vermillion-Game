

public class Goal
{
    public QuestBehaviour Quest { get; set; }
    public string Description { get; set; }
    public bool Completed { get; set; }
    public int CurrentAmount { get; set; }
    public int RequiredAmount { get; set; }

    public virtual void Init()
    {

    }
    public void Check()
    {
        if (CurrentAmount == RequiredAmount)
        {
            Completed = true;
            Quest.CheckGoals();
        }
    }

}
