using UnityEngine;
using TMPro;
using System.Collections;

public class TextController : MonoBehaviour
{
    public TextMeshProUGUI centralText;
    public TextMeshProUGUI totalGames;
    public TextMeshProUGUI wonAfterChange;
    public TextMeshProUGUI winRate;

    private string defaultText = "Select a door!";
    private string switchText = "You can switch or stay!";
    private string winText = "You won!\n(click to restart)";
    private string loseText = "You lost!\n(click to restart)!";

    public float flickerInterval = 1300f; // Time interval between flickers in seconds

    private float timer; // Timer to keep track of time passed

    // Public getters for the private string variables
    public string DefaultText
    {
        get { return defaultText; }
    }

    public string SwitchText
    {
        get { return switchText; }
    }

    public string WinText
    {
        get { return winText; }
    }

    public string LoseText
    {
        get { return loseText; }
    }
    void Start()
    {
        centralText.text = defaultText;
        // Start the flicker coroutine
        StartCoroutine(FlickerText());
    }
    void Update(){
    }
    IEnumerator FlickerText()
    {
        while (true)
        {
            // Toggle the text visibility
            centralText.enabled = !centralText.enabled;

            yield return new WaitForSeconds(1f);
        }
    }
    // Function to update the text
    public void UpdateText(string newText)
    {
        centralText.text = newText;
    }

    public void UpdateTotalGames(int number)
    {
        totalGames.text = "Total played: " + number;
    }

    public void UpdateWonAfterChange(int number)
    {
        wonAfterChange.text = "Wins After Change: " + number;
    }
    
    public void UpdateWinRate(double number)
    {
        winRate.text = "Win Rate(if change): " + number +"%";
    }
}