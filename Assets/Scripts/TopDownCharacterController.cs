using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class TopDownCharacterController : MonoBehaviour
    {
        public bool active = false;

        private Animator animator;

        private Player playerScript;

        private void Start()
        {
            animator = GetComponent<Animator>();
            playerScript=GetComponent<Player>();
        }


        private void Update()
        {
            if (active) {
                Vector2 dir = Vector2.zero;
                if(!playerScript.immobile())
                {
                    if (Input.GetKey(KeyCode.A))
                    {
                        dir.x = -1;
                        animator.SetInteger("Direction", 3);
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        dir.x = 1;
                        animator.SetInteger("Direction", 2);
                    }

                    if (Input.GetKey(KeyCode.W))
                    {
                        dir.y = 1;
                        animator.SetInteger("Direction", 1);
                    }
                    else if (Input.GetKey(KeyCode.S))
                    {
                        dir.y = -1;
                        animator.SetInteger("Direction", 0);
                    }

                    dir.Normalize();
                }
                
                animator.SetBool("IsMoving", dir.magnitude > 0);

                GetComponent<Rigidbody2D>().velocity = Game.Instance.Settings.playerSpeed * dir;
            }
        }
    }
}
