using UnityEngine;
using DG.Tweening;

public class ZoneColorController : MonoBehaviour
{
    [Header("Renderer de la zone")]
    public Renderer zoneRenderer; // Composant Renderer de la zone (mat¨¦riau ¨¤ changer)

    [Header("Couleurs d'¨¦tat")]
    public Color couleurParDefaut = new Color(136f / 255f, 1f, 136f / 255f, 0.45f); // Couleur verte initiale
    public Color couleurSucces = new Color(1f, 0.85f, 0f, 0.6f); // Couleur dor¨¦e en cas de succ¨¨s

    private Material mat; // R¨¦f¨¦rence au mat¨¦riau utilis¨¦

    void Awake()
    {
        // On r¨¦cup¨¨re le mat¨¦riau depuis le renderer
        if (zoneRenderer != null)
        {
            mat = zoneRenderer.material;

            // On applique la couleur par d¨¦faut au d¨¦but
            mat.color = couleurParDefaut;
        }
    }

    public void SetDefaultColor()
    {
        // Changement progressif vers la couleur par d¨¦faut (vert)
        if (mat != null)
        {
            mat.DOColor(couleurParDefaut, 0.3f);
        }
    }

    public void SetSuccessColor()
    {
        // Changement progressif vers la couleur de succ¨¨s (jaune)
        if (mat != null)
        {
            mat.DOColor(couleurSucces, "_BaseColor", 0.3f);
        }
    }
}
