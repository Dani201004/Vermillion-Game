using UnityEngine;

public class SwordItem : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    public bool hasSword;
    bool isDetected;
    [SerializeField] GameObject sword;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas.gameObject.SetActive(false);
        hasSword = false;
        sword = this.gameObject;
    }

    void Update()
    {
        if (isDetected)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Has recogido la espada");
                sword.gameObject.SetActive(false);
                hasSword = true;
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
