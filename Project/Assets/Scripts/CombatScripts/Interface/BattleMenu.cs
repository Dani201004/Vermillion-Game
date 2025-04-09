using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleMenu : MonoBehaviour
{
    private Button[] buttons;
    private int currentIndex = 0;

    [Header("Sprites")]
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite highlightedSprite;
    [SerializeField] private Sprite clickedSprite;

    [Header("Botón Movimiento y Color")]
    [SerializeField] private Vector3 clickOffset = new Vector3(10f, 0f, 0f);
    [SerializeField] private Color clickedColor = Color.gray;

    [Header("Eventos Personalizados")]
    [SerializeField] private List<UnityEvent> buttonClickEvents;

    // Referencia al TurnManager (buscada automáticamente)
    private TurnManager turnManager;

    public void RedefineOnClick(List<UnityEvent> onClickEvents)
    {
        buttonClickEvents = onClickEvents;
    }

    void Start()
    {
        turnManager = FindFirstObjectByType<TurnManager>();

        buttons = GetComponentsInChildren<Button>();

        if (buttons.Length > 0)
        {
            EventSystem.current.SetSelectedGameObject(buttons[currentIndex].gameObject);
        }
        else
        {
            Debug.LogWarning("No se encontraron botones hijos en el objeto.");
        }

        ResetMenus();

        if (buttonClickEvents.Count < buttons.Length)
        {
            Debug.LogWarning("No hay suficientes eventos personalizados asignados para todos los botones.");
        }

        // Asignar dinámicamente los eventos de clic a cada botón
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Captura la variable en el ámbito local
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }
    }


    void Update()
    {
        // Solo permitir el reset si es el turno del jugador
        if (turnManager != null && turnManager.GetCurrentTurnEntity() != null && turnManager.GetCurrentTurnEntity().CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetMenus();
                buttons = GetComponentsInChildren<Button>(); // Se obtiene de nuevo en caso de que los botones hayan cambiado
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            currentIndex = (currentIndex > 0) ? currentIndex - 1 : buttons.Length - 1;
            UpdateHighlightedButton();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            currentIndex = (currentIndex < buttons.Length - 1) ? currentIndex + 1 : 0;
            UpdateHighlightedButton();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnButtonClick();
        }
    }

    void UpdateHighlightedButton()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            Image buttonImage = buttons[i].GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = (i == currentIndex) ? highlightedSprite : defaultSprite;
            }
        }

        if (buttons[currentIndex] != null)
        {
            EventSystem.current.SetSelectedGameObject(buttons[currentIndex].gameObject);
        }
        else
        {
            Debug.LogWarning("El botón seleccionado es nulo.");
        }
    }

    public void OnButtonClick(int buttonIndex = -1)
    {
        if (buttonIndex == -1) // Si no se especifica índice, usa el índice actual
        {
            buttonIndex = currentIndex;
        }

        Button clickedButton = buttons[buttonIndex];
        Image buttonImage = clickedButton.GetComponent<Image>();
        RectTransform buttonRect = clickedButton.GetComponent<RectTransform>();

        if (buttonImage != null && buttonRect != null)
        {
            buttonImage.sprite = clickedSprite;
            StartCoroutine(MoveButtonToRight(buttonRect));
            buttonImage.color = clickedColor;
            ShowSubMenu(clickedButton);
            HideOtherButtons(clickedButton);
            this.enabled = false;

            // Ejecutar el evento personalizado asignado al botón
            if (buttonIndex < buttonClickEvents.Count && buttonClickEvents[buttonIndex] != null)
            {
                buttonClickEvents[buttonIndex].Invoke();
            }
        }
        else
        {
            Debug.LogWarning("El botón no tiene los componentes necesarios (Image y RectTransform).");
        }
    }

    private bool hasMoved = false;

    private IEnumerator MoveButtonToRight(RectTransform buttonRect)
    {
        if (hasMoved) yield break;

        hasMoved = true;

        Vector3 initialPosition = buttonRect.localPosition;
        Vector3 targetPosition = initialPosition + clickOffset;
        float animationDuration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            buttonRect.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        buttonRect.localPosition = targetPosition;
    }

    private void ShowSubMenu(Button clickedButton)
    {
        foreach (Button button in buttons)
        {
            Transform subMenu = button.transform.Find("SubMenu");
            if (subMenu != null)
            {
                subMenu.gameObject.SetActive(false);
            }
        }

        Transform clickedSubMenu = clickedButton.transform.Find("SubMenu");
        if (clickedSubMenu != null)
        {
            clickedSubMenu.gameObject.SetActive(true);
        }
    }

    private void HideOtherButtons(Button clickedButton)
    {
        foreach (Button button in buttons)
        {
            if (button != clickedButton)
            {
                button.gameObject.SetActive(false);
            }
        }
    }

    public void ResetMenus()
    {
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(true);
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.sprite = defaultSprite;
                buttonImage.color = Color.white;
            }
        }

        foreach (Button button in buttons)
        {
            Transform subMenu = button.transform.Find("SubMenu");
            if (subMenu != null)
            {
                subMenu.gameObject.SetActive(false);
            }
        }

        currentIndex = 0;
        EventSystem.current.SetSelectedGameObject(buttons[currentIndex].gameObject);
    }

    public int GetButtonsCount()
    {
        return buttons.Length;
    }
}
