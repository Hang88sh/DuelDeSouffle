using UnityEngine;

public class JudgementLine : MonoBehaviour
{
    public string ballTag = "Ball";  // Tag de la balle pour la trouver automatiquement
    public float heightTolerance = 0.5f; // Tol¨¦rance de hauteur

    private Transform ball;          // R¨¦f¨¦rence ¨¤ la balle
    private Note currentNote;
    private float stayTime = 0f;

    void Start()
    {
        // Trouver la balle dynamiquement au lancement
        GameObject ballObj = GameObject.FindGameObjectWithTag(ballTag);
        if (ballObj != null)
        {
            ball = ballObj.transform;
        }
        else
        {
            Debug.LogWarning("Balle non trouv¨¦e, v¨¦rifiez le tag !");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Note"))
        {
            currentNote = other.GetComponent<Note>();
            stayTime = 0f;
            Debug.Log("D¨¦but du jugement : " + other.name);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentNote != null && ball != null && other.CompareTag("Note"))
        {
            float ballY = ball.position.y;
            float noteY = currentNote.transform.position.y;

            if (Mathf.Abs(ballY - noteY) <= heightTolerance)
            {
                stayTime += Time.deltaTime;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Note") && currentNote != null)
        {
            if (stayTime >= currentNote.Duration)
            {
                Debug.Log("Note longue r¨¦ussie !");
            }
            else
            {
                Debug.Log("Note longue ¨¦chou¨¦e !");
            }

            currentNote = null;
            stayTime = 0f;
        }
    }

    public void SetBall(Transform newBall)
    {
        ball = newBall;
    }
}
