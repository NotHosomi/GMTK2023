using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_SpawnCmd
{
    chance,
    cannot,
    must
}

public class Square
{
    static int s_moveSinceLastUncapture = 0;
    int m_x;
    int m_y;
    Piece m_occupant = null;
    GameObject highlight = null;

    public Square(int x, int y)
    {
        m_x = x;
        m_y = y;
        // TODO grab highlight object
    }

    public void deoccupy(E_SpawnCmd pawn_flag = E_SpawnCmd.chance)
    {
        switch (pawn_flag)
        {
            case E_SpawnCmd.cannot:
                m_occupant = null;
                break;
            case E_SpawnCmd.must:
                spawn();
                break;
            case E_SpawnCmd.chance:
                bool proc = Random.Range(0, 9) == 0;
                if (proc)
                    spawn();
                else
                    m_occupant = null;
                break;
        }
    }

    void spawn()
    {
        // TODO: this needs more validation
        E_PieceType type = (E_PieceType)Random.Range(0, 4);
        E_Team team = m_occupant.getTeam() == E_Team.Black ? E_Team.White : E_Team.Black;
        Board._i.createPiece(m_x, m_y, type, team);
    }

    public void occupy(Piece piece)
    {
        m_occupant = piece;
    }
    public Piece getOccupant()
    {
        return m_occupant;
    }
}
