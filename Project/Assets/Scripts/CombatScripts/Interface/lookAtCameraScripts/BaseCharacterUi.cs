using System.Collections;
using UnityEngine;
using TMPro;

public class BaseCharacterUI : MonoBehaviour
{
    [Header("Texto de daño")]
    public TMP_Text damageText;

    [Header("Texto de curación")]
    public TMP_Text healthText;

    [Header("Texto de maná recuperado")]
    public TMP_Text manaText; // Nuevo texto para maná

    [Header("Cámara para el daño")]
    [SerializeField] private Camera damageCamera;

    [Header("Desplazamiento del texto")]
    [SerializeField] private float textOffset = 2f;

    [Header("Parámetros de animación")]
    public float damageDisplayDuration = 1f;

    protected virtual void LateUpdate()
    {
        if (damageText != null && damageCamera != null)
        {
            damageText.transform.LookAt(damageText.transform.position + damageCamera.transform.rotation * Vector3.forward, damageCamera.transform.rotation * Vector3.up);
            damageText.transform.position += damageCamera.transform.forward * textOffset;
        }

        if (healthText != null && damageCamera != null)
        {
            healthText.transform.LookAt(healthText.transform.position + damageCamera.transform.rotation * Vector3.forward, damageCamera.transform.rotation * Vector3.up);
            healthText.transform.position += damageCamera.transform.forward * textOffset;
        }

        if (manaText != null && damageCamera != null)
        {
            manaText.transform.LookAt(manaText.transform.position + damageCamera.transform.rotation * Vector3.forward, damageCamera.transform.rotation * Vector3.up);
            manaText.transform.position += damageCamera.transform.forward * textOffset;
        }
    }

    public void ShowDamage(int damage)
    {
        if (damageText != null)
        {
            damageText.text = damage.ToString();
            damageText.color = Color.red;
            damageText.gameObject.SetActive(true);
            StartCoroutine(FadeOutDamageText());
        }
    }

    public void ShowHealth(int healAmount)
    {
        if (healthText != null)
        {
            healthText.text = "+" + healAmount.ToString();
            healthText.color = Color.green;
            healthText.gameObject.SetActive(true);
            StartCoroutine(FadeOutHealthText());
        }
    }

    public void ShowMana(int manaAmount)
    {
        if (manaText != null)
        {
            manaText.text = "+" + manaAmount.ToString();
            manaText.color = Color.blue; // Color azul para el maná
            manaText.gameObject.SetActive(true);
            StartCoroutine(FadeOutManaText());
        }
    }

    private IEnumerator FadeOutDamageText()
    {
        yield return FadeOutText(damageText);
    }

    private IEnumerator FadeOutHealthText()
    {
        yield return FadeOutText(healthText);
    }

    private IEnumerator FadeOutManaText()
    {
        yield return FadeOutText(manaText);
    }

    private IEnumerator FadeOutText(TMP_Text text)
    {
        float elapsed = 0f;
        Color originalColor = text.color;

        while (elapsed < damageDisplayDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / damageDisplayDuration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        text.gameObject.SetActive(false);
        text.color = originalColor;
    }
}
