using System.Xml;
using System.Xml.Serialization;

namespace SignAndEncyptTool.Utils;

internal class GenericXmlSerializer<T> where T : class
{
    public string Serialize(T obj)
    {
        XmlSerializer xsSubmit = new XmlSerializer(typeof(T));
        using (var sww = new StringWriter())
        {
            using (XmlTextWriter writer = new XmlTextWriter(sww) { Formatting = Formatting.Indented })
            {
                xsSubmit.Serialize(writer, obj);
                return sww.ToString();
            }
        }
    }
}
