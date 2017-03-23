using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    Animator eyesAnim;

	// Use this for initialization
	void Start () {
        eyesAnim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxis("Horizontal") != 0)
        {
            eyesAnim.Play("EyesWalk");
        }
        else
        {
            eyesAnim.Play("EyesIdle");
        }
	}
}
