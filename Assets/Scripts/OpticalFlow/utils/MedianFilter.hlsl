#define order(a, b)					                temp = a; a = min(a, b); b = max(temp, b);
#define order2(a, b)				                order(pix[a], pix[b]);
#define orderMin3(a, b, c)			                order2(a, b); order2(a, c);
#define orderMax3(a, b, c)			                order2(b, c); order2(a, c);
#define order3(a, b, c)				                orderMax3(a, b, c); order2(a, b);
#define order4(a, b, c, d)			                order2(a, b); order2(c, d); order2(a, c); order2(b, d);
#define order5(a, b, c, d, e)		                order2(a, b); order2(c, d); orderMin3(a, c, e); orderMax3(b, d, e);
#define order6(a, b, c, d, e, f)	                order2(a, d); order2(b, e); order2(c, f); orderMin3(a, b, c); orderMax3(d, e, f);
#define order2by4(a, b, c, d, e, f, g, h)			order2(a, b); order2(c, d); order2(e, f); order2(g, h); 
#define order2by5(a, b, c, d, e, f, g, h, i, j)		order2by4(a, b, c, d, e, f, g, h); order2(i, j);

float4 Median3x3(sampler2D tex, float2 uv, float2 texel){
    float3 pix[9];
	float3 temp;

	pix[0] = tex2D(tex, uv + float2(-1.0, -1.0) * texel).rgb;
	pix[1] = tex2D(tex, uv + float2(0.0, -1.0)  * texel).rgb;
	pix[2] = tex2D(tex, uv + float2(1.0, -1.0)  * texel).rgb;
	pix[3] = tex2D(tex, uv + float2(-1.0, 0.0)  * texel).rgb;
	pix[4] = tex2D(tex, uv + float2(.0, .0)     * texel).rgb;
	pix[5] = tex2D(tex, uv + float2(1.0, .0)    * texel).rgb;
	pix[6] = tex2D(tex, uv + float2(-1.0, 1.0)  * texel).rgb;
	pix[7] = tex2D(tex, uv + float2(.0, 1.0)    * texel).rgb;
	pix[8] = tex2D(tex, uv + float2(1.0, 1.0)   * texel).rgb;

	order6(0, 1, 2, 3, 4, 5);
	order5(1, 2, 3, 4, 6);
	order4(2, 3, 4, 7);
	order3(3, 4, 8);

    return float4(pix[4], 1.0);
}

float4 Median5x5(sampler2D tex, float2 uv, float2 texel){
    float3 pix[25];
	float3 temp;

	pix[0] 	= tex2D(tex, uv + float2(-2.0, -2.0) * texel).rgb;
	pix[1] 	= tex2D(tex, uv + float2(-1.0, -2.0) * texel).rgb;
	pix[2] 	= tex2D(tex, uv + float2( 0.0, -2.0) * texel).rgb;
	pix[3] 	= tex2D(tex, uv + float2( 1.0, -2.0) * texel).rgb;
	pix[4] 	= tex2D(tex, uv + float2( 2.0, -2.0) * texel).rgb;
	pix[5] 	= tex2D(tex, uv + float2(-2.0, -1.0) * texel).rgb;
	pix[6] 	= tex2D(tex, uv + float2(-1.0, -1.0) * texel).rgb;
	pix[7] 	= tex2D(tex, uv + float2( 0.0, -1.0) * texel).rgb;
	pix[8] 	= tex2D(tex, uv + float2( 1.0, -1.0) * texel).rgb;
	pix[9] 	= tex2D(tex, uv + float2( 2.0, -1.0) * texel).rgb;
	pix[10] = tex2D(tex, uv + float2(-2.0,  0.0) * texel).rgb;
	pix[11] = tex2D(tex, uv + float2(-1.0,  0.0) * texel).rgb;
	pix[12] = tex2D(tex, uv + float2( 0.0,  0.0) * texel).rgb;
	pix[13] = tex2D(tex, uv + float2( 1.0,  0.0) * texel).rgb;
	pix[14] = tex2D(tex, uv + float2( 2.0,  0.0) * texel).rgb;
	pix[15] = tex2D(tex, uv + float2(-2.0,  1.0) * texel).rgb;
	pix[16] = tex2D(tex, uv + float2(-1.0,  1.0) * texel).rgb;
	pix[17] = tex2D(tex, uv + float2( 0.0,  1.0) * texel).rgb;
	pix[18] = tex2D(tex, uv + float2( 1.0,  1.0) * texel).rgb;
	pix[19] = tex2D(tex, uv + float2( 2.0,  1.0) * texel).rgb;
	pix[20] = tex2D(tex, uv + float2(-2.0,  2.0) * texel).rgb;
	pix[21] = tex2D(tex, uv + float2(-1.0,  2.0) * texel).rgb;
	pix[22] = tex2D(tex, uv + float2( 0.0,  2.0) * texel).rgb;
	pix[23] = tex2D(tex, uv + float2( 1.0,  2.0) * texel).rgb;
	pix[24] = tex2D(tex, uv + float2( 2.0,  2.0) * texel).rgb;

	//order 5*5 neighbors pixels
	order2by5(0,1, 3,4, 2,4, 2,3, 6,7);
	order2by5(5,7, 5,6, 9,7, 1,7, 1,4);
	order2by5(12,13, 11,13, 11,12, 15,16, 14,16);
	order2by5(14,15, 18,19, 17,19, 17,18, 21,22);
	order2by5(20,22, 20,21, 23,24, 2,5, 3,6);
	order2by5(0,6, 0,3, 4,7, 1,7, 1,4);
	order2by5(11,14, 8,14, 8,11, 12,15, 9,15);
	order2by5(9,12, 13,16, 10,16, 10,13, 20,23);
	order2by5(17,23, 17,20, 21,24, 18,24, 18,21);
	order2by5(19,22, 8,17, 9,18, 0,18, 0,9);
	order2by5(10,19, 1,19, 1,10, 11,20, 2,20);
	order2by5(2,11, 12,21, 3,21, 3,12, 13,22);
	order2by5(4,22, 4,13, 14,23,5,23,5,14);
	order2by5(15,24, 6,24, 6,15, 7,16, 7,19);
	order2by5(3,11, 5,17, 11,17, 9,17, 4,10);
	order2by5(6,12, 7,14, 4,6, 4,7, 12,14);
	order2by5(10,14, 6,7, 10,12, 6,10, 6,17);
	order2by5(12,17, 7,17, 7,10, 12,18, 7,12);
	order2by4(10,12, 12,20, 10,20, 10,12);

    return float4(pix[12], 1.0);
}