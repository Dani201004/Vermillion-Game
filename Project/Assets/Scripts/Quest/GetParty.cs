using UnityEngine;

public class GetParty : QuestBehaviour
{
    public bool uiCanBeShown;
    void Start()
    {
        Debug.Log("Misi�n: Encontrar 3 aliados comenzada");
        QuestName = "Consigue Aliados";
        Description = "Para inscribirte en el equipo de b�squeda, es necesario que encuentres a otros tres aliados y as� formar un equipo. La taberna parece un buen lugar para buscar.";

        Goals.Add(new GetPartyGoal(this, 1, "Encuentra 3 aliados", false, 0, 3));
        Goals.ForEach(g => g.Init());
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
    }
}
