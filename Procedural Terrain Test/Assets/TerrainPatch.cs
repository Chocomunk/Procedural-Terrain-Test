using System;
using UnityEngine;
using System.Collections;
using System.Threading;

public class TerrainPatch : IPatch
{
	private NoiseModule m_mountainNoise;
	private NoiseModule m_plainsNoise;
	
	public TerrainPatch(float globTileX_i, float globTileZ_i, Chunk chunk_i, float h0_i, float h1_i)
	{
		globalTileX = globTileX_i;
		globalTileZ = globTileZ_i;
		chunk = chunk_i;
		h0 = h0_i;
		h1 = h1_i;

		m_mountainNoise = new PerlinNoise (chunk.seed);
		m_plainsNoise = new PerlinNoise (chunk.seed);
	}
	
	private float globalTileX, globalTileZ, h0, h1;
	private Chunk chunk;
	
	public void Execute()
	{
		ChunkManager m = ChunkManager.manager;

		int resolution = ChunkManager.manager.MAP_SIZE;
		float size = ChunkManager.manager.baseDimension;
		float ratio = size / resolution;
		float sizeRatio = ChunkManager.manager.dimension/size;

		float[,] terrain_map = new float[resolution,resolution];

		for(int i=0; i<resolution; i++){
			float z = globalTileZ + i*ratio;
			for(int j=0; j<resolution; j++){
				float x = globalTileX + j*ratio;

				terrain_map[i,j] = m_mountainNoise.FractalNoise2D(x,z,m.octaves_ground,m.frequency_ground*sizeRatio,m.amplitude_ground,m.lacunarity_ground,m.persistence_ground) + 0.5f;
			}
		}
		chunk.SetHeights(terrain_map);
	}
}
