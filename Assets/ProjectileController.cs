using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {

    float duration;
    public LayerMask enemyLayer;

    double damage;
    int speed;

	// Use this for initialization
	void Start () {
        duration = 3.0f;
	}
	
	// Update is called once per frame
	void Update () {
        duration -= Time.deltaTime;
        if(duration<=0.0f)
        {
            Destroy(gameObject);
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 5, enemyLayer);
        foreach (Collider2D c in colliders)
        {
            BasicEnemyController enemy = c.gameObject.GetComponent<BasicEnemyController>();
            if (enemy.GetComponent<CircleCollider2D>().OverlapPoint(transform.position))
            {
                enemy.damage(damage);
                Destroy(gameObject);
            }
        }
        }


    public void target (Vector3 destination, double d, int s)
    {
        damage = d;
        speed = s;
        float angle = Mathf.Atan2(destination.y - transform.position.y, destination.x - transform.position.x);
        GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle))*speed;
    }


}
