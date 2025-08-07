using UnityEngine;
using TMPro;

/// <summary>
/// G¨¨re le changement de langue via le menu d¨¦roulant (Dropdown)
/// </summary>
public class LanguageDropdownHandler : MonoBehaviour
{
    public TMP_Dropdown languageDropdown;

    private void Start()
    {
        // Appliquer la langue courante au d¨¦marrage
        if (LocalizationManager.Instance != null)
        {
            if (LocalizationManager.Instance.CurrentLanguage == "fr")
                languageDropdown.value = 0;
            else
                languageDropdown.value = 1;
        }

        // Ajouter l'¨¦couteur d'¨¦v¨¦nement
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    /// <summary>
    /// Change la langue selon l'index s¨¦lectionn¨¦
    /// </summary>
    /// <param name="index">Index de l'option s¨¦lectionn¨¦e</param>
    public void OnLanguageChanged(int index)
    {
        string selectedLang = index == 0 ? "fr" : "en";
        LocalizationManager.Instance.LoadLocalizedText(selectedLang);
    }
}
