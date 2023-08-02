using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{

    Goal[] goalTiles;
    bool levelComplete;
    // Start is called before the first frame update
    void Start()
    {
        goalTiles = gameObject.transform.GetComponentsInChildren<Goal>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GoalCheck()
    {
        foreach (Goal goal in goalTiles)
        {

            GridManager.instance.grid.GetXY(goal.transform.position, out int x, out int y);
            PathNode goalNode = GridManager.instance.GetNode(x, y);
            if (!goalNode.HasCultMember())
            {
                levelComplete = false;
                break;
            }
            else
            {
                Debug.Log("Load Next Level");
                levelComplete = true;
            }
        }

        if (levelComplete)
        {
            if (!AudioManager.instance.isPlaying("Glow SFX"))
            {
                AudioManager.instance.Play("Glow SFX");
            }
            foreach (Goal goal in goalTiles)
            {
                goal.GetComponent<Animator>().SetTrigger("Glow");
            }
           
            StartCoroutine(GameManager.instance.LoadNextLevel());
        }

    }

   
}
