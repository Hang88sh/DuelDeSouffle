using UnityEngine;

[ExecuteAlways] // Permet d'exécuter le script même en mode Édition (pas besoin d'appuyer sur Play)
public class BackgroundFitter : MonoBehaviour
{
    public Camera mainCamera;         // Caméra principale utilisée pour calculer le ratio d'écran
    public float desiredHeight = 10f; // Hauteur souhaitée de l'objet pour remplir l'écran

    void Update()
    {
        // Assigner automatiquement la caméra principale si elle n'est pas définie
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        // Calcul du ratio écran (largeur / hauteur)
        float screenRatio = (float)Screen.width / Screen.height;

        // Calcul de la largeur nécessaire pour remplir l'écran en fonction de la hauteur
        float desiredWidth = desiredHeight * screenRatio;

        // Application du scale sur l'objet pour qu'il remplisse parfaitement l'écran
        transform.localScale = new Vector3(desiredWidth, desiredHeight, 1f);
    }

    void OnValidate()
    {
        // Appelé automatiquement quand un paramètre est modifié dans l'inspecteur
        Update();
    }
}
