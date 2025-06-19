using UnityEngine;

public class BackgroundElementMotion : MonoBehaviour
{
    public enum MotionType { Cloud, Tree }
    public MotionType motionType = MotionType.Cloud;

    public float amplitude = 0.5f; // Amplitude du mouvement (distance de va-et-vient)
    public float speed = 1f;       // Vitesse du mouvement
    private Vector3 initialPosition;

    void Start()
    {
        // On m¨¦morise la position d¡¯origine
        initialPosition = transform.position;
    }

    void Update()
    {
        // Calcul du d¨¦calage avec un sinus
        float offset = Mathf.Sin(Time.time * speed) * amplitude;

        switch (motionType)
        {
            case MotionType.Cloud:
                // Mouvement fluide gauche-droite pour les nuages
                transform.position = initialPosition + new Vector3(offset, 0f, 0f);
                break;

            case MotionType.Tree:
                // Petit balancement plus doux pour les arbres
                transform.position = initialPosition + new Vector3(offset * 0.3f, 0f, 0f);
                break;
        }
    }
}
