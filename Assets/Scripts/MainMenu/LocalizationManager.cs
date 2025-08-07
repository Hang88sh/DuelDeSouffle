using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    private Dictionary<string, string> localizedText;
    private string currentLanguage = "fr";
    public string CurrentLanguage => currentLanguage;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            //Charger la langue sauvegard¨¦e si disponible
            if (PlayerPrefs.HasKey("language"))
            {
                currentLanguage = PlayerPrefs.GetString("language");
            }

            LoadLocalizedText(currentLanguage);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadLocalizedText(string languageCode)
    {
        currentLanguage = languageCode;

        //Sauvegarder la langue choisie
        PlayerPrefs.SetString("language", languageCode);
        PlayerPrefs.Save();

        localizedText = new Dictionary<string, string>();

        TextAsset jsonData = Resources.Load<TextAsset>("localization");
        if (jsonData == null)
        {
            Debug.LogError("Fichier de localisation introuvable dans Resources !");
            return;
        }

        LocalizationDataWrapper wrapper = JsonUtility.FromJson<LocalizationDataWrapper>(jsonData.text);
        if (wrapper != null && wrapper.languages != null)
        {
            foreach (var lang in wrapper.languages)
            {
                if (lang.code == languageCode)
                {
                    foreach (var entry in lang.entries)
                    {
                        localizedText[entry.key] = entry.value;
                    }
                    break;
                }
            }
        }
        else
        {
            Debug.LogError("Structure JSON invalide.");
        }

        LocalizedText[] localizedTexts = Object.FindObjectsByType<LocalizedText>(FindObjectsSortMode.None);
        foreach (var lt in localizedTexts)
        {
            lt.UpdateText();
        }
    }

    public string GetText(string key)
    {
        if (localizedText != null && localizedText.ContainsKey(key))
            return localizedText[key];
        return key;
    }

    [System.Serializable]
    public class LocalizationDataWrapper
    {
        public List<LanguageData> languages;
    }

    [System.Serializable]
    public class LanguageData
    {
        public string code;
        public List<LocalizationEntry> entries;
    }

    [System.Serializable]
    public class LocalizationEntry
    {
        public string key;
        public string value;
    }
}
