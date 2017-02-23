using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour {

    GameManager manager;
    public Sprite Basic;
    public Sprite Dart;
    public Sprite Brute;

    public Vector3 velocity;
    int turn = 0;
    public float lifetime = 0.0f;
    public int health = 15;

    float speed = 0.02f;
    float negSpeed = -0.02f;

	// Use this for initialization
	void Start () {
        velocity = new Vector3(0, negSpeed, 0);
        manager = GameObject.FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 where = transform.position;
        where += velocity;
        transform.position = where;

        if (health <= 0)
        {
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
        

        lifetime += Time.deltaTime;

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

    public void damage(int impact)
    {
        health -= impact;
    }

    public void special(int type) //Gives the enemy special stats, 1 is fast, 2 is tanky
    {
        if(type==1)
        {
            GetComponent<SpriteRenderer>().sprite = Dart;
            speed = 0.08f;
            negSpeed = -0.08f;
            health = 10;
            velocity = new Vector3(0, negSpeed, 0);
        }
        if(type==2)
        {
            GetComponent<SpriteRenderer>().sprite = Brute;
            speed = 0.015f;
            negSpeed = -0.015f;
            health = 70;
            velocity = new Vector3(0, negSpeed, 0);
        }
    }
}
