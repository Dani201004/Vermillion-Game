using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillTree_Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int skillLevel = 1;
    public SkillDescScript skillScript;

    [SerializeField] string characterName;

    public RawImage rawImageSkill;
    public SkillTree_Link[] linkToThis;
    public SkillTree_Link[] linkGoOut;

    public bool isLearned = false;

    public RawImage m_RawImage;

    void Start()
    {
        m_RawImage = GetComponent<RawImage>();
        rawImageSkill.texture = skillScript.skillImage;
    }

    private SkillTree_Manager GetActiveSkillTreeManager()
    {
        return FindFirstObjectByType<SkillTree_Manager>();
    }

    public void Learn()
    {
        SkillTree_Manager skillTreeManager = GetActiveSkillTreeManager();
        if (!isLearned && skillTreeManager != null && skillTreeManager.skillPoints > 0 && CheckLinkToThis())
        {
            isLearned = true;
            m_RawImage.texture = linkToThis[0].linkedSprite.texture; // Cambiar a sprite de habilidad aprendida
            skillTreeManager.skillPoints--; // Reducir puntos de habilidad
            skillTreeManager.UpdateSkillPoints(); // Actualizar los puntos de habilidad

            // Intentamos encontrar PlayerStats en la escena
            PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.AddSkill(skillScript.skillName);
            }
            else
            {
                Debug.LogWarning($" PlayerStats NO encontrado en la escena. Guardando {skillScript.skillName} en PlayerPrefs.");
                skillTreeManager.SaveSkillToPlayerPrefs(skillScript.skillName, characterName);
            }

            // Reabrimos las conexiones visuales si es necesario
            LinkedLinks();
            OpenLinks(linkGoOut); // Aseguramos que las salidas de esta habilidad se desbloqueen
        }
    }

    public void UnLearn()
    {
        SkillTree_Manager skillTreeManager = GetActiveSkillTreeManager();
        if (isLearned && skillTreeManager != null)
        {
            isLearned = false;
            m_RawImage.color = Color.white; // Restauramos el color original del slot
            skillTreeManager.skillPoints++; // Recuperamos el punto de habilidad
            skillTreeManager.UpdateSkillPoints(); // Actualizamos los puntos de habilidad

            // Eliminar la habilidad de PlayerPrefs
            skillTreeManager.RemoveSkillFromPlayerPrefs(skillScript.skillName, characterName);

            // Cambiar las conexiones visualmente (no desactivar isOpen)
            foreach (SkillTree_Link link in linkToThis)
            {
                link.Unlinked(); // Cambiar a sprite no vinculado
                                 // No desactivar isOpen aquí, solo dejamos el sprite en el estado no vinculado.
            }

            // Cambiar las conexiones de salida visualmente (sin desactivar isOpen)
            foreach (SkillTree_Link link in linkGoOut)
            {
                link.Unlinked(); // Cambiar a sprite no vinculado
                                 // De nuevo, no desactivamos isOpen, solo cambiamos el sprite.
            }
        }
    }

    public void TryAutoLearn()
    {
        SkillTree_Manager skillTreeManager = GetActiveSkillTreeManager();
        if (!isLearned && skillTreeManager != null && CheckLinkToThis() && skillTreeManager.skillPoints > 0)
        {
            Learn();
            OpenLinks(linkGoOut);
        }
    }

    public void UnlinkConnections()
    {
        foreach (SkillTree_Link link in linkGoOut)
        {
            link.Unlinked();
            link.CloseLink();
        }
    }

    bool CheckLinkToThis()
    {
        foreach (SkillTree_Link links in linkToThis)
        {
            if (!links.isOpen)
                return false;
        }
        return true;
    }

    void LinkedLinks()
    {
        foreach (SkillTree_Link links in linkToThis)
        {
            links.Linked();
        }
    }

    public void OpenLinks(SkillTree_Link[] arr)
    {
        foreach (SkillTree_Link links in arr)
        {
            links.OpenLink();
        }

        TryAutoLearn();
    }

    void CloseLinks(SkillTree_Link[] arr)
    {
        foreach (SkillTree_Link links in arr)
        {
            links.CloseLink();
        }
    }

    bool CheckIfAnyLinkedToThis()
    {
        foreach (SkillTree_Link links in linkGoOut)
        {
            if (links.isLinked)
                return false;
        }
        return true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SkillTree_Manager skillTreeManager = GetActiveSkillTreeManager();
        if (skillTreeManager == null) return;

        if (CheckLinkToThis())
        {
            if (!isLearned && skillTreeManager.skillPoints > 0)
            {
                if (linkGoOut.Length > 0)
                    OpenLinks(linkGoOut);
                LinkedLinks();
                Learn();
            }
            else
            {
                if (linkGoOut.Length > 0)
                {
                    if (CheckIfAnyLinkedToThis())
                    {
                        CloseLinks(linkGoOut);
                        OpenLinks(linkToThis);
                        UnLearn();
                    }
                }
                else
                {
                    OpenLinks(linkToThis);
                    UnLearn();
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SkillTree_ToolTip.instance.ShowDesc(skillScript, skillLevel, transform.position);
        if (!isLearned)
            m_RawImage.color = Color.grey;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SkillTree_ToolTip.instance.HideDesc();
        if (!isLearned)
            m_RawImage.color = Color.white;
    }
}
