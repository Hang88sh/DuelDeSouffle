using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public GameObject notePrefab;        // Le prefab de la note à générer
    public float tempsTrajet = 2f;       // Temps de trajet pour atteindre la ligne de jugement (en secondes)
    public float defaultZ = 2.5f;        // Profondeur Z des notes
    public int laneCount = 3;            // Nombre total de lanes (3 par défaut)

    private float baseY;                 // Position Y de la première lane (la plus basse)
    private float laneHeight;            // Distance verticale entre chaque lane
    private float spawnX;                // Position X de génération (bord droit de l'écran)
    private float positionJugementX;     // Position X de la ligne de jugement

    void Awake()
    {
        Camera cam = Camera.main;

        // Définir la position X de la ligne de jugement (basé sur l'image fournie)
        positionJugementX = 1.5f;

        // Calcul automatique de la position X du bord droit de l'écran
        float largeurCamera = cam.orthographicSize * cam.aspect;
        spawnX = cam.transform.position.x + largeurCamera + 1f; // Décalage supplémentaire pour plus de fluidité

        // Définir la plage verticale des lanes
        float minY = 1f;       // Juste au-dessus des arbres
        float maxY = 4.5f;     // Juste en dessous des nuages

        // Calcul de l'espacement vertical entre les lanes
        laneHeight = (maxY - minY) / (laneCount - 1);
        baseY = minY;
    }

    public void SpawnNote(NoteData data)
    {
        GameObject noteObj = Instantiate(notePrefab);

        // Calcul de la position verticale selon la lane
        float yPos = baseY + (data.lane * laneHeight);

        // Position initiale de la note (hors de l'écran à droite)
        noteObj.transform.position = new Vector3(spawnX, yPos, defaultZ);

        // Calcul de la vitesse pour arriver à temps à la ligne de jugement
        float distance = spawnX - positionJugementX;
        float vitesse = distance / tempsTrajet;

        // Initialisation de la note avec durée et vitesse
        Note note = noteObj.GetComponent<Note>();
        note.Init(data.duration, vitesse);

        // Recalibrage de la position selon la taille de la note (collider centré)
        BoxCollider box = noteObj.GetComponent<BoxCollider>();
        if (box != null)
        {
            float largeurFinale = box.size.x * noteObj.transform.lossyScale.x;
            noteObj.transform.position = new Vector3(spawnX + largeurFinale / 2f, yPos, defaultZ);
        }
    }
}
