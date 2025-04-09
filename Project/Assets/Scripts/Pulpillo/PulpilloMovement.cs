using UnityEngine;

public class PulpilloMovement : MonoBehaviour
{
    private Transform target;
    private Rigidbody rb;

    [SerializeField] private Vector3 offset = new Vector3(0.54f, 1f, 0f);
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    // Par�metros para el movimiento de bobbing (subir y bajar)
    [SerializeField] private float bobbingAmplitude = 0.2f;  // Altura m�xima del bobbing
    [SerializeField] private float bobbingFrequency = 1f;      // Frecuencia (ciclos por segundo)

    private void Start()
    {
        // Buscamos el objeto con tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogError("No se encontr� ning�n objeto con el tag 'Player'");
        }

        // Obtenemos el Rigidbody del objeto
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No se encontr� Rigidbody en el objeto. Por favor, a�ade uno.");
        }
        else
        {
            // Usamos detecci�n continua de colisiones para evitar tunneling en movimientos r�pidos
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }
    private void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogError("No se encontr� ning�n objeto con el tag 'Player'");
        }
    }
    private void FixedUpdate()
    {
        if (target != null && rb != null)
        {
            // Calcula un desplazamiento vertical oscilante usando una funci�n seno.
            float bobbingOffset = bobbingAmplitude * Mathf.Sin(Time.time * bobbingFrequency * 2 * Mathf.PI);

            // Calculamos la posici�n deseada relativa al player, sumando el bobbing al offset vertical.
            Vector3 desiredPosition = target.position
                + target.right * offset.x
                + target.up * (offset.y + bobbingOffset);

            // Suavizamos el movimiento con Lerp y movemos el Rigidbody
            Vector3 newPosition = Vector3.Lerp(rb.position, desiredPosition, followSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);

            // Calculamos la rotaci�n deseada copiando la del player y sum�ndole 90� en el eje Y
            Quaternion targetRotation = Quaternion.Euler(0, target.eulerAngles.y + 90f, 0);
            Quaternion newRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newRotation);
        }
    }
}
