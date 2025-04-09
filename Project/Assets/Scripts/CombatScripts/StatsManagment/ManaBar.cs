using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [SerializeField] private Image manaBarImage;
    [SerializeField] private CharacterStats characterStats;

    [Header("Valores para aumento de mana")]
    [SerializeField] private float manaAmount = 100f;  // Cantidad de mana que se incrementar�
    [SerializeField] private float maxManaAmount = 200f;  // L�mite m�ximo de mana

    private void Start()
    {
        if (characterStats != null)
        {
            characterStats.OnManaChanged += UpdateManaBar;
            UpdateManaBar();
        }
        else
        {
            //Debug.LogError("No se encontr� el componente CharacterStats en la escena.");
        }
    }
    private void Update()
    {
        UpdateManaBar();
    }

    private void UpdateManaBar()
    {
        if (characterStats != null && manaBarImage != null)
        {
            if (characterStats.MaxMana > 0)
            {
                float manaPercentage = Mathf.Clamp01((float)characterStats.CurrentMana / characterStats.MaxMana);
                manaBarImage.fillAmount = manaPercentage;
            }
        }
    }

    public void HealCharacter()
    {
        if (characterStats != null)
        {
            // Incrementamos el mana del personaje sin sobrepasar el mana m�ximo
            int newMana = Mathf.Min(Mathf.RoundToInt(characterStats.CurrentMana + manaAmount), Mathf.RoundToInt(characterStats.MaxMana));


            characterStats.MaxMana = newMana;

            UpdateManaBar();
        }
    }
}
