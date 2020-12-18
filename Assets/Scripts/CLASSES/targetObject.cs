using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using System.IO;
using System;
using UnityEngine.UI;

[System.Serializable]
class targetObject
{
    public GameObject imageTarget
    {
        get;
        set;
    }
    public Matrix4x4 H_Target_to_World
    {
        get;
        set;
    }

    public Matrix4x4 H_Target_to_T1
    {
        get;
        set;
    }

    public Matrix4x4 H_World_to_Target
    {
        get;
        set;
    }
    public bool hasBeenFound
    {
        get;
        set;
    }

    public string annotation
    {
        get;
        set;
    }

    public Quaternion rotation
    {
        get;
        set;
    }

    public Vector3 position
    {
        get;
        set;
    }

    public targetObject(GameObject imageTargetValue, Matrix4x4 H_Target_WorldValue, bool hasBeenFoundValue)
    {
        this.imageTarget = imageTargetValue;
        this.H_Target_to_World = H_Target_WorldValue;
        this.hasBeenFound = hasBeenFoundValue;
        this.H_Target_to_T1 = Matrix4x4.identity;
        this.H_World_to_Target = Matrix4x4.identity;
    }

    public targetObject genericProperty { get; set; }

}