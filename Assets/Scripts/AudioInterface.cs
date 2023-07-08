using System.Collections;
using System.Collections.Generic;
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
    AudioInterface _i;
    private void Awake()
    {
        if(_i != null)
            Destroy(_i.gameObject);
        _i = this;
    }

    public void play(E_Sound sound)
    {
        switch(sound)
        {
            case E_Sound.PieceHover:
                playPieceHover();
                break;
            case E_Sound.PieceSelect:
                playPieceSelect()
;                break;
            case E_Sound.PieceMove:
                playPieceMove();
                break;
            case E_Sound.PieceMoveWithSpawn:
                playPieceWithSpawn();
                break;
            case E_Sound.PieceDemote:
                playPieceDemote();
                break;
            case E_Sound.Win:
                playWin();
                break;
            case E_Sound.Loss:
                playLoss();
                break;
            case E_Sound.ButtonClick:
                playButtonClick();
                break;
            case E_Sound.ButtonHover:
                playButtonHover();
                break;
            default:
                break;
        }
    }

    void playPieceHover()
    {

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
