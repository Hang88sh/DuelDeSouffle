using System;
using System.Collections.Generic;

// Donn¨¦es d'une seule note
[Serializable]
public class NoteData
{
    public float time;      // Temps d'apparition de la note (en secondes)
    public float duration;  // Dur¨¦e de la note (0 pour une note courte)
    public int lane;        // Num¨¦ro de la ligne ou de la piste (0 si une seule piste)
}

// Donn¨¦es compl¨¨tes du chart (partition enti¨¨re)
[Serializable]
public class ChartData
{
    public string musicName;            // Nom du fichier audio ou de la musique
    public List<NoteData> notes = new List<NoteData>();  // Liste de toutes les notes
}
