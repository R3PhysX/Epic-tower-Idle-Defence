using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [SerializeField] private GameObject ring;

    internal void Set(int DestroyTime)
    {
        StartCoroutine(CheckForEnemy());
        float scale = 2f;
        ring.transform.localScale = Vector3.one * scale;
    }

    IEnumerator CheckForEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.075f);
            List<Enemy> nearestEnemy = new List<Enemy>();

            foreach ((Enemy, UnityPool) enemy in EnemyGenerator.Get.spawnedEnemy)
            {
                float distance = Vector2.Distance(transform.position, enemy.Item1.transform.position);
                if (distance <= 0.3f)
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
                var obj = GameObject.Instantiate(MinesManager.Get.blastParticle.gameObject, pos, Quaternion.identity);

                foreach (Enemy enemy in nearestEnemy)
                {
                    enemy.TakeDamage(40, DamageType.Kill);
                }

                LeanTween.delayedCall(1f, () =>
                {
                    if (obj != null) Destroy(obj);
                });
                Destroy(this.gameObject);
                MinesManager.Get.minesSpawned -= 1;
                AudioManager.Instance?.PlaySFXSound(AudioClipsType.BombSound);
                break;
            }
        }
    }
}   