using System;
using System.Text;

/// <summary>
/// Show the BALL button according to the coordinates X and Y.
/// </summary>
/// <param name="x">The coordinate X.</param>
/// <param name="y">The coordinate Y.</param>
/// <param name="ball">The ball image number.</param>
public delegate void ShowBox(int x, int y, int ball);

/// <summary>
/// Play a melody when the balls are located 
/// in a row, a column, a diagonal 
/// in the required MINBALLS quantity.
/// </summary>
public delegate void PlayCut();


/// <summary>
/// The Model class (MVC).
/// </summary>
public class Lines
{
    /// <summary>
    /// The size of the game board.
    /// </summary>
    public const int SIZE = 9;

    /// <summary>
    /// The total number of game balls.
    /// </summary>
    public const int BALLS = 7;

    /// <summary>
    /// The number of new game balls 
    /// appearing when moved the game ball.
    /// </summary>
    const int ADD_BALLS = 3;

    /// <summary>
    /// The minimum number of balls 
    /// that need to be collected 
    /// in a row, a column, a diagonal.
    /// </summary>
    const int MINBALLS = 4;


    /// <summary>
    /// For generating a random number.
    /// </summary>
    readonly Random random = new();

    /// <summary>
    /// The instance of the delegate 
    /// for showing the button on the game board.
    /// </summary>
    readonly ShowBox showBox;

    /// <summary>
    /// The instance of the delegate 
    /// for playing a melody.
    /// </summary>
    readonly PlayCut playCut;

    /// <summary>
    /// The array of the game ball number on the gameboard.
    /// </summary>
    readonly int[,] map;

    /// <summary>
    /// The coordinates of the clicked ball to move.
    /// </summary>
    int fromX, fromY;

    /// <summary>
    /// Is the game ball selected using a mouse click?
    /// </summary>
    bool isBallSelected;



    /// <summary>
    /// The class constructor.
    /// </summary>
    /// <param name="showBox">The instance of the delegate 
    /// for showing the button on the game board.</param>
    /// <param name="playCut">The instance of the delegate 
    /// for playing a melody.</param>
    public Lines(ShowBox showBox, PlayCut playCut)
    {
        this.showBox = showBox;
        this.playCut = playCut;

        map = new int[SIZE, SIZE];
    }


    /// <summary>
    /// Start the game.
    /// </summary>
    public void Start()
    {
        ClearMap();
        AddRandomBalls();
        isBallSelected = false;
    }


    /// <summary>
    /// Adding some balls randomly on the gameboard.
    /// </summary>
    private void AddRandomBalls()
    {
        for (int i = 0; i < ADD_BALLS; i++)
        {
            AddRandomBall();
        }
    }


    /// <summary>
    /// Adding the one ball on the gameboard.
    /// </summary>
    private void AddRandomBall()
    {
        int x, y;
        int loop = SIZE * SIZE;

        do
        {
            x = random.Next(SIZE);
            y = random.Next(SIZE);

            if (--loop <= 0)
            {
                return;
            }

        } while (map[x, y] > 0);

        int ball = 1 + random.Next(BALLS - 1);

        SetMap(x, y, ball);
    }


