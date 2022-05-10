using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// The View-Controller class (MVC).
/// </summary>
public class Game : MonoBehaviour
{
    /// <summary>
    /// The url-address of the server where the map of balls sends.
    /// </summary>
    const string API_URL = "http://lines/save-map.php?map=";
    
    /// <summary>
    /// For playing a melody.
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// The array of the buttons on the game board.
    /// </summary>
    Button[,] buttons;

    /// <summary>
    /// The images of the game balls.
    /// </summary>
    Image[] images;

    /// <summary>
    /// The instance of the Model class (MVC).
    /// </summary>
    Lines lines;

    // Start is called before the first frame update
    void Start()
    {
        lines = new Lines(ShowBox, PlayCut);
        InitButtons();
        InitImages();

        lines.Start();

        SendMapToServer();
    }


    /// <summary>
    /// Show the BALL image according to the coordinates X and Y.
    /// </summary>
    /// <param name="x">The coordinate X.</param>
    /// <param name="y">The coordinate Y.</param>
    /// <param name="ball">The ball image number.</param>
    public void ShowBox(int x, int y, int ball)
    {
        buttons[x, y].GetComponent<Image>().sprite = images[ball].sprite;
    }


    /// <summary>
    /// Play a melody when the balls are located 
    /// in a row, a column or a diagonal 
    /// in the required quantity.
    /// </summary>
    public void PlayCut()
    {
        audioSource.Play();
    }


    /// <summary>
    /// Click by the cell on the game board.
    /// </summary>
    public void Click()
    {
        string name = EventSystem.current.currentSelectedGameObject.name;

        int nr = GetNumber(name);
        int x = nr % Lines.SIZE;
        int y = nr / Lines.SIZE;

        lines.Click(x, y);

        SendMapToServer();
    }

    /// <summary>
    /// Send the map of balls to the server.
    /// </summary>
    public void SendMapToServer()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(API_URL + lines.GetMapForSendServer());

        // If you need to get an answer from the server, then:
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new(response.GetResponseStream());
        string answer = reader.ReadToEnd();
    }


    /// <summary>
    /// Initialization of the cells of the game board.
    /// </summary>
    private void InitButtons()
    {
        buttons = new Button[Lines.SIZE, Lines.SIZE];

        for (int nr = 0; nr < buttons.Length; nr++)
        {
            buttons[nr % Lines.SIZE, nr / Lines.SIZE] = GameObject.Find($"Button ({nr})").GetComponent<Button>();
        }
    }

    /// <summary>
    /// Initialization of the ball images.
    /// </summary>
    private void InitImages()
    {
        images = new Image[Lines.BALLS];

        for (int j = 0; j < images.Length; j++)
        {
            images[j] = GameObject.Find($"Image ({j})").GetComponent<Image>();
        }
    }


    /// <summary>
    /// Get the game cell number by the name.
    /// </summary>
    /// <param name="name">The cell name.</param>
    /// <returns>The number of the game cell.</returns>
    /// <exception cref="Exception">Unrecognized object name.</exception>
    private int GetNumber(string name)
    {
        Regex regex = new(@"\((\d+)\)");
        Match match = regex.Match(name);

        if (!match.Success)
        {
            throw new Exception("Unrecognized object name");
        }

        Group group = match.Groups[1];
        string number = group.Value;

        return Convert.ToInt32(number);
    }
}
