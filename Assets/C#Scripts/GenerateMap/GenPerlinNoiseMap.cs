using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

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
    public float ViewDistance;
    public int Count;
    private Vector3 CurPart;
    private List<Matrix4x4> BlockMatrices = new List<Matrix4x4>();
    private Dictionary<PartBlockPro,List<Matrix4x4>> PartBlocks = new Dictionary<PartBlockPro,List<Matrix4x4>>();
    private Mesh NewMesh;
    private void Start()
    {
        // StartGenMap();
        AddGenPerlinNoiseMapPer(new Vector2(0, 0));
        foreach (var part in PartBlocks)
        {
            var mesh = VertexCombine(part.Key.Count, part.Value);
            NewMesh = mesh;
        }
    }
    private void StartGenMap()
    {
        for(int i = 0; i < Count; i++)
        {
            for (int j = 0; j < Count; j++)
            {
                AddGenPerlinNoiseMapPer(new Vector2(i, j));
            }
        }
    }
    private void Update()
    {
        Test();
        //UpdateCurPart();
        //CheckPerlinNoiseMapPer();
        //OnGenPerlinNoiseMap();
    }
    private void Test()
    {
        foreach (var part in PartBlocks)
        {
            Graphics.DrawMeshInstanced(NewMesh, 0, mat,part.Value.ToArray(), 1);
        }
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
    private void AddGenPerlinNoiseMapPer(Vector2 AddPart)
    {
        int BlockCount = 0;
        BlockMatrices.Clear();
        for (int m = 0; m < 10; m++)
        {
            for (int n = 0; n < 10; n++)
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
    private Mesh VertexCombine(int CombineCount, List<Matrix4x4> Transform)
    {
        CombineInstance[] combine = new CombineInstance[CombineCount];
        for (int i = 0; i < CombineCount; i++)
        {
            combine[i].mesh = mesh;
            combine[i].transform = Transform[i];
        }
        Mesh newMesh = new Mesh();
        newMesh.indexFormat = IndexFormat.UInt32;
        newMesh.CombineMeshes(combine,false);//是否合并原子网格，后续需要分配材质
        List<int> VisibleTriangles = new List<int>();
        for(int i = 0;i < newMesh.triangles.Length; i+= 3)
        {
            var Vertex1 = newMesh.vertices[newMesh.triangles[i]];
            var Vertex2 = newMesh.vertices[newMesh.triangles[i + 1]];
            var Vertex3 = newMesh.vertices[newMesh.triangles[i + 2]];
            var normal = Vector3.Cross(Vertex2 - Vertex1, Vertex3 - Vertex1).normalized;
            if(Vector3.Dot(normal, Camera.main.transform.forward) < 0)
            {
                VisibleTriangles.Add(newMesh.triangles[i]);
                VisibleTriangles.Add(newMesh.triangles[i + 1]);
                VisibleTriangles.Add(newMesh.triangles[i + 2]);
            }
        }
        newMesh.triangles = VisibleTriangles.ToArray();
        newMesh.RecalculateNormals();  // 修复法线计算
        newMesh.RecalculateBounds();   // 修复包围盒计算
        return newMesh;
    }
    private void CombineMaterial()
    {

    }
}
