using System.Dynamic;
using FastExpressionEngine;
using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace UnitTest
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
            
            var bre = new ExpressionEngine();

            dynamic datas = new ExpandoObject();
            datas.count = 1;
            datas.name = "avqqq";
            var inputs = new dynamic[]
            {
                datas
            };

            var resultList =
                bre.Execute("count < 3 AND name.Contains(\"av\") AND name.StartsWith(\"av\")", inputs);

            var resultListIsSuccess = resultList.IsSuccess;
            That(resultListIsSuccess, Is.True);
        }
        
        [Test]
        public void Test2()
        {
            
            var bre = new ExpressionEngine();

            dynamic datas = new ExpandoObject();
            datas.count = 1;
            datas.name = "avqqq";
            var inputs = new dynamic[]
            {
                datas
            };

            var resultList =
                bre.Execute("input1.count < 3", inputs);

            var resultListIsSuccess = resultList.IsSuccess;
            That(resultListIsSuccess, Is.True);
        }
    }
}