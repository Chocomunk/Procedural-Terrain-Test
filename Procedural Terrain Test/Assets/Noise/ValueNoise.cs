using UnityEngine;
using System.Collections;

public class ValueNoise : NoiseModule {

	//Pass the seed down
	public ValueNoise(int seed) : base(seed){}

	/// <summary>
	/// Calculates Value noise in 1 dimension
	/// </summary>
	/// <returns>Noise value</returns>
	/// <param name="x">The x coordinate.</param>
	public override float Noise1D(float x){
		int i0=0,i1=0;	float t=0;
		float tf=0;		//Trash Variables
		HashNoise1D(x,out i0,out i1,out t,out tf);

		int h0 = hash_perm[i0];								//Hash values to interpolate between
		int h1 = hash_perm[i1];

		NoiseUtil.PerlinFade(t);
		return  NoiseUtil.LERP(h0,h1,t) * (1f/(hashMask-1));	//Ratio of the hash to the maximum value (hashMask-1)
	}

	/// <summary>
	/// Calculates Value noise in 2 dimensions
	/// </summary>
	/// <returns>Noise value</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public override float Noise2D(float x, float y){
		int ix0=0,iy0=0,ix1=0,iy1=0;
		float tx=0,ty=0;
		float tf=0;		//Trash Variables
		HashNoise2D(x,y,out ix0,out ix1,out tx,out tf,out iy0,out iy1,out ty,out tf);

		int h0 = hash_perm[ix0];								//Hash values to interpolate between
		int h1 = hash_perm[ix1];
		int h00 = hash_perm[h0 + iy0];
		int h10 = hash_perm[h1 + iy0];
		int h01 = hash_perm[h0 + iy1];
		int h11 = hash_perm[h1 + iy1];

		NoiseUtil.PerlinFade(tx);
		NoiseUtil.PerlinFade(ty);

		return NoiseUtil.LERP(				//Takes the hash of ix then shifts it by iy, and computes ratio from the new hash
			NoiseUtil.LERP(h00,h10,tx),
			NoiseUtil.LERP(h01,h11,tx),
			ty) * (1f/(hashMask-1));
	}

	/// <summary>
	/// Calculates Value noise in 3 dimensions
	/// </summary>
	/// <returns>Noise value</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	public override float Noise3D(float x, float y, float z){
		int ix0=0,iy0=0,iz0=0,ix1=0,iy1=0,iz1=0;
		float tx=0,ty=0,tz=0;
		float tf=0;		//Trash Variables
		HashNoise3D(x,y,z,out ix0,out ix0,out tx,out tf,out iy0,out iy1,out ty,out tf,out iz0,out iz1,out tz,out tf);

		int h0 = hash_perm[ix0];								//Hash values to interpolate between
		int h1 = hash_perm[ix1];
		int h00 = hash_perm[h0 + iy0];
		int h10 = hash_perm[h1 + iy0];
		int h01 = hash_perm[h0 + iy1];
		int h11 = hash_perm[h1 + iy1];
		int h000 = hash_perm[h00 + iz0];
		int h100 = hash_perm[h10 + iz0];
		int h010 = hash_perm[h01 + iz0];
		int h110 = hash_perm[h11 + iz0];
		int h001 = hash_perm[h00 + iz1];
		int h101 = hash_perm[h10 + iz1];
		int h011 = hash_perm[h01 + iz1];
		int h111 = hash_perm[h11 + iz1];
		
		NoiseUtil.PerlinFade(tx);
		NoiseUtil.PerlinFade(ty);
		NoiseUtil.PerlinFade(tz);

		return NoiseUtil.LERP(				//Takes the hash of ix then shifts it by iy, then by iz, and computes ratio from the new hash
			NoiseUtil.LERP(NoiseUtil.LERP(h000, h100, tx), NoiseUtil.LERP(h010, h110, tx), ty),
			NoiseUtil.LERP(NoiseUtil.LERP(h001, h101, tx), NoiseUtil.LERP(h011, h111, tx), ty),
			tz) * (1f/(hashMask-1));
	}
}
