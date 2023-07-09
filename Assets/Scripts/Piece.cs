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
public enum E_Team : int
{
    Black,
    White
}

public struct T_Team
{
    public int nPawnCandidates;
    public int[] aTypes;
    public int nPieces;
    public int nWhiteBishop;
    public int nBlackBishop;
    public List<Piece> vPieces;

    public T_Team(bool doesNothing=false)
    {
        nPawnCandidates = 0;
        aTypes = new int[] { 0, 0, 0, 0, 0, 0 };
        nPieces = 0;
        nWhiteBishop = 0;
        nBlackBishop = 0;
        vPieces = new List<Piece>();
    }

}

public class Piece : MonoBehaviour
{
    public static int[] zTypeTargets = { 8, 2, 2, 2, 1, 1 };
    public static T_Team[] s_tPieces = { new T_Team(false), new T_Team(false)};
    int m_x;
    int m_y;
    public Vector2 getPos() { return new Vector2(m_x, m_y); }
    private E_Team m_team;
    public E_Team getTeam() { return m_team; }
    private E_PieceType m_type;
    public E_PieceType getType() { return m_type; }
    Square currentSquare = null;
    static Sprite[] zSprites;

    public static void onStart(Sprite[] _sprites)
    {
        zSprites = _sprites;
        zTypeTargets[0] = 8;
        zTypeTargets[1] = 2;
        zTypeTargets[2] = 2;
        zTypeTargets[3] = 2;
        zTypeTargets[4] = 1;
        zTypeTargets[5] = 1;
        s_tPieces[0] = new T_Team(false);
        s_tPieces[1] = new T_Team(false);
    }

    public void init(int x, int y, E_PieceType type, E_Team team)
    {
        m_x = x;
        m_y = y;
        m_team = team;
        m_type = type;

        int i = (int)team;
        int j = (int)type;
        ++(s_tPieces[i].nPieces);
        ++(s_tPieces[i].aTypes[j]);
        s_tPieces[i].vPieces.Add(this);
        reloadSprite();
        currentSquare = Board._i.getSquare(x, y);
    }
    ~Piece()
    {
    }

