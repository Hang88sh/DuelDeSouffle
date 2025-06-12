using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
    //public Slider breathSlider; // Ancien slider linéaire
    //public Image gaugeFillImage; // Jauge circulaire
    public ImgsFillDynamic roundGauge;

    private float breathStrength = 0f; // Intensité du souffle [0–1]
    private bool isBlowing = false; // Statut de souffle
    private float timeSinceSpawn = 0f; // Temps depuis apparition

    private float stopGracePeriod = 0.15f; // Temps de tolérance post-souffle
    private float timeSinceLastValidInput = 999f;

    public PersistentBreathText persistentText;
    private string lastPhase = "";

    private bool forceAppliedThisBreath = false;

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

    // Mise à jour de l’intensité du souffle selon l’entrée simulée
    void UpdateBreathStrength()
    {
        float raw = breathStrength * 10f + 10f;
        float target = isBlowing ? 20f : 10f;
        float moved = Mathf.MoveTowards(raw, target, Time.deltaTime * (isBlowing ? increaseSpeed : decreaseSpeed) * 10f);
        breathStrength = Mathf.InverseLerp(10f, 20f, moved);
        breathStrength = Mathf.Clamp01(breathStrength);
    }

    // Gère le temps de grâce pour ne pas couper le souffle trop brutalement
    void UpdateBreathStatus()
    {
        if (isBlowing)
            timeSinceLastValidInput = 0f;
        else
            timeSinceLastValidInput += Time.deltaTime;
    }

    // Applique une vitesse initiale une seule fois par souffle
    void ApplyBreathVelocity()
    {
        if (ballRb == null || timeSinceSpawn < activationDelay)
            return;

        bool isStillBreathing = timeSinceLastValidInput < stopGracePeriod;

        if (isStillBreathing && !forceAppliedThisBreath)
        {
            float targetVelocity = Mathf.Lerp(0f, maxUpVelocity, breathStrength);
            Vector3 velocity = ballRb.linearVelocity;
            velocity.y = targetVelocity;
            ballRb.linearVelocity = velocity;

            forceAppliedThisBreath = true;
            Debug.Log($"Souffle appliqué une fois - vitesse Y: {velocity.y:F2}");
        }
    }

    // Met à jour la jauge visuelle (slider ou cercle)
    void UpdateUI()
    {
        if (roundGauge != null)
            roundGauge.SetValue(breathStrength, false, 2f);
    }

    void UpdateBreathZoneText()
    {
        if (persistentText == null) return;

        string phase = "";
        Color color = Color.white;
        float offset = 0f;

        if (breathStrength < 0.05f && timeSinceLastValidInput > 1f)
        {
            //Trop de temps sans souffle → masquer le texte
            persistentText.gameObject.SetActive(false);
            lastPhase = "";
            return;
        }

        if (breathStrength < 0.3f)
        {
            phase = "Trop faible !";
            color = Color.yellow;
            offset = -15f;
        }
        else if (breathStrength > 0.8f)
        {
            phase = "Trop fort !";
            color = Color.red;
            offset = 20f;
        }
        else
        {
            phase = "Parfait !";
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

    // Réception des messages du capteur ou simulateur de souffle
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
