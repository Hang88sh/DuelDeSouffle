using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public GameObject notePrefab;   // Le prefab de la note à générer
    public float speed = 5f;        // Vitesse de déplacement des notes
    public float defaultZ = 2.5f;   // Profondeur Z des notes
    public int laneCount = 3;       // Nombre total de lanes (3 par défaut)

    private float baseY;            // Y de la première lane (la plus basse)
    private float laneHeight;       // Distance verticale entre chaque lane
    private float spawnX;           // Position X calculée automatiquement (bord droit de l’écran)

    void Awake()
    {
        Camera cam = Camera.main;

        // Calcul automatique du bord droit de l'écran
        float camWidth = cam.orthographicSize * cam.aspect;
        spawnX = cam.transform.position.x + camWidth;

        // Définir la zone verticale des lanes (adapter à ta scène)
        float minY = 1f;  // Juste au-dessus des arbres
        float maxY = 4.5f;    // Juste en dessous des nuages

        // Calcul de l'espacement entre lanes
        laneHeight = (maxY - minY) / (laneCount - 1);
        baseY = minY;
    }

    public void SpawnNote(NoteData data)
    {
        GameObject noteObj = Instantiate(notePrefab);

        // Calculer la position verticale (lane → Y)
        float yPos = baseY + (data.lane * laneHeight);

        // D'abord placer la note au bord droit, avant l'initialisation
        noteObj.transform.position = new Vector3(spawnX, yPos, defaultZ);

        // Initialisation (peut modifier la taille/longueur de la note)
        Note note = noteObj.GetComponent<Note>();
        note.Init(data.duration, speed);

        // Recalculer la largeur finale après Init()
        BoxCollider box = noteObj.GetComponent<BoxCollider>();
        if (box != null)
        {
            float finalWidth = box.size.x * noteObj.transform.lossyScale.x;
            noteObj.transform.position = new Vector3(spawnX + finalWidth / 2f, yPos, defaultZ);
        }
    }
}
