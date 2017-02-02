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
    
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyDown(KeyCode.Space)) //Test spawns enemies
        {
            Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
            BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
            BasicEnemies.Add(e);
        }

        if(Input.GetMouseButtonDown(0)) //Creates towers
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool pathCheck = Physics.Raycast(new Ray(mousePos, Vector3.forward), 10, pathMask);
            bool towerCheck = Physics2D.Raycast(mouseRay.origin, mouseRay.direction, 10, towerMask);
            if (!pathCheck && !towerCheck)
            {
                Object obj = Instantiate(Tower.gameObject, new Vector3(mousePos.x, mousePos.y, 0), Quaternion.identity);
                TowerController t = ((GameObject)obj).GetComponent<TowerController>();
                Towers.Add(t);
            }
        }

        /*foreach (BasicEnemyController b in BasicEnemies)
        {
            if(b.breakthrough()) //Checks if any enemies reached the end
            {
                
                print("DELETED");
            }
        }*/
	}

    public void removeEnemy(BasicEnemyController b)
    {
        BasicEnemies.Remove(b);
        Destroy(b.gameObject);
    }
    
}
