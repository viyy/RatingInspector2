using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Services
{
    public static class SimpleXmlStream
    {
        public static IEnumerable<XElement> SimpleStreamAxis(string inputUrl, string matchName)
        {
            using (var reader = XmlReader.Create(inputUrl))
            {
                reader.MoveToContent(); // will not advance reader if already on a content node; if successful, ReadState is Interactive
                reader.Read(); // this is needed, even with MoveToContent and ReadState.Interactive
                while (!reader.EOF && reader.ReadState == ReadState.Interactive)
                    // corrected for bug noted by Wes below...
                    if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals(matchName))
                    {
                        // this advances the reader...so it's either XNode.ReadFrom() or reader.Read(), but not both
                        if (XNode.ReadFrom(reader) is XElement matchedElement)
                            yield return matchedElement;
                    }
                    else
                    {
                        reader.Read();
                    }
            }
        }
    }
}