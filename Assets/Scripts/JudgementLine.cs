using UnityEngine;
using TMPro;

public class JudgementLine : MonoBehaviour
{
    public TextMeshProUGUI scoreText;    // Texte du score
    public TextMeshProUGUI comboText;    // Texte du combo

    private Transform ball;              // Référence à la balle actuelle
    private Note currentNote;            // Note actuellement jugée
    private float stayTime = 0f;         // Temps passé dans la zone de jugement
    private int score = 0;               // Score total
    private int combo = 0;               // Combo actuel

    private bool noteHit = false;        // Si la note a été touchée par la balle

    public void SetBall(Transform newBall)
    {
        ball = newBall;
    }

    void Update()
    {
        if (ball == null || currentNote == null || noteHit) return;

        // --- Vérifie si la balle est proche de la note ---
        float distance = Vector3.Distance(ball.position, currentNote.transform.position);

        // Rayon de jugement (ajuster selon la taille réelle)
        float hitRange = 0.6f;

        if (distance < hitRange)
        {
            noteHit = true;
            Debug.Log("Note touchée !");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Note"))
        {
            currentNote = other.GetComponent<Note>();
            stayTime = 0f;
            noteHit = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Note") && ball != null)
        {
            float verticalDistance = Mathf.Abs(ball.position.y - other.transform.position.y);
            float hitThreshold = 0.3f; // Seuil de tolérance en hauteur

            if (verticalDistance < hitThreshold)
            {
                Debug.Log("Parfait !");

                // Ne pas détruire la note, mais empêcher le double jugement
                other.tag = "Untagged"; // évite les jugements futurs

                AddScore();
            }
        }
    }




    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Note") && currentNote != null)
        {
            // --- Si la balle a touché la note et que le temps est suffisant ---
            if (noteHit && stayTime >= currentNote.Duration)
            {
                Debug.Log("Parfait !");
                AddScore();
            }
            else
            {
                Debug.Log("Échec !");
                ResetCombo();
            }

            // Réinitialisation
            currentNote = null;
            stayTime = 0f;
            noteHit = false;
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
