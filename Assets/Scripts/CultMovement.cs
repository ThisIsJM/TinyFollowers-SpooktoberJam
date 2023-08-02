using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CultMovement : MonoBehaviour
{
    public enum State
    {
        Petrified,
        Idle,
        Moving,
        Teleporting,
        BeingEaten
    }

    public List<GameObject> cultMembers;

    Grid<PathNode> grid;

    public static CultMovement instance;

    public float moveDelay;
    private float moveDelayCounter;

    public float speed;
    Vector3 destination;

    public Action ReachedDestination;
    public Action CultMemberAction;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        grid = GridManager.instance.grid;

        cultMembers = new List<GameObject>();
        foreach (Transform cultMember in transform)
        {
            cultMembers.Add(cultMember.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cultMembers.Count == 0)
        {
            StartCoroutine(GameManager.instance.RestartLevel());
        }
        if (AllMemberIdle() && moveDelayCounter <= 0 && !GameManager.instance.IsNextLevelLoading() && cultMembers.Count > 0 && !GameManager.instance.IsPaused())
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                moveCult("Up");
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                moveCult("Down");
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveCult("Left");
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                moveCult("Right");
            }
        }
        else if(AllMemberIdle()) //Check if its not doing Any Action
        {
            moveDelayCounter -= Time.deltaTime;
        }

        if (CheckCultMemberState(State.Moving))
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, destination) < 0.1f)
            {
                AudioManager.instance.Play("Move SFX");
                transform.position = destination;
                ReachedDestination?.Invoke();
                CultMemberAction?.Invoke();
            }
        }
    }

    public bool CheckCultMemberState(State state) // Check if one of the cult member is currently in that state
    {
        foreach (GameObject cult in cultMembers)
        {
            if (state == cult.GetComponent<CultMember>().GetCurrentState()) { return true; }
        }
        return false;
    }

    public bool AllMemberIdle()
    {
        foreach (GameObject cult in cultMembers)
        {
            if (State.Idle != cult.GetComponent<CultMember>().GetCurrentState()) { /*Debug.Log(cult.GetComponent<CultMember>().GetCurrentState());*/ return false;  };
        }

        return true;
    }

    public void SetAllCultState(State state) // Change the current state of all the cultMember inside the list
    {
        foreach (GameObject cult in cultMembers)
        {
            cult.GetComponent<CultMember>().SetCurrentState(state);
        }
    }

    void moveCult(string dir)
    {
        bool tryToMove = true;
        Vector3 moveDistance = Vector3.zero;

        foreach (GameObject cult in cultMembers) // Checks if all of the members can move
        {
            Vector2Int gridDirection = GetGridDirection(cult.transform.position, dir);
       
            if (grid.GetGridObject(gridDirection.x, gridDirection.y) == null || !grid.GetGridObject(gridDirection.x, gridDirection.y).IsWalkable())
            {
                tryToMove = false;
                break;
            }
        }
        if (tryToMove) // Make all of the members move if it can
        {
            foreach (GameObject cult in cultMembers)
            {
                Vector2Int gridDirection = GetGridDirection(cult.transform.position, dir);
                cult.GetComponent<CultMember>().MoveToGridPosition(gridDirection.x, gridDirection.y);

                moveDistance = cult.GetComponent<CultMember>().destination - cult.transform.position; 
            }

            //Move the Parent Object
            destination = transform.position + moveDistance;

            moveDelayCounter = moveDelay;
            SetAllCultState(State.Moving);
        }
    }

    Vector2Int GetGridDirection(Vector3 position, string dir)
    {
        
        //Get XY
        grid.GetXY(position, out int x, out int y);
        PathNode currentNode = grid.GetGridObject(x, y);
        switch (dir)
        {
            case "Up":
                y++; break;
            case "Down":
                y--; break;
            case "Left":
                x--; break;
            case "Right":
                x++; break;
            default:
                break;
        }
        return new Vector2Int(x, y);
    }

    public void RemoveMember(GameObject member)
    {
        if (cultMembers.Contains(member))
        {
            cultMembers.Remove(member);
            ReachedDestination -= member.GetComponent<CultMember>().LandedToDestination;
            Destroy(member);
        }
    }

    public void AddMember(GameObject member)
    {
        if (!cultMembers.Contains(member))
        {
            //Add it to the List
            CultMember cultMember = member.GetComponent<CultMember>();
            cultMembers.Add(member);
            
            //Fucking Recruit the cult Member
            member.GetComponent<Animator>().SetBool("isRecruited", true);
            cultMember.SetCurrentState(State.Idle);
            ReachedDestination += cultMember.LandedToDestination;
            cultMember.FindCultMemberInNodes(member.GetComponent<CultMember>().currentNode);

            //Set the Node that the member is on to walkable
            GridManager.instance.grid.GetXY(member.transform.position, out int x, out int y);
            PathNode pathNode = GridManager.instance.GetNode(x, y);
            pathNode.isWalkable = true;

            if (!AudioManager.instance.isPlaying("Awaken SFX"))
            {
                AudioManager.instance.Play("Awaken SFX");
            }  
        }
    }
}
