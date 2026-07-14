using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StarburstSparks : MonoBehaviour
{
    public GameObject sparkPrefab;
    public float spawnRate = 0.15f;
    public float sparkSpeed = 300.0f;
    private float timer = 0.0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            timer = 0.0f;
            SpawnSpark();
        }
    }

    void SpawnSpark()
    {
        if (sparkPrefab == null) return;
        GameObject spark = Instantiate(sparkPrefab, transform);
        spark.transform.localPosition = Vector3.zero;
        
        float randomAngle = Random.Range(0.0f, 360.0f);
        spark.transform.localRotation = Quaternion.Euler(0, 0, randomAngle);
        
        StartCoroutine(MoveSpark(spark.GetComponent<RectTransform>()));
    }

    IEnumerator MoveSpark(RectTransform rect)
    {
        float time = 0;
        float duration = 1.2f;
        Vector3 direction = rect.up;

        while (time < duration)
        {
            if (rect == null) yield break;
            time += Time.deltaTime;
            rect.localPosition += direction * sparkSpeed * Time.deltaTime;
            yield return null;
        }

        if (rect != null)
        {
            Destroy(rect.gameObject);
        }
    }
}