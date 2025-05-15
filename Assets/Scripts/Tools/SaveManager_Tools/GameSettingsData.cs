/// <summary>
/// 需要保存的数据
/// </summary>
[System.Serializable] 
public class GameSettingsData
{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    // 可以添加更多设置，如分辨率、语言、上次玩的关卡等
    public string playerName;
    public int lastLevelCompleted;

    // 构造函数，用于设置默认值
    public GameSettingsData()
    {
        masterVolume = 1f;
        musicVolume = 0.8f;
        sfxVolume = 0.8f;
        playerName = "Player";
        lastLevelCompleted = 0;
    }
}
