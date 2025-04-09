using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class CinematicChangeScene : MonoBehaviour
{
    [SerializeField] VideoPlayer player;
    private SceneTransition transition;

    [SerializeField] Image fondo;

    // Variable para controlar el cooldown del skip
    private bool skipCinematicCooldown = false;

    private void Start()
    {
        player = this.gameObject.GetComponent<VideoPlayer>();

        transition = FindFirstObjectByType<SceneTransition>();

        if (transition == null)
        {
            Debug.LogError("No se encontró SceneTransition en la escena.");
        }

        // Desactivar la imagen de fondo
        if (fondo != null)
        {
            fondo.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Fondo no asignado en el inspector.");
        }

        player.loopPointReached += OnCinematicEnd;
        SoundManager.Instance.MuteForCinematic();
        player.Play();
    }

    private void OnCinematicEnd(VideoPlayer vp)
    {
        // La cinemática terminó, se procede a cambiar de escena.
        transition.LoadLevelCinematic();
    }

    public void SkipCinematic()
    {
        if (skipCinematicCooldown)
            return; // Si está en cooldown, se ignora la acción.

        skipCinematicCooldown = true; // Activar el cooldown.

        // Activar la imagen de fondo
        if (fondo != null)
        {
            fondo.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Fondo no asignado en el inspector.");
        }

        transition.LoadLevelCinematic();
        player.Stop();
        SoundManager.Instance?.StopMusicForCombat();

        StartCoroutine(ResetSkipCinematicCooldown());
    }

    private IEnumerator ResetSkipCinematicCooldown()
    {
        yield return new WaitForSeconds(10);
        skipCinematicCooldown = false;
    }
}
