using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatchManager {

	private static int terrainPatchRes = 96;

	public static Queue<IPatch> patchQueue = new Queue<IPatch>();
	private static List<TerrainInfo> patchList = new List<TerrainInfo>();

	private class TerrainInfo
	{
		public TerrainInfo(float globX, float globZ, Chunk chunk)
		{
			globalX = globX;
			globalZ = globZ;
			this.chunk = chunk;
		}

		public float globalX;
		public float globalZ;
		public Chunk chunk;
	}

	public static void AddTerrainInfo(float global_x, float global_z, Chunk chunk){
		patchList.Add(new TerrainInfo(global_x, global_z, chunk));
	}

	public static void QueuePatches(){
		foreach (TerrainInfo info in patchList){
			for (int i = 0; i < terrainPatchRes; i++)
				patchQueue.Enqueue(new TerrainPatch(info.globalX, info.globalZ, info.chunk, ChunkManager.MAP_SIZE * i / terrainPatchRes, ChunkManager.MAP_SIZE * (i + 1) / terrainPatchRes));
		}
	}
}
