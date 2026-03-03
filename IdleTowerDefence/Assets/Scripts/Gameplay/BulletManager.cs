using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform bulletContainer;

    private UnityPool bulletPool;

    private void Start()
    {
        bulletPool = new UnityPool(bulletPrefab.gameObject, 20, bulletContainer);
    }

    internal void ShootAt(Enemy target)
    {
        Bullet bullet = bulletPool.Get<Bullet>(bulletContainer);
        AudioManager.Instance?.PlaySFXSound(AudioClipsType.Bullet);
        //  Bullet bullet = Instantiate(bulletPrefab.gameObject, bulletContainer.parent).GetComponent<Bullet>();
        bullet.transform.position = transform.position;
        bullet.Shoot(target, bulletPool);
    }
}
