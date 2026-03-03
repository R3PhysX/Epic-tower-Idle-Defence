using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Get;

    [SerializeField] private ParticleSystem deadParticle;

    private UnityPool pool;
    
    private void Awake()
    {
        Get = this;
    }

    private void Start()
    {
        pool = new UnityPool(deadParticle.gameObject, 10, transform);
    }

    internal void ShowDeadParticle(Vector3 position, Color color)
    {
        if (ActiveGameData.Instance.saveData.VisualEffect == 0)
            return;

        var obj = pool.Get<ParticleSystem>(transform);
        obj.transform.position = position;
        var main = obj.main;
        main.startColor = color;

        LeanTween.delayedCall(2.5f, ()=> {
            pool.Add(obj);
        });
    }
}