    void reloadSprite()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = zSprites[(int)m_team * 6 + (int)m_type];
    }

    public E_PieceType move(int x, int y, out E_PieceType demotedFrom)
    {
        // demote check
        int endzone = m_team == E_Team.Black ? 0 : 7;
        demotedFrom = E_PieceType.None;
        if (y == endzone && m_type != E_PieceType.Pawn && m_type != E_PieceType.King && validatePawn(m_team)
            /*&& s_tPieces[(int)m_team].aTypes[(int)m_type] > zTypeTargets[(int)m_type])*/)
        {
            demotedFrom = m_type;
            demote();
        }
        
        E_SpawnCmd pawn_flag = E_SpawnCmd.chance;
        if (m_type == E_PieceType.Pawn)
        {
            if (x != m_x)
                pawn_flag = E_SpawnCmd.must;
            else
                pawn_flag = E_SpawnCmd.cannot;
        }
        // switch tile
        currentSquare.deoccupy();
        E_PieceType spawn = currentSquare.rollSpawn(m_team, pawn_flag);
        currentSquare = Board._i.getSquare(x, y);
        currentSquare.occupy(this);

        // move gameobject
        Vector3 pos = transform.position;
        pos.x = m_x = x;
        pos.y = m_y = y;
        transform.position = pos;
        return spawn;
    }

    void demote()
    {
        --s_tPieces[(int)m_team].nPawnCandidates;
        --s_tPieces[(int)m_team].aTypes[(int)m_type];
        if (m_type == E_PieceType.Bish)
        {
            if (Board._i.isWhiteSquare(m_x, m_y))
            {
                --s_tPieces[(int)m_team].nWhiteBishop;
            }
            else
            {
                --s_tPieces[(int)m_team].nBlackBishop;
            }
        }

        m_type = E_PieceType.Pawn;
        ++s_tPieces[(int)m_team].aTypes[(int)m_type];

        reloadSprite();
    }
    public void promote(E_PieceType to)
    {
        --s_tPieces[(int)m_team].aTypes[(int)m_type];
        m_type = to;
        ++s_tPieces[(int)m_team].nPawnCandidates;
        ++s_tPieces[(int)m_team].aTypes[(int)m_type];
        if(m_type==E_PieceType.Bish)
        {
            if (Board._i.isWhiteSquare(m_x, m_y))
            {
                ++s_tPieces[(int)m_team].nWhiteBishop;
            }
            else
            {
                ++s_tPieces[(int)m_team].nBlackBishop;
            }
        }

        reloadSprite();
    }

    public List<Vector2> getMoveset(int srcX = -1, int srcY = -1)
    {
        if (srcX == -1)
            srcX = m_x;
        if (srcY == -1)
            srcY = m_y;
        switch (m_type)
        {
            case E_PieceType.Pawn:  return getPawnMoveset(srcX, srcY);
            case E_PieceType.Rook:  return getRookMoveset(srcX, srcY);
            case E_PieceType.Hors: return getHorseMoveset(srcX, srcY);
            case E_PieceType.Bish:return getBishopMoveset(srcX, srcY);
            case E_PieceType.Quee: return getQueenMoveset(srcX, srcY);
            case E_PieceType.King:  return getKingMoveset(srcX, srcY);
            default:                return new List<Vector2>();
        }
    }
    List<Vector2> getPawnMoveset(int srcX, int srcY)
    {
        List<Vector2> moves = new List<Vector2>();

        // regular move
        bool isBlack = m_team == E_Team.Black;
        int newY = srcY + (isBlack ? 1 : -1);
        if (newY > 6 || newY < 1) // can't move onto row 0 or row 7
            return moves;
        moves.Add(new Vector2(m_x, newY));

        // diag
        if (canPawnDiag(srcX, srcY))
        {
            moves.Add(new Vector2(srcX + 1, newY));
            moves.Add(new Vector2(srcX - 1, newY));
        }

        // doublestep
        if(Board._i.getSquare(m_x, newY).getOccupant() == null)
        {
            if (isBlack && srcY == 4)
                moves.Add(new Vector2(m_x, srcY + 2));
            if (!isBlack && srcY == 3)
                moves.Add(new Vector2(m_x, srcY - 2));
        }

        return moves;
    }
    public bool canPawnDiag(int srcX, int srcY)
    {
        int enemy = (int)Player.otherPlayer(m_team);
        // can spawn something
        bool can = s_tPieces[enemy].nPieces < 16;
        if (m_team == E_Team.Black && srcY == 0)
        {
            // if on the backline, can spawn something other than a pawn
            can |= (s_tPieces[enemy].nPieces - s_tPieces[enemy].aTypes[(int)E_PieceType.Pawn]) < 8;
        }
        else if (m_team == E_Team.White && srcY == 7)
        {
            can |= (s_tPieces[enemy].nPieces - s_tPieces[enemy].aTypes[(int)E_PieceType.Pawn]) < 8;
        }
        return can;
    }
    List<Vector2> getRookMoveset(int srcX, int srcY)
    {
        List<Vector2> moves = new List<Vector2>();
        bool[] blocked = { false, false, false, false};
        for (int d = 1; d <= 7; ++d)
        {
            AddIfNotBlocked(ref moves, ref blocked[0], srcX + d, srcY);
            AddIfNotBlocked(ref moves, ref blocked[1], srcX - d, srcY);
            AddIfNotBlocked(ref moves, ref blocked[2], srcX,     srcY + d);
            AddIfNotBlocked(ref moves, ref blocked[3], srcX,     srcY - d);
        }
        return moves;
    }
    List<Vector2> getHorseMoveset(int srcX, int srcY)
    {
        List<Vector2> moves = new List<Vector2>();
        moves.Add(new Vector2(srcX + 2, srcY + 1));
        moves.Add(new Vector2(srcX + 2, srcY - 1));
        moves.Add(new Vector2(srcX - 2, srcY + 1));
        moves.Add(new Vector2(srcX - 2, srcY - 1));

        moves.Add(new Vector2(srcX + 1, srcY + 2));
        moves.Add(new Vector2(srcX + 1, srcY - 2));
        moves.Add(new Vector2(srcX - 1, srcY + 2));
        moves.Add(new Vector2(srcX - 1, srcY - 2));

        return moves;
    }
    List<Vector2> getBishopMoveset(int srcX, int srcY)
    {
        List<Vector2> moves = new List<Vector2>();
        bool[] blocked = { false, false, false, false };
        for (int y = 1; y <= 7; ++y)
        {
            AddIfNotBlocked(ref moves, ref blocked[0], srcX - y, srcY + y);
            AddIfNotBlocked(ref moves, ref blocked[1], srcX + y, srcY + y);
            AddIfNotBlocked(ref moves, ref blocked[2], srcX - y, srcY - y);
            AddIfNotBlocked(ref moves, ref blocked[3], srcX + y, srcY - y);
        }
        return moves;
    }
    List<Vector2> getQueenMoveset(int srcX, int srcY)
    {
        List<Vector2> moves = getBishopMoveset(srcX, srcY);
        moves.AddRange(getRookMoveset(srcX, srcY));
        return moves;
    }
    List<Vector2> getKingMoveset(int srcX, int srcY)
    {
        List<Vector2> moves = new List<Vector2>();
        for (int y = -1; y <= 1; ++y)
            for (int x = -1; x <= 1; ++x)
                moves.Add(new Vector2(srcX+x, srcY+y));
        return moves;
    }
    void AddIfNotBlocked(ref List<Vector2> moves, ref bool blocked, int x, int y)
    {
        if (blocked)
            return;
        Square s = Board._i.getSquare(x, y);
        if (s != null && s.getOccupant() != null)
        {
            blocked = true;
            return;
        }
        moves.Add(new Vector2(x, y));
    }


    // piece counting
    public static bool validateBishop(E_Team team, int x, int y)
    {
        int nColouredBishop;
        nColouredBishop = Board._i.isWhiteSquare(x, y) ?
            s_tPieces[(int)team].nWhiteBishop :
            s_tPieces[(int)team].nBlackBishop;
        return
            s_tPieces[(int)team].nPawnCandidates +                // pawns to be
            s_tPieces[(int)team].aTypes[(int)E_PieceType.Pawn] +  // existing pawns
            nColouredBishop                                       // the existing bishops of this colour
            <
            zTypeTargets[(int)E_PieceType.Pawn] +     // 8 pawns
            zTypeTargets[(int)E_PieceType.Bish] / 2;  // 1 of this coloured bishol
    }

    // Returns true if the sum of pawns or excess of other types is less than 8
    public static bool validatePawn(E_Team team)
    {
        return
            s_tPieces[(int)team].nPawnCandidates +
            s_tPieces[(int)team].aTypes[(int)E_PieceType.Pawn]
            <
            zTypeTargets[(int)E_PieceType.Pawn];    // 8 pawns
    }

    public static bool validateOther(E_PieceType type, E_Team team)
    {
        return
            s_tPieces[(int)team].nPawnCandidates +
            s_tPieces[(int)team].aTypes[(int)E_PieceType.Pawn] +
            s_tPieces[(int)team].aTypes[(int)type]
            <
            zTypeTargets[(int)E_PieceType.Pawn] + // 8 pawns + 2 of this type
            zTypeTargets[(int)type];
    }

    public void setPos(int x, int y)
    {
        currentSquare.deoccupy();
        currentSquare = Board._i.getSquare(x, y);
        currentSquare.occupy(this);

        // move gameobject
        Vector3 pos = transform.position;
        pos.x = m_x = x;
        pos.y = m_y = y;
        transform.position = pos;
    }

    //todo
    // change s_tPieces bool bWhiteBishop into int nWhiteBishop, etc
}
