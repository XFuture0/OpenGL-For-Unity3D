using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Mesh mesh;
    public Material mat;
    private int InstanceCount = 10000;
    private Matrix4x4[] InstanceMatrices;
    private void Start()
    {
        int count = 0;
        InstanceMatrices = new Matrix4x4[InstanceCount];//´æ´¢100¸ö¾ØÕó
        for(int i = 0; i < 100; i++)
        {
            for(int j = 0; j < 100; j++)
            {
                InstanceMatrices[count] = Matrix4x4.TRS(new Vector3(i, 0, j), Quaternion.identity, Vector3.one);
                count++;
            }
        }
    }
    private void Update()
    {
        Graphics.DrawMeshInstanced(mesh, 0, mat, InstanceMatrices, InstanceCount);
    }
}
