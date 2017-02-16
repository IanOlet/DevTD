using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour {

    GameManager manager;
    public Vector3 velocity;
    int turn = 0;
    public float lifetime = 0.0f;
    public int health = 15;

	// Use this for initialization
	void Start () {
        velocity = new Vector3(0, -.02f, 0);
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
            velocity = new Vector3(.02f, 0, 0);
        }
        else if (turn == 1 && transform.position.x >= 2.3)
        {
            turn = 2;
            velocity = new Vector3(0, -.02f, 0);
        }
        else if (turn == 2 && transform.position.y <= -0.15)
        {
            turn = 3;
            velocity = new Vector3(-.02f, 0, 0);
        }
        else if (turn == 3 && transform.position.x <= -2.65)
        {
            turn = 4;
            velocity = new Vector3(0, -.02f, 0);
        }
        else if (turn == 4 && transform.position.y <= -2.85)
        {
            turn = 5;
            velocity = new Vector3(.02f, 0, 0);
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
}
