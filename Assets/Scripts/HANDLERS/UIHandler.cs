using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
class UIHandler : MonoBehaviour {

    // public static
    public static void showArrowCompass () {
        GameObject arrowCompass = GameObject.Find ("compass");
        Color displayColor = arrowCompass.GetComponent<UnityEngine.UI.Image> ().color;
        displayColor.a = 1f;
        arrowCompass.GetComponent<UnityEngine.UI.Image> ().color = displayColor;
    }
    public static void hideArrowCompass () {
        GameObject arrowCompass = GameObject.Find ("compass");
        Color invisibleColor = arrowCompass.GetComponent<UnityEngine.UI.Image> ().color;
        invisibleColor.a = 0f;
        arrowCompass.GetComponent<UnityEngine.UI.Image> ().color = invisibleColor;
    }

    public static void hideImageUI () {
        GameObject RawImage = tools.GetChildWithName (GameObject.Find ("Canvas"), "RawImage");
        GameObject rawImageText = tools.GetChildWithName (RawImage, "rawImageText");
        GameObject rawImageTextBackground = tools.GetChildWithName (RawImage, "raw-image-text-background");

        Color aColor = RawImage.GetComponent<RawImage> ().color;
        aColor.a = 0f;
        RawImage.GetComponent<RawImage> ().color = aColor;
        RawImage.GetComponent<RawImage> ().texture = null;
        rawImageText.GetComponent<TextMeshProUGUI> ().color = aColor;
        rawImageTextBackground.GetComponent<UnityEngine.UI.Image> ().color = aColor;
    }

    public static void displayImageUI (instructionObject instructions, int currentStep) {
        GameObject RawImage = tools.GetChildWithName (GameObject.Find ("Canvas"), "RawImage");
        GameObject rawImageText = tools.GetChildWithName (RawImage, "rawImageText");
        GameObject rawImageTextBackground = tools.GetChildWithName (RawImage, "raw-image-text-background");

        var aColor = RawImage.GetComponent<RawImage> ().color;
        aColor.a = 1f;
        RawImage.GetComponent<RawImage> ().color = aColor;
        RawImage.GetComponent<RawImage> ().texture = instructions.steps[currentStep].image;
        rawImageText.GetComponent<TextMeshProUGUI> ().color = aColor;

        Color lightBlack;
        //Set RGBA for colors
        lightBlack.a = 0.50f;
        lightBlack.r = 0;
        lightBlack.g = 0;
        lightBlack.b = 0;

        rawImageTextBackground.GetComponent<UnityEngine.UI.Image> ().color = lightBlack;
    }


}