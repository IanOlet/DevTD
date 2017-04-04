using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour {

    GameManager manager;
    public Sprite Basic;
    public Sprite Dart;
    public Sprite Brute;
    public GameObject BasicParticle;
    public GameObject FastParticle;
    public GameObject HeavyParticle;
    public GameObject BasicDeadParticles;
    public GameObject FastDeadParticles;
    public GameObject HeavyDeadParticles;

    public Vector3 velocity;
    int turn = 0;
    public float lifetime = 0.0f;
    public double health = 15f;
    public int specialty = 0;
    int type = 0;

    float speed = 1.2f;
    float negSpeed = -1.2f;

    public AudioClip hitSound;
    public AudioClip armorSound;

    private AudioSource sound;

	// Use this for initialization
	void Start () {
        sound = GetComponent<AudioSource>();
        velocity = new Vector3(0, negSpeed, 0);
        manager = GameObject.FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 where = transform.position;
        where += velocity*Time.deltaTime;
        transform.position = where;

        if (health <= 0)
        {
            if (type == 0)
            {
                Instantiate(BasicDeadParticles, transform.position, Quaternion.identity);
            }
            else if (type == 1)
            {
                Instantiate(FastDeadParticles, transform.position, Quaternion.identity);
            }
            else if (type == 2)
            {
                Instantiate(HeavyDeadParticles, transform.position, Quaternion.identity);
            }
            manager.removeEnemy(this, false);
        }

        if (turn == 0 && transform.position.y <= 2.4)
        {
            turn = 1;
            velocity = new Vector3(speed, 0, 0);
        }
        else if (turn == 1 && transform.position.x >= 2.3)
        {
            turn = 2;
            velocity = new Vector3(0, negSpeed, 0);
        }
        else if (turn == 2 && transform.position.y <= -0.15)
        {
            turn = 3;
            velocity = new Vector3(negSpeed, 0, 0);
        }
        else if (turn == 3 && transform.position.x <= -2.65)
        {
            turn = 4;
            velocity = new Vector3(0, negSpeed, 0);
        }
        else if (turn == 4 && transform.position.y <= -2.85)
        {
            turn = 5;
            velocity = new Vector3(speed, 0, 0);
        }


        if (transform.position.x > 11.5 && turn == 5)
        {
            manager.removeEnemy(this, true);
        }
        

        lifetime += (Time.deltaTime*speed);

    }

    public bool breakthrough()
    {
        if (transform.position.x > 11.5 && turn == 5)
        {
            manager.removeEnemy(this, true);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void damage(double impact, Vector3 pos, Quaternion rotation)
    {
        Quaternion rot = Quaternion.Euler(-rotation.eulerAngles);
        if (specialty == 2)
        {
            impact -= 0.5;
        }
        if (impact <= 1 && type == 2)
        {
            sound.PlayOneShot(armorSound, 0.5f);
        }
        else
        {
            sound.PlayOneShot(hitSound, 0.5f);
        }
        health -= impact;
        if (type == 0)
        {
            Instantiate(BasicParticle, pos, rot);
        }
        else if (type == 1)
        {
            Instantiate(FastParticle, pos, rot);
        }
        else if (type == 2)
        {
            Instantiate(HeavyParticle, pos, rot);
        }
    }

    public void special(int t) //Gives the enemy special stats, 1 is fast, 2 is tanky
    {
        if(t==1)
        {
            type = 1;
            GetComponent<SpriteRenderer>().sprite = Dart;
            speed = 3f;
            negSpeed = -3f;
            health = 12f;
            velocity = new Vector3(0, negSpeed, 0);
        }
        if(t==2)
        {
            type = 2;
            GetComponent<SpriteRenderer>().sprite = Brute;
            speed = 1f;
            negSpeed = -1f;
            health = 100f;
            velocity = new Vector3(0, negSpeed, 0);
        }
    }
}
