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
    int type = 0; //Int to keep track of what upgrade combination the tower is
    //0 is basic
    //1 is firerate
    //2 is range
    //3 is damage
    //4 is firerate/range
    //5 is firerate/damage
    //6 is range/damage
    //7 is all upgrades

    public bool UpRate = false;
    public bool UpRange = false;
    public bool UpDamage = false;

    public LayerMask enemyLayer;
    public ProjectileController projectiles;

    public GameObject rangeIndicator;
    public GameObject upgradedIndicator;
    public GameObject orbital;
    public GameObject highlight;

    public Sprite blueTower;
    public Sprite yellowTower;
    public Sprite redTower;
    public Sprite orangeTower;
    public Sprite greenTower;
    public Sprite violetTower;
    public Sprite superTower;

    public AudioClip SpeedSound;
    public AudioClip RangeSound;
    public AudioClip PowerSound;
    public AudioClip SuperSound;

    private AudioSource sound;

    public Color orange = new Color(1f, 0.6f, 0f, 1f);

	// Use this for initialization
	void Start () {
        GetComponent<SpriteRenderer>().color = Color.grey;
        sound = GetComponent<AudioSource>();
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
                bullet.target(target, damage, speed, type);
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
            orbital.GetComponent<SpriteRenderer>().color = Color.white;
            highlight.GetComponent<SpriteRenderer>().color = Color.white;
            GetComponent<SpriteRenderer>().sprite = superTower;
            type = 7;
            sound.PlayOneShot(SuperSound);
        }
        else if (UpRange)
        {
            orbital.GetComponent<SpriteRenderer>().color = Color.green;
            highlight.GetComponent<SpriteRenderer>().color = Color.green;
            GetComponent<SpriteRenderer>().sprite = greenTower;
            type = 4;
            sound.PlayOneShot(SpeedSound);
        }
        else if (UpDamage)
        {
            orbital.GetComponent<SpriteRenderer>().color = Color.magenta;
            highlight.GetComponent<SpriteRenderer>().color = Color.magenta;
            GetComponent<SpriteRenderer>().sprite = violetTower;
            type = 5;
            sound.PlayOneShot(SpeedSound);
        }
        else
        {
            orbital.GetComponent<SpriteRenderer>().color = Color.blue;
            highlight.GetComponent<SpriteRenderer>().color = Color.blue;
            GetComponent<SpriteRenderer>().sprite = blueTower;
            type = 1;
            sound.PlayOneShot(SpeedSound);
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
            orbital.GetComponent<SpriteRenderer>().color = Color.white;
            highlight.GetComponent<SpriteRenderer>().color = Color.white;
            GetComponent<SpriteRenderer>().sprite = superTower;
            type = 7;
            sound.PlayOneShot(SuperSound);
        }
        else if (UpRate)
        {
            orbital.GetComponent<SpriteRenderer>().color = Color.green;
            highlight.GetComponent<SpriteRenderer>().color = Color.green;
            GetComponent<SpriteRenderer>().sprite = greenTower;
            type = 4;
            sound.PlayOneShot(RangeSound);
        }
        else if (UpDamage)
        {
            orbital.GetComponent<SpriteRenderer>().color = orange;
            highlight.GetComponent<SpriteRenderer>().color = orange;
            GetComponent<SpriteRenderer>().sprite = orangeTower;
            type = 6;
            sound.PlayOneShot(RangeSound);
        }
        else
        {
            orbital.GetComponent<SpriteRenderer>().color = Color.yellow;
            highlight.GetComponent<SpriteRenderer>().color = Color.yellow;
            GetComponent<SpriteRenderer>().sprite = yellowTower;
            type = 2;
            sound.PlayOneShot(RangeSound);
        }
    }

    public void DamageUpgrade()
    {
        damage = 2;
        UpDamage = true;
        if (UpRange && UpRate)
        {
            orbital.GetComponent<SpriteRenderer>().color = Color.white;
            highlight.GetComponent<SpriteRenderer>().color = Color.white;
            GetComponent<SpriteRenderer>().sprite = superTower;
            type = 7;
            sound.PlayOneShot(SuperSound);
        }
        else if (UpRange)
        {
            orbital.GetComponent<SpriteRenderer>().color = orange;
            highlight.GetComponent<SpriteRenderer>().color = orange;
            GetComponent<SpriteRenderer>().sprite = orangeTower;
            type = 6;
            sound.PlayOneShot(PowerSound);
        }
        else if (UpRate)
        {
            orbital.GetComponent<SpriteRenderer>().color = Color.magenta;
            highlight.GetComponent<SpriteRenderer>().color = Color.magenta;
            GetComponent<SpriteRenderer>().sprite = violetTower;
            type = 5;
            sound.PlayOneShot(PowerSound);
        }
        else
        {
            orbital.GetComponent<SpriteRenderer>().color = Color.red;
            highlight.GetComponent<SpriteRenderer>().color = Color.red;
            GetComponent<SpriteRenderer>().sprite = redTower;
            type = 3;
            sound.PlayOneShot(PowerSound);
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
