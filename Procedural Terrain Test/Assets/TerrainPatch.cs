using System;
using UnityEngine;
using System.Collections;
using System.Threading;

public class TerrainPatch : IPatch
{
	private NoiseModule m_mountainNoise = new PerlinNoise(1);
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
		int resolution = ChunkManager.MAP_SIZE;
		float[,] terrain_map = new float[resolution,resolution];

		for(int i=0; i<resolution; i++){
			float z = (globalTileZ-1) + i*100/resolution;
			for(int j=0; j<resolution; j++){
				float x = (globalTileX-1) + j*100/resolution;

				terrain_map[i,j] = m_mountainNoise.FractalNoise2D(x,z,8,0.001f,0.1f,2,0.5f);
			}
		}
		Debug.Log(globalTileX+", "+globalTileZ);
		chunk.SetHeights(terrain_map);
	}
}
