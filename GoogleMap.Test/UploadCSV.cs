using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Xunit;

namespace GoogleMap.Test
{
    public static class UploadCSVExtension
    {
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return $"{input.First().ToString().ToUpper()}{input.Substring(1)}";
            }
        }
        public static string[] GetDataArray(this string line)
        {
            return line.Split(',').Select(x => x.FirstCharToUpper()).ToArray();
        }
    }

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

        [Fact(DisplayName = "處理不動產實價登錄網下載資料-全國")]
        public void ParseDownloadData()
        {
            string downloadDirectory = $@"C:\Data_ALL\";
            string[] downFilesA = Directory.GetFiles(downloadDirectory, "*_a*.xml");
            string[] downFilesB = Directory.GetFiles(downloadDirectory, "*_b*.xml");
            string[] downFilesC = Directory.GetFiles(downloadDirectory, "*_c*.xml");

            string filePath = @"C:\XMLData\a_lvr_land_a.xml";
            //---
            string xmlFile = File.ReadAllText(filePath);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFile);
            XmlElement documentElement = doc.DocumentElement;
            if (documentElement != null)
            {
                XmlNodeList xnList = documentElement.ChildNodes;
                List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
                if (xnList.Count > 0)
                {
                    foreach (XmlNode xn in xnList)
                    {
                        XmlNode item = xn;
                        Dictionary<string, string> keyValue = new Dictionary<string, string>();
                        foreach (XmlElement e in item)
                        {
                            string name = e.Name;
                            string value = e.InnerText;
                            keyValue.Add(name, value);
                        }
                        result.Add(keyValue);
                    }
                }

            }
        }

        [Fact(DisplayName = "Parse 不動產實價登錄網下載資料-清單")]
        public void ParseDownloadList()
        {
            string mainDirectory = @"C:\Data_ALL_CSV\";
            LVRMain downloadList = GetMainCSV($@"{mainDirectory}manifest.csv");
            downloadList.FileList.ForEach(d =>
            {
                string lvrFileName = d.Name;
                string lvrSchema = d.Schema;
                string lvrDescription = d.Description;
                bool isXML = d.ExtName == "xml";
                if (isXML)
                {

                    List<Dictionary<string, string>> xmlData = GetXMLData($@"{mainDirectory}{lvrFileName}");
                }
                else
                {
                    List<Dictionary<string, string>> schemaData = GetSchemaCSV($@"{mainDirectory}{lvrSchema}");
                    List<Dictionary<string, string>> xmlData = GetCSVData($@"{mainDirectory}{lvrFileName}", schemaData);
                }
            });
        }

        private static List<Dictionary<string, string>> GetXMLData(string path)
        {
            string filePath = path;
            //---
            string xmlFile = File.ReadAllText(filePath);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlFile);
            XmlElement documentElement = doc.DocumentElement;
            if (documentElement != null)
            {
                XmlNodeList xnList = documentElement.ChildNodes;
                List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
                if (xnList.Count > 0)
                {
                    foreach (XmlNode xn in xnList)
                    {
                        XmlNode item = xn;
                        Dictionary<string, string> keyValue = new Dictionary<string, string>();
                        foreach (XmlElement e in item)
                        {
                            string name = e.Name;
                            string value = e.InnerText;
                            keyValue.Add(name, value);
                        }
                        result.Add(keyValue);
                    }
                }
                return result;
            }
            return new List<Dictionary<string, string>>();
        }

        private static List<Dictionary<string, string>> GetCSVData(string path, List<Dictionary<string, string>> header)
        {
            string[] lines = File.ReadAllLines(path);
            string[] tcHeader = lines[0].Split(',');
            string[] engHeader = lines[1].Split(',');
            var csv = new List<Dictionary<string, string>>();
            for (int i = 2; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] values = line.Split(',');
                var item = new Dictionary<string, string>();
                for (int j = 0; j < values.Length; j++)
                {
                    string key = header[j]["Name"];
                    string value = values[j];
                    item.Add(key, value);
                }
                csv.Add(item);
            }
            return csv;
        }

        private LVRMain GetMainCSV(string path)
        {
            //string filePath = $@"C:\Data_ALL\manifest.csv";
            List<string> fileData = GetCSVLine(path);
            string[] columnHeader = GetCSVHeader(path);
            fileData.RemoveAt(0);
            List<Dictionary<string, string>> dictList = new List<Dictionary<string, string>>();
            fileData.ForEach(d =>
            {
                string[] dataLine = d.Split(',');
                Dictionary<string, string> dictItem = new Dictionary<string, string>();
                for (int i = 0; i < columnHeader.Length; i++)
                {
                    string key = columnHeader[i];
                    string value = dataLine[i];
                    dictItem.Add(key, value);
                }
                dictList.Add(dictItem);
            });
            var jsonObject = new { FileList = dictList };
            var jsonString = JsonConvert.SerializeObject(jsonObject);
            var jsonMain = JsonConvert.DeserializeObject<LVRMain>(jsonString);
            return jsonMain;
        }

        private List<Dictionary<string, string>> GetSchemaCSV(string path)
        {
            List<string> fileData = GetCSVLine(path);
            string[] columnHeader = GetCSVHeader(path);
            fileData.RemoveAt(0);
            List<Dictionary<string, string>> dictList = new List<Dictionary<string, string>>();
            fileData.ForEach(d =>
            {
                string[] dataLine = d.Split(',');
                Dictionary<string, string> dictItem = new Dictionary<string, string>();
                for (int i = 0; i < columnHeader.Length; i++)
                {
                    string key = columnHeader[i];
                    string value = dataLine[i];
                    dictItem.Add(key, value);
                }
                dictList.Add(dictItem);
            });
            return dictList;
        }

        private string[] GetCSVHeader(string path)
        {
            string filePath = path;
            List<string> fileData = GetCSVLine(filePath);
            string[] columnHeader = fileData[0].GetDataArray();
            return columnHeader;
        }
        /// <summary>
        /// Get single Line From CSV File
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private List<string> GetCSVLine(string filePath)
        {
            List<string> fileData = new List<string>();
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    fileData.Add(line);
                }
            }
            return fileData;
        }

        public struct LVRMain
        {
            public List<LVRFile> FileList { get; set; }
        }
        public struct LVRFile
        {
            public string Name { get; set; }
            public string ExtName => GetExtName(Name);
            public string Schema { get; set; }
            public string Description { get; set; }

            private string GetExtName(string name)
            {
                return name.Split('.').Last().ToLower();
            }
        }

    }
}
