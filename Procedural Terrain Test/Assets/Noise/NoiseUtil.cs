using UnityEngine;
using System.Collections;

public class NoiseUtil {

	//For safety
	public static int MaximumOctave = 16;

	//5th degree fading graph by Ken Perlin: 6t^5 - 15t^4 + 10t^3
	public static float PerlinFade(float t){return t * t * t * ( t * ( t*6f - 15f) + 10f);}

	//Because I dont trust Unity's for being fast enough
	public static float LERP(float a, float b, float t){return (a) + (t) * ((b) - (a));}

/*	MESSAGE ON GRADIENT FUNCTIONS:
 * 
 *	This calculation of Gradients is Ken Perlin's Gradient functions.
 *	I choose to use his adaptation of the algorithm because I believe
 *	it is faster than a more straight-forward method.
 *	
 *	How it works:
 *		For every dimension of calculations there exists a set of vectors
 *	pointing in different dimensions (for example one dimension would
 *	have the vectors {<-1>,<1>}). The point of the hash in this algorithm
 *	is to introduce randomness. You take the remainder of the vector
 *	above the size of the set, and would then be able to get a vector
 *	by index due to it fitting the size of the array after the operation.
 *	You then take the dot product of the gradient and vectors from the
 *	point to nearest surrounding points on the graphing plane. To find
 *	those vectors you essentially just take the fractional parts of
 *	their values in all dimensions. This is because graphing planes we
 *	use are always spaced by 1 whole integer. Once you have this vector
 *	it is easy to find the dot products because the gradients point
 *	along 1 or 2 axes all the time with length 1 or 0 in any specified
 *	axis. Because of this property, we can just multiply matching
 *	directions (the x, y, or z dimentions), then add them together.
 *		Ken Perlin accomplished this in a very short, but hard to
 *	read way. He visualized all possible gradients and gave them a
 *	label of a binary number. Through patterns of these labels (which 
 *	are not clear to see unless you draw them all out) he applied 
 *	operations onto different variables, setting them to either + or - 
 *	of the x, y, and z vector quantities (depending the dimension), then 
 *	added them together.
 */

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
