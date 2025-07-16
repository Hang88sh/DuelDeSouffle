using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public GameObject notePrefab; // Le prefab de la note ¨¤ g¨¦n¨¦rer
    public float speed = 5f;      // Vitesse de d¨¦placement de la note
    public float spawnX = 10f;    // Position X de g¨¦n¨¦ration (¨¤ droite de l¡¯¨¦cran)

    /// <summary>
    /// G¨¦n¨¦rer une note en fonction des donn¨¦es du chart
    /// </summary>
    public void SpawnNote(NoteData data)
    {
        // Instancier une nouvelle note
        GameObject noteObj = Instantiate(notePrefab);

        // D¨¦finir la position (selon la hauteur d¨¦finie dans le chart)
        noteObj.transform.position = new Vector3(spawnX, data.height, 2.5f);

        // Initialiser la longueur et la vitesse de la note
        Note note = noteObj.GetComponent<Note>();
        note.Init(data.duration, speed);
    }
}

