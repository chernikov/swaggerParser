using NUnit.Framework;
using swaggerParser.Output;
using swaggerParser.Output.Base;
using swaggerParser.Output.Typescript;
using swaggerParser.Tests.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swaggerParser.Tests.Dart
{
    public class DartClassTests
    {
        [Test]
        public void CheckIntClass()
        {
            var classFactory = new TypescriptTypeWriter();
            var files = classFactory.GenerateFiles(new List<BaseType> { SimpleIntClass.ClassInstance });
            var file = files[0];

            Assert.AreEqual(file.FileName, "simple_int.class.dart");

            Assert.AreEqual(file.Content, @"
class SimpleInt {
  int id;

  SimpleInt() {
    this.id = 0;
  }
}
");
        }

        [Test]
        public void CheckStringClass()
        {
            var classFactory = new TypescriptTypeWriter();
            var files = classFactory.GenerateFiles(new List<BaseType> { SimpleStringClass.ClassInstance });
            var file = files[0];

            Assert.AreEqual(file.FileName, "simple-string.class.ts");

            Assert.AreEqual(file.Content, @"
class SimpleString {
  String id;

  SimpleString() {
    this.id = '';
  }
}
");
        }
    }
}

