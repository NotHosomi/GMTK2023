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
    GameObject failhighlight = null;
    static GameObject highlight_prefab;
    static GameObject failhighlight_prefab;
    public static void onStart(GameObject _highlight_prefab, GameObject _fail_prefab)
    {
        s_moveSinceLastUncapture[0] = 0;
        s_moveSinceLastUncapture[1] = 0;
        highlight_prefab = _highlight_prefab;
        failhighlight_prefab = _fail_prefab;
    }

    public Square(int x, int y)
    {
        m_x = x;
        m_y = y;
        highlight = GameObject.Instantiate(highlight_prefab, new Vector3(x, y, 2), Quaternion.identity);
        failhighlight = GameObject.Instantiate(failhighlight_prefab, new Vector3(x, y, 2), Quaternion.identity);
        failhighlight.SetActive(false);
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
        team = Player.otherPlayer(team);
        E_PieceType spawned = E_PieceType.None;

        if (Piece.s_tPieces[(int)team].nPieces >= 16)
        {
            return spawned;
        }
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
        List<E_PieceType> possibilities = new List<E_PieceType>();
        possibilities.Add(E_PieceType.Pawn);
        possibilities.Add(E_PieceType.Rook);
        possibilities.Add(E_PieceType.Hors);
        possibilities.Add(E_PieceType.Bish);
        possibilities.Add(E_PieceType.Quee);

        bool valid;
        E_PieceType type;
        do
        {
            int index = Random.Range(0, possibilities.Count);
            type = possibilities[index];
            // TODO: this needs more validation
            switch (type)
            {
                case E_PieceType.Pawn:
                    valid = Piece.validatePawn(team) && !(m_y == 0 && team == E_Team.White || m_y == 7 && team == E_Team.Black);
                    break;
                case E_PieceType.Bish:
                    valid = Piece.validateBishop(team, m_x, m_y);
                    break;
                default:
                    valid = Piece.validateOther(type, team);
                    break;
            }
            if (!valid)
                possibilities.RemoveAt(index);
            if(possibilities.Count == 0)
            {
                Debug.Log("Was not able to spawn a piece");
                return E_PieceType.None;
            }
        } while (!valid);

        Board._i.createPiece(m_x, m_y, type, team);
        s_moveSinceLastUncapture[(int)team] = 0;
        return type;
    }

    public void setMoveSelector(bool show)
    {
        highlight.SetActive(show);
    }

    public void setFailHint(bool state)
    {
        failhighlight.SetActive(state);
    }
}
