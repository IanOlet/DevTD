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
    public int wave = 1;
    public int spawncounter = 0;
    public float spawntimer = 0f;
    public bool activeWave = false;
    public bool waveDone = false;

    public TextMesh LivesCounter;
    public TextMesh MoneyCounter;
    public TextMesh WaveCounter;

    bool placementMode = false;
    public GameObject overlay;

    bool selecting = false;
    TowerController selectedTower;

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

        if(Input.GetMouseButtonDown(0)) //Creates and selects towers
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
            else if (placementMode && !pathCheck && !towerCheck && money >= 10)
            {
                Object obj = Instantiate(Tower.gameObject, new Vector3(mousePos.x, mousePos.y, 0), Quaternion.identity);
                TowerController t = ((GameObject)obj).GetComponent<TowerController>();
                Towers.Add(t);
                money -= 10;
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
            if (Input.GetKeyDown(KeyCode.Alpha1) && money >= 5 && !selectedTower.UpRate)
            {
                selectedTower.FireRateUpgrade();
                money -= 5;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && money >= 5 && !selectedTower.UpRange)
            {
                selectedTower.RangeUpgrade();
                money -= 5;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && money >= 5 && !selectedTower.UpDamage)
            {
                selectedTower.DamageUpgrade();
                money -= 5;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                selectedTower.select(false);
                selecting = false;
                Towers.Remove(selectedTower);
                Destroy(selectedTower.gameObject);
                money += 5;
            }
        }

        if(Input.GetKeyDown(KeyCode.Space) && !activeWave) //Starts the current wave
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
            else if (wave >= 5)
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
                else if (spawncounter == 31 + (wave * 2))
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
            money += 10;
            wave++;
        }

        LivesCounter.text = "Lives: " + lives.ToString();
        MoneyCounter.text = "Money: " + money.ToString();
        WaveCounter.text = "Wave " + wave.ToString();
        
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
