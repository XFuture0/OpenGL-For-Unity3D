using UnityEngine;

public static class StaticBlock
{
    public static Vector3[] Cube_Vertex =
    {
        new Vector3(0.50f, -0.50f, 0.50f),
        new Vector3(-0.50f, -0.50f, 0.50f),
        new Vector3(0.50f, 0.50f, 0.50f),
        new Vector3(-0.50f, 0.50f, 0.50f),
        new Vector3(0.50f, 0.50f, -0.50f),
        new Vector3(-0.50f, 0.50f, -0.50f),
        new Vector3(0.50f, -0.50f, -0.50f),
        new Vector3(-0.50f, -0.50f, -0.50f),
        new Vector3(0.50f, 0.50f, 0.50f),
        new Vector3(-0.50f, 0.50f, 0.50f),
        new Vector3(0.50f, 0.50f, -0.50f),
        new Vector3(-0.50f, 0.50f, -0.50f),
        new Vector3(0.50f, -0.50f, -0.50f),
        new Vector3(0.50f, -0.50f, 0.50f),
        new Vector3(-0.50f, -0.50f, 0.50f),
        new Vector3(-0.50f, -0.50f, -0.50f),
        new Vector3(-0.50f, -0.50f, 0.50f),
        new Vector3(-0.50f, 0.50f, 0.50f),
        new Vector3(-0.50f, 0.50f, -0.50f),
        new Vector3(-0.50f, -0.50f, -0.50f),
        new Vector3(0.50f, -0.50f, -0.50f),
        new Vector3(0.50f, 0.50f, -0.50f),
        new Vector3(0.50f, 0.50f, 0.50f),
        new Vector3(0.50f, -0.50f, 0.50f),
    };
    public static int[] Cube_Index =
    {
        0,2,3,
        0,3,1,
        8,4,5,
        8,5,9,
        10,6,7,
        10,7,11,
        12,13,14,
        12,14,15,
        16,17,18,
        16,18,19,
        20,21,22,
        20,22,23
    };
    public static Vector2[] Cube_UV =
    {
        new Vector2(0.0f, 0.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 1.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 1.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 1.0f),
        new Vector2(0.0f, 0.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(0.0f, 0.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(0.0f, 0.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(0.0f, 0.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
        new Vector2(0.0f, 0.0f),
        new Vector2(0.0f, 1.0f),
        new Vector2(1.0f, 1.0f),
        new Vector2(1.0f, 0.0f),
    };
}