    /// <summary>
    /// Is the game over?
    /// </summary>
    /// <returns>Yes | No.</returns>
    private bool IsGameOver()
    {
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                if (map[x, y] == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Create the map of balls to send it to the server.
    /// </summary>
    /// <returns>The map as the string line.</returns>
    public string GetMapForSendServer()
    {
        StringBuilder stringBuilder = new();

        for (int y = 0; y < SIZE; y++)
        {
            for (int x = 0; x < SIZE; x++)
            {
                stringBuilder.Append(map[x, y].ToString());
            }
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Clean the game board.
    /// </summary>
    private void ClearMap()
    {
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                SetMap(x, y, 0);
            }
        }
    }


    /// <summary>
    /// Is the game cell on the board?
    /// </summary>
    /// <param name="x">The coordinate X.</param>
    /// <param name="y">The coordinate Y.</param>
    /// <returns>Yes | No.</returns>
    private bool OnMap(int x, int y) 
        => x >= 0 && x < SIZE && y >= 0 && y < SIZE;



    /// <summary>
    /// Get the game ball number.
    /// </summary>
    /// <param name="x">The coordinate X.</param>
    /// <param name="y">The coordinate Y.</param>
    /// <returns>The game ball number.</returns>
    private int Getmap(int x, int y)
    {
        if (!OnMap(x, y))
        {
            return 0;
        }

        return map[x, y];
    }


    /// <summary>
    /// Set the game ball number.
    /// </summary>
    /// <param name="x">The coordinate X.</param>
    /// <param name="y">The coordinate X.</param>
    /// <param name="ball">The game ball number.</param>
    private void SetMap(int x, int y, int ball)
    {
        map[x, y] = ball;
        showBox(x, y, ball);
    }


    /// <summary>
    /// Click by the cell on the game board.
    /// </summary>
    /// <param name="x">The coordinate X.</param>
    /// <param name="y">The coordinate Y.</param>
    public void Click(int x, int y)
    {
        if (IsGameOver())
        {
            Start();
        }
        else
        {
            if (map[x, y] > 0)
            {
                TakeBall(x, y);
            }
            else
            {
                MoveBall(x, y);
            }
        }
    }


    /// <summary>
    /// The clicked ball to move.
    /// </summary>
    /// <param name="x">The coordinate X.</param>
    /// <param name="y">The coordinate Y.</param>
    private void TakeBall(int x, int y)
    {
        fromX = x;
        fromY = y;
        isBallSelected = true;
    }


    /// <summary>
    /// Moving the ball to the new cell on the board.
    /// </summary>
    /// <param name="x">The coordinate X to move.</param>
    /// <param name="y">The coordinate Y to move.</param>
    private void MoveBall(int x, int y)
    {
        if (!isBallSelected)
        {
            return;
        }

        if (!CanMove(x, y))
        {
            return;
        }

        SetMap(x, y, map[fromX, fromY]);
        SetMap(fromX, fromY, 0);

        isBallSelected = false;

        if (!CutLines())
        {
            AddRandomBalls();
            CutLines();
        }
    }


    /// <summary>
    /// The array of the marked cell for cutting the lines.
    /// </summary>
    private bool[,] mark;


    /// <summary>
    /// Cutting the lines.
    /// </summary>
    /// <returns>Cutted lines | No.</returns>
    private bool CutLines()
    {
        int balls = 0;
        mark = new bool[SIZE, SIZE];

        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                balls += CutLine(x, y,  1, 0);
                balls += CutLine(x, y,  0, 1);
                balls += CutLine(x, y,  1, 1);
                balls += CutLine(x, y, -1, 1);
            }
        }

        if (balls > 0)
        {
            playCut();

            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    if (mark[x, y])
                    {
                        SetMap(x, y, 0);
                    }
                }
            }

            return true;
        }


        return false;
    }


    /// <summary>
    /// Searching the lines to cut.
    /// </summary>
    /// <param name="x0">The coordinate X to search the lime.</param>
    /// <param name="y0">The coordinate Y to search the line.</param>
    /// <param name="vx">Direction of the search by coordinate X.</param>
    /// <param name="vy">Direction of the search by coordinate Y.</param>
    /// <returns>The number of balls 
    /// in a row or a column or a diagonal.</returns>
    private int CutLine(int x0, int y0, int vx, int vy)
    {
        int ball = map[x0, y0];

        if (ball == 0) return 0;

        int count = 0;

        for (int x = x0, y = y0; Getmap(x, y) == ball; x += vx, y += vy)
        {
            count++;
        }

        if (count < MINBALLS)
        {
            return 0;
        }

        for (int x = x0, y = y0; Getmap(x, y) == ball; x += vx, y += vy)
        {
            mark[x, y] = true;
        }

        return count;
    }

    /// <summary>
    /// The array to search for the path of moving the ball.
    /// </summary>
    private bool[,] used;


    /// <summary>
    /// Searching for the path of moving the ball.
    /// </summary>
    /// <param name="toX">The coordinate X to move.</param>
    /// <param name="toY">The coordinate Y to move.</param>
    /// <returns>Yes | No.</returns>
    private bool CanMove(int toX, int toY)
    {
        used = new bool[SIZE, SIZE];

        Walk(fromX, fromY, true);

        return used[toX, toY];
    }


    /// <summary>
    /// The recursive search for the ball path.
    /// </summary>
    /// <param name="x">The coordinate X.</param>
    /// <param name="y">The coordinate Y.</param>
    /// <param name="start">To check the start position of the search.</param>
    private void Walk(int x, int y, bool start = false)
    {
        if (!start)
        {
            if (!OnMap(x, y)) return;

            if (map[x, y] > 0) return;

            if (used[x, y]) return;
        }

        used[x, y] = true;

        Walk(x + 1, y);
        Walk(x - 1, y);
        Walk(x, y + 1);
        Walk(x, y - 1);
    }
}
