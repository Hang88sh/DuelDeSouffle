using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Panneaux UI")]
    public GameObject panneauMenuPrincipal;
    public GameObject panneauSelectionNiveau;
    public GameObject panneauParametres;

    [Header("Liste des niveaux")]
    public TMP_Dropdown dropdownNiveaux;
    public string[] nomsScenes;

    [Header("UI des paramètres")]
    public TMP_Dropdown dropdownLangue;
    public Slider sliderVolume;

    void Start()
    {
        // Lier les événements
        if (dropdownLangue != null)
        {
            dropdownLangue.onValueChanged.AddListener(ChangerLangue);

            // Charger la langue sauvegardée
            string langueSauvegardee = PlayerPrefs.GetString("Langue", "fr");
            dropdownLangue.value = (langueSauvegardee == "fr") ? 0 : 1;
        }

        if (sliderVolume != null)
        {
            sliderVolume.onValueChanged.AddListener(ChangerVolume);
            sliderVolume.value = AudioListener.volume;
        }

        // Appliquer la langue au démarrage
        LocalizationManager.Instance.LoadLocalizedText(PlayerPrefs.GetString("Langue", "fr"));
        RafraichirTextes(panneauMenuPrincipal);
    }

    public void AppuyerCommencerJeu()
    {
        panneauMenuPrincipal.SetActive(false);
        panneauSelectionNiveau.SetActive(true);
        RafraichirTextes(panneauSelectionNiveau);
    }

    public void AppuyerRetour()
    {
        panneauSelectionNiveau.SetActive(false);
        panneauMenuPrincipal.SetActive(true);
        RafraichirTextes(panneauMenuPrincipal);
    }

    public void AppuyerJouer()
    {
        int index = dropdownNiveaux.value;
        SceneManager.LoadScene(nomsScenes[index]);
    }

    public void AppuyerQuitter()
    {
        Application.Quit();
        Debug.Log("Quitter le jeu");
    }

    public void AppuyerParametres()
    {
        panneauMenuPrincipal.SetActive(false);
        panneauParametres.SetActive(true);
        RafraichirTextes(panneauParametres);
    }

    public void AppuyerRetourDepuisParametres()
    {
        panneauParametres.SetActive(false);
        panneauMenuPrincipal.SetActive(true);
        RafraichirTextes(panneauMenuPrincipal);
    }

    public void ChangerLangue(int index)
    {
        string langueChoisie = (index == 0) ? "fr" : "en";
        PlayerPrefs.SetString("Langue", langueChoisie);
        PlayerPrefs.Save();

        LocalizationManager.Instance.LoadLocalizedText(langueChoisie);
        RafraichirTextes(panneauParametres);
    }

    public void ChangerVolume(float valeur)
    {
        AudioListener.volume = valeur;
        Debug.Log("Volume : " + valeur);
    }

    // Recharge tous les textes traduits dans un panneau donné
    private void RafraichirTextes(GameObject panneau)
    {
        LocalizedText[] textes = panneau.GetComponentsInChildren<LocalizedText>(true);
        foreach (var t in textes)
        {
            t.UpdateText();
        }
    }
}
