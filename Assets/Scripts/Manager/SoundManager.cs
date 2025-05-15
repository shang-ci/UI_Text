using UnityEngine;
using System.Collections.Generic; 
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; } 

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource; // 可以用一个 sfxSource并通过 PlayOneShot 播放，或创建对象池

    [Header("Audio Clips")]
    public AudioClip mainThemeBGM;
    // 可以用一个列表或字典来管理多个音效
    public List<AudioClipWithName> sfxClips;
    private Dictionary<string, AudioClip> _sfxDictionary;

    [Header("Audio Mixer Groups (Optional)")]
    public AudioMixerGroup masterMixerGroup;
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup sfxMixerGroup;

    // 用于在 Inspector 中方便地命名 AudioClip
    [System.Serializable]
    public struct AudioClipWithName
    {
        public string name;
        public AudioClip clip;
    }

    private float _masterVolume = 1f;
    private float _musicVolume = 0.8f;
    private float _sfxVolume = 0.8f;


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

        // 初始化音效字典
        _sfxDictionary = new Dictionary<string, AudioClip>();
        foreach (var sfx in sfxClips)
        {
            if (!_sfxDictionary.ContainsKey(sfx.name) && sfx.clip != null)
            {
                _sfxDictionary.Add(sfx.name, sfx.clip);
            }
        }
    }

    void Start()
    {
        // 加载音量设置
        if (SaveManager.Instance != null && SaveManager.Instance.LoadedData != null)
        {
            GameSettingsData settings = SaveManager.Instance.LoadedData;
            SetMasterVolume(settings.masterVolume);
            SetMusicVolume(settings.musicVolume);
            SetSFXVolume(settings.sfxVolume);
        }
        else // 默认音量
        {
            SetMasterVolume(1f);
            SetMusicVolume(0.8f);
            SetSFXVolume(0.8f);
        }

        PlayBGM(mainThemeBGM); // 首页自动播放BGM
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (bgmSource && clip)
        {
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        if (bgmSource)
        {
            bgmSource.Stop();
        }
    }

    public void PlaySFX(string clipName)
    {
        if (sfxSource && _sfxDictionary.TryGetValue(clipName, out AudioClip clipToPlay))
        {
            sfxSource.PlayOneShot(clipToPlay); // PlayOneShot 不会打断当前正在播放的其他SFX（如果sfxSource同时在播放其他长音效）
        }
        else
        {
            Debug.LogWarning($"SoundManager: SFX clip '{clipName}' not found or sfxSource is null.");
        }
    }

    // 使用 AudioMixer (推荐方式，更灵活)
    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);
        if (masterMixerGroup)
        {
            // AudioMixer 使用对数单位 (dB)，0dB 是原始音量，-80dB 是静音
            // 转换公式: Mathf.Log10(volume) * 20
            masterMixerGroup.audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(0.0001f, _masterVolume)) * 20);
        }
        else
        {
            AudioListener.volume = _masterVolume; // 如果不用Mixer，这是全局音量
        }
    }

    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        if (musicMixerGroup)
        {
            musicMixerGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(0.0001f, _musicVolume)) * 20);
        }
        else if (bgmSource) // 直接控制 AudioSource
        {
            bgmSource.volume = _musicVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
        if (sfxMixerGroup)
        {
            sfxMixerGroup.audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(0.0001f, _sfxVolume)) * 20);
        }
        else if (sfxSource) // 直接控制 AudioSource
        {
            sfxSource.volume = _sfxVolume;
        }
    }

    // 可选：获取当前音量值 (主要用于初始化UI)
    public float GetMasterVolume() => _masterVolume;
    public float GetMusicVolume() => _musicVolume;
    public float GetSFXVolume() => _sfxVolume;
}
