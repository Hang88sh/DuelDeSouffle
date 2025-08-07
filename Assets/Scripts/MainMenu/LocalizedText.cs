using TMPro;
using UnityEngine;

/// <summary>
/// Composant pour localiser dynamiquement les textes d'interface.
/// </summary>
public class LocalizedText : MonoBehaviour
{
    public string key;  // Cl¨¦ de traduction
    private TextMeshProUGUI textUI;

    void Awake()
    {
        textUI = GetComponent<TextMeshProUGUI>();
        if (textUI == null)
        {
            // NE PAS CHERCHER DANS L'OBJET ACTUEL (le bouton), CHERCHER DANS LES ENFANTS
            textUI = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    public void UpdateText()
    {
        if (textUI != null && LocalizationManager.Instance != null)
        {
            textUI.text = LocalizationManager.Instance.GetText(key);
        }
        else
        {
            Debug.LogWarning("Composant Text non trouv¨¦ ou syst¨¨me de localisation absent sur : " + gameObject.name);
        }
    }
}
