// Thanks to MilkstoneStudios for example : http://www.milkstonestudios.com/2009/03/plain-xml-content-in-xna-content-pipeline/
using System;
using System.Collections.Generic;
using System.Text;

namespace Barrage.Content
{
    /// <summary>
    /// This is the importer for a shader.
    /// </summary>
    public class XmlSource: IDisposable
    {
        public XmlSource(string xmlCode)
        {
            this.xmlCode = xmlCode;
        }

        private string xmlCode;
        public string XmlCode { get { return xmlCode; } }

        #region Miembros de IDisposable

        public void Dispose()
        {
            xmlCode = null;
        }

        #endregion
    }
}
