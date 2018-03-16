using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseORMGenerator.Internal;

namespace DatabaseORMGenerator.Tests
{
    [TestClass]
    public class TSQLSchemaRepositoryTests
    {
        [TestMethod]
        public void GetSchemaFromMSSQLServer()
        {
            var t_ConnectionString = "Server=localhost;Database=Test;Trusted_Connection=True;";
            var t_Repo = new TSQLSchemaRepository(t_ConnectionString);
            var t_Schema = t_Repo.GetSchema();

            Assert.IsNotNull(t_Schema);
        }
    }
}
