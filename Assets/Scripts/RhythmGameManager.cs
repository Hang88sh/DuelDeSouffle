using System.Collections;
using UnityEngine;
using TMPro;

public class RhythmGameManager : MonoBehaviour
{
    [Header("Références")]
    public BallSpawner_Rhythm ballSpawner;            // Gestionnaire de génération de balles
    public MusicTimelineManager musicTimeline;        // Contrôle de la timeline musicale
    public TextMeshProUGUI countdownText;             // Texte pour le compte à rebours
    public EndScreenUI endScreenUI;                   // UI de fin
    public AudioSource musicSource;                   // Source audio de la musique

    [Header("État du jeu")]
    public bool isPaused = false;                     // Indique si le jeu est en pause
    public float startTime = 0f;                      // Temps de démarrage actuel (Time.time)
    public float accumulatedPlayTime = 0f;            // Temps cumulé joué avant la pause

    private bool gameEnded = false;                   // Marqueur de fin de jeu
    private bool musicFinished = false;

    void Start()
    {
        // S'assure que le gestionnaire de langue est présent
        if (LocalizationManager.Instance == null)
        {
            GameObject loc = Instantiate(Resources.Load<GameObject>("LocalizationManager"));
            loc.name = "LocalizationManager";
        }

        StartCoroutine(StartCountdownRoutine());
    }

    void Update()
    {
        if (gameEnded || isPaused) return;

        Debug.Log($"音乐状态: 播放中 = {musicSource.isPlaying}, 结束 = {musicFinished}");

        if (musicSource != null && !musicSource.isPlaying && !musicFinished)
        {
            Debug.Log($"音乐状态: 播放中 = {musicSource.isPlaying}, 结束 = {musicFinished}");
            musicFinished = true;
            EndGame();
            return;
        }
    }

    // Coroutine de compte à rebours avant le démarrage
    IEnumerator StartCountdownRoutine()
    {
        countdownText.gameObject.SetActive(true);

        string text3 = LocalizationManager.Instance.GetText("countdown_3");
        string text2 = LocalizationManager.Instance.GetText("countdown_2");
        string text1 = LocalizationManager.Instance.GetText("countdown_1");
        string startText = LocalizationManager.Instance.GetText("start");

        yield return AnimateCountdown(text3);
        yield return AnimateCountdown(text2);
        yield return AnimateCountdown(text1);
        yield return AnimateCountdown(startText);

        countdownText.gameObject.SetActive(false);

        ballSpawner?.SpawnNewBall();
        musicTimeline?.StartTimeline();
        musicSource?.Play();

        startTime = Time.time;            // Démarre le chronomètre
        accumulatedPlayTime = 0f;         // Réinitialise le temps cumulé
        isPaused = false;
    }

    // Met en pause le jeu (chronomètre logique uniquement)
    public void PauseGame()
    {
        isPaused = true;
        accumulatedPlayTime += Time.time - startTime;

        if (musicSource != null)
        {
            Debug.Log("[PauseGame] Pause musique manuelle !");
            musicSource.Pause();
        }
    }

    // Reprend le jeu (le temps repart de maintenant)
    public void ResumeGame()
    {
        isPaused = false;
        startTime = Time.time;

        if (musicSource != null)
        {
            Debug.Log("[ResumeGame] Reprise musique !");
            musicSource.UnPause(); 
        }
    }

    // Décale startTime pour compenser un délai externe (ex : pause menu)
    public void AddToStartTime(float seconds)
    {
        startTime += seconds;
    }

    void EndGame()
    {
        gameEnded = true;

        Debug.Log("EndGame() 被调用！");
        if (endScreenUI != null)
        {
            Debug.Log("EndGame() 被调用！");
            endScreenUI.ShowScore();
        }

        else
        {
            Debug.LogWarning("endScreenUI n’est pas assigné !");
        }
            
    }


    // Animation individuelle du compte à rebours (texte qui grossit puis diminue)
    IEnumerator AnimateCountdown(string text)
    {
        countdownText.text = text;
        countdownText.transform.localScale = Vector3.one * 0.5f;

        float t = 0f, d = 0.5f;
        while (t < d)
        {
            float k = Mathf.SmoothStep(0.5f, 1.5f, t / d);
            countdownText.transform.localScale = Vector3.one * k;
            t += Time.deltaTime;
            yield return null;
        }

        countdownText.transform.localScale = Vector3.one * 1.5f;
        yield return new WaitForSeconds(0.2f);

        t = 0f; d = 0.3f;
        while (t < d)
        {
            float k = Mathf.SmoothStep(1.5f, 1f, t / d);
            countdownText.transform.localScale = Vector3.one * k;
            t += Time.deltaTime;
            yield return null;
        }

        countdownText.transform.localScale = Vector3.one;
        yield return new WaitForSeconds(0.2f);
    }
}
