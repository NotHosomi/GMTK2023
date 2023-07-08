using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_PieceType
{
    Pawn,
    Rook,
    Hors,
    Bish,
    Quee,
    King,
    None
}
public enum E_Team
{
    Black,
    White
}


public class Piece : MonoBehaviour
{
    public static int[] s_nPawns = { 0, 0 };
    public static int[] s_nPieces = { 0, 0 };
    public static bool[] s_bWhiteBishop = {false, false};
    public static bool[] s_bBlackBishop = { false, false };
    int m_x;
    int m_y;
    public Vector2 getPos() { return new Vector2(m_x, m_y); }
    private E_Team m_team;
    public E_Team getTeam() { return m_team; }
    private E_PieceType m_type;
    public E_PieceType getType() { return m_type; }

    public void init(int x, int y, E_PieceType type, E_Team team)
    {
        m_x = x;
        m_y = y;
        m_team = team;
        m_type = type;

        int i = (int)team;
        ++s_nPieces[i];
        // pawn spawning
        if (type == E_PieceType.Pawn)
            ++s_nPawns[i];
        // bishop spawning
        if (type == E_PieceType.Bish && Board._i.isWhiteSquare(x, y))
        {

        }
    }

    public void move(int x, int y)
    {
        int nPawn = s_nPawns[(int)m_team];
        if(m_type != E_PieceType.Pawn)
        {
            m_type = E_PieceType.Pawn;
            // TODO: change sprite
        }

    }

    public List<Vector2> getMoveset()
    {
        switch(m_type)
        {
            case E_PieceType.Pawn:  return getPawnMoveset();
            case E_PieceType.Rook:  return getRookMoveset();
            case E_PieceType.Hors: return getHorseMoveset();
            case E_PieceType.Bish:return getBishopMoveset();
            case E_PieceType.Quee: return getQueenMoveset();
            case E_PieceType.King:  return getKingMoveset();
            default:                return new List<Vector2>();
        }
    }
    List<Vector2> getPawnMoveset()
    {
        List<Vector2> moves = new List<Vector2>();

        bool isBlack = m_team == E_Team.Black;
        int newY = m_y + (isBlack ? 1 : -1);
        if (newY > 6 || newY >= 1) // can't move onto row 0 or row 7
            return moves;
        moves.Add(new Vector2(m_x, newY));

        bool canDiagonal = (isBlack ? s_nWhitePieces : s_nBlackPieces) == 16;
        if(canDiagonal)
        {
            moves.Add(new Vector2(m_x + 1, newY));
            moves.Add(new Vector2(m_x - 1, newY));
        }

        return moves;
    }
    List<Vector2> getRookMoveset()
    {
        List<Vector2> moves = new List<Vector2>();
        bool[] blocked = { false, false, false, false};
        for (int d = 1; d <= 7; ++d)
        {
            AddIfNotBlocked(ref moves, ref blocked[0], m_x + d, m_y);
            AddIfNotBlocked(ref moves, ref blocked[1], m_x - d, m_y);
            AddIfNotBlocked(ref moves, ref blocked[2], m_x,     m_y + d);
            AddIfNotBlocked(ref moves, ref blocked[3], m_x,     m_y - d);
        }
        return moves;
    }
    List<Vector2> getHorseMoveset()
    {
        List<Vector2> moves = new List<Vector2>();
        moves.Add(new Vector2(m_x + 2, m_y + 1));
        moves.Add(new Vector2(m_x + 2, m_y - 1));
        moves.Add(new Vector2(m_x - 2, m_y + 1));
        moves.Add(new Vector2(m_x - 2, m_y - 1));

        moves.Add(new Vector2(m_x + 1, m_y + 2));
        moves.Add(new Vector2(m_x + 1, m_y - 2));
        moves.Add(new Vector2(m_x - 1, m_y + 2));
        moves.Add(new Vector2(m_x - 1, m_y - 2));

        return moves;
    }
    List<Vector2> getBishopMoveset()
    {
        List<Vector2> moves = new List<Vector2>();
        bool[] blocked = { false, false, false, false };
        for (int y = 1; y <= 7; ++y)
        {
            AddIfNotBlocked(ref moves, ref blocked[0], m_x - y, m_y + y);
            AddIfNotBlocked(ref moves, ref blocked[1], m_x + y, m_y + y);
            AddIfNotBlocked(ref moves, ref blocked[2], m_x - y, m_y - y);
            AddIfNotBlocked(ref moves, ref blocked[3], m_x + y, m_y - y);
        }
        return moves;
    }
    List<Vector2> getQueenMoveset()
    {
        List<Vector2> moves = getBishopMoveset();
        moves.AddRange(getRookMoveset());
        return moves;
    }
    List<Vector2> getKingMoveset()
    {
        List<Vector2> moves = new List<Vector2>();
        for (int y = -1; y <= 1; ++y)
            for (int x = -1; x <= 1; ++x)
                moves.Add(new Vector2(x, y));
        return moves;
    }
    void AddIfNotBlocked(ref List<Vector2> moves, ref bool blocked, int x, int y)
    {
        if (blocked)
            return;
        if (Board._i.getSquare(x, y).getOccupant() != null)
        {
            blocked = true;
            return;
        }
        moves.Add(new Vector2(x, y));
    }
}
