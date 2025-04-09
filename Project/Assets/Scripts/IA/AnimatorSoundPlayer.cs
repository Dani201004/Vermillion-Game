using UnityEngine;

public class AnimatorSoundPlayer : MonoBehaviour
{
    [SerializeField] private Animator animator; // Animator del objeto
    [SerializeField] private string stateName; // Nombre del estado que activará el sonido
    [SerializeField] private AudioClip soundClip; // Sonido a reproducir
    private AudioSource audioSource;

    private bool hasPlayed = false; // Para evitar que el sonido se repita constantemente

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        CheckAnimationState();
    }

    void CheckAnimationState()
    {
        if (animator == null) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName(stateName) && !hasPlayed)
        {
            PlaySound();
            hasPlayed = true; // Evita que el sonido se repita en cada frame
        }
        else if (!stateInfo.IsName(stateName))
        {
            hasPlayed = false; // Permite que el sonido vuelva a sonar si el estado se repite
        }
    }

    void PlaySound()
    {
        if (soundClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundClip);
        }
    }
}
