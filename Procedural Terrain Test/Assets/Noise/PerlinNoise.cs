using UnityEngine;
using System.Collections;

public class PerlinNoise : NoiseModule {

	public PerlinNoise(int seed) : base(seed){}

	/// <summary>
	/// Returns Perlin noise at a point in 1D
	/// </summary>
	/// <returns>Perlin Noise value</returns>
	/// <param name="x">The x coordinate.</param>
	public override float Noise1D(float x){
		int ix0=0,ix1=0;
		float fx0=0,fx1=0;
		float s, n0, n1;		//Noise interpolation variables
		HashNoise1D(x,out ix0,out ix1,out fx0,out fx1);
		s = NoiseUtil.PerlinFade(fx0);							//Fade the ratio variable
		n0 = NoiseUtil.Gradient1D(hash_perm[ix0], fx0);			//Interpolate the two points
		n1 = NoiseUtil.Gradient1D(hash_perm[ix1], fx1);
		return 0.188f * NoiseUtil.LERP(n0,n1,s);							//Interpolate along two noise values
	}

	/// <summary>
	/// Returns Perlin noise at a point in 2D
	/// </summary>
	/// <returns>Perlin Noise value</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="x">The y coordinate.</param>
	public override float Noise2D(float x, float y){
		int ix0=0,iy0=0,ix1=0,iy1=0;
		float fx0=0,fy0=0,fx1=0,fy1=0;
		float s, t, na0, na1, n0, n1;		//Noise interpolation variables
		HashNoise2D(x,y,out ix0,out ix1,out fx0,out fx1,out iy0,out iy1,out fy0,out fy1);
		t = NoiseUtil.PerlinFade(fx0);							//Fade the ratio variables
		s = NoiseUtil.PerlinFade(fy0);

		na0 = NoiseUtil.Gradient2D(hash_perm[ix0 + hash_perm[iy0]], fx0, fy0);		//Interpolate along 4 corners
		na1 = NoiseUtil.Gradient2D(hash_perm[ix0 + hash_perm[iy1]], fx0, fy1);
		n0 = NoiseUtil.LERP(na0, na1, t);
		na0 = NoiseUtil.Gradient2D(hash_perm[ix1 + hash_perm[iy0]], fx1, fy0);
		na1 = NoiseUtil.Gradient2D(hash_perm[ix1 + hash_perm[iy1]], fx1, fy1);
		n1 = NoiseUtil.LERP(na0, na1, t);

		return NoiseUtil.LERP(n0,n1,s);							//Interpolate along two noise values
//		return 0.507f * NoiseUtil.LERP(n0,n1,s);
	}

	/// <summary>
	/// Returns Perlin noise at a point in 2D
	/// </summary>
	/// <returns>Perlin Noise value</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="x">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	public override float Noise3D(float x, float y, float z){
		int ix0=0,iy0=0,iz0=0,ix1=0,iy1=0,iz1=0;
		float fx0=0,fy0=0,fz0=0,fx1=0,fy1=0,fz1=0;
		float s, t, r;				//Interpolation ratio variables
		float nb0, nb1, na0, na1, n0, n1;		//Noise interpolation variables
		HashNoise2D(x,y,out ix0,out ix1,out fx0,out fx1,out iy0,out iy1,out fy0,out fy1);
		r = NoiseUtil.PerlinFade(fx0);							//Fade the ratio variables
		t = NoiseUtil.PerlinFade(fy0);
		s = NoiseUtil.PerlinFade(fy0);
		
		nb0 = NoiseUtil.Gradient3D(hash_perm[ix0 + hash_perm[iy0 + hash_perm[iz0]]], fx0, fy0, fz0);	//Interpolate along 8 corners
		nb1 = NoiseUtil.Gradient3D(hash_perm[ix0 + hash_perm[iy0 + hash_perm[iz1]]], fx0, fy0, fz1);
		na0 = NoiseUtil.LERP(nb0, nb1, r);

		nb0 = NoiseUtil.Gradient3D(hash_perm[ix0 + hash_perm[iy1 + hash_perm[iz0]]], fx0, fy1, fz0);
		nb1 = NoiseUtil.Gradient3D(hash_perm[ix0 + hash_perm[iy1 + hash_perm[iz1]]], fx0, fy1, fz1);
		na1 = NoiseUtil.LERP(nb0, nb1, r);
		n0 = NoiseUtil.LERP(na0, na1, t);

		nb0 = NoiseUtil.Gradient3D(hash_perm[ix1 + hash_perm[iy0 + hash_perm[iz0]]], fx1, fy0, fz0);
		nb1 = NoiseUtil.Gradient3D(hash_perm[ix1 + hash_perm[iy0 + hash_perm[iz1]]], fx1, fy0, fz1);
		na0 = NoiseUtil.LERP(nb0, nb1, r);

		nb0 = NoiseUtil.Gradient3D(hash_perm[ix1 + hash_perm[iy1 + hash_perm[iz0]]], fx1, fy1, fz0);
		nb1 = NoiseUtil.Gradient3D(hash_perm[ix1 + hash_perm[iy1 + hash_perm[iz1]]], fx1, fy1, fz1);
		na1 = NoiseUtil.LERP(nb0, nb1, r);
		n1 = NoiseUtil.LERP(na0, na1, t);
		
		return 0.936f * NoiseUtil.LERP(n0,n1,s);							//Interpolate along two noise values
	}
}
