using UnityEngine;

[ExecuteAlways]//le script s'execute tout le temps
public class MassScaler : MonoBehaviour
{
    public Rigidbody rb;

    [Header("Parametres de configuration")]
    

    public float masse = 1f;//Masse par defaut
    public float facteurEchelle = 0.5f;//Mutiplicateur de taille selon la masse
    public float masseMin = 0.1f;
    public float masseMax = 10f;

    // Update is called once per frame
    void Update()
    {
        if (rb == null) return;

        
        //limiter la masse dans les bornes definies
        float masseLimitee = Mathf.Clamp(masse, masseMin, masseMax);

        //Appliquer la masse au Rigidbody
        rb.mass = masseLimitee;

        //Adapter la taille de l'objet en fonction de la masse
        float echelle=Mathf.Max(0.1f, masseLimitee*facteurEchelle);

        transform.localScale = new Vector3(echelle, echelle, echelle);
        
    }

    private void OnValidate()
    {
        //Empecher la masse de depasser les bornes meme en mode edition
        masse=Mathf.Clamp(masse,masseMin, masseMax);
    }
}
