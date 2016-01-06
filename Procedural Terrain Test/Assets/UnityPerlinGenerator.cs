using UnityEngine;
using System.Collections;

public class UnityPerlinGenerator : MonoBehaviour {
	
	public const int MAP_SIZE = 65;
	public static int dim = 3;
	
	public float maxHeight = 3;
	public float dimension = 300;
	public float BaseDimension = 300;
	[Range(-1,1)]public float hardChange = 0;
	
	[Range(2,16)]
	public int octaves_ground = 8;
	public float frequency_ground;
	public float amplitude_ground;
	public float lacunarity_ground;
	public float persistence_ground;
	
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
				Gizmos.DrawCube(new Vector3((j-MAP_SIZE*1.5f)*dimension/(MAP_SIZE*dim), (heightMapTable[i,j])*maxHeight, (i-MAP_SIZE*1.5f)*dimension/(MAP_SIZE*dim)), 
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
				terrainObj.transform.position = new Vector3(initPos.x+((i-1.5f)*dimension/dim),initPos.y,initPos.z+((j-1.5f)*dimension/dim));
				
				terrainObj.transform.parent = this.transform;
				Terrains[i,j] = terrainObj.GetComponent<Terrain>();
			}
		}
		
		for(int i=0; i<dim; i++){
			for(int j=0; j<dim; j++){
				calcNeighbors(j,i,Terrains[i,j]);
				Terrains[i,j].Flush();
			}
		}
	}
	
	public  void CalcTerrain(){
		NoiseModule noise = new PerlinNoise(1);
		for(int i=0; i<MAP_SIZE*dim; i++){
			float z = (initPos.z-(BaseDimension*1.5f/dim)) + i*BaseDimension/(MAP_SIZE*dim);
			for(int j=0; j<MAP_SIZE*dim; j++){
				
				float x = (initPos.x-(BaseDimension*1.5f/dim)) + j*BaseDimension/(MAP_SIZE*dim);
				float p2 = noise.FractalNoise2D(x,z,octaves_ground,frequency_ground*(dimension/BaseDimension),amplitude_ground,lacunarity_ground,persistence_ground);
				
				heightMapTable[i,j] = 0.5f + p2;
			}
		}
	}
	
	float[,] generatePartitionedTable(int x_index, int y_index){
		float[,] table = new float[MAP_SIZE,MAP_SIZE];
		for(int i=0; i<MAP_SIZE; i++){
			for(int j=0; j<MAP_SIZE; j++){
				table[i,j] = heightMapTable[((y_index)*MAP_SIZE)+i,((x_index)*MAP_SIZE)+j];
			}
		}
		return table;
	}
	
	void calcNeighbors(int x_index, int y_index, Terrain terrain){
		Terrain left = null;
		Terrain top = null;
		Terrain right = null;
		Terrain bottom = null;
		
		if (dim>1) {
			if (x_index == 0) {
				right = Terrains [1, y_index];
			} else if (x_index == dim - 1) {
				left = Terrains [dim - 1, y_index];
			} else {
				right = Terrains [x_index + 1, y_index];
				left = Terrains [x_index - 1, y_index];
			}
			
			if (y_index == 0) {
				top = Terrains [x_index, 1];
			} else if (y_index == dim - 1) {
				bottom = Terrains [x_index, dim - 1];
			} else {
				top = Terrains [x_index, y_index + 1];
				bottom = Terrains [x_index, y_index - 1];
			}
		}
		
		terrain.SetNeighbors(left,top,right,bottom);
//		fixNeighbors(terrain,right,bottom);
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
