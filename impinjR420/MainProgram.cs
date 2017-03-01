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
        private static string csv_path; 
        private static StreamWriter textWriter;
        private static CsvWriter csvw;
        private static TagReportCSV tagreport = new TagReportCSV();
        private static int cntt = 0;
        private static int cntl = 0;
        private static int laststate = 0;
        private static double refrssi;
        private static double buttonrssi;
        private static double last_buttonrssi;
        private static Queue<double> RSSI_ref = new Queue<double>();
        private static Queue<double> RSSI_button = new Queue<double>();
        private static Queue<double> RSSI_smooth = new Queue<double>();
        private static ulong LST_ref;

        static void Main(string[] args)
        {
            csv_path = Path.GetFullPath(SolutionConstants.csvpath + "sensor_testdata.csv");
            Console.WriteLine("The test report path is :{0}", csv_path);
            textWriter = new StreamWriter(csv_path);
            csvw = new CsvWriter(textWriter);
            //ulong epoch = Convert.ToUInt64((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000)/10000);
            //Console.WriteLine(epoch);
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
                //Console.WriteLine("total count :{0} lose count:{1}", cntt,cntl);

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
                //settings.Antennas.GetAntenna(2).IsEnabled = true;

                // ReaderMode must be set to DenseReaderM8.
                //settings.ReaderMode = ReaderMode.DenseReaderM8;
                settings.ReaderMode = ReaderMode.MaxThroughput;
               // settings.Antennas.GetAntenna(1).TxPowerInDbm = 10;

                // Apply the newly modified settings.
                reader.ApplySettings(settings);

                TestCSV header = new TestCSV();
                header.first = "Sensor#";
                //header.second = "FirstSeenTime";
                //header.third = "LastSeenTime";
                //header.fourth = "Channel";
                //header.fifth = "PeakRSSI";
                //header.sixth = "PhaseAngle";
                //header.seventh = "DopplerFreq";
                header.second = "LastSeenTime";
                header.third = "state";
                csvw.WriteRecord(header);

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

            int index=0;
            foreach (Tag tag in report)
            {
                //ulong epoch = Convert.ToUInt64((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
                //Console.WriteLine("epc: {0} timestamp : {1} timed : {2} state0 : {3}", tag.Epc, tag.LastSeenTime.Utc, tag.LastSeenTime.Utc / 1000 - epoch, SensorParams.states[0]);
                index = onoff_check_LST(tag);
                //Console.WriteLine("epc: {0} timestamp : {1} index : {2}", tag.Epc, tag.LastSeenTime.Utc, index);
                //if (index == -2)
                //{
                tagreport.sensor = 0;
                if (index == -2)
                {
                    tagreport.RefTime = tag.LastSeenTime.Utc;
                }
                if(index == 0)
                {
                    tagreport.LastSeenTime = SensorParams.LST[0];
                }
                tagreport.state = SensorParams.states[0];
                csvw.WriteRecord(tagreport);
                Console.WriteLine("epc: {0} reftime : {1} tagtime : {2} state0 : {3}", tagreport.sensor, tagreport.RefTime, tagreport.LastSeenTime, tagreport.state);

                if (index == -2 || index == 0)
                {
                   // ulong epoch = Convert.ToUInt64((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
                  //  Console.WriteLine("epc: {0} timestamp : {1} state0 : {2}", tag.Epc, tag.LastSeenTime.Utc, SensorParams.states[0]);
                }
                if (SensorParams.states[0] - laststate == 1)
                {
                    //Form1.Mouse_Click();
                    cntt++;
                }
                Console.WriteLine("cnt:{0}", cntt);
                //if (SensorParams.states[0] == 1)
                //{
                //    Form1.Mouse_LeftUp();
                //}
                //else
                //{
                //    Form1.Mouse_LeftDown();
                //}
                //Console.WriteLine("timestamp : {0} state0 : {1} state1 : {2} state2 : {3} state3 : {4} ", tag.LastSeenTime.Utc, SensorParams.states[0], SensorParams.states[1], SensorParams.states[2], SensorParams.states[3]);
                //Console.WriteLine("Ref lst : {0} Button lst : {1} Status :{2} substraction:{3} subneg:{4}", LST_ref, LST_button,onoff.ToString(), (LST_ref.Utc-LST_button.Utc), (LST_button.Utc - LST_ref.Utc));
                //tagreport.epc = tag.Epc;
                // tagreport.FirstSeenTime = tag.FirstSeenTime;
                // tagreport.PeakRSSI = tag.PeakRssiInDbm;
                // tagreport.PhaseAngle = tag.PhaseAngleInRadians;
                // tagreport.DopplerFreq = tag.RfDopplerFrequency;
                // csvw.WriteRecord(tagreport);
                //if(SensorParams.epcs.Contains(tag.Epc.ToString()))
                // {
                //Console.WriteLine("EPC : {0} PEAKRSSI(dBm) : {1} Phase Angle(Radians) : {2} LastSeenTime : {3} Status : {4} ",
                //                                      tag.Epc, tag.PeakRssiInDbm.ToString("0.00"), tag.PhaseAngleInRadians.ToString("0.00"), tag.LastSeenTime.ToString(), SensorParams.states[0]);
                // }

            }
            laststate = SensorParams.states[0];
            //Console.WriteLine("Sensor1:{0} Sensor2:{1} Sensor3:{2} Sensor4:{3}", SensorParams.states[0], SensorParams.states[0], SensorParams.states[0], SensorParams.states[0]);
        }

        static int onoff_check_LST(Tag tag)
        {
            //it's the ref tag, then check all sensor tags
            if (tag.Epc.ToString() == SolutionConstants.refepc)
            {
                LST_ref = tag.LastSeenTime.Utc ;
                for(int i=0;i<SensorParams.count;i++)
                {
                    setstate(LST_ref, SensorParams.LST[i], i);
                    //Console.WriteLine("index :{0} diff:{1}", i, LST_ref - SensorParams.LST[i]);
                }
                return -2;
            }
            //it's a sensor tag, check this tag only
            int index = Array.IndexOf(SensorParams.epcs, tag.Epc.ToString());
            if (index != -1)
            {
                //Console.WriteLine("sensor tag:{0}  index :{1}", tag.Epc.ToString(), index);
                SensorParams.LST[index] = tag.LastSeenTime.Utc ;
                setstate(LST_ref, SensorParams.LST[index], index);
                //Console.WriteLine("index :{0} diff:{1}", index, LST_ref - SensorParams.LST[index]);
            }
            return index;
            
        }

        static void onoff_check_report(Tag tag)
        {
            int index = Array.IndexOf(SensorParams.epcs, tag.Epc.ToString());
            if (index != -1)
            {
                //SensorParams.LST[index] = tag.LastSeenTime.Utc;
                SensorParams.states[index] = 0;
            }
        }

        static void setstate(ulong timeref, ulong timetag, int index)
        {
            if ((timeref > timetag) && (timeref - timetag) > SensorParams.threshold)
            {
                SensorParams.states[index] = 1;
                //Console.WriteLine("detected!");
            }
            else
            {
                SensorParams.states[index] = 0;
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

    }
}
