using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ThirdPartyNinjas.XnaUtility.Content
{
	[ContentSerializerRuntimeType("ThirdPartyNinjas.XnaUtility.TextureRegion, ThirdPartyNinjas.XnaUtility")]
	public class TextureRegionContent
	{
		public Rectangle Bounds { get; set; }
		public Vector2 OriginTopLeft { get; set; }
		public Vector2 OriginCenter { get; set; }
		public Vector2 OriginBottomRight { get; set; }
		public bool Rotated { get; set; }
	}
}
