using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public bool isOccupied;
    GoalManager goalManager;
    // Start is called before the first frame update
    void Start()
    {
        goalManager = gameObject.GetComponentInParent<GoalManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        isOccupied = true;
        goalManager.GoalCheck();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isOccupied = false;
    }
}
