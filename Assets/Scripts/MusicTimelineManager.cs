using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class NoteData
{
    public float time;      // Temps d¡¯apparition (en secondes)
    public float duration;  // Dur¨¦e de maintien
    public float height;    // Position Y de la note
}

[System.Serializable]
public class NoteDataList
{
    public List<NoteData> notes;  // Liste compl¨¨te des notes
}

public class MusicTimelineManager : MonoBehaviour
{
    public AudioSource musicSource;     // Lecteur audio
    public TextAsset chartJSON;         // Fichier JSON du chart
    public NoteSpawner noteSpawner;     // G¨¦n¨¦rateur des notes

    private List<NoteData> notes;
    private float startTime;
    private int nextNoteIndex = 0;
    private bool isPlaying = false;     // Indique si le timeline est actif

    /// <summary>
    /// D¨¦marrer manuellement le timeline (appel¨¦ par RhythmGameManager)
    /// </summary>
    public void StartTimeline()
    {
        notes = JsonUtility.FromJson<NoteDataList>("{\"notes\":" + chartJSON.text + "}").notes;

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

        if (nextNoteIndex < notes.Count && currentTime >= notes[nextNoteIndex].time)
        {
            noteSpawner.SpawnNote(notes[nextNoteIndex]);
            nextNoteIndex++;
        }
    }
}
