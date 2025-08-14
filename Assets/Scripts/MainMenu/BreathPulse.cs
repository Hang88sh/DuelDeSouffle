using UnityEngine;

/// <summary>
/// Animation de "respiration" très légère pour un titre ou un groupe UI.
/// Fait osciller l'échelle (et optionnellement l'alpha) en temps non-scalé
/// afin que l'effet continue même lorsque le jeu est en pause.
/// </summary>
public class BreathPulse : MonoBehaviour
{
    [Header("Cible")]
    public RectTransform target;     // RectTransform à animer (TitleGroup ou le Text lui-même)

    [Header("Effets optionnels")]
    public CanvasGroup cg;           // CanvasGroup du conteneur (optionnel, pour faire varier l'opacité)

    [Header("Paramètres")]
    [Range(0f, 0.15f)]
    public float amplitude = 0.04f;  // Amplitude de l'agrandissement (0.04 = +4% / -4%)
    public float period = 3.5f;      // Durée d'un cycle de respiration (en secondes)
    public bool fadeWithBreath = true; // Si vrai, l'alpha suit la respiration (léger fondu)

    // État interne
    Vector3 baseScale;               // Échelle de base mémorisée au démarrage
    float t0;                        // Décalage de phase aléatoire pour éviter l'uniformité

    void Awake()
    {
        // Si aucune cible n'est assignée, on utilise ce GameObject
        if (!target) target = transform as RectTransform;

        // Récupère automatiquement le CanvasGroup s'il existe sur l'objet
        if (!cg) cg = GetComponent<CanvasGroup>();

        // Mémorise l'échelle initiale pour revenir autour de cette valeur
        baseScale = target.localScale;

        // Décalage aléatoire afin que plusieurs instances ne respirent pas en phase
        t0 = Random.value * 10f;
    }

    void Update()
    {
        // Temps non-scalé : l'animation tourne même si Time.timeScale = 0 (pause)
        float omega = Mathf.PI * 2f / period;
        float sin = Mathf.Sin((Time.unscaledTime + t0) * omega);

        // Échelle "qui respire" autour de 1
        float s = 1f + sin * amplitude;
        target.localScale = baseScale * s;

        // Variation douce de l'opacité (optionnelle)
        if (fadeWithBreath && cg)
        {
            // Alpha moyen à 0.92 avec ±0.04 d'amplitude
            float a = 0.92f + sin * 0.04f;
            cg.alpha = a;
        }
    }
}
