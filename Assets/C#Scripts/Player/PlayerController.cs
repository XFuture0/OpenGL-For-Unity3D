using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public float JumpSpeed;
    public float MouseSensitivityX;
    public float MouseSensitivityY;
    private float YRotation;
    private float XRotation;
    private void Update()
    {
        BreakBlock();
        CreateBlock();
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
        float mouseX = InputManager.Instance.GetKey_MouseX() * MouseSensitivityX * Time.deltaTime;
        float mouseY = InputManager.Instance.GetKey_MouseY() * MouseSensitivityY * Time.deltaTime;
        YRotation += mouseX;
        XRotation -= mouseY;
        XRotation = Mathf.Clamp(XRotation, -90, 90);
        transform.rotation = Quaternion.Euler(XRotation, YRotation, 0);
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
    private void CreateBlock()
    {
        if (InputManager.Instance.GetKeyDown_MouseRight())
        {
            BlockGraphicsRayCastHit hit = new BlockGraphicsRayCastHit();
            List<Vector3> Angles = new List<Vector3>(3);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (GraphicsRayCast.TryBlockGraphicsRayCast(ray, GraphicsRayCast.GetRayCastPartBlocks(ray, MapManager.Instance.genPerlinNoiseMap.PartBlocks), out hit))
            {
                Vector3 LocalRay = ray.origin - hit.Position.GetPosition();
                if (Vector3.Dot(LocalRay, new Vector3(1, 0, 0)) > 0) Angles.Add(new Vector3(1,0,0));
                if (Vector3.Dot(LocalRay, new Vector3(-1, 0, 0)) > 0) Angles.Add(new Vector3(-1,0,0));
                if (Vector3.Dot(LocalRay, new Vector3(0, 0, 1)) > 0) Angles.Add(new Vector3(0,0,1));
                if (Vector3.Dot(LocalRay, new Vector3(0, 0, -1)) > 0) Angles.Add(new Vector3(0,0,-1));
                if (Vector3.Dot(LocalRay, new Vector3(0, 1, 0)) > 0) Angles.Add(new Vector3(0, 1, 0));
                if (Vector3.Dot(LocalRay, new Vector3(0, -1, 0)) > 0) Angles.Add(new Vector3(0, -1, 0));
                float MinAngle = int.MaxValue;
                Vector3 AngleVector = Vector3.zero;
                foreach (var Angle in Angles)
                {
                    if(Vector3.Angle(Angle, LocalRay) < MinAngle)
                    {
                        MinAngle = Vector3.Angle(Angle, LocalRay);
                        AngleVector = Angle;
                    }
                }
                hit.Position.SetTRS(hit.Position.GetPosition() + AngleVector, Quaternion.identity,new Vector3(1,1,1));
                MapManager.Instance.CreateBlocks(hit.Position);
            }
        }
    }
}
