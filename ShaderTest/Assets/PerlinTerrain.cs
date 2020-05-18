using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinTerrain : MonoBehaviour {

    struct MetaData {
        public Vector3 groupDim;
        public uint octave;
        public uint hashSize;
        public float amplitude;
        public float frequency;
        public float persistence;
        public float lacunarity;
    }

    public int seed = 0;

    public Vector3 groupDim = new Vector3(8,8,1);
    public uint octave = 8;
    public uint hashSize = 256;
    public float amplitude = 0.5f;
    public float frequency = 2;
    public float persistence = 0.5f;
    public float lacunarity = 2;

    public Vector3 centerPos = new Vector3(0, 0, 0);
    public float MaxHeight = 3;

    public ComputeShader shader;
    public Terrain terrain;

    // Use this for initialization
    void Start() {
        PerlinCompute();
    }

    void PerlinCompute() {
        int kernel = shader.FindKernel("PerlinNoise2D");

        int planeSize = 256 * 256;
        float[] heightmap = new float[planeSize];

        ComputeBuffer meta_buffer = new ComputeBuffer(1, sizeof(uint) * 2 + sizeof(float) * (4 + 3));
        ComputeBuffer data_buffer = new ComputeBuffer(planeSize, sizeof(float) * 3);
        ComputeBuffer hash_buffer = new ComputeBuffer((int)hashSize * 2, sizeof(uint));
        ComputeBuffer out_buffer = new ComputeBuffer(planeSize, sizeof(float));

        meta_buffer.SetData(getMetaData());
        data_buffer.SetData(generateVertices(centerPos, 256));
        hash_buffer.SetData(getHashTable());

        shader.SetBuffer(kernel, "m_data", meta_buffer);
        shader.SetBuffer(kernel, "input_coords", data_buffer);
        shader.SetBuffer(kernel, "hash_table", hash_buffer);
        shader.SetBuffer(kernel, "output_map", out_buffer);

        shader.Dispatch(kernel, (int)groupDim.x, (int)groupDim.y, (int)(groupDim.z * octave));
        out_buffer.GetData(heightmap);

        meta_buffer.Release();
        data_buffer.Release();
        hash_buffer.Release();
        out_buffer.Release();

        generateTerrain(reshapeSquare(heightmap, 256), 100);
    }

    float[,] reshapeSquare(float[] inmap, int dim) {
        float[,] heightmap = new float[dim, dim];
        for(int i=0; i<inmap.Length; i++) {
            heightmap[i / dim, i % dim] = inmap[i];
            if (i % 16 == 0) {
                string outp = "";
                //if(inmap[i] != 0) {
                //    outp += (i / dim) + ", " + (i % dim) + ": ";
                //}
                Debug.Log(outp + inmap[i]);
            }
        }
        return heightmap;
    }

    void generateTerrain(float[,] heightmap, int side) {
        TerrainData t_data = new TerrainData();
        t_data.heightmapResolution = 256;
        t_data.size = new Vector3(side, MaxHeight, side);
        t_data.SetHeights(0, 0, heightmap);

        GameObject terrainObj = Terrain.CreateTerrainGameObject(t_data);
        terrainObj.transform.parent = this.transform;
        terrainObj.transform.position = centerPos - new Vector3(side/2, -MaxHeight/2, side/2);
        terrain = terrainObj.GetComponent<Terrain>();
        Debug.Log(terrainObj.transform.localPosition);
    }
    Vector3[] generateVertices(Vector3 center, int dim) {
        int half = dim / 2;
        Vector3[] list = new Vector3[dim*dim];

        for(int i=0; i<dim; i++) {
            for(int j=0; j<dim; j++) {
                list[i * dim + j] = new Vector3(j - half + center.x, 0, i - half + center.z);
            }
        }
        return list;
    }

    MetaData[] getMetaData() {
        MetaData data = new MetaData();
        data.groupDim = groupDim;
        data.groupDim.z = groupDim.z * octave;
        data.octave = octave;
        data.hashSize = hashSize;
        data.amplitude = amplitude;
        data.frequency = frequency;
        data.persistence = persistence;
        data.lacunarity = lacunarity;
        return new MetaData[] { data };
    }

    void PerlinUnlitShaderSet () {
        Material material = this.GetComponent<Renderer>().sharedMaterial;
        if (material != null) {
            UnityEngine.Random.seed = seed;

            //float[] hashtable = getHashTable();

            //material.SetInt("_HashSize", hashSize);
            //material.SetFloatArray("_HashTable", hashtable);

            //string outp = "";
            //foreach(float f in hashtable) {
                //outp += f + ", ";
            //}
            //Debug.Log(outp);
        }
	}

    int[] getHashTable() {
        int index, swap;
        int temp;
        int[] hashtable = new int[hashSize + hashSize];

        for(index=0; index<hashSize; index++) {
            hashtable[index] = index;
        }

        while (--index > 0) {
            swap = UnityEngine.Random.Range(0, (int)hashSize);
            temp = hashtable[index];
            hashtable[index] = hashtable[swap];
            hashtable[swap] = temp;
        }

        for(index=0; index<hashSize; index++) {
            hashtable[hashSize + index] = hashtable[index];
        }

        return hashtable;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
