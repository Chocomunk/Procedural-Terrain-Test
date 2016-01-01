using UnityEngine;
using System.Collections;
using System;

public class NoiseModule {
	
	protected const int hashMask = 256;		//Size of permutation, must be power of two. Larger value gives more hash
	protected int[] hash_perm = new int[hashMask + hashMask];				//Hash permutation to pull noise info from
		
	//Noise methods to be implemented down the line
	public virtual float Noise1D(float x){return -1;}
	public virtual float Noise2D(float x, float y){return -1;}
	public virtual float Noise3D(float x, float y, float z){return -1;}

	/// <summary>
	/// Calculates Fractal Noise in 1 dimension using Perlin Noise
	/// </summary>
	/// <returns>The noise value</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="octave">Octave.</param>
	/// <param name="frequency">Frequency.</param>
	/// <param name="amplitude">Amplitude.</param>
	/// <param name="lacunarity">Lacunarity.</param>
	/// <param name="persistence">Persistence.</param>
	public float FractalNoise1D(float x, int octave, float frequency, float amplitude, float lacunarity, float persistence){
		float sum = 0;
		for(int i=0; i<octave; i++){
			sum+=Noise1D(x*frequency)*amplitude;
			frequency*=lacunarity;
			amplitude*=persistence;
		}
		return sum;
	}
	
	/// <summary>
	/// Calculates Fractal Noise in 2 dimensions using Perlin Noise
	/// </summary>
	/// <returns>The noise value</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="octave">Octave.</param>
	/// <param name="frequency">Frequency.</param>
	/// <param name="amplitude">Amplitude.</param>
	/// <param name="lacunarity">Lacunarity.</param>
	/// <param name="persistence">Persistence.</param>
	public float FractalNoise2D(float x, float y, int octave, float frequency, float amplitude, float lacunarity, float persistence){
		float sum = 0;
		float maxValue = 0;
		for(int i=0; i<octave; i++){
			sum+=Noise2D(x*frequency,y*frequency)*amplitude;
			frequency*=lacunarity;
			amplitude*=persistence;
			maxValue+=amplitude;
		}
		return sum;
	}
	
	/// <summary>
	/// Calculates Fractal Noise in 3 dimensions using Perlin Noise
	/// </summary>
	/// <returns>The noise value</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	/// <param name="octave">Octave.</param>
	/// <param name="frequency">Frequency.</param>
	/// <param name="amplitude">Amplitude.</param>
	/// <param name="lacunarity">Lacunarity.</param>
	/// <param name="persistence">Persistence.</param>
	public float FractalNoise3D(float x, float y, float z, int octave, float frequency, float amplitude, float lacunarity, float persistence){
		float sum = 0;
		float maxValue = 0;
		for(int i=0; i<octave; i++){
			sum+=Noise3D(x*frequency,y*frequency,z*frequency)*amplitude;
			frequency*=lacunarity;
			amplitude*=persistence;
			maxValue+=amplitude;
		}
		return sum/maxValue;
	}

	/// <summary>
	/// Creates a Noise generating object from a seed
	/// </summary>
	/// <param name="seed">Seed.</param>
	public NoiseModule(int seed){
		UnityEngine.Random.seed = seed;

		/* Defines variables to:
		 * - Store an index value of the permutation
		 * - Store a value to swap values in the permutation
		 * - Define a temporary storage variable
		 */
		int index,swap,temp;

		//Fill the hash table with integers 0-{hashMask-1}
		for(index=0; index<hashMask; index++)
			hash_perm[index] = index;

		//Generate random indexes and swap values at the given indexes
		while(--index!=0){
			swap = UnityEngine.Random.Range(0, hashMask);
			temp = hash_perm[index];
			hash_perm[index] = hash_perm[swap];
			hash_perm[swap] = temp;
		}

		//Repeat hash table for overflow issues
		for(index=0; index<hashMask; index++)
			hash_perm[hashMask + index] = hash_perm [index];
	}

	/// <summary>
	/// Calculates 1 dimensional noise
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="i0">Integer part of 0 position noise</param>
	/// <param name="i1">Integer part of 1 position noise</param>
	/// <param name="f0">Fractional part of 0 position noise</param>
	/// <param name="f1">Fractional part of 0 position noise</param>
	public void HashNoise1D (float x, out int i0, out int i1, out float f0, out float f1){
		i0 = (int)Mathf.Floor(x);		//Integer value of x
		f0 = x - i0;					//Fractional value of x
		f1 = f0 - 1f;					//Fractional value of x 1 coordinate away
		i1 = (i0 + 1) & (hashMask-1);	//Integer value of x 1 coordinate away, then hashed
		i0 = i0 & (hashMask-1);			//Hashed value of i0 (modulus hash mask)
	}

