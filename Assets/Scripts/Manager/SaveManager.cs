using UnityEngine;
using System.IO; 

/// <summary>
/// 简单的存档系统
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public GameSettingsData LoadedData { get; private set; }

    private string _saveFileName = "gameSettings.json";
    private string _savePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _savePath = Path.Combine(Application.persistentDataPath, _saveFileName);
        LoadGameSettings();
    }

    public void LoadGameSettings()
    {
        if (File.Exists(_savePath))
        {
            try
            {
                string json = File.ReadAllText(_savePath);
                LoadedData = JsonUtility.FromJson<GameSettingsData>(json);
                Debug.Log("Game settings loaded from: " + _savePath);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading game settings: " + e.Message);
                CreateDefaultSettings();
            }
        }
        else
        {
            Debug.Log("No save file found. Creating default settings.");
            CreateDefaultSettings();
            SaveGameSettings(); // 保存一次默认设置
        }
    }

    private void CreateDefaultSettings()
    {
        LoadedData = new GameSettingsData();
    }

    public void SaveGameSettings()
    {
        if (LoadedData == null)
        {
            Debug.LogWarning("No data to save. Creating default settings first.");
            CreateDefaultSettings();
        }

        try
        {
            string json = JsonUtility.ToJson(LoadedData, true); // true for pretty print
            File.WriteAllText(_savePath, json);
            Debug.Log("Game settings saved to: " + _savePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving game settings: " + e.Message);
        }
    }

    // 示例：当游戏退出时自动保存
    void OnApplicationQuit()
    {
        SaveGameSettings();
    }
}
