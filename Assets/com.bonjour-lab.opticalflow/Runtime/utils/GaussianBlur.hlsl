#define PI 3.14159265359
uniform float2 _Dir     = float2(0.0, 1.0);


uniform float _BlurSize = 1.0; // This should usually be equal to
                         // 1.0f / texture_pixel_width for a horizontal blur, and
                         // 1.0f / texture_pixel_height for a vertical blur.
uniform float _Sigma     = 4.0; //The sigma value for the gaussian function: higher value means more blur

#define TYPE5x5 	0
#define TYPE7x7 	1
#define TYPE9x9 	2
#define TYPE13x13 	3

#define avgPos(i, tex, uv) 	    avgValue += tex2D(tex, uv - i * _BlurSize * _Dir) * incrementalGaussian.x;
#define avgNeg(i, tex, uv) 	    avgValue += tex2D(tex, uv + i * _BlurSize * _Dir) * incrementalGaussian.x; 
#define coeff(i)	  	        coefficientSum += 2.0 * incrementalGaussian.x;
#define incGauss(i)	            incrementalGaussian.xy *= incrementalGaussian.yz;
#define blur(i, tex, uv)		avgPos(i, tex, uv); avgNeg(i, tex, uv); coeff(i); incGauss(i);
#define blur5x5(uv, tex)	    blur(1, tex, uv); blur(2, tex, uv);
#define blur7x7(uv, tex)	    blur5x5(uv, tex); blur(3, tex, uv);
#define blur9x9(uv, tex)	    blur7x7(uv, tex); blur(4, tex, uv);
#define blur13x13(uv, tex)	    blur9x9(uv, tex); blur(5, tex, uv); blur(6, tex, uv); blur(7, tex, uv);

float4 Gaussian(sampler2D tex, float2 uv){
	// Incremental Gaussian Coefficent Calculation (See GPU Gems 3 pp. 877 - 889)
	float3 incrementalGaussian;
	incrementalGaussian.x   = 1.0 / (sqrt(2.0 * PI) * _Sigma);
	incrementalGaussian.y   = exp(-0.5 / (_Sigma * _Sigma));
	incrementalGaussian.z   = incrementalGaussian.y * incrementalGaussian.y;

 	float4 avgValue         = float4(0.0, 0.0, 0.0, 0.0);
 	float coefficientSum    = 0.0;

  	// Take the central sample first...
  	avgValue                += tex2D(tex, uv) * incrementalGaussian.x;
  	coefficientSum          += incrementalGaussian.x;
  	incrementalGaussian.xy  *= incrementalGaussian.yz;

	if(TYPE == TYPE13x13){
		blur13x13(uv, tex);
	}else if(TYPE == TYPE7x7){
		blur7x7(uv, tex);
	}else if(TYPE == TYPE9x9){
		blur9x9(uv, tex);
	}else if(TYPE == TYPE5x5){
		blur5x5(uv, tex);
	}
  	
  	return avgValue / coefficientSum;
}