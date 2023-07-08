using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    static Player _i = null;
    int turns = 0;
    Piece selected = null;
    [SerializeField] GameObject selection_highlight;
    List<Vector2> currentMoveset;
    E_Team currentPlayer = E_Team.White;


    public static E_Team otherPlayer(E_Team player)
    {
        return (E_Team)(((int)player + 1) % 2);
    }
    public static E_Team otherPlayer()
    {
        return (E_Team)(((int)_i.currentPlayer + 1) % 2);
    }

    private void Awake()
    {
        if (_i != null)
            Destroy(_i.gameObject);
        _i = this;
    }

    private void Start()
    {
        selectPiece(null);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Audio test");
            AudioInterface.play(E_Sound.Win);
        }

        if (Input.GetMouseButtonDown(1) && currentPlayer == E_Team.White)
        {
            selectPiece(null);
        }
        if (Input.GetKeyDown(KeyCode.Escape) && currentPlayer == E_Team.White)
        {
            selectPiece(null);
        }
        if (Input.GetMouseButtonDown(0) && currentPlayer == E_Team.White)
        {
            Vector3 mouse_pos = Input.mousePosition;
            mouse_pos.z = 1;
            Vector2 pos = (Vector2)Camera.main.ScreenToWorldPoint(mouse_pos);
            int x = (int)(pos.x + 0.5f);
            int y = (int)(pos.y + 0.5f);
            Debug.Log("Clicked " + x + ", " + y);
            onClick(x, y);
        }
    }

    void onClick(int x, int y)
    {
        if (x > 7 || x < 0 || y > 7 || y < 0)
            return;
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

    void selectPiece(Piece piece)
    {
        selected = piece;
        Board._i.hideMoveset();
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
            Board._i.drawMoveset(currentMoveset);
        }
    }

    void movePiece(int x, int y)
    {
        Board._i.movePiece(selected, x, y);
        endTurn();
        if (Board._i.checkWin())
        {
            onWin();
        }
        else 
        {
            List<Vector2> indicators = new List<Vector2>();
            E_FailState state = Board._i.checkFail(ref indicators);
            if (state != E_FailState.None)
            {
                onLoss(state, indicators);
            }
            // AI turn
            StartCoroutine(aiTurn());
        }
    }

    IEnumerator aiTurn()
    {
        // TODO: disable undo button

        yield return new WaitForSeconds(.25f);
        List<Piece> pieces = Piece.s_tPieces[(int)E_Team.Black].vPieces;
        int index = Random.Range(0, pieces.Count - 1);
        selectPiece(pieces[index]);
        int iterations = pieces.Count;
        while (currentMoveset.Count == 0)
        {
            if (--iterations == 0)
            {
                Debug.Log("AI cannot move");
                endTurn();
                yield break;
            }
            index = ((index + 1) % pieces.Count);

            Debug.Log("selecting index " + index);
            selectPiece(pieces[index]);
            yield return new WaitForSeconds(.25f);
        }
        yield return new WaitForSeconds(.25f);
        index = Random.Range(0, currentMoveset.Count-1);
        Board._i.movePiece(selected, (int)currentMoveset[index].x, (int)currentMoveset[index].y);

        // TODO: renable undo button

        endTurn();
        
    }
    void endTurn()
    {
        selectPiece(null);
        currentPlayer = otherPlayer(currentPlayer);
        selection_highlight.SetActive(false);
    }

    public void onWin()
    {
        // Winscreen
        AudioInterface.play(E_Sound.Win);
    }

    [SerializeField] WarningHover failNotif;
    public void onLoss(E_FailState failtype, List<Vector2> failIndicators)
    {
        string failmessage = "";
        switch(failtype)
        {
            case E_FailState.BishopLockout:
                failmessage =
                    "Your pawns have blocked your bishop";
                break;
            case E_FailState.BishopLockin:
                failmessage =
                    "Their pawns have blocked your bishop";
                break;
            case E_FailState.PawnTrapped:
                failmessage =
                    "Their pawns have blocked your pawn";
                break;
            case E_FailState.PawnTrappedNoDiag:
                failmessage =
                    "Their pawn has blocked your pawn." +
                    "\nYour pawn cannot move diagonally as there" +
                    "\nare no more pieces left to be uncaptured";
                break;
            case E_FailState.PawnWall:
                failmessage = "Your pawns have blocked the other pieces";
                break;
            case E_FailState.NoValidMoves:
                failmessage =
                    "The enemy has no valid moves, this is a" +
                    "\nstalemate";
                break;
        }
        failNotif.SetWarning(true, failmessage);
        AudioInterface.play(E_Sound.Loss);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
