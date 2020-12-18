using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public enum therblig {
    FIND,
    TRANSPORT,
    GRASP,
    HOLD,
    RELEASE,
    POSITION,
    PLACE,
    ASSEMBLE,
    USE,
    INSPECT,
    INSERT,
    DISSEMBLE,
    REMOVE,
    GET,
    UNKNOWN,
    PULL,
    PUSH,
    SCREW,
    ALIGN,
    LOOK,
    TWOHANDGRASP
}

[System.Serializable]
/// <summary> Step(s) for the instructions</summary>
class stepObject {

    public therblig therblig {
        get;
        set;
    }

    /// <summary> The instruction text for the current step</summary>
    public string instructionStepText {
        get;
        set;
    }

    /// <summary> All the annotation(s) required to complete the current step</summary>
    public List<annotObject> annotations {
        get;
        set;
    }

    /// <summary> Image that may come with the step</summary>
    public Texture2D image {
        get;
        set;
    }

    /// <summary> Constructor </summary>
    public stepObject (string stepInstruction, therblig therblig) {
        this.instructionStepText = stepInstruction;
        this.annotations = new List<annotObject> ();
        this.therblig = therblig;
    }

    public stepObject genericProperty { get; set; }

}