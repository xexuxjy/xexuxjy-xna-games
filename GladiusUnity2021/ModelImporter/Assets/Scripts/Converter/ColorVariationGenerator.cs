using System.Collections.Generic;
using BCnEncoder.Shared;
using GCTextureTools;

namespace BCnEncoder.Encoder
{
	internal static class ColorVariationGenerator
	{

		private static readonly int[] variatePatternEp0R = new int[] { 1, 1, 0, 0, -1, 0, 0, -1, 1, -1, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		private static readonly int[] variatePatternEp0G = new int[] { 1, 0, 1, 0, 0, -1, 0, -1, 1, -1, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		private static readonly int[] variatePatternEp0B = new int[] { 1, 0, 0, 1, 0, 0, -1, -1, 1, -1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0 };
		private static readonly int[] variatePatternEp1R = new int[] { -1, -1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, -1, 1, 0, 0, -1, 0, 0 };
		private static readonly int[] variatePatternEp1G = new int[] { -1, 0, -1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, -1, 0, 1, 0, 0, -1, 0 };
		private static readonly int[] variatePatternEp1B = new int[] { -1, 0, 0, -1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, -1, 0, 0, 1, 0, 0, -1 };
		public static int VarPatternCount => variatePatternEp0R.Length;

		public static (ColorRgb565, ColorRgb565) Variate565(ColorRgb565 c0, ColorRgb565 c1, int i)
		{
			var idx = i % variatePatternEp0R.Length;
			var newEp0 = new ColorRgb565();
			var newEp1 = new ColorRgb565();

			newEp0.RawR = ByteHelper.ClampToByte(c0.RawR + variatePatternEp0R[idx]);
			newEp0.RawG = ByteHelper.ClampToByte(c0.RawG + variatePatternEp0G[idx]);
			newEp0.RawB = ByteHelper.ClampToByte(c0.RawB + variatePatternEp0B[idx]);

			newEp1.RawR = ByteHelper.ClampToByte(c1.RawR + variatePatternEp1R[idx]);
			newEp1.RawG = ByteHelper.ClampToByte(c1.RawG + variatePatternEp1G[idx]);
			newEp1.RawB = ByteHelper.ClampToByte(c1.RawB + variatePatternEp1B[idx]);

			return (newEp0, newEp1);
		}

	}
}
