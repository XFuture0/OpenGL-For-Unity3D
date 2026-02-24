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
    public void BreakBlocks(Matrix4x4 BlockMatrix)
    {
        genPerlinNoiseMap.BreakBlocks(BlockMatrix);
    }
    public void CreateBlocks(Matrix4x4 BlockMatrix)
    {
        genPerlinNoiseMap.CreateBlocks(BlockMatrix);
    }
}
