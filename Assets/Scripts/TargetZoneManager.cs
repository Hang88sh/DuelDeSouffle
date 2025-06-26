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
    //public GameObject comboTrailPrefab;
    public ZoneColorController zoneColorController;

    private bool isInsideZone = false;
    private float toleranceTime = 0.3f;
    private float graceTimer = 0f;



    void Start()
    {
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
        yield return AnimateCountdown("3");
        yield return AnimateCountdown("2");
        yield return AnimateCountdown("1");
        yield return AnimateCountdown("Commence !");
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

        // --- Vérifie si le centre de la boule est dans la zone ---
        bool ballInside = zoneCollider.bounds.Contains(ball.position);

        if (ballInside)
        {
            // Si la boule vient d’entrer dans la zone
            if (!isInsideZone)
            {
                isInsideZone = true;
                graceTimer = 0f;
            }

            // La boule est dans la zone, on cumule le temps
            stayTimer += Time.deltaTime;
            graceTimer = 0f;

            if (zoneColorController != null)
                zoneColorController.SetSuccessColor();

            // Si la durée est suffisante → succès
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
                // La boule vient de sortir → commence le temps de tolérance
                graceTimer += Time.deltaTime;

                if (graceTimer > toleranceTime)
                {
                    // Si trop longtemps dehors, on réinitialise tout
                    isInsideZone = false;
                    stayTimer = 0f;
                    graceTimer = 0f;

                    if (zoneColorController != null)
                        zoneColorController.SetDefaultColor();
                }
            }
            else
            {
                // Déjà hors de la zone → couleur par défaut
                stayTimer = 0f;
                if (zoneColorController != null)
                    zoneColorController.SetDefaultColor();
            }
        }

        // --- Fin du temps de zone ---
        if (zoneTimer >= zoneDuration)
        {
            EndZone();
        }
    }

    //void OnTriggerStay(Collider other)
    //{
    //    if (!zoneActive || !gameStarted) return;

    //    if (other.CompareTag("Ball"))
    //    {
    //        stayTimer += Time.deltaTime;

    //        if (stayTimer >= requiredStayTime)
    //        {
    //            AddScore();
    //            EndZone();
    //        }
    //    }
    //}

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

        if (zoneColorController != null)// Réinitialiser la couleur à vert au début de la nouvelle zone
            zoneColorController.SetDefaultColor();
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

    // Anime le texte combo : secousse + disparition après 1 seconde
    IEnumerator AnimateCombo()
    {
        RectTransform rt = comboText.GetComponent<RectTransform>();

        // Réinitialise l’échelle et la couleur
        rt.localScale = Vector3.zero;
        comboText.color = Color.white;
        comboText.gameObject.SetActive(true);

        // Effet de punch (secousse rapide)
        rt.DOPunchScale(Vector3.one * 1.2f, 0.3f, 6, 1f);

        // Monte doucement à l’échelle normale en parallèle
        rt.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);

        // Ajoute un fade progressif (facultatif si tu veux que ça fonde)
        comboText.DOFade(0f, 0.5f).SetDelay(0.6f);

        // Attends 1 seconde entière avant de désactiver
        yield return new WaitForSeconds(1.1f);

        comboText.color = Color.white;
        comboText.gameObject.SetActive(false);
    }






    void HideComboText()
    {
        comboText.gameObject.SetActive(false);
    }
}
