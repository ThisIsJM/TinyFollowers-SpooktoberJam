using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TeleportTiles : MonoBehaviour
{
    public GameObject exit;
    PathNode exitNode;
    PathNode currentNode;
    GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        GridManager.instance.grid.GetXY(exit.transform.position, out int x, out int y);
        exitNode = GridManager.instance.GetNode(x, y);
        GridManager.instance.grid.GetXY(transform.position, out x, out y);
       currentNode = GridManager.instance.GetNode(x, y);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (currentNode.HasCultMember())
            {
                Debug.Log("Node has cult Member "+ currentNode.x + "," + currentNode.y);
                
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
         if (collision.CompareTag("Cult Member"))
         {
             CultMember cultMember = collision.GetComponent<CultMember>();

             if (cultMember.cameFromNode != exitNode && cultMember.cameFromNode != currentNode && cultMember.GetCurrentState() != CultMovement.State.Teleporting)
             {
                target = collision.gameObject;
                CultMovement.instance.CultMemberAction += TeleportCult; //Set the state to teleporting after moving               
             }
         }
    }

  
    void TeleportCult()
    {
       
        target.GetComponent<Animator>().SetBool("isTeleporting", true);
        target.GetComponent<CultMember>().SetCurrentState(CultMovement.State.Teleporting);
        target.GetComponent<CultMember>().destination = GridManager.instance.GetGridPosition(exit.transform.position);
        target.GetComponent<CultMember>().LandedToDestination();
        currentNode.cultMember = null;
        CultMovement.instance.CultMemberAction -= this.TeleportCult;
    }


}
