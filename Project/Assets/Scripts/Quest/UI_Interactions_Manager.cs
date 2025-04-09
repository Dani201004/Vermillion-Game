using UnityEngine;

public class UI_Interactions_Manager : MonoBehaviour
{
    [SerializeField] GameObject questsPanel;
    [SerializeField] GetParty quest1;
    [SerializeField] ReturnBook quest2;
    [SerializeField] Combat quest3;
    [SerializeField] ReturnBook quest4;
    [SerializeField] GameObject questManager;
    [SerializeField] GameObject quest1UI;
    [SerializeField] GameObject quest2UI;
    [SerializeField] GameObject quest3UI;
    [SerializeField] GameObject quest4UI;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //questsPanel.SetActive(false);
        quest1UI.SetActive(false);
        quest2UI.SetActive(false);
        quest3UI.SetActive(false);
        quest4UI.SetActive(false);


    }
    private void Update()
    {

        quest1 = questManager.GetComponent<GetParty>();
        quest2 = questManager.GetComponent<ReturnBook>();
        quest3 = questManager.GetComponent<Combat>();


        ShowMenu();
        ShowQuests();
    }

    private void ShowMenu()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            questsPanel.SetActive(!questsPanel.activeInHierarchy);
        }
    }

    private void ShowQuests()
    {
        if (quest1 != null && quest1.uiCanBeShown == true)
        {
            quest1UI.SetActive(true);
        }

        if (quest2 != null && quest2.uiCanBeShown == true)
        {
            quest2UI.SetActive(true);
        }
        if (quest3 != null && quest3.uiCanBeShown == true)
        {
            quest3UI.SetActive(true);
        }
        if (quest4 != null && quest4.uiCanBeShown == true)
        {
            quest4UI.SetActive(true);
        }

    }
}