using Microsoft.Xna.Framework;

namespace ThirdPartyNinjas.XnaUtility
{
	public class TextureRegion
	{
		public Rectangle Bounds { get; set; }
		public Vector2 OriginTopLeft { get; set; }
		public Vector2 OriginCenter { get; set; }
		public Vector2 OriginBottomRight { get; set; }
		public bool Rotated { get; set; }
	}
}
