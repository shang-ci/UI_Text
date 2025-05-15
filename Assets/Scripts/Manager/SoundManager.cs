using UnityEngine;
using System.Collections.Generic; 
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; } 

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource; // ������һ�� sfxSource��ͨ�� PlayOneShot ���ţ��򴴽������

    [Header("Audio Clips")]
    public AudioClip mainThemeBGM;
    // ������һ���б���ֵ�����������Ч
    public List<AudioClipWithName> sfxClips;
    private Dictionary<string, AudioClip> _sfxDictionary;

    [Header("Audio Mixer Groups (Optional)")]
    public AudioMixerGroup masterMixerGroup;
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup sfxMixerGroup;

    // ������ Inspector �з�������� AudioClip
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

        // ��ʼ����Ч�ֵ�
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
        // ������������
        if (SaveManager.Instance != null && SaveManager.Instance.LoadedData != null)
        {
            GameSettingsData settings = SaveManager.Instance.LoadedData;
            SetMasterVolume(settings.masterVolume);
            SetMusicVolume(settings.musicVolume);
            SetSFXVolume(settings.sfxVolume);
        }
        else // Ĭ������
        {
            SetMasterVolume(1f);
            SetMusicVolume(0.8f);
            SetSFXVolume(0.8f);
        }

        PlayBGM(mainThemeBGM); // ��ҳ�Զ�����BGM
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
            sfxSource.PlayOneShot(clipToPlay); // PlayOneShot �����ϵ�ǰ���ڲ��ŵ�����SFX�����sfxSourceͬʱ�ڲ�����������Ч��
        }
        else
        {
            Debug.LogWarning($"SoundManager: SFX clip '{clipName}' not found or sfxSource is null.");
        }
    }

    // ʹ�� AudioMixer (�Ƽ���ʽ�������)
    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);
        if (masterMixerGroup)
        {
            // AudioMixer ʹ�ö�����λ (dB)��0dB ��ԭʼ������-80dB �Ǿ���
            // ת����ʽ: Mathf.Log10(volume) * 20
            masterMixerGroup.audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(0.0001f, _masterVolume)) * 20);
        }
        else
        {
            AudioListener.volume = _masterVolume; // �������Mixer������ȫ������
        }
    }

    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        if (musicMixerGroup)
        {
            musicMixerGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(0.0001f, _musicVolume)) * 20);
        }
        else if (bgmSource) // ֱ�ӿ��� AudioSource
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
        else if (sfxSource) // ֱ�ӿ��� AudioSource
        {
            sfxSource.volume = _sfxVolume;
        }
    }

    // ��ѡ����ȡ��ǰ����ֵ (��Ҫ���ڳ�ʼ��UI)
    public float GetMasterVolume() => _masterVolume;
    public float GetMusicVolume() => _musicVolume;
    public float GetSFXVolume() => _sfxVolume;
}
