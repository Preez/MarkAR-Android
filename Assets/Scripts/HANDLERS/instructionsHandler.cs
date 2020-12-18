using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vuforia;

public class instructionsHandler : MonoBehaviour {

    //Array with the location of arrows depending of the step
    private string[] annotLocationData;
    private int currentStep;
    private TextMeshPro textBox;

    private TextMeshPro annotationT1, annotationT2, annotationT3;

    private GameObject MarkerX1Comp, MarkerA2Comp, MarkerQ3Comp;

    private GameObject MarkerX1Obj, MarkerA2Obj, MarkerQ3Obj, compassTarget;

    private GameObject miniCanvas, miniInstructions, miniInstructionsBackground, RawImage, rawImageText, rawImageTextBackground, childrenInsideMarkerX1Comp, arrowCompass;

    private targetObject T1, T2, T3, T4;

    private annotObject annotOneObj, annotTwoObj, annotThreeObj;

    private IDictionary<string, annotObject> objectsOfInterest = new Dictionary<string, annotObject> ();

    private Vector3 arrowLength;

    private Matrix4x4 H_N_N = Matrix4x4.identity; //Empty matrix null

    private float WindowsPlayerAdjustment;
    public static int PROPERTIES_NUM = 4; // Rotation, Position, Annotation, and Marker number

    //Instructions object
    private instructionObject instructions = new instructionObject ();

    //Array of Targets objects  
    private List<targetObject> TArray = new List<targetObject> ();

    //steps list for convinient adding
    private List<stepObject> stepList = new List<stepObject> ();

    // Colors
    private Color invisibleColor, displayColor;

    // Start is called before the first frame update
    void Start () {
        //Get overall objects
        annotationT1 = GameObject.Find ("MarkerX1Annotation").GetComponent<TextMeshPro> ();
        annotationT2 = GameObject.Find ("MarkerA2Annotation").GetComponent<TextMeshPro> ();
        annotationT3 = GameObject.Find ("MarkerQ3Annotation").GetComponent<TextMeshPro> ();

        MarkerX1Comp = GameObject.Find ("MarkerX1Comp");
        MarkerA2Comp = GameObject.Find ("MarkerA2Comp");
        MarkerQ3Comp = GameObject.Find ("MarkerQ3Comp");

        MarkerX1Obj = GameObject.Find ("MarkerX1Obj");
        MarkerA2Obj = GameObject.Find ("MarkerA2Obj");
        MarkerQ3Obj = GameObject.Find ("MarkerQ3Obj");

        // Get UI components
        miniInstructions = tools.GetChildWithName (GameObject.Find ("Canvas"), "mini-instructions");
        miniInstructionsBackground = tools.GetChildWithName (GameObject.Find ("Canvas"), "mini-instructions-background");

        RawImage = GameObject.Find ("RawImage");
        rawImageText = tools.GetChildWithName (RawImage, "rawImageText");
        rawImageTextBackground = tools.GetChildWithName (RawImage, "raw-image-text-background");
        childrenInsideMarkerX1Comp = tools.GetAnyChild (MarkerX1Comp);
        arrowCompass = GameObject.Find ("compass");
        compassTarget = GameObject.Find ("MarkerX1Comp");

        currentStep = 0;

        displayStepName ();

        loadAnnotIntoMatrixes ();

        loadInstructions ();
        //Load text files
        loadTargetData ();

        miniCanvas = GameObject.Find ("mini-canvas");

        textBox = GameObject.Find ("instructionsText").GetComponent<TextMeshPro> ();
        textBox.SetText (instructions.steps[currentStep].instructionStepText);

        miniInstructions.GetComponent<TextMeshProUGUI> ().SetText (instructions.steps[currentStep].instructionStepText);
        miniInstructionsBackground.GetComponent<TextMeshProUGUI> ().SetText ("<mark=#000000>" + instructions.steps[currentStep].instructionStepText);

        rawImageText.GetComponent<TextMeshProUGUI> ().color = invisibleColor;

        //Check if we are using Windows or any other platoform
        arrowLength = new Vector3 (float.Parse ("0.0", CultureInfo.InvariantCulture.NumberFormat), float.Parse ("0.0", CultureInfo.InvariantCulture.NumberFormat), float.Parse ("0.05", CultureInfo.InvariantCulture.NumberFormat));

    }

