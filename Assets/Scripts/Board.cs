using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    struct Move
    {
        public int srcX;
        public int srcY;
        public int destX;
        public int destY;
        public Move(int oldX, int oldY, int newX, int newY)
        {
            srcX = oldX;
            srcY = oldY;
            destX = newX;
            destY = newY;
        }
    }
    public static Board _i;
    [SerializeField] GameObject prefab;
    List<Square> slots;
    List<Move> history;

    private void Awake()
    {
        if (_i != null)
        {
            Destroy(_i.gameObject);
        }
        _i = this;
        init();
    }

    void init()
    {
        slots = new List<Square>();
        for (int y = 0; y < 8; ++y)
        {
            for (int x = 0; x < 8; ++x)
            {
                slots.Add(new Square(x, y));
            }
        }
        buildStartState();
    }

    public void createPiece(int x, int y, E_PieceType type, E_Team team)
    {
        GameObject obj = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity);
        Piece piece = obj.GetComponent<Piece>();
        piece.init(x, y, type, team);

        Square square = getSquare(x, y);
        square.occupy(piece);
    }

    public void movePiece(Piece piece, int x, int y)
    {
        int oldX = (int)piece.getPos().x;
        int oldY = (int)piece.getPos().y;

        E_SpawnCmd pawn_flag = E_SpawnCmd.chance;
        if (piece.getType() == E_PieceType.Pawn)
        {
            if (x != oldX)
                pawn_flag = E_SpawnCmd.must;
            else
                pawn_flag = E_SpawnCmd.cannot;
        }
        Board._i.getSquare(oldX, oldY).deoccupy(pawn_flag);
        history.Add(new Move( oldX, oldY, x, y));
    }

    #region GridInterface
    public Square getSquare(int x, int y)
    {
        if (x >= 8 || y >= 8 || x < 0 || y < 0)
            return null;
        return slots[x + y * 8];
    }
    public Square getSquare(Vector2 pos)
    {
        if (pos.x >= 8 || pos.y >= 8 || pos.x < 0 || pos.y < 0)
            return null;
        return slots[(int)pos.x + (int)pos.y * 8];
    }
    public int getIndex(int x, int y)
    {
        if (x >= 8 || y >= 8 || x < 0 || y < 0)
            return 0;
        return x + y * 8;
    }
    public int getIndex(Vector2 pos)
    {
        if (pos.x >= 8 || pos.y >= 8 || pos.x < 0 || pos.y < 0)
            return 0;
        return (int)pos.x + (int)pos.y * 8;
    }
    public Vector2 getCoord(int index)
    {
        Vector2 pos;
        pos.x = index % 8;
        pos.y = index / 8;
        return pos;
    }
    public bool isWhiteSquare(int x, int y)
    {
        return getIndex(x, y) % 2 == 1;
    }
    #endregion

    public void drawMoveset(List<Vector2> moveset)
    {
        // TODO
    }

    public bool checkWin()
    {
        for (int i = 0; i < 8 * 2; ++i)
        {
            Piece p = slots[i].getOccupant();
            if (!p)
                return false;
            if (p.getTeam() != E_Team.White)
                return false;
        }
        return true;
    }

    public void buildStartState()
    {
        createPiece(7, 7, E_PieceType.King, E_Team.Black);
        createPiece(5, 5, E_PieceType.King, E_Team.White);
        createPiece(1, 5, E_PieceType.Pawn, E_Team.Black);
        createPiece(2, 4, E_PieceType.Pawn, E_Team.Black);
        createPiece(7, 1, E_PieceType.Rook, E_Team.White);
        createPiece(7, 1, E_PieceType.Rook, E_Team.White);
        createPiece(3, 4, E_PieceType.Bish, E_Team.White);
    }
}
