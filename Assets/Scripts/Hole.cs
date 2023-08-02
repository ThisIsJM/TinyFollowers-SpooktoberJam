using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    Animator animator;
    GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Cult Member"))
        {

            target = collision.gameObject;
            //Set the state to being eaten after finishing to move
            CultMovement.instance.CultMemberAction += EatCultMember;
           
            animator.SetTrigger("Eat");
        }
    }

    void EatCultMember()
    {
        target.GetComponent<CultMember>()?.SetCurrentState(CultMovement.State.BeingEaten);
        CultMovement.instance.CultMemberAction -= this.EatCultMember;
    }

    void EatAnimationKey()
    {
        if (target != null)
        {
            AudioManager.instance.Play("Eat SFX");
            target.GetComponentInParent<CultMovement>().RemoveMember(target);
        }
           
    }
}