    void Update () {

        makeMiniCanvasLookAtCamera ();

        // Determines whether or not to display the arrow
        childrenInsideMarkerX1Comp = tools.GetAnyChild (MarkerX1Comp); // <------------------

        makeArrowPointTarget (compassTarget);
        if (currentStep == 0 || currentStep == (instructions.steps.Count - 1) || childrenInsideMarkerX1Comp.GetComponent<Renderer> ().isVisible) {
            UIHandler.hideArrowCompass ();
        } else if (!childrenInsideMarkerX1Comp.GetComponent<Renderer> ().isVisible && currentStep > 0) { //If is not visible, display arrow
            UIHandler.showArrowCompass ();
        }

        if (T1 != null && T1.imageTarget.GetComponent<TrackableBehaviour> ().CurrentStatus == TrackableBehaviour.Status.TRACKED) // Found 
        {
            calcAnnotBasedOnT1 ();
        }

        if (T2 != null && T2.imageTarget.GetComponent<TrackableBehaviour> ().CurrentStatus == TrackableBehaviour.Status.TRACKED) // Found 
        {
            calcAnnotBasedOnT2 ();
        }

        if (T3 != null && T3.imageTarget.GetComponent<TrackableBehaviour> ().CurrentStatus == TrackableBehaviour.Status.TRACKED) // Found 
        {
            calcAnnotBasedOnT3 ();
        }

        updateStepAnnot ();
        changeTherbligShape ();
        placeAnnotArrows ();
        handleAnnotText ();
    }

    void loadInstructions () {

    
        //origin,screen,right-handle,left-handle,green-lever,big-green-handle,slot

        stepList.Add (new stepObject ("Do-it-yourself toner maintenance. Replace the toner from the printer. Press the '>' symbol to move to the next step. Press the '<' symbol to move to the previous step. Make sure the Marker is always visible to the camera.", therblig.UNKNOWN));
        stepList.Last ().annotations.Add (objectsOfInterest["origin"]);

        stepList.Add (new stepObject ("Find the printer screen", therblig.FIND));
        stepList.Last ().annotations.Add (objectsOfInterest["screen"]);
        stepList.Add (new stepObject ("Inspect the screen to make sure the priner is turned on.", therblig.INSPECT));
        stepList.Last ().annotations.Add (objectsOfInterest["screen"]);

        stepList.Add (new stepObject ("Find the right handle.", therblig.FIND));
        stepList.Last ().annotations.Add (objectsOfInterest["right-handle"]);
        stepList.Add (new stepObject ("Pull the front cover from the right handle.", therblig.PULL));
        stepList.Last ().annotations.Add (objectsOfInterest["right-handle"]);
        stepList.Add (new stepObject ("Release the right handle.", therblig.RELEASE));
        stepList.Last ().annotations.Add (objectsOfInterest["right-handle"]);
        stepList.Add (new stepObject ("Verify the front cover has opened.", therblig.INSPECT));
        stepList.Last ().annotations.Add (objectsOfInterest["right-handle"]);

        stepList.Add (new stepObject ("Find the green lock lever.", therblig.FIND));
        stepList.Last ().annotations.Add (objectsOfInterest["green-lever"]);
        stepList.Last ().image = Resources.Load<Texture2D> ("printer-images/1");
        stepList.Add (new stepObject ("Push down the green lock lever.", therblig.PUSH));
        stepList.Last ().annotations.Add (objectsOfInterest["green-lever"]);
        stepList.Last ().image = Resources.Load<Texture2D> ("printer-images/1");

        stepList.Add (new stepObject ("Verify the green lock lever cannot be pushed more.", therblig.INSPECT));
        stepList.Last ().image = Resources.Load<Texture2D> ("printer-images/1");
        stepList.Last ().annotations.Add (objectsOfInterest["green-lever"]);

        stepList.Add (new stepObject ("Find the big green handle.", therblig.FIND));
        stepList.Last ().annotations.Add (objectsOfInterest["big-green-handle"]);
        stepList.Last ().image = Resources.Load<Texture2D> ("printer-images/2");
        stepList.Add (new stepObject ("Grasp the big green handle.", therblig.GRASP));
        stepList.Last ().annotations.Add (objectsOfInterest["big-green-handle"]);
        stepList.Last ().image = Resources.Load<Texture2D> ("printer-images/2");
        stepList.Add (new stepObject ("Pull the toner cartridge from the green handle.", therblig.PULL));
        stepList.Last ().annotations.Add (objectsOfInterest["big-green-handle"]);
        stepList.Last ().image = Resources.Load<Texture2D> ("printer-images/2");
        stepList.Add (new stepObject ("Place the toner cartidge in a safe place", therblig.UNKNOWN));

        stepList.Add (new stepObject ("Obtain the new toner cartidge", therblig.UNKNOWN));
        stepList.Add (new stepObject ("Find the toner cartidge slot", therblig.FIND));
        stepList.Last ().annotations.Add (objectsOfInterest["slot"]);
        stepList.Add (new stepObject ("Align the toner cartidge with the slot", therblig.ALIGN));
        stepList.Last ().annotations.Add (objectsOfInterest["slot"]);
        stepList.Add (new stepObject ("Push the toner cartidge into the slot", therblig.PUSH));
        stepList.Last ().annotations.Add (objectsOfInterest["slot-push"]);

        stepList.Add (new stepObject ("Push the front cover from the right handle", therblig.PUSH));
        stepList.Last ().annotations.Add (objectsOfInterest["right-handle"]);
        stepList.Add (new stepObject ("Verify the front cover has assembled correctly from the right handle", therblig.INSPECT));
        stepList.Last ().annotations.Add (objectsOfInterest["right-handle"]);
        stepList.Add (new stepObject ("Now, verify the front cover has assembled correctly from the left handle", therblig.INSPECT));
        stepList.Last ().annotations.Add (objectsOfInterest["left-handle"]);
        stepList.Add (new stepObject ("That has been completed.", therblig.UNKNOWN));

        // tools.writeCSVSummary(instructions.steps);

        //Add steps into the instructions list
        foreach (stepObject step in stepList) {
            instructions.steps.Add (step);
        }

    }

