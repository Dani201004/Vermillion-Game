using System;
using System.IO;
using UnityEngine;

public class SaveFileService
{
    /// <summary>
    /// Ruta de la carpeta donde se guardan los archivos.
    /// </summary>
    public string SaveFolderPath { get; private set; }

    /// <summary>
    /// Límite máximo de archivos guardados.
    /// </summary>
    public int MaxSaveFiles { get; set; } = 10;

    public SaveFileService(string folderName = "Saves")
    {
        SaveFolderPath = Path.Combine(Application.persistentDataPath, folderName);
        EnsureSaveFolderExists();
    }

    private void EnsureSaveFolderExists()
    {
        if (!Directory.Exists(SaveFolderPath))
        {
            Directory.CreateDirectory(SaveFolderPath);
        }
    }

    /// <summary>
    /// Genera una ruta de archivo única para el guardado basado en la escena actual.
    /// </summary>
    public string GenerateSaveFilePath(string sceneName)
    {
        // Se incluyen milisegundos para asegurar la unicidad.
        string identifier = "Partida_" + DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
        string fileName = $"{sceneName}_{identifier}.json";
        return Path.Combine(SaveFolderPath, fileName);
    }

    /// <summary>
    /// Escribe el contenido JSON en el archivo especificado.
    /// </summary>
    public void SaveToFile(string filePath, string jsonData)
    {
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Game saved to: " + filePath);
    }

    /// <summary>
    /// Lee y retorna el contenido de un archivo de guardado.
    /// </summary>
    public string LoadFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }
        return null;
    }

    /// <summary>
    /// Si hay más archivos de guardado que el límite, elimina el más antiguo.
    /// </summary>
    public void ManageSaveFileLimit()
    {
        string[] saveFiles = Directory.GetFiles(SaveFolderPath, "*.json");
        if (saveFiles.Length > MaxSaveFiles)
        {
            Array.Sort(saveFiles, (a, b) => File.GetCreationTime(a).CompareTo(File.GetCreationTime(b)));
            string oldestFile = saveFiles[0];
            File.Delete(oldestFile);
            Debug.Log("Deleted oldest save file: " + oldestFile);
        }
    }

    /// <summary>
    /// Captura una captura de pantalla y la guarda en la ruta indicada.
    /// </summary>
    public void CaptureScreenshot(string screenshotPath)
    {
        ScreenCapture.CaptureScreenshot(screenshotPath);
        Debug.Log("Screenshot saved to: " + screenshotPath);
    }
}