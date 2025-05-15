/// <summary>
/// ��Ҫ���������
/// </summary>
[System.Serializable] 
public class GameSettingsData
{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    // ������Ӹ������ã���ֱ��ʡ����ԡ��ϴ���Ĺؿ���
    public string playerName;
    public int lastLevelCompleted;

    // ���캯������������Ĭ��ֵ
    public GameSettingsData()
    {
        masterVolume = 1f;
        musicVolume = 0.8f;
        sfxVolume = 0.8f;
        playerName = "Player";
        lastLevelCompleted = 0;
    }
}
