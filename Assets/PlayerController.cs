using BasePatterns;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IController
{
    Animator animator;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    BoxCollider2D bxc;
    public SwordAttack swordAttack; //import script
    public Health health;

    Vector2 movementInput;
    public float moveSpeed = 1f;
    public float collisionOffset = 0.01f;
    public ContactFilter2D movementFilter;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    bool canMove = true;
    public float healthAmount = 25;

    public static PlayerController Instance { get; private set; }
    Health IController.health { get => health; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        bxc = GetComponent<BoxCollider2D>();
        Instance = this;
        health = new Health(healthAmount, ReceivedDamage, Defeated);
    }

    private void FixedUpdate()
    {
        //if (!Game.current.IsRunning()) { 
            if (canMove && !Game.IsRewinding && !PauseMenu.Paused)
            {
                //If movement input is not 0, try to move
                if (movementInput != Vector2.zero)
                {
                    bool success = TryMove(movementInput);
                    if (!success)
                    {
                        success = TryMove(new Vector2(movementInput.x, 0));
                        if (!success)
                        {
                            success = TryMove(new Vector2(0, movementInput.y));
                        }
                    }
                    //set "moving" animation
                    animator.SetBool("isMoving", success);
                }
                else
                {
                    //set "idle" animation
                    animator.SetBool("isMoving", false);
                }

                //Set direction of sprite to movemnet direction
                if (movementInput.x < 0)
                {
                    spriteRenderer.flipX = true;
                }
                else if (movementInput.x > 0)
                {
                    spriteRenderer.flipX = false;
                }
            }
        //}
    }

    private bool TryMove(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            //Check for potential collisions
            int count = rb.Cast(
                   direction, //x and y values between -1 and 1 that represent the direction from the body to look for collisions
                   movementFilter, //the settings that determine where a collision can occur on such as layers to collide with
                   castCollisions, //list of collisions to store the found collisions into after the cast is finished
                   moveSpeed * Time.fixedDeltaTime + collisionOffset); //the amount to cast equal to the movement plus an offset

            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            //Can't move if there's no directionn to move in
            return false;
        }
    }

    //Function called by the "Player Input" component and processes the movement of the char
    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnFire()
    {
        //start "Sword Attack" animation for player
        animator.SetTrigger("swordAttack");
    }
    public void SwordAttack()
    {
        LockMovement();
        if (spriteRenderer.flipX == true)
        {
            swordAttack.AttackLeft();
        }
        else
        {
            swordAttack.AttackRight();
        }
    }
    public void EndSwordAttack()
    {
        UnlockMovement();
        swordAttack.StopAttack();
    }
    public void LockMovement()
    {
        canMove = false;
    }
    public void UnlockMovement()
    {
        canMove = true;
    }

    public void Defeated(float val)
    {
        Debug.Log("Player has been defeated");
        // trigger death animation of player
        animator.SetBool("defeated", true);
    }

    public void ReceivedDamage(float val)
    {
        Debug.Log("Player received " + val + " damage");
        // trigger damage receiving animation of player
        animator.SetTrigger("receivesDamage");
    }

    public void PlayerDefeated()
    { // called from inside "death"-animation
      //TODO implement logic for player death
        gameObject.SetActive(false);
        bxc.enabled = false;
        animator.SetBool("defeated", false);
    }

    public Vector3 GetPosition() 
    {
        return transform.position;
    }
}