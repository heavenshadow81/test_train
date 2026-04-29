using UnityEngine;
using System.Collections;

public class BrushUtils {
	static public HSV RGBToHSV (Color input) {
		HSV output = new HSV ();
		
		float min = Mathf.Min (input.r, input.g, input.b);
		float max = Mathf.Max (input.r, input.g, input.b);
		
		output.v = max;                // v
		
		float d = max - min;
		if (max != 0.0f) { 
			output.s = (d / max);      // s
		} else {
			// r, g, b = 0             // s = 0, v is undefined
			output.s = 0.0f;
			output.h = float.NaN;
			return output;
		}
		
		if (input.r == max) {
			output.h = (input.g - input.b) / d;         // between yellow & magenta
		} else if (input.g == max) {
			output.h = 2.0f + (input.b - input.r) / d;  // between cyan & yellow
		} else {
			output.h = 4.0f + (input.r - input.g) / d;  // between magenta & cyan
		}
		
		output.h *= 60.0f;
		
		if (output.h < 0.0f) {
			output.h += 360.0f;
		}   
		
		return output;
	}
	
	static public Color HSVToRGB (HSV input)
	{
		Color output = new Color (0, 0, 0, 1);
		
		if (input.s == 0.0f) {         
			if (float.IsNaN (input.h)) { // input.h == NaN
				output.r = output.g = output.b = input.v;
				return output;
			}
			// error - should never happen
			output.r = 0.0f;
			output.g = 0.0f;
			output.b = 0.0f;
			return output;
		}
		
		if (input.h == 360.0f)
			input.h = 0.0f;
		input.h /= 60.0f;
		int i = (int)input.h;
		float f = input.h - i;
		float p = input.v * (1.0f - input.s);
		float q = input.v * (1.0f - (input.s * f));
		float t = input.v * (1.0f - (input.s * (1.0f - f)));
		
		switch (i) {
		case 0:
			output.r = input.v;
			output.g = t;
			output.b = p;
			break;
			
		case 1:
			output.r = q;
			output.g = input.v;
			output.b = p;
			break;
			
		case 2:
			output.r = p;
			output.g = input.v;
			output.b = t;
			break;
			
		case 3:
			output.r = p;
			output.g = q;
			output.b = input.v;
			break;
			
		case 4:
			output.r = t;
			output.g = p;
			output.b = input.v;
			break;
			
		case 5:
		default:
			output.r = input.v;
			output.g = p;
			output.b = q;
			break;
		}
		
		return output; 
	}
	
	static public Vector3 Min (Vector3 lhv, Vector3 rhv)
	{
		return new Vector3 (Mathf.Min (lhv.x, rhv.x), Mathf.Min (lhv.y, rhv.y), Mathf.Min (lhv.z, rhv.z));
	}
	
	static public Vector3 Max (Vector3 lhv, Vector3 rhv)
	{
		return new Vector3 (Mathf.Max (lhv.x, rhv.x), Mathf.Max (lhv.y, rhv.y), Mathf.Max (lhv.z, rhv.z));
	}
	
	static public Vector3 CatmullRom (Vector3 previous, Vector3 start, Vector3 end, Vector3 next, float elapsedTime, float duration)
	{
		
		float percentComplete = elapsedTime / duration;
		float percentCompleteSquared = percentComplete * percentComplete;
		float percentCompleteCubed = percentCompleteSquared * percentComplete;
		
		return previous * (-0.5f * percentCompleteCubed +
		                   percentCompleteSquared -
		                   0.5f * percentComplete) +
			start * (1.5f * percentCompleteCubed +
			         -2.5f * percentCompleteSquared + 1.0f) +
				end * (-1.5f * percentCompleteCubed +
				       2.0f * percentCompleteSquared +
				       0.5f * percentComplete) +
				next * (0.5f * percentCompleteCubed -
				        0.5f * percentCompleteSquared);
	}
	
	static public float ColorBurnf (float baseComponent, float blendComponent)
	{
		return blendComponent == 0.0f ? blendComponent : Mathf.Max ((1.0f - ((1.0f - baseComponent) / blendComponent)), 0.0f);
	}

    public static float Screenf(float baseComponent, float blendComponent)
    {
        return 1.0f - (1.0f * baseComponent) * (1.0f - blendComponent);
    }

