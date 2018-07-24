using System;
using Newtonsoft;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using AggregateGDPPopulation;
using Newtonsoft.Json.Linq;

namespace AggregateGDPPopulation.Tests
{
    public class AggregateDataTests
    {
        [Fact]
        public void ShouldBeSameAsExpectedOutput()
        {
            Class1 aggregateData = new Class1();
            aggregateData.AggregateData();
            string actualContent = "";
            using (StreamReader reader = new StreamReader(@"../../../../AggregateGDPPopulation.Tests/output/output.json"))
            {
                actualContent = reader.ReadToEnd();
            }

            string ExpectedContent = "";
            using (StreamReader reader = new StreamReader(@"../../../expected-output.json"))
            {
                ExpectedContent = reader.ReadToEnd();
            }
            ExpectedContent = ExpectedContent.Replace("\n", "").Replace("\t", "").Replace(" ", "");
            actualContent = actualContent.Replace("\n", "").Replace("\t", "").Replace(" ", "");
            Assert.Equal(ExpectedContent, actualContent);
        }
    }
}
