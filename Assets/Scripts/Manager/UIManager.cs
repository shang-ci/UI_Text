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
    [SerializeField] private GameObject savePanle;//�浵���

    [Header("���")] // Inspector�еı���
    public GameObject mainMenuPanel; // ��Inspector��ָ�����MainMenuPanel
    public GameObject[] contentPanels; // ��Inspector�а�˳��ָ�����Panel1��Panel6
    // ������Ӹ������

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
    // ������Ӹ����������ֱ��ʡ����Ե�

    [Header("Credits Panel Elements")]
    private Button backButtonFromCredits;

    [Header("��������")]
    [SerializeField]private Transform currentPanel; // ��ǰ��ʾ�����
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
        // ��ť�¼����� 
        // �����
        if (newGameButton) newGameButton.onClick.AddListener(NewGame);
        if (loadGameButton) loadGameButton.onClick.AddListener(OpenSavePanle);
        if (settingsButton) settingsButton.onClick.AddListener(OpenSettingsPanel);
        if (creditsButton) creditsButton.onClick.AddListener(OpenCreditsPanel);
        if (quitGameButton) quitGameButton.onClick.AddListener(QuitGame);

        // �������
        if (backButtonFromSettings) backButtonFromSettings.onClick.AddListener(CloseSettingsPanel);
        if (masterVolumeSlider) masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (musicVolumeSlider) musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (sfxVolumeSlider) sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        
        if (backButtonFromCredits) backButtonFromCredits.onClick.AddListener(CloseCreditsPanel);


        // ----- ��ʼ��UI״̬ -----
        ShowMainPanel(); // Ĭ����ʾ�����
        LoadSettingsToUI(); // �Ӵ浵�������õ�UI
    }

    private void Update()
    {
        // �������ⲿ�ر�
        if (savePanle != null && savePanle.activeSelf && Input.GetMouseButtonDown(0))
        {
            // ������Ƿ���savePanle�ⲿ
            if (!IsPointerOverUIObject(savePanle))
            {
                savePanle.SetActive(false);
            }
        }

        // ���Tab�������¼�
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            AudioManager.Instance?.PlaySFX("PageSwitching"); // �����л�ҳ����Ч
            //�������˵�
            EnterMainMenu();
        }

        // ���Esc�������¼�
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AudioManager.Instance?.PlaySFX("PageSwitching"); // �����л�ҳ����Ч
            //������һ���˵�
            BackToLastMenu();
        }
    }

    private void OnDisable()
    {
        currentPanel = null; // �����ǰ�������

        // ����ť�¼�����
    }

    #region ������
    /// <summary>
    /// ��Ŀ������塪����Ҫ�Ӱ�ť�� OnClick() �¼��е���
    /// </summary>
    /// <param name="panelIndex"></param>
    public void ShowContentPanel(int panelIndex)
    {
        AudioManager.Instance?.PlaySFX("Button"); // ���Ű�ť�����Ч


        // ��֤�����Ƿ���Ч
        if (panelIndex < 0 || panelIndex >= contentPanels.Length)
        {
            Debug.LogError("��Ч���������: " + panelIndex);
            return;
        }

        // �������˵�
        HideMainMenu();

        // �������������������
        HideAllContentPanels();

        // ��ʾѡ�����������
        contentPanels[panelIndex].SetActive(true);

        if (contentPanels[panelIndex] == canvasManager.equipmentMenuPanel)
        {
            canvasManager.ShowButtonPanel2(); // ��ʾ��ť���
            canvasManager.ShowStarPanel2(); // ��ʾ�������
        }

        currentPanel = contentPanels[panelIndex].transform;
    }

    /// <summary>
    /// ��ѡ��һ���������˵��ķ��� (���磬���������ġ����ء���ť����)
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
    /// �������˵����
    /// </summary>
    private void HideMainMenu()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
            currentPanel = null;
            canvasManager?.HideButtonPanel(); //�Ѱ�ť�Ƴ�ȥ���������˵�ʱ������������Ч
            canvasManager?.HideIconPanel();
        }
    }

    /// <summary>
    /// �����������������
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
    /// ��ʾ����岢�����������
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
        LoadSettingsToUI(); // ÿ�δ��������ʱ��ȷ����ʾ���ǵ�ǰ���µ�����
    }

    private void CloseSettingsPanel()
    {
        ShowMainPanel();
        // ���������ﴥ��һ�����ñ���
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
    /// �жϵ�ǰ����Ƿ���ָ��UI���󣨼����Ӷ�����
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

    #region ��������
    /// <summary>
    /// ���ص���һ���˵�
    /// </summary>
    private void BackToLastMenu()
    {
        //�Ӳ˵���
        if (currentPanel != null && currentPanel.gameObject != mainMenuPanel)
        {
            ShowMainMenu();
            canvasManager.HideButtonPanel2();
        }
        else if(currentPanel != null)//���˵���
        {
            HideMainMenu();

            //TODO:������Ϸ

        }
    }

    /// <summary>
    /// �������˵�����TAB
    /// </summary>
    private void EnterMainMenu()
    {
        if (currentPanel != null)
        {
            Debug.Log("��ǰ���: " + currentPanel.name + "���ܴ����˵�");

            //TODO:������Ч
        }
        else
        {
            // ���û�е�ǰ��壬����ʾ���˵�
            ShowMainMenu();
        }
    }


    // ��ť����
    private void NewGame()
    {
        Debug.Log("��ʼ��Ϸ!");

        SceneManager.LoadSceneAsync("First"); 
        SoundManager.Instance?.PlaySFX("ButtonClick");
    }

    private void QuitGame()
    {
        Debug.Log("�˳���Ϸ!");
        SoundManager.Instance?.PlaySFX("ButtonClick");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // �ڱ༭����ֹͣ����
#endif
    }

    /// <summary>
    /// ������� 
    /// </summary>
    private void LoadSettingsToUI()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.LoadedData != null)
        {
            GameSettingsData settings = SaveManager.Instance.LoadedData;
            if (masterVolumeSlider) masterVolumeSlider.value = settings.masterVolume;
            if (musicVolumeSlider) musicVolumeSlider.value = settings.musicVolume;
            if (sfxVolumeSlider) sfxVolumeSlider.value = settings.sfxVolume;

            // ����SoundManager�е�����
            SoundManager.Instance?.SetMasterVolume(settings.masterVolume);
            SoundManager.Instance?.SetMusicVolume(settings.musicVolume);
            SoundManager.Instance?.SetSFXVolume(settings.sfxVolume);
        }
        else
        {
            // ���û�д浵���ݣ�����UIΪĬ��ֵ
            if (masterVolumeSlider) masterVolumeSlider.value = 1f;
            if (musicVolumeSlider) musicVolumeSlider.value = 0.8f;
            if (sfxVolumeSlider) sfxVolumeSlider.value = 0.8f;
        }
    }

    /// <summary>
    /// �����������仯�¼�
    /// </summary>
    /// <param name="value"></param>
    private void OnMasterVolumeChanged(float value)
    {
        SoundManager.Instance?.SetMasterVolume(value);
        if (SaveManager.Instance != null && SaveManager.Instance.LoadedData != null)
        {
            SaveManager.Instance.LoadedData.masterVolume = value;
            // SaveManager.Instance.SaveGameSettings(); // ����ѡ��ʵʱ������ڹر�����ʱ����
        }
    }

    /// <summary>
    /// �������������仯�¼�
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

    // ��ѡ����ĳЩUI�����󲥷ű�׼�����Ч
    private void PlayStandardClickSound()
    {
        SoundManager.Instance?.PlaySFX("ButtonClick"); 
    }

    #endregion
}
