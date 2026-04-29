using UnityEngine;
using System.Collections;
using Poly2Tri;

public static class Vector3Extension {
	public static uint GetID(this Vector3 v) {
		return GetID (v, 4.0f);
	}

	public static uint GetID(this Vector3 v, float precision) {
		float fx = (float)MathUtil.RoundWithPrecision(v.x, precision);
		float fy = (float)MathUtil.RoundWithPrecision(v.y, precision);
		float fz = (float)MathUtil.RoundWithPrecision(v.z, precision);
		
		if(Mathf.Abs(fz) < 0.0001f) {
			fz = 0.0f;
		}
		
		uint vc = MathUtil.Jenkins32Hash(System.BitConverter.GetBytes(fx), 0);
		vc = MathUtil.Jenkins32Hash(System.BitConverter.GetBytes(fy), vc);
		vc = MathUtil.Jenkins32Hash(System.BitConverter.GetBytes(fz), vc);

		return vc;
	}
}