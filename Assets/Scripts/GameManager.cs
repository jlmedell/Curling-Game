using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject prefab; //stone prefab
    public Transform spawn; //spawn point
    private int round = 1; //current round
    private int throws = 0; //number of stones thrown
    private int currentTeam = 0;
    private string[] team = {"Blue", "Red"};
    private int score1 = 0; //red team score
    private int score2 = 0; //blue team score
    private GameObject curr; //current stone

    //scoring
    public GameObject scoringCircle;
    
    //UI
    public TextMeshProUGUI scoreDisplay;
    public TextMeshProUGUI currentEndDisplay;

    void Start()
    {
        StartRound();
    }
    void StartRound()
    {
        throws = 0;
        currentTeam = 0;

        updateRoundDisplay();
        updateScoreDisplay();

        Cleanup(); //delete stones

        NextStone();
    }

    void Cleanup()
{
    StoneController[] stones = FindObjectsOfType<StoneController>();

    foreach (StoneController stone in stones)
    {
        Destroy(stone.gameObject);
    }
}
    public void NextStone()
    {
        if (throws >= 8)
        {
            EndRound();
            return;
        }
        curr = Instantiate(prefab, spawn.position, spawn.rotation);

        Renderer r = curr.GetComponentInChildren<Renderer>();
        if (r != null)
        {
            r.material.color = (currentTeam == 0) ? Color.red : Color.blue;
        }
        curr.tag = (currentTeam == 0) ? "RedStone" : "BlueStone";

        throws = throws + 1;
        currentTeam = 1 - currentTeam;
        updateRoundDisplay();
    }

    void updateRoundDisplay()
    {
        currentEndDisplay.text = string.Format("Round {0}\nTeam {1}", (round), (team[currentTeam]));
    }

    void updateScoreDisplay()
    {
        scoreDisplay.text = string.Format("Scores\n Red {0}\n Blue {1}", (score1), (score2));
    }

    void EndRound()
    {
        scoring();

        round = round + 1;
        updateRoundDisplay();

        if (round > 2)
        {
            end();
        }
        else
        {
            Invoke(nameof(StartRound), 2f);
        }
    }

    void scoring()
    {
        List<StoneController> reds = new List<StoneController>(); //red stones
        List<StoneController> blues = new List<StoneController>(); //blue stones

        StoneController[] stones = FindObjectsOfType<StoneController>();

        StoneController closestRed = null;
        StoneController closestBlue = null;
        
        //seperate stones
        foreach (StoneController stone in stones)
        {
            if(stone.tag == "RedStone")
            {
                reds.Add(stone);
            }
            else
            {
                blues.Add(stone);
            }
        }

        
        foreach (StoneController stone in reds)
        {
            if(stone.inHome == true)
            {
                stone.distanceFromCenter = Vector3.Distance(stone.transform.position, scoringCircle.transform.position);
            }
        }
        foreach (StoneController stone in blues)
        {
            if(stone.inHome == true)
            {
                stone.distanceFromCenter = Vector3.Distance(stone.transform.position, scoringCircle.transform.position);
            }
        }

        //check closest stone
        foreach (StoneController stone in reds)
        {
            if(stone.inHome == true)
            {
                if(closestRed == null)
                {
                    closestRed = stone;
                }
                else if(stone.distanceFromCenter < closestRed.distanceFromCenter)
                {
                    closestRed = stone;
                }
            }
        }
        foreach (StoneController stone in blues)
        {
            if(stone.inHome == true)
            {
                if(closestBlue == null)
                {
                    closestBlue = stone;
                }
                else if(stone.distanceFromCenter < closestBlue.distanceFromCenter)
                {
                    closestBlue = stone;
                }
            }
        }

        if(closestRed == null && closestBlue == null)
        {
            Debug.Log("Lol Get Good");
        }
        else if(closestRed == null && closestBlue != null)
        {
            foreach (StoneController stone in blues)
            {
                if(stone.inHome == true)
                {
                    score2 = score2 + 1;
                }
            }
        }
        else if(closestRed != null && closestBlue == null)
        {
            foreach (StoneController stone in reds)
            {
                if(stone.inHome == true)
                {
                    score1 = score1 + 1;
                }
            }
        }
        else if(closestRed.distanceFromCenter < closestBlue.distanceFromCenter)
        {
            foreach (StoneController stone in reds)
            {
                if(stone.distanceFromCenter < closestBlue.distanceFromCenter)
                {
                    if(stone.inHome == true)
                    {
                        score1 = score1 + 1;
                    }
                }
            }
        }
        else if(closestBlue.distanceFromCenter <= closestRed.distanceFromCenter)
        {
            foreach (StoneController stone in blues)
            {
                if(stone.distanceFromCenter < closestRed.distanceFromCenter)
                {
                    if(stone.inHome == true)
                    {
                        score2 = score2 + 1;
                    }
                }
            }
        }
        
        updateScoreDisplay();
    }

    void end()
    {
        Debug.Log("GAME OVER");
    }

    void OnRestart(InputValue press)
    {
        Debug.Log("Restart Pressed!");
        round = 1;
        score1 = 0;
        score2 = 0;
        StartRound();

    }

    public void Stopped()
    {
        Invoke(nameof(NextStone), 0.5f); //delay before next turn
    }
}
