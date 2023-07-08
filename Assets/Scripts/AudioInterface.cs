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
    public Sound[] = sounds;

    static AudioInterface _i;
    private void Awake()
    {
        if(_i != null)
            Destroy(_i.gameObject);
        _i = this;

        foreach(E_Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }

    }

    public static void play(E_Sound sound)
    {
        switch(sound)
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
                _i.playPieceWithSpawn();
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
        .source.Play();
    }

    void playPieceSelect()
    {

    }

    void playPieceMove()
    {

    }

    void playPieceWithSpawn()
    {

    }

    void playPieceDemote()
    {

    }

    void playWin()
    {

    }

    void playLoss()
    {

    }

    void playButtonClick()
    {

    }

    void playButtonHover()
    {

    }
}
