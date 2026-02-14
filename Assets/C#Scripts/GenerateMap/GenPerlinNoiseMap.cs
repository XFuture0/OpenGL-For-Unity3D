using System.Collections.Generic;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
public class GenPerlinNoiseMap : MonoBehaviour
{
    private class PartBlockPro
    {
        public int Count;
        public Vector3 PartOffect;
        public Mesh PartMesh;
        public LodLayer lodLayer;
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
    public Material mat;
    public float ViewDistance;
    public int LayerCount;
    public int Lod1_LayerCount;
    public int Lod2_LayerCount;
    private Vector3 CurPart;
    private List<Matrix4x4> BlockMatrices = new List<Matrix4x4>();
    private Dictionary<PartBlockPro,List<Matrix4x4>> PartBlocks = new Dictionary<PartBlockPro,List<Matrix4x4>>();
    private Mesh NewMesh;
    private void Start()
    {
        StartGenMap();
    }
    private void StartGenMap()
    {
        for(int i = 0; i < LayerCount; i++)
        {
            for (int j = 0; j < LayerCount; j++)
            {
                AddGenPerlinNoiseMapPer(new Vector2(i, j));
                AddGenPerlinNoiseMapPer(new Vector2(-i - 1, j));
                AddGenPerlinNoiseMapPer(new Vector2(i, -j - 1));
                AddGenPerlinNoiseMapPer(new Vector2(-i - 1, -j - 1));
            }
        }
    }
    private void Update()
    {
        UpdateCurPart();
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
            var Distance = Vector3.Distance(part.Key.PartOffect, CurPart * 50 + new Vector3(25, 0, 25));
            if (Distance < ViewDistance)
            {
                RefreshPartBlockLodLayer(part.Key, part.Value, CurPart, part.Key.PartOffect);
                Graphics.DrawMeshInstanced(part.Key.PartMesh, 0, mat, part.Value.ToArray(), 1);
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
                    BlockMatrices.Add(Matrix4x4.TRS(new Vector3(25 * AddPart.x + m, k, 25 * AddPart.y + n), Quaternion.identity, Vector3.one));
                    BlockCount++;
                }
            }
        }
        var CurBlockPro = new PartBlockPro();
        CurBlockPro.PartOffect = new Vector3(50 * AddPart.x + 25, 0, 50 * AddPart.y + 25);
        if (!PartBlocks.ContainsKey(CurBlockPro))
        {
            var CurBlockMatrices = new List<Matrix4x4>();
            CurBlockMatrices.AddRange(BlockMatrices);
            CurBlockPro.Count = BlockCount;
            CurBlockPro.lodLayer = LodLayer.NULL;
            PartBlocks.Add(CurBlockPro, CurBlockMatrices);
        }
    }
    private Mesh VertexCombine(int CombineCount, List<Matrix4x4> Transform, Vector3[] Cube_Vertex, int[] Cube_Index, Vector2[] Cube_UV)
    {
        Mesh newMesh = new Mesh();
        newMesh.indexFormat = IndexFormat.UInt32;
        NativeArray<Vector3> Position = new NativeArray<Vector3>(CombineCount, Allocator.TempJob);
        for(int i = 0;i < Transform.Count; i++)
        {
            Position[i] = Transform[i].GetPosition();
        }
        NativeArray<Vector2> Mesh_UV = new NativeArray<Vector2>(Cube_UV.Length, Allocator.TempJob);
        NativeArray<Vector3> Mesh_Vertex = new NativeArray<Vector3>(Cube_Vertex.Length, Allocator.TempJob);
        NativeArray<int> Mesh_Index = new NativeArray<int>(Cube_Index.Length, Allocator.TempJob);
        NativeArray<Vector3> CombineVertex = new NativeArray<Vector3>(Cube_Vertex.Length * CombineCount, Allocator.TempJob);
        NativeArray<Vector2> CombineUV = new NativeArray<Vector2>(Cube_UV.Length * CombineCount, Allocator.TempJob);
        NativeArray<int> CombineIndex = new NativeArray<int>(Cube_Index.Length * CombineCount, Allocator.TempJob);
        Mesh_UV.CopyFrom(Cube_UV);
        Mesh_Vertex.CopyFrom(Cube_Vertex);
        Mesh_Index.CopyFrom(Cube_Index);
        CombineMeshJob combineMeshJob = new CombineMeshJob
        {
            Position = Position,
            Mesh_Vertex = Mesh_Vertex,
            Mesh_UV = Mesh_UV,
            Mesh_Triangles = Mesh_Index,
            CombineCount = CombineCount,
            CombinesVertex = CombineVertex,
            CombineUV = CombineUV,
            CombinesIndex = CombineIndex
        };
        var CombineMeshJobHandle = combineMeshJob.Schedule();
        CombineMeshJobHandle.Complete();
        newMesh.vertices = CombineVertex.ToArray();
        newMesh.triangles = CombineIndex.ToArray();
        newMesh.uv = CombineUV.ToArray();
        Position.Dispose();
        Mesh_UV.Dispose();
        Mesh_Vertex.Dispose();
        Mesh_Index.Dispose();
        CombineVertex.Dispose();
        CombineUV.Dispose();
        CombineIndex.Dispose();
        newMesh.RecalculateNormals();  // –ﬁ∏¥∑®œﬂº∆À„
        newMesh.RecalculateBounds();   // –ﬁ∏¥∞¸Œß∫–º∆À„
        return newMesh;
    }
    private void RefreshPartBlockLodLayer(PartBlockPro PartBlockPro, List<Matrix4x4> Transform, Vector3 CurPart, Vector3 PartOffect)
    {
        Vector3 PartOffect_Normal = new Vector3((PartOffect.x - 25) / 50, 0, (PartOffect.z - 25) / 50);
        var LodDistance = math.abs(PartOffect_Normal.x - CurPart.x) > math.abs(PartOffect_Normal.z - CurPart.z) ? math.abs(PartOffect_Normal.x - CurPart.x) : math.abs(PartOffect_Normal.z - CurPart.z);
        bool IsOffectX = math.abs(PartOffect_Normal.x - CurPart.x) > math.abs(PartOffect_Normal.z - CurPart.z);
        if (PartOffect_Normal.x > CurPart.x && IsOffectX || PartOffect_Normal.z >CurPart.z && !IsOffectX) LodDistance++;
        if (LodDistance <= Lod1_LayerCount && PartBlockPro.lodLayer != LodLayer.Lod_Top)
        {
            PartBlockPro.lodLayer = LodLayer.Lod_Top;
            PartBlockPro.PartMesh = VertexCombine(PartBlockPro.Count,Transform,StaticBlock_Lod_Top.Cube_Vertex,StaticBlock_Lod_Top.Cube_Index,StaticBlock_Lod_Top.Cube_UV);
        }
        else if(LodDistance > Lod1_LayerCount && LodDistance <= Lod2_LayerCount && PartBlockPro.lodLayer != LodLayer.Lod_Middle)
        {
            PartBlockPro.lodLayer = LodLayer.Lod_Middle;
            PartBlockPro.PartMesh = VertexCombine(PartBlockPro.Count, Transform, StaticBlock_Lod_Middle.Cube_Vertex, StaticBlock_Lod_Middle.Cube_Index, StaticBlock_Lod_Middle.Cube_UV);
        }
        else if(LodDistance > Lod2_LayerCount && PartBlockPro.lodLayer != LodLayer.Lod_Bottom)
        {
            PartBlockPro.lodLayer = LodLayer.Lod_Bottom;
            PartBlockPro.PartMesh = new Mesh();
        }
    }
}
[BurstCompile]
public struct CombineMeshJob : IJob
{
    public NativeArray<Vector3> Position;
    public int CombineCount;
    public NativeArray<Vector3> Mesh_Vertex;
    public NativeArray<Vector2> Mesh_UV;
    public NativeArray<int> Mesh_Triangles;
    public NativeArray<Vector3> CombinesVertex;
    public NativeArray<Vector2> CombineUV;
    public NativeArray<int> CombinesIndex;
    public void Execute()
    {
        int TotalVertices = 0;
        int TotalIndex = 0;
        for (int i = 0; i < CombineCount; i++)
        {
            for(int j = 0;j < Mesh_Vertex.Length; j++)
            {
                CombinesVertex[TotalVertices + j] = Mesh_Vertex[j] + Position[i];
                CombineUV[TotalVertices + j] = Mesh_UV[j];
            }
            for (int j = 0; j < Mesh_Triangles.Length; j++)
            {
                CombinesIndex[TotalIndex + j] = Mesh_Triangles[j] + Mesh_Vertex.Length * i;
            }
            TotalIndex += Mesh_Triangles.Length;
            TotalVertices += Mesh_Vertex.Length;
        }
    }
}
