using UnityEngine;

public class ChangeCameras : MonoBehaviour
{
    [SerializeField] private Camera[] cameras;
    [SerializeField] private Canvas[] canvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        cameras[0].gameObject.SetActive(true);
        cameras[1].gameObject.SetActive(false);
        cameras[2].gameObject.SetActive(false);
        cameras[3].gameObject.SetActive(false);
        cameras[4].gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        OnInteractConversation();
    }

    private void OnInteractConversation()
    {

        if (canvas[0].isActiveAndEnabled)
        {
            //Debug.Log("Cam1");
            cameras[0].gameObject.SetActive(false);
            cameras[1].gameObject.SetActive(true);
            cameras[2].gameObject.SetActive(false);
            cameras[3].gameObject.SetActive(false);
            cameras[4].gameObject.SetActive(false);
        }
        else if (canvas[1].isActiveAndEnabled)
        {
            //Debug.Log("Cam2");
            cameras[0].gameObject.SetActive(false);
            cameras[1].gameObject.SetActive(false);
            cameras[2].gameObject.SetActive(true);
            cameras[3].gameObject.SetActive(false);
            cameras[4].gameObject.SetActive(false);
        }
        else if (canvas[2].isActiveAndEnabled)
        {
            //Debug.Log("Cam3");
            cameras[0].gameObject.SetActive(false);
            cameras[1].gameObject.SetActive(false);
            cameras[2].gameObject.SetActive(false);
            cameras[3].gameObject.SetActive(true);
            cameras[4].gameObject.SetActive(false);
        }
        else if (canvas[3].isActiveAndEnabled)
        {
            //Debug.Log("Cam4");
            cameras[0].gameObject.SetActive(false);
            cameras[1].gameObject.SetActive(false);
            cameras[2].gameObject.SetActive(false);
            cameras[3].gameObject.SetActive(false);
            cameras[4].gameObject.SetActive(true);
        }
        else
        {
            //Debug.Log("Cam0");
            cameras[0].gameObject.SetActive(true);
            cameras[1].gameObject.SetActive(false);
            cameras[2].gameObject.SetActive(false);
            cameras[3].gameObject.SetActive(false);
            cameras[4].gameObject.SetActive(false);
        }

    }
}
