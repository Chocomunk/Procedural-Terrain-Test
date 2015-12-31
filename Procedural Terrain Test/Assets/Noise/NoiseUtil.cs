using UnityEngine;
using System.Collections;

public class NoiseUtil {

	//For safety
	public static int MaximumOctave = 16;

	//5th degree fading graph by Ken Perlin: 6t^5 - 15t^4 + 10t^3
	public static float PerlinFade(float t){return t * t * t * ( t * ( t*6f - 15f) + 10f);}

	//Because I dont trust Unity's for being fast enough
	public static float LERP(float a, float b, float t){return (a) + (t) * ((b) - (a));}

	/// <summary>
	/// Calculates a 1D gradient of a point and its dot product
	/// </summary>
	/// <returns>Gradient noise</returns>
	/// <param name="hash">Hash.</param>
	/// <param name="x">The x coordinate.</param>
	public static float Gradient1D(int hash, float x){
		int h = hash & 15;				//Take first 4 bits of hash
		float grad  = 1f + (h & 7);		//Calculate grad from 1st 3 bits of hash
		if((h&8) != 0) grad = -grad;	//Determine negative if 1st significant bit is 1
		return grad * x;
	}

	/// <summary>
	/// Calculates a 2D gradient of a point and its dot product
	/// </summary>
	/// <returns>Gradient noise</returns>
	/// <param name="hash">Hash.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public static float Gradient2D(int hash, float x, float y){
		int h = hash & 7;				//Take first 3 bits of hash
		float u = h < 4 ? x : y;		// If top two significant bits are both 0 then u=x
		float v = h < 4 ? y : x;		//v is not u
		return (((h & 1) != 0) ? -u : u) + (((h & 2) != 0) ? -2f*v : 2f*v);		//Calculate + or - from last 2 bits, then sum u and v
	}

	/// <summary>
	/// Calculates a 3D gradient of a point and its dot product
	/// </summary>
	/// <returns>Gradient noise</returns>
	/// <param name="hash">Hash.</param>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	public static float Gradient3D(int hash, float x, float y, float z){
		int h = hash & 15;				//Take first 4 bits of hash
		float u = h < 8 ? x : y;		//If 1st sig bit is 0, u=x
		float v = (h < 4) ? y : (h==12||h==14) ? x : z;		//If first two sig bits are 0 then v=y, if they are 1 then v=x, otherwise they are different (01 or 10) then v=z
		return (((h & 1) != 0) ? -u : u) + (((h & 2) != 0) ? -v : v);	//Calculate + or - from last 2 bits, then sum u and v
	}
}