	/// <summary>
	/// Calculates 2 dimensional noise
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="ix0">Integer part of 0 position noise at x</param>
	/// <param name="ix1">Integer part of 1 position noise at x</param>
	/// <param name="fx0">Fractional part of 0 position noise at x</param>
	/// <param name="fx1">Fractional part of 0 position noise at x</param>
	/// <param name="iy0">Integer part of 0 position noise at y</param>
	/// <param name="iy1">Integer part of 1 position noise at y</param>
	/// <param name="fy0">Fractional part of 0 position noise at y</param>
	/// <param name="fy1">Fractional part of 0 position noise at y</param>
	public void HashNoise2D (float x, float y, out int ix0, out int ix1, out float fx0, out float fx1, out int iy0, out int iy1, out float fy0, out float fy1){
		HashNoise1D(x, out ix0, out ix1, out fx0, out fx1);
		HashNoise1D(y, out iy0, out iy1, out fy0, out fy1);

//		ix0 = (int)Mathf.Floor(x);		//Integer value of x
//		iy0 = (int)Mathf.Floor(y);		//Integer value of y
//
//		fx0 = x - ix0;					//Fractional value of x
//		fy0 = y - iy0;					//Fractional value of y
//
//		fx1 = fx0 - 1f;					//Fractional value of x 1 coordinate away
//		fy1 = fy0 - 1f;					//Fractional value of y 1 coordinate away
//
//		ix1 = (ix0 + 1) & (hashMask-1);	//Integer value of x 1 coordinate away, then hashed
//		iy1 = (iy0 + 1) & (hashMask-1);	//Integer value of y 1 coordinate away, then hashed
//
//		ix0 = ix0 & (hashMask-1);		//Hashed value of ix0 (modulus hash mask)
//		iy0 = iy0 & (hashMask-1);		//Hashed value of iy0 (modulus hash mask)
	}

	/// <summary>
	/// Calculates 3 dimensional noise
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	/// <param name="ix0">Integer part of 0 position noise at x</param>
	/// <param name="ix1">Integer part of 1 position noise at x</param>
	/// <param name="fx0">Fractional part of 0 position noise at x</param>
	/// <param name="fx1">Fractional part of 0 position noise at x</param>
	/// <param name="iy0">Integer part of 0 position noise at y</param>
	/// <param name="iy1">Integer part of 1 position noise at y</param>
	/// <param name="fy0">Fractional part of 0 position noise at y</param>
	/// <param name="fy1">Fractional part of 0 position noise at y</param>
	/// <param name="iz0">Integer part of 0 position noise at z</param>
	/// <param name="iz1">Integer part of 1 position noise at z</param>
	/// <param name="fz0">Fractional part of 0 position noise at z</param>
	/// <param name="fz1">Fractional part of 0 position noise at z</param>
	public void HashNoise3D (float x, float y, float z, out int ix0, out int ix1, out float fx0, out float fx1, out int iy0, out int iy1, out float fy0, out float fy1, out int iz0, out int iz1, out float fz0, out float fz1){
		HashNoise1D(x, out ix0, out ix1, out fx0, out fx1);
		HashNoise1D(y, out iy0, out iy1, out fy0, out fy1);
		HashNoise1D(z, out iz0, out iz1, out fz0, out fz1);

//		ix0 = (int)Mathf.Floor(x);		//Integer value of x
//		iy0 = (int)Mathf.Floor(y);		//Integer value of y
//		iz0 = (int)Mathf.Floor(z);		//Integer value of z
//
//		fx0 = x - ix0;					//Fractional value of x
//		fy0 = y - iy0;					//Fractional value of y
//		fz0 = z - iz0;					//Fractional value of z
//
//		fx1 = fx0 - 1f;					//Fractional value of x 1 coordinate away
//		fy1 = fy0 - 1f;					//Fractional value of y 1 coordinate away
//		fz1 = fz0 - 1f;					//Fractional value of z 1 coordinate away
//
//		ix1 = (ix0 + 1) & (hashMask-1);	//Integer value of x 1 coordinate away, then hashed
//		iy1 = (iy0 + 1) & (hashMask-1);	//Integer value of y 1 coordinate away, then hashed
//		iz1 = (iz0 + 1) & (hashMask-1);	//Integer value of z 1 coordinate away, then hashed
//
//		ix0 = ix0 & (hashMask-1);		//Hashed value of ix0 (modulus hash mask)
//		iy0 = iy0 & (hashMask-1);		//Hashed value of iy0 (modulus hash mask)
//		iz0 = iz0 & (hashMask-1);		//Hashed value of iz0 (modulus hash mask)
	}
}
