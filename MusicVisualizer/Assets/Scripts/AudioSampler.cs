using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSampler : MonoBehaviour
{
    public AudioSource audioSource;
    float[] samples = new float[512];
    public static float[] freqBands = new float[8];
    float[] bandBuffers = new float[8];
    public float[] bufferDecrease = new float[8];


    public float[] freqBandHighest = new float[8];
    public static float[] audioBand = new float[8];
    public static float[] audioBandBuffer = new float[8];

    // Update is called once per frame
    void Update()
    {
        GetSamples();
        MakeFreqBands();
        BandBuffer();
        CreateAudioBand();
    }

    public void GetSamples()
    {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }
    public void CreateAudioBand(){
        for (int i = 0; i < 8; i++)
        {
            if(freqBands[i]> freqBandHighest[i]){
                freqBandHighest[i] = freqBands[i];
            }
            audioBand[i] = (freqBands[i] / freqBandHighest[i]);
            audioBandBuffer[i] = (bandBuffers[i] / freqBandHighest[i]);
        }
    }
    public void MakeFreqBands()
    {
        /*
         * 
         * 
         * 
         * 
         */
        int count = 0;
        for (int i = 1; i <= 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i);

            for (int j = 0; j < sampleCount; j++)
            {
                average += samples[count] * (count + 1);
                count++;
            }
            average /= count;
            freqBands[i - 1] = average * 10;
        }
    }

    public void BandBuffer()
    {
        for (int i = 0; i < 8; i++)
        {
            if (freqBands[i] > bandBuffers[i])
            {
                bandBuffers[i] = freqBands[i];
                bufferDecrease[i] = 0.005F;
            }
            if (freqBands[i] < bandBuffers[i])
            {
                bandBuffers[i] -= bufferDecrease[i];
                bufferDecrease[i] *= 1.2f;
            }

        }
    }
}
