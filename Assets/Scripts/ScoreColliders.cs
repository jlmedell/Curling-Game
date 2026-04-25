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

    private void OnTriggerEnter(Collider other)
    {
        StoneController stone = other.gameObject.GetComponent<StoneController>();
        stone.scoreString = objectName;
        Debug.Log("Stone enters " + stone.scoreString);
    }

    private  void OnTriggerStay(Collider other)
    {
        StoneController stone = other.gameObject.GetComponent<StoneController>();
        stone.scoreString = objectName;
        Debug.Log("Stone stays " + stone.scoreString);
    }

    private void OnTriggerExit(Collider other)
    {
        StoneController stone = other.gameObject.GetComponent<StoneController>();
        stone.scoreString = "None";
        Debug.Log("Stone leaves " + stone.scoreString);
    }
}
