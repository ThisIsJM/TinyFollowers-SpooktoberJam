using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyeball : MonoBehaviour
{
    public GameObject target;
    public PathNode pathNode;
    // Start is called before the first frame update
    void Start()
    {
        GridManager.instance.grid.GetXY(transform.position, out int x, out int y);
        pathNode = GridManager.instance.GetNode(x, y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Cult Member"))
        {

            target = collision.gameObject;

            //Set the state to being petrified after finishing to move
            CultMovement.instance.CultMemberAction += PetrifyCultMember;

            
        }
    }

    void PetrifyCultMember()
    {
        CultMember cultMember = target.GetComponent<CultMember>();
        //Petrify SFX
        AudioManager.instance.Play("Awaken SFX");

        //Set the current node to unwalkable
        pathNode.isWalkable = false;

        //Cult Member turns to stone and current state is changed
        cultMember.SetCurrentState(CultMovement.State.Petrified);
        cultMember.GetComponent<Animator>().SetBool("isRecruited", false);

        //Remove the Cult Member from the cult List and remove it from the cult parent
        CultMovement.instance.cultMembers.Remove(cultMember.gameObject);
        cultMember.transform.SetParent(null);

        //Unsubscribe from the CultMovement's event Handlers
        CultMovement.instance.ReachedDestination -= cultMember.LandedToDestination;
        CultMovement.instance.CultMemberAction -= this.PetrifyCultMember;
    }


}
