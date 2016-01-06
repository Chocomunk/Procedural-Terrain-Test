using UnityEngine;
using System.Collections;

public class ChunkManager : TerrainManager {

	public static ChunkManager manager;

	public Vector3 centerPos = new Vector3(0,0,0);
	public Chunk[,] chunks;

	[Range(2,16)]
	public int octaves_ground = 8;
	public float frequency_ground;
	public float amplitude_ground;
	public float lacunarity_ground;
	public float persistence_ground;

	public void Awake(){
		if (manager == null) {
			manager = this;
		} else if (manager != this) {
			DestroyImmediate (this.gameObject);
		}

		chunks = new Chunk[manager.dim, manager.dim];

		for(int i=0; i<dim; i++){
			for(int j=0; j<dim; j++){
				TerrainData t_data = new TerrainData();
				t_data.heightmapResolution = MAP_SIZE;
				t_data.size = new Vector3(dimension, maxHeight, dimension);
				chunks[i,j] = new Chunk(Terrain.CreateTerrainGameObject(t_data).GetComponent<Terrain>(),i,j,1);
			}
		}

		Chunk centerChunk = chunks[1,1];
		centerChunk.SetGlobalPos (centerChunk.globalPosX + centerPos.x - dimension/2,
		                          centerChunk.globalPosZ + centerPos.y - dimension/2);

		float ratio = baseDimension / dimension;

		for(int i=0; i<dim; i++){
			for(int j=0; j<dim; j++){
				Chunk thisChunk = chunks[i,j];

				float x_global = centerChunk.globalPosX + (i-1)*dimension;
				float z_global = centerChunk.globalPosZ + (j-1)*dimension;

				thisChunk.GetChunk().transform.parent = this.transform;

				thisChunk.SetGlobalPos(x_global,z_global);

				thisChunk.Deactivate();
				thisChunk.GetChunk().basemapDistance = 4000;
				thisChunk.GetChunk().castShadows = false;

				PatchManager.AddTerrainInfo(x_global*ratio,z_global*ratio,thisChunk);
			}
		}

		PatchManager.QueuePatches();

		int patchCount = PatchManager.patchQueue.Count;
		for(int i = 0; i < patchCount; i++)
			PatchManager.patchQueue.Dequeue().Execute();

		UpdateIndexes();
		UpdateNeighbors();

		StartCoroutine(FlushChunks());

		chunks[curCyclicIndexX,curCyclicIndexZ].Activate();
	}

	void UpdateNeighbors(){
		for(int i=0; i<dim; i++){
			for(int j=0; j<dim; j++){
				calcNeighbors(i,j,chunks[i,j].GetChunk());
			}
		}
	}

	IEnumerator FlushChunks()
	{
		for (int i = 0; i < dim; i++)
			for (int j = 0; j < dim; j++)
		{
			chunks[i, j].GetChunk().Flush();
			yield return new WaitForEndOfFrame();
		}
	}

	void calcNeighbors(int x_index, int y_index, Terrain terrain){
		Terrain left = null;
		Terrain top = null;
		Terrain right = null;
		Terrain bottom = null;
		
		if(x_index == 0){
			right = chunks[1,y_index].GetChunk();
		}else if(x_index == dim-1){
			left = chunks[dim-1,y_index].GetChunk();
		}else{
			right = chunks[x_index+1,y_index].GetChunk();
			left = chunks[x_index-1,y_index].GetChunk();
		}
		
		if(y_index == 0){
			top = chunks[x_index,1].GetChunk();
		}else if(y_index == dim-1){
			bottom = chunks[x_index,dim-1].GetChunk();
		}else{
			top = chunks[x_index,y_index+1].GetChunk();
			bottom = chunks[x_index,y_index-1].GetChunk();
		}
		
		terrain.SetNeighbors(left,top,right,bottom);
	}
}
