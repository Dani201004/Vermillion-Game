using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarImage;
    [SerializeField] private CharacterStats characterStats;

    [Header("Valores para aumento de salud")]
    [SerializeField] private float healAmount = 100f;  // Cantidad de vida que se incrementará
    [SerializeField] private float maxHealAmount = 200f;  // Límite máximo de vida para curar

    private void Start()
    {
        if (characterStats != null)
        {
            characterStats.OnHealthChanged += UpdateHealthBar;
            UpdateHealthBar();
        }
        else
        {
            //Debug.LogError("No se encontró el componente CharacterStats en la escena.");
        }
    }
    private void Update()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (characterStats != null && healthBarImage != null)
        {
            if (characterStats.CurrentHealth > 0)
            {
                float healthPercentage = Mathf.Clamp01((float)characterStats.CurrentHealth / characterStats.MaxHealth);
                healthBarImage.fillAmount = healthPercentage;
            }
        }
    }

    public void HealCharacter()
    {
        if (characterStats != null)
        {
            // Incrementamos la vida del personaje sin sobrepasar la vida máxima
            int newHealth = Mathf.Min(Mathf.RoundToInt(characterStats.CurrentHealth + healAmount), Mathf.RoundToInt(characterStats.MaxHealth));


            characterStats.CurrentHealth = newHealth;

            UpdateHealthBar();
        }
    }
}