using UnityEngine;
using System.Collections.Generic;

public class BallPoolManager : MonoBehaviour
{
    [Header("Liste des prefabs de balles")]
    public GameObject[] ballPrefabs; //tous les balles qu'on veut utiliser

    [Header("Point d'apparition de la balle")]
    public Transform spawnPoint;//l'endorit ou la balle sera generee

    [Header("Taille du pool")]
    public int poolSize = 10; //nom de balles 

    private Queue<GameObject> ballPool = new Queue<GameObject>(); //La file d’attente qui stocke les balles inactives
    private GameObject currentBall; //la balle actuellement activite

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initializepool();
        Debug.Log($" BallPool initialized with {ballPool.Count} balls.");

    }

    void Initializepool()
    {
        for (int i = 0;  i < poolSize; i++)
        {
            //choisit un prefab dans la liste
            int index=Random.Range(0, ballPrefabs.Length);

            //instancie une balle mais la desactive immediatement
            GameObject ball=Instantiate(ballPrefabs[index],Vector3.zero,Quaternion.identity);
            ball.SetActive(false);

            //ajoute la balle au pool
            ballPool.Enqueue(ball);
        }
    }

    public GameObject SpawnBall()
    {
        if (ballPool.Count == 0)
        {
            Debug.LogWarning("Ball pool empty!");
            return null;
        }

        //desactive l'ancienne balle si elle exisite
        if (currentBall!=null)
        {
            currentBall.SetActive(false);
        }

        //prend une balle dans le pool
        currentBall= ballPool.Dequeue();

        //place la balle au bon endroit
        currentBall.transform.position=spawnPoint.position;
        currentBall.transform.rotation = Quaternion.identity;
        currentBall.SetActive(true);

        //remet a zero la physique et applique une force vers le haut
        Rigidbody rb= currentBall.GetComponent<Rigidbody>();
        if(rb != null )
        {
            rb.angularVelocity=Vector3.zero;
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(Vector3.up*5f,ForceMode.Impulse);
           
        }

        //remet la balle dans le pool pour reutilisation plus tard
        //ballPool.Enqueue(currentBall);

        //retourne la balle pour l'initialiser si besoin
        return currentBall;
    }
    
}
