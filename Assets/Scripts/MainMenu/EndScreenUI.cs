using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndScreenUI : MonoBehaviour
{
    [Header("Texte pour afficher le score final")]
    public TextMeshProUGUI scoreText;

    [Header("Nom de la scène du niveau actuel")]
    public string sceneDuNiveau;

    [Header("Nom de la scène du menu principal")]
    public string sceneMenuPrincipal;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        //gameObject.SetActive(false);
        // Récupère le CanvasGroup pour contrôler la visibilité
        canvasGroup = GetComponent<CanvasGroup>();
        Cacher(); // Cache l'écran de fin au démarrage
    }

    
    // Affiche automatiquement le score en trouvant le BallScorer    
    public void ShowScore()
    {
        
        gameObject.SetActive(true);

        // Cherche le BallScorer et récupère le score
        BallScorer scorer = FindFirstObjectByType<BallScorer>();
        int finalScore = (scorer != null) ? scorer.GetScore() : 0;

        // Affiche le score dans l'interface
        scoreText.text = "Score : " + finalScore;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    
    // Affiche manuellement un score donné (ancienne méthode)   
    public void Afficher(int scoreFinal)
    {
        scoreText.text = "Score : " + scoreFinal;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    
    // Cache l'écran de fin    
    public void Cacher()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
       
    
    // Recharge la scène actuelle (rejouer)    
    public void Rejouer()
    {
        SceneManager.LoadScene(sceneDuNiveau);
    }
    
    // Retourne au menu principal    
    public void RetourMenu()
    {
        SceneManager.LoadScene(sceneMenuPrincipal);
    }
}
