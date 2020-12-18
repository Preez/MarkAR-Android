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
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
class tools : MonoBehaviour {

    public static GameObject GetChildWithName (GameObject obj, string name) {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find (name);
        if (childTrans != null) {
            return childTrans.gameObject;
        } else {
            return null;
        }
    }

    public static GameObject GetAnyChild (GameObject parent) {
        Transform trans = parent.transform;
        Transform childTrans = trans.GetChild (0);

        if (childTrans != null) {
            return childTrans.gameObject;
        }

        return null;
    }
    public static Vector3 createPosition (float x, float y, float z) {
        return new Vector3 (x, y, z);
    }

    public static Quaternion fromEulerToQuaternion (float X, float Y, float Z) {
        Quaternion myRotation = Quaternion.identity;
        myRotation.eulerAngles = new Vector3 (X, Y, Z);
        return myRotation;
    }

    public static void writeCSVSummary (List<stepObject> stepList) {
        //string path = "Assets/Resources/targets.txt";

        //Write some text to the CurrentData.txt file
        //Writes to C:\Users\jjavi\AppData\LocalLow\DefaultCompany\IndependentStudyProjectV1-ANDROID

        string data = "";
        foreach (stepObject step in stepList) {
            data += step.instructionStepText+" , "+step.therblig+"\n";
        }

        StreamWriter writer = new StreamWriter (Application.persistentDataPath + "/therbligsCSVSummary.txt", false);
        writer.WriteLine (data);

        // //writer.AutoFlush = true;
        writer.Close ();
    }
}