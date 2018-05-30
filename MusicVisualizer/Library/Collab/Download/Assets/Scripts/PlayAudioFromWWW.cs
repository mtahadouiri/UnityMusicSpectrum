using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class PlayAudioFromWWW : MonoBehaviour {

    public enum SeekDirection { Forward, Backward }

    public AudioSource source;
    public List<AudioClip> clips = new List<AudioClip>();
    public IntPtr handle_mpg;
    public IntPtr errPtr;
    public IntPtr rate;
    public IntPtr channels;
    public IntPtr encoding;
    public IntPtr id3v1;
    public IntPtr id3v2;
    public IntPtr done;
    public TextMeshPro songName;
    public int x;
    public int intRate;
    public int intChannels;
    public int intEncoding;
    public int FrameSize;
    public int lengthSamples;
    public AudioClip myClip;
    [SerializeField] [HideInInspector] private int currentIndex = 0;

    private FileInfo[] soundFiles;
    private List<string> validExtensions = new List<string> { ".ogg", ".wav" ,".mp3" }; // Don't forget the "." i.e. "ogg" won't work - cause Path.GetExtension(filePath) will return .ext, not just ext.
    private string absolutePath = "./music"; // relative path to where the app is running - change this to "./music" in your case

    #region Consts: Standard values used in almost all conversions.
    private const float const_1_div_128_ = 1.0f / 128.0f;  // 8 bit multiplier
    private const float const_1_div_32768_ = 1.0f / 32768.0f; // 16 bit multiplier
    private const double const_1_div_2147483648_ = 1.0 / 2147483648.0; // 32 bit
    #endregion

    void Start()
    {
        //being able to test in unity
        if (Application.isEditor) absolutePath = "Assets/";

        if (source == null) source = gameObject.AddComponent<AudioSource>();

        ReloadSounds();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Previous"))
        {
            Seek(SeekDirection.Backward);
            PlayCurrent();
        }
        if (GUILayout.Button("Play current"))
        {
            PlayCurrent();
        }
        if (GUILayout.Button("Next"))
        {
            Seek(SeekDirection.Forward);
            PlayCurrent();
        }
        if (GUILayout.Button("Reload"))
        {
            ReloadSounds();
        }
        if (GUILayout.Button("Close"))
        {
            Application.Quit();
        }
    }

    void Seek(SeekDirection d)
    {
        if (d == SeekDirection.Forward)
            currentIndex = (currentIndex + 1) % clips.Count;
        else
        {
            currentIndex--;
            if (currentIndex < 0) currentIndex = clips.Count - 1;
        }
    }

    void Update()
    {
        if(source.time >= source.clip.length - 0.3f)
        {
            Seek(SeekDirection.Forward);
            PlayCurrent();
        }
    }

    void PlayCurrent()
    {
        source.clip = clips[currentIndex];
        String name = source.clip.name.Substring(0, source.clip.name.Length - 4);
        songName.text = name;
        source.Play();
    }

    void ReloadSounds()
    {
        clips.Clear();
        // get all valid files
        var info = new DirectoryInfo(absolutePath);
        Debug.Log(info);
        soundFiles = info.GetFiles()
            .Where(f => IsValidFileType(f.Name))
            .ToArray();

        // and load them
        foreach (var s in soundFiles)
            StartImport(s.FullName);
        //StartCoroutine(LoadFile(s.FullName));
        PlayCurrent();
    }

    bool IsValidFileType(string fileName)
    {
        return validExtensions.Contains(Path.GetExtension(fileName));
        // Alternatively, you could go fileName.SubString(fileName.LastIndexOf('.') + 1); that way you don't need the '.' when you add your extensions
    }

    IEnumerator LoadFile(string path)
    {
        WWW www = new WWW("file://" + path);
        print("loading " + path);

        AudioClip clip = www.GetAudioClip(false);
        while (clip.loadState != AudioDataLoadState.Loaded)
            yield return www;

        print("done loading");
        clip.name = Path.GetFileName(path);
        clips.Add(clip);
    }

    public void StartImport(String mPath)
    {
        source = (AudioSource)gameObject.GetComponent(typeof(AudioSource));
        if (source == null) source = (AudioSource)gameObject.AddComponent<AudioSource>();
        MPGImport.mpg123_init();
        handle_mpg = MPGImport.mpg123_new(null, errPtr);
        x = MPGImport.mpg123_open(handle_mpg, mPath);
        MPGImport.mpg123_getformat(handle_mpg, out rate, out channels, out encoding);
        intRate = rate.ToInt32();
        intChannels = channels.ToInt32();
        intEncoding = encoding.ToInt32();

        MPGImport.mpg123_id3(handle_mpg, out id3v1, out id3v2);
        MPGImport.mpg123_format_none(handle_mpg);
        MPGImport.mpg123_format(handle_mpg, intRate, intChannels, 208);

        FrameSize = MPGImport.mpg123_outblock(handle_mpg);
        byte[] Buffer = new byte[FrameSize];
        lengthSamples = MPGImport.mpg123_length(handle_mpg);

        myClip = AudioClip.Create("myClip", lengthSamples, intChannels, intRate, false, false);

        int importIndex = 0;

        while (0 == MPGImport.mpg123_read(handle_mpg, Buffer, FrameSize, out done))
        {


            float[] fArray;
            fArray = ByteToFloat(Buffer);

            myClip.SetData(fArray, (importIndex * fArray.Length) / 2);

            importIndex++;
        }

        MPGImport.mpg123_close(handle_mpg);

        print("done loading");
        myClip.name = Path.GetFileName(mPath);
        clips.Add(myClip);
    }

    public float[] IntToFloat(Int16[] from)
    {
        float[] to = new float[from.Length];

        for (int i = 0; i < from.Length; i++)
            to[i] = (float)(from[i] * const_1_div_32768_);

        return to;
    }

    public Int16[] ByteToInt16(byte[] buffer)
    {
        Int16[] result = new Int16[1];
        int size = buffer.Length;
        if ((size % 2) != 0)
        {
            /* Error here */
            Console.WriteLine("error");
            return result;
        }
        else
        {
            result = new Int16[size / 2];
            IntPtr ptr_src = Marshal.AllocHGlobal(size);
            Marshal.Copy(buffer, 0, ptr_src, size);
            Marshal.Copy(ptr_src, result, 0, result.Length);
            Marshal.FreeHGlobal(ptr_src);
            return result;
        }
    }

    public float[] ByteToFloat(byte[] bArray)
    {
        Int16[] iArray;

        iArray = ByteToInt16(bArray);

        return IntToFloat(iArray);
    }

}
