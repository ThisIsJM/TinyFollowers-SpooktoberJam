using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CultMember : MonoBehaviour
{
 
    Grid<PathNode> grid;
    public Vector3 destination;
    public float speed;

    public PathNode currentNode;
    public PathNode cameFromNode;
    public Vector2 cameFromNodeCoords;
    [Header("GFX")]
    public Animator dustGFX;
    Animator animator;
    [SerializeField]
    CultMovement.State currentState = CultMovement.State.Petrified;

    bool isFacingRight = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (gameObject.transform.parent == CultMovement.instance.transform)
        {
            animator.SetBool("isRecruited", true);
            CultMovement.instance.ReachedDestination += LandedToDestination;
            currentState = CultMovement.State.Idle;
        }
       
        grid = GridManager.instance.grid;
        transform.position =  SnapToGrid(transform.position);
        SetCurrentNode();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState.Equals(CultMovement.State.Moving))
        {
            Flip();
         
            if (Vector3.Distance(transform.position, destination) < 0.1)
            {
                
                LandedToDestination();
            }
        }
    }

    public void LandedToDestination()
    {
        currentState = CultMovement.State.Idle;
        SetCurrentNode();
        FindCultMemberInNodes(currentNode);
        dustGFX.SetTrigger("Poof");
    }

    void Flip()
    {
        grid.GetXY(destination, out int x, out int y);
        PathNode currentNode = GridManager.instance.GetNode(x, y);
        if ((currentNode.x > cameFromNode.x && !isFacingRight) || (currentNode.x < cameFromNode.x && isFacingRight))
        {
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
            isFacingRight = !isFacingRight;
        }
    }
    Vector3 SnapToGrid(Vector3 worldPosition)
    {
      
        grid.GetXY(worldPosition, out int x, out int y);
        return grid.GetWorldPosition(x, y) + new Vector3(grid.GetCellSize(), grid.GetCellSize()) * .5f;
    }

    public void MoveToGridPosition(int x, int y)
    {
        SetCameFromNode();
        //Move the cult member to the new position
        currentState = CultMovement.State.Moving;
        destination =  SnapToGrid(grid.GetWorldPosition(x, y));
    }

    public void SetCameFromNode()
    {
        //Set current node as a came from node
        grid.GetXY(transform.position, out int x, out int y);
        cameFromNode = GridManager.instance.GetNode(x, y);
        //Remove member from node
        if (cameFromNode.cultMember == this)
        {
            cameFromNodeCoords = new Vector2(x, y);
            cameFromNode.cultMember = null;
        }
        
    }

    public void SetCurrentNode()
    {
        //Set current node as a came from node
        grid.GetXY(transform.position, out int x, out int y);
        currentNode = GridManager.instance.GetNode(x, y);

        //Add member from node
        currentNode.cultMember = this;
    }

    public void TeleportToDestination() //Move the CultMember to the destination using the animator
    {
        transform.position = destination;
        SetCurrentNode();
        FindCultMemberInNodes(currentNode);
        animator.SetBool("isTeleporting", false);
    }

    public void FindCultMemberInNodes(PathNode node)
    {
        List<PathNode> neighbourNodes = GridManager.instance.GetNeighbourNode(node);

        foreach (PathNode neighbour in neighbourNodes)
        {
            //Check if it has a cult member
            if (neighbour.HasCultMember())
            {
                GameObject member = neighbour.cultMember.gameObject;
                if (!CultMovement.instance.cultMembers.Contains(member)) //Check if the obj is not yet inside the cult list
                {
                    Debug.Log(neighbour.x + "," + neighbour.y);
                    //Add it as a child of the cult object
                    member.transform.SetParent(transform.parent);
                    //Add it in the list
                    CultMovement.instance.AddMember(member);
                }
            }
        }
    }
    
    public CultMovement.State GetCurrentState()
    {
        return currentState;
    }

    public void SetCurrentState(CultMovement.State state)
    {
        currentState = state;
    }
}
