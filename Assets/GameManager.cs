using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject BasicEnemy;
    public List<BasicEnemyController> BasicEnemies;

    public GameObject Tower;
    public List<TowerController> Towers;

    public LayerMask pathMask;
    public LayerMask towerMask;

    public int lives = 10;
    public int money = 50;

    public TextMesh LivesCounter;
    public TextMesh MoneyCounter;

    bool placementMode = false;
    public GameObject overlay;

    public GameObject LoseText;
    
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyDown(KeyCode.E)) //Test spawns enemies
        {
            Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
            BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
            BasicEnemies.Add(e);
        }
        if (Input.GetKeyDown(KeyCode.Space)) //Turns on placement overlay
        {
            if (placementMode)
            {
                overlay.SetActive(false);
                placementMode = false;
            }
            else
            {
                overlay.SetActive(true);
                placementMode = true;
            }
        }

        if(Input.GetMouseButtonDown(0)) //Creates towers
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool pathCheck = Physics.Raycast(new Ray(mousePos, Vector3.forward), 10, pathMask);
            bool towerCheck = Physics2D.Raycast(mouseRay.origin, mouseRay.direction, 10, towerMask);
            if (!pathCheck && !towerCheck && money >= 10)
            {
                Object obj = Instantiate(Tower.gameObject, new Vector3(mousePos.x, mousePos.y, 0), Quaternion.identity);
                TowerController t = ((GameObject)obj).GetComponent<TowerController>();
                Towers.Add(t);
                money -= 10;
            }
        }

        if (lives <= 0)  //Add Game Over Screen
        {
            LoseText.SetActive(true);
        }

        LivesCounter.text = "Lives: " + lives.ToString();
        MoneyCounter.text = "Money: " + money.ToString();

        /*foreach (BasicEnemyController b in BasicEnemies)
        {
            if(b.breakthrough()) //Checks if any enemies reached the end
            {
                
                print("DELETED");
            }
        }*/
    }

    public void removeEnemy(BasicEnemyController b, bool breakthrough)
    {
        if (breakthrough)
        {
            lives--;
        }
        else
        {
            money += 1;
        }
        BasicEnemies.Remove(b);
        Destroy(b.gameObject);
    }
    
}
