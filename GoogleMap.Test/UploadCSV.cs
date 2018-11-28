using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Xunit;

namespace GoogleMap.Test
{
    
    public class UploadCSV
    {
        [Fact(DisplayName = "CSV2JSON")]
        public void CSV2JSON()
        {
            string[] lines = System.IO.File.ReadAllLines(@"C:\NewData\a_lvr_land_a.csv");
            string[] tcHeader = lines[0].Split(',');
            string[] engHeader = lines[1].Split(',');
            List<string[]> csv = new List<string[]>();
            for (int i = 2; i < lines.Length; i++)
            {
                string line = lines[i];
                csv.Add(line.Split(','));
            }
            string json = JsonConvert.SerializeObject(csv);
        }

        [Fact(DisplayName = "XML2JSON")]
        public void XML2Json()
        {
            string xmlFile = System.IO.File.ReadAllText(@"C:\XMLData\a_lvr_land_a.xml");
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFile);
            XmlNodeList xnList = doc.SelectNodes("/lvr_land/買賣");
            var result = new List<Dictionary<string,string>>();
            foreach (XmlNode xn in xnList)
            {
                XmlNode item = xn;
                Dictionary<string, string> keyValue = new Dictionary<string,string>();
                foreach (XmlElement e in item)
                {
                    XmlElement ele = e;
                    var name = ele.Name;
                    var value = ele.InnerText;
                    keyValue.Add(name,value);
                }
                result.Add(keyValue);
            }
            string jsonDict = JsonConvert.SerializeObject(result);
        }
    }
}
