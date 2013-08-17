﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SetFontMetrics
{
    public static class ObjectExtensions
    {
        public static string ToXml(this object o)
        {
            if (o == null) return "";
            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer xs = new XmlSerializer(o.GetType());
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            xs.Serialize(xmlTextWriter, o);
            memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
            return new UTF8Encoding().GetString(memoryStream.ToArray());
        }


        public static T FromXML<T>(this string str)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            MemoryStream memoryStream = new MemoryStream(new UTF8Encoding().GetBytes(str));
            //XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            return (T)xs.Deserialize(memoryStream);
        }



    }
}
