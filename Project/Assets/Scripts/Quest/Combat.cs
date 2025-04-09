using UnityEngine;

public class Combat : QuestBehaviour
{
    public bool uiCanBeShown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Misión: Combate contra los goblins comenzada");
        QuestName = "Encuentra a los goblins y derrotalos";
        Description = "Se me ha encomendado la mision de buscar y acabar con unos goblins que acechan la zona.";
        Goals.Add(new CombatGoal(this, "Encuentra a los goblins y derrotalos", false, 0, 1));
        Goals.ForEach(g => g.Init());
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
    }
}