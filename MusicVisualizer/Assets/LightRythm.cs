using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRythm : MonoBehaviour {
    public int index;
    public Light light;
    public float min, max;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        light.intensity = (AudioSampler.audioBandBuffer[index] * (max - min) + min);
	}
}
