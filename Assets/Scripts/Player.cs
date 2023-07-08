using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    static Player _i = null;
    int turns = 0;
    Piece selected = null;
    GameObject selection_highlight = null;
    [SerializeField] GameObject selection_highlight_prefab = null;
    List<Vector2> currentMoveset;
    E_Team currentPlayer;


    public static E_Team otherPlayer(E_Team player)
    {
        return (E_Team)((int)player + 1 % 2);
    }

    private void Awake()
    {
        if (_i != null)
            Destroy(_i.gameObject);
        _i = this;
    }

    private void Start()
    {
        currentMoveset = new List<Vector2>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentPlayer == E_Team.White)
        {
            Vector3 mouse_pos = Input.mousePosition;
            mouse_pos.z = 1;
            Vector2 pos = (Vector2)Camera.main.ScreenToWorldPoint(mouse_pos);
            int x = (int)pos.x;
            int y = (int)pos.y;
            Piece piece = Board._i.getSquare(x, y).getOccupant();
            // selecting one of our pieces
            if (piece != null && piece.getTeam() == currentPlayer)
            {
                selectPiece(piece);
                return;
            }
            if (selected != null)
            {
                // if the target tile is within the moveset
                if (currentMoveset.Exists(move => move.x == x && move.y == y))
                    movePiece(x, y);
            }
        }
    }

    void selectPiece(Piece piece)
    {
        selected = piece;
        if (selected == null)
        {
            currentMoveset = new List<Vector2>();
            selection_highlight.SetActive(false);
        }
        else
        {
            // get the moveset
            currentMoveset = selected.getMoveset();
            Board._i.filterMoveset(ref currentMoveset, currentPlayer, selected);

            // move the selection cursor
            selection_highlight.SetActive(true);
            Vector3 pos = selection_highlight.transform.position;
            pos.x = selected.getPos().x;
            pos.y = selected.getPos().y;
            selection_highlight.transform.position = pos;
        }
        Board._i.drawMoveset(currentMoveset);
    }

    void movePiece(int x, int y)
    {
        Board._i.movePiece(selected, x, y);
        if(Board._i.checkWin())
        {
            // Winscreen
            AudioInterface.play(E_Sound.Win);
        }
        else if(Board._i.checkLoss())
        {
            AudioInterface.play(E_Sound.Loss);

        }
        endTurn();
        // AI turn
        StartCoroutine(aiTurn());
    }
    IEnumerator aiTurn()
    {
        yield return new WaitForSeconds(.1f);
        List<Piece> pieces = Piece.s_tPieces[(int)E_Team.Black].vPieces;
        int index = Random.Range(0, pieces.Count - 1);
        selectPiece(pieces[index]);
        yield return new WaitForSeconds(.1f);
        index = Random.Range(0, currentMoveset.Count-1);
        Board._i.movePiece(selected, (int)currentMoveset[index].x, (int)currentMoveset[index].y);
        endTurn();
    }
    void endTurn()
    {
        selectPiece(null);
        currentPlayer = otherPlayer(currentPlayer);
        selection_highlight.SetActive(false);
    }
}
