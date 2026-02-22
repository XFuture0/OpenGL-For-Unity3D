using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BlockGraphicsRayCastHit
{
    public float Distance = 999;
    public Matrix4x4 Position;
}
public static class GraphicsRayCast
{
    public static bool TryBlockGraphicsRayCast(Ray ray, List<Matrix4x4> Vertexs, out BlockGraphicsRayCastHit hit)
    {
        hit = new BlockGraphicsRayCastHit();
        float[] CastDistances = new float[100];
        int[] CastIndexs = new int[100];
        float Curdistance = 0;
        int CastCount = 0;
        for(int i =0; i < Vertexs.Count; i++)
        {
            hit.Position = Vertexs[i];
            Bounds bound = new Bounds(hit.Position.GetPosition(), new Vector3(1.0f, 1.0f, 1.0f));
            if(bound.IntersectRay(ray,out Curdistance))
            {
                CastDistances[CastCount] = Curdistance;
                CastIndexs[CastCount] = i;
                CastCount++;
            }
        }
        int CastMin = int.MaxValue;
        for(int i = 0; i < CastCount; i++)
        {
            if(CastDistances[i] < hit.Distance)
            {
                hit.Distance = CastDistances[i];
                CastMin = CastIndexs[i];
            }
        }
        if(CastMin != 999)
        {
            hit.Position = Vertexs[CastMin];
            return true;
        }
        return false;
    }
}
