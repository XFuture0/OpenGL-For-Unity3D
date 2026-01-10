using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SingleTons<InputManager>
{
    public float GetKeyDown_Horizontal()
    {
        return Input.GetAxisRaw("Horizontal");
    }
    public float GetKeyDown_Vertical()
    {
        return Input.GetAxisRaw("Vertical");
    }
    public bool GetKey_Space()
    {
        return Input.GetKey(KeyCode.Space);
    }
}
