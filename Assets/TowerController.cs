using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour {

    GameManager manager;
    int target;
    int cooldown;
    int range = 5;

    public LayerMask enemyLayer;
    public ProjectileController projectiles;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if(cooldown==0)
        {
            Vector3 target = Vector3.zero;
            float targetTime = 0;
            bool targeted = false;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
            foreach(Collider2D c in colliders)
            {
                BasicEnemyController enemy = c.gameObject.GetComponent<BasicEnemyController>();
                if(enemy.lifetime > targetTime)
                {
                    targetTime = enemy.lifetime;
                    target = c.transform.position;
                    targeted = true;
                }
            }
            if (targeted)
            {
                ProjectileController bullet = Instantiate<ProjectileController>(projectiles, transform.position, transform.rotation);
                bullet.target(target);
                cooldown = 15;
            }
        }
        else
        {
            cooldown--;
        }
		
	}
}
