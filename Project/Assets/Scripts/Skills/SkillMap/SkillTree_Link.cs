using UnityEngine;
using UnityEngine.UI;

public class SkillTree_Link : MonoBehaviour
{
    public bool isStart = false;
    public Sprite linkedSprite;
    public Sprite unlinkedSprite;
    public Color openColor;
    public Color closeColor;

    public bool isOpen = false;
    public bool isLinked = false;

    private RawImage m_RawImage;

    // Nuevo: Referencia al SkillTree_Slot conectado
    public SkillTree_Slot linkedSlot;

    void Start()
    {
        m_RawImage = GetComponent<RawImage>();

        CloseLink();
        if (isStart)
            OpenLink();
    }

    public void CloseLink()
    {
        m_RawImage.color = closeColor;
        isOpen = false;
        isLinked = false;
    }

    public void Linked()
    {
        m_RawImage.texture = linkedSprite.texture;
        isLinked = true;
    }

    public void Unlinked()
    {
        m_RawImage.texture = unlinkedSprite.texture;
        isLinked = false;
    }

    public void OpenLink()
    {
        m_RawImage.color = openColor;
        isOpen = true;
        isLinked = false;

        // Nuevo: Intentar aprender la habilidad automáticamente
        if (linkedSlot != null)
        {
            linkedSlot.TryAutoLearn();
        }
    }
}