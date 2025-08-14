using UnityEngine;

// Ce script fait tourner continuellement la note
public class NoteSpin : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0f, 90f, 0f); // Ã¿Ãë90¶ÈÈÆYÖáÐý×ª

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
