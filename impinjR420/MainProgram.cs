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
        private static string csv_path = Path.GetFullPath("../../test2.csv");
        private static StreamWriter textWriter = new StreamWriter(csv_path);
        private static CsvWriter csvw = new CsvWriter(textWriter);

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
            //string csv_path = Path.GetFullPath("../../test2.csv");
            //Console.WriteLine(csv_path); 
            //Console.WriteLine("All records:");
            //var sr = new StreamReader(@"C:\Users\saintnever\OneDrive\HCI\MISTI\ImpinjR420\impinjR420\test1.csv");
            //var sw = new StreamWriter(csv_path);
            //var csr = new CsvReader(sr);
            //var csw = new CsvWriter(sw);
            //TestCSV header = new TestCSV();
            //header.first = "first";
            //header.second = "second";
            //header.third = "third";
            //header.fourth = "fourth";
            //header.fifth = "fifth";
            //csw.WriteRecord(header);
            //int cnt = 5;
            //while (cnt>0)
            //{
            //    //var record = csr.GetRecord<TestCSV>();
            //    // var intField = csr.GetField<int>(0);
            //    //Console.WriteLine(intField.ToString("0.00"));
            //    TestCSV cotent = new TestCSV();
            //    cotent.first = (5 - cnt).ToString();
            //    cotent.second = (4 - cnt).ToString();
            //    cotent.third = (5 - cnt).ToString();
            //    cotent.fourth = (4 - cnt).ToString();
            //    cotent.fifth = (5 - cnt).ToString();
            //    Console.WriteLine(cotent.first);
            //    Console.WriteLine(cotent.second);
            //    Console.WriteLine(cotent.third);
            //    Console.WriteLine(cotent.fourth);
            //    Console.WriteLine(cotent.fifth);
            //    csw.WriteRecord(cotent);
            //    cnt--;
            //    //Console.WriteLine(record.ToString("0.00"));
            //}
            //sr.Close();
            //sw.Close();
            //// Wait for the user to press enter.
            //Console.WriteLine("Press enter to exit.");
            //Console.ReadLine();

            ConnectAsync(reader);
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
                // Wait for the user to press enter.
                Console.WriteLine("Press enter to exit.");
                Console.ReadLine();
                textWriter.Close();

                //cleanup
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
          
                //TagReport report;
                reader.TagsReported += OnTagsReported;

                // Start reading.
                reader.Start();
         
                // Stop reading.
               // reader.Stop();

                // Disconnect from the reader.
              //  reader.Disconnect();
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
          
            TagReportCSV tagreport = new TagReportCSV();
            foreach (Tag tag in report)
            {
                tagreport.epc = tag.Epc;
                tagreport.FirstSeenTime = tag.FirstSeenTime;
                tagreport.PeakRSSI = tag.PeakRssiInDbm;
                tagreport.PhaseAngle = tag.PhaseAngleInRadians;
                tagreport.DopplerFreq = tag.RfDopplerFrequency;
                csvw.WriteRecord(tagreport);
                Console.WriteLine("EPC : {0} PEAKRSSI(dBm) : {1} Phase Angle(Radians) : {2} Doppler Frequency (Hz) : {3} ",
                                    tag.FirstSeenTime, tag.PeakRssiInDbm.ToString("0.00"), tag.PhaseAngleInRadians.ToString("0.00"), tag.RfDopplerFrequency.ToString("0.00"));
            }
        }

    }
}
