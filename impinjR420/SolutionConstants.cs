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
        //public const string ReaderHostname = "SpeedwayR-11-8A-24";
        public const string ReaderHostname = "192.168.0.199";
        public const string tagfilter = "E2004000770E";
        public const string csvpath = "../../TH_3_";
        //StreamReader sr = new StreamReader(filePath);
        // StreamWriter textWriter = new StreamWriter(@"test.csv");
        // CsvWriter csv = new CsvWriter(textWriter);
    }



    public class TagReportCSV
    {
        //tag report to write into the csv file
        public string epc { get; set; }
        public ulong FirstSeenTime { get; set; }
        public ulong LastSeenTime { get; set; }
        public double Channel { get; set; }
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
        public string sixth { get; set; }
        public string seventh { get; set; }

        public override string ToString()
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(this);
        }

    }

}