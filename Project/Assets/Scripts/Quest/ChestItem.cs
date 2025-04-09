using UnityEngine;

public class ChestItem : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    public bool hasChest;
    bool isDetected;
    [SerializeField] GameObject chest;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas.gameObject.SetActive(false);
        hasChest = false;
        chest = this.gameObject;
    }

    void Update()
    {
        if (isDetected)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Has abierto el cofre");
                hasChest = true;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isDetected = true;
            canvas.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isDetected = false;
        canvas.gameObject.SetActive(false);
    }
}

