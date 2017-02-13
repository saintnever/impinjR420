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
        //private static string csv_path; 
        private static StreamWriter textWriter;
        private static CsvWriter csvw;
        //= new CsvWriter(textWriter);
        private static TagReportCSV tagreport = new TagReportCSV();

        static void Main(string[] args)
        {
            Console.WriteLine("Input interaction mode: 1.hover; 2.touch; 3.dclick; 4.slide; 5.pinch");
            string mode = ModeSelect(Console.ReadLine());
            string csv_path = Path.GetFullPath(SolutionConstants.csvpath + mode+".csv");
            Console.WriteLine("The test report path is :{0}", csv_path);
            //csv_path = Path.GetFullPath("../../TH_3");
            textWriter = new StreamWriter(csv_path);
            csvw = new CsvWriter(textWriter);
            //Console.WriteLine("Press enter to exit.");
            //Console.ReadLine();
            ConnectAsync(reader);
        }

        static string ModeSelect(string input)
        {
            string dirpath;
            switch(input)
            {
                case "1":
                    dirpath = "hover";
                    break;
                case "2":
                    dirpath = "touch";
                    break;
                case "3":
                    dirpath = "dclick";
                    break;
                case "4":
                    dirpath = "slide";
                    break;
                case "5":
                    dirpath = "pinch";
                    break;
                default:
                    dirpath = "test";
                    break;
            }
            return dirpath;
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
                // assign event handler which runs after read is complete
                reader.TagOpComplete += OnTagOpComplete;
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


                // Use antenna #
                settings.Antennas.DisableAll();
                settings.Antennas.GetAntenna(1).IsEnabled = true;

                // ReaderMode must be set to DenseReaderM8.
                //settings.ReaderMode = ReaderMode.DenseReaderM8;
                settings.ReaderMode = ReaderMode.MaxThroughput;
                // Send a tag report for every tag read.
                //settings.Report.Mode = ReportMode.Individual;
                // Read the sensor code and chip RSSI code
                TagOpSequence seq = new TagOpSequence();
                
                
                // create a tag read operation
                TagReadOp readSensorCodeOp = new TagReadOp();
                readSensorCodeOp.MemoryBank = MemoryBank.Reserved;
                // Read one (8-bit) words
                readSensorCodeOp.WordCount = 1;
                // Starting at word 0xB
                readSensorCodeOp.WordPointer = 0xD;
                // Add this tag read op to the tag operation sequence.
                seq.Ops.Add(readSensorCodeOp);
                settings.Report.OptimizedReadOps.Add(readSensorCodeOp);
                settings.Report.IncludeChannel = true;

                // Specify a target tag based on the EPC.
                seq.TargetTag.MemoryBank = MemoryBank.User;
                seq.TargetTag.BitPointer = 0xD0;
                seq.TargetTag.Mask = "1F";
                // Setting this to null will specify any tag.
                // Replace this line with the one below it to target a particular tag.
                seq.TargetTag.Data = "1F";
                // Add the tag operation sequence to the reader.
                // The reader supports multiple sequences.
                reader.AddOpSequence(seq);
                // Setup a tag filter.
                // Only the tags that match this filter will respond.
                // First, setup tag filter #1.
                // We want to apply the filter to the EPC memory bank.
                //settings.Filters.TagFilter1.MemoryBank = MemoryBank.Epc;
                // Start matching at the third word (bit 32), since the 
                // first two words of the EPC memory bank are the
                // CRC and control bits. BitPointers.Epc is a helper
                // enumeration you can use, so you don't have to remember this.
                //settings.Filters.TagFilter1.BitPointer = BitPointers.Epc;
                // Only match tags with EPCs that start with "E200 5147"
                //settings.Filters.TagFilter1.TagMask = SolutionConstants.tagfilter;
                // This filter is 16 bits long (one word).
                // settings.Filters.TagFilter1.BitCount = 48;

                // Set the filter mode.
                // Both filters must match.
                // settings.Filters.Mode = TagFilterMode.OnlyFilter1;

                // Apply the newly modified settings.
                reader.ApplySettings(settings);

                // Assign the TagsReported event handler.
                // This specifies which method to call
                // when tags reports are available.
                TestCSV header = new TestCSV();
                header.first = "epc";
                header.second = "FirstSeenTime";
                header.third = "LastSeenTime";
                header.fourth = "Channel";
                header.fifth = "PeakRSSI";
                header.sixth = "PhaseAngle";
                header.seventh = "DopplerFreq";
                csvw.WriteRecord(header);
                //TagReport report;
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
                textWriter.Close();

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

            foreach (Tag tag in report)
            {
                tagreport.epc = tag.Epc.ToString();
                tagreport.FirstSeenTime = tag.FirstSeenTime.Utc;
                tagreport.LastSeenTime = tag.LastSeenTime.Utc;
                tagreport.Channel = tag.ChannelInMhz;
                tagreport.PeakRSSI = tag.PeakRssiInDbm;
                tagreport.PhaseAngle = tag.PhaseAngleInRadians;
                tagreport.DopplerFreq = tag.RfDopplerFrequency;
                csvw.WriteRecord(tagreport);
                //TODO: the last row sometimes is not complete. Need to figure out why. 
               // Console.WriteLine("EPC : {0} PEAKRSSI(dBm) : {1} Phase Angle(Radians) : {2} Doppler Frequency (Hz) : {3} ",
                //                    tag.Epc.ToString(), tag.PeakRssiInDbm.ToString("0.00"), tag.PhaseAngleInRadians.ToString("0.00"), tag.RfDopplerFrequency.ToString("0.00"));
            }
        }

        // This event handler will be called when tag 
        // operations have been executed by the reader.
        static void OnTagOpComplete(ImpinjReader reader, TagOpReport report)
        {
            // Loop through all the completed tag operations
            foreach (TagOpResult result in report)
            {
                // Was this completed operation a tag read operation?
                if (result is TagReadOpResult)
                {
                    // Cast it to the correct type.
                    TagReadOpResult readResult = result as TagReadOpResult;
                    // Print out the results.
                    Console.WriteLine("Read complete. Status : {0}", readResult.Result);
                    Console.WriteLine("EPC:{0} RSSI:{1} Phase:{2} SensorCode:{3}", readResult.Tag.Epc, readResult.Tag.PeakRssiInDbm, readResult.Tag.PhaseAngleInRadians, readResult.Data);
                }
            }
        }


    }
}
