using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Barrage.Content
{
    /// <summary>
    /// This class will be instantiated by the XNA Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    /// 
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentImporter(".xml", 
          DisplayName = "Barrage Xml Source Importer")]
    class XmlSourceImporter : ContentImporter<XmlSource>
    {
        public override XmlSource Import(string filename, ContentImporterContext context)
        {
            string sourceCode = System.IO.File.ReadAllText(filename);
            return new XmlSource(sourceCode);
        }
    }

}
