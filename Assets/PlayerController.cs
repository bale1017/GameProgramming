using BasePatterns;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    public PlayerSwordAttack swordAttack; //import script

    Vector2 movementInput;
    public float moveSpeed = 1f;
    public float collisionOffset = 0.01f;
    public ContactFilter2D movementFilter;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private bool canMove = true;
    private bool isDead = false;

    Health health;

    public static PlayerController Instance { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Instance = this;
        Health health = GetComponent<Health>();
        if (health == null)
        {
            health = GetComponent<Health>();
        }
        health.OnDeath.AddListener(Defeated);
        health.OnHealthDecreaseBy.AddListener(ReceivedDamage);
    }

    private void FixedUpdate()
    {
        if (Game.current.IsRunning()) 
        { 
            if (canMove && !isDead && !Game.IsRewinding)
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
                if (movementInput.x < 0 && !spriteRenderer.flipX)
                {
                    GetComponent<ReTime>().AddKeyFrame(
                        g => g.GetComponent<SpriteRenderer>().flipX = true,
                        g => g.GetComponent<SpriteRenderer>().flipX = false
                    );
                }
                else if (movementInput.x > 0 && spriteRenderer.flipX)
                {
                    GetComponent<ReTime>().AddKeyFrame(
                        g => g.GetComponent<SpriteRenderer>().flipX = false,
                        g => g.GetComponent<SpriteRenderer>().flipX = true
                    );
                }
            }
    }
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
        if (Game.current.IsRunning())
        {
            if (canMove && !isDead && !Game.IsRewinding)
            {
                //start "Sword Attack" animation for player
                animator.SetTrigger("swordAttack");
            }
        }
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
        StartCoroutine(EndSwordAttack());
    }

    public IEnumerator EndSwordAttack()
    {
        yield return new WaitForSeconds(1);
        swordAttack.StopAttack();
        UnlockMovement();
    }

    public void LockMovement()
    {
        canMove = false;
    }

    public void UnlockMovement()
    {
        canMove = true;
    }

    public void Defeated()
    {
        isDead = true;
        Debug.Log("Player has been defeated");
        // trigger death animation of player
        animator.SetBool("isMoving", false);
        animator.SetBool("defeated", true);
        SoundPlayer.current.PlaySound(Sound.PLAYER_DEATH);
        StartCoroutine(PlayerDefeated());
    }

    public void ReceivedDamage(float val)
    {
        // TakeDamage.Play();
        SoundPlayer.current.PlaySound(Sound.PLAYER_DAMAGE);
        Debug.Log("Player received " + val + " damage");
        // trigger damage receiving animation of player
        animator.SetTrigger("receivesDamage");
    }

    public IEnumerator PlayerDefeated()
    { 
        yield return new WaitForSeconds(1);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        animator.SetBool("defeated", false);
        Game.current.FailGame();
    }

    public Vector3 GetPosition() 
    {
        return transform.position;
    }
}