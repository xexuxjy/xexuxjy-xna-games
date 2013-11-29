using Microsoft.Xna.Framework.Content.Pipeline;

namespace ThirdPartyNinjas.XnaUtility.Content
{
	[ContentImporter(".json", ".taxml", DisplayName = "Texture Atlas Importer", DefaultProcessor = "TextureAtlasProcessor")]
	public class TextureAtlasImporter : ContentImporter<TextureAtlasContent>
	{
		public override TextureAtlasContent Import(string fileName, ContentImporterContext context)
		{
			return new TextureAtlasContent(fileName);
		}
	}
}
