using System;
using UnityEngine;
using System.Collections;
using System.Threading;

public class TerrainPatch : IPatch
{
	private NoiseModule m_mountainNoise = new ValueNoise(1);
	private NoiseModule m_plainsNoise = new PerlinNoise(1);
	
	public TerrainPatch(float globTileX_i, float globTileZ_i, Chunk chunk_i, float h0_i, float h1_i)
	{
		globalTileX = globTileX_i;
		globalTileZ = globTileZ_i;
		chunk = chunk_i;
		h0 = h0_i;
		h1 = h1_i;
	}
	
	private float globalTileX, globalTileZ, h0, h1;
	private Chunk chunk;
	
	public void Execute()
	{
//		FillTerrainPatch();
//		if (h1 == InfiniteTerrain.m_heightMapSize)
//		{
//			terrain.terrainData.SetHeights(0, 0, InfiniteTerrain.m_terrainHeights);  //SetHeights calculates terrain collider
//			terrain.transform.position = pos;
//		}

		int resolution = ChunkManager.MAP_SIZE;
		float[,] terrain_map = new float[resolution,resolution];

		for(int i=0; i<resolution; i++){
			float z = 300*(globalTileZ+i)/resolution;
			for(int j=0; j<resolution; j++){
				float x = 300*(globalTileX+j)/resolution;

				terrain_map[i,j] = m_mountainNoise.FractalNoise2D(x,z,8,0.001f,0.1f,2,0.5f);
			}
		}

		chunk.SetHeights(terrain_map);
	}
	
//	private void FillTerrainPatch()
//	{
//		int hRes = InfiniteTerrain.m_heightMapSize;
//		float ratio = (float)InfiniteTerrain.m_landScapeSize / (float)hRes;
//		
//		float z0 = (InfiniteLandscape.initialGlobalIndex * (InfiniteTerrain.m_heightMapSize - 1)) * ratio;
//		float z1 = (InfiniteLandscape.initialGlobalIndex * (InfiniteTerrain.m_heightMapSize - 1)) * ratio + hRes * ratio;
//		float y0 = 0.0f;
//		float y1 = 1.0f;
//		
//		for (int z = h0; z < h1; z++)
//		{
//			float worldPosZ = (z + globalTileZ * (InfiniteTerrain.m_heightMapSize - 1)) * ratio;
//			float hx = Mathf.Clamp((y1 - y0) / (z1 - z0) * (worldPosZ - z0) + y1, -4, 8);
//			
//			for (int x = 0; x < hRes; x++)
//			{
//				float worldPosX = (x + globalTileX * (InfiniteTerrain.m_heightMapSize - 1)) * ratio;
//				float mountainsPerlin = m_mountainNoise.FractalNoise2D(worldPosX, worldPosZ, 4, 3000, 0.4f);
//				float mountainsRidged = m_mountainNoiseRidged.FractalNoise2D(worldPosX, worldPosZ, 4, 3000, 0.2f);
//				float height = (mountainsRidged + mountainsPerlin) + 0.03f * hx;
//				InfiniteTerrain.m_terrainHeights[z, x] = height;
//			}
//		}
//	}
}
