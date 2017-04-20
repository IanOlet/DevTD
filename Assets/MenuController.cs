using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    public LayerMask pathMask;
    public LayerMask towerMask;

    public GameObject startText;
    public GameObject learnText;
    public GameObject startHighlight;
    public GameObject learnHighlight;
    public GameObject information;
    public GameObject title;

    bool learning = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space) ||Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) //Loads the game when the key is pressed
        {
            SceneManager.LoadScene("Level");
        }
        if (!learning)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction, 10, pathMask);
            bool pathCheck = hit.collider != null;
            RaycastHit2D hit2 = Physics2D.Raycast(mouseRay.origin, mouseRay.direction, 10, towerMask);
            bool towerCheck = hit2.collider != null;

            if (pathCheck)
            {
                startHighlight.SetActive(true);
            }
            else
            {
                startHighlight.SetActive(false);
            }

            if (towerCheck)
            {
                learnHighlight.SetActive(true);
            }
            else
            {
                learnHighlight.SetActive(false);
            }

            if (Input.GetMouseButton(0))
            {
                if (pathCheck)
                {
                    SceneManager.LoadScene("Level");
                }
                else if (towerCheck)
                {
                    title.SetActive(false);
                    information.SetActive(true);
                }
            }
        }
    }
}
