////////////////////////////////////////////////////////////////////////////////
//
//    Solution Constants
//
////////////////////////////////////////////////////////////////////////////////
using Impinj.OctaneSdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Web.Script.Serialization;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper;
namespace impinjR420
{
    public static class SolutionConstants
    {
        public const string refepc = "E200 4A1D 66BD CE71 36B1 C739";
        public const string ReaderHostname = "192.168.0.199";
        //StreamReader sr = new StreamReader(filePath);
        // StreamWriter textWriter = new StreamWriter(@"test.csv");
        // CsvWriter csv = new CsvWriter(textWriter);
    }

    public class SensorParams
    {
        public static int count = 2;
        public static ulong threshold = 500000;
        public static string[] epcs = new string[] { "E200 5147 960D 0170 1620 717A", "E200 4000 770E 0069 1760 5EE7" };
        public static ulong[] LST = new ulong[count];
        public static int[] states = new int[count] ;
}

    public class TagReportCSV
    {
        //tag report to write into the csv file
        public TagData epc { get; set; }
        public ImpinjTimestamp FirstSeenTime { get; set; }
        public double PeakRSSI { get; set; }
        public double PhaseAngle { get; set; }
        public double DopplerFreq { get; set; }

    }
    public class TestCSV
    {
        //tag report to write into the csv file
        public string first { get; set; }
        public string second { get; set; }
        public string third { get; set; }
        public string fourth { get; set; }
        public string fifth { get; set; }

        public override string ToString()
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(this);
        }

    }

}