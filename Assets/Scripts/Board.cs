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
        public E_PieceType spawned;
        public Move(int oldX, int oldY, int newX, int newY, E_PieceType _spawned)
        {
            srcX = oldX;
            srcY = oldY;
            destX = newX;
            destY = newY;
            spawned = _spawned;
        }
    }
    public static Board _i;
    [SerializeField] GameObject piecePrefab;
    [SerializeField] GameObject moveIndicatorPrefab;
    [SerializeField] Sprite[] pieceSprites;
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
        Piece.mountTextures(pieceSprites);
        Square.assignHighlightPrefab(moveIndicatorPrefab);
        slots = new List<Square>();
        history = new List<Move>();
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
        GameObject obj = Instantiate(piecePrefab, new Vector3(x, y, 0), Quaternion.identity);
        Piece piece = obj.GetComponent<Piece>();
        piece.init(x, y, type, team);

        Square square = getSquare(x, y);
        square.occupy(piece);
    }

    public void movePiece(Piece piece, int x, int y)
    {
        int oldX = (int)piece.getPos().x;
        int oldY = (int)piece.getPos().y;

        // TODO: if the move matches next in the move list, then its just a redo

        E_PieceType spawn = piece.move(x, y);
        history.Add(new Move( oldX, oldY, x, y, spawn));
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
        foreach (Vector2 pos in moveset)
        {
            getSquare(pos).setMoveSelector(true);
        }
    }
    public void hideMoveset()
    {
        foreach (Square s in slots)
        {
            s.setMoveSelector(false);
        }
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

    public bool checkLoss()
    {
        // check if they've pawn blocked themselves
        bool pawnBlockFlag = false;
        // check bishop entry
        if (!testSquareOccupant(2, 0, E_Team.White, E_PieceType.Bish))
        {
            if (testSquareOccupant(1, 1, E_Team.White, E_PieceType.Pawn) && testSquareOccupant(3, 1, E_Team.White, E_PieceType.Pawn))
                pawnBlockFlag = true;

        }
        // check rook entry
        for(int i = 8; i < 16; ++i)
        {
            if (testSquareOccupant(i, 1, E_Team.White, E_PieceType.Pawn))
            {
                pawnBlockFlag = true;
                break;
            }
        }

        return pawnBlockFlag;
    }

    // returns true if there is a piece at the coords that matches the conditions
    public bool testSquareOccupant(int x, int y, E_Team team, E_PieceType type)
    {
        Square s = getSquare(x, y);
        if (s == null)
            return false;
        Piece p = s.getOccupant();
        if (!p)
            return false;
        return p.getTeam() == team && p.getType() == type;
    }

    public void buildStartState()
    {
        createPiece(7, 7, E_PieceType.King, E_Team.Black);
        createPiece(5, 5, E_PieceType.King, E_Team.White);
        createPiece(1, 5, E_PieceType.Pawn, E_Team.Black);
        createPiece(2, 4, E_PieceType.Pawn, E_Team.Black);
        createPiece(7, 1, E_PieceType.Rook, E_Team.White);
        createPiece(5, 2, E_PieceType.Pawn, E_Team.Black);
        createPiece(3, 4, E_PieceType.Bish, E_Team.White);
        createPiece(0, 4, E_PieceType.Pawn, E_Team.White);
    }

    public void filterMoveset(ref List<Vector2> moveset, E_Team currentPlayer, Piece piece)
    {
        int i = 0;
        while(i < moveset.Count)
        {
            int newX = (int)moveset[i].x;
            int newY = (int)moveset[i].y;
            // can't move off the board
            if (newX < 0 || newY < 0 || newX >= 8 || newY >= 8)
            {
                moveset.RemoveAt(i);
                continue;
            }
            // can't move to occupied squares
            Piece occupant = getSquare(newX, newY).getOccupant();
            if (occupant != null)
            {
                moveset.RemoveAt(i);
                continue;
            }
            // can't put enemy king in check
            List<Vector2> destMoveset = piece.getMoveset(newX, newY);
            E_Team enemy = Player.otherPlayer(currentPlayer);
            bool checkFlag = false;
            foreach (Vector2 dest in destMoveset)
            {
                // if there is a black king in the would-be moveset
                if(testSquareOccupant((int)dest.x, (int)dest.y, enemy, E_PieceType.King))
                {
                    // check
                    moveset.RemoveAt(i);
                    checkFlag = true;
                    break;
                }
            }
            if (checkFlag)
                continue;
            ++i;
        }
    }

    public void redo()
    {

    }

    public void undo()
    {

    }
}
