using UnityEngine;

public class Travel : MonoBehaviour
{
    private Animator animator;
    private Transform player;
    private Rigidbody playerRb;
    private bool isTraveling = false;

    [SerializeField] private Vector3 destination; // Posición de destino ajustable (por cada barco)
    [SerializeField] private Vector3 playerOffset; // Ajuste de posición del jugador en el barco

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("No se encontró un Animator en " + gameObject.name);
        }
    }


    public void ForestTravel()
    {
        if (animator != null)
        {
            animator.SetTrigger("Ship");
        }

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.isKinematic = true;
            }
        }
        else
        {
            Debug.LogError("No se encontró un GameObject con la etiqueta 'Player' en la escena.");
        }

        if (player != null)
        {
            player.position = destination;
            player.SetParent(this.transform);
            player.localPosition = playerOffset;
            isTraveling = true;
            //Debug.Log("Jugador teletransportado y ahora es hijo del barco: " + gameObject.name);
        }
        else
        {
            Debug.LogWarning("No se puede teletransportar porque no se encontró al jugador.");
        }
    }

    public void FinalTravel()
    {
        if (animator != null)
        {
            animator.SetTrigger("Ship");
        }

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.isKinematic = true;
            }
        }
        else
        {
            Debug.LogError("No se encontró un GameObject con la etiqueta 'Player' en la escena.");
        }

        if (player != null)
        {
            player.position = destination;
            player.SetParent(this.transform);
            player.localPosition = playerOffset;
            isTraveling = true;
            //Debug.Log("Jugador teletransportado y ahora es hijo del barco: " + gameObject.name);
        }
        else
        {
            Debug.LogWarning("No se puede teletransportar porque no se encontró al jugador.");
        }
    }

    private void LateUpdate()
    {
        if (isTraveling && player != null)
        {
            // Mantiene al jugador en la posición correcta relativa al barco
            player.localPosition = playerOffset;
        }
    }

    private void ReleasePlayer()
    {
        if (player != null)
        {
            player.SetParent(null);
            isTraveling = false;
            if (playerRb != null)
            {
                playerRb.isKinematic = false;
            }
            //Debug.Log("Jugador liberado del barco: " + gameObject.name);
        }
    }
}