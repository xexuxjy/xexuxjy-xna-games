using Microsoft.Xna.Framework.Content.Pipeline;

namespace ThirdPartyNinjas.XnaUtility.Content
{
	[ContentProcessor(DisplayName = "Texture Atlas Processor")]
	public class TextureAtlasProcessor : ContentProcessor<TextureAtlasContent, TextureAtlasContent>
	{
		public override TextureAtlasContent Process(TextureAtlasContent input, ContentProcessorContext context)
		{
			return input;
		}
	}
}
