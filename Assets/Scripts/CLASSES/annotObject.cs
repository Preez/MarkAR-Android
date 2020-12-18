using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

[System.Serializable]
/// <summary> Represents the annotation object</summary>
class annotObject {

    public Matrix4x4 H_Annot_to_World {
        get;
        set;
    }

    public Matrix4x4 H_Annot_to_T1 {
        get;
        set;
    }

    public Matrix4x4 H_Annot_to_Target {
        get;
        set;
    }

    public string annotation {
        get;
        set;
    }

    public Quaternion rotation {
        get;
        set;
    }

    public Vector3 position {
        get;
        set;
    }

    public annotObject (string annotation, Quaternion rotation, Vector3 position) {
        this.annotation = annotation;
        this.position = position;
        this.rotation = rotation;
        this.H_Annot_to_T1 = Matrix4x4.TRS (position, rotation.normalized, new Vector3 (1, 1, 1));
    }

    public annotObject genericProperty { get; set; }
}