// https://github.com/Nominom/BCnEncoder.NET/blob/master/BCnEnc.Net/Encoder/Bc1BlockEncoder.cs

using BCnEncoder.Encoder;
using BCnEncoder.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GCTextureTools
{

    public static class BlockEncoder
    {
		public static class Bc1BlockEncoderSlow
		{
			public const int MaxTries = 9999;
			public const float ErrorThreshold = 0.01f;


			public static Vector4 CalculateMean(Vector4[] colors)
			{

				float r = 0;
				float g = 0;
				float b = 0;
				float a = 0;

				for (var i = 0; i < colors.Length; i++)
				{
					r += colors[i].X;
					g += colors[i].Y;
					b += colors[i].Z;
					a += colors[i].W;
				}

				return new Vector4(
					r / colors.Length,
					g / colors.Length,
					b / colors.Length,
					a / colors.Length
					);
			}


			static Matrix4x4 CalculateCovariance(Vector4[] values, out Vector4 mean)
			{
				mean = CalculateMean(values);
				for (var i = 0; i < values.Length; i++)
				{
					values[i] -= mean;
				}

				//4x4 matrix
				var mat = new Matrix4x4();

				for (var i = 0; i < values.Length; i++)
				{
					mat.M11 += values[i].X * values[i].X;
					mat.M12 += values[i].X * values[i].Y;
					mat.M13 += values[i].X * values[i].Z;
					mat.M14 += values[i].X * values[i].W;

					mat.M22 += values[i].Y * values[i].Y;
					mat.M23 += values[i].Y * values[i].Z;
					mat.M24 += values[i].Y * values[i].W;

					mat.M33 += values[i].Z * values[i].Z;
					mat.M34 += values[i].Z * values[i].W;

					mat.M44 += values[i].W * values[i].W;
				}

				mat = Matrix4x4.Multiply(mat, 1f / (values.Length - 1));

				mat.M21 = mat.M12;
				mat.M31 = mat.M13;
				mat.M32 = mat.M23;
				mat.M41 = mat.M14;
				mat.M42 = mat.M24;
				mat.M43 = mat.M34;

				return mat;
			}

			internal static Vector4 CalculatePrincipalAxis(Matrix4x4 covarianceMatrix)
			{
				var lastDa = Vector4.UnitY;

				for (var i = 0; i < 30; i++)
				{
					var dA = Vector4.Transform(lastDa, covarianceMatrix);

					if (dA.LengthSquared() == 0)
					{
						break;
					}

					dA = Vector4.Normalize(dA);
					if (Vector4.Dot(lastDa, dA) > 0.999999)
					{
						lastDa = dA;
						break;
					}
					else
					{
						lastDa = dA;
					}
				}
				return lastDa;
			}



			public static void ConvertToVector4(Color[] colors,Vector4[] vectors)
            {
				for(int i=0;i<colors.Length;++i)
                {
					vectors[i].X += colors[i].R / 255f;
					vectors[i].Y += colors[i].G / 255f;
					vectors[i].Z += colors[i].B / 255f;
					vectors[i].W += colors[i].A / 255f;
				}
            }

			public static void CreateVectors(Color[] colors, out Vector3 mean, out Vector3 principalAxis)
			{
				Vector4[] vectors = new Vector4[colors.Length];
				ConvertToVector4(colors, vectors);


				var cov = CalculateCovariance(vectors, out var v4Mean);
				mean = new Vector3(v4Mean.X, v4Mean.Y, v4Mean.Z);

				var pa = CalculatePrincipalAxis(cov);
				principalAxis = new Vector3(pa.X, pa.Y, pa.Z);
				if (principalAxis.LengthSquared() == 0)
				{
					principalAxis = Vector3.UnitY;
				}
				else
				{
					principalAxis = Vector3.Normalize(principalAxis);
				}

			}


			public static void GetMinMaxColor565(Color[] colors, Vector3 mean, Vector3 principalAxis,out ColorRgb565 min, out ColorRgb565 max)
			{

				float minD = 0;
				float maxD = 0;

				for (var i = 0; i < colors.Length; i++)
				{
					var colorVec = new Vector3(colors[i].R / 255f, colors[i].G / 255f, colors[i].B / 255f);

					var v = colorVec - mean;
					var d = Vector3.Dot(v, principalAxis);
					if (d < minD) minD = d;
					if (d > maxD) maxD = d;
				}

				//Inset
				minD *= 15 / 16f;
				maxD *= 15 / 16f;

				var minVec = mean + principalAxis * minD;
				var maxVec = mean + principalAxis * maxD;

				var minR = (int)(minVec.X * 255);
				var minG = (int)(minVec.Y * 255);
				var minB = (int)(minVec.Z * 255);

				var maxR = (int)(maxVec.X * 255);
				var maxG = (int)(maxVec.Y * 255);
				var maxB = (int)(maxVec.Z * 255);

				minR = minR >= 0 ? minR : 0;
				minG = minG >= 0 ? minG : 0;
				minB = minB >= 0 ? minB : 0;

				maxR = maxR <= 255 ? maxR : 255;
				maxG = maxG <= 255 ? maxG : 255;
				maxB = maxB <= 255 ? maxB : 255;

				// Optimal round
				minR = (minR & ColorRgb565.C565_5Mask) | (minR >> 5);
				minG = (minG & ColorRgb565.C565_6Mask) | (minG >> 6);
				minB = (minB & ColorRgb565.C565_5Mask) | (minB >> 5);

				maxR = (maxR & ColorRgb565.C565_5Mask) | (maxR >> 5);
				maxG = (maxG & ColorRgb565.C565_6Mask) | (maxG >> 6);
				maxB = (maxB & ColorRgb565.C565_5Mask) | (maxB >> 5);

				min = new ColorRgb565((byte)minR, (byte)minG, (byte)minB);
				max = new ColorRgb565((byte)maxR, (byte)maxG, (byte)maxB);

			}



			public static DXTBlock EncodeBlock(DXTBlock rawBlock)
			{
				CreateVectors(rawBlock.SourceColours, out var mean, out var pa);
				
				GetMinMaxColor565(rawBlock.SourceColours, mean, pa, out var min, out var max);

				var c0 = max;
				var c1 = min;

				if (c0.data < c1.data)
				{
					var c = c0;
					c0 = c1;
					c1 = c;
				}


				DXTBlock best = TryColors(rawBlock, c0, c1, out var bestError);


				var lastChanged = 0;

				for (var i = 0; i < MaxTries; i++)
				{
					var (newC0, newC1) = ColorVariationGenerator.Variate565(c0, c1, i);

					if (newC0.data < newC1.data)
					{
						var c = newC0;
						newC0 = newC1;
						newC1 = c;
					}

					DXTBlock block = TryColors(rawBlock, newC0, newC1, out var error);

					lastChanged++;

					if (error < bestError)
					{
						best = block;
						bestError = error;
						c0 = newC0;
						c1 = newC1;
						lastChanged = 0;
					}

					if (bestError < ErrorThreshold || lastChanged > ColorVariationGenerator.VarPatternCount)
					{
						break;
					}
				}

				return best;
			}
		}

		public static DXTBlock TryColors(DXTBlock rawBlock, ColorRgb565 color0, ColorRgb565 color1, out float error, float rWeight = 0.3f, float gWeight = 0.6f, float bWeight = 0.1f)
		{
			DXTBlock resultBlock = new DXTBlock();
			var pixels = rawBlock.SourceColours;

			resultBlock.CalculatedColor0  = color0;
			resultBlock.CalculatedColor1 = color1;

			bool hasAlpha = resultBlock.HasAlphaOrBlack;

			var c0 = color0.ToColor();
			var c1 = color1.ToColor();

			Color[] colors = resultBlock.HasAlphaOrBlack ?new Color[] {c0,c1,c0.InterpolateHalf(c1),Color.FromArgb(255,0, 0, 0)} : new Color[] {c0,c1,c0.InterpolateThird(c1, 1),c0.InterpolateThird(c1, 2)};

			error = 0;
			for (var i = 0; i < 16; i++)
			{
				var color = pixels[i];
				//resultBlock.DecodedColours[i] = Color.FromArgb(ChooseClosestColor4AlphaCutOff(colors, color, rWeight, gWeight, bWeight, 128,hasAlpha,out var e));
				//resultBlock.LineIndices[i] = (uint)ChooseClosestColor4AlphaCutOff(colors, color, rWeight, gWeight, bWeight, 128, hasAlpha, out var e);
				resultBlock.LineIndices[i] = (uint)ChooseClosestColor4(colors, color, rWeight, gWeight, bWeight, out var e);
				error += e;
			}
			return resultBlock;
		}

		public static Color InterpolateHalf(this Color c0, Color c1) =>
		InterpolateColor(c0, c1, 1, 2);

		/// <summary>
		/// Interpolates two colors by third.
		/// </summary>
		/// <param name="c0">The first color endpoint.</param>
		/// <param name="c1">The second color endpoint.</param>
		/// <param name="num">The dividend in the third.</param>
		/// <returns>The interpolated color.</returns>
		public static Color InterpolateThird(this Color c0, Color c1, int num) =>
			InterpolateColor(c0, c1, num, 3);


		public static Color InterpolateColor(Color c0, Color c1, int num, int den) => Color.FromArgb(255,
		(byte)Interpolate(c0.R, c1.R, num, den),
		(byte)Interpolate(c0.G, c1.G, num, den),
		(byte)Interpolate(c0.B, c1.B, num, den));

		public static int Interpolate(int a, int b, int num, int den, int correction = 0) =>
			(int)(((den - num) * a + num * b + correction) / (float)den);



		public static int ChooseClosestColor4(Color[] colors, Color color, float rWeight, float gWeight, float bWeight, out float error)
		{
			 float[] d = new float[4] {
				Math.Abs(colors[0].R - color.R) * rWeight
				+ Math.Abs(colors[0].G - color.G) * gWeight
				+ Math.Abs(colors[0].B - color.B) * bWeight,
				Math.Abs(colors[1].R - color.R) * rWeight
				+ Math.Abs(colors[1].G - color.G) * gWeight
				+ Math.Abs(colors[1].B - color.B) * bWeight,
				Math.Abs(colors[2].R - color.R) * rWeight
				+ Math.Abs(colors[2].G - color.G) * gWeight
				+ Math.Abs(colors[2].B - color.B) * bWeight,
				Math.Abs(colors[3].R - color.R) * rWeight
				+ Math.Abs(colors[3].G - color.G) * gWeight
				+ Math.Abs(colors[3].B - color.B) * bWeight,
			};

			var b0 = d[0] > d[3] ? 1 : 0;
			var b1 = d[1] > d[2] ? 1 : 0;
			var b2 = d[0] > d[2] ? 1 : 0;
			var b3 = d[1] > d[3] ? 1 : 0;
			var b4 = d[2] > d[3] ? 1 : 0;

			var x0 = b1 & b2;
			var x1 = b0 & b3;
			var x2 = b0 & b4;

			var idx = x2 | ((x0 | x1) << 1);
			error = d[idx];
			return idx;
		}


		public static int ChooseClosestColor4AlphaCutOff(Color[] colors, Color color, float rWeight, float gWeight, float bWeight, int alphaCutOff, bool hasAlpha, out float error)
		{
			if (hasAlpha && color.A < alphaCutOff)
			{
				error = 0;
				return 3;
			}

			float[] d = new float[4] {
			Math.Abs(colors[0].R - color.R) * rWeight
			+ Math.Abs(colors[0].G - color.G) * gWeight
			+ Math.Abs(colors[0].B - color.B) * bWeight,
			Math.Abs(colors[1].R - color.R) * rWeight
			+ Math.Abs(colors[1].G - color.G) * gWeight
			+ Math.Abs(colors[1].B - color.B) * bWeight,
			Math.Abs(colors[2].R - color.R) * rWeight
			+ Math.Abs(colors[2].G - color.G) * gWeight
			+ Math.Abs(colors[2].B - color.B) * bWeight,

			hasAlpha ? 999 :
			Math.Abs(colors[3].R - color.R) * rWeight
			+ Math.Abs(colors[3].G - color.G) * gWeight
			+ Math.Abs(colors[3].B - color.B) * bWeight,
		};

			var b0 = d[0] > d[2] ? 1 : 0;
			var b1 = d[1] > d[3] ? 1 : 0;
			var b2 = d[0] > d[3] ? 1 : 0;
			var b3 = d[1] > d[2] ? 1 : 0;
			var nb3 = d[1] > d[2] ? 0 : 1;
			var b4 = d[0] > d[1] ? 1 : 0;
			var b5 = d[2] > d[3] ? 1 : 0;

			var idx = (nb3 & b4) | (b2 & b5) | (((b0 & b3) | (b1 & b2)) << 1);

			error = d[idx];
			return idx;
		}

	}



	public struct ColorRgb565 : IEquatable<ColorRgb565>
	{
		public const int C565_5Mask = 0xF8;
		public const int C565_6Mask = 0xFC;


		public bool Equals(ColorRgb565 other)
		{
			return data == other.data;
		}

		public override bool Equals(object obj)
		{
			return obj is ColorRgb565 other && Equals(other);
		}

		public override int GetHashCode()
		{
			return data.GetHashCode();
		}

		public static bool operator ==(ColorRgb565 left, ColorRgb565 right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ColorRgb565 left, ColorRgb565 right)
		{
			return !left.Equals(right);
		}

		public const ushort RedMask = 0b11111_000000_00000;
		public const int RedShift = 11;
		public const ushort GreenMask = 0b00000_111111_00000;
		public const int GreenShift = 5;
		public const ushort BlueMask = 0b00000_000000_11111;

		public ushort data;

		public byte R
		{
			get
			{
				var r5 = (data & RedMask) >> RedShift;
				return (byte)((r5 << 3) | (r5 >> 2));
			}
			set
			{
				var r5 = value >> 3;
				data = (ushort)(data & ~RedMask);
				data = (ushort)(data | (r5 << RedShift));
			}
		}

		public byte G
		{
			get
			{
				var g6 = (data & GreenMask) >> GreenShift;
				return (byte)((g6 << 2) | (g6 >> 4));
			}
			set
			{
				var g6 = value >> 2;
				data = (ushort)(data & ~GreenMask);
				data = (ushort)(data | (g6 << GreenShift));
			}
		}

		public byte B
		{
			get
			{
				var b5 = data & BlueMask;
				return (byte)((b5 << 3) | (b5 >> 2));
			}
			set
			{
				var b5 = value >> 3;
				data = (ushort)(data & ~BlueMask);
				data = (ushort)(data | b5);
			}
		}

		public int RawR
		{
			get { return (data & RedMask) >> RedShift; }
			set
			{
				if (value > 31) value = 31;
				if (value < 0) value = 0;
				data = (ushort)(data & ~RedMask);
				data = (ushort)(data | (value << RedShift));
			}
		}

		public int RawG
		{
			get { return (data & GreenMask) >> GreenShift; }
			set
			{
				if (value > 63) value = 63;
				if (value < 0) value = 0;
				data = (ushort)(data & ~GreenMask);
				data = (ushort)(data | (value << GreenShift));
			}
		}

		public int RawB
		{
			get { return data & BlueMask; }
			set
			{
				if (value > 31) value = 31;
				if (value < 0) value = 0;
				data = (ushort)(data & ~BlueMask);
				data = (ushort)(data | value);
			}
		}

		public ColorRgb565(byte r, byte g, byte b)
		{
			data = 0;
			R = r;
			G = g;
			B = b;
		}

		public ColorRgb565(Vector3 colorVector)
		{
			data = 0;
			R = ByteHelper.ClampToByte(colorVector.X * 255);
			G = ByteHelper.ClampToByte(colorVector.Y * 255);
			B = ByteHelper.ClampToByte(colorVector.Z * 255);
		}

		public Color ToColor()
        {
			return Color.FromArgb(255,R, G, B);
        }


		public override string ToString()
		{
			return $"r : {R} g : {G} b : {B}";
		}

	}

}
