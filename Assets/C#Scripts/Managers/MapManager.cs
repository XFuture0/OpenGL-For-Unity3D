using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GenPerlinNoiseMap;

public class MapManager : SingleTons<MapManager>
{
    public GenPerlinNoiseMap genPerlinNoiseMap;
    protected override void Awake()
    {
        base.Awake();
    }
    public List<Matrix4x4> GetCurPartBlocks()
    {
        return genPerlinNoiseMap.GetCurPartBlocks();
    }
    public void BreakCurBlocks(Matrix4x4 BlockMatrix)
    {
        genPerlinNoiseMap.BreakCurBlocks(BlockMatrix);
    }
}
