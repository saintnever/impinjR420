using System;
using System.Collections;
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
        private static int refcnt = 0;
        private static int buttoncnt = 0;
        private static int onoff = 0;
        private static double refrssi;
        private static double buttonrssi;
        private static double last_buttonrssi;
        private static Queue<double> RSSI_ref = new Queue<double>();
        private static Queue<double> RSSI_button = new Queue<double>();
        private static Queue<double> RSSI_smooth = new Queue<double>();
        private static ulong LST_ref;

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
            /*while (true)
            {
                Console.WriteLine("time:{0}", LST_ref);
            };*/
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
                settings.Report.IncludeSeenCount = true;
                settings.Report.IncludeLastSeenTime = true;


                // Use antenna #4
                settings.Antennas.DisableAll();
                settings.Antennas.GetAntenna(1).IsEnabled = true;

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
          
            foreach (Tag tag in report)
            {
                onoff_check_LST(tag);
                Console.WriteLine("Sensor1:{0} Sensor2:{1}",SensorParams.states[0], SensorParams.states[1]);
                //Console.WriteLine("Ref Average RSSI : {0} Button Average RSSI : {1} Status :{2}", refrssi.ToString("0.00"), buttonrssi.ToString("0.00"),onoff.ToString());
                //Console.WriteLine("Ref lst : {0} Button lst : {1} Status :{2} substraction:{3} subneg:{4}", LST_ref, LST_button,onoff.ToString(), (LST_ref.Utc-LST_button.Utc), (LST_button.Utc - LST_ref.Utc));
                //tagreport.epc = tag.Epc;
                // tagreport.FirstSeenTime = tag.FirstSeenTime;
                // tagreport.PeakRSSI = tag.PeakRssiInDbm;
                // tagreport.PhaseAngle = tag.PhaseAngleInRadians;
                // tagreport.DopplerFreq = tag.RfDopplerFrequency;
                // csvw.WriteRecord(tagreport);

               // Console.WriteLine("EPC : {0} PEAKRSSI(dBm) : {1} Phase Angle(Radians) : {2} LastSeenTime : {3} Status : {4} ",
                //                   tag.Epc, tag.PeakRssiInDbm.ToString("0.00"), tag.PhaseAngleInRadians.ToString("0.00"), tag.LastSeenTime.ToString(), SensorParams.states[0]);
            }
        }

        static int onoff_check_rssi(Tag tag)
        {
            if (tag.Epc.ToString() == SolutionConstants.refepc)
            {
                if (RSSI_ref.Count < 10)
                {
                    RSSI_ref.Enqueue(tag.PeakRssiInDbm);
                }
                else
                {
                    RSSI_ref.Dequeue();
                    RSSI_ref.Enqueue(tag.PeakRssiInDbm);
                }
                double[] temp1 = new double[RSSI_ref.Count];
                RSSI_ref.CopyTo(temp1, 0);
                refrssi = RSSI_ref.Average();
            }

            if (SensorParams.epcs.Contains(tag.Epc.ToString()))
            {
                if (RSSI_button.Count < 10)
                {
                    RSSI_button.Enqueue(tag.PeakRssiInDbm);
                }
                else
                {
                    RSSI_button.Dequeue();
                    RSSI_button.Enqueue(tag.PeakRssiInDbm);
                }

                double[] temp2 = new double[RSSI_button.Count];
                RSSI_button.CopyTo(temp2, 0);
                buttonrssi = RSSI_button.Average();

            }
            if (Math.Abs(refrssi - buttonrssi) < 8)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        static void onoff_check_LST(Tag tag)
        {
            //it's the ref tag, then check all sensor tags
            if (tag.Epc.ToString() == SolutionConstants.refepc)
            {
                LST_ref = tag.LastSeenTime.Utc;
                for(int i=0;i<SensorParams.count;i++)
                {
                    if ((LST_ref > SensorParams.LST[i]) && (LST_ref - SensorParams.LST[i]) > SensorParams.threshold)
                    {
                        SensorParams.states[i] = 1;
                    }
                    else
                    {
                        SensorParams.states[i] = 0;
                    }
                }
                return;
            }
            //it's a sensor tag, check this tag only
            int index = Array.IndexOf(SensorParams.epcs, tag.Epc.ToString());
            if (index != -1)
            {
                //Console.WriteLine("sensor tag:{0}  index :{1}", tag.Epc.ToString(), index);
                SensorParams.LST[index] = tag.LastSeenTime.Utc;
                //Console.WriteLine("diff:{0}", LST_ref - SensorParams.LST[index]);
                if ((LST_ref > SensorParams.LST[index]) && (LST_ref - SensorParams.LST[index]) > SensorParams.threshold)
                {
                    SensorParams.states[index] = 1;
                }
                else
                {
                    SensorParams.states[index] = 0;
                }
            }
            return;
            
        }

    }
}
