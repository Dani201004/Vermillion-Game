using UnityEngine;
using TMPro;

public class SkillTreeSelector : MonoBehaviour
{
    public TMP_Dropdown dropdown; // Referencia al dropdown
    public GameObject skillTreeThief;
    public GameObject skillTreePaladin;
    public GameObject skillTreeCleric;
    public GameObject skillTreeWitch;

    // Referencia al SkillTree_Manager del �rbol actualmente activo (opcional, si necesitas acceder a �l directamente)
    public SkillTree_Manager currentSkillTreeManager;

    void Start()
    {
        dropdown.onValueChanged.AddListener(ChangeSkillTree);
        ChangeSkillTree(dropdown.value); // Activar el �rbol correcto al inicio
    }

    void ChangeSkillTree(int index)
    {
        // Desactivar todos los �rboles
        skillTreeThief.SetActive(false);
        skillTreePaladin.SetActive(false);
        skillTreeCleric.SetActive(false);
        skillTreeWitch.SetActive(false);

        // Activar el �rbol seleccionado y obtener su SkillTree_Manager
        switch (index)
        {
            case 0:
                skillTreeThief.SetActive(true);
                currentSkillTreeManager = skillTreeThief.GetComponent<SkillTree_Manager>();
                break;
            case 1:
                skillTreePaladin.SetActive(true);
                currentSkillTreeManager = skillTreePaladin.GetComponent<SkillTree_Manager>();
                break;
            case 2:
                skillTreeCleric.SetActive(true);
                currentSkillTreeManager = skillTreeCleric.GetComponent<SkillTree_Manager>();
                break;
            case 3:
                skillTreeWitch.SetActive(true);
                currentSkillTreeManager = skillTreeWitch.GetComponent<SkillTree_Manager>();
                break;
        }

        // Actualizar la UI de los skill points seg�n el �rbol activo
        if (currentSkillTreeManager != null)
            currentSkillTreeManager.UpdateSkillPoints();
    }
}