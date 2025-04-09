using UnityEngine;

public class BookItem : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    public bool hasBook;
    bool isDetected;
    [SerializeField] GameObject book;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas.gameObject.SetActive(false);
        hasBook = false;
        book = this.gameObject;
    }

    void Update()
    {
        if (isDetected)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Has recogido el libro");
                book.gameObject.SetActive(false);
                hasBook = true;
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
