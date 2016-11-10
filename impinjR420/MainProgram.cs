using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Impinj.OctaneSdk;
using System.Threading;
using Org.LLRP.LTK.LLRPV1;
using Org.LLRP.LTK.LLRPV1.Impinj;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace impinjR420
{
    class MainProgram
    {
        // Create an instance of the ImpinjReader class.
        static ImpinjReader reader = new ImpinjReader();
        private const string columnSeparator = ",";

        static void Main(string[] args)
        {
            //var sr = new StreamReader(@"./test.csv");
            //CsvReader csvread = new CsvReader(sr);
            //StreamWriter textWriter = new StreamWriter(@"test.csv");
            //CsvWriter csw = new CsvWriter(textWriter);
            //TestCSV tagreport = new TestCSV();
            //tagreport.first = "first";
            //tagreport.second = "second";
            //tagreport.third = "third";
            //tagreport.fourth = "fourth";
            //tagreport.fifth = "fifth";
            //csw.WriteRecord(tagreport);
            //csw.WriteRecord(tagreport);
            //csw.WriteRecord(tagreport);
            //csw.WriteRecord(tagreport);
            //textWriter.Close();

            Console.WriteLine("All records:");
            var sr = new StreamReader(@"C:\Users\TX\Documents\Visual Studio 2015\Projects\impinjR420\impinjR420\test1.csv");
            //var sw = new StreamWriter(@"C:\Users\TX\Documents\Visual Studio 2015\Projects\impinjR420\impinjR420\test1.csv");
            var csr = new CsvReader(sr);
            //var csw = new CsvWriter(sw);
            while (csr.Read())
            {
                var record = csr.GetRecord<TestCSV>();
               // var intField = csr.GetField<int>(0);
                //Console.WriteLine(intField.ToString("0.00"));
                Console.WriteLine(record.first);
                Console.WriteLine(record.second);
                Console.WriteLine(record.third);
                Console.WriteLine(record.fourth);
                Console.WriteLine(record.fifth);
                //Console.WriteLine(record.ToString("0.00"));
            }
            // Wait for the user to press enter.
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
            //var records = csr.GetRecord<TestCSV>();
            //Console.WriteLine(records);
            //using (var reader = new CsvReader(new StreamReader(GetDataStream(true, false))))
            //{
            //    var records = reader.GetRecords<TestCSV>();
            //    foreach (var record in records)
            //    {
            //        Console.WriteLine(record);
            //    }
            //}
            //Console.WriteLine();

            // ConnectAsync(reader);
        }

        public static MemoryStream GetDataStream(bool hasHeader, bool hasSpacesInHeaderNames)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            if (hasHeader)
            {
                var header = hasSpacesInHeaderNames
                                 ? "String Column,Int Column,Guid Column,Custom Type Column"
                                 : "StringColumn,IntColumn,GuidColumn,CustomTypeColumn";
                writer.WriteLine(header);
            }
            writer.WriteLine("one,1,{0},1|2|3", Guid.NewGuid());
            writer.WriteLine("two,2,{0},4|5|6", Guid.NewGuid());
            writer.WriteLine("\"this, has a comma\",2,{0},7|8|9", Guid.NewGuid());
            writer.WriteLine("\"this has \"\"'s\",4,{0},10|11|12", Guid.NewGuid());
            writer.Flush();
            stream.Position = 0;

            return stream;
        }

        static void ConnectAsync(ImpinjReader reader)
        {
            try
            {
                // Assign the ConnectAsyncComplete event handler.
                // This specifies which method to call
                // when the connection attempt has completed.
                reader.ConnectAsyncComplete += OnConnectAsyncComplete;

                // Attempt to connect to this reader asynchronously. 
                // This method return immediately. 
                // An event will be raised when the connect attempt has succeeded or failed.
                Console.WriteLine("Attempting connection to {0}", SolutionConstants.ReaderHostname);
                reader.ConnectAsync(SolutionConstants.ReaderHostname);

                // Wait for the user to press enter.
                Console.WriteLine("Press enter to exit.");
                Console.ReadLine();
            }
            catch (OctaneSdkException e)
            {
                // Handle Octane SDK errors.
                Console.WriteLine("Octane SDK exception: {0}", e.Message);
            }
            catch (Exception e)
            {
                // Handle other .NET errors.
                Console.WriteLine("Exception : {0}", e.Message);
            }
        }
        static void OnConnectAsyncComplete(ImpinjReader reader, ConnectAsyncResult result, string errorMessage)
        {
            // This event handler is called asynchronously 
            // when the connection attempt has completed.

            // Check the result of the connection attempt
            if (result == ConnectAsyncResult.Success)
            {
                // Successfully connection to the reader. Now configure  and start it.
                Console.WriteLine("Successfully connected to {0}", reader.Address);
                reader.ApplyDefaultSettings();
                Console.WriteLine("Starting reader...");
                //reader.Start();
                SetReport(reader);
                reader.Stop();
                reader.Disconnect();
                Console.WriteLine("Reader stopped. Press enter to exit.");
            }
            else
            {
                // Failed to connect to the reader
                Console.WriteLine("Failure while connecting to {0} : {1}", reader.Address, errorMessage);
            }
        }

        static void SetReport(ImpinjReader reader)
        {
            try
            {
                // Connect to the reader.
                // Change the ReaderHostname constant in SolutionConstants.cs 
                // to the IP address or hostname of your reader.
              //  reader.Connect(SolutionConstants.ReaderHostname);

                // Get the default settings
                // We'll use these as a starting point
                // and then modify the settings we're 
                // interested in.
                Settings settings = reader.QueryDefaultSettings();

                // Tell the reader to include the
                // RF doppler frequency in all tag reports. 
                settings.Report.IncludeFirstSeenTime = true;
                settings.Report.IncludePeakRssi = true;
                settings.Report.IncludeDopplerFrequency = true;
                settings.Report.IncludePhaseAngle = true;
                settings.Report.IncludeChannel = true;


                // Use antenna #4
                settings.Antennas.DisableAll();
                settings.Antennas.GetAntenna(4).IsEnabled = true;

                // ReaderMode must be set to DenseReaderM8.
                //settings.ReaderMode = ReaderMode.DenseReaderM8;
                settings.ReaderMode = ReaderMode.MaxThroughput;

                // Apply the newly modified settings.
                reader.ApplySettings(settings);

                // Assign the TagsReported event handler.
                // This specifies which method to call
                // when tags reports are available.
                reader.TagsReported += OnTagsReported;

                // Start reading.
                reader.Start();

                // Wait for the user to press enter.
                Console.WriteLine("Press enter to exit.");
                Console.ReadLine();

                // Stop reading.
                reader.Stop();

                // Disconnect from the reader.
                reader.Disconnect();
            }
            catch (OctaneSdkException e)
            {
                // Handle Octane SDK errors.
                Console.WriteLine("Octane SDK exception: {0}", e.Message);
            }
            catch (Exception e)
            {
                // Handle other .NET errors.
                Console.WriteLine("Exception : {0}", e.Message);
            }
        }

        static void OnTagsReported(ImpinjReader sender, TagReport report)
        {
            // This event handler is called asynchronously 
            // when tag reports are available.
            // Loop through each tag in the report 
            // and print the data.
            //var textWriter = new StreamWriter(@"C:\Users\TX\OneDrive\HCI\MISTI\RFID_test_data\TH_2_test.csv");
            var textWriter = new StreamWriter(@"test.csv");
            var csv = new CsvWriter(textWriter);

            foreach (Tag tag in report)
            {
                TagReportCSV tagreport = new TagReportCSV();
                tagreport.epc = tag.Epc;
                tagreport.FirstSeenTime = tag.FirstSeenTime;
                tagreport.PeakRSSI = tag.PeakRssiInDbm;
                tagreport.PhaseAngle = tag.PhaseAngleInRadians;
                tagreport.DopplerFreq = tag.RfDopplerFrequency;
                csv.WriteRecord<TagReportCSV>(tagreport);
                Console.WriteLine("EPC : {0} PEAKRSSI(dBm) : {1} Phase Angle(Radians) : {2} Doppler Frequency (Hz) : {3} ",
                                    tag.FirstSeenTime, tag.PeakRssiInDbm.ToString("0.00"), tag.PhaseAngleInRadians.ToString("0.00"), tag.RfDopplerFrequency.ToString("0.00"));
            }
            textWriter.Close(); 
        }

    }
}
