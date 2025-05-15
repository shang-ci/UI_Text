using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField]private GameObject savePanle;//存档面板
    // 可以添加更多面板

    [Header("Main Panel Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton; 
    private Button settingsButton;
    private Button creditsButton;
    private Button quitGameButton;

    [Header("Settings Panel Elements")]
    private Slider masterVolumeSlider;
    private Slider musicVolumeSlider;
    private Slider sfxVolumeSlider;
    private Button backButtonFromSettings;
    // 可以添加更多设置项，如分辨率、语言等

    [Header("Credits Panel Elements")]
    private Button backButtonFromCredits;


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
    }

    void Start()
    {
        // 按钮事件监听 
        // 主面板
        if (newGameButton) newGameButton.onClick.AddListener(NewGame);
        if (loadGameButton) loadGameButton.onClick.AddListener(OpenSavePanle);
        if (settingsButton) settingsButton.onClick.AddListener(OpenSettingsPanel);
        if (creditsButton) creditsButton.onClick.AddListener(OpenCreditsPanel);
        if (quitGameButton) quitGameButton.onClick.AddListener(QuitGame);

        // 设置面板
        if (backButtonFromSettings) backButtonFromSettings.onClick.AddListener(CloseSettingsPanel);
        if (masterVolumeSlider) masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (musicVolumeSlider) musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (sfxVolumeSlider) sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        
        if (backButtonFromCredits) backButtonFromCredits.onClick.AddListener(CloseCreditsPanel);


        // ----- 初始化UI状态 -----
        ShowMainPanel(); // 默认显示主面板
        LoadSettingsToUI(); // 从存档加载设置到UI
    }

    private void Update()
    {
        // 点击面板外部关闭
        if (savePanle != null && savePanle.activeSelf && Input.GetMouseButtonDown(0))
        {
            // 检查点击是否在savePanle外部
            if (!IsPointerOverUIObject(savePanle))
            {
                savePanle.SetActive(false);
            }
        }
    }

    #region 面板控制
    /// <summary>
    /// 显示主面板并隐藏其他面板
    /// </summary>
    private void ShowMainPanel()
    {
        if (mainPanel) mainPanel.SetActive(true);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false);
    }

    private void OpenSettingsPanel()
    {
        if (mainPanel) mainPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(true);
        LoadSettingsToUI(); // 每次打开设置面板时，确保显示的是当前最新的设置
    }

    private void CloseSettingsPanel()
    {
        ShowMainPanel();
        // 可以在这里触发一次设置保存
        SaveManager.Instance?.SaveGameSettings();
    }

    private void OpenCreditsPanel()
    {
        if (mainPanel) mainPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(true);
    }

    private void CloseCreditsPanel()
    {
        ShowMainPanel();
    }

    public void OpenSavePanle()
    {
        savePanle.SetActive(true);
    }

    /// <summary>
    /// 判断当前鼠标是否在指定UI对象（及其子对象）上
    /// </summary>
    private bool IsPointerOverUIObject(GameObject panel)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject == panel || result.gameObject.transform.IsChildOf(panel.transform))
            {
                return true;
            }
        }
        return false;
    }

    #endregion

    // 按钮功能
    private void NewGame()
    {
        Debug.Log("开始游戏!");

        SceneManager.LoadSceneAsync("First"); 
        SoundManager.Instance?.PlaySFX("ButtonClick");
    }

    private void QuitGame()
    {
        Debug.Log("退出游戏!");
        SoundManager.Instance?.PlaySFX("ButtonClick");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 在编辑器中停止播放
#endif
    }

    // 设置相关 
    private void LoadSettingsToUI()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.LoadedData != null)
        {
            GameSettingsData settings = SaveManager.Instance.LoadedData;
            if (masterVolumeSlider) masterVolumeSlider.value = settings.masterVolume;
            if (musicVolumeSlider) musicVolumeSlider.value = settings.musicVolume;
            if (sfxVolumeSlider) sfxVolumeSlider.value = settings.sfxVolume;

            // 更新SoundManager中的音量
            SoundManager.Instance?.SetMasterVolume(settings.masterVolume);
            SoundManager.Instance?.SetMusicVolume(settings.musicVolume);
            SoundManager.Instance?.SetSFXVolume(settings.sfxVolume);
        }
        else
        {
            // 如果没有存档数据，设置UI为默认值
            if (masterVolumeSlider) masterVolumeSlider.value = 1f;
            if (musicVolumeSlider) musicVolumeSlider.value = 0.8f;
            if (sfxVolumeSlider) sfxVolumeSlider.value = 0.8f;
        }
    }

    private void OnMasterVolumeChanged(float value)
    {
        SoundManager.Instance?.SetMasterVolume(value);
        if (SaveManager.Instance != null && SaveManager.Instance.LoadedData != null)
        {
            SaveManager.Instance.LoadedData.masterVolume = value;
            // SaveManager.Instance.SaveGameSettings(); // 可以选择实时保存或在关闭设置时保存
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        SoundManager.Instance?.SetMusicVolume(value);
        if (SaveManager.Instance != null && SaveManager.Instance.LoadedData != null)
        {
            SaveManager.Instance.LoadedData.musicVolume = value;
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        SoundManager.Instance?.SetSFXVolume(value);
        if (SaveManager.Instance != null && SaveManager.Instance.LoadedData != null)
        {
            SaveManager.Instance.LoadedData.sfxVolume = value;
        }
    }

    // 可选：在某些UI操作后播放标准点击音效
    private void PlayStandardClickSound()
    {
        SoundManager.Instance?.PlaySFX("ButtonClick"); 
    }
}
