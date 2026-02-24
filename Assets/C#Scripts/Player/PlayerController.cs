using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public float JumpSpeed;
    public float MouseSensitivity;
    private void Update()
    {
        BreakBlock();
    }
    private void FixedUpdate()
    {
        ViewRoll();
        Move();
        Jump();
    }
    private void Move()
    {
        float MoveHorizontal = InputManager.Instance.GetKeyDown_Horizontal();
        float MoveVertical = InputManager.Instance.GetKeyDown_Vertical();
        Vector3 MoveForward = transform.forward;
        Vector3 MoveRight = transform.right;
        MoveForward.y = 0;
        MoveRight.y = 0;
        Vector3 MoveDirection = MoveForward * MoveVertical + MoveRight * MoveHorizontal;
        MoveDirection.Normalize();
        if(MoveDirection != Vector3.zero)
        {
            transform.position += MoveDirection * Speed * Time.deltaTime;
        }
    }
    private void ViewRoll()
    {
        float mouseX = InputManager.Instance.GetKey_MouseX();
        float mouseY = InputManager.Instance.GetKey_MouseY();
        transform.Rotate(new Vector3(0,1,0) * mouseX * MouseSensitivity);
        transform.Rotate(new Vector3(-1,0,0) * mouseY * MouseSensitivity);
        if(transform.localRotation.eulerAngles.x > 80 && transform.localRotation.eulerAngles.x < 270)
        {
            transform.localRotation = Quaternion.Euler(new Vector3(80,transform.localRotation.eulerAngles.y,0));
        }
        else if(transform.localRotation.eulerAngles.x < -80)
        {
            transform.localRotation = Quaternion.Euler(new Vector3(-80,transform.localRotation.eulerAngles.y,0));
        }
        else
        {transform.rotation = Quaternion.Euler(new Vector3(transform.localRotation.eulerAngles.x,transform.localRotation.eulerAngles.y,0));
        }
    }
    private void Jump()
    {
        if(InputManager.Instance.GetKey_Space())
        {
            transform.position += Vector3.up * JumpSpeed * Time.deltaTime;
        }
    }
    private void BreakBlock()
    {
        if (InputManager.Instance.GetKeyDown_MouseLeft())
        {
            BlockGraphicsRayCastHit hit = new BlockGraphicsRayCastHit();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(GraphicsRayCast.TryBlockGraphicsRayCast(ray, GraphicsRayCast.GetRayCastPartBlocks(ray,MapManager.Instance.genPerlinNoiseMap.PartBlocks), out hit))
            {
                MapManager.Instance.BreakBlocks(hit.Position);
            }
        }
    }
}
