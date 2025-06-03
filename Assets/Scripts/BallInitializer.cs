using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BallInitializer : MonoBehaviour
{
    [Header("Controle du souffle(UI)")]
    public Slider breathSlider;//Barre de souffle a l'ecran

    public void Initialize()
    {
        //recupere ou ajoute un rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb=gameObject.AddComponent<Rigidbody>();
        }

        //ajouter le script PlayerInput
        PlayerInput input = GetComponent<PlayerInput>();
        if(input==null)
        {
            input=gameObject.AddComponent<PlayerInput>();

        }
        input.actions = Resources.Load<InputActionAsset>("BreathControls");
        input.defaultActionMap = "Player";
        input.defaultControlScheme = "Keyboard1";
        
        BreathInputHandler handler=GetComponent<BreathInputHandler>();
        if(handler==null)
        {
            handler=gameObject.AddComponent<BreathInputHandler>();
        }

        handler.ballRb = rb;
        handler.maxUpForce = 50f;
        handler.increaseSpeed = 1f;
        handler.decreaseSpeed = 1f;

        if(breathSlider!=null)
        {
            handler.breathSlider = breathSlider;
        }
    }
}
