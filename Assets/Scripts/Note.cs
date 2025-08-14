using UnityEngine;

public class Note : MonoBehaviour
{
    private float speed;       // Vitesse de déplacement vers la gauche
    private float duration;    // Durée de la note (en secondes)

    public float Duration => duration;

    public GameObject visualPrefab; // Préfab visuel optionnel (cristal)

    public void Init(float duration, float speed)
    {
        this.duration = duration;
        this.speed = speed;

        // Assure que l'objet principal a le tag "Note" pour la détection de collision
        this.gameObject.tag = "Note";

        // Désactive l'affichage du cube gris d'origine (MeshRenderer du GameObject principal)
        MeshRenderer rend = GetComponent<MeshRenderer>();
        if (rend != null)
            rend.enabled = false;

        // Calcule la longueur visuelle en fonction de la durée
        float newLength = duration * speed;

        // Crée l'objet visuel comme enfant, avec l'échelle modifiée
        if (visualPrefab != null)
        {
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.transform.SetParent(transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.Euler(45f, 45f, 45f);
            visual.transform.localScale = new Vector3(newLength, 1f, 1f);

            
            Destroy(visual.GetComponent<Collider>());
        }
        else
        {
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.transform.SetParent(transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.Euler(45f, 45f, 45f);
            visual.transform.localScale = new Vector3(newLength, 1f, 1f);

            
            Destroy(visual.GetComponent<Collider>());

            Material crystalMat = Resources.Load<Material>("CrystalGreenMat");
            if (crystalMat != null)
                visual.GetComponent<Renderer>().material = crystalMat;

            visual.AddComponent<NoteSpin>();
        }
    }

    void Update()
    {
        // Déplacement constant vers la gauche
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // Détruire la note si elle sort de l'écran
        if (transform.position.x < -15f)
            Destroy(gameObject);
    }
}
