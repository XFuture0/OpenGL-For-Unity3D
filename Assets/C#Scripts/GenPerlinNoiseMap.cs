using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class GenPerlinNoiseMap : MonoBehaviour
{
    public class PartBlockPro
    {
        public int Count;
        public Vector3 PartOffect;
        public override bool Equals(object obj)
        {
            return obj is PartBlockPro other &&
               PartOffect == other.PartOffect;
        }
        public override int GetHashCode()
        {
            return PartOffect.GetHashCode();
        }
    }
    public float scale;
    public Mesh mesh;
    public Material mat;
    public int LayerCount;
    public float ViewDistance;
    private Vector3 CurPart;
    private List<Matrix4x4> BlockMatrices = new List<Matrix4x4>();
    private Dictionary<PartBlockPro,List<Matrix4x4>> PartBlocks = new Dictionary<PartBlockPro,List<Matrix4x4>>();
    private void Start()
    {
        StartGenPerlinNoiseMapPer(LayerCount);
    }
    private void Update()
    {
        UpdateCurPart();
        CheckPerlinNoiseMapPer();
        OnGenPerlinNoiseMap();
    }
    private void UpdateCurPart()
    {
        var CameraPosition = Camera.main.transform.position;
        CurPart = new Vector3((int)CameraPosition.x / 50, 0, (int)CameraPosition.z / 50);
        if(CameraPosition.x < 0) CurPart.x--;
        if(CameraPosition.z < 0) CurPart.z--;
    }
    private void OnGenPerlinNoiseMap()
    {
        foreach (var part in PartBlocks)
        {
            var PartDistance = math.abs(Vector3.Distance(Camera.main.transform.position,part.Key.PartOffect));
            if(PartDistance < ViewDistance)
            {
                Graphics.DrawMeshInstanced(mesh, 0, mat, part.Value.ToArray(), part.Key.Count);
            }
        }
    }
    private void StartGenPerlinNoiseMapPer(int LayerCount)
    {
        for (int i = 0; i < LayerCount; i++)
        {
            for (int j = 0; j < LayerCount; j++)
            {
                AddGenPerlinNoiseMapPer(new Vector2(i, j));
            }
        }
    }
    private void AddGenPerlinNoiseMapPer(Vector2 AddPart)
    {
        int BlockCount = 0;
        BlockMatrices.Clear();
        for (int m = 0; m < 50; m++)
        {
            for (int n = 0; n < 50; n++)
            {
                int GroundHigh = (int)(Mathf.PerlinNoise((50 * AddPart.x + m) * scale, (50 * AddPart.y + n) * scale) * 10);
                for (int k = 0; k < GroundHigh; k++)
                {
                    BlockMatrices.Add(Matrix4x4.TRS(new Vector3(50 * AddPart.x + m, k, 50 * AddPart.y + n), Quaternion.identity, Vector3.one));
                    BlockCount++;
                }
            }
        }
        var CurBlockMatrices = new List<Matrix4x4>();
        CurBlockMatrices.AddRange(BlockMatrices);
        var CurBlockPro = new PartBlockPro();
        CurBlockPro.Count = BlockCount;
        CurBlockPro.PartOffect = new Vector3(50 * AddPart.x + 25, 0, 50 * AddPart.y + 25);
        PartBlocks.Add(CurBlockPro, CurBlockMatrices);
    }
    private void CheckPerlinNoiseMapPer()
    {
        var CheckPart = new PartBlockPro();
        CheckPart.PartOffect = new Vector3(50 * CurPart.x + 25, 0, 50 * CurPart.z + 25);
        if (!PartBlocks.ContainsKey(CheckPart)) AddGenPerlinNoiseMapPer(new Vector2(CurPart.x, CurPart.z));
        CheckPart.PartOffect = new Vector3(50 * (CurPart.x + 1) + 25, 0, 50 * CurPart.z + 25);
        if (!PartBlocks.ContainsKey(CheckPart)) AddGenPerlinNoiseMapPer(new Vector2(CurPart.x + 1, CurPart.z));
        CheckPart.PartOffect = new Vector3(50 * CurPart.x + 25, 0, 50 * (CurPart.z + 1) + 25);
        if (!PartBlocks.ContainsKey(CheckPart)) AddGenPerlinNoiseMapPer(new Vector2(CurPart.x, CurPart.z + 1));
        CheckPart.PartOffect = new Vector3(50 * (CurPart.x - 1) + 25, 0, 50 * CurPart.z + 25);
        if (!PartBlocks.ContainsKey(CheckPart)) AddGenPerlinNoiseMapPer(new Vector2(CurPart.x - 1, CurPart.z));
        CheckPart.PartOffect = new Vector3(50 * CurPart.x + 25, 0, 50 * (CurPart.z - 1) + 25);
        if (!PartBlocks.ContainsKey(CheckPart)) AddGenPerlinNoiseMapPer(new Vector2(CurPart.x, CurPart.z - 1));
        CheckPart.PartOffect = new Vector3(50 * (CurPart.x + 1) + 25, 0, 50 * (CurPart.z + 1) + 25);
        if (!PartBlocks.ContainsKey(CheckPart)) AddGenPerlinNoiseMapPer(new Vector2(CurPart.x + 1, CurPart.z + 1));
        CheckPart.PartOffect = new Vector3(50 * (CurPart.x + 1) + 25, 0, 50 * (CurPart.z - 1) + 25);
        if (!PartBlocks.ContainsKey(CheckPart)) AddGenPerlinNoiseMapPer(new Vector2(CurPart.x + 1, CurPart.z - 1));
        CheckPart.PartOffect = new Vector3(50 * (CurPart.x - 1) + 25, 0, 50 * (CurPart.z + 1) + 25);
        if (!PartBlocks.ContainsKey(CheckPart)) AddGenPerlinNoiseMapPer(new Vector2(CurPart.x - 1, CurPart.z + 1));
        CheckPart.PartOffect = new Vector3(50 * (CurPart.x - 1) + 25, 0, 50 * (CurPart.z - 1) + 25);
        if (!PartBlocks.ContainsKey(CheckPart)) AddGenPerlinNoiseMapPer(new Vector2(CurPart.x - 1, CurPart.z - 1));
    }
}
