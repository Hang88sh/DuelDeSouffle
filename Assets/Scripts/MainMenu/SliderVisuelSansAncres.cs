using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Slider))]
public class SliderVisuelSansAncres : MonoBehaviour
{
    [Header("R¨¦f¨¦rences UI (ne pas donner au Slider)")]
    public Image fillImage;                 // Image du Fill (Type=Filled/Horizontal)
    public RectTransform handle;            // Rect du bouton visuel (le rond)
    public RectTransform handleArea;        // Zone de d¨¦placement (ex: Handle Slide Area)

    private Slider s;

    void Awake()
    {
        s = GetComponent<Slider>();
        MettreAJour();
    }

    void OnEnable()
    {
        if (s == null) s = GetComponent<Slider>();
        s.onValueChanged.AddListener(_ => MettreAJour());
        MettreAJour();
    }

    void OnDisable()
    {
        if (s != null) s.onValueChanged.RemoveListener(_ => MettreAJour());
    }

    void OnValidate()
    {
        if (s == null) s = GetComponent<Slider>();
        MettreAJour();
    }

    // Applique l'¨¦tat visuel sans toucher aux ancres
    void MettreAJour()
    {
        float t = (s != null) ? s.normalizedValue : 0f;

        // 1) Remplissage de la barre
        if (fillImage != null) fillImage.fillAmount = t;

        // 2) Position du handle (rest¨¦ centr¨¦ verticalement)
        if (handle != null && handleArea != null)
        {
            var zone = handleArea.rect;
            float demi = zone.width * 0.5f;

            // on garde le bouton dans la zone, en tenant compte de sa demi-largeur
            float demiHandle = handle.rect.width * 0.5f;
            float gauche = -demi + demiHandle;
            float droite = demi - demiHandle;

            var p = handle.anchoredPosition;
            p.x = Mathf.Lerp(gauche, droite, t);
            p.y = 0f;
            handle.anchoredPosition = p;
        }
    }
}
