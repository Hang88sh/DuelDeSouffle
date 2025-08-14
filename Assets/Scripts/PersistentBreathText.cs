using UnityEngine;
using TMPro;
using DG.Tweening;

public class PersistentBreathText : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    private RectTransform rt;
    private Tween shakeTween;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rt = textUI.rectTransform;
        canvasGroup = GetComponent<CanvasGroup>();

        // D¨¦marre le tremblement en boucle
        ShakeLoop();

        // Masque initialement le texte
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    void ShakeLoop()
    {
        shakeTween = rt
            .DOShakeRotation(0.5f, strength: 5f, vibrato: 4, randomness: 30f)
            .OnComplete(ShakeLoop);
    }

    // Met ¨¤ jour le texte, la couleur et la rotation. Rend visible si masqu¨¦.
    public void SetText(string message, Color color, float angle)
    {
        if (canvasGroup != null && canvasGroup.alpha < 1f)
            canvasGroup.alpha = 1f;

        textUI.text = message;
        textUI.color = color;
        rt.localRotation = Quaternion.Euler(0, 0, angle);
    }

    // Cache visuellement le texte
    public void HideText()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    void OnDestroy()
    {
        shakeTween?.Kill();
    }
}
