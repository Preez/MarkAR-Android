using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

[System.Serializable]
/// <summary> The group of steps that generate an instruction(s)</summary>
class instructionObject {

    /// <summary> List of step(s) for the instruction</summary>
    public List<stepObject> steps = new List<stepObject> ();
    
    public instructionObject genericProperty { get; set; }

}