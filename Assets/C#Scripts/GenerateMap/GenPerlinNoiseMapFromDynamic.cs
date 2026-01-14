using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenPerlinNoiseMapFromDynamic : MonoBehaviour
{
    public GameObject Block;
    public Transform BlockBox;
    private void Start()
    {
        for (int i = 0; i < 1000; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                Instantiate(Block, new Vector3(i, 0, j), Quaternion.identity,BlockBox);
            }
        }
    }
}
