using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class miniInstructionsHandler : MonoBehaviour {

    private GameObject Canvas, miniCanvas, instructionsText, arrowObj, arrowToTextText, arrowObjPivot, xTarget, miniInstructionsBackground, miniInstructions;

    private bool textSeenOnce = false;

    private Color whiteColor, blackColor, transparentColor;

    // Start is called before the first frame update
    void Start () {
        whiteColor.a = 255f;
        whiteColor.r = 255f;
        whiteColor.g = 255f;
        whiteColor.b = 255f;

        blackColor.a = 255f;
        blackColor.r = 0f;
        blackColor.g = 0f;
        blackColor.b = 0f;

        transparentColor.a = 0f;

        Canvas = GameObject.Find ("Canvas");

        miniCanvas = GameObject.Find ("mini-canvas");
        instructionsText = tools.GetChildWithName (miniCanvas, "instructionsText");
        xTarget = GameObject.Find ("MarkerX1Obj");

        // miniInstructionsBackground = tools.GetChildWithName (Canvas, "mini-instructions-background");
        miniInstructions = tools.GetChildWithName (Canvas, "mini-instructions");
        miniInstructionsBackground = tools.GetChildWithName (GameObject.Find ("Canvas"), "mini-instructions-background");

        miniInstructions.GetComponent<TextMeshProUGUI> ().color = transparentColor;
        miniInstructionsBackground.GetComponent<TextMeshProUGUI> ().color = transparentColor;
    }

    // Update is called once per frame
    void LateUpdate () {

        var aColor = miniInstructions.GetComponent<TextMeshProUGUI> ().color;

        if (instructionsText.GetComponent<MeshRenderer> ().isVisible) {
            textSeenOnce = true;

            miniInstructions.GetComponent<TextMeshProUGUI> ().color = transparentColor;
            miniInstructionsBackground.GetComponent<TextMeshProUGUI> ().color = transparentColor;
            // miniInstructionsBackground.GetComponent<UnityEngine.UI.Image> ().color = aColor;
        } else if (!instructionsText.GetComponent<MeshRenderer> ().isVisible && textSeenOnce == true) {
            // Change only opacity
            miniInstructions.GetComponent<TextMeshProUGUI> ().color = whiteColor;
            miniInstructionsBackground.GetComponent<TextMeshProUGUI> ().color = blackColor;

        }
    }

}