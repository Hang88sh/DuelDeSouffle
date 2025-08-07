using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class TargetZoneManager : MonoBehaviour
{
    [Header("Références")]
    public Transform targetZone;
    public BoxCollider zoneCollider;

    [Header("UI")]
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;

    [Header("Paramètres")]
    public float zoneDuration = 3.5f;
    public float requiredStayTime = 1.5f;
    public float refreshDelay = 3f;

    private Transform ball;
    private float stayTimer = 0f;
    private float zoneTimer = 0f;
    private bool zoneActive = false;
    private bool gameStarted = false;

    private int score = 0;
    private int comboCount = 0;
    private Coroutine comboAnimRoutine;
    public BallSpawner ballSpawner;
    public PersistentBreathText persistentText;
    public ZoneColorController zoneColorController;

    private bool isInsideZone = false;
    private float toleranceTime = 0.3f;
    private float graceTimer = 0f;

    void Start()
    {
        if (LocalizationManager.Instance == null)
        {
            GameObject loc = Instantiate(Resources.Load<GameObject>("LocalizationManager"));
            loc.name = "LocalizationManager";
        }
        countdownText.gameObject.SetActive(false);
        targetZone.gameObject.SetActive(false);
        scoreText.text = "Score: 0";
        comboText.gameObject.SetActive(false);

        if (persistentText != null)
            persistentText.SetText("", Color.white, 0f);

        StartCoroutine(StartCountdownRoutine());
    }

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

        if (ballSpawner != null)
        {
            ballSpawner.SpawnNewBall();
        }

        gameStarted = true;
        NewZone();
    }

    IEnumerator AnimateCountdown(string text)
    {
        countdownText.text = text;
        countdownText.transform.localScale = Vector3.one * 0.5f;

        float t = 0f;
        float duration = 0.5f;

        while (t < duration)
        {
            float scale = Mathf.SmoothStep(0.5f, 1.5f, t / duration);
            countdownText.transform.localScale = Vector3.one * scale;
            t += Time.deltaTime;
            yield return null;
        }

        countdownText.transform.localScale = Vector3.one * 1.5f;
        yield return new WaitForSeconds(0.2f);

        t = 0f;
        duration = 0.3f;

        while (t < duration)
        {
            float scale = Mathf.SmoothStep(1.5f, 1f, t / duration);
            countdownText.transform.localScale = Vector3.one * scale;
            t += Time.deltaTime;
            yield return null;
        }

        countdownText.transform.localScale = Vector3.one;
        yield return new WaitForSeconds(0.2f);
    }

    void Update()
    {
        if (!gameStarted || !zoneActive || ball == null) return;

        zoneTimer += Time.deltaTime;

        bool ballInside = zoneCollider.bounds.Contains(ball.position);

        if (ballInside)
        {
            if (!isInsideZone)
            {
                isInsideZone = true;
                graceTimer = 0f;
            }

            stayTimer += Time.deltaTime;
            graceTimer = 0f;

            zoneColorController?.SetSuccessColor();

            if (stayTimer >= requiredStayTime)
            {
                AddScore();
                EndZone();
            }
        }
        else
        {
            if (isInsideZone)
            {
                graceTimer += Time.deltaTime;

                if (graceTimer > toleranceTime)
                {
                    isInsideZone = false;
                    stayTimer = 0f;
                    graceTimer = 0f;
                    zoneColorController?.SetDefaultColor();
                }
            }
            else
            {
                stayTimer = 0f;
                zoneColorController?.SetDefaultColor();
            }
        }

        if (zoneTimer >= zoneDuration)
        {
            EndZone();
        }
    }

    void AddScore()
    {
        score += 1;
        scoreText.text = "Score: " + score;
        comboCount += 1;
        ShowComboText(comboCount);
    }

    void EndZone()
    {
        zoneActive = false;
        targetZone.gameObject.SetActive(false);

        if (stayTimer < requiredStayTime)
        {
            comboCount = 0;
            HideComboText();
        }

        StartCoroutine(NextZoneAfterDelay());
    }

    IEnumerator NextZoneAfterDelay()
    {
        yield return new WaitForSeconds(refreshDelay);
        NewZone();
    }

    void NewZone()
    {
        float newY = Random.Range(1.5f, 3.5f);
        Vector3 pos = targetZone.position;
        targetZone.position = new Vector3(pos.x, newY, pos.z);

        stayTimer = 0f;
        zoneTimer = 0f;
        targetZone.gameObject.SetActive(true);
        zoneActive = true;

        zoneColorController?.SetDefaultColor();
    }

    public void SetBall(Transform newBall)
    {
        ball = newBall;
    }

    void ShowComboText(int count)
    {
        if (comboText == null || count < 2) return;

        comboText.text = $"Combo x{count}";
        comboText.gameObject.SetActive(true);

        if (comboAnimRoutine != null)
            StopCoroutine(comboAnimRoutine);

        comboAnimRoutine = StartCoroutine(AnimateCombo());
    }

    IEnumerator AnimateCombo()
    {
        RectTransform rt = comboText.GetComponent<RectTransform>();

        rt.localScale = Vector3.zero;
        comboText.color = Color.white;
        comboText.gameObject.SetActive(true);

        rt.DOPunchScale(Vector3.one * 1.2f, 0.3f, 6, 1f);
        rt.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
        comboText.DOFade(0f, 0.5f).SetDelay(0.6f);

        yield return new WaitForSeconds(1.1f);

        comboText.color = Color.white;
        comboText.gameObject.SetActive(false);
    }

    void HideComboText()
    {
        comboText.gameObject.SetActive(false);
    }
}
