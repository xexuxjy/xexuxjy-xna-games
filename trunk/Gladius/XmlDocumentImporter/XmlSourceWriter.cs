using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System.Xml;
using System.IO;

namespace Barrage.Content
{
    /// <summary>
    /// This class will be instantiated by the XNA Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    class XmlSourceWriter : ContentTypeWriter<XmlSource>
    {
        protected override void Write(ContentWriter output, XmlSource value)
        {
            /*StringWriter sw=new StringWriter();
            value.Save(sw);
            string content = sw.ToString();*/
            output.Write(value.XmlCode);
        }
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(XmlDocument).AssemblyQualifiedName;
        }
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(XmlDocumentReader).AssemblyQualifiedName;
        }
    }
}
