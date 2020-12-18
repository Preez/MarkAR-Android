using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animate : MonoBehaviour
{
 Animation yourAnimation;
 
     void Awake(){
         yourAnimation = GameObject.Find("MarkerX1Comp").GetComponent<Animation>();
     }
 
     // This is an example only
 
     void Update(){
        
             yourAnimation.Play("yourAnimationName");
        
     }
}
