using UnityEngine;
using TMPro;
using DG.Tweening;

public class PersistentBreathText : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    private RectTransform rt;
    private Tween shakeTween;

    void Awake()
    {
        rt = textUI.rectTransform;


        ShakeLoop();
    }

    void ShakeLoop()
    {
        shakeTween = rt
            .DOShakeRotation(0.5f, strength: 5f, vibrato: 4, randomness: 30f)
            .OnComplete(ShakeLoop); 
    }

    public void SetText(string message, Color color, float angle)
    {
        textUI.text = message;
        textUI.color = color;
        rt.localRotation = Quaternion.Euler(0, 0, angle);
    }

    void OnDestroy()
    {
        shakeTween?.Kill();
    }
}
