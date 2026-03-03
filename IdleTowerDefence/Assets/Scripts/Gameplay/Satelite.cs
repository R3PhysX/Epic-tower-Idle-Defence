using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satelite : MonoBehaviour
{
    public float radius = 1f; // Radius of the circular path
    public float speed = 1f; // Speed at which the object flies around the circle

    private float angle = 0f;

    private bool set;
    internal void Set(float radius = 1)
    {
        StartCoroutine(Fly());
        this.radius = 0;
        angle = Random.Range(0.1f, 1.9f) * Mathf.PI;
        speed = radius;
        set = true;
    }

    IEnumerator Fly()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            List<Enemy> nearestEnemy = new List<Enemy>();

            foreach ((Enemy, UnityPool) enemy in EnemyGenerator.Get.spawnedEnemy)
            {
                float distance = Vector2.Distance(transform.position, enemy.Item1.transform.position);
                if (distance <= 0.2f)
                {
                    nearestEnemy.Add(enemy.Item1);
                }
            }

            if (nearestEnemy.Count > 0)
            {
                //Blast
                Debug.Log("Blast " + gameObject.name);
                Vector3 pos = transform.position;
                pos.z = -2;
                var obj = GameObject.Instantiate(SateliteManager.Get.blastParticle.gameObject, pos, Quaternion.identity);

                foreach (Enemy enemy in nearestEnemy)
                {
                    enemy.TakeDamage(40, DamageType.Kill);
                }

                LeanTween.delayedCall(1f, () =>
                {
                    if (obj != null) Destroy(obj);
                });
                Destroy(this.gameObject);
                SateliteManager.Get.spawnedSatelite -= 1;
                AudioManager.Instance?.PlaySFXSound(AudioClipsType.BombSound);
                break;
            }
        }
    }

    private void Update()
    {
        if (set)
        {
            float x = Player.Instance.transform.position.x + radius * Mathf.Cos(angle);
            float y = Player.Instance.transform.position.y + radius * Mathf.Sin(angle);
            Vector3 targetPosition = new Vector3(x, y, transform.position.z);

            transform.position = targetPosition;

            // Increment the angle for the next frame
            angle += speed * Time.deltaTime;

            // Reset angle to keep it within the range [0, 2 * PI]
            if (angle >= 2 * Mathf.PI)
            {
                angle = 0f;
            }

            if (radius != speed)
            {
                radius += Time.deltaTime * speed;
                radius = Mathf.Clamp(radius, 0f, speed);
            }
        }
    }
}