    void loadAnnotIntoMatrixes () {
        // Y AND Z ARE SHIFTED

        /////////////////////////////
        // PRINTER   /  
        ////////////////////////////
        //     ________________
        //   _/_______________/|
        //  /___________/___//||
        // |===        |----| ||
        // |           |    | ||
        // |___________|    | ||
        // | ||/.---.||     | ||
        // |-||/_____\||-.  | |
        // |_||=L==H==||_|__|/     
        /////////////////////////////

        objectsOfInterest.Add ("origin", new annotObject ("", tools.fromEulerToQuaternion (0, 0, 0), tools.createPosition (0, 0, 0)));

        objectsOfInterest.Add ("screen", new annotObject ("Printer screen", tools.fromEulerToQuaternion (0, 0, 0), tools.createPosition (0.11f, -0.04f, 0.20f)));

        objectsOfInterest.Add ("right-handle", new annotObject ("Right Handle", tools.fromEulerToQuaternion (90, 90, -90), tools.createPosition (0.30f, 0, 0.15f)));
        objectsOfInterest.Add ("left-handle", new annotObject ("Left Handle", tools.fromEulerToQuaternion (90, 90, -90), tools.createPosition (-0.07f, 0, 0.15f)));

        objectsOfInterest.Add ("green-lever", new annotObject ("Green Lever", tools.fromEulerToQuaternion (0, 0, 0), tools.createPosition (-0.01f, -0.035f, 0.09f)));
        objectsOfInterest.Add ("big-green-handle", new annotObject ("Big Green Handle", tools.fromEulerToQuaternion (90, 90, -90), tools.createPosition (0.11f, -0.001f, 0.105f)));

        //Slot should be 0.27 m wide and 0.04 m tall
        objectsOfInterest.Add ("slot", new annotObject ("Toner Slot", tools.fromEulerToQuaternion (0, 0, 0), tools.createPosition (0.11f, 0, 0.105f)));
        objectsOfInterest.Add ("slot-push", new annotObject ("Toner Slot", tools.fromEulerToQuaternion (90, 90, -90), tools.createPosition (0.11f, 0, 0.105f)));

        ////////////////////////
        // CASETTE PLAYER      /
        ////////////////////////
        // objectsOfInterest.Add ("origin", new annotObject ("", tools.fromEulerToQuaternion (0, 0, 0), tools.createPosition (0, 0, 0)));

        // objectsOfInterest.Add ("screw-1", new annotObject `("Left-side Front screw #1", tools.fromEulerToQuaternion (-90, 90, 0), tools.createPosition (-0.205f, -0.10f, -0.085f)));
        // objectsOfInterest.Add ("screw-2", new annotObject ("Left-side Back screw #2", tools.fromEulerToQuaternion (-90, 90, 0), tools.createPosition (-0.205f, -0.10f, 0.07f)));

        // objectsOfInterest.Add ("screw-3", new annotObject ("Right-side Front screw #3", tools.fromEulerToQuaternion (-90, -90, 0), tools.createPosition (0.205f, -0.10f, -0.085f)));
        // objectsOfInterest.Add ("screw-4", new annotObject ("Right-side Back screw #4", tools.fromEulerToQuaternion (-90, -90, 0), tools.createPosition (0.205f, -0.10f, 0.07f)));

        // objectsOfInterest.Add ("screw-5", new annotObject ("Back-side Left screw #5", tools.fromEulerToQuaternion (-90, 180, 0), tools.createPosition (-0.1525f, -0.01f, 0.095f)));
        // objectsOfInterest.Add ("screw-6", new annotObject ("Back-side Right screw #6", tools.fromEulerToQuaternion (-90, 180, 0), tools.createPosition (0.15f, -0.01f, 0.095f)));

        // objectsOfInterest.Add ("black-cable", new annotObject ("Black Cable", tools.fromEulerToQuaternion (0, 90, 0), tools.createPosition (0.12f, -0.02f, -0.05f)));

        /////////////////////////////////
        // CAMRY OBJECTS OF INTEREST    /
        /////////////////////////////////

        // objectsOfInterest.Add ("engine-switch", new annotObject ("Engine Switch", tools.fromEulerToQuaternion (0, 0, 0), tools.createPosition (-0.73f, -0.16f, -0.06f)));

        // objectsOfInterest.Add ("glove-box", new annotObject ("Glove Box", tools.fromEulerToQuaternion (0, 0, 0), tools.createPosition (0, 0, -0.14f)));
        // objectsOfInterest.Add ("glove-box-handle", new annotObject ("Glove Box Handle", tools.fromEulerToQuaternion (90, 180, 0), tools.createPosition (-0.25f, -0.10f, -0.09f)));

        // objectsOfInterest.Add ("glove-box-cover", new annotObject ("Glove Box Cover", tools.fromEulerToQuaternion (90, 90, 0), tools.createPosition (0.08f, -0.25f, -0.04f)));
        // objectsOfInterest.Add ("filter-right-pinch", new annotObject ("Filter Right Pinch", tools.fromEulerToQuaternion (90, 90, 0), tools.createPosition (0.02f, -0.27f, -0.02f)));
        // objectsOfInterest.Add ("filter-left-pinch", new annotObject ("Filter Left Pinch", tools.fromEulerToQuaternion (-90, 90, 0), tools.createPosition (-0.17f, -0.27f, -0.02f)));
        // objectsOfInterest.Add ("filter-cover", new annotObject ("Filter Cover", tools.fromEulerToQuaternion (90, 90, -90), tools.createPosition (-0.05f, -0.27f, -0.02f)));
        // objectsOfInterest.Add ("filter", new annotObject ("Filter", tools.fromEulerToQuaternion (90, 90, -90), tools.createPosition (-0.10f, -0.27f, -0.02f)));

        /////////////////////////////////
        // COROLLA OBJECTS OF INTEREST  /
        /////////////////////////////////

        // objectsOfInterest.Add ("Engine Switch", new annotObject ("Engine Switch", tools.fromEulerToQuaternion (0, 0, 0), tools.createPosition (-0.75f, -0.05f, 0.03f)));
        // objectsOfInterest.Add ("Glove Box Handle", new annotObject ("Glove Box Handle", tools.fromEulerToQuaternion (0, 0, 0), tools.createPosition (-0.11f, 0.06f, -0.08f)));
        // objectsOfInterest.Add ("Glove Box", new annotObject ("Glove Box", tools.fromEulerToQuaternion (0, 0, 0), tools.createPosition (-0.11f, 0.06f, -0.08f)));
        // objectsOfInterest.Add ("Glove Box Cover", new annotObject ("Glove Box Cover", tools.fromEulerToQuaternion (0, 0, 0), tools.createPosition (-0.11f, 0.06f, -0.08f)));
        // objectsOfInterest.Add ("Damper", new annotObject ("Damper", tools.fromEulerToQuaternion (0, 0, 0), tools.createPosition (0.06f, 0.06f, -0.08f)));
        // objectsOfInterest.Add ("Left Hook", new annotObject ("Left Hook", tools.fromEulerToQuaternion (0, 0, 0), tools.createPosition (-0.16f, -0.20f, -0.30f)));
        // objectsOfInterest.Add ("Right Hook", new annotObject ("Right Hook", tools.fromEulerToQuaternion (0, 0, 0), tools.createPosition (0.14f, -0.20f, -0.30f)));
        // objectsOfInterest.Add ("Left wall", new annotObject ("Right wall", tools.fromEulerToQuaternion (0, 0, 0), tools.createPosition (-0.22f, -0.06f, -0.14f)));
        // objectsOfInterest.Add ("Right wall", new annotObject ("Right wall", tools.fromEulerToQuaternion (0, 0, 0), tools.createPosition (0.19f, -0.06f, -0.14f)));
        // objectsOfInterest.Add ("Air Filter (white rectangle)", new annotObject ("Air Filter (white rectangle)", tools.fromEulerToQuaternion (0, 0, 0), tools.createPosition (0, -0.06f, -0.08f)));

    }

