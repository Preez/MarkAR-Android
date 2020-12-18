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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class alwaysFacesCamera : MonoBehaviour {
    // Start is called before the first frame update
    void Start () { }

    // Update is called once per frame
    // Makes any object's children look at the camera
    void Update () {
        Transform trans = this.transform;
        Transform thisChild = trans.GetChild (0);
        //thisChild.transform.LookAt (thisChild.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        thisChild.transform.LookAt (thisChild.transform.position + Camera.main.transform.rotation * Vector3.down);
    }

}