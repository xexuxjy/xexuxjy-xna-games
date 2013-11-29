using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ThirdPartyNinjas.XnaUtility
{
	public class TextureAtlasReader : ContentTypeReader<TextureAtlas>
	{
		protected override TextureAtlas Read(ContentReader input, TextureAtlas existingInstance)
		{
			Dictionary<string, TextureRegion> regions = input.ReadObject<Dictionary<string, TextureRegion>>();
			return new TextureAtlas(regions);
		}
	}
}
