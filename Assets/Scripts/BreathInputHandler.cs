using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class BreathInputHandler : MonoBehaviour
{
    [Header("Physique")]
    public Rigidbody ballRb;                   // Rigidbody de la balle
    public float maxUpVelocity = 20f;          // Vitesse maximale vers le haut
    public float activationDelay = 0.5f;       // Délai d’activation après l’apparition

    [Header("Interface utilisateur")]
    public ImgsFillDynamic roundGauge;         // Jauge circulaire de souffle
    public PersistentBreathText persistentText;// Texte persistant ("trop fort", etc.)

    [Header("Entrée simulée")]
    public float increaseSpeed = 1f;           // Vitesse d’augmentation du souffle
    public float decreaseSpeed = 1f;           // Vitesse de diminution du souffle

    private float breathStrength = 0f;         // Force de souffle lissée (0–1)
    private float smoothedBreath = 0f;         // Souffle lissé (pour affichage/UI)
    private float lastRawBreathValue = 0f;     // Dernière valeur brute reçue du capteur
    private bool isBlowing = false;            // Indique si le joueur souffle actuellement

    private float timeSinceSpawn = 0f;         // Temps écoulé depuis la génération
    private float stopGracePeriod = 0.5f;      // Période de grâce sans souffle
    private float timeSinceLastValidInput = 999f;

    private string lastPhase = "";
    private string keyTooWeak = "too_weak";
    private string keyTooStrong = "too_strong";
    private string keyPerfect = "perfect";

    void Start()
    {
        isBlowing = false;
        breathStrength = 0f;
        smoothedBreath = 0f;
        timeSinceSpawn = 0f;
        timeSinceLastValidInput = 999f;

        Physics.gravity = new Vector3(0f, -6.5f, 0f);

        if (ballRb != null)
        {
            ballRb.linearDamping = 1.5f; 
        }

        if (persistentText != null)
            persistentText.gameObject.SetActive(false);
    }

    void Update()
    {
        timeSinceSpawn += Time.deltaTime;

        UpdateSmoothedBreath();       // Lissage du souffle
        UpdateBreathStatus();         // Statut de souffle (actif ou non)
        ApplyBreathVelocity();        // Appliquer la force à la balle
        UpdateBreathZoneText();       // Affichage du texte indicatif
        UpdateUI();                   // Mise à jour de la jauge
    }

    void UpdateSmoothedBreath()
    {
        float speed = (breathStrength > smoothedBreath) ? increaseSpeed : decreaseSpeed;
        smoothedBreath = Mathf.MoveTowards(smoothedBreath, breathStrength, Time.deltaTime * speed);
    }

    void UpdateBreathStatus()
    {
        if (isBlowing)
            timeSinceLastValidInput = 0f;
        else
            timeSinceLastValidInput += Time.deltaTime;
    }

    void ApplyBreathVelocity()
    {
        if (ballRb == null || timeSinceSpawn < activationDelay) return;

        bool isStillBreathing = timeSinceLastValidInput < stopGracePeriod;
        if (!isStillBreathing || breathStrength <= 0f) return;

        // --- Paramètres de force ---
        float forcePower = 8f;            // Multiplicateur de force principale
        float minForce = 1.5f;            // Force minimale (permet de faire bouger la balle même avec un petit souffle)
        float currentY = ballRb.linearVelocity.y;

        if (currentY >= maxUpVelocity) return; // Ne pas dépasser la vitesse max

        // --- Calcul de la force vers le haut ---
        float upwardForce = breathStrength * forcePower + minForce;
        ballRb.AddForce(Vector3.up * upwardForce, ForceMode.Force);
    }

    void UpdateBreathZoneText()
    {
        if (persistentText == null) return;

        bool isStillBreathing = timeSinceLastValidInput < stopGracePeriod;

        if (!isStillBreathing || smoothedBreath < 0.05f)
        {
            if (persistentText.gameObject.activeSelf)
            {
                persistentText.gameObject.SetActive(false);
                lastPhase = "";
            }
            return;
        }

        string phase = "";
        Color color = Color.white;
        float offset = 0f;

        if (smoothedBreath < 0.3f)
        {
            phase = LocalizationManager.Instance.GetText(keyTooWeak);
            color = Color.yellow;
            offset = -15f;
        }
        else if (smoothedBreath > 0.8f)
        {
            phase = LocalizationManager.Instance.GetText(keyTooStrong);
            color = Color.red;
            offset = 20f;
        }
        else
        {
            phase = LocalizationManager.Instance.GetText(keyPerfect);
            color = Color.green;
            offset = 0f;
        }

        if (phase != lastPhase)
        {
            persistentText.gameObject.SetActive(true);
            persistentText.SetText(phase, color, offset);
            lastPhase = phase;
        }
    }

    void UpdateUI()
    {
        if (roundGauge == null) return;

        // Affiche la valeur brute même hors zone de souffle valide
        float uiDisplay = Mathf.InverseLerp(5f, 25f, lastRawBreathValue);
        roundGauge.SetValue(uiDisplay, true);
    }

    // --- Ce bloc NE DOIT PAS être modifié (donné par le dispositif) ---
    public void OnMessageArrived(string msg)
    {
        if (float.TryParse(msg, out float value) && value > 0)
        {
            lastRawBreathValue = value;

            if (value < 10f)
            {
                isBlowing = false;
                //Debug.Log("Souffle trop faible");
            }
            else if (value > 20f)
            {
                isBlowing = false;
                //Debug.Log("Souffle trop fort");
            }
            else
            {
                isBlowing = true;
                breathStrength = Mathf.InverseLerp(10f, 20f, value);
                //Debug.Log($"Souffle valide: {value} → strength = {breathStrength:F2}");
            }
        }
        else
        {
            isBlowing = false;
            Debug.LogWarning($"Valeur invalide ou nulle reçue: '{msg}'");
        }
        //Debug.Log($"Souffle valide: {value} → strength = {breathStrength:F2}");
    }

    public void OnConnectionEvent(bool success)
    {
        Debug.Log(success ? "Device connected" : "Device disconnected");
    }
}
