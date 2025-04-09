using System.Collections;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] GameObject dropDown;
    [SerializeField] GameObject map;
    [SerializeField] GameObject inventory;
    [SerializeField] GameObject settings;
    private GameObject skilltree;
    [SerializeField] GameObject quests;

    private SceneTransition sceneTransition;
    private GameObject activePanel = null;

    // Variables para el cooldown del cambio a MainMenu
    [SerializeField] private float goToMainMenuCooldownDuration = 10f;
    private bool canGoToMainMenu = true;

    private void Start()
    {
        sceneTransition = FindFirstObjectByType<SceneTransition>();
    }

    private void LateUpdate()
    {
        // Buscar el objeto que contiene el componente SkillTreeCanvas (aunque esté desactivado)
        SkillTreeCanvas onLoadComponent = FindObjectOfType<SkillTreeCanvas>(true);
        if (onLoadComponent != null)
        {
            skilltree = onLoadComponent.gameObject;
            // Debug.Log("Skilltree encontrado: " + skilltree.name);
        }
        else
        {
            Debug.LogError("No se encontró el componente SkillTreeCanvas en la escena.");
        }
    }

    private void TogglePanel(GameObject panel)
    {
        if (activePanel == panel)
        {
            panel.SetActive(false);
            activePanel = null;
            ResumeGame();
        }
        else
        {
            if (activePanel != null)
            {
                activePanel.SetActive(false);
            }
            panel.SetActive(true);
            activePanel = panel;

            if (panel == dropDown)
            {
                PauseGame();
            }
        }
    }

    public void ToggleMap() => TogglePanel(map);
    public void ToggleInventory() => TogglePanel(inventory);
    public void ToggleSettings() => TogglePanel(settings);
    public void ToggleQuests() => TogglePanel(quests);
    public void ToggleSkilltree() => TogglePanel(skilltree);
    public void ToggleDropDown() => TogglePanel(dropDown);

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void GoToMainMenu()
    {
        if (!canGoToMainMenu)
        {
            Debug.Log("El cambio a Main Menu está en cooldown.");
            return;
        }

        canGoToMainMenu = false;
        Time.timeScale = 1f;
        sceneTransition.LoadLevelMenu();
        StartCoroutine(ResetMainMenuCooldown());
    }

    private IEnumerator ResetMainMenuCooldown()
    {
        yield return new WaitForSeconds(goToMainMenuCooldownDuration);
        canGoToMainMenu = true;
    }
}



