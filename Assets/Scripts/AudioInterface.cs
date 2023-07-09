using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using UnityEngine;

public enum E_Sound
{
    PieceHover,
    PieceSelect,
    PieceMove,
    PieceMoveWithSpawn,
    PieceDemote,
    Win,
    Loss,
    ButtonClick,
    ButtonHover
}

public class AudioInterface : MonoBehaviour
{
    [SerializeField] public Sound[] sounds;
    public bool MuteMusic = false;

    static AudioInterface _i;
    private void Awake()
    {
        if(_i != null)
            Destroy(_i.gameObject);
        _i = this;

       foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    public static void play(E_Sound soundId)
    {
        switch(soundId)
        {
            case E_Sound.PieceHover:
                _i.playPieceHover();
                break;
            case E_Sound.PieceSelect:
                _i.playPieceSelect()
;               break;
            case E_Sound.PieceMove:
                _i.playPieceMove();
                break;
            case E_Sound.PieceMoveWithSpawn:
                _i.playPieceMoveWithSpawn();
                break;
            case E_Sound.PieceDemote:
                _i.playPieceDemote();
                break;
            case E_Sound.Win:
                _i.playWin();
                break;
            case E_Sound.Loss:
                _i.playLoss();
                break;
            case E_Sound.ButtonClick:
                _i.playButtonClick();
                break;
            case E_Sound.ButtonHover:
                _i.playButtonHover();
                break;
            default:
                break;
        }
    }


    void playPieceHover()
    {
        AudioSource asource = gameObject.GetComponent<AudioSource>();
        asource.clip = Array.Find(sounds, sounds => sounds.name == "hover").clip;
        asource.Play();
    }

    void playPieceSelect()
    {
        AudioSource asource = gameObject.GetComponent<AudioSource>();
        asource.clip = Array.Find(sounds, sounds => sounds.name == "select").clip;
        asource.Play();
    }

    void playPieceMove()
    {
        AudioSource asource = gameObject.GetComponent<AudioSource>();
        asource.clip = sounds[UnityEngine.Random.Range(1, 6)].clip;
        asource.Play();
    }

    void playPieceMoveWithSpawn()
    {
        // temp
        playPieceMove();
    }

    void playPieceDemote()
    {
        // temp
        playPieceMove();
    }

    void playWin()
    {
        AudioSource asource = gameObject.GetComponent<AudioSource>();
        asource.clip = Array.Find(sounds, sounds => sounds.name == "win").clip;
        asource.Play(); 
    }

    void playLoss()
    {
        AudioSource asource = gameObject.GetComponent<AudioSource>();
        asource.clip = Array.Find(sounds, sounds => sounds.name == "invalid_action").clip;
        asource.Play();
    }

    void playButtonClick()
    {
        AudioSource asource = gameObject.GetComponent<AudioSource>();
        asource.clip = Array.Find(sounds, sounds => sounds.name == "button_down").clip;
        asource.Play();
    }

    void playButtonHover()
    {
        // temp
        playPieceHover();
    }

    public void clickMute()
    {
        MuteMusic = !MuteMusic;

        GameObject canvas = GameObject.Find("Canvas");
        AudioSource asource = canvas.GetComponent<AudioSource>();
        asource.mute = MuteMusic;

        GameObject button_sprite = GameObject.Find("button_mute");
        button_sprite.SetActive(MuteMusic);

        Debug.Log("Mute Music");
    }
}
