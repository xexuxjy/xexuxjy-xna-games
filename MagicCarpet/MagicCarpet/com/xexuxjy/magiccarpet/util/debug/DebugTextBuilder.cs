using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.IO;
using BulletXNA;

namespace com.xexuxjy.magiccarpet.util.debug
{
    /// <summary>
    ///  Provides a friendlier way of building the xml tree node string for debug info
    /// </summary>
    public class DebugTextBuilder
    {
        public DebugTextBuilder()
        {
            m_resultStringBuilder = new StringBuilder();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void openNode(String nodeText)
        {
            if (Globals.DebugTextEnabled)
            {
                m_resultStringBuilder.Append("<");
                m_resultStringBuilder.Append(Globals.DebugXMLNodeTag);
                m_resultStringBuilder.Append(" ");
                m_resultStringBuilder.Append(Globals.DebugXMLNodeText);
                m_resultStringBuilder.Append("=\"");
                m_resultStringBuilder.Append(nodeText);
                m_resultStringBuilder.Append("\" >");
                m_nodeCount++;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void closeNode()
        {
            if (Globals.DebugTextEnabled)
            {
                m_resultStringBuilder.Append("</");
                m_resultStringBuilder.Append(Globals.DebugXMLNodeTag);
                m_resultStringBuilder.Append(">");
                m_nodeCount--;
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        /// <summary>
        /// Allow insertion of 'raw' text without tags, this is for inserting text from other debugtextbuilders.
        /// NB - Think about all this stuff again as it's getting untidy / unwieldy.
        /// </summary>
        /// <param name="text"></param>
        public void setNodeTextRaw(String text)
        {
            if (Globals.DebugTextEnabled)
            {
                m_resultStringBuilder.Append(text);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public String getFullText()
        {
            if (Globals.DebugTextEnabled)
            {
                Debug.Assert(m_nodeCount == 0, "Nodes not closed properly");
                return m_resultStringBuilder.ToString();
            }
            return "";
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        // arse about face way of doing it but convert the structured data into a series of tabs and carriage returns
        public String getFullTextPlain()
        {
            if (Globals.DebugTextEnabled)
            {

                // simple parser to convert the object debug string into a set of more structured info
                StringBuilder outputString = new StringBuilder();

                MemoryStream memoryStream = new MemoryStream(System.Text.Encoding.Default.GetBytes(getFullText()));
                XmlTextReader xmlReader = new XmlTextReader(memoryStream);
                int tabDepth = 0;

                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        if (xmlReader.LocalName.Equals(Globals.DebugXMLNodeTag))
                        {
                            // put the tree node on the stack with it's data
                            String infoText = xmlReader.GetAttribute(Globals.DebugXMLNodeText);
                            for (int i = 0; i < tabDepth; ++i)
                            {
                                // doesn't seem to work with tab's
                                //outputString.Append("\t");
                                outputString.Append("    ");
                            }
                            outputString.Append(infoText);
                            outputString.Append("\n");
                            ++tabDepth;
                        }
                    }
                    else if (xmlReader.NodeType == XmlNodeType.EndElement)
                    {
                        if (xmlReader.LocalName.Equals(Globals.DebugXMLNodeTag))
                        {
                            --tabDepth;
                        }
                    }
                }
                return outputString.ToString();
            }
            return "";
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void reset()
        {
            m_resultStringBuilder.Remove(0, m_resultStringBuilder.Length);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        private int m_nodeCount = 0;
        private StringBuilder m_resultStringBuilder;
    }
}
