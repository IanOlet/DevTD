using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour {

    // public: 
    [Header("Lists")]
    public List<BasicEnemyController> BasicEnemies;
    public List<TowerController> Towers;

    [Header("LayerMasks")]
    public LayerMask pathMask;
    public LayerMask towerMask;

    [Header("TunableVar")]
    public int lives = 10;

    public int money;
    public int Money
    {
        get { return money; }
        set { ShowMoneyChange(money - value);
            money = value; }
    }

    public int wave = 1;
    public int spawncounter = 0;
    public float spawntimer = 0f;
    public bool activeWave = false;
    public bool waveDone = false;
    public bool debugMode = false;

    [Header("TextObjects")]
    public TextMesh LivesCounter;
    public TextMesh MoneyCounter;
    public TextMesh WaveCounter;
    public TextMesh EnemyCounter;
    public TextMesh ScoreText;
    public Text moneyInd;

    [Header("Prefabs")]
    public GameObject LoseText;
    public GameObject WinText;
    public GameObject ContinueText;
    public GameObject EndShade;
    public GameObject LoseShade;
    public GameObject indicatorTextForUpgrading;
    public GameObject overlay;
    public GameObject Tower;
    public GameObject BasicEnemy;
    public GameObject ResetText;
    public GameObject upgradeText;
    public GameObject rateBox;
    public GameObject rangeBox;
    public GameObject damageBox;
    public GameObject debugIndicator;

    [Header("AudioFiles")]
    public AudioClip ActiveWaveLoop;
    public AudioClip DowntimeLoop;
    public AudioClip FinalWaveLoop;
    public AudioClip modeSwitch;
    public AudioClip towerBuild;
    public AudioClip waveStart;
    public AudioClip lifeLost;
    public AudioClip gameLost;
    public AudioClip sellTower;
    public AudioClip destroySound;
    public AudioClip hitSound;
    public AudioClip endBlips;

    // private:
    bool selecting = false;
    TowerController selectedTower;
    TowerController hoverTower;

    bool placementMode = false;

    float countdown = 0;
    int score = 0;
    int postPhase = 0;

    bool victory = false;
    bool lossSet = false;


    private AudioSource sound;
    private AudioSource music;

    //--------------------------------------------------------------------<
    void Start () {
        var check = GetComponents<AudioSource>();
        money = 40;

        sound = check[0];
        music = check[1];
        music.clip = DowntimeLoop;
        music.Play();
	}

	void Update ()
    {
        TestEnemies();
        CreateTurret();
        ReturnToMenu();
        StartCurrentWave();
        WaveHandling();
    }

    IEnumerator sleep(float time) //Pauses the game for a moment, called when towers are placed and lives are lost
    {
        Time.timeScale = 0.0f;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1.0f;
    }

    private void TestEnemies()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if(debugMode)
            {
                debugMode = false;
                debugIndicator.SetActive(false);
            }
            else
            {
                debugMode = true;
                debugIndicator.SetActive(true);
            }
        }
        if (debugMode)
        {
            if (Input.GetKeyDown(KeyCode.E)) //Test spawns enemies
            {
                GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                BasicEnemyController e = ((GameObject)obj).GetComponent<BasicEnemyController>();
                BasicEnemies.Add(e);
            }
            if (Input.GetKeyDown(KeyCode.W)) //Test spawns fast enemies
            {
                GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
                e.special(1);
                BasicEnemies.Add(e);
            }
            if (Input.GetKeyDown(KeyCode.Q)) //Test spawns tanky enemies
            {
                GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
                e.special(2);
                BasicEnemies.Add(e);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                lives++;
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                money += 10;
            }
        }
    }

    private void CreateTurret()
    {
        if (selecting)
        {
            if (selectedTower.UpRate)
            {
                rateBox.SetActive(true);
            }

            if (selectedTower.UpRange)
            {
                rangeBox.SetActive(true);
            }

            if (selectedTower.UpDamage)
            {
                damageBox.SetActive(true);
            }
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
                //sound.PlayOneShot(modeSwitch, 0.5f);
            }

            if (indicatorTextForUpgrading != null)
            {
                indicatorTextForUpgrading.SetActive(false);
            }
        }
        
        foreach(TowerController t in Towers)
        {
            t.hover(false);
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool pathCheck = Physics.Raycast(new Ray(mousePos, Vector3.forward), 10, pathMask);
        RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction, 10, towerMask);
        bool towerCheck = hit.collider != null;
        
        if(towerCheck)
        {
            hoverTower = hit.collider.gameObject.GetComponent<TowerController>();
            hoverTower.hover(true);
        }


        if (placementMode)  //Checks if there is room to place a tower, changes overlay color to represent this
        {
            if (pathCheck || towerCheck || Money < 10)
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
            if (towerCheck)
            {
                if (selecting)
                {
                    rateBox.SetActive(false);
                    rangeBox.SetActive(false);
                    damageBox.SetActive(false);
                    upgradeText.SetActive(false);
                    selectedTower.select(false);
                }
                selectedTower = hit.collider.gameObject.GetComponent<TowerController>();
                selectedTower.select(true);
                upgradeText.SetActive(true);
                if (selectedTower.UpRate)
                {
                    rateBox.SetActive(true);
                }

                if (selectedTower.UpRange)
                {
                    rangeBox.SetActive(true);
                }

                if (selectedTower.UpDamage)
                {
                    damageBox.SetActive(true);
                }
                selecting = true;
                placementMode = false;
                overlay.SetActive(false);

                // ------------
                if (indicatorTextForUpgrading != null && selectedTower != null)
                {
                    indicatorTextForUpgrading.SetActive(true);
                    Text[] texts = indicatorTextForUpgrading.GetComponentsInChildren<Text>();

                    texts[0].text = "1) Up rate       ";
                    texts[1].text = "2) Up range    ";
                    texts[2].text = "3) Up damage";
                }
                // ------------
                //sound.PlayOneShot(modeSwitch, 0.5f);
            }
            else if (placementMode && !pathCheck && !towerCheck && Money >= 10) //Creates Towers
            {
                GameObject obj = Instantiate(Tower.gameObject, new Vector3(mousePos.x, mousePos.y, 0), Quaternion.identity) as GameObject;
                TowerController t = (obj).GetComponent<TowerController>();
                Towers.Add(t);
                Money -= 10;
                if (selecting)
                {
                    selectedTower.select(false);
                    selecting = false;
                    rateBox.SetActive(false);
                    rangeBox.SetActive(false);
                    damageBox.SetActive(false);
                    upgradeText.SetActive(false);
                }
                sound.PlayOneShot(towerBuild, 0.5f);
                StartCoroutine("sleep", 0.1f);
            }
            else if (selecting)  //Deselects towers if nothing is clicked on
            {
                selectedTower.select(false);
                selecting = false;
                rateBox.SetActive(false);
                rangeBox.SetActive(false);
                damageBox.SetActive(false);
                upgradeText.SetActive(false);
            }
        }

        if (indicatorTextForUpgrading.activeSelf && selectedTower != null)
        {
            Image[] images = indicatorTextForUpgrading.GetComponentsInChildren<Image>();

            images[0].color = selectedTower.UpRate ? Color.red : Color.green;

            images[1].color = selectedTower.UpRange ? Color.red : Color.green;

            images[2].color = selectedTower.UpDamage ? Color.red : Color.green;
        }
    }

    private void ReturnToMenu()
    {
        //Goes back to menu
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("Title");
        }

        if (selecting) //Handles upgrading and selling selected towers
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && Money >= 10 && !selectedTower.UpRate)
            {
                selectedTower.FireRateUpgrade();
                Money -= 10;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && Money >= 10 && !selectedTower.UpRange)
            {
                selectedTower.RangeUpgrade();
                Money -= 10;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && Money >= 10 && !selectedTower.UpDamage)
            {
                selectedTower.DamageUpgrade();
                Money -= 10;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                if (selectedTower.UpDamage)
                {
                    Money += 5;
                }
                if (selectedTower.UpRange)
                {
                    Money += 5;
                }
                if (selectedTower.UpRate)
                {
                    Money += 5;
                }
                indicatorTextForUpgrading.SetActive(false);
                selectedTower.select(false);
                selecting = false;
                Towers.Remove(selectedTower);
                Destroy(selectedTower.gameObject);
                Money += 5;
                sound.PlayOneShot(sellTower, 0.75f);
            }
        }
    }

    private void StartCurrentWave()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !activeWave && !victory) //Starts the current wave
        {
            activeWave = true;
            spawncounter = 0;
            spawntimer = 0;
            waveDone = false;
            //sound.PlayOneShot(waveStart, 0.75f);              Sound for starting wave was a bit jarring
            music.Stop();
            if (wave == 10)
            {
                music.clip = FinalWaveLoop;
            }
            else
            {
                music.clip = ActiveWaveLoop;
            }
            music.Play();
        }
    }

    private void WaveHandling()
    {
        if (activeWave) //Handles enemy spawning during waves. Skip to line 568 if you don't need to change this.
        {

            //Wave Spawnlist
            //wave 1: 16 basic
            //wave 2: 11 basic, 10 basic
            //wave 3: 15 basic, 5 fast
            //wave 4: 5 tanky, 15 basic
            //wave 5: 11 basic, 5 fast, 10 basic, 5 fast, 10 tanky
            //wave 6: 71 basic, 15 tanky
            //wave 7: 21 fast, 20 fast, 20 fast
            //wave 8: 6 tanky, 15 fast, 10 tanky, 20 fast
            //wave 9: 51 basic
            //wave 10: 101 basic, 50 fast, 25 tanky

            if (wave == 1)
            {
                if (spawncounter <= 15 && spawntimer >= 1.5)
                {
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
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
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 10 && spawncounter <= 20 && spawntimer >= 0.7)
                {
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
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
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter == 15 && spawntimer >= 5)
                {
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
                    e.special(1);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 15 && spawncounter <= 19 && spawntimer >= 0.5)
                {
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
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
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
                    e.special(2);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 4 && spawncounter <= 20 && spawntimer >= 0.6)
                {
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
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
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 10 && spawncounter <= 15 && spawntimer >= 0.2)
                {
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
                    e.special(1);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 15 && spawncounter <= 25 && spawntimer >= 0.6)
                {
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 25 && spawncounter <= 30 && spawntimer >= 0.2)
                {
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
                    e.special(1);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 30 && spawncounter <= 30 + (wave * 2) && spawntimer >= 1)
                {
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
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
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 70 && spawncounter <= 85 && spawntimer >= 0.4)
                {
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = obj.GetComponent<BasicEnemyController>();
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
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
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
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity)as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
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
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
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
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
                    e.special(2);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 5 && spawncounter <= 20 && spawntimer >= 0.1)
                {
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
                    e.special(1);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                if (spawncounter > 20 && spawncounter <= 30 && spawntimer >= 0.2)
                {
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
                    e.special(2);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 30 && spawncounter <= 50 && spawntimer >= 0.1)
                {
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
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
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
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
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 100 && spawncounter <= 150 && spawntimer >= 0.1)
                {
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
                    e.special(1);
                    BasicEnemies.Add(e);
                    spawncounter++;
                    spawntimer = 0;
                }
                else if (spawncounter > 150 && spawncounter <= 175 && spawntimer >= 0.1)
                {
                    GameObject obj = Instantiate(BasicEnemy.gameObject, new Vector3(-6.7f, 6.21f, 0), Quaternion.identity) as GameObject;
                    BasicEnemyController e = (obj).GetComponent<BasicEnemyController>();
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

        if (lives <= 0 && !lossSet && !victory)  //Triggers a game-over if all lives are lost
        {
            countdown = 2;
            sound.PlayOneShot(gameLost, 0.5f);
            lossSet = true;
            music.Stop();
        }

        if ((Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.KeypadEnter)) && (victory || lossSet))
        {
            SceneManager.LoadScene("Title");
        }

        if ((Input.GetKey(KeyCode.Space)) && lossSet)
        {
            SceneManager.LoadScene("Level");
        }

        if (countdown > 0) //Deals with the timing for the post game screens
        {
            countdown -= Time.deltaTime;
        }
        else if (postPhase == 1)
        {
            if (lossSet)
            {
                postPhase = 3;
                ResetText.gameObject.SetActive(true);
            }
            else
            {
                ScoreText.gameObject.SetActive(true);
                postPhase = 2;
            }
            sound.PlayOneShot(endBlips);
            countdown = 0.5f;
        }
        else if (postPhase == 2)
        {
            ScoreText.text = "Score: " + score;
            if (Money > 0)
            {
                sound.PlayOneShot(hitSound, 0.5f);
                Money--;
                score++;
            }
            else if (Money <= 0 && lives > 0)
            {
                sound.PlayOneShot(sellTower, 0.3f);
                lives--;
                score += 10;
                countdown = 0.3f;
            }
            else if (Money <= 0 && lives <= 0)
            {
                postPhase = 3;
                countdown = 1;
            }
        }
        else if (postPhase == 3)
        {
            sound.PlayOneShot(endBlips);
            ContinueText.SetActive(true);
            postPhase = 4;
        }
        else if (postPhase == 4)
        {
            //Do nothing
        }
        else if (lossSet && countdown <= 0)
        {
            sound.PlayOneShot(endBlips);
            LoseShade.SetActive(true);
            LoseText.SetActive(true);
            postPhase = 1;
            countdown = 1.5f;
        }
        else if (victory && countdown <= 0)
        {
            sound.PlayOneShot(endBlips);
            EndShade.SetActive(true);
            WinText.SetActive(true);
            postPhase = 1;
            countdown = 1.5f;
        }

        if (activeWave && waveDone && BasicEnemies.Count == 0) //Determines if the wave is done
        {
            activeWave = false;
            Money += 10;
            if (wave == 10) //Checks if the game is won, possibly initiate a scene change here
            {
                victory = true;
                countdown = 2;
            }
            else
            {
                wave++;
            }
            music.Stop();
            if (!victory && !lossSet)
            {
                music.clip = DowntimeLoop;
                music.Play();
            }
        }

        //Handles UI text
        LivesCounter.text = "Lives: " + lives.ToString();
        MoneyCounter.text = "Money: " + Money.ToString();
        WaveCounter.text = "Wave " + wave.ToString();
        if (activeWave)
        {
            EnemyCounter.text = "Enemies: " + BasicEnemies.Count;
        }
        else if (wave >= 10 || lossSet)
        {
            EnemyCounter.text = "";
        }
        else
        {
            EnemyCounter.text = "Press Space to begin wave";
        }
    }

    public void removeEnemy(BasicEnemyController b, bool breakthrough) //Removes enemies that are destroyed or reach the end
    {
        if (breakthrough && !lossSet)
        {
            lives--;
            StartCoroutine("sleep", 0.2f);
            if (lives > 0)
            {
                sound.PlayOneShot(lifeLost);
            }
        }
        else if (!lossSet)
        {
            Money += 1;
            sound.PlayOneShot(destroySound, 0.75f);
        }
        BasicEnemies.Remove(b);
        Destroy(b.gameObject);
    }

    private void ShowMoneyChange(int i)
    {
        
        StartCoroutine("showMoneyChangeRoutine", i);
    }

    IEnumerator showMoneyChangeRoutine(int i)
    {
        Vector3 pos = moneyInd.transform.position;
        float time = 50f;

        if(moneyInd!= null)
        {
            moneyInd.gameObject.SetActive(true);
            moneyInd.text = (i > 0)? "-" : "" + i.ToString();
            moneyInd.rectTransform.position = pos;
        }

        while (time > 0)
        {
            moneyInd.rectTransform.position += new Vector3(0f, 3f, 0f);
            time--;
            yield return null;
        }

        moneyInd.gameObject.SetActive(false);
        time = 20;
        moneyInd.text = "";
        moneyInd.rectTransform.position = pos;
    }
}
