using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    //References to objects in our Unity Scene
    public GameObject controller;
    public GameObject movePlate;

    //Position for this Chesspiece on the Board
    //The correct position will be set later
    private int xBoard = -1;
    private int yBoard = -1;

    //Variable for keeping track of the player it belongs to "black" or "white"
    private string player = "white";

    //References to all the possible Sprites that this Chesspiece could be
    public Sprite Dragon_Black, Emperor_Black, Empress_Black, Knight_Black, Paladin_Black, Priest_Black;
    public Sprite Dragon_White, Emperor_White, Empress_White, Knight_White, Paladin_White, Priest_White;

    void Update()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Vector3 white = new Vector3(0, 0, 0);
        Vector3 black = new Vector3(0, 0, 180);
        if (controller.GetComponent<Game>().GetCurrentPlayer() == "white")
            transform.eulerAngles = white;
        else
            transform.eulerAngles = black;
    }

    public void Activate()
    {
        //Get the game controller
        controller = GameObject.FindGameObjectWithTag("GameController");

        //Take the instantiated location and adjust transform
        SetCoords();

        //Choose correct sprite based on piece's name
        switch (this.name)
        {
            case "Dragon_Black": this.GetComponent<SpriteRenderer>().sprite = Dragon_Black; player = "black"; break;
            case "Emperor_Black": this.GetComponent<SpriteRenderer>().sprite = Emperor_Black; player = "black"; break;
            case "Empress_Black": this.GetComponent<SpriteRenderer>().sprite = Empress_Black; player = "black"; break;
            case "Knight_Black": this.GetComponent<SpriteRenderer>().sprite = Knight_Black; player = "black"; break;
            case "Paladin_Black": this.GetComponent<SpriteRenderer>().sprite = Paladin_Black; player = "black"; break;
            case "Priest_Black": this.GetComponent<SpriteRenderer>().sprite = Priest_Black; player = "black"; break;

            case "Dragon_White": this.GetComponent<SpriteRenderer>().sprite = Dragon_White; player = "white"; break;
            case "Emperor_White": this.GetComponent<SpriteRenderer>().sprite = Emperor_White; player = "white"; break;
            case "Empress_White": this.GetComponent<SpriteRenderer>().sprite = Empress_White; player = "white"; break;
            case "Knight_White": this.GetComponent<SpriteRenderer>().sprite = Knight_White; player = "white"; break;
            case "Paladin_White": this.GetComponent<SpriteRenderer>().sprite = Paladin_White; player = "white"; break;
            case "Priest_White": this.GetComponent<SpriteRenderer>().sprite = Priest_White; player = "white"; break;
        }
    }

    public void SetCoords()
    {
        //Get the board value in order to convert to xy coords
        float x = xBoard;
        float y = yBoard;

        //Adjust by variable offset
        x *= 1.2f;
        y *= 1.2f;

        //Add constants (pos 0,0)
        x += -4.2f;
        y += -4.2f;

        //Set actual unity values
        this.transform.position = new Vector3(x, y, -1.0f);
    }

    public int GetXBoard()
    {
        return xBoard;
    }

    public int GetYBoard()
    {
        return yBoard;
    }

    public void SetXBoard(int x)
    {
        xBoard = x;
    }

    public void SetYBoard(int y)
    {
        yBoard = y;
    }

    private void OnMouseUp()
    {
        if (!controller.GetComponent<Game>().IsGameOver() && controller.GetComponent<Game>().GetCurrentPlayer() == player)
        {
            //Remove all moveplates relating to previously selected piece
            DestroyMovePlates();

            //Create new MovePlates
            InitiateMovePlates();
        }
    }

    public void DestroyMovePlates()
    {
        //Destroy old MovePlates
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]); //Be careful with this function "Destroy" it is asynchronous
        }
    }

    public void InitiateMovePlates()
    {
        Game sc = controller.GetComponent<Game>();
        switch (this.name)
        {
            case "Empress_Black":
            case "Empress_White":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(1, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                LineMovePlate(-1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(1, -1);
                break;
            case "Paladin_Black":
            case "Paladin_White":
                PaladinMovePlate();
                break;
            case "Priest_Black":
            case "Priest_White":
                PriestMovePlate();
                LMovePlate();
                break;
            case "Emperor_Black":
            case "Emperor_White":
                SurroundMovePlate();
                break;
            case "Dragon_Black":
            case "Dragon_White":
                DragonMovePlate(0, 1);
                DragonMovePlate(0, -1);
                break;
            case "Knight_Black":
                if (yBoard == 6)
                {
                    PawnMovePlate(xBoard, yBoard - 1);
                    if(sc.GetPosition(xBoard, yBoard - 2) == null)
                        PointMovePlate(xBoard, yBoard - 2);
                }
                else
                {
                    PawnMovePlate(xBoard, yBoard - 1);
                }
                break;
            case "Knight_White":
                if (yBoard == 1)
                {
                    PawnMovePlate(xBoard, yBoard + 1);
                    if (sc.GetPosition(xBoard, yBoard + 2) == null)
                        PointMovePlate(xBoard, yBoard + 2);
                }
                else
                {
                    PawnMovePlate(xBoard, yBoard + 1);
                }
                break;
        }
    }

    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();

        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
        }

        if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<Chessman>().player != player)
        {
            MovePlateAttackSpawn(x, y);
        }
    }
    public void DragonMovePlate(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();

        int x = xBoard;
        int y = yBoard;
        sc.GetPosition(x, y);
        int right = 7 - x;
        int left = x;
        int up = 7 - y;
        int down = y;

        for (int i = 1; i <= right; i++)
        {
            if (sc.GetPosition(xBoard + i - 1, yBoard) == null || sc.GetPosition(xBoard + i - 1, yBoard).GetComponent<Chessman>().player == player)
                PointMovePlate(xBoard + i, yBoard);
        }
        for (int i = 1; i <= up; i++)
        {
            if (sc.GetPosition(xBoard, yBoard + i - 1) == null || sc.GetPosition(xBoard, yBoard + i - 1).GetComponent<Chessman>().player == player)
                PointMovePlate(xBoard, yBoard + i);
        }
        for (int i = 1; i <= left; i++)
        {
            if (sc.GetPosition(xBoard - i + 1, yBoard) == null || sc.GetPosition(xBoard - i + 1, yBoard).GetComponent<Chessman>().player == player)
                PointMovePlate(xBoard - i, yBoard);
        }
        for (int i = 1; i <= down; i++)
        {
            if (sc.GetPosition(xBoard, yBoard - i + 1) == null || sc.GetPosition(xBoard, yBoard - i + 1).GetComponent<Chessman>().player == player)
                PointMovePlate(xBoard, yBoard - i);
        }
    }

    public void PriestMovePlate()
    {
        Game sc = controller.GetComponent<Game>();

        int x = xBoard;
        int y = yBoard;
        sc.GetPosition(x, y);
        int right = 7 - x;
        int left = x;
        int up = 7 - y;
        int down = y;

        if (up < right)
            ru(up);
        else
            ru(right);

        if (up < left)
            lu(up);
        else
            lu(left);

        if (down < right)
            rd(down);
        else
            rd(right);

        if (down < left)
            ld(down);
        else
            ld(left);
    }
    public void ru(int max)
    {
        Game sc = controller.GetComponent<Game>();
        for (int i = 1; i <= max; i++)
        {
            if (sc.GetPosition(xBoard + i - 1, yBoard + i - 1) == null || sc.GetPosition(xBoard + i - 1, yBoard + i - 1).GetComponent<Chessman>().player == player)
                PointMovePlate(xBoard + i, yBoard + i);
        }
    }
    public void lu(int max)
    {
        Game sc = controller.GetComponent<Game>();
        for (int i = 1; i <= max; i++)
        {
            if (sc.GetPosition(xBoard - i + 1, yBoard + i - 1) == null || sc.GetPosition(xBoard - i + 1, yBoard + i - 1).GetComponent<Chessman>().player == player)
                PointMovePlate(xBoard - i, yBoard + i);
        }
    }
    public void rd(int max)
    {
        Game sc = controller.GetComponent<Game>();
        for (int i = 1; i <= max; i++)
        {
            if (sc.GetPosition(xBoard + i - 1, yBoard - i + 1) == null || sc.GetPosition(xBoard + i - 1, yBoard - i + 1).GetComponent<Chessman>().player == player)
                PointMovePlate(xBoard + i, yBoard - i);
        }
    }
    public void ld(int max)
    {
        Game sc = controller.GetComponent<Game>();
        for (int i = 1; i <= max; i++)
        {
            if (sc.GetPosition(xBoard - i + 1, yBoard - i + 1) == null || sc.GetPosition(xBoard - i + 1, yBoard - i + 1).GetComponent<Chessman>().player == player)
                PointMovePlate(xBoard - i, yBoard - i);
        }
    }

    public void LMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }

    public void SurroundMovePlate()
    {
        Game sc = controller.GetComponent<Game>();

        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 0);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard + 0);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard + 1);
        if (sc.PositionOnBoard(xBoard, yBoard + 2) && sc.GetPosition(xBoard, yBoard + 2) != null && sc.PositionOnBoard(xBoard, yBoard + 1) && sc.GetPosition(xBoard, yBoard + 1) == null)
            PointMovePlate(xBoard, yBoard + 2);
        if (sc.PositionOnBoard(xBoard, yBoard - 2) && sc.GetPosition(xBoard, yBoard - 2) != null && sc.PositionOnBoard(xBoard, yBoard - 1) && sc.GetPosition(xBoard, yBoard - 1) == null)
            PointMovePlate(xBoard, yBoard - 2);
        if (sc.PositionOnBoard(xBoard + 2, yBoard) && sc.GetPosition(xBoard + 2, yBoard) != null && sc.PositionOnBoard(xBoard + 1, yBoard) && sc.GetPosition(xBoard + 1, yBoard) == null)
            PointMovePlate(xBoard + 2, yBoard);
        if (sc.PositionOnBoard(xBoard - 2, yBoard) && sc.GetPosition(xBoard - 2, yBoard) != null && sc.PositionOnBoard(xBoard - 1, yBoard) && sc.GetPosition(xBoard - 1, yBoard) == null)
            PointMovePlate(xBoard - 2, yBoard);

        if (sc.PositionOnBoard(xBoard + 2, yBoard + 1) && sc.GetPosition(xBoard + 2, yBoard + 1) != null && sc.PositionOnBoard(xBoard + 1, yBoard + 1) && sc.GetPosition(xBoard + 1, yBoard + 1) == null)
            PointMovePlate(xBoard + 2, yBoard + 1);
        if (sc.PositionOnBoard(xBoard + 1, yBoard + 2) && sc.GetPosition(xBoard + 1, yBoard + 2) != null && sc.PositionOnBoard(xBoard + 1, yBoard + 1) && sc.GetPosition(xBoard + 1, yBoard + 1) == null)
            PointMovePlate(xBoard + 1, yBoard + 2);

        if (sc.PositionOnBoard(xBoard - 2, yBoard + 1) && sc.GetPosition(xBoard - 2, yBoard + 1) != null && sc.PositionOnBoard(xBoard - 1, yBoard + 1) && sc.GetPosition(xBoard - 1, yBoard + 1) == null)
            PointMovePlate(xBoard - 2, yBoard + 1);
        if (sc.PositionOnBoard(xBoard - 1, yBoard + 2) && sc.GetPosition(xBoard - 1, yBoard + 2) != null && sc.PositionOnBoard(xBoard - 1, yBoard + 1) && sc.GetPosition(xBoard - 1, yBoard + 1) == null)
            PointMovePlate(xBoard - 1, yBoard + 2);

        if (sc.PositionOnBoard(xBoard - 2, yBoard - 1) && sc.GetPosition(xBoard - 2, yBoard - 1) != null && sc.PositionOnBoard(xBoard - 1, yBoard - 1) && sc.GetPosition(xBoard - 1, yBoard - 1) == null)
            PointMovePlate(xBoard - 2, yBoard - 1);
        if (sc.PositionOnBoard(xBoard - 1, yBoard - 2) && sc.GetPosition(xBoard - 1, yBoard - 2) != null && sc.PositionOnBoard(xBoard - 1, yBoard - 1) && sc.GetPosition(xBoard - 1, yBoard - 1) == null)
            PointMovePlate(xBoard - 1, yBoard - 2);

        if (sc.PositionOnBoard(xBoard + 2, yBoard - 1) && sc.GetPosition(xBoard + 2, yBoard - 1) != null && sc.PositionOnBoard(xBoard + 1, yBoard - 1) && sc.GetPosition(xBoard + 1, yBoard - 1) == null)
            PointMovePlate(xBoard + 2, yBoard - 1);
        if (sc.PositionOnBoard(xBoard + 1, yBoard - 2) && sc.GetPosition(xBoard + 1, yBoard - 2) != null && sc.PositionOnBoard(xBoard + 1, yBoard - 1) && sc.GetPosition(xBoard + 1, yBoard - 1) == null)
            PointMovePlate(xBoard + 1, yBoard - 2);
    }

    public void PaladinMovePlate()
    {
        Game sc = controller.GetComponent<Game>();

        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard);
        PointMovePlate(xBoard - 1, yBoard);
        if (sc.PositionOnBoard(xBoard, yBoard + 1) && sc.GetPosition(xBoard, yBoard + 1) == null)
        {
            PointMovePlate(xBoard, yBoard + 2);
        }    
        if (sc.PositionOnBoard(xBoard, yBoard - 1) && sc.GetPosition(xBoard, yBoard - 1) == null)
        {
            PointMovePlate(xBoard, yBoard - 2);
        } 
        if (sc.PositionOnBoard(xBoard + 1, yBoard) && sc.GetPosition(xBoard + 1, yBoard) == null)
        {
            PointMovePlate(xBoard + 2, yBoard);
        } 
        if (sc.PositionOnBoard(xBoard - 1, yBoard) && sc.GetPosition(xBoard - 1, yBoard) == null)
        {
            PointMovePlate(xBoard - 2, yBoard);
        }

        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }

    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            if (cp == null)
            {
                MovePlateSpawn(x, y);
            }
            else if (cp.GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x, y);
            }
        }
    }

    public void PawnMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            if (sc.GetPosition(x, y) == null)
            {
                MovePlateSpawn(x, y);
            }

            if (sc.PositionOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null && sc.GetPosition(x + 1, y).GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x + 1, y);
            }

            if (sc.PositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null && sc.GetPosition(x - 1, y).GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x - 1, y);
            }
        }
    }

    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        //Get the board value in order to convert to xy coords
        float x = matrixX;
        float y = matrixY;

        //Adjust by variable offset
        x *= 1.2f;
        y *= 1.2f;

        //Add constants (pos 0,0)
        x += -4.2f;
        y += -4.2f;

        //Set actual unity values
        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        //Get the board value in order to convert to xy coords
        float x = matrixX;
        float y = matrixY;

        //Adjust by variable offset
        x *= 1.2f;
        y *= 1.2f;

        //Add constants (pos 0,0)
        x += -4.2f;
        y += -4.2f;

        //Set actual unity values
        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }
}
