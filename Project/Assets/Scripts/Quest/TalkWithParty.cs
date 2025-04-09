using UnityEngine;

public class TalkWithParty : MonoBehaviour
{
    [SerializeField] GameObject uiQuest;
    [SerializeField] GameObject uiQuestPanel;
    [SerializeField] Collider githyankiCollider;
    private bool isDetected;
    private SceneDependentToggle sceneDependentToggle;

    private void Start()
    {
        Debug.Log("Misión: Hablar con tus aliados comenzada");
        uiQuest.SetActive(true);
        uiQuestPanel.SetActive(true);
        sceneDependentToggle = FindObjectOfType<SceneDependentToggle>(true);
    }

    private void Update()
    {
        if (isDetected && Input.GetKey(KeyCode.E))
        {
            uiQuest.SetActive(false);
            uiQuestPanel.SetActive(false);

            // Verificar que la referencia no sea nula antes de destruirla
            if (githyankiCollider != null && sceneDependentToggle.toggleState == true)
            {
                Destroy(githyankiCollider);
                githyankiCollider = null;  // Limpiar la referencia para evitar futuros accesos
            }

            // Destruir este componente para detener la ejecución del script
            Destroy(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isDetected = true;
        }
    }
}
