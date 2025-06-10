using UnityEngine;
using System.Collections;

/// <summary>
/// ������Ƶ������
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("��Ƶ����")]
    public SoundLibrary soundLibrary;
    public int poolSize = 10;

    private AudioSourcePool audioPool;
    private AudioSource bgmSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioPool = new AudioSourcePool(transform, poolSize);
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
    }

    /// <summary>
    /// ����������Ч
    /// </summary>
    /// <param name="id"></param>
    public void PlaySFX(string id)
    {
        SoundData data = soundLibrary.GetSound(id);
        if (data == null)
        {
            Debug.LogWarning($"SFX ID {id} not found.");
            return;
        }

        AudioSource source = audioPool.Get();
        source.clip = data.clip;
        source.volume = data.volume;
        source.loop = data.loop;
        source.Play();
        StartCoroutine(ReleaseAfterPlay(source));
    }

    /// <summary>
    /// �ȴ���Ƶ������ɺ��ͷ���ƵԴ
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    private IEnumerator ReleaseAfterPlay(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);
        audioPool.Release(source);
    }

    /// <summary>
    /// ���ű�������
    /// </summary>
    /// <param name="id"></param>
    public void PlayBGM(string id)
    {
        SoundData data = soundLibrary.GetSound(id);
        if (data == null)
        {
            Debug.LogWarning($"BGM ID {id} not found.");
            return;
        }

        bgmSource.clip = data.clip;
        bgmSource.volume = data.volume;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }
}
