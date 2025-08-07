using UnityEngine;
using System.Collections;

public class SceneLocalizationInitializer : MonoBehaviour
{
    void Start()
    {
        // V¨¦rifie si le gestionnaire de localisation est d¨¦j¨¤ pr¨¦sent dans la sc¨¨ne
        if (LocalizationManager.Instance == null)
        {
            Debug.LogWarning("Aucun LocalizationManager d¨¦tect¨¦. Chargement du prefab depuis le dossier Resources.");

            // Instancie le prefab ¨¤ partir du dossier Resources
            GameObject locManager = Instantiate(Resources.Load<GameObject>("LocalizationManager"));
            locManager.name = "LocalizationManager"; // Supprime le suffixe (Clone) automatiquement ajout¨¦
        }

        // D¨¦marre une routine pour charger la langue apr¨¨s une frame
        StartCoroutine(ChargementDiffere());
    }

    IEnumerator ChargementDiffere()
    {
        // Attend une frame pour s'assurer que le Singleton est bien initialis¨¦
        yield return null;

        // Charge la langue sauvegard¨¦e (ou "fr" par d¨¦faut)
        string langue = PlayerPrefs.GetString("Langue", "fr");
        LocalizationManager.Instance.LoadLocalizedText(langue);
    }
}
