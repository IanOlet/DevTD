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
    public int money = 60;
    public int wave = 1;
    public int spawncounter = 0;
    public float spawntimer = 0f;
    public bool activeWave = false;
    public bool waveDone = false;
    bool victory = false;

    public TextMesh LivesCounter;
    public TextMesh MoneyCounter;
    public TextMesh WaveCounter;
    public TextMesh EnemyCounter;
    public GameObject LoseText;
    public GameObject WinText;

    bool placementMode = false;
    public GameObject overlay;

    bool selecting = false;
    TowerController selectedTower;
    
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
        if (Input.GetKeyDown(KeyCode.W)) //Test spawns fast enemies
        {
            Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
            BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
            e.special(1);
            BasicEnemies.Add(e);
        }
        if (Input.GetKeyDown(KeyCode.Q)) //Test spawns tanky enemies
        {
            Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
            BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
            e.special(2);
            BasicEnemies.Add(e);
        }

        if (Input.GetMouseButtonDown(1)) //Turns on placement overlay
        {
            if (placementMode)
            {
                overlay.SetActive(false);
                placementMode = false;
            }
            else
            {
                if (selecting)
                {
                    selectedTower.select(false);
                    selecting = false;
                }
                overlay.SetActive(true);
                placementMode = true;
            }
        }

        if (placementMode)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool pathCheck = Physics.Raycast(new Ray(mousePos, Vector3.forward), 10, pathMask);
            RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction, 10, towerMask);
            bool towerCheck = hit.collider != null;

            if (pathCheck || towerCheck || money < 20)
            {
                overlay.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                overlay.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }

        if (Input.GetMouseButtonDown(0)) //Creates and selects towers
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool pathCheck = Physics.Raycast(new Ray(mousePos, Vector3.forward), 10, pathMask);
            RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction, 10, towerMask);
            bool towerCheck = hit.collider != null;
            if (towerCheck)
            {
                if (selecting)
                {
                    selectedTower.select(false);
                }
                selectedTower = hit.collider.gameObject.GetComponent<TowerController>();
                selectedTower.select(true);
                selecting = true;
                placementMode = false;
                overlay.SetActive(false);
            }
            else if (placementMode && !pathCheck && !towerCheck && money >= 20)
            {
                Object obj = Instantiate(Tower.gameObject, new Vector3(mousePos.x, mousePos.y, 0), Quaternion.identity);
                TowerController t = ((GameObject)obj).GetComponent<TowerController>();
                Towers.Add(t);
                money -= 20;
                if (selecting)
                {
                    selectedTower.select(false);
                    selecting = false;
                }
            }
            else if(selecting)
            {
                selectedTower.select(false);
                selecting = false;
            }
        }

        if (selecting) //Handles upgrading and selling selected towers
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && money >= 10 && !selectedTower.UpRate)
            {
                selectedTower.FireRateUpgrade();
                money -= 10;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && money >= 10 && !selectedTower.UpRange)
            {
                selectedTower.RangeUpgrade();
                money -= 10;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && money >= 10 && !selectedTower.UpDamage)
            {
                selectedTower.DamageUpgrade();
                money -= 10;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                if(selectedTower.UpDamage)
                {
                    money += 5;
                }
                if(selectedTower.UpRange)
                {
                    money += 5;
                }
                if(selectedTower.UpRate)
                {
                    money += 5;
                }
                selectedTower.select(false);
                selecting = false;
                Towers.Remove(selectedTower);
                Destroy(selectedTower.gameObject);
                money += 10;
            }
        }

        if(Input.GetKeyDown(KeyCode.Space) && !activeWave && !victory) //Starts the current wave
        {
            activeWave = true;
            spawncounter = 0;
            spawntimer = 0;
            waveDone = false;
        }

        if(activeWave) //Handles enemy spawning during waves
        {
            if (wave == 1)
            {
                if (spawncounter <= 15 && spawntimer >= 1.5)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter == 16)
                {
                    waveDone = true;
                }
            }
            else if (wave == 2)
            {
                if (spawncounter <= 10 && spawntimer >= 1)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 10 && spawncounter <= 20 && spawntimer >= 0.7)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter == 21)
                {
                    waveDone = true;
                }
            }
            else if (wave == 3)
            {
                if (spawncounter < 15 && spawntimer >= 0.8)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter == 15 && spawntimer >= 5)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    e.special(1);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 15 && spawncounter <= 19 && spawntimer >= 0.5)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    e.special(1);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter == 20)
                {
                    waveDone = true;
                }
            }
            else if (wave == 4)
            {
                if (spawncounter <= 4 && spawntimer >= 3)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    e.special(2);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 4 && spawncounter <= 20 && spawntimer >= 0.6)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter == 21)
                {
                    waveDone = true;
                }
            }
            else if (wave == 5)
            {
                if (spawncounter <= 10 && spawntimer >= 0.6)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 10 && spawncounter <= 15 && spawntimer >= 0.2)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    e.special(1);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 15 && spawncounter <= 25 && spawntimer >= 0.6)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 25 && spawncounter <= 30 && spawntimer >= 0.2)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    e.special(1);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 30 && spawncounter <= 30 + (wave * 2) && spawntimer >= 1)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    e.special(2);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter == 41)
                {
                    waveDone = true;
                }
            }
            else if (wave == 6)
            {
                if (spawncounter <= 70 && spawntimer >= 0.15)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 70 && spawncounter <= 85 && spawntimer >= 0.4)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    e.special(2);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 85)
                {
                    waveDone = true;
                }
            }
            else if (wave == 7)
            {
                if (spawncounter <= 20 && spawntimer >= 0.1)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    e.special(1);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 20 && spawncounter <= 21 && spawntimer >= 3)
                {
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 21 && spawncounter <= 41 && spawntimer >= 0.1)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    e.special(1);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 41 && spawncounter <= 42 && spawntimer >= 3)
                {
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 42 && spawncounter <= 62 && spawntimer >= 0.1)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    e.special(1);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 62)
                {
                    waveDone = true;
                }
            }
            else if (wave == 8)
            {
                if (spawncounter <= 5 && spawntimer >= 0.2)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    e.special(2);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 5 && spawncounter <= 20 && spawntimer >= 0.1)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    e.special(1);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                if (spawncounter > 20 && spawncounter <= 30 && spawntimer >= 0.2)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    e.special(2);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 30 && spawncounter <= 50 && spawntimer >= 0.1)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    e.special(1);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 50)
                {
                    waveDone = true;
                }
            }
            else if (wave == 9)
            {
                if (spawncounter <= 50 && spawntimer >= 0.05)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    e.special(1);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 50)
                {
                    waveDone = true;
                }
            }
            else if (wave == 10)
            {
                if (spawncounter <= 100 && spawntimer >= 0.1)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 100 && spawncounter <= 150 && spawntimer >= 0.1)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    e.special(1);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 150 && spawncounter <= 175 && spawntimer >= 0.1)
                {
                    Object obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity);
                    BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                    e.special(2);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 175)
                {
                    waveDone = true;
                }
            }

            spawntimer += Time.deltaTime;
        }

        if (lives <= 0)  //Triggers a game-over if all lives are lost
        {
            LoseText.SetActive(true);
        }

        if (activeWave && waveDone && BasicEnemies.Count==0)
        {
            activeWave = false;
            money += 20;
            if (wave == 10)
            {
                victory = true;
                WinText.SetActive(true);
            }
            else
            {
                wave++;
            }
        }

        LivesCounter.text = "Lives: " + lives.ToString();
        MoneyCounter.text = "Money: " + money.ToString();
        WaveCounter.text = "Wave " + wave.ToString();
        EnemyCounter.text = "Enemies: " + BasicEnemies.Count;
        
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
