using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider _sliderMusicVolume;
    [SerializeField] private Slider _sliderSFXVolume;
    [SerializeField] private Slider _sliderFOV;

    [SerializeField] private AudioMixer _musicMixer;
    [SerializeField] private AudioMixer _SFXMixer;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private CinemachineVirtualCamera cam;

    public void AdjustMusicVolume()
    {
        _musicMixer.SetFloat(name:"BG_Music", _sliderMusicVolume.value);
    }

    public void AdjustSFXVolume()
    {
        _SFXMixer.SetFloat(name: "SFX", _sliderSFXVolume.value);
    }

    public void AdjustFOV()
    {
        cam.m_Lens.FieldOfView = _sliderFOV.value;
    }

    public void Back()
    {
        foreach (Transform child in pauseMenu.transform)
            child.gameObject.SetActive(true);

        this.gameObject.SetActive(false);
    }

    public void RestoreDefaults()
    {
        _sliderMusicVolume.value = 0;
        _sliderSFXVolume.value = 0;
        _sliderFOV.value = 40;

        _musicMixer.SetFloat(name: "BG_Music", _sliderMusicVolume.value);
        _SFXMixer.SetFloat(name: "SFX", _sliderSFXVolume.value);
        cam.m_Lens.FieldOfView = _sliderFOV.value;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
