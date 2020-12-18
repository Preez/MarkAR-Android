using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animateNewPullArrow : MonoBehaviour {
    Animator animator;
    private bool move;

    // Start is called before the first frame update
    void Start () {
        animator = GetComponent<Animator> ();
        move = true;
    }

    // Update is called once per frame
    void Update () {

        animator.SetBool ("move", move); // Set the Animator parameter named "move"
        // Debug.Log(move);    // uncomment to print value of move

    }

}