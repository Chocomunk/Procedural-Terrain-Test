using UnityEngine;
using System.Collections;

public class Chunk {

	public int localChunkPosX = -2;
	public int localChunkPosZ = -2;
	public float globalPosX = 0;
	public float globalPosZ = 0;

	Terrain t_chunk;
	TerrainData t_data;
	NoiseModule module;

	public Chunk(Terrain t_chunk, int chunk_x, int chunk_z, int seed){
		this.t_chunk = t_chunk;
		this.t_data = t_chunk.terrainData;
		this.SetChunkPos(chunk_x,chunk_z);
		this.module = new PerlinNoise(seed);
	}

	public void Activate(){
		this.GetTerrainObject().GetComponent<Collider>().enabled = true;
	}
	public void Deactivate(){
		this.GetTerrainObject().GetComponent<Collider>().enabled = false;
	}

	public void SetHeights(float[,] terrain_heights){
		t_data.SetHeights(0,0,terrain_heights);
	}

	public void SetGlobalPos(float x_world, float z_world){
		Transform t_trans = this.GetTerrainObject().transform;
		globalPosX = x_world;
		globalPosZ = z_world;
		t_trans.position = new Vector3(
			globalPosX,
			t_trans.position.y,
			globalPosZ);
	}

	public void SetChunkPos(int chunk_x, int chunk_z){
		this.localChunkPosX = chunk_x;
		this.localChunkPosZ = chunk_z;
	}

	public Terrain GetChunk(){return t_chunk;}
	public TerrainData GetData(){return t_data;}
	public GameObject GetTerrainObject(){return t_chunk.gameObject;}
	public bool IsActive(){return this.GetTerrainObject().GetComponent<Collider>().enabled;}
}
