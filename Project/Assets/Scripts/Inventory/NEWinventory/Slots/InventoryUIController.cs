using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance { get; private set; }

    [Header("Referencias en la UI (Padres de los slots)")]
    public Transform inventorySlotsParent;
    public Transform equipmentSlotsParent;

    [Header("Prefab del Item")]
    public GameObject itemPrefab;

    [Header("Tutorial UI")]
    public GameObject tutorialPanel; // Asigna el panel del tutorial en el Inspector

    private InventorySlotInventory[] inventorySlots;
    private InventorySlotEquip[] equipmentSlots;

    private const string TutorialKey = "TutorialCompleted"; // Clave para guardar en PlayerPrefs

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        inventorySlots = inventorySlotsParent.GetComponentsInChildren<InventorySlotInventory>();
        equipmentSlots = equipmentSlotsParent.GetComponentsInChildren<InventorySlotEquip>();
    }

    private void Start()
    {
        CheckTutorial();
    }

    public void ChangeEquipment(string character)
    {
        InventoryManager.ChangeCharacter(character);
        UpdateUI();
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        // Actualiza el inventario.
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            Transform slotTransform = inventorySlots[i].transform;
            for (int c = slotTransform.childCount - 1; c >= 0; c--)
            {
                Destroy(slotTransform.GetChild(c).gameObject);
            }

            if (i < InventoryManager.inventoryItems.Length && InventoryManager.inventoryItems[i] != null)
            {
                GameObject newItemObj = Instantiate(itemPrefab, slotTransform);
                newItemObj.transform.localPosition = Vector3.zero;

                DraggableItem di = newItemObj.GetComponent<DraggableItem>();
                di.item = InventoryManager.inventoryItems[i];

                Image itemImage = newItemObj.GetComponent<Image>();
                if (itemImage != null && di.item.iconSprite != null)
                {
                    itemImage.sprite = di.item.iconSprite;
                    itemImage.enabled = true;
                }
            }
        }

        // Actualiza el equipamiento.
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            Transform slotTransform = equipmentSlots[i].transform;
            for (int c = slotTransform.childCount - 1; c >= 0; c--)
            {
                Destroy(slotTransform.GetChild(c).gameObject);
            }

            if (i < InventoryManager.currentEquipmentItems.Length && InventoryManager.currentEquipmentItems[i] != null)
            {
                GameObject newItemObj = Instantiate(itemPrefab, slotTransform);
                newItemObj.transform.localPosition = Vector3.zero;

                DraggableItem di = newItemObj.GetComponent<DraggableItem>();
                di.item = InventoryManager.currentEquipmentItems[i];

                Image itemImage = newItemObj.GetComponent<Image>();
                if (itemImage != null && di.item.iconSprite != null)
                {
                    itemImage.sprite = di.item.iconSprite;
                    itemImage.enabled = true;
                }
            }
        }
    }

    private void CheckTutorial()
    {
        if (PlayerPrefs.GetInt(TutorialKey, 0) == 1)
        {
            // Si el tutorial ya fue completado, lo desactiva
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(false);
            }
        }
        else
        {
            // Si el tutorial no ha sido completado, lo activa
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(true);
            }
        }
    }

    public void CompleteTutorial()
    {
        // Guarda que el tutorial ha sido completado
        PlayerPrefs.SetInt(TutorialKey, 1);
        PlayerPrefs.Save();

        // Oculta el tutorial
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }
}