using UnityEngine;
using TMPro;

public class JudgementLine : MonoBehaviour
{
    public TextMeshProUGUI scoreText;    // Texte pour le score
    public TextMeshProUGUI comboText;    // Texte pour le combo

    private Transform ball;              // Balle actuelle
    private Note currentNote;            // Note en cours de jugement
    private float stayTime = 0f;         // Temps passé sur la ligne
    private int score = 0;
    private int combo = 0;

    public void SetBall(Transform newBall)
    {
        ball = newBall;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Note"))
        {
            currentNote = other.GetComponent<Note>();
            stayTime = 0f;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Note") && currentNote != null)
        {
            stayTime += Time.deltaTime;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Note") && currentNote != null)
        {
            if (stayTime >= currentNote.Duration)
            {
                Debug.Log("Parfait !");
                AddScore();
            }
            else
            {
                Debug.Log("Échec !");
                ResetCombo();
            }

            currentNote = null;
            stayTime = 0f;
        }
    }

    void AddScore()
    {
        score += 1;
        combo += 1;

        if (scoreText != null)
            scoreText.text = "Score : " + score;

        if (combo >= 2 && comboText != null)
        {
            comboText.gameObject.SetActive(true);
            comboText.text = $"Combo x{combo}";
        }
    }

    void ResetCombo()
    {
        combo = 0;
        if (comboText != null)
            comboText.gameObject.SetActive(false);
    }
}
