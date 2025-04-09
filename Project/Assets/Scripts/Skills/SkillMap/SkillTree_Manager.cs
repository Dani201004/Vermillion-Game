using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillTree_Manager : MonoBehaviour
{

    public TextMeshProUGUI skillPointsText;
    public int skillPoints = 10;
    public string characterName;


    void Start()
    {
        LoadSkillPoints();
        UpdateSkillPoints();
    }
    void OnEnable()
    {
        // Cuando se activa el SkillTree, actualizamos los skillPoints
        UpdateSkillPoints();
    }
    public void UpdateSkillPoints()
    {
        skillPointsText.SetText("" + skillPoints);
    }

    public void GetSkills()
    {

        SkillTree_Slot[] skillsSlots = FindObjectsOfType<SkillTree_Slot>();


        //Check LearnSkill
        foreach (SkillTree_Slot slot in skillsSlots)
        {

            if (slot.isLearned)
            {
                Debug.Log("Aprendiste " + slot.skillScript.name + " lvl:" + slot.skillLevel);
            }
        }

        gameObject.SetActive(false);
    }
    public void SaveSkillToPlayerPrefs(string skillName, string characterClass)
    {
        string key = "SkillTree" + characterClass;
        string savedSkills = PlayerPrefs.GetString(key, "");
        List<string> skillList = new List<string>(savedSkills.Split(','));

        skillList.RemoveAll(s => string.IsNullOrEmpty(s)); // Eliminar strings vacíos

        if (!skillList.Contains(skillName))
        {
            skillList.Add(skillName);
            PlayerPrefs.SetString(key, string.Join(",", skillList));
            PlayerPrefs.Save();
            Debug.Log($"💾 Habilidad {skillName} guardada para {characterClass} en PlayerPrefs.");
        }
    }
    public List<string> LoadSkillsFromPlayerPrefs(string characterClass)
    {
        Debug.Log($" Clase usada para cargar habilidades: {characterClass}");

        string key = "SkillTree" + characterClass;

        Debug.Log($" Clave usada en GetString: {key}");

        string savedSkills = PlayerPrefs.GetString(key, ""); // Aquí verificamos si GetString usa mal la clave
        List<string> skillList = new List<string>(savedSkills.Split(','));
        skillList.RemoveAll(s => string.IsNullOrEmpty(s));

        return skillList;
    }
    public void RemoveSkillFromPlayerPrefs(string skillName, string characterClass)
    {
        string key = "SkillTree" + characterClass;
        string savedSkills = PlayerPrefs.GetString(key, "");
        List<string> skillList = new List<string>(savedSkills.Split(','));

        // Eliminar la habilidad si está en la lista
        if (skillList.Contains(skillName))
        {
            skillList.Remove(skillName);
            PlayerPrefs.SetString(key, string.Join(",", skillList));
            PlayerPrefs.Save();
            Debug.Log($"🗑️ Habilidad {skillName} eliminada de PlayerPrefs para {characterClass}.");
        }
    }
    public void LoadSkillsForTree(string characterClass)
    {
        SkillTree_Slot[] skillsSlots = FindObjectsOfType<SkillTree_Slot>();

        string savedSkills = PlayerPrefs.GetString("SkillTree_" + characterClass, "");
        List<string> savedSkillList = new List<string>(savedSkills.Split(','));

        foreach (SkillTree_Slot slot in skillsSlots)
        {
            if (savedSkillList.Contains(slot.skillScript.skillName))
            {
                slot.isLearned = true;
                slot.m_RawImage.texture = slot.linkToThis[0].linkedSprite.texture; // Actualiza la imagen del slot
                slot.OpenLinks(slot.linkGoOut); // Si es necesario, desbloquea las habilidades siguientes
            }
            else
            {
                slot.isLearned = false;
                slot.m_RawImage.color = Color.white; // Si no está aprendida, vuelve al estado inicial
                slot.UnlinkConnections(); // Cierra las conexiones visualmente
            }
        }
    }
    public void SaveSkillPoints()
    {
        PlayerPrefs.SetInt("SkillPoints", skillPoints);
        PlayerPrefs.Save();
        Debug.Log("SkillPoints guardados: " + skillPoints);
    }

    /// <summary>
    /// Carga la variable skillPoints desde PlayerPrefs.
    /// Si no existe, se usará el valor por defecto (10).
    /// </summary>
    public void LoadSkillPoints()
    {
        skillPoints = PlayerPrefs.GetInt("SkillPoints", 0);
        Debug.Log("SkillPoints cargados: " + skillPoints);
    }
}

