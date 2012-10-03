using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using ImageNexus.BenScharbach.Common.PasswordGenerator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestPasswordGenerator
{
    
    
    /// <summary>
    ///This is a test class for PasswordGeneratorTest and is intended
    ///to contain all PasswordGeneratorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PasswordGeneratorTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion
        
        /// <summary>
        ///A test to Generate a password.
        ///</summary>
        [TestMethod()]
        public void GenerateTest()
        {
            const int passwordLength = 75; 
            var target = new PasswordGenerator(passwordLength) {UseSymbolSet = false, UseAlphaUppercaseSet = true, UseAlphaLowercaseSet =  true, UseNumericSet =  true};
            const int expected = 75;

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            string password = target.Generate();

            stopWatch.Stop();

            MessageBox.Show(string.Format("Elapsed time is {0}", stopWatch.ElapsedMilliseconds));

            Assert.AreEqual(expected, 75);
            
        }

        /// <summary>
        ///A test to Generate a BATCH of passwords.
        ///</summary>
        [TestMethod()]
        public void GenerateBatchTest()
        {
            const int batchSize = 525000;
            var batchOfPasswords = new List<string>(batchSize);

            const int passwordLength = 75;
            var target = new PasswordGenerator(passwordLength)
            {
                UseSymbolSet = false,
                UseAlphaUppercaseSet = true,
                UseAlphaLowercaseSet = true,
                UseNumericSet = true
            };

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            for (int i = 0; i < batchSize; i++)
            {
                batchOfPasswords.Add(target.Generate());
            }

            stopWatch.Stop();

            MessageBox.Show(string.Format("Elapsed time is {0}", stopWatch.ElapsedMilliseconds));

            Assert.AreEqual(true, true);
        }

        /// <summary>
        ///A test to Generate a password using parallel call.
        ///</summary>
        [TestMethod()]
        public void GenerateParallelTest()
        {
            const int passwordLength = 75;
            var target = new PasswordGenerator(passwordLength) { UseSymbolSet = false, UseAlphaUppercaseSet = true, UseAlphaLowercaseSet = true, UseNumericSet = true };
            const int expected = 75;

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var password = target.GenerateParallel(1);

            stopWatch.Stop();

            MessageBox.Show(string.Format("Elapsed time is {0}", stopWatch.ElapsedMilliseconds));

            Assert.AreEqual(expected, 75);
        }

        /// <summary>
        ///A test to Generate a BATCH of passwords using parallel call.
        ///</summary>
        [TestMethod()]
        public void GenerateParallelBatchTest()
        {
            const int batchSize = 525000;
            List<string> batchOfPasswords = null;

            const int passwordLength = 75;
            var target = new PasswordGenerator(passwordLength) { UseSymbolSet = false, UseAlphaUppercaseSet = true, UseAlphaLowercaseSet = true, UseNumericSet = true };
            const int expected = 75;

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            batchOfPasswords = target.GenerateParallel(batchSize);

            stopWatch.Stop();

            MessageBox.Show(string.Format("Elapsed time is {0}", stopWatch.ElapsedMilliseconds));

            Assert.AreEqual(true, true);
        }
    }
}
