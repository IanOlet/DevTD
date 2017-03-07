using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour {

    GameManager manager;
    int target;
    int cooldown;  //Delay between shots
    int range = 3;  //Range in Unity Units
    double damage = 1;
    int speed = 10;

    public bool UpRate = false;
    public bool UpRange = false;
    public bool UpDamage = false;

    public LayerMask enemyLayer;
    public ProjectileController projectiles;

    public GameObject rangeIndicator;
    public GameObject upgradedIndicator;

    public Color orange = new Color(1f, 0.5f, 0f, 1f);

	// Use this for initialization
	void Start () {
        GetComponent<SpriteRenderer>().color = Color.grey;
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
                bullet.target(target, damage, speed);
                if (UpRate)
                {
                    cooldown = 15;
                }
                else
                {
                    cooldown = 30;
                }
            }
        }
        else
        {
            cooldown--;
        }
		
	}

    public void FireRateUpgrade()
    {
        UpRate = true;
        if (UpRange && UpDamage)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else if (UpRange)
        {
            GetComponent<SpriteRenderer>().color = Color.green;
        }
        else if (UpDamage)
        {
            GetComponent<SpriteRenderer>().color = Color.magenta;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

    public void RangeUpgrade()
    {
        range = 6;
        UpRange = true;
        speed = 20;
        rangeIndicator.SetActive(false);
        upgradedIndicator.SetActive(true);
        if (UpRate && UpDamage)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else if (UpRate)
        {
            GetComponent<SpriteRenderer>().color = Color.green;
        }
        else if (UpDamage)
        {
            GetComponent<SpriteRenderer>().color = orange;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    }

    public void DamageUpgrade()
    {
        damage = 2;
        UpDamage = true;
        if (UpRange && UpRate)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else if (UpRange)
        {
            GetComponent<SpriteRenderer>().color = orange;
        }
        else if (UpRate)
        {
            GetComponent<SpriteRenderer>().color = Color.magenta;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public void select(bool activation)
    {
        if(activation)
        {
            if(UpRange)
            {
                upgradedIndicator.SetActive(true);
            }
            else
            {
                rangeIndicator.SetActive(true);
            }
        }
        else
        {
            if (UpRange)
            {
                upgradedIndicator.SetActive(false);
            }
            else
            {
                rangeIndicator.SetActive(false);
            }
        }
    }
}
