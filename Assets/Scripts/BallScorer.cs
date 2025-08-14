using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BallScorer : MonoBehaviour
{
    public TextMeshProUGUI scoreText;             // Texte du score
    public TextMeshProUGUI comboText;             // Texte du combo

    public AudioSource audioSource;               // Source audio pour les effets
    public AudioClip noteHitSFX;                  // Effet sonore à jouer lors d’un hit

    private int score = 0;                        // Score actuel
    private int combo = 0;                        // Combo actuel

    private float comboResetDelay = 1.5f;         // Temps max sans hit avant reset du combo
    private float lastHitTime = -999f;            // Temps du dernier hit valide

    private HashSet<GameObject> notesComptées = new HashSet<GameObject>(); // Notes déjà comptées

    void Start()
    {
        // Recherche automatique du texte du score
        if (scoreText == null)
        {
            GameObject scoreObj = GameObject.Find("ScoreText");
            if (scoreObj != null)
                scoreText = scoreObj.GetComponent<TextMeshProUGUI>();
        }

        // Recherche automatique du texte du combo
        if (comboText == null)
        {
            GameObject comboObj = GameObject.Find("ComboText");
            if (comboObj != null)
                comboText = comboObj.GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        // Si un combo est actif et qu’on dépasse la durée limite sans hit, on reset
        if (combo > 0 && Time.time - lastHitTime > comboResetDelay)
        {
            ResetCombo();
        }
    }

    // Déclenché lors d’une collision avec une note
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Note"))
        {
            GameObject rootNote = other.transform.root.gameObject;
            if (!notesComptées.Contains(rootNote))
            {
                Debug.Log("Note touchée : " + rootNote.name);

                if (audioSource != null && noteHitSFX != null)
                    audioSource.PlayOneShot(noteHitSFX);

                AddScore();
                notesComptées.Add(rootNote);
            }
        }
    }

    // Ajout du score et du combo
    void AddScore()
    {
        score += 1;
        combo += 1;
        lastHitTime = Time.time; // Met à jour le temps du dernier hit

        if (scoreText != null)
            scoreText.text = "Score : " + score;

        if (combo >= 2 && comboText != null)
        {
            comboText.gameObject.SetActive(true);
            comboText.text = $"Combo x{combo}";
        }
    }

    // Réinitialisation du combo
    public void ResetCombo()
    {
        combo = 0;

        if (comboText != null)
            comboText.gameObject.SetActive(false);
    }

    // Accès au score final
    public int GetScore()
    {
        return score;
    }
}
