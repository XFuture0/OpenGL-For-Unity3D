using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
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
            var mesh = VertexCombine(part.Key.Count, part.Value,new Vector2(0,0));
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
    private Mesh VertexCombine(int CombineCount, List<Matrix4x4> Transform,Vector2 CombinePart)
    {
        Mesh newMesh = new Mesh();
        newMesh.indexFormat = IndexFormat.UInt32;
        NativeArray<Vector3> Position = new NativeArray<Vector3>(CombineCount, Allocator.TempJob);
        for(int i = 0;i < Transform.Count; i++)
        {
            Position[i] = Transform[i].GetPosition();
        }
        NativeArray<Vector2> Mesh_UV = new NativeArray<Vector2>(StaticBlock.Cube_UV.Length, Allocator.TempJob);
        NativeArray<Vector3> Mesh_Vertex = new NativeArray<Vector3>(StaticBlock.Cube_Vertex.Length, Allocator.TempJob);
        NativeArray<int> Mesh_Index = new NativeArray<int>(StaticBlock.Cube_Index.Length, Allocator.TempJob);
        Mesh_UV.CopyFrom(StaticBlock.Cube_UV);
        Mesh_Vertex.CopyFrom(StaticBlock.Cube_Vertex);
        Mesh_Index.CopyFrom(StaticBlock.Cube_Index);
        NativeArray<Vector3> CombineVertex = new NativeArray<Vector3>(mesh.vertexCount * CombineCount, Allocator.TempJob);
        NativeArray<Vector2> CombineUV = new NativeArray<Vector2>(mesh.uv.Length * CombineCount, Allocator.TempJob);
        NativeArray<int> CombineIndex = new NativeArray<int>(mesh.triangles.Length * CombineCount, Allocator.TempJob);
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
       /* List<int> VisibleTriangles = new List<int>();
        for (int i = 0;i < newMesh.triangles.Length; i+= 3)
        {
            var Vertex1 = newMesh.vertices[newMesh.triangles[i]];
            var Vertex2 = newMesh.vertices[newMesh.triangles[i + 1]];
            var Vertex3 = newMesh.vertices[newMesh.triangles[i + 2]];
            bool IsVisible1 = VertexDeepTest(Vertex1,CombinePart);
            bool IsVisible2 = VertexDeepTest(Vertex2, CombinePart);
            bool IsVisible3 = VertexDeepTest(Vertex3, CombinePart);
            if (IsVisible1 && IsVisible2 && IsVisible3)
            {
                VisibleTriangles.Add(newMesh.triangles[i]);
                VisibleTriangles.Add(newMesh.triangles[i + 1]);
                VisibleTriangles.Add(newMesh.triangles[i + 2]);
            }
        }
        newMesh.triangles = VisibleTriangles.ToArray();
        newMesh.RecalculateNormals();  // –ﬁ∏¥∑®œﬂº∆À„
        newMesh.RecalculateBounds();   // –ﬁ∏¥∞¸Œß∫–º∆À„*/
        return newMesh;
    }
    
    private bool VertexDeepTest(Vector3 Vertex,Vector2 CombinePart)
    {
        var GroundHigh1 = (int)(Mathf.PerlinNoise((50 * CombinePart.x + Vertex.x) * scale, (50 * CombinePart.y + Vertex.z) * scale) * 10);
        var GroundHigh2 = (int)(Mathf.PerlinNoise((50 * CombinePart.x + Vertex.x - 1) * scale, (50 * CombinePart.y + Vertex.z) * scale) * 10);
        var GroundHigh3 = (int)(Mathf.PerlinNoise((50 * CombinePart.x + Vertex.x + 1) * scale, (50 * CombinePart.y + Vertex.z) * scale) * 10);
        var GroundHigh4 = (int)(Mathf.PerlinNoise((50 * CombinePart.x + Vertex.x) * scale, (50 * CombinePart.y + Vertex.z - 1) * scale) * 10);
        var GroundHigh5 = (int)(Mathf.PerlinNoise((50 * CombinePart.x + Vertex.x) * scale, (50 * CombinePart.y + Vertex.z + 1) * scale) * 10);
        var GroundHighMin = Mathf.Min(GroundHigh1, GroundHigh2, GroundHigh3, GroundHigh4, GroundHigh5);
        return Vertex.y >= GroundHighMin - 1;
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
