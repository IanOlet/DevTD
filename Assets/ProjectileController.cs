using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {

    float duration;
    public LayerMask enemyLayer;

    double damage;
    int speed;

    public Color orange = new Color(1f, 0.6f, 0f, 1f);

    public Sprite projectile;
    public Sprite damageProjectile;
    public Sprite rangeProjectile;
    public Sprite superProjectile;

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
                enemy.damage(damage, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }


    public void target (Vector3 destination, double d, int s, int type)
    {
        //See comment on the type variable in TowerController
        if(type==0)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else if (type == 1)
        {
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else if (type == 2)
        {
            GetComponent<SpriteRenderer>().sprite = rangeProjectile;
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        else if (type == 3)
        {
            GetComponent<SpriteRenderer>().sprite = damageProjectile;
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if (type == 4)
        {
            GetComponent<SpriteRenderer>().sprite = rangeProjectile;
            GetComponent<SpriteRenderer>().color = Color.green;
        }
        else if (type == 5)
        {
            GetComponent<SpriteRenderer>().sprite = damageProjectile;
            GetComponent<SpriteRenderer>().color = Color.magenta;
        }
        else if (type == 6)
        {
            GetComponent<SpriteRenderer>().sprite = superProjectile;
            GetComponent<SpriteRenderer>().color = orange;
        }
        else if (type == 7)
        {
            GetComponent<SpriteRenderer>().sprite = superProjectile;
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        damage = d;
        speed = s;
        float angle = Mathf.Atan2(destination.y - transform.position.y, destination.x - transform.position.x);
        GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle))*speed;
    }


}
