using System.Collections;
using System.Collections.Generic;
using EZCameraShake;
using UnityEngine;

public class CameraMusicShaker : MonoBehaviour
{
    CameraShakeInstance shake;
    public bool _shake;
    public Vector3 posInf = new Vector3(0.25f, 0.25f, 0.25f);
    public Vector3 rotInf = new Vector3(1, 1, 1);
    public float magn = 3, rough = 3, fadeIn = 0.1f, fadeOut = 1f;

    bool modValues;

    // Update is called once per frame
    void Update()
    {
        if (_shake)
        {
 print("Shaking");
            CameraShaker.Instance.StopAllCoroutines();
        CameraShakeInstance c = CameraShaker.Instance.ShakeOnce(AudioSampler.freqBands[0], AudioSampler.freqBands[0], fadeIn, fadeOut);
            c.PositionInfluence = posInf;
            c.RotationInfluence = rotInf;
        }
           




    }
}
