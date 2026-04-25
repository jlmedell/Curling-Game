using UnityEngine;

public class ScoreColliders : MonoBehaviour
{
    public string objectName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void onTriggerEnter(Collider other)
    {
        StoneController stone = other.gameObject.GetComponent<StoneController>();
        stone.scoreString = objectName;
        Debug.Log("Stone enters " + stone.scoreString);
    }
    
    private void onTriggerExit(Collider other)
    {
        StoneController stone = other.gameObject.GetComponent<StoneController>();
        if(stone.scoreString == "Ring 12")
        {
            stone.scoreString = "None";
        }
        Debug.Log("Stone leaves " + stone.scoreString);
    }
}
