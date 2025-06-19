using System.Collections;
using UnityEngine;

public class TargetZoneManager : MonoBehaviour
{
    public Transform ball;                 // R¨¦f¨¦rence ¨¤ la boule
    public Transform targetZone;           // L'objet zone cible (cube semi-transparent)

    public float zoneDuration = 3.5f;      // Dur¨¦e d'affichage d'une zone
    public float requiredStayTime = 3f;    // Temps requis pour r¨¦ussir
    public float checkRadius = 0.5f;       // Rayon pour consid¨¦rer que la boule est ¨¤ l'int¨¦rieur

    public float refreshDelay = 3f;        // D¨¦lai avant nouvelle zone

    private float stayTimer = 0f;          // Temps que la boule reste dans la zone
    private float zoneTimer = 0f;          // Temps ¨¦coul¨¦ depuis apparition zone

    private bool zoneActive = false;

    

    void Start()
    {
        targetZone.gameObject.SetActive(false);
        StartCoroutine(StartGameRoutine());        
    }

    IEnumerator StartGameRoutine()
    {
        yield return new WaitForSeconds(2f);
        // Affiche un compte ¨¤ rebours ici si tu veux
        
        Debug.Log("3"); yield return new WaitForSeconds(1f);
        Debug.Log("2"); yield return new WaitForSeconds(1f);
        Debug.Log("1"); yield return new WaitForSeconds(1f);

        NewZone();
    }
       

    void Update()
    {
        if (!zoneActive) return;

        zoneTimer += Time.deltaTime;

        float distance = Mathf.Abs(ball.position.y - targetZone.position.y);
        if (distance < checkRadius)
        {
            stayTimer += Time.deltaTime;

            if (stayTimer >= requiredStayTime)
            {
                Debug.Log("Zone r¨¦ussie !");
                zoneActive = false;
                targetZone.gameObject.SetActive(false);
                StartCoroutine(NextZoneAfterDelay());
            }
        }
        else
        {
            stayTimer = 0f;
        }

        if (zoneTimer >= zoneDuration)
        {
            Debug.Log("Zone ¨¦chou¨¦e.");
            zoneActive = false;
            targetZone.gameObject.SetActive(false);
            StartCoroutine(NextZoneAfterDelay());
        }
    }

    IEnumerator NextZoneAfterDelay()
    {
        yield return new WaitForSeconds(refreshDelay);
        NewZone();
    }

    public void SetBall(Transform newBall)
    {
        ball = newBall;
    }

    void NewZone()
    {
        float newY = Random.Range(1.5f, 3.5f); // adapte cette plage selon tes besoins
        targetZone.position = new Vector3(targetZone.position.x, newY, targetZone.position.z);
        targetZone.gameObject.SetActive(true);

        stayTimer = 0f;
        zoneTimer = 0f;
        zoneActive = true;

        Debug.Log("Nouvelle zone cr¨¦¨¦e ¨¤ Y = " + newY.ToString("F2"));
    }
}
