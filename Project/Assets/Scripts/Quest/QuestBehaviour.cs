using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestBehaviour : MonoBehaviour
{
    public List<Goal> Goals { get; set; } = new List<Goal>();
    public string QuestName { get; set; }
    public string Description { get; set; }
    public bool Completed { get; set; }

    public bool realCompleted { get; set; }

    public int rewardExperience;

    private void Awake()
    {
        GameObject[] questManagers = GameObject.FindGameObjectsWithTag("QuestManager");

        if (questManagers.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        rewardExperience = PlayerPrefs.GetInt("ExperienciaMisiones", 0);
    }

    public void CheckGoals()
    {
        Completed = Goals.All(g => g.Completed);
    }

    public void GiveReward()
    {
        rewardExperience += 4;
        PlayerPrefs.SetInt("ExperienciaMisiones", rewardExperience);
        PlayerPrefs.Save();
    }

    public static void ResetQuests()
    {
        QuestBehaviour[] quests = FindObjectsOfType<QuestBehaviour>();
        foreach (QuestBehaviour quest in quests)
        {
            Destroy(quest.gameObject);
        }
    }
}
