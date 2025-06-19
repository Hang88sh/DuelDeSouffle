using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BallSpawner : MonoBehaviour
{
    public BallPoolManager ballPoolManager;
    public SFXManager sfxManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DelaySpawn());
        
    }

    IEnumerator DelaySpawn()
    {
        yield return null;
        SpawnNewBall();
    }

    public void SpawnNewBall()
    {
        if (ballPoolManager == null)
        {
            Debug.LogError("BallPoolManager is not assigned!");
            return;
        }

        GameObject newBall=ballPoolManager.SpawnBall();

        Rigidbody rb = newBall.GetComponent<Rigidbody>();
       
        if (newBall == null)
        {
            Debug.LogError("newBall is null — pool might be empty!");
            return;
        }

        PlaySpawnEffect(newBall.transform.position);

        if (sfxManager != null)
        {
            Debug.Log("appele PlaySpawn");
            sfxManager.PlaySpawn();
        }
        else
        {
            Debug.LogWarning("SFXManager non assigné !");
        }

        Slider uiSlider = GameObject.FindFirstObjectByType<Slider>();
        if (uiSlider == null)
        {
            Debug.LogWarning("Aucun Slider trouvé dans la scène.");
        }

        BreathInputHandler handler = newBall.GetComponent<BreathInputHandler>();
        if (handler != null)
        {
            handler.breathSlider = uiSlider;
        }
        else
        {
            Debug.LogWarning("BreathInputHandler introuvable sur la balle !");
        }
    }
    void PlaySpawnEffect(Vector3 position)
    {
        GameObject fxPrefab = Resources.Load<GameObject>("Effects/CFXR Magic Poof");//Charge le prefab de l'effet à partir du dossier Resources
        if (fxPrefab != null)
        {
            GameObject fx = Instantiate(fxPrefab, position, Quaternion.identity);
            Destroy(fx, 2f); // Détruit l'effet après 2 secondes pour éviter l'accumulation
        }
        else
        {
            Debug.LogWarning("Échec du chargement de l'effet ! Vérifiez le chemin Resources/Effects/CFXR Magic Poof");
        }
    }
   
}

