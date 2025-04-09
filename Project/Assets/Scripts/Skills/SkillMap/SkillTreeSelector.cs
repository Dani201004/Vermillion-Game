using UnityEngine;
using TMPro;

public class SkillTreeSelector : MonoBehaviour
{
    public TMP_Dropdown dropdown; // Referencia al dropdown
    public GameObject skillTreeThief;
    public GameObject skillTreePaladin;
    public GameObject skillTreeCleric;
    public GameObject skillTreeWitch;

    // Referencia al SkillTree_Manager del árbol actualmente activo (opcional, si necesitas acceder a él directamente)
    public SkillTree_Manager currentSkillTreeManager;

    void Start()
    {
        dropdown.onValueChanged.AddListener(ChangeSkillTree);
        ChangeSkillTree(dropdown.value); // Activar el árbol correcto al inicio
    }

    void ChangeSkillTree(int index)
    {
        // Desactivar todos los árboles
        skillTreeThief.SetActive(false);
        skillTreePaladin.SetActive(false);
        skillTreeCleric.SetActive(false);
        skillTreeWitch.SetActive(false);

        // Activar el árbol seleccionado y obtener su SkillTree_Manager
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

        // Actualizar la UI de los skill points según el árbol activo
        if (currentSkillTreeManager != null)
            currentSkillTreeManager.UpdateSkillPoints();
    }
}