using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BallSpawner : MonoBehaviour
{
    public BallPoolManager ballPoolManager;
    public Slider breathSlider;

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
            Debug.LogError("ballPoolManager is NULL! Please assign it in the Inspector.");
            return;
        }

        GameObject newBall=ballPoolManager.SpawnBall();

        if (newBall == null)
        {
            Debug.LogError("⚠️ newBall is null — pool might be empty!");
            return;
        }

        BallInitializer init=newBall.AddComponent<BallInitializer>();
        init.breathSlider=breathSlider;
        init.Initialize();
    }
}
