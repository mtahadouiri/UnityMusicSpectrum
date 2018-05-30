using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeVisualizer : MonoBehaviour
{
    public int index;
    public int maxScale;
    public int minScale;
    public bool useBuffer;
    public bool sphere;
    public Material material;
    public Color emitterColor;
    public float intensity;
    private float initY;
    // Use this for initialization
    void Start()
    {
        initY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if ((AudioSampler.audioBandBuffer[index]) > 0){
            if (useBuffer)
            {
                if (!sphere)
                {
                    transform.localScale = new Vector3(1, (AudioSampler.audioBandBuffer[index] * maxScale) + 1, 1);
                    transform.position = new Vector3(transform.position.x, transform.localScale.y / 2 + initY, transform.position.z);
                }
                else
                {
                    transform.localScale = new Vector3((AudioSampler.audioBandBuffer[index] * maxScale) + 1, (AudioSampler.audioBandBuffer[index] * maxScale) + 1, (AudioSampler.audioBandBuffer[index] * maxScale) + 1);
                    transform.position = new Vector3(transform.position.x, transform.localScale.y / 2 + initY, transform.position.z);

                }

                material.SetColor("_EmissionColor", emitterColor * intensity * AudioSampler.audioBandBuffer[index]);
            }
            else
            {
                if (!sphere)
                {
                    transform.localScale = new Vector3(1, (AudioSampler.audioBand[index] * maxScale) + 1, 1);
                    transform.position = new Vector3(transform.position.x, transform.localScale.y / 2 + initY, transform.position.z);
                }
                else
                {
                    transform.localScale = new Vector3((AudioSampler.audioBandBuffer[index] * maxScale) + 1, (AudioSampler.audioBandBuffer[index] * maxScale) + 1, (AudioSampler.audioBandBuffer[index] * maxScale) + 1);
                    transform.position = new Vector3(transform.position.x, transform.localScale.y / 2 + initY, transform.position.z);
                }
                material.SetColor("_EmissionColor", emitterColor * intensity * AudioSampler.audioBandBuffer[index]);
            }
        }
        
    }
}
