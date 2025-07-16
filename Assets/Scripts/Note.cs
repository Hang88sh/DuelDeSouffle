using UnityEngine;

public class Note : MonoBehaviour
{
    private float speed;         // Vitesse de d¨¦placement de la note
    private float duration;      // Dur¨¦e pendant laquelle la note doit ¨ºtre maintenue

    public float Duration => duration;  // Propri¨¦t¨¦ publique pour acc¨¦der ¨¤ la dur¨¦e

    // M¨¦thode d'initialisation, appel¨¦e avec les donn¨¦es du chart
    public void Init(float duration, float speed)
    {
        this.duration = duration;
        this.speed = speed;

        // Ajuster la longueur de la note en fonction de sa dur¨¦e
        Vector3 scale = transform.localScale;
        scale.x = duration * speed; // Plus la vitesse est rapide, plus la note est longue
        transform.localScale = scale;
    }

    void Update()
    {
        // D¨¦placer continuellement la note vers la gauche
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // D¨¦truire automatiquement l'objet lorsqu'il sort de l'¨¦cran (¨¦viter les fuites de m¨¦moire)
        if (transform.position.x < -15f) // Marge sur le bord gauche
        {
            Destroy(gameObject);
        }
    }
}
