using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenuManager : MonoBehaviour
{
    [SerializeField] TMP_Dropdown resolutionDropDown;
    [SerializeField] Toggle fullscreenToggle;

    Resolution[] allResolutions;
    private bool isFullscreen;
    private int selectedResolution;
    private Resolution[] selectedResolutions; // Ahora es un array en lugar de una lista

    private void Start()
    {
        isFullscreen = Screen.fullScreen;
        allResolutions = Screen.resolutions;

        // Filtrar resoluciones con relación de aspecto 16:9 y eliminar duplicados
        string[] resolutionStrings;
        selectedResolutions = GetFilteredResolutions(out resolutionStrings);

        resolutionDropDown.ClearOptions();
        resolutionDropDown.AddOptions(new System.Collections.Generic.List<string>(resolutionStrings));

        // Obtener la resolución actual
        Resolution currentResolution = Screen.currentResolution;
        selectedResolution = FindClosestResolution(currentResolution);

        resolutionDropDown.value = selectedResolution;
        resolutionDropDown.RefreshShownValue();
    }

    private Resolution[] GetFilteredResolutions(out string[] resolutionStrings)
    {
        System.Collections.Generic.List<Resolution> tempList = new System.Collections.Generic.List<Resolution>();
        System.Collections.Generic.List<string> tempStringList = new System.Collections.Generic.List<string>();

        foreach (Resolution res in allResolutions)
        {
            float aspectRatio = (float)res.width / res.height;

            if (Mathf.Approximately(aspectRatio, 16f / 9f))
            {
                string newRes = res.width + " x " + res.height;

                if (!tempStringList.Contains(newRes))
                {
                    tempStringList.Add(newRes);
                    tempList.Add(res);
                }
            }
        }

        resolutionStrings = tempStringList.ToArray();
        return tempList.ToArray();
    }

    private int FindClosestResolution(Resolution currentResolution)
    {
        for (int i = 0; i < selectedResolutions.Length; i++)
        {
            if (selectedResolutions[i].width == currentResolution.width &&
                selectedResolutions[i].height == currentResolution.height)
            {
                return i;
            }
        }
        return 0; // Si no se encuentra la resolución exacta, seleccionar la primera
    }

    private void ChangeResolution()
    {
        selectedResolution = resolutionDropDown.value;
        Screen.SetResolution(selectedResolutions[selectedResolution].width, selectedResolutions[selectedResolution].height, isFullscreen);
    }

    private void ChangeFullScreen()
    {
        isFullscreen = fullscreenToggle.isOn;
        Screen.SetResolution(selectedResolutions[selectedResolution].width, selectedResolutions[selectedResolution].height, isFullscreen);
    }
}

