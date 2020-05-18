float2 fade(float2 t) {
	return t*t*t*(t*(t * 6.0 - 15.0) + 10.0);
}

float4 modf4(float4 a, float4 b) {
	return a - floor(a / b);
}

float4 frac_parts(float2 a) {
	float4 outp;
	if (a.y < 0) {
		outp.yw = a.yy - ceil(a.yy) - float2(0,1);
	} else {
		outp.yw = a.yy - floor(a.yy) + float2(0,1);
	}
	//if (a.y < 0) {
	//	outp.yw = a.yy - ceil(a.yy) - float2(0, 1);
	//} else {
	//	outp.yw = a.yy - floor(a.yy) - float2(0, 1);
	//}
	outp.xz = frac(a.xx) - float4(0., 0., 1., 1.);
	return outp;
}

uint mod(uint a, uint b) {
	return a - (a / b);
}

uint permute(float x, uint hashSize) {
	return mod(((x * 34) + 1)*x, hashSize);
}

float4 permutef4(float4 x, uint hashSize) {
	return modf4(((x * 34.) + 1.)*x, (float4)hashSize);
}

float gradient2D(uint hash, float2 coord) {
	switch (mod(hash, 0x4)) {
	case 0x0:
		return coord.x + coord.y;
	case 0x1:
		return -coord.x + coord.y;
	case 0x2:
		return -coord.x + coord.y;
	case 0x3:
		return -coord.x - coord.y;
	default:
		return 0;
	}
}

void grad2D(float4 hash, out float2 grads[4]) {
	float4 g1 = frac(hash / 41.0) * 2.0 - 1.0;
	float4 g2 = abs(g1) - 0.5;
	g1 = g1 - floor(g1 + 0.5);

	grads[0] = float2(g1.x, g2.x);
	grads[1] = float2(g1.y, g2.y);
	grads[2] = float2(g1.z, g2.z);
	grads[3] = float2(g1.w, g2.w);

	float4 norm = normalize(float4(dot(grads[0], grads[0]), dot(grads[1], grads[1]), dot(grads[2], grads[2]), dot(grads[3], grads[3])));
	grads[0] *= norm.x;
	grads[1] *= norm.y;
	grads[2] *= norm.z;
	grads[3] *= norm.w;
}

float perlinNoise2D(float2 coordinate, float repeatDim, uint hashSize) {
	float4 iparts = floor(coordinate.xyxy) + float4(0., 0., 1., 1.);	// Lower (x,y), upper (x,y) for 1x1 box
	float4 fparts = frac(coordinate.xyxy) - float4(0., 0., 1., 1.);	// Point distance from all four sides of 1x1 enclosing box

	iparts = modf4(iparts, (float4)(hashSize-1));
	
    //float g00, g01, g10, g11;
    //float2 t, interp;
    //t = fade(fparts.xy);

    //g00 = gradient2D(permute(permute(iparts.x, hashSize) + iparts.y, hashSize), fparts.xy);
    //g01 = gradient2D(permute(permute(iparts.x, hashSize) + iparts.w, hashSize), fparts.zy);

    //g10 = gradient2D(permute(permute(iparts.z, hashSize) + iparts.y, hashSize), fparts.xw);
    //g11 = gradient2D(permute(permute(iparts.z, hashSize) + iparts.w, hashSize), fparts.zw);

    //interp = lerp(float2(g00, g10), float2(g01, g11), t.x);

    //return lerp(interp.x, interp.y, t.y);

    float2 grads[4];
    float4 hash = permutef4(permutef4(iparts.xzxz, hashSize) + iparts.yyww, hashSize);
    grad2D(hash, grads);

    float n00 = dot(grads[0], float2(fparts.x, fparts.y));
    float n01 = dot(grads[1], float2(fparts.z, fparts.y));
    float n10 = dot(grads[2], float2(fparts.x, fparts.w));
    float n11 = dot(grads[3], float2(fparts.z, fparts.w));

    float2 t = fade(fparts.xy);
    float2 n = lerp(float2(n00, n10), float2(n01, n11), t.x);
    return lerp(n.x, n.y, t.y);
}
