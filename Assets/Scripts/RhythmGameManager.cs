using System.Collections;
using UnityEngine;
using TMPro;

public class RhythmGameManager : MonoBehaviour
{
    [Header("R¨¦f¨¦rences")]
    public BallSpawner_Rhythm ballSpawner;        // G¨¦n¨¦rateur de balle pour le mode rythme
    public MusicTimelineManager musicTimeline;    // Gestionnaire du timeline musical
    public TextMeshProUGUI countdownText;         // Texte pour le compte ¨¤ rebours

    void Start()
    {
        if (LocalizationManager.Instance == null)
        {
            GameObject loc = Instantiate(Resources.Load<GameObject>("LocalizationManager"));
            loc.name = "LocalizationManager";             
        }
        StartCoroutine(StartCountdownRoutine());
    }

    IEnumerator StartCountdownRoutine()
    {
        // Affiche le texte de compte ¨¤ rebours
        countdownText.gameObject.SetActive(true);

        // R¨¦cup¨¨re les traductions depuis le gestionnaire de localisation
        string text3 = LocalizationManager.Instance.GetText("countdown_3");
        string text2 = LocalizationManager.Instance.GetText("countdown_2");
        string text1 = LocalizationManager.Instance.GetText("countdown_1");
        string startText = LocalizationManager.Instance.GetText("start");

        // Lance les animations une par une
        yield return AnimateCountdown(text3);
        yield return AnimateCountdown(text2);
        yield return AnimateCountdown(text1);
        yield return AnimateCountdown(startText);

        // Cache le texte apr¨¨s le d¨¦but du jeu
        countdownText.gameObject.SetActive(false);

        // G¨¦n¨¨re la balle
        if (ballSpawner != null)
        {
            ballSpawner.SpawnNewBall();
        }

        // D¨¦marre le timeline musical
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

        // Animation : agrandissement
        while (t < duration)
        {
            float scale = Mathf.SmoothStep(0.5f, 1.5f, t / duration);
            countdownText.transform.localScale = Vector3.one * scale;
            t += Time.deltaTime;
            yield return null;
        }

        countdownText.transform.localScale = Vector3.one * 1.5f;
        yield return new WaitForSeconds(0.2f);

        // Animation : retour ¨¤ l'¨¦chelle normale
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
