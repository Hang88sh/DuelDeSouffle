using UnityEngine;

public class Note : MonoBehaviour
{
    private float speed;
    private float duration;

    public float Duration => duration;

    public void Init(float duration, float speed)
    {
        this.duration = duration;
        this.speed = speed;

        float newLength = duration * speed;

        transform.localScale = new Vector3(newLength, 1f, 1f);
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < -15f)
            Destroy(gameObject);
    }
}
