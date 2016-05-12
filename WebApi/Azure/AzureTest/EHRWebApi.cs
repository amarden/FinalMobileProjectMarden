using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Azure.EhrAssets;
using Azure.DataObjects;

namespace AzureTest
{
    /// <summary>
    /// Class used for tests on the EHR service
    /// </summary>
    [TestClass]
    public class EHRWebApi
    {
        EHR ehr;
        [TestInitialize]
        public void Initialize()
        {
            this.ehr = new EHR(true);
        }

        [TestMethod]
        public void TestCheckBPStatusStable()
        {
            int systolic = 120;
            int diastolic = 80;
            string status = ehr.checkBpStatus(systolic, diastolic);
            Assert.AreEqual(status, "stable");
        }

        [TestMethod]
        public void TestCheckBPStatusCritical()
        {
            int systolic1 = 200;
            int diastolic1 = 80;
            string status1 = ehr.checkBpStatus(systolic1, diastolic1);

            int systolic2 = 120;
            int diastolic2 = 15;
            string status2 = ehr.checkBpStatus(systolic2, diastolic2);
            Assert.AreEqual(status1, "critical");
            Assert.AreEqual(status2, "critical");
        }

        [TestMethod]
        public void TestCheckBPStatusUnstable()
        {
            int systolic1 = 180;
            int diastolic1 = 80;
            string status1 = ehr.checkBpStatus(systolic1, diastolic1);

            int systolic2 = 120;
            int diastolic2 = 25;
            string status2 = ehr.checkBpStatus(systolic2, diastolic2);
            Assert.AreEqual(status1, "unstable");
            Assert.AreEqual(status2, "unstable");
        }


        [TestMethod]
        public void TestCheckOxygenStable()
        {
            int oxygen = 95;
            string status = ehr.checkOxygenStatus(oxygen);
            Assert.AreEqual(status, "stable");
        }


        [TestMethod]
        public void TestCheckOxygenUnstable()
        {
            int oxygen = 80;
            string status = ehr.checkOxygenStatus(oxygen);
            Assert.AreEqual(status, "unstable");
        }


        [TestMethod]
        public void TestCheckOxygenCritical()
        {
            int oxygen = 60;
            string status = ehr.checkOxygenStatus(oxygen);
            Assert.AreEqual(status, "critical");
        }


        [TestMethod]
        public void TestCheckGlucoseStable()
        {
            int glucose = 100;
            string status = ehr.checkGlucoseStatus(glucose);
            Assert.AreEqual(status, "stable");
        }


        [TestMethod]
        public void TestCheckGlucoseUnstable()
        {
            int glucose = 150;
            string status = ehr.checkGlucoseStatus(glucose);
            Assert.AreEqual(status, "unstable");
        }


        [TestMethod]
        public void TestCheckGlucoseCritical()
        {
            int glucose = 10;
            string status = ehr.checkGlucoseStatus(glucose);
            Assert.AreEqual(status, "critical");
        }

        [TestMethod]
        public void TestDeathModifierWithAllStables()
        {
            Biometric measure = new Biometric { Oxygen = 95, Systolic = 120, Diastolic = 80, Glucose = 100 };
            int modifierLOS1 = ehr.getDeathModifierCalculated(1, measure);
            int modifierLOS5 = ehr.getDeathModifierCalculated(5, measure);
            int modifierLOS10 = ehr.getDeathModifierCalculated(10, measure);

            Assert.AreEqual(modifierLOS1, -5);
            Assert.AreEqual(modifierLOS5, -1);
            Assert.AreEqual(modifierLOS10, 4);
        }

        [TestMethod]
        public void TestDeathModifierWithAllUnstables()
        {
            Biometric measure = new Biometric { Oxygen = 80, Systolic = 180, Diastolic = 80, Glucose = 25 };
            int modifierLOS1 = ehr.getDeathModifierCalculated(1, measure);
            int modifierLOS5 = ehr.getDeathModifierCalculated(5, measure);
            int modifierLOS10 = ehr.getDeathModifierCalculated(10, measure);

            Assert.AreEqual(modifierLOS1, 7);
            Assert.AreEqual(modifierLOS5, 11);
            Assert.AreEqual(modifierLOS10, 16);
        }

        [TestMethod]
        public void TestDeathModifierWithAllCriticals()
        {
            Biometric measure = new Biometric { Oxygen = 60, Systolic = 200, Diastolic =80, Glucose = 300 };
            int modifierLOS1 = ehr.getDeathModifierCalculated(1, measure);
            int modifierLOS5 = ehr.getDeathModifierCalculated(5, measure);
            int modifierLOS10 = ehr.getDeathModifierCalculated(10, measure);

            Assert.AreEqual(modifierLOS1, 31);
            Assert.AreEqual(modifierLOS5, 35);
            Assert.AreEqual(modifierLOS10, 40);
        }
    }
}
