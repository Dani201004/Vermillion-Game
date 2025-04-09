using System.IO;
using UnityEngine;

public class SaveDataCleaner : MonoBehaviour
{
    /// <summary>
    /// Borra todos los archivos de guardado (datos de partida y datos de stats) y limpia los PlayerPrefs.
    /// </summary>
    public void ClearAllSaveDataAndPlayerPrefs()
    {
        // Primero, borrar los archivos de guardado mediante el SaveGameManager
        if (SaveGameManager.Instance != null)
        {
            SaveGameManager.Instance.DeleteAllSaves();
            //Debug.Log("Datos de guardado eliminados.");
        }
        else
        {
            Debug.LogWarning("SaveGameManager no está presente en la escena.");
        }

        // Luego, eliminar los archivos de la carpeta que guarda los stats de los personajes.
        string statsFolder = Path.Combine(Application.persistentDataPath, "PlayerStatsSaves");
        if (Directory.Exists(statsFolder))
        {
            try
            {
                string[] statsFiles = Directory.GetFiles(statsFolder);
                foreach (string file in statsFiles)
                {
                    File.Delete(file);
                    //Debug.Log("Archivo de stats eliminado: " + file);
                }
                //Debug.Log("Todos los archivos de stats han sido eliminados.");
            }
            catch (System.Exception ex)
            {
                //Debug.LogError("Error al borrar los archivos de stats: " + ex.Message);
            }
        }
        else
        {
            Debug.LogWarning("La carpeta de stats no existe: " + statsFolder);
        }

        // Finalmente, borrar todos los PlayerPrefs del juego.
        PlayerPrefs.DeleteAll();
        QuestBehaviour.ResetQuests();
        QuestManager.DestroyAllInstances();
        PlayerPrefs.Save();
        //Debug.Log("Todos los PlayerPrefs han sido eliminados.");
    }
}