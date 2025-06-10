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
    [SerializeField] private GameObject savePanle;//存档面板

    [Header("面板")] // Inspector中的标题
    public GameObject mainMenuPanel; // 在Inspector中指定你的MainMenuPanel
    public GameObject[] contentPanels; // 在Inspector中按顺序指定你的Panel1到Panel6
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

    [Header("辅助属性")]
    [SerializeField]private Transform currentPanel; // 当前显示的面板
    [SerializeField]private CanvasManager canvasManager; 


    private void Awake()
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

        canvasManager = GetComponent<CanvasManager>();
    }

    private void Start()
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

        // 检测Tab键按下事件
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            AudioManager.Instance?.PlaySFX("PageSwitching"); // 播放切换页面音效
            //进入主菜单
            EnterMainMenu();
        }

        // 检测Esc键按下事件
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AudioManager.Instance?.PlaySFX("PageSwitching"); // 播放切换页面音效
            //返回上一级菜单
            BackToLastMenu();
        }
    }

    private void OnDisable()
    {
        currentPanel = null; // 清除当前面板引用

        // 清理按钮事件监听
    }

    #region 面板控制
    /// <summary>
    /// 打开目标子面板——需要从按钮的 OnClick() 事件中调用
    /// </summary>
    /// <param name="panelIndex"></param>
    public void ShowContentPanel(int panelIndex)
    {
        AudioManager.Instance?.PlaySFX("Button"); // 播放按钮点击音效


        // 验证索引是否有效
        if (panelIndex < 0 || panelIndex >= contentPanels.Length)
        {
            Debug.LogError("无效的面板索引: " + panelIndex);
            return;
        }

        // 隐藏主菜单
        HideMainMenu();

        // 先隐藏所有子内容面板
        HideAllContentPanels();

        // 显示选定的内容面板
        contentPanels[panelIndex].SetActive(true);

        if (contentPanels[panelIndex] == canvasManager.equipmentMenuPanel)
        {
            canvasManager.ShowButtonPanel2(); // 显示按钮面板
            canvasManager.ShowStarPanel2(); // 显示星星面板
        }

        currentPanel = contentPanels[panelIndex].transform;
    }

    /// <summary>
    /// 可选：一个返回主菜单的方法 (例如，从内容面板的“返回”按钮调用)
    /// </summary>
    public void ShowMainMenu()
    {
        HideAllContentPanels();

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            canvasManager.ShowIconPanel();
            canvasManager.AnimateStarPanel();
            currentPanel = mainMenuPanel.transform;
        }
    }

    /// <summary>
    /// 隐藏主菜单面板
    /// </summary>
    private void HideMainMenu()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
            currentPanel = null;
            canvasManager?.HideButtonPanel(); //把按钮移出去，返回主菜单时可以重新做动效
            canvasManager?.HideIconPanel();
        }
    }

    /// <summary>
    /// 隐藏所有子内容面板
    /// </summary>
    private void HideAllContentPanels()
    {
        foreach (GameObject panel in contentPanels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }


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

    #region 辅助函数
    /// <summary>
    /// 返回到上一个菜单
    /// </summary>
    private void BackToLastMenu()
    {
        //子菜单打开
        if (currentPanel != null && currentPanel.gameObject != mainMenuPanel)
        {
            ShowMainMenu();
            canvasManager.HideButtonPanel2();
        }
        else if(currentPanel != null)//主菜单打开
        {
            HideMainMenu();

            //TODO:继续游戏

        }
    }

    /// <summary>
    /// 进入主菜单——TAB
    /// </summary>
    private void EnterMainMenu()
    {
        if (currentPanel != null)
        {
            Debug.Log("当前面板: " + currentPanel.name + "不能打开主菜单");

            //TODO:播放音效
        }
        else
        {
            // 如果没有当前面板，则显示主菜单
            ShowMainMenu();
        }
    }


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

    /// <summary>
    /// 设置相关 
    /// </summary>
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

    /// <summary>
    /// 处理主音量变化事件
    /// </summary>
    /// <param name="value"></param>
    private void OnMasterVolumeChanged(float value)
    {
        SoundManager.Instance?.SetMasterVolume(value);
        if (SaveManager.Instance != null && SaveManager.Instance.LoadedData != null)
        {
            SaveManager.Instance.LoadedData.masterVolume = value;
            // SaveManager.Instance.SaveGameSettings(); // 可以选择实时保存或在关闭设置时保存
        }
    }

    /// <summary>
    /// 处理音乐音量变化事件
    /// </summary>
    /// <param name="value"></param>
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

    #endregion
}
