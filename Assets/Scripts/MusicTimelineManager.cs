using UnityEngine;
using System.Collections.Generic;

public class MusicTimelineManager : MonoBehaviour
{
    public AudioSource musicSource;      // Lecteur audio
    public TextAsset chartJSON;          // Fichier JSON du chart (export¨¦ par ChartEditor)
    public NoteSpawner noteSpawner;      // G¨¦n¨¦rateur des notes

    private List<NoteData> notes;        // Liste des notes charg¨¦es
    private float startTime;
    private int nextNoteIndex = 0;
    private bool isPlaying = false;      // Indique si le timeline est en cours

    // D¨¦marrer le timeline (appel¨¦ par RhythmGameManager ou manuellement)
    public void StartTimeline()
    {
        // Charger le chart complet depuis le JSON
        ChartData chart = JsonUtility.FromJson<ChartData>(chartJSON.text);
        notes = chart.notes;

        startTime = Time.time;
        nextNoteIndex = 0;
        isPlaying = true;

        if (musicSource != null)
        {
            musicSource.Play();
        }

        Debug.Log("Timeline d¨¦marr¨¦, nombre total de notes : " + notes.Count);
    }

    void Update()
    {
        if (!isPlaying) return;

        float currentTime = Time.time - startTime;

        // V¨¦rifie si une nouvelle note doit ¨ºtre g¨¦n¨¦r¨¦e
        if (nextNoteIndex < notes.Count && currentTime >= notes[nextNoteIndex].time)
        {
            noteSpawner.SpawnNote(notes[nextNoteIndex]);
            nextNoteIndex++;
        }
    }
}
