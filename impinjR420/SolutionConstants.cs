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
        public const string csvpath = "./";
        public const int antenna = 2;
        //StreamReader sr = new StreamReader(filePath);
        // StreamWriter textWriter = new StreamWriter(@"test.csv");
        // CsvWriter csv = new CsvWriter(textWriter);
    }

    public class SensorParams
    {
        public static int count = 5;

        public static ulong tdif = 283000000-00000;
        public static ulong threshold = 5000;
        //public static string[] epcs = new string[] { "E200 5147 960D 0170 1620 717A", "E200 4000 770E 0069 1760 5EE7", "E200 7CC5 ABCC 6971 3EDB 81A5", "E200 4000 770E 0069 1760 5EE7" };
        //public static string[] epcs = new string[] { "E200 5147 960D 0170 1490 7CB7", "E200 5147 960D 0170 1590 73BA"};
        //public static string[] epcs = new string[] { "E200 5147 960D 0170 1300 956A", "E200 5147 960D 0170 1310 9332", "E200 5147 960D 0170 1420 87FA", "E200 5147 960D 0170 1610 6F3E" };
        //public static string[] epcs = new string[] { "E200 5147 960D 0170 1590 73BA", "E200 5147 960D 0170 1610 6F3E", "E200 4A1D 66BD CCB1 36B1 C732" };
        public static string[] epcs = new string[] {SolutionConstants.refepc, "E200 5147 960D 0170 1490 7CB7", "E200 4000 770E 0069 1660 6C24", "E200 5147 960D 0170 1620 717A", "E200 4000 770E 0069 1760 5EE7" };//cabinet, coffeemaker,  fridge, door
        public static string[] names = new string[] {"Ref", "Cabinet", "Coffee Maker", "Light Switch","Door" };//cabinet, coffeemaker,  fridge
        public static ulong[] LST = new ulong[count];
        public static int[] states = new int[count] ;
        public static int[] laststate = new int[count];
        public static int[] edge = new int[] { -1,-1,1};
    }

    public class TagReportCSV
    {
        //tag report to write into the csv file
        //public TagData epc { get; set; }
        public int sensor_num { get; set; }
        public string sensor_name { get; set; }
        //public ulong RefTime { get; set; }
        public ulong LastSeenTime { get; set; }
        public int state { get; set; }
       // public double PeakRSSI { get; set; }
        //public double PhaseAngle { get; set; }
       // public double DopplerFreq { get; set; }

    }
    public class TestCSV
    {
        //tag report to write into the csv file
        public string first { get; set; }
        public string second { get; set; }
        public string third { get; set; }
        public string fourth { get; set; }
        //public string fifth { get; set; }

        public override string ToString()
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(this);
        }

    }

}