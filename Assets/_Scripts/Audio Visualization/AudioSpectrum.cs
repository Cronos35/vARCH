using UnityEngine;
using Assets.WasapiAudio.Scripts.Unity;

public class AudioSpectrum : MonoBehaviour
{
    public static float spectrumValue { get; private set; }
    public static float[] SpectrumData;
    [SerializeField] private AudioSource micAudioOutput;

    private WasapiAudioSource loopbackAudioSource;
    private string selectedMic = string.Empty;

    private void Awake()
    {
        loopbackAudioSource = GetComponent<WasapiAudioSource>();
        SpectrumData = new float[loopbackAudioSource.GetSpectrumSampleSize()];
    }

    private void Start()
    {

        //if (Microphone.devices.Length > 0)
        //{
        //    selectedMic = Microphone.devices[0];
        //    micAudioOutput.clip = Microphone.Start(selectedMic, true, 10, AudioSettings.outputSampleRate);
        //}

        //micAudioOutput.Play();
    }

    // Update is called once per frame
    private void Update()
    {
        SpectrumData = loopbackAudioSource.GetSpectrumData();
        //for (int i = 0; i < SpectrumData.Length; i++)
        //{
        //    Debug.Log("Spectrum Data: " + SpectrumData[i]);
        //}
    }
}
