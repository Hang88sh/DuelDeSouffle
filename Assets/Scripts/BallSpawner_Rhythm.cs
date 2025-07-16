using System.Collections;
using UnityEngine;

public class BallSpawner_Rhythm : MonoBehaviour
{
    public BallPoolManager ballPoolManager;    // Gestionnaire du pool de balles
    public SFXManager sfxManager;              // Gestionnaire des effets sonores
    public JudgementLine judgementLine;        // Ligne de jugement pour le rythme

    public void SpawnNewBall()
    {
        if (ballPoolManager == null)
        {
            Debug.LogError("BallPoolManager non assigné !");
            return;
        }

        GameObject newBall = ballPoolManager.SpawnBall();
        if (newBall == null)
        {
            Debug.LogError("newBall est null — le pool est peut-être vide !");
            return;
        }

        // Effet visuel de génération
        PlaySpawnEffect(newBall.transform.position);

        // Effet sonore
        if (sfxManager != null)
        {
            sfxManager.PlaySpawn();
        }

        // Liaison UI (même logique que l'original)
        BreathInputHandler handler = newBall.GetComponent<BreathInputHandler>();
        if (handler != null)
        {
            ImgsFillDynamic uiGauge = GameObject.FindFirstObjectByType<ImgsFillDynamic>();
            if (uiGauge != null)
            {
                handler.roundGauge = uiGauge;
            }

            PersistentBreathText breathText = GameObject.FindFirstObjectByType<PersistentBreathText>();
            if (breathText != null)
            {
                handler.persistentText = breathText;
            }
        }

        // ★ Envoyer la balle à la ligne de jugement
        if (judgementLine != null)
        {
            judgementLine.SendMessage("SetBall", newBall.transform);
            Debug.Log("Balle assignée à la ligne de jugement");
        }
        else
        {
            Debug.LogWarning("Ligne de jugement non assignée !");
        }
    }

    void PlaySpawnEffect(Vector3 position)
    {
        GameObject fxPrefab = Resources.Load<GameObject>("Effects/CFXR Magic Poof");
        if (fxPrefab != null)
        {
            GameObject fx = Instantiate(fxPrefab, position, Quaternion.identity);
            Destroy(fx, 2f);
        }
    }
}
