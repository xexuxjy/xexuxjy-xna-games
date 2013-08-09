using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using System.Xml;

namespace Barrage.Content
{
    /// <summary>
    /// This class will be instantiated by the XNA Content Pipeline to
    /// apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same
    /// if the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// </summary>
    [ContentProcessor(DisplayName = "XML Document Processor")]
    class XmlDocumentProcessor : ContentProcessor<XmlSource, XmlDocument>
    {
        public override XmlDocument Process(XmlSource input, ContentProcessorContext context)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(input.XmlCode);
            return doc;
        }
    }
}
