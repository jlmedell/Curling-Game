using UnityEngine;

public class ScoreColliders : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        StoneController stone = other.gameObject.GetComponent<StoneController>();
        stone.inHome = true;
        Debug.Log("Stone enters " + stone.inHome);
    }


    private void OnTriggerExit(Collider other)
    {
        StoneController stone = other.gameObject.GetComponent<StoneController>();
        stone.inHome = false;
        Debug.Log("Stone leaves " + stone.inHome);
    }
}
