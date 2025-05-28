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

    private BreathControls controls;

    private void Awake()
    {
        controls= new BreathControls();

        //commence a souffler quand la touche est presse

        controls.Player.Blow.started += ctx => isBlowing = true;

        //arrete de souffler quand la touche est relache

        controls.Player.Blow.canceled += ctx => isBlowing = false;
    }

    private void OnEnable()
    {
        controls.Enable();//activite les controles
    }

    private void OnDisable()
    {
         controls.Disable();//desactive
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

        Debug.Log("呼吸强度: " + breathStrength);
    }
}
