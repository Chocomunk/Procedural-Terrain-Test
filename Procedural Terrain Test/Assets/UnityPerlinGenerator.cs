using UnityEngine;
using System.Collections;

public class UnityPerlinGenerator : MonoBehaviour {

	public const int MAP_SIZE = 65;
	public static int dim = 3;

	public float maxHeight = 2;
	public float dimension = 200;
	[Range(-1,1)]public float hardChange = 0;
	public float tileOffsetX = 0;
	public float tileOffsetZ = 0;

	public float groundNoiseScale = 1;
	public float groundScaleBuffer = 1;
	public float groundNoiseFactor = 1;
	public float groundFrequencyDuller = 1;
	public float groundFrequencyReducer = 0;
	public float groundReduceBuffer = 1;

	public float rockNoiseScale = 1;
	public float rockScaleBuffer = 1;
	public float rockNoiseFactor = 1;
	public float rockFrequencyDuller = 1;
	public float rockFrequencyReducer = 0;
	public float rockReduceBuffer = 1;
	
	public float hillNoiseScale = 1;
	public float hillScaleBuffer = 1;
	public float hillNoiseFactor = 1;
	public float hillFrequencyDuller = 1;
	public float hillFrequencyReducer = 0;
	public float hillReduceBuffer = 1;

	public Terrain[,] Terrains = new Terrain[dim,dim];
	public Vector3 initPos;
	public float[,] heightMapTable = new float[MAP_SIZE*dim,MAP_SIZE*dim];
	
	void Start(){
		this.Generate();
	}

	void OnDrawGizmos(){
		Gizmos.color = new Color(0,0,1,0.25f);

		for(int i=0; i<MAP_SIZE*dim; i++){
			for(int j=0; j<MAP_SIZE*dim; j++){
				Gizmos.DrawCube(new Vector3(i*dimension/(MAP_SIZE*dim), (heightMapTable[i,j])*maxHeight, j*dimension/(MAP_SIZE*dim)), 
				                new Vector3(dimension/(MAP_SIZE*dim),dimension/(MAP_SIZE*dim),dimension/(MAP_SIZE*dim)));
			}
		}
	}

	public void Generate(){
		CalcTerrain();
		
		for(int i=0; i<dim; i++){
			for(int j=0; j<dim; j++){
				TerrainData terrainData = new TerrainData();
				terrainData.heightmapResolution = MAP_SIZE;
				terrainData.size = new Vector3(dimension/dim, maxHeight, dimension/dim);
				terrainData.SetHeights(0,0,generatePartitionedTable(i,j));

				GameObject terrainObj = Terrain.CreateTerrainGameObject(terrainData);
				terrainObj.transform.position = new Vector3(initPos.x+(i*((dimension/dim))),initPos.y,initPos.z+(j*((dimension/dim))));

				terrainObj.transform.parent = this.transform;
				Terrains[i,j] = terrainObj.GetComponent<Terrain>();
			}
		}

		for(int i=0; i<dim; i++){
			for(int j=0; j<dim; j++){
				calcNeighbors(i,j,Terrains[i,j]);
				Terrains[i,j].Flush();
			}
		}
	}

	public  void CalcTerrain(){
		NoiseModule noise = new PerlinNoise(1);
		for(int i=0; i<MAP_SIZE*dim; i++){
			for(int j=0; j<MAP_SIZE*dim; j++){

				float x = (MAP_SIZE*dim*initPos.x/dimension)+i;
				float z = (MAP_SIZE*dim*initPos.z/dimension)+j;

				float p1 = -1*groundFrequencyReducer/groundReduceBuffer + 
					Mathf.PerlinNoise(
						x*groundNoiseFactor/groundFrequencyDuller,z*groundNoiseFactor/groundFrequencyDuller
						)*groundNoiseScale/groundScaleBuffer;

				float p2 = 
					noise.Noise2D(
						x/(rockFrequencyDuller*MAP_SIZE),0/*z/(rockFrequencyDuller*MAP_SIZE)*/
						)/rockScaleBuffer;

				float p3 = -1*hillFrequencyReducer/hillReduceBuffer + 
					noise.Noise2D(
					 	x*hillNoiseFactor/hillFrequencyDuller,z*hillNoiseFactor/hillFrequencyDuller
						)*hillNoiseScale/hillScaleBuffer;

//				if(p1<0)p1=0;
//				if(p2<0)p2=0;
//				if(p3<0)p3=0;

				heightMapTable[j,i] = p2 -hardChange;
//				heightMapTable[i,j] = 0.1f*maxHeight;
//				Debug.Log(i+", "+j+": " + heightMapTable[i,j]);
			}
		}
	}

	float[,] generatePartitionedTable(int x_index, int y_index){
		float[,] table = new float[MAP_SIZE,MAP_SIZE];
//		for(int i=0; i<MAP_SIZE; i++){
//			for(int j=0; j<MAP_SIZE; j++){
//				table[i,j] = heightMapTable[(x_index*MAP_SIZE)+i,(y_index*MAP_SIZE)+j];
//			}
//		}
		for(int i=0; i<MAP_SIZE; i++){
			for(int j=0; j<MAP_SIZE; j++){
				table[i,j] = heightMapTable[((x_index)*MAP_SIZE)+j,((y_index)*MAP_SIZE)+i];
			}
		}
		return table;
	}

	void calcNeighbors(int x_index, int y_index, Terrain terrain){
		Terrain left = null;
		Terrain top = null;
		Terrain right = null;
		Terrain bottom = null;

		if(x_index == 0){
			right = Terrains[1,y_index];
		}else if(x_index == dim-1){
			left = Terrains[dim-1,y_index];
		}else{
			right = Terrains[x_index+1,y_index];
			left = Terrains[x_index-1,y_index];
		}
		
		if(y_index == 0){
			top = Terrains[x_index,1];
		}else if(y_index == dim-1){
			bottom = Terrains[x_index,dim-1];
		}else{
			top = Terrains[x_index,y_index+1];
			bottom = Terrains[x_index,y_index-1];
		}

		terrain.SetNeighbors(left,top,right,bottom);
//		fixNeighbors(terrain,right,top);
	}

	void fixNeighbors(Terrain currTerrain, Terrain right, Terrain top){
		float[,] newHeights = new float[MAP_SIZE, MAP_SIZE];
		float[,] rightHeights = new float[MAP_SIZE, MAP_SIZE], topHeights = new float[MAP_SIZE, MAP_SIZE];
		
		if (right != null)
			rightHeights = right.terrainData.GetHeights(0, 0, MAP_SIZE, MAP_SIZE);
		if (top != null)
			topHeights = top.terrainData.GetHeights(0, 0, MAP_SIZE, MAP_SIZE);
		
		if (right != null || top != null)
		{
			newHeights = currTerrain.terrainData.GetHeights(0, 0, MAP_SIZE, MAP_SIZE);
			
			for (int i = 0; i < MAP_SIZE; i++)
			{
				if (right != null)
					newHeights[i, MAP_SIZE - 1] = rightHeights[i, 0];
				
				if (top != null)
					newHeights[MAP_SIZE - 1, i] = topHeights[0, i];
			}
			currTerrain.terrainData.SetHeights(0, 0, newHeights);
		}
	}
}
