using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ������
/// </summary>
[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Audio/SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    public List<SoundData> sounds;

    public SoundData GetSound(string id)
    {
        return sounds.Find(s => s.id == id);
    }
}
