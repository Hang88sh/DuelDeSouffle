using System.Collections;
using UnityEngine;
using TMPro;

public class RhythmGameManager : MonoBehaviour
{
    [Header("R¨¦f¨¦rences")]
    public BallSpawner_Rhythm ballSpawner;      // G¨¦n¨¦rateur de balle pour le mode rythme
    public MusicTimelineManager musicTimeline;  // Gestionnaire du timeline musical
    public TextMeshProUGUI countdownText;       // Texte pour le compte ¨¤ rebours

    void Start()
    {
        StartCoroutine(StartCountdownRoutine());
    }

    IEnumerator StartCountdownRoutine()
    {
        countdownText.gameObject.SetActive(true);

        yield return AnimateCountdown("3");
        yield return AnimateCountdown("2");
        yield return AnimateCountdown("1");
        yield return AnimateCountdown("C'est parti !");

        countdownText.gameObject.SetActive(false);

        // G¨¦n¨¦rer la balle
        if (ballSpawner != null)
        {
            ballSpawner.SpawnNewBall();
        }

        // Lancer le timeline musical
        if (musicTimeline != null)
        {
            musicTimeline.StartTimeline();
        }
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
}
