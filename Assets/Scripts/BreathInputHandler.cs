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

    [Header("Interface utilisateur")]
    public Slider breathSlider;


    void Start()
    {
        var input = GetComponent<PlayerInput>();
        //Debug.Log($"{gameObject.name} 当前输入 Map：{input.currentActionMap?.name}");
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
        //mise a jour de la force de souffle
        if (isBlowing)
        {
             breathStrength = Mathf.MoveTowards(breathStrength, 1f, Time.deltaTime * increaseSpeed); 
        }
        else
        {
            breathStrength = Mathf.MoveTowards(breathStrength, 0f, Time.deltaTime * decreaseSpeed);
        }

        //limiter entre 0 et 1
        breathStrength = Mathf.Clamp01(breathStrength);

        //appliquer la force vers le haut
        if(ballRb != null)
        {
            ballRb.AddForce(Vector3.up * breathStrength * maxUpForce, ForceMode.Force);
        }

        //mettre a jour a l'intyerface
        if(breathSlider != null)
        {
            breathSlider.value = breathStrength;
        }

        Debug.Log($"{gameObject.name} | isBlowing: {isBlowing} | breathStrength: {breathStrength}");          
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
