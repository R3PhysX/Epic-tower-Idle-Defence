using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 12;
    [SerializeField] private float damage = 100;
    private Enemy target;
    private Vector3 direction;

    private UnityPool bulletPool;

    internal void Shoot(Enemy target, UnityPool pool)
    {
        this.bulletPool = pool;
        this.target = target;
        if (Vector3.Distance(target.transform.position, Player.Instance.transform.position) < 1f)
            damage = Constants.Get.BulletDamage * (Constants.Get.RangeDamageBonus / 100f);
        else
            damage = Constants.Get.BulletDamage;

        direction = (target.transform.position - transform.position).normalized;

        float time = Vector3.Distance(transform.position, target.transform.position) / 4f;

        LeanTween.move(gameObject, target.transform.position, time).setEase(LeanTweenType.linear);

        LeanTween.delayedCall(Vector3.Distance(transform.position, target.transform.position) / 5f, () => { HandleBulletHit(target); });
        LeanTween.delayedCall(gameObject, 2f, Destroy);
    }

    private void Destroy()
    {
        if (gameObject.activeSelf)
        {
            bulletPool.Add(this);
            LeanTween.cancel(gameObject);
        }
    }

    private void HandleBulletHit(Enemy enemy)
    {
        LeanTween.cancel(gameObject);
        bulletPool.Add(this);

        var randomVal = GameplayManager.Get.randomGenerator.Next(0, 100);

        if (randomVal < Constants.Get.CriticalShotChance)
        {
            var critDamage = (damage * Constants.Get.CriticalShotDamage / 100f);
            enemy.TakeDamage(critDamage, DamageType.Crit);
        }
        else if (randomVal < Constants.Get.CriticalShotChance + Constants.Get.StunChance)
        {
            enemy.TakeDamage(damage, DamageType.Stun);
        }
        else if (randomVal < Constants.Get.CriticalShotChance + Constants.Get.StunChance + Constants.Get.DeadHitChance)
        {
            enemy.TakeDamage(damage, DamageType.DeadHit);
        }
        else
            enemy.TakeDamage(damage);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            bulletPool.Add(this);

            var randomVal = GameplayManager.Get.randomGenerator.Next(0, 100);

            if (randomVal < Constants.Get.CriticalShotChance)
            {
                var critDamage = (damage * Constants.Get.CriticalShotDamage / 100f);
                collision.GetComponent<Enemy>().TakeDamage(critDamage, DamageType.Crit);
            }
            else if (randomVal < Constants.Get.CriticalShotChance + Constants.Get.StunChance)
            {
                collision.GetComponent<Enemy>().TakeDamage(damage, DamageType.Stun);
            }
            else if (randomVal < Constants.Get.CriticalShotChance + Constants.Get.StunChance + Constants.Get.DeadHitChance)
            {
                collision.GetComponent<Enemy>().TakeDamage(damage, DamageType.DeadHit);
            }
            else
                collision.GetComponent<Enemy>().TakeDamage(damage);
        }
    }
}
