 using System.Collections.Generic;
 using System.Collections;
 using System.Globalization;
 using System.IO;
 using System;
 using UnityEngine.SceneManagement;
 using UnityEngine;
 using Vuforia;

 // Load and process has to be its own class since start and update get called in the Handler.cs classs, affecting the values.
 public class loadAndProcessHandler : MonoBehaviour {
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

     public static int PROPERTIES_NUM = 4; // Rotation, Position, Annotation, and Marker number

     void Start () {
         loadData ();
     }

     void Awake () {

     }
     // Update is called once per frame
     void Update () {

         if (T1 != null && T1.imageTarget.GetComponent<TrackableBehaviour> ().CurrentStatus == TrackableBehaviour.Status.TRACKED) // Found 
         {
             calculatePosesBasedOnT1 ();
             setObjectPoses ();
         }

         if (T2 != null && T2.imageTarget.GetComponent<TrackableBehaviour> ().CurrentStatus == TrackableBehaviour.Status.TRACKED) // Found 
         {
             calculatePosesBasedOnT2 ();
             setObjectPoses ();
         }

         if (T3 != null && T3.imageTarget.GetComponent<TrackableBehaviour> ().CurrentStatus == TrackableBehaviour.Status.TRACKED) // Found 
         {
             calculatePosesBasedOnT3 ();
             setObjectPoses ();
         }

     }

     public void loadData () {

         //string path = "Assets/Resources/targets.txt";
         //Read the text from directly from the test.txt file

         //  StreamReader reader = new StreamReader (Application.persistentDataPath + "/targets.txt");

         //  string imageTargets = reader.ReadToEnd ();

         string imageTargets = "MarkerX1$(-0.69863330, -0.03082720, 0.09498671, 0.70847630)$(0.02588051, 0.07055369, -62.37314000)$Annotation ONE GoesHere";

         string[] values = imageTargets.Split ('$');

         string position, rotation;

         int counter = PROPERTIES_NUM;

         // Adds only the corresponding markers for the respected values
         if (values.Length >= 4) {
             T1 = new targetObject (GameObject.Find ("MarkerX1"), H_N_N, false);
             TArray.Add (T1);
         } else if (values.Length >= 8) {
             T2 = new targetObject (GameObject.Find ("MarkerA2"), H_N_N, false);
             TArray.Add (T2);
         } else if (values.Length >= 12) {
             T3 = new targetObject (GameObject.Find ("MarkerQ3"), H_N_N, false);
             TArray.Add (T3);
         }

 
         foreach (var target in TArray) {
             //PARSE ROTATION
             rotation = values[counter - 3];

             // //Remove parenthesis
             rotation = rotation.Remove (0, 1);
             rotation = rotation.Remove (rotation.Length - 1, 1);
             string[] singleValue = rotation.Split (',');
             target.rotation = new Quaternion (float.Parse (singleValue[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[2], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[3], CultureInfo.InvariantCulture.NumberFormat));

             //PARSE POSITION
             position = values[counter - 2].ToString ();
             //Remove parenthesis
             position = position.Remove (0, 1);
             position = position.Remove (position.Length - 1, 1);
             singleValue = position.Split (',');
             target.position = new Vector3 (float.Parse (singleValue[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[2], CultureInfo.InvariantCulture.NumberFormat));

             target.H_Target_to_T1 = Matrix4x4.TRS (target.position, target.rotation, new Vector3 (1, 1, 1));

             //PARSE ANNOTATION
             target.annotation = values[counter - 1].ToString ();

             counter += PROPERTIES_NUM;

         }

         //reader.Close ();
     }

     void calculatePosesBasedOnT1 () {

         //Obtain real location from world
         T1.H_Target_to_World = Matrix4x4.TRS (T1.imageTarget.transform.position, T1.imageTarget.transform.rotation, new Vector3 ((float) 1, (float) 1, (float) 1));

         //Multiply matrices
         if (T2 != null) {
             T2.H_Target_to_World = T1.H_Target_to_World * T2.H_Target_to_T1;
         }
         if (T3 != null) {
             T3.H_Target_to_World = T1.H_Target_to_World * T3.H_Target_to_T1;
         }
     }

     void calculatePosesBasedOnT2 () {

         //Obtain real location from world
         T2.H_Target_to_World = Matrix4x4.TRS (T2.imageTarget.transform.position, T2.imageTarget.transform.rotation, new Vector3 ((float) 1, (float) 1, (float) 1));

         //Multiply matrices
         if (T1 != null) {
             T1.H_Target_to_World = T2.H_Target_to_World * T2.H_Target_to_T1.inverse;
         }
         if (T3 != null) {
             T3.H_Target_to_World = T1.H_Target_to_World * T3.H_Target_to_T1;
         }

     }

     void calculatePosesBasedOnT3 () {

         //Obtain real location from world
         T3.H_Target_to_World = Matrix4x4.TRS (T3.imageTarget.transform.position, T3.imageTarget.transform.rotation, new Vector3 ((float) 1, (float) 1, (float) 1));

         //Multiply matrices
         if (T1 != null) {
             T1.H_Target_to_World = T3.H_Target_to_World * T3.H_Target_to_T1.inverse;
         }
         if (T2 != null) {
             T2.H_Target_to_World = T1.H_Target_to_World * T2.H_Target_to_T1;
         }

     }

     void setObjectPoses () {
         T1Obj = GameObject.Find ("MarkerX1Obj");
         T2Obj = GameObject.Find ("MarkerA2Obj");
         T3Obj = GameObject.Find ("MarkerQ3Obj");

         if (T1 != null) {
             T1Obj.transform.position = new Vector3 (T1.H_Target_to_World[0, 3], T1.H_Target_to_World[1, 3], T1.H_Target_to_World[2, 3]);
             T1Obj.transform.rotation = T1.H_Target_to_World.rotation;
         }
         if (T2 != null) {
             T2Obj.transform.position = new Vector3 (T2.H_Target_to_World[0, 3], T2.H_Target_to_World[1, 3], T2.H_Target_to_World[2, 3]);
             T2Obj.transform.rotation = T2.H_Target_to_World.rotation;
         }
         if (T3 != null) {
             T3Obj.transform.position = new Vector3 (T3.H_Target_to_World[0, 3], T3.H_Target_to_World[1, 3], T3.H_Target_to_World[2, 3]);
             T3Obj.transform.rotation = T3.H_Target_to_World.rotation;
         }
     }

     public void changeToWriteScene () {
         //Used from Scene 1 to go to Scene 0
         SceneManager.LoadScene (0);
     }

 }