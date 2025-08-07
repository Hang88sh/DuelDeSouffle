using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class BreathInputHandler : MonoBehaviour
{
    [Header("Physique")]
    public Rigidbody ballRb; // Rigidbody de la balle
    public float maxUpVelocity = 8f; // Vitesse maximale de montée
    public float activationDelay = 0.5f; // Délai avant activation

    [Header("Entrée simulée")]
    public float increaseSpeed = 1f; // Vitesse de montée du souffle simulé
    public float decreaseSpeed = 1f; // Vitesse de descente

    [Header("Interface utilisateur")]
    public ImgsFillDynamic roundGauge; // Jauge circulaire
    public PersistentBreathText persistentText; // Texte persistant de feedback

    private float breathStrength = 0f; // Intensité du souffle [0–1]
    private bool isBlowing = false; // Statut de souffle
    private float timeSinceSpawn = 0f; // Temps depuis l’apparition
    private float stopGracePeriod = 0.15f; // Tolérance après arrêt du souffle
    private float timeSinceLastValidInput = 999f;
    private bool forceAppliedThisBreath = false;

    private string lastPhase = ""; // Dernière phase affichée

    // Ajout : clés de traduction à utiliser
    private string keyTooWeak = "too_weak";
    private string keyTooStrong = "too_strong";
    private string keyPerfect = "perfect";

    public void OnBlow(InputAction.CallbackContext context)
    {
        isBlowing = context.performed;
        if (isBlowing)
        {
            forceAppliedThisBreath = false;
        }
    }

    void Start()
    {
        breathStrength = 0f;
        isBlowing = false;
        timeSinceSpawn = 0f;
        timeSinceLastValidInput = 999f;
        forceAppliedThisBreath = false;

        if (persistentText != null)
        {
            persistentText.gameObject.SetActive(false); // Masquer au démarrage
        }
    }

    void Update()
    {
        timeSinceSpawn += Time.deltaTime;

        UpdateBreathStrength();
        UpdateBreathStatus();
        ApplyBreathVelocity();
        UpdateBreathZoneText();
        UpdateUI();
    }

    // Met à jour l’intensité du souffle (entrée simulée ou réelle)
    void UpdateBreathStrength()
    {
        float raw = breathStrength * 10f + 10f;
        float target = isBlowing ? 20f : 10f;
        float moved = Mathf.MoveTowards(raw, target, Time.deltaTime * (isBlowing ? increaseSpeed : decreaseSpeed) * 10f);
        breathStrength = Mathf.InverseLerp(10f, 20f, moved);
        breathStrength = Mathf.Clamp01(breathStrength);
    }

    // Gère le temps de grâce après arrêt du souffle
    void UpdateBreathStatus()
    {
        if (isBlowing)
            timeSinceLastValidInput = 0f;
        else
            timeSinceLastValidInput += Time.deltaTime;
    }

    // Applique une vitesse de montée à la balle
    void ApplyBreathVelocity()
    {
        if (ballRb == null || timeSinceSpawn < activationDelay) return;

        bool isStillBreathing = timeSinceLastValidInput < stopGracePeriod;

        if (isStillBreathing)
        {
            float targetVelocity;

            // Convertit le souffle en valeur brute (10 ~ 20) pour décider des zones
            float rawValue = Mathf.Lerp(10f, 20f, breathStrength);

            if (rawValue >= 13f && rawValue <= 18f)
            {
                // Zone stable : souffle confortable → vitesse constante
                targetVelocity = 10f;
            }
            else if (rawValue < 13f)
            {
                // En dessous : interpole progressivement vers 10
                float t = Mathf.InverseLerp(10f, 13f, rawValue);
                targetVelocity = Mathf.Lerp(0f, 10f, t); // montée douce
            }
            else // rawValue > 18f
            {
                // Au-dessus : interpole de 10 à une vitesse plus forte (par exemple 16)
                float t = Mathf.InverseLerp(18f, 20f, rawValue);
                targetVelocity = Mathf.Lerp(10f, 16f, t); // montée contrôlée
            }

            // Lissage de la transition
            Vector3 currentVelocity = ballRb.linearVelocity;
            float smoothedY = Mathf.MoveTowards(currentVelocity.y, targetVelocity, Time.deltaTime * 50f);
            currentVelocity.y = smoothedY;
            ballRb.linearVelocity = currentVelocity;
        }
    }

    // Affiche le texte correspondant à la zone de souffle
    void UpdateBreathZoneText()
    {
        if (persistentText == null) return;

        string phase = "";
        Color color = Color.white;
        float offset = 0f;

        if (breathStrength < 0.05f && timeSinceLastValidInput > 1f)
        {
            persistentText.gameObject.SetActive(false);
            lastPhase = "";
            return;
        }

        if (breathStrength < 0.3f)
        {
            phase = LocalizationManager.Instance.GetText(keyTooWeak); // Trop faible
            color = Color.yellow;
            offset = -15f;
        }
        else if (breathStrength > 0.8f)
        {
            phase = LocalizationManager.Instance.GetText(keyTooStrong); // Trop fort
            color = Color.red;
            offset = 20f;
        }
        else
        {
            phase = LocalizationManager.Instance.GetText(keyPerfect); // Parfait
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

    // Met à jour la jauge circulaire visuelle
    void UpdateUI()
    {
        if (roundGauge == null) return;

        bool isStillBreathing = timeSinceLastValidInput < stopGracePeriod;
        float displayValue = isStillBreathing ? breathStrength : 0f;

        roundGauge.SetValue(displayValue, true);
    }

    // Réception de message via capteur (USB/port série)
    public void OnMessageArrived(string msg)
    {
        if (float.TryParse(msg, out float value) && value > 0)
        {
            if (value < 10f || value > 20f)
            {
                isBlowing = false;
                Debug.Log("Souffle hors plage !");
            }
            else
            {
                isBlowing = true;
                breathStrength = Mathf.InverseLerp(10f, 20f, value);
                forceAppliedThisBreath = false;
                Debug.Log($"Souffle détecté : {value} → intensité = {breathStrength:F2}");
            }
        }
        else
        {
            isBlowing = false;
            Debug.LogWarning("Entrée invalide du souffle !");
        }
    }

    public void OnConnectionEvent(bool success)
    {
        Debug.Log(success ? "Périphérique connecté" : "Périphérique déconnecté");
    }
}