    void placeAnnotArrows () {
        if (annotOneObj != null) {
            MarkerX1Comp.transform.position = new Vector3 (annotOneObj.H_Annot_to_World[0, 3], annotOneObj.H_Annot_to_World[1, 3], annotOneObj.H_Annot_to_World[2, 3]);
            MarkerX1Comp.transform.rotation = annotOneObj.rotation;
        }
        if (annotTwoObj != null) {
            MarkerA2Comp.transform.position = new Vector3 (annotTwoObj.H_Annot_to_World[0, 3], annotTwoObj.H_Annot_to_World[1, 3], annotTwoObj.H_Annot_to_World[2, 3]);
            MarkerA2Comp.transform.rotation = annotTwoObj.rotation;
        }
        if (annotThreeObj != null) {
            MarkerQ3Comp.transform.position = new Vector3 (annotThreeObj.H_Annot_to_World[0, 3], annotThreeObj.H_Annot_to_World[1, 3], annotThreeObj.H_Annot_to_World[2, 3]);
            MarkerQ3Comp.transform.rotation = annotThreeObj.rotation;
        }
    }

    void calcAnnotBasedOnT1 () {

        T1.H_Target_to_World = Matrix4x4.TRS (T1.imageTarget.transform.position, T1.imageTarget.transform.rotation, new Vector3 ((float) 1, (float) 1, (float) 1));

        if (annotOneObj != null) {
            annotOneObj.H_Annot_to_World = T1.H_Target_to_World * annotOneObj.H_Annot_to_T1;
        }

        if (annotTwoObj != null) {
            annotTwoObj.H_Annot_to_World = T1.H_Target_to_World * annotTwoObj.H_Annot_to_T1;
        }

        if (annotThreeObj != null) {
            annotThreeObj.H_Annot_to_World = T1.H_Target_to_World * annotThreeObj.H_Annot_to_T1;
        }

    }
    void calcAnnotBasedOnT2 () {
        T2.H_Target_to_World = Matrix4x4.TRS (T2.imageTarget.transform.position, T2.imageTarget.transform.rotation, new Vector3 ((float) 1, (float) 1, (float) 1));

        if (annotOneObj != null) {
            annotOneObj.H_Annot_to_World = T2.H_Target_to_World * T2.H_Target_to_T1.inverse * annotOneObj.H_Annot_to_T1;
        }
        if (annotTwoObj != null) {
            annotTwoObj.H_Annot_to_World = T2.H_Target_to_World * T2.H_Target_to_T1.inverse * annotTwoObj.H_Annot_to_T1;
        }
        if (annotThreeObj != null) {
            annotThreeObj.H_Annot_to_World = T2.H_Target_to_World * T2.H_Target_to_T1.inverse * annotThreeObj.H_Annot_to_T1;
        }
    }

