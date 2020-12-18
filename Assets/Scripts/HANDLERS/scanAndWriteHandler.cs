using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vuforia;

public class scanAndWriteHandler : MonoBehaviour {

    private targetObject T1;
    private targetObject T2;
    private targetObject T3;

    //Frames
    private int frames = 0;

    //Overall variables
    private int numberOfTargets = 1;
    private int foundTargets = 0;
    private Matrix4x4 H_N_N = Matrix4x4.identity; //Empty matrix null, necessary for creating the targetObject in this instance. Don't remove
    public Text notification;
    public Text targetCounterText;
    public GameObject notificationBackground;

    List<targetObject> targetObjectArray = new List<targetObject> ();

    void Start () {
        T1 = new targetObject (GameObject.Find ("MarkerX1"), H_N_N, false);
        T2 = new targetObject (GameObject.Find ("MarkerA2"), H_N_N, false);
        T3 = new targetObject (GameObject.Find ("MarkerQ3"), H_N_N, false);

        if (T1 == null || T2 == null || T3 == null) {
            Debug.Log ("Couldn't find a target object");
        }
        notificationBackground.SetActive (false);

    }

    // Update is called once per frame
    void Update () {
        frames++;
        if (frames % 10 == 0) {

            //Debug.Log(this.foundTargets);
            if (this.foundTargets != this.numberOfTargets) {
                T1.H_Target_to_World = lookingForObject (T1);
                T2.H_Target_to_World = lookingForObject (T2);
                T3.H_Target_to_World = lookingForObject (T3);
            }

            setAnnotations ();

            if (frames % 300 == 0) { //Cleans notification text every 300 frames
                notification.text = "";
                notificationBackground.SetActive (false);
            }

            if (foundTargets == numberOfTargets) {
                targetCounterText.text = "All targets were found";

            } else {
                targetCounterText.text = "TARGETS TO FIND: " + (numberOfTargets - foundTargets).ToString ();

            }
        }
    }

    private Matrix4x4 lookingForObject (targetObject Target) {
        if (Target.imageTarget.GetComponent<TrackableBehaviour> ().CurrentStatus == TrackableBehaviour.Status.TRACKED) // Found 
        {

            if (Target.hasBeenFound == false) {
                Target.hasBeenFound = true;
                foundTargets++;
                notification.text = Target.imageTarget.name + " was found";
                notificationBackground.SetActive (true);
            }
            return Matrix4x4.TRS (Target.imageTarget.transform.position, Target.imageTarget.transform.rotation, new Vector3 (1, 1, 1));
        }

        return Target.H_Target_to_World;
    }

    public void writeData () {

        if (foundTargets == numberOfTargets) {
            notification.text = "All targets were found! Writing data now...";
            notificationBackground.SetActive (true);

            // Compute starting poses with respect to target 1 and save them.

            T1.H_World_to_Target = T1.H_Target_to_World.inverse; //Obtain the inverse and save it into the object

            T2.H_Target_to_T1 = T1.H_World_to_Target * T2.H_Target_to_World; //Calculate difference and re-saved them
            T3.H_Target_to_T1 = T1.H_World_to_Target * T3.H_Target_to_World; //Calculate difference and re-saved them

            //Obtain individual values
            //WORLD TO TARGET VALUES
            T1.position = new Vector3 (T1.H_Target_to_World[0, 3], T1.H_Target_to_World[1, 3], T1.H_Target_to_World[2, 3]);
            // T1.rotation = T1.H_Target_to_World.rotation;
            T1.annotation = "Annotation ONE GoesHere";

            //TARGET TO TARGET VALUES
            T2.position = new Vector3 (T2.H_Target_to_T1[0, 3], T2.H_Target_to_T1[1, 3], T2.H_Target_to_T1[2, 3]);
            // T2.rotation = T2.H_Target_to_T1.rotation;
            T2.annotation = "Annotation DOS GoesHere";

            T3.position = new Vector3 (T3.H_Target_to_T1[0, 3], T3.H_Target_to_T1[1, 3], T3.H_Target_to_T1[2, 3]);
            // T3.rotation = T3.H_Target_to_T1.rotation;
            T3.annotation = "Annotation TRES GoesHere";

            string data = "";

            data += T1.imageTarget.name + "$" + T1.H_Target_to_World.rotation.ToString ("F8") + "$" + T1.position.ToString ("F8") + "$" + T1.annotation;

            data += "$" + T2.imageTarget.name + "$" + T2.H_Target_to_T1.rotation.ToString ("F8") + "$" + T2.position.ToString ("F8") + "$" + T2.annotation;

            data += "$" + T3.imageTarget.name + "$" + T3.H_Target_to_T1.rotation.ToString ("F8") + "$" + T3.position.ToString ("F8") + "$" + T3.annotation;

            //string path = "Assets/Resources/targets.txt";

            //Write some text to the CurrentData.txt file
            //Writes to C:\Users\jjavi\AppData\LocalLow\DefaultCompany\IndependentStudyProjectV1-ANDROID
            StreamWriter writer = new StreamWriter (Application.persistentDataPath + "/targets.txt", false);
            writer.WriteLine (data);

            //writer.AutoFlush = true;
            writer.Close ();

        } else {
            notification.text = "Find the remaining " + (this.numberOfTargets - this.foundTargets) + " targets";
            notificationBackground.SetActive (true);

        }

    }

    void setAnnotations () {
        //Arrow length is about 10 cm
        Vector3 arrayLengthOffset = new Vector3 (float.Parse ("0.0", CultureInfo.InvariantCulture.NumberFormat), float.Parse ("0.0", CultureInfo.InvariantCulture.NumberFormat), float.Parse ("0.11", CultureInfo.InvariantCulture.NumberFormat));

        //Set Annotation 1
        GameObject MarkerX1Annotation = GameObject.Find ("MarkerX1Annotation");
        MarkerX1Annotation.transform.position = T1.imageTarget.transform.position + arrayLengthOffset;
        MarkerX1Annotation.transform.LookAt (MarkerX1Annotation.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

        //Set Annotation 2
        GameObject MarkerA2Annotation = GameObject.Find ("MarkerA2Annotation");
        MarkerA2Annotation.transform.position = T2.imageTarget.transform.position + arrayLengthOffset;
        MarkerA2Annotation.transform.LookAt (MarkerA2Annotation.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

        //Set Annotation 3
        GameObject MarkerQ3Annotation = GameObject.Find ("MarkerQ3Annotation");
        MarkerQ3Annotation.transform.position = T3.imageTarget.transform.position + arrayLengthOffset;
        MarkerQ3Annotation.transform.LookAt (MarkerQ3Annotation.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

    }

    public void changeToLoadScene () {
        //Used from Scene 0 to go to Scene 1
        SceneManager.LoadScene (1);

    }

}