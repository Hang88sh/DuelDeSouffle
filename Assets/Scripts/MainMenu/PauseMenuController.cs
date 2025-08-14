using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PauseMenuController : MonoBehaviour
{
    public enum SceneFilterMode { AllExceptListed, OnlyListed }

    [Header("Activation par scène")]
    public SceneFilterMode sceneFilterMode = SceneFilterMode.AllExceptListed;
    public string[] sceneList = new string[] { "MainMenu" };

    [Header("Références UI")]
    public CanvasGroup cg;
    public RectTransform card;
    public Button btnResume;
    public Button btnRestart;
    public Button btnMainMenu;
    public string mainMenuScene = "MainMenu";

    [Header("Animation")]
    public Vector2 shownPos = Vector2.zero;
    public Vector2 hiddenPos = new Vector2(800, 0);
    [Range(0.05f, 1f)] public float slideDuration = 0.25f;

    bool shown = false;
    float prevTimeScale = 1f;
    bool allowedThisScene = true;
    float pauseTimestamp = 0f;

    private RhythmGameManager rhythmManager;
    private AudioSource musicSource;
    private bool wasMusicPlaying = false;

    void Awake()
    {
        if (!cg) cg = GetComponent<CanvasGroup>();
        if (card) card.anchoredPosition = hiddenPos;

        if (btnResume) btnResume.onClick.AddListener(Resume);
        if (btnRestart) btnRestart.onClick.AddListener(RestartScene);
        if (btnMainMenu) btnMainMenu.onClick.AddListener(GoToMainMenu);

        SetCgState(false, false);

        rhythmManager = FindAnyObjectByType<RhythmGameManager>();
        //musicSource = FindAnyObjectByType<AudioSource>();
        musicSource = rhythmManager?.musicSource;
    }

    void OnEnable()
    {
        RecomputeSceneAllowance();
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (shown) Time.timeScale = 1f;
    }

    void OnActiveSceneChanged(Scene oldS, Scene newS) => RecomputeSceneAllowance();
    void OnSceneLoaded(Scene s, LoadSceneMode m) => RecomputeSceneAllowance();

    void RecomputeSceneAllowance()
    {
        allowedThisScene = IsAllowedInThisScene();

        if (!allowedThisScene)
        {
            shown = false;
            SetCgState(false, false);
            if (card) card.anchoredPosition = hiddenPos;
            Time.timeScale = 1f;
        }
    }

    void Update()
    {
        if (!allowedThisScene) return;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            Toggle();
#else
        if (Input.GetKeyDown(KeyCode.Escape))
            Toggle();
#endif
    }

    // Toggle entre pause et reprise
    public void Toggle() { if (shown) Resume(); else Pause(); }

    // Met en pause le jeu, la musique, et le chronomètre
    public void Pause()
    {
        if (shown || !allowedThisScene) return;
        shown = true;

        prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        StopAllCoroutines();
        StartCoroutine(Anim(true));

        if (musicSource != null)
        {
            wasMusicPlaying = musicSource.isPlaying;
            if (wasMusicPlaying)
                musicSource.Pause(); // Pause de la musique
        }

        pauseTimestamp = Time.realtimeSinceStartup;

        if (rhythmManager != null)
            rhythmManager.PauseGame(); // Pause logique du jeu

        if (btnResume)
            EventSystem.current?.SetSelectedGameObject(btnResume.gameObject);
    }

    // Reprend le jeu, la musique, et le chronomètre
    public void Resume()
    {
        if (!shown) return;
        shown = false;

        Time.timeScale = Mathf.Approximately(prevTimeScale, 0f) ? 1f : prevTimeScale;

        StopAllCoroutines();
        StartCoroutine(Anim(false));

        if (musicSource != null && wasMusicPlaying)
            musicSource.Play(); // Reprise musique

        float pauseDuration = Time.realtimeSinceStartup - pauseTimestamp;

        if (rhythmManager != null)
        {
            rhythmManager.ResumeGame();               // Reprise logique
            rhythmManager.AddToStartTime(pauseDuration); // Corrige le timer
        }
    }

    public void RestartScene()
    {
        StartCoroutine(_RestartRoutine());
    }

    private IEnumerator _RestartRoutine()
    {
        shown = false;
        SetCgState(false, false);
        if (card) card.anchoredPosition = hiddenPos;
        Time.timeScale = 1f;

        yield return null;

        SceneManager.sceneLoaded += _OnSceneReloaded;
        var s = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(s, LoadSceneMode.Single);
    }

    private void _OnSceneReloaded(Scene s, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= _OnSceneReloaded;

        shown = false;
        SetCgState(false, false);
        if (card) card.anchoredPosition = hiddenPos;
        Time.timeScale = 1f;
    }

    void GoToMainMenu()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(mainMenuScene))
            SceneManager.LoadScene(mainMenuScene);
    }

    IEnumerator Anim(bool show)
    {
        cg.blocksRaycasts = true;
        float t = 0f, d = Mathf.Max(0.01f, slideDuration);
        Vector2 p0 = card ? card.anchoredPosition : Vector2.zero;
        Vector2 p1 = show ? shownPos : hiddenPos;
        float a0 = cg.alpha, a1 = show ? 1f : 0f;

        while (t < d)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / d);
            if (card) card.anchoredPosition = Vector2.LerpUnclamped(p0, p1, k);
            cg.alpha = Mathf.LerpUnclamped(a0, a1, k);
            yield return null;
        }

        if (card) card.anchoredPosition = p1;
        SetCgState(show, show);
    }

    void SetCgState(bool visible, bool interactive)
    {
        cg.alpha = visible ? 1f : 0f;
        cg.interactable = interactive;
        cg.blocksRaycasts = interactive;
    }

    bool IsAllowedInThisScene()
    {
        string cur = SceneManager.GetActiveScene().name;
        bool listed = false;
        for (int i = 0; i < sceneList.Length; i++)
            if (!string.IsNullOrEmpty(sceneList[i]) && sceneList[i] == cur) { listed = true; break; }

        return sceneFilterMode == SceneFilterMode.AllExceptListed ? !listed : listed;
    }
}
