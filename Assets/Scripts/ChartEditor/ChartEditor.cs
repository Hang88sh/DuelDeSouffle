using UnityEngine;
using System.Collections.Generic;

public class ChartEditor : MonoBehaviour
{
    public AudioSource musicSource;           // Source audio utilis¨¦e pour lire la musique
    public ChartData currentChart = new ChartData(); // Stockage des notes enregistr¨¦es

    private Dictionary<int, NoteData> activeLongNotes = new Dictionary<int, NoteData>();

    void Update()
    {
        if (!musicSource.isPlaying) return;   // Si la musique n'est pas en cours, on ne fait rien

        float currentTime = musicSource.time; // Temps actuel de la musique

        // --- Trois lanes : A / S / D ---
        CheckKey(KeyCode.A, 0, currentTime);
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("A-0");
        }
        CheckKey(KeyCode.S, 1, currentTime);
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("S-1");
        }
        CheckKey(KeyCode.D, 2, currentTime);
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("D-2");
        }
    }

    private void CheckKey(KeyCode key, int lane, float currentTime)
    {
        // --- D¨¦but d'une note longue ---
        if (Input.GetKeyDown(key))
        {
            NoteData n = new NoteData
            {
                time = currentTime,
                duration = 0,
                lane = lane
            };
            activeLongNotes[lane] = n;
        }

        // --- Fin d'une note longue ---
        if (Input.GetKeyUp(key))
        {
            NoteData n;
            if (activeLongNotes.TryGetValue(lane, out n))
            {
                n.duration = currentTime - n.time;
                currentChart.notes.Add(n);
                activeLongNotes.Remove(lane);
            }
        }

        // --- Note courte (si juste un tap) ---
        if (Input.GetKeyDown(key) && !Input.GetKey(key))
        {
            currentChart.notes.Add(new NoteData
            {
                time = currentTime,
                duration = 0,
                lane = lane
            });
        }
    }
}
