/**
 * Copyright 2020 Jose Javier Perez Rodriguez
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
 using System.Collections.Generic;
 using System.Collections;
 using System.Globalization;
 using System.IO;
 using System;
 using UnityEngine.SceneManagement;
 using UnityEngine;
 using Vuforia;

 public class moveAnnotations : MonoBehaviour { //TEST IS NOT IMPLEMENTED 

     // Start is called before the first frame update

     //Array of Targets objects  
     private List<targetObject> TArray = new List<targetObject> ();

     private targetObject T1;
     private targetObject T2;
     private targetObject T3;

     private GameObject T1Obj;
     private GameObject T2Obj;
     private GameObject T3Obj;

     private Matrix4x4 H_N_N = Matrix4x4.identity; //Empty matrix null

     //Hoff's script
     public Material selectedMaterial; // If object is selected, change its material to this
     private GameObject selectedObject; // Keep track of the selected object
     private Material selectedObjectOriginalMaterial; // Remember selected object's original material
     private bool isAnObjectSelected; // Is true if we have a selected object
     private Transform selectedObjectOriginalParentTransform; // Remember select object's original parent

     void Start () {
         //Loads data first 
         loadData ();
     }

     void Awake () {

     }

     Vector3 positionOffset;
     // Update is called once per frame
     void Update () {

         //GameObject mainCamera = Camera.main.gameObject;
         Vector3 origin = Camera.main.transform.position;
         Vector3 direction = Camera.main.transform.forward;
         Ray ray = new Ray (origin, direction);
         RaycastHit hit;
         bool isThereAHit = Physics.Raycast (ray, out hit);

         if (!getUserTap ()) {

             if (T1.imageTarget.GetComponent<TrackableBehaviour> ().CurrentStatus == TrackableBehaviour.Status.TRACKED && !getUserTap ()) // Found 
             {
                 calculatePosesBasedOnT1 ();

             }
             if (T2.imageTarget.GetComponent<TrackableBehaviour> ().CurrentStatus == TrackableBehaviour.Status.TRACKED) // Found 
             {
                 calculatePosesBasedOnT2 ();

             }

             if (T3.imageTarget.GetComponent<TrackableBehaviour> ().CurrentStatus == TrackableBehaviour.Status.TRACKED) // Found 
             {
                 calculatePosesBasedOnT3 ();

             }

             setObjectPoses ();
             setAnnotations ();

             if (isThereAHit) {
                 selectedObject = hit.collider.gameObject;

                 if (selectedObject != null && selectedObject.transform.childCount > 1 && selectedObject.name == "arrow") {

                     GameObject ChildGameObject1 = selectedObject.transform.GetChild (0).gameObject;
                     GameObject ChildGameObject2 = selectedObject.transform.GetChild (1).gameObject;

                     ChildGameObject1.GetComponent<Renderer> ().material = selectedObjectOriginalMaterial;
                     ChildGameObject2.GetComponent<Renderer> ().material = selectedObjectOriginalMaterial;
                 }
             }

         } else {
             if (isThereAHit) {

                 selectedObject = hit.collider.gameObject;

                 if (selectedObject.transform.childCount > 1 && selectedObject.name == "arrow") {
                     GameObject ChildGameObject1 = selectedObject.transform.GetChild (0).gameObject;
                     GameObject ChildGameObject2 = selectedObject.transform.GetChild (1).gameObject;

                     selectedObjectOriginalMaterial = ChildGameObject1.GetComponent<Renderer> ().material;

                     ChildGameObject1.GetComponent<Renderer> ().material = selectedMaterial;
                     ChildGameObject2.GetComponent<Renderer> ().material = selectedMaterial;

                 }
                 positionOffset = Camera.main.transform.position;

             }
         }
     }

     public void loadData () {

         T1 = new targetObject (GameObject.Find ("MarkerX1"), H_N_N, false);
         T2 = new targetObject (GameObject.Find ("MarkerA2"), H_N_N, false);
         T3 = new targetObject (GameObject.Find ("MarkerQ3"), H_N_N, false);

         TArray.Add (T1);
         TArray.Add (T2);
         TArray.Add (T3);

         //string path = "Assets/Resources/targets.txt";
         //Read the text from directly from the test.txt file

         StreamReader reader = new StreamReader (Application.persistentDataPath + "/targets.txt");

         string imageTargets = reader.ReadToEnd ();

         string[] values = imageTargets.Split ('$');

         int i;
         string position, rotation;
         foreach (var target in TArray) {
             //Debug.Log ("Values length" + values.Length);

             for (i = 4; i < values.Length + 1; i += 4) // for because we read four properties for each
             {
                 Debug.Log ("Value of i :" + i);

                 if (i == 4) {
                     //Debug.Log("Target 1 \n");
                     //Debug.Log(values[i - 3].ToString()); // rotation
                     //PARSE ROTATION
                     rotation = values[i - 3];

                     // //Remove parenthesis
                     rotation = rotation.Remove (0, 1);
                     rotation = rotation.Remove (rotation.Length - 1, 1);
                     string[] singleValue = rotation.Split (',');
                     T1.rotation = new Quaternion (float.Parse (singleValue[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[2], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[3], CultureInfo.InvariantCulture.NumberFormat));

                     //PARSE POSITION
                     position = values[i - 2].ToString ();
                     //Remove parenthesis
                     position = position.Remove (0, 1);
                     position = position.Remove (position.Length - 1, 1);
                     singleValue = position.Split (',');
                     T1.position = new Vector3 (float.Parse (singleValue[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[2], CultureInfo.InvariantCulture.NumberFormat));

                     Debug.Log ("T1 Rotation before loading into matrix: \n");
                     Debug.Log (T1.rotation.ToString ("F8") + "\n");

                     T1.H_Target_to_T1 = Matrix4x4.TRS (T1.position, T1.rotation, new Vector3 (1, 1, 1));

                     Debug.Log ("T1 Rotation after loading into matrix H_T_T1: \n");
                     Debug.Log (T1.H_Target_to_T1.rotation.ToString ("F8") + "\n");
                     //PARSE ANNOTATION
                     T1.annotation = values[i - 1].ToString ();

                 }
                 if (i == 8) {
                     rotation = values[i - 3].ToString ();
                     //Remove parenthesis
                     rotation = rotation.Remove (0, 1);
                     rotation = rotation.Remove (rotation.Length - 1, 1);
                     string[] singleValue = rotation.Split (',');
                     T2.rotation = new Quaternion (float.Parse (singleValue[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[2], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[3], CultureInfo.InvariantCulture.NumberFormat));

                     //PARSE POSITION
                     position = values[i - 2].ToString ();
                     //Remove parenthesis
                     position = position.Remove (0, 1);
                     position = position.Remove (position.Length - 1, 1);
                     singleValue = position.Split (',');
                     T2.position = new Vector3 (float.Parse (singleValue[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[2], CultureInfo.InvariantCulture.NumberFormat));

                     Debug.Log ("T2 Rotation before loading into matrix: \n");
                     Debug.Log (T2.rotation.ToString ("F8") + "\n");

                     T2.H_Target_to_T1 = Matrix4x4.TRS (T2.position, T2.rotation, new Vector3 (1, 1, 1));

                     Debug.Log ("T2 Rotation after loading into matrix H_T_T1: \n");
                     Debug.Log (T2.H_Target_to_T1.rotation.ToString ("F8") + "\n");
                     //PARSE ANNOTATION
                     T2.annotation = values[i - 1].ToString ();

                 }
                 if (i == 12) {
                     rotation = values[i - 3].ToString ();
                     //Remove parenthesis
                     rotation = rotation.Remove (0, 1);
                     rotation = rotation.Remove (rotation.Length - 1, 1);
                     string[] singleValue = rotation.Split (',');
                     T3.rotation = new Quaternion (float.Parse (singleValue[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[2], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[3], CultureInfo.InvariantCulture.NumberFormat));

                     //PARSE POSITION
                     position = values[i - 2].ToString ();
                     //Remove parenthesis
                     position = position.Remove (0, 1);
                     position = position.Remove (position.Length - 1, 1);
                     singleValue = position.Split (',');
                     T3.position = new Vector3 (float.Parse (singleValue[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[2], CultureInfo.InvariantCulture.NumberFormat));

                     Debug.Log ("T3 Rotation before loading into matrix: \n");
                     Debug.Log (T3.rotation.ToString ("F8") + "\n");

                     /////////SETTRS??
                     T3.H_Target_to_T1 = Matrix4x4.TRS (T3.position, T3.rotation, new Vector3 (1, 1, 1));

                     Debug.Log ("T3 Rotation after loading into matrix H_T_T1: \n");
                     Debug.Log (T3.H_Target_to_T1.rotation.ToString ("F8") + "\n");
                     //PARSE ANNOTATION
                     T3.annotation = values[i - 1].ToString ();
                 }
             }

         }
         reader.Close ();

     }

     void calculatePosesBasedOnT1 () {

         //Obtain real location from world
         T1.H_Target_to_World = Matrix4x4.TRS (T1.imageTarget.transform.position, T1.imageTarget.transform.rotation, new Vector3 ((float) 1, (float) 1, (float) 1));

         //Multiply matrices
         T2.H_Target_to_World = T1.H_Target_to_World * T2.H_Target_to_T1;
         T3.H_Target_to_World = T1.H_Target_to_World * T3.H_Target_to_T1;

     }

     void calculatePosesBasedOnT2 () {

         //Obtain real location from world
         T2.H_Target_to_World = Matrix4x4.TRS (T2.imageTarget.transform.position, T2.imageTarget.transform.rotation, new Vector3 ((float) 1, (float) 1, (float) 1));

         //Multiply matrices
         T1.H_Target_to_World = T2.H_Target_to_World * T2.H_Target_to_T1.inverse;
         T3.H_Target_to_World = T1.H_Target_to_World * T3.H_Target_to_T1;

     }

     void calculatePosesBasedOnT3 () {

         //Obtain real location from world
         T3.H_Target_to_World = Matrix4x4.TRS (T3.imageTarget.transform.position, T3.imageTarget.transform.rotation, new Vector3 ((float) 1, (float) 1, (float) 1));

         //Multiply matrices
         T1.H_Target_to_World = T3.H_Target_to_World * T3.H_Target_to_T1.inverse;
         T2.H_Target_to_World = T1.H_Target_to_World * T2.H_Target_to_T1;

     }

     void setObjectPoses () {
         T1Obj = GameObject.Find ("MarkerX1Obj");
         T2Obj = GameObject.Find ("MarkerA2Obj");
         T3Obj = GameObject.Find ("MarkerQ3Obj");

         T1Obj.transform.position = new Vector3 (T1.H_Target_to_World[0, 3], T1.H_Target_to_World[1, 3], T1.H_Target_to_World[2, 3]);
         T1Obj.transform.rotation = T1.H_Target_to_World.rotation;

         T2Obj.transform.position = new Vector3 (T2.H_Target_to_World[0, 3], T2.H_Target_to_World[1, 3], T2.H_Target_to_World[2, 3]);
         T2Obj.transform.rotation = T2.H_Target_to_World.rotation;

         T3Obj.transform.position = new Vector3 (T3.H_Target_to_World[0, 3], T3.H_Target_to_World[1, 3], T3.H_Target_to_World[2, 3]);
         T3Obj.transform.rotation = T3.H_Target_to_World.rotation;

     }

     void setAnnotations () {

         //Arrow length is about 10 cm
         Vector3 arrowLengthOffset = new Vector3 (float.Parse ("0.0", CultureInfo.InvariantCulture.NumberFormat), float.Parse ("0.0", CultureInfo.InvariantCulture.NumberFormat), float.Parse ("0.10", CultureInfo.InvariantCulture.NumberFormat));

         ///////////////////////////////
         //Annotation & Marker 1 /////////////////////////////////
         //////////////////////////////
         //Get annotation composition (Includes empty object pivot)
         GameObject MarkerX1AnnotationComp = GameObject.Find ("MarkerX1AnnotationComp");
         MarkerX1AnnotationComp.transform.position = T1Obj.transform.position;
         MarkerX1AnnotationComp.transform.rotation = T1Obj.transform.rotation;
         //Position get Marker 1 composition (Includes empty object pivot)
         GameObject MarkerX1Comp = GameObject.Find ("MarkerX1Comp");
         MarkerX1Comp.transform.position = T1Obj.transform.position + positionOffset;
         MarkerX1Comp.transform.rotation = T1Obj.transform.rotation;

         //Look at for Annotation 1
         TextMesh MarkerX1Annotation = GameObject.Find ("MarkerX1Annotation").GetComponent<TextMesh> ();
         MarkerX1Annotation.text = "1";
         //  MarkerX1Annotation.transform.position = MarkerX1Comp.transform.position;
         //  //Rotate number
         //  MarkerX1AnnotationComp.transform.rotation = MarkerX1Comp.transform.rotation;

         //Look at makes the arrows always face the camera
         MarkerX1Annotation.transform.LookAt (MarkerX1Annotation.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

         ///////////////////////////////
         //Annotation & Marker 2 /////////////////////////////////
         //////////////////////////////
         //Get annotation composition (Includes empty object pivot)
         GameObject MarkerA2AnnotationComp = GameObject.Find ("MarkerA2AnnotationComp");
         MarkerA2AnnotationComp.transform.position = T2Obj.transform.position;
         MarkerA2AnnotationComp.transform.rotation = T2Obj.transform.rotation;
         //Position get Marker 2 composition (Includes empty object pivot)
         GameObject MarkerA2Comp = GameObject.Find ("MarkerA2Comp");
         MarkerA2Comp.transform.position = T2Obj.transform.position;
         MarkerA2Comp.transform.rotation = T2Obj.transform.rotation;

         TextMesh MarkerA2Annotation = GameObject.Find ("MarkerA2Annotation").GetComponent<TextMesh> ();
         MarkerA2Annotation.text = "2";
         //  MarkerA2Annotation.transform.position = MarkerA2Comp.transform.position;
         //  //Rotate number
         //  MarkerA2Annotation.transform.rotation = MarkerA2Comp.transform.rotation;

         //Look at for Annotation 2
         MarkerA2Annotation.transform.LookAt (MarkerA2Annotation.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

         ///////////////////////////////
         //Annotation & Marker 3 /////////////////////////////////
         //////////////////////////////
         //Get annotation composition (Includes empty object pivot)
         GameObject MarkerQ3AnnotationComp = GameObject.Find ("MarkerQ3AnnotationComp");
         MarkerQ3AnnotationComp.transform.position = T3Obj.transform.position;
         MarkerQ3AnnotationComp.transform.rotation = T3Obj.transform.rotation;
         //Position get Marker 2 composition (Includes empty object pivot)
         GameObject MarkerQ3Comp = GameObject.Find ("MarkerQ3Comp");
         MarkerQ3Comp.transform.position = T3Obj.transform.position;
         MarkerQ3Comp.transform.rotation = T3Obj.transform.rotation;

         TextMesh MarkerQ3Annotation = GameObject.Find ("MarkerQ3Annotation").GetComponent<TextMesh> ();
         MarkerQ3Annotation.text = "3";
         //  MarkerQ3Annotation.transform.rotation = MarkerQ3Comp.transform.rotation;
         //  MarkerQ3Annotation.transform.position = MarkerQ3Comp.transform.position;

         //Look at for Annotation 2
         MarkerQ3Annotation.transform.LookAt (MarkerQ3Annotation.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

     }

     // Get user input - either a tap on the screen, or a tap to the space bar.
     private bool getUserTap () {
         bool isTap = false;

         // Check for a touch (if we have smart phone).
         if (Input.touchCount > 0) {
             // We have a tap on the screen.
             Touch touch = Input.GetTouch (0);
             if (touch.phase == TouchPhase.Began) {
                 Vector2 p = touch.position;

                 // Check if touch position is not too close to the edge of the screen.
                 float fractScreenBorder = 0.3f;
                 if (p.x > fractScreenBorder * Screen.width && p.x < (1 - fractScreenBorder) * Screen.width &&
                     p.y > fractScreenBorder * Screen.height && p.y < (1 - fractScreenBorder) * Screen.height) {
                     isTap = true;
                 }
             }
         } else {
             // Check for keypress.
             isTap = Input.anyKeyDown && Input.GetKey (KeyCode.Space);
         }
         return isTap;
     }

 }