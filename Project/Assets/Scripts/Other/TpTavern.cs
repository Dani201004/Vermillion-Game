using System.Collections;
using UnityEngine;

public class TpTavern : MonoBehaviour
{
    [SerializeField] private GameObject tavern;
    [SerializeField] private GameObject allMap;
    [SerializeField] private Transform tavernPosition;
    [SerializeField] private Transform outsidePosition;

    [SerializeField] private GameObject leaves1;
    [SerializeField] private GameObject leaves2;
    [SerializeField] private GameObject leaves3;

    private void Start()
    {
        tavern.SetActive(false);
        allMap.SetActive(true);
        leaves1.SetActive(true);
        leaves2.SetActive(true);
        leaves3.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (gameObject.name == "EnterTrigger")
            {
                StartCoroutine(EnterTavern(other));
            }
            else if (gameObject.name == "ExitTrigger")
            {
                StartCoroutine(ExitTavern(other));
            }
        }
    }

    private IEnumerator EnterTavern(Collider other)
    {
        CharacterController characterController = other.GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        // Espera un frame para asegurar que la física ya terminó su callback
        yield return null;

        tavern.SetActive(true);
        allMap.SetActive(false);
        leaves1.SetActive(false);
        leaves2.SetActive(false);
        leaves3.SetActive(false);

        if (tavernPosition != null)
        {
            other.transform.position = tavernPosition.position;
        }
        else
        {
            Debug.LogWarning("La posición de la taberna no está asignada.");
        }

        if (characterController != null)
        {
            characterController.enabled = true;
        }
    }

    private IEnumerator ExitTavern(Collider other)
    {
        CharacterController characterController = other.GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        // Espera un frame
        yield return null;

        tavern.SetActive(false);
        allMap.SetActive(true);
        leaves1.SetActive(true);
        leaves2.SetActive(true);
        leaves3.SetActive(true);

        if (outsidePosition != null)
        {
            other.transform.position = outsidePosition.position;
        }
        else
        {
            Debug.LogWarning("La posición de salida no está asignada.");
        }

        if (characterController != null)
        {
            characterController.enabled = true;
        }
    }
}