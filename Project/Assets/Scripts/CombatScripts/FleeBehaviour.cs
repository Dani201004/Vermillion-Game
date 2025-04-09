using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class FleeBehaviour : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] VideoClip[] videoClips;
    [SerializeField] GameObject videoContainer;
    private TurnManager turnManager;

    private SceneTransitionManager sceneTransitionManager;
    private SceneDependentToggle sceneDependentToggle;

    private void Start()
    {
        // Buscar dinámicamente el objeto en la escena
        videoContainer = GameObject.Find("RawImage");

        if (videoContainer == null)
        {
            Debug.LogError("VideoContainer no encontrado en la escena.");
        }

        turnManager = FindFirstObjectByType<TurnManager>();

        sceneTransitionManager = FindFirstObjectByType<SceneTransitionManager>();
        if (sceneTransitionManager == null)
        {
            Debug.LogError("SceneTransitionManager no encontrado en la escena.");
        }

        SceneDependentToggle sceneDependentToggle = FindObjectOfType<SceneDependentToggle>(true);
        if (sceneDependentToggle == null)
        {
            Debug.LogWarning("No se encontró un objeto con SceneDependentToggle en la escena.");
        }
    }

    public void Flee()
    {
        sceneTransitionManager.LoadExplorationScene();
        if (sceneDependentToggle == null)
        {
            sceneDependentToggle = FindObjectOfType<SceneDependentToggle>(true);
            if (sceneDependentToggle == null)
            {
                Debug.LogError("No se encontró SceneDependentToggle en la escena.");
            }
        }

        // Si se encontró, se llama a ToggleState
        sceneDependentToggle?.ToggleState();
    }

    public void CheckDiceRoll()
    {
        videoContainer.SetActive(true);

        int randomNumber = Random.Range(1, 20);
        videoPlayer.clip = videoClips[randomNumber - 1];
        videoPlayer.Play();

        ChangeResult.ChangeTextResult(randomNumber.ToString());

        StartCoroutine(HandleDiceRoll(randomNumber));
    }

    private IEnumerator HandleDiceRoll(int randomNumber)
    {
        yield return new WaitForSeconds(4f);

        if (randomNumber <= 10)
        {
            //Debug.Log("Unable to flee! Dice Roll: " + randomNumber);
            turnManager.EndTurn();
        }
        else
        {
            //Debug.Log("Fleed successfully! Dice Roll: " + randomNumber);
            Flee();
        }
    }
}