	static public Color Blend (BlendMode blendMode, Color baseColor, Color blendColor)
	{
		if (blendMode == BlendMode.AlphaBlend) {
			float outAlpha = baseColor.a * (1.0f - blendColor.a) + blendColor.a;
			if (outAlpha == 0.0f) {
				return Color.clear;
			} else {
				return new Color (
					(baseColor.r * baseColor.a * (1.0f - blendColor.a) + blendColor.r * blendColor.a) / outAlpha,
					(baseColor.g * baseColor.a * (1.0f - blendColor.a) + blendColor.g * blendColor.a) / outAlpha,
					(baseColor.b * baseColor.a * (1.0f - blendColor.a) + blendColor.b * blendColor.a) / outAlpha,
					outAlpha
					);
			}
		} else if (blendMode == BlendMode.Multiply) {
			float outAlpha = baseColor.a * (1.0f - blendColor.a) + blendColor.a;
			if (outAlpha == 0.0f) {
				return Color.clear;
			} else {
				return new Color (
					(baseColor.r * baseColor.a * (1.0f - blendColor.a) + Mathf.Lerp(1.0f, baseColor.r, baseColor.a) * blendColor.r * blendColor.a) / outAlpha,
					(baseColor.g * baseColor.a * (1.0f - blendColor.a) + Mathf.Lerp(1.0f, baseColor.g, baseColor.a) * blendColor.g * blendColor.a) / outAlpha,
					(baseColor.b * baseColor.a * (1.0f - blendColor.a) + Mathf.Lerp(1.0f, baseColor.b, baseColor.a) * blendColor.b * blendColor.a) / outAlpha,
					outAlpha
					);
			}
		} else if (blendMode == BlendMode.ColorBurn) {
			float outAlpha = baseColor.a * (1.0f - blendColor.a) + blendColor.a;
			if (outAlpha == 0.0f) {
				return Color.clear;
			} else {
				return new Color (
					(baseColor.r * baseColor.a * (1.0f - blendColor.a) + Mathf.Lerp(blendColor.r, ColorBurnf (baseColor.r, blendColor.r), baseColor.a) * blendColor.a) / outAlpha,
					(baseColor.g * baseColor.a * (1.0f - blendColor.a) + Mathf.Lerp(blendColor.g, ColorBurnf (baseColor.g, blendColor.g), baseColor.a) * blendColor.a) / outAlpha,
					(baseColor.b * baseColor.a * (1.0f - blendColor.a) + Mathf.Lerp(blendColor.b, ColorBurnf (baseColor.b, blendColor.b), baseColor.a) * blendColor.a) / outAlpha,
					outAlpha
					);
			}
        }
        else if (blendMode == BlendMode.Screen)
        {
            float outAlpha = baseColor.a * (1.0f - blendColor.a) + blendColor.a;
            if (outAlpha == 0.0f)
            {
                return Color.clear;
            }
            else
            {
                return new Color(
                    (baseColor.r * baseColor.a * (1.0f - blendColor.a) + Mathf.Lerp(blendColor.r, Screenf(baseColor.r, blendColor.r), baseColor.a) * blendColor.a) / outAlpha,
                    (baseColor.g * baseColor.a * (1.0f - blendColor.a) + Mathf.Lerp(blendColor.g, Screenf(baseColor.g, blendColor.g), baseColor.a) * blendColor.a) / outAlpha,
                    (baseColor.b * baseColor.a * (1.0f - blendColor.a) + Mathf.Lerp(blendColor.b, Screenf(baseColor.b, blendColor.b), baseColor.a) * blendColor.a) / outAlpha,
                    outAlpha
                    );
            }
        }
        else if (blendMode == BlendMode.MultiplyRaw)
        {
            return baseColor * blendColor;
        }
        else
        {
            return baseColor;
        }
	}

	/*
	static public Color Blend (BlendMode blendMode, Color baseColor, Color blendColor)
	{
		Color newColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		float outAlpha = baseColor.a * (1.0f - blendColor.a) + blendColor.a;

		if(outAlpha >= 0.00000001f) {
			if(blendMode == BlendMode.AlphaBlend) {
				newColor = (baseColor * baseColor.a * (1.0f - blendColor.a) + blendColor * blendColor.a) / outAlpha;
				newColor.a = outAlpha;
			}
			else if(blendMode == BlendMode.Multiply) {
				//newColor = (baseColor * baseColor.a * (1.0f - blendColor.a) + baseColor * blendColor * blendColor.a) / outAlpha;
				//newColor.a = outAlpha;
				newColor = baseColor * blendColor;
			}
			else if(blendMode == BlendMode.Multiply) {
				newColor = (baseColor * baseColor.a * (1.0f - blendColor.a) + (Color.white - (Color.white - baseColor) * (Color.white - blendColor)) * blendColor.a) / outAlpha;
				newColor.a = outAlpha;
			}
			else if(blendMode == BlendMode.Overlay) {
				Color blend = Color.white;
				if(baseColor.r < 0.5f) {
					blend = 2.0f * baseColor * blendColor;
				}
				else {
					blend = Color.white - 2.0f * (Color.white - baseColor) * (Color.white - blendColor);
				}
				
				newColor = (baseColor * baseColor.a * (1.0f - blendColor.a) + blend * blendColor.a) / outAlpha;
				newColor.a = outAlpha;
			}
			else if(blendMode == BlendMode.ColorBurn) {
				newColor = new Color (
					ColorBurnf (baseColor.r, blendColor.r), 
					ColorBurnf (baseColor.g, blendColor.g), 
					ColorBurnf (baseColor.b, blendColor.b), 
					outAlpha
				);
			}
		}

		return newColor;
	}
	*/

}