using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace LanguageLearningTool.Models
{
    public class QuizDataReader
    {
        public string CreateQuizDataXml(GrammarQuizRoot root)
        {
            var serializer = new DataContractSerializer(typeof(GrammarQuizRoot));
            using (var stringWriter = new StringWriter()) {
                using (var xmlWriter = new XmlTextWriter(stringWriter)) {
                    serializer.WriteObject(xmlWriter, root);
                    return stringWriter.ToString();
                }
            }
        }

        public GrammarQuizRoot GetQuizData()
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("LanguageLearningTool.Resources." + "test.xml")) {
                if (stream == null) {
                    throw new InvalidOperationException("Failed to load quiz data from xml resource.");
                }
                using (var xmlReader = XmlReader.Create(stream)) {
                    var serializer = new DataContractSerializer(typeof(GrammarQuizRoot));
                    return (GrammarQuizRoot)serializer.ReadObject(xmlReader);
                }
            }
        }
    }
}
