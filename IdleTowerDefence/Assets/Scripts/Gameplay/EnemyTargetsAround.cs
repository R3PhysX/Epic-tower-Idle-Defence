using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetsAround : MonoBehaviour
{

    public List<Enemy> friendlyUnits;

    private Enemy owner;
    public CircleCollider2D cc;
    // Start is called before the first frame update

    private void Awake()
    {
        owner = GetComponentInParent<Enemy>();
        cc = GetComponent<CircleCollider2D>();
    }
    void Start()
    {
        friendlyUnits = new List<Enemy>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        if (!other.gameObject.activeInHierarchy)
            return;

        //Debug.Log(other);
        if (other.transform.gameObject.CompareTag("Enemy"))
        {
            Enemy unit = other.GetComponent<Enemy>();
            if (unit != null) //&& unit != owner
            {
                friendlyUnits.Add(unit);

            }

        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.gameObject.CompareTag("Enemy"))
        {
            Enemy unit = other.GetComponent<Enemy>();
            if (unit != null)
            {

                friendlyUnits.Remove(unit);

            }

        }
    }

    // Update is called once per frame
    void LateUpdate()
    {

        foreach (Enemy unit in friendlyUnits.ToArray())
        {
            if (unit == null || !unit.gameObject.activeSelf)
            {
                friendlyUnits.Remove(unit);
            }

        }

    }
}
