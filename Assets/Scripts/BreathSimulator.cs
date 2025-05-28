using UnityEngine;
using UnityEngine.UI;

public class BreathSimulator : MonoBehaviour
{
    [Header("Simulation Parameters")]
    [Range(0f, 1f)]

    public float breathStrength = 0f;//Force de respiration actuelle (0 à 1)
    public float increaseSpeed = 1f;//Vitesse d'augmentation quand on appuie
    public float decreaseSpeed = 1f;

    [Header("Physics Control")]
    public Rigidbody ballrb;
    public float maxUpForce = 10f;//Force maximale vers le haut

    [Header("UI")]
    public Slider breatherSlider;


    
    void Update()
    {
        //// Augmente la force de respiration si la touche Espace est enfoncée
        if (Input.GetKey(KeyCode.Space))
        {
            breathStrength += Time.deltaTime * increaseSpeed;

        }
        else
        {
            //// Diminue la force quand la touche est relâchée
            breathStrength -= Time.deltaTime * decreaseSpeed;
        }

        //Limite la valeur entre 0 et 1
        breathStrength = Mathf.Clamp01(breathStrength);

        //Applique une force vers le haut sur la balle en fonction de lla respiration
        if (ballrb!=null)
        {
            ballrb.AddForce(Vector3.up * breathStrength * maxUpForce, ForceMode.Force);
        }

        //met a jour la barre de force dans l'interface
        if (breatherSlider != null)
        {
            breatherSlider.value = breathStrength;
        }

        Debug.Log("呼吸强度: " + breathStrength);
    }
}
