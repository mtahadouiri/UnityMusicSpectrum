using UnityEngine;
public class InstantiateSpectrum : MonoBehaviour {
    public float distance = 10;
    public GameObject prefab;
    GameObject[] _sampleCubes = new GameObject[512];
    public float maxScale;


	// Use this for initialization
	void Start () {
        for (int i = 0; i < _sampleCubes.Length; i++)
        {
            GameObject _currentCube = Instantiate(prefab,transform.position,Quaternion.identity,transform);
            _currentCube.name = "Spectre " + i;
            _currentCube.transform.position = Vector3.forward * distance;
            _sampleCubes[i] = _currentCube;
            transform.Rotate(Vector3.up, -0.703125f);

        }
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < _sampleCubes.Length; i++)
        {
            _sampleCubes[i].transform.localScale = new Vector3(1, (AudioSampler.audioBandBuffer[i] * maxScale) + 1, 1);
            _sampleCubes[i].transform.position = new Vector3(_sampleCubes[i].transform.position.x,_sampleCubes[i].transform.localScale.y / 2,_sampleCubes[i].transform.position.z)  ;
        }
    }
}
