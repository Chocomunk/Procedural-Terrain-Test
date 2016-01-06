using UnityEngine;
using System.Collections;

public class TerrainManager : MonoBehaviour {

	public GameObject player;

	public int MAP_SIZE = 65;
	public int dim = 3;
	
	public float maxHeight = 1000;
	public float dimension = 300;
	public float baseDimension = 300;

	[HideInInspector]public int playerChunkX = 1;
	[HideInInspector]public int playerChunkZ = 1;

	public static int initialChunkIndex = 0;

	protected int prevChunkIndexX = -1;
	protected int prevChunkIndexZ = -1;
	protected int curChunkIndexX = initialChunkIndex + 1;
	protected int curChunkIndexZ = initialChunkIndex + 1;
	
	protected int prevLocalIndexX = -1;
	protected int prevLocalIndexZ = -1;
	protected int curLocalIndexX = 1;
	protected int curLocalIndexZ = 1;
	
	protected int prevCyclicIndexX = -1;
	protected int prevCyclicIndexZ = -1;
	protected int curCyclicIndexX = 1;
	protected int curCyclicIndexZ = 1;
	
	protected bool patchIsFilling = false;
	protected bool updateLandscape = false;

	protected bool UpdateIndexes()
	{
		int currentLocalIndexX = Mathf.CeilToInt(player.transform.position.x/dimension);
		int currentLocalIndexZ = Mathf.CeilToInt(player.transform.position.z/dimension);
		
		if (curLocalIndexX != currentLocalIndexX || curLocalIndexZ != currentLocalIndexZ)
		{
			prevLocalIndexX = curLocalIndexX;
			curLocalIndexX = currentLocalIndexX;
			prevLocalIndexZ = curLocalIndexZ;
			curLocalIndexZ = currentLocalIndexZ;
			
			int dx = curLocalIndexX - prevLocalIndexX;
			int dz = curLocalIndexZ - prevLocalIndexZ;
			prevChunkIndexX = curChunkIndexX;
			curChunkIndexX += dx;
			prevChunkIndexZ = curChunkIndexZ;
			curChunkIndexZ += dz;
			
			prevCyclicIndexX = curCyclicIndexX;
			curCyclicIndexX = curChunkIndexX % dim;
			prevCyclicIndexZ = curCyclicIndexZ;
			curCyclicIndexZ = curChunkIndexZ % dim;
			
			return true;
		}
		else return false;
	}

	protected void Update()
	{
		if (UpdateIndexes())
			updateLandscape = true;
		else
			updateLandscape = false;
	}
}
