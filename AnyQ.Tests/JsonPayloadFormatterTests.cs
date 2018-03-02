using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace AnyQ.Formatters.Tests {
    [TestClass]
    public class JsonPayloadFormatterTests {
        private class TestData {
            public string Foo { get; set; }
        }

        [TestMethod]
        public void Read_After_Write_Yields_Same_Data() {
            var encoding = Encoding.UTF8;
            var jsonBytes = encoding.GetBytes("{\"Foo\":\"bar\"}");
            var obj = new TestData {
                Foo = "bar"
            };

            var sut = new JsonPayloadFormatter();

            var toRead = sut.Write(obj);
            CollectionAssert.AreEqual(toRead, jsonBytes);
            var read = sut.Read<TestData>(jsonBytes);
            Assert.AreEqual(read.Foo, obj.Foo);
        }
    }
}