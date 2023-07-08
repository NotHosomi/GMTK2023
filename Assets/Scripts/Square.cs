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
    static int[] s_moveSinceLastUncapture = { 0, 0 };
    int m_x;
    int m_y;
    Piece m_occupant = null;
    GameObject highlight = null;
    static GameObject highlight_prefab;
    public static void assignHighlightPrefab(GameObject prefab)
    {
        highlight_prefab = prefab;
    }

    public Square(int x, int y)
    {
        m_x = x;
        m_y = y;
        highlight = GameObject.Instantiate(highlight_prefab, new Vector3(x, y, 2), Quaternion.identity);
    }

    public void occupy(Piece piece)
    {
        m_occupant = piece;
    }
    public Piece getOccupant()
    {
        return m_occupant;
    }

    public void deoccupy()
    {
        m_occupant = null;
    }

    public E_PieceType rollSpawn(E_Team team, E_SpawnCmd spawn_flag = E_SpawnCmd.chance)
    {
        const int MAX_SPAWNLESS = 8;
        E_PieceType spawned = E_PieceType.None;
        switch (spawn_flag)
        {
            case E_SpawnCmd.cannot:
                if (++s_moveSinceLastUncapture[(int)team] > MAX_SPAWNLESS)
                    s_moveSinceLastUncapture[(int)team] = MAX_SPAWNLESS;
                break;
            case E_SpawnCmd.must:
                spawned = uncapture(team);
                break;
            case E_SpawnCmd.chance:
                bool proc = Random.Range(0, MAX_SPAWNLESS + 1 - s_moveSinceLastUncapture[(int)team]) == 0;
                if (proc)
                    spawned = uncapture(team);
                else
                    ++s_moveSinceLastUncapture[(int)team];
                break;
        }
        return spawned;
    }

    E_PieceType uncapture(E_Team team)
    {
        // TODO: this needs more validation
        E_PieceType type = (E_PieceType)Random.Range(0, (int)E_PieceType.King);
        bool valid;
        switch (type)
        {
            case E_PieceType.Pawn:
                valid = Piece.validatePawn(team);
                break;
            case E_PieceType.Bish:
                valid = Piece.validateBishop(team, m_x, m_y);
                break;
            default:
                valid = Piece.validateOther(type, team);
                break;
        }
        if (!valid)
            return E_PieceType.None;

        Board._i.createPiece(m_x, m_y, type, team);
        s_moveSinceLastUncapture[(int)team] = 0;
        return type;
    }

    public void setMoveSelector(bool show)
    {
        highlight.SetActive(show);
    }
}
