using UnityEngine;

public class MelodyFade : MonoBehaviour
{
    public float speed = 2f;
    public float lifeTime = 2f;
    private SpriteRenderer sr;
    private float timer = 0;

    void Start() => sr = GetComponent<SpriteRenderer>();

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1, 0, timer / lifeTime);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

        if (timer >= lifeTime) Destroy(gameObject);
    }
}