    void calcAnnotBasedOnT3 () {
        T3.H_Target_to_World = Matrix4x4.TRS (T3.imageTarget.transform.position, T3.imageTarget.transform.rotation, new Vector3 ((float) 1, (float) 1, (float) 1));

        if (annotOneObj != null) {
            annotOneObj.H_Annot_to_World = T3.H_Target_to_World * T3.H_Target_to_T1.inverse * annotOneObj.H_Annot_to_T1;
        }
        if (annotTwoObj != null) {
            annotTwoObj.H_Annot_to_World = T3.H_Target_to_World * T3.H_Target_to_T1.inverse * annotTwoObj.H_Annot_to_T1;
        }
        if (annotThreeObj != null) {
            annotThreeObj.H_Annot_to_World = T3.H_Target_to_World * T3.H_Target_to_T1.inverse * annotThreeObj.H_Annot_to_T1;
        }

    }

    void handleAnnotText () {

        annotationT1.SetText ("");
        annotationT2.SetText ("");
        annotationT3.SetText ("");

        if (instructions.steps[currentStep].annotations.Count >= 1) {
            annotationT1.SetText (instructions.steps[currentStep].annotations[0].annotation);

            //Resets the annotOneObj in case it is not the corresponding one inside the steps array
            annotOneObj = instructions.steps[currentStep].annotations[0];

            annotationT1.transform.position = MarkerX1Comp.transform.position + arrowLength;
            annotationT1.transform.LookAt (annotationT1.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

        } else {
            annotationT3.SetText ("");
            MarkerX1Comp.transform.position = new Vector3 (5000, 5000, 5000);
        }

        if (instructions.steps[currentStep].annotations.Count >= 2) {
            annotationT2.SetText (instructions.steps[currentStep].annotations[1].annotation);

            //Resets the annotTwoObj in case it is not the corresponding one inside the steps array
            annotTwoObj = instructions.steps[currentStep].annotations[1];

            annotationT2.transform.position = MarkerA2Comp.transform.position + arrowLength;
            annotationT2.transform.LookAt (annotationT2.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

        } else {
            annotationT3.SetText ("");
            MarkerA2Comp.transform.position = new Vector3 (5000, 5000, 5000);
        }

        if (instructions.steps[currentStep].annotations.Count >= 3) {
            annotationT3.SetText (instructions.steps[currentStep].annotations[2].annotation);

            //Resets the annotThreeObj in case it is not the corresponding one inside the steps array
            annotThreeObj = instructions.steps[currentStep].annotations[2];
            annotationT3.transform.position = MarkerQ3Comp.transform.position + arrowLength;
            annotationT3.transform.LookAt (annotationT3.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

        } else {
            annotationT3.SetText ("");
            MarkerQ3Comp.transform.position = new Vector3 (5000, 5000, 5000);
        }

    }

    void loadTargetData () {
        //string path = "Assets / Resources / targets.txt ";
        //Read the text from directly from the test.txt file

        //  StreamReader reader = new StreamReader (Application.persistentDataPath + " / targets.txt ");

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

        if (TArray.Count == 1) {

            rotation = values[counter - 3];
            // //Remove parenthesis
            rotation = rotation.Remove (0, 1);
            rotation = rotation.Remove (rotation.Length - 1, 1);
            string[] singleValue = rotation.Split (',');
            T1.rotation = new Quaternion (float.Parse (singleValue[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[2], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[3], CultureInfo.InvariantCulture.NumberFormat));

            //PARSE POSITION
            position = values[counter - 2].ToString ();
            //Remove parenthesis
            position = position.Remove (0, 1);
            position = position.Remove (position.Length - 1, 1);
            singleValue = position.Split (',');
            T1.position = new Vector3 (float.Parse (singleValue[0], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[1], CultureInfo.InvariantCulture.NumberFormat), float.Parse (singleValue[2], CultureInfo.InvariantCulture.NumberFormat));

            T1.H_Target_to_T1 = Matrix4x4.TRS (T1.position, T1.rotation.normalized, new Vector3 (1, 1, 1));

            //PARSE ANNOTATION
            T1.annotation = values[counter - 1].ToString ();

        } else {

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

                target.H_Target_to_T1 = Matrix4x4.TRS (target.position, target.rotation.normalized, new Vector3 (1, 1, 1));

                //PARSE ANNOTATION
                target.annotation = values[counter - 1].ToString ();

                counter += PROPERTIES_NUM;

            }
        }
    }

    void nextStep () {
        currentStep++;
        displayStepName ();

        if (currentStep < instructions.steps.Count) {
            textBox.SetText (instructions.steps[currentStep].instructionStepText);
            miniInstructions.GetComponent<TextMeshProUGUI> ().SetText (instructions.steps[currentStep].instructionStepText);
            miniInstructionsBackground.GetComponent<TextMeshProUGUI> ().SetText ("<mark=#000000>" + instructions.steps[currentStep].instructionStepText);

            var aColor = RawImage.GetComponent<RawImage> ().color;
            if (instructions.steps[currentStep].image != null) {
                UIHandler.displayImageUI (instructions, currentStep);
            } else {
                UIHandler.hideImageUI ();
            }

        } else {
            currentStep = currentStep - 1;
        }

    }

    void previousStep () {
        currentStep = currentStep - 1;
        displayStepName ();
        if (currentStep > -1) {
            textBox.SetText (instructions.steps[currentStep].instructionStepText);
            miniInstructions.GetComponent<TextMeshProUGUI> ().SetText (instructions.steps[currentStep].instructionStepText);
            miniInstructionsBackground.GetComponent<TextMeshProUGUI> ().SetText ("<mark=#000000>" + instructions.steps[currentStep].instructionStepText);

            if (instructions.steps[currentStep].image != null) {
                UIHandler.displayImageUI (instructions, currentStep);
            } else {
                UIHandler.hideImageUI ();
            }

        } else {
            currentStep++;
        }
    }

    void displayStepName () {
        GameObject stepName = tools.GetChildWithName (GameObject.Find ("Canvas"), "stepName");
        stepName.GetComponent<TextMeshProUGUI> ().SetText ("STEP: " + currentStep + "/" + (instructions.steps.Count));
    }

    void makeMiniCanvasLookAtCamera () {
        Vector3 p = transform.position;
        p = MarkerX1Obj.transform.position;
        miniCanvas.GetComponent<RectTransform> ().transform.position = p + new Vector3 (0, (float) 0.05, 0);

        miniCanvas.GetComponent<RectTransform> ().transform.LookAt (miniCanvas.GetComponent<RectTransform> ().transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        Vector3 r = miniCanvas.GetComponent<RectTransform> ().transform.rotation.eulerAngles;
        miniCanvas.GetComponent<RectTransform> ().transform.rotation = Quaternion.Euler (r.x - 30, r.y, r.z);
    }

    void updateStepAnnot () {
        if (instructions.steps[currentStep].annotations.Count > 0) {
            if (instructions.steps[currentStep].annotations.Count == 1 && instructions.steps[currentStep].annotations[0] != null) {
                annotOneObj = instructions.steps[currentStep].annotations[0];
            }
            if (instructions.steps[currentStep].annotations.Count == 2 && instructions.steps[currentStep].annotations[1] != null) {
                annotTwoObj = instructions.steps[currentStep].annotations[1];
            }
            if (instructions.steps[currentStep].annotations.Count == 3 && instructions.steps[currentStep].annotations[2] != null) {
                annotThreeObj = instructions.steps[currentStep].annotations[2];
            }

        } else {
            annotOneObj = null;
            annotTwoObj = null;
            annotThreeObj = null;
        }
    }

    void changeTherbligShape () {
        GameObject therbligComps = GameObject.Find ("therbligComps");

        if (MarkerX1Comp != null) {
            MarkerX1Comp.transform.position = new Vector3 (5000, 5000, 5000);
            if (instructions.steps[currentStep].therblig == therblig.FIND) {
                MarkerX1Comp = tools.GetChildWithName (therbligComps, "FindComp");
                compassTarget = GameObject.Find ("FindComp");
            } else if (instructions.steps[currentStep].therblig == therblig.TRANSPORT) {
                MarkerX1Comp = tools.GetChildWithName (therbligComps, "TransportComp");
                compassTarget = GameObject.Find ("TransportComp");
            } else if (instructions.steps[currentStep].therblig == therblig.GRASP) {
                MarkerX1Comp = tools.GetChildWithName (therbligComps, "GraspComp");
                compassTarget = GameObject.Find ("GraspComp");
            } else if (instructions.steps[currentStep].therblig == therblig.HOLD) {

            } else if (instructions.steps[currentStep].therblig == therblig.RELEASE) {
                MarkerX1Comp = tools.GetChildWithName (therbligComps, "ReleaseComp");
                compassTarget = GameObject.Find ("ReleaseComp");
            } else if (instructions.steps[currentStep].therblig == therblig.POSITION) {

            } else if (instructions.steps[currentStep].therblig == therblig.LOOK) {
                MarkerX1Comp = tools.GetChildWithName (therbligComps, "LookComp");
                compassTarget = GameObject.Find ("LookComp");
            } else if (instructions.steps[currentStep].therblig == therblig.PLACE) {

            } else if (instructions.steps[currentStep].therblig == therblig.ASSEMBLE) {

            } else if (instructions.steps[currentStep].therblig == therblig.USE) {

            } else if (instructions.steps[currentStep].therblig == therblig.INSPECT) {
                MarkerX1Comp = tools.GetChildWithName (therbligComps, "InspectComp");
                compassTarget = GameObject.Find ("InspectComp");
            } else if (instructions.steps[currentStep].therblig == therblig.INSERT) {
                MarkerX1Comp = tools.GetChildWithName (therbligComps, "InsertComp");
                compassTarget = GameObject.Find ("InsertComp");
            } else if (instructions.steps[currentStep].therblig == therblig.DISSEMBLE) {

            } else if (instructions.steps[currentStep].therblig == therblig.REMOVE) {

            } else if (instructions.steps[currentStep].therblig == therblig.GET) {

            } else if (instructions.steps[currentStep].therblig == therblig.UNKNOWN) {
                MarkerX1Comp = tools.GetChildWithName (therbligComps, "MarkerX1Comp");
                compassTarget = GameObject.Find ("MarkerX1Comp");
            } else if (instructions.steps[currentStep].therblig == therblig.PULL) {
                MarkerX1Comp = tools.GetChildWithName (therbligComps, "PullComp");
                compassTarget = GameObject.Find ("PullComp");
            } else if (instructions.steps[currentStep].therblig == therblig.PUSH) {
                MarkerX1Comp = tools.GetChildWithName (therbligComps, "PushComp");
                compassTarget = GameObject.Find ("PushComp");
            } else if (instructions.steps[currentStep].therblig == therblig.ALIGN) {
                MarkerX1Comp = tools.GetChildWithName (therbligComps, "AlignComp");
                compassTarget = GameObject.Find ("AlignComp");
            } else if (instructions.steps[currentStep].therblig == therblig.SCREW) {
                MarkerX1Comp = tools.GetChildWithName (therbligComps, "ScrewComp");
                compassTarget = GameObject.Find ("ScrewComp");
            } else if (instructions.steps[currentStep].therblig == therblig.TWOHANDGRASP) {
                MarkerX1Comp = tools.GetChildWithName (therbligComps, "TwoHandGraspComp");
                compassTarget = tools.GetChildWithName (tools.GetChildWithName (tools.GetChildWithName (MarkerX1Comp, "grasp-claw-fingers"), "palm"), "Sphere");

                ///Sphere
            } else {
                MarkerX1Comp = GameObject.Find ("MarkerX1Comp");
                compassTarget = GameObject.Find ("MarkerX1Comp");
            }

            // If therblig can't be found we default to the original arrow
            if (MarkerX1Comp == null) {
                MarkerX1Comp = GameObject.Find ("MarkerX1Comp");
                compassTarget = GameObject.Find ("MarkerX1Comp");
            }
        } // If therblig can't be found we default to the original arrow
        else if (MarkerX1Comp == null) {
            MarkerX1Comp = GameObject.Find ("MarkerX1Comp");
            compassTarget = GameObject.Find ("MarkerX1Comp");
        }
    }

    void makeArrowPointTarget (GameObject compassTarget) {
        GameObject mainCamera = Camera.main.gameObject;

        GameObject target = compassTarget;

        var target_position = mainCamera.transform.InverseTransformPoint (target.transform.position);

        var positionProjected = new Vector3 (target_position.x, target_position.y, 0); //Ignore Z

        var target_position_normalized = positionProjected.normalized;

        float target_angle = Mathf.Rad2Deg * Mathf.Acos (target_position_normalized.x);

        if (target_position_normalized.y < 0) {
            target_angle = -target_angle;
        }
        var speed = 0.01f;
        var offset = 90;

        arrowCompass.transform.localRotation = Quaternion.Lerp (arrowCompass.transform.rotation, Quaternion.Euler (0, 0, target_angle - offset), Time.time * speed);

    }

}