using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class BreathInputHandler : MonoBehaviour
{
    [Header("Controle Physique")]
    public Rigidbody ballRb; //reference a la boule a faire monter
    public float maxUpForce = 50f; //Force maximale appliquee vers le haut

    [Header("Parametres de respiration")]
    public float increaseSpeed = 1f;
    public float decreaseSpeed = 1f;
    private float breathStrength = 0f;//Force actuelle
    private bool isBlowing=false;//État : souffler ou non

    public PersistentBreathText persistentText;
    private string currentPhase = "";
    private bool hasActivatedText = false;

    [Header("Interface utilisateur")]
    public Slider breathSlider;


    void Start()
    {
        var input = GetComponent<PlayerInput>();
        //Debug.Log($"{gameObject.name}  Map：{input.currentActionMap?.name}");

        if (persistentText == null)
        {
            persistentText = FindFirstObjectByType<PersistentBreathText>();
            if (persistentText != null)
                Debug.Log($"[AutoBind] Texte lié automatiquement : {persistentText.name}");
            else
                Debug.LogWarning("Aucun objet PersistentBreathText trouvé dans la scène !");
        }
        if (persistentText != null)
            persistentText.gameObject.SetActive(false);
    }

    public void OnBlow(InputAction.CallbackContext context)//appele automatiquement par PlayerInput
    {
        //Debug.Log($"[OnBlow] {gameObject.name} phase: {context.phase}");
        if (context.performed)
        {
            isBlowing = true;
        }
        else if(context.canceled) 
        {
            isBlowing = false;
        }
    }

    // Update is called once per frame
    void Update()    
    {
        // Mise à jour de la force simulée selon l'entrée souris/clavier
        if (isBlowing)
        {
            float simulatedValue = Mathf.MoveTowards(breathStrength * 10f + 10f, 20f, Time.deltaTime * increaseSpeed * 10f);
            breathStrength = Mathf.InverseLerp(10f, 20f, simulatedValue);
        }
        else
        {
            float simulatedValue = Mathf.MoveTowards(breathStrength * 10f + 10f, 10f, Time.deltaTime * decreaseSpeed * 10f);
            breathStrength = Mathf.InverseLerp(10f, 20f, simulatedValue);
        }

        breathStrength = Mathf.Clamp01(breathStrength); // Clamp entre 0 et 1

        // Calcul de la force de montée
        if (ballRb != null)
        {
            float g = Physics.gravity.magnitude; // ~9.81
            float mass = ballRb.mass;
            float liftForce = mass * g;

            float inputValue = Mathf.Lerp(10f, 20f, breathStrength);
            float appliedForce = 0f;

            if (isBlowing)
            {
                if (inputValue < 10f)
                {
                    float factor = Mathf.InverseLerp(0f, 10f, inputValue);
                    appliedForce = Mathf.Lerp(0f, liftForce * 0.8f, factor);
                    Debug.Log("Souffle insuffisant.");
                }
                else if (inputValue <= 20f)
                {
                    appliedForce = liftForce;
                    Debug.Log("Souffle optimal !");
                }
                else
                {
                    float factor = Mathf.InverseLerp(20f, 30f, inputValue);
                    appliedForce = Mathf.Lerp(liftForce, liftForce * 2f, factor);
                    Debug.Log("Souffle trop fort !");
                }

                ballRb.AddForce(Vector3.up * appliedForce, ForceMode.Force);
            }
        }

        if (breathSlider != null)
        {
            breathSlider.value = breathStrength;
        }

        if (!hasActivatedText && isBlowing && persistentText != null)//Affichage une seule fois après la première activation
        {
            persistentText.gameObject.SetActive(true);
            hasActivatedText = true;
        }

        //Toujours détecter la phase même après arrêt du souffle
        if (persistentText != null && hasActivatedText)
        {
            string newPhase = "";

            if (breathStrength < 0.3f)
                newPhase = "low";
            else if (breathStrength > 0.8f)
                newPhase = "high";
            else
                newPhase = "normal";

            if (newPhase != currentPhase)
            {
                currentPhase = newPhase;

                switch (newPhase)
                {
                    case "low":
                        persistentText.SetText("Allez!!", Color.yellow, -15f);
                        break;
                    case "high":
                        persistentText.SetText("Trop Fort!!", Color.red, 20f);
                        break;
                    case "normal":
                        persistentText.SetText("Parfait!!", Color.green, 0f);
                        break;
                }
            }
        }
    }

    public void OnMessageArrived(string msg)
    {
        if (float.TryParse(msg, out float value) && value > 0)
        {
            if (value < 10f)
            {
                // too weak
                isBlowing = false;
                Debug.Log("Souffle trop faible");
            }
            else if (value > 20f)
            {
                // too strong
                isBlowing = false;
                Debug.Log("Souffle trop fort");
            }
            else
            {
                // valid range: set isBlowing to true and normalize the value
                isBlowing = true;
                breathStrength = Mathf.InverseLerp(10f, 20f, value); // maps 10–20 to 0–1
                Debug.Log($"Souffle valide: {value} → strength = {breathStrength:F2}");
            }
        }
        else
        {
            isBlowing = false;
            Debug.LogWarning($"Valeur invalide ou nulle reçue: '{msg}'");
        }
    }

    public void OnConnectionEvent(bool success)
    {
        Debug.Log(success ? "Device connected" : "Device disconnected");
    }
}
