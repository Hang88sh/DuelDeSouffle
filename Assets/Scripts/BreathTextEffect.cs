using UnityEngine;
using TMPro;
using DG.Tweening;

public class BreathTextEffect : MonoBehaviour
{
    public TextMeshProUGUI textUI;

    [Header("Parametre")]
    public string message = "Parfait!";
    public Color textColor = Color.yellow;
    public Vector2 offset = new Vector2(0, 200); 
    public float holdDuration = 1.2f;
    public float startAngle = -20f;

    private void OnEnable()
    {
        PlayEffect();
    }

    public void PlayEffect()
    {
        textUI.text = message;
        textUI.color = textColor;
        textUI.alpha = 0;

        //État initial : petit, incliné, en bas à gauche
        RectTransform rt = textUI.rectTransform;
        rt.localScale = new Vector3(0.2f, 0.2f, 1f); // Taille réduite au départ
        rt.localRotation = Quaternion.Euler(0, 0, startAngle); // Angle d'apparition
        rt.anchoredPosition = offset;

        Sequence seq = DOTween.Sequence();

        // Étape 1 : jaillissement vers le haut droit + effet d'étirement
        seq.Append(textUI.DOFade(1f, 0.05f));
        seq.Join(rt.DOScale(new Vector3(1.6f, 1.2f, 1f), 0.2f).SetEase(Ease.OutExpo)); // largissement explosif
        seq.Join(rt.DOAnchorPos(rt.anchoredPosition + new Vector2(160, 80), 0.2f).SetEase(Ease.OutQuad)); // Mouvement diagonale
        seq.Join(rt.DORotate(Vector3.zero, 0.2f).SetEase(Ease.OutCubic)); // Retour à l'angle normal


        // Étape 2 : vibration + maintien
        seq.Append(rt.DOShakeRotation(holdDuration, 10, 10, 90)); // Secousses pendant quelques secondes
        seq.AppendInterval(0.1f);

        // Étape 3 : disparition + montée
        seq.Append(textUI.DOFade(0f, 0.4f));
        seq.Join(rt.DOScale(0.5f, 0.4f));
        seq.Join(rt.DOAnchorPos(rt.anchoredPosition + new Vector2(0, 50), 0.4f));

        // détruire l’objet après animation
        seq.OnComplete(() => Destroy(gameObject));
    }
}
