using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OLinqProvider;

namespace OLinqTests
{
    [TestClass]
    public class QueryTest
    {

        [TestMethod]
        public void SimpleWhereTest()
        {

            var odata = new OProvider(
                "http://odata.netflix.com/Catalog", "results");

            var titles = odata.CreateQuery<Entry>("Titles");


            var result = titles.Where(x => x.Name == "'The Name of the Rose'").ToList();
            Assert.AreEqual(result.Count, 1);
        }
        [TestMethod]
        public void SimpleWhereAndTest()
        {

            var odata = new OProvider(
                "http://odata.netflix.com/Catalog", "results");
  

            var titles = odata.CreateQuery<Entry>("Titles");
       

            var result = titles.Where(x => x.Name == "'The Name of the Rose'" && x.AverageRating==3.6).ToList();
            Assert.AreEqual(result.Count, 1);
        }
        [TestMethod]
        public void WhereMultipleAndTest()
        {

            var odata = new OProvider(
                "http://odata.netflix.com/Catalog", "results");
  

            var titles = odata.CreateQuery<Entry>("Titles");
      

            var result = titles.Where(x => x.Name == "'The Name of the Rose'" && x.AverageRating == 3.6 && x.ReleaseYear == 1986 && x.ShortName == "'The Name of the Rose'").ToList();
            Assert.AreEqual(result.Count, 1);
        }
        [TestMethod]
        public void SimpleWhereOrTest()
        {

            var odata = new OProvider(
                "http://odata.netflix.com/Catalog", "results");

            var titles = odata.CreateQuery<Entry>("Titles");
    

            var result = titles.Where(x => x.Name == "'The Name of the Rose'" || x.AverageRating == 3.6).ToList();
            Assert.AreEqual(result.Any(), true);
        }
        [TestMethod]
        public void WhereMultipleOrTest()
        {

            var odata = new OProvider(
                "http://odata.netflix.com/Catalog", "results");

            var titles = odata.CreateQuery<Entry>("Titles");
    

            var result = titles.Where(x => x.Name == "'The Name of the Rose'" || x.AverageRating == 3.6 || x.ShortName=="'The Name of the Rose'").ToList();
            Assert.AreEqual(result.Any(), true);
        }
        [TestMethod]
        public void SimpleWhereOrAndTest()
        {

            var odata = new OProvider(
                "http://odata.netflix.com/Catalog", "results");

            var titles = odata.CreateQuery<Entry>("Titles");


            var result = titles.Where(x => x.Name == "'The Name of the Rose'" || x.AverageRating == 3.6 && x.ReleaseYear==1986).ToList();
            Assert.AreEqual(result.Any(), true);
        }
        [TestMethod]
        public void FirstTest()
        {

            var odata = new OProvider(
                "http://odata.netflix.com/Catalog");

            var titles = odata.CreateQuery<Entry>("Titles");


            var result = titles.First(x => x.AverageRating > 3);
           
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void EndsWithTest()
        {

            var odata = new OProvider(
                "http://odata.netflix.com/Catalog", "results");

            var titles = odata.CreateQuery<Entry>("Titles");


            var result = titles.Where(x => x.Name.EndsWith("Rose")).ToList();

            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void StartsWithTest()
        {

            var odata = new OProvider(
                "http://odata.netflix.com/Catalog", "results");

            var titles = odata.CreateQuery<Entry>("Titles");


            var result = titles.Where(x => x.Name.StartsWith("The Name")).ToList();

            Assert.IsTrue(result.Count > 0);
        }
        [TestMethod]
        public void LengthTest()
        {

            var odata = new OProvider(
                "http://odata.netflix.com/Catalog", "results");

            var titles = odata.CreateQuery<Entry>("Titles");


            var result = titles.Where(x => x.Name.Length() > 165).ToList();

            Assert.IsTrue(result.Any());
        }
        [TestMethod]
        public void ContainsTest()
        {

            var odata = new OProvider(
                "http://odata.netflix.com/Catalog", "results");

            var titles = odata.CreateQuery<Entry>("Titles");


            var result = titles.Where(x => x.Name.Contains("Rose")).ToList();

            Assert.IsTrue(result.Any());
        }
        [TestMethod]
        public void ToUpperTest()
        {

            var odata = new OProvider(
                "http://odata.netflix.com/Catalog", "results");

            var titles = odata.CreateQuery<Entry>("Titles");


            var result = titles.Where(x => x.Name.ToUpper() == "'THE NAME OF THE ROSE'").ToList();

            Assert.IsTrue(result.Any());
        }
        [TestMethod]
        public void ToLowerTest()
        {

            var odata = new OProvider(
                "http://odata.netflix.com/Catalog", "results");

            var titles = odata.CreateQuery<Entry>("Titles");


            var result = titles.Where(x => x.Name.ToLower() == "'the name of the rose'").ToList();

            Assert.IsTrue(result.Any());
        }
        [TestMethod]
        public void OrderByTest()
        {

            var odata = new OProvider(
                "http://odata.netflix.com/Catalog", "results");

            var titles = odata.CreateQuery<Entry>("Titles");


            var result = titles.Where(x => x.AverageRating == 3.6).OrderBy(x=>x.Name).ToList();

            Assert.IsTrue(result.Any());
        }
        [TestMethod]
        public void OrderByDescTest()
        {

            var odata = new OProvider(
                "http://odata.netflix.com/Catalog", "results");

            var titles = odata.CreateQuery<Entry>("Titles");


            var result = titles.Where(x => x.AverageRating == 3.6).OrderByDescending(x => x.Name).ToList();

            Assert.IsTrue(result.Any());
        }
        [TestMethod]
        public void SkipTest()
        {

            var odata = new OProvider(
                "http://odata.netflix.com/Catalog", "results");

            var titles = odata.CreateQuery<Entry>("Titles");


            var result = titles.Where(x => x.AverageRating == 3.6).Skip(100).ToList();

            Assert.IsTrue(result.Any());
        }
        [TestMethod]
        public void TakeTest()
        {

            var odata = new OProvider(
                "http://odata.netflix.com/Catalog");

            var titles = odata.CreateQuery<Entry>("Titles");


            var result = titles.Where(x => x.AverageRating == 3.6).Take(100).ToList();

            Assert.AreEqual(result.Count, 100);
        }
        [TestMethod]
        public void SkipTakeTest()
        {

            var odata = new OProvider(
                "http://odata.netflix.com/Catalog");

            var titles = odata.CreateQuery<Entry>("Titles");


            var result = titles.Where(x => x.AverageRating == 3.6).Skip(100).Take(100).ToList();

            Assert.AreEqual(result.Count, 100);
        }
        [TestMethod]
        public void YearTest()
        {

            var odata = new OProvider("http://odata.netflix.com/Catalog", "results");

            var titles = odata.CreateQuery<Entry>("Titles");


            var result = titles.Where(x => x.DateModified.YearCompare()==2012).ToList();

            Assert.IsTrue(result.Any());
        }
    }
}
