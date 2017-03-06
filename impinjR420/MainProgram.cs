using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Text;
using System.Threading.Tasks;
using Impinj.OctaneSdk;
using System.Threading;
using Org.LLRP.LTK.LLRPV1;
using Org.LLRP.LTK.LLRPV1.Impinj;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using YeelightCMD;

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
        private static int tagcnt = 0;
        private static double refrssi;
        private static double buttonrssi;
        private static double last_buttonrssi;
        private static Queue<double> RSSI_ref = new Queue<double>();
        private static Queue<double> RSSI_button = new Queue<double>();
        private static Queue<double> RSSI_smooth = new Queue<double>();
        private static ulong LST_ref;
        private static System.Timers.Timer aTimer;
        private static ulong epoch;
        private static int flag_report;
        private static Yeelight device;
        private static int[] rgb;

        static void Main(string[] args)
        {
            //setup parameters
            csv_path = Path.GetFullPath(SolutionConstants.csvpath + "sensor_testdata.csv");
            Console.WriteLine("The test report path is :{0}", csv_path);
            textWriter = new StreamWriter(csv_path);
            csvw = new CsvWriter(textWriter);
            device = new Yeelight("0x00000000033622a3", "192.168.0.166", 55443);
            device.set_power(1);
            device.set_bright(30);
            device.set_rgb(255,255,128); //a warm color
            Thread.Sleep(1000);

            // Timer setup. Use a timer to check tag pool every 2ms
            aTimer = new System.Timers.Timer();
            aTimer.Interval = 10;

            aTimer.Elapsed += CheckTag;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            //connect to the reader
            ConnectAsync(reader);
            device.set_power(0);
        }

        static void CheckTag(Object source, System.Timers.ElapsedEventArgs e)
        {
            flag_report = 0;
            tagcnt = 0;
            int cnt=5;
            //for (int i = 0; i < SensorParams.count; i++)
            //{
            //    SensorParams.states[i] = 1;
            //}
            reader.QueryTags();
            while ((flag_report==0) && (cnt!=0))
            {
                Thread.Sleep(1);
                cnt--;
            }
          
            if(flag_report==1)
            {
                //cntt++;
                //if (cntt % 3 == 0)
                //{
                //    rgb[0] = 255;
                //    rgb[1] = 0;
                //    rgb[2] = 0;
                //}
                //else if (cntt % 3 == 1)
                //{
                //    rgb[0] = 0;
                //    rgb[1] = 255;
                //    rgb[2] = 0;
                //}
                //else if (cntt % 3 == 2)
                //{
                //    rgb[0] = 0;
                //    rgb[1] = 0;
                //    rgb[2] = 255;
                //}
                for (int i = 0; i < SensorParams.count; i++)
                {
                    Console.WriteLine("tagcnt {0},  sensor id {1}, state {2}", tagcnt, i, SensorParams.states[i]);
                    if (SensorParams.states[0]-SensorParams.laststate[0] == 1)
                    {
                        //Form1.Mouse_Click();
                        if (device.Toggle())
                        {
                            Console.WriteLine("succesfully toggled and set brightness");
                        }
                    }
                    //else
                    //{
                    //    Form1.Mouse_LeftUp();
                    //}
                    SensorParams.laststate[i] = SensorParams.states[i];
                }
            }

        }

        static void OnTagsReported(ImpinjReader sender, TagReport report)
        {
            // This event handler is called asynchronously 
            // when tag reports are available.
            // Loop through each tag in the report 
            // and print the data.
            for (int i = 0; i < SensorParams.count; i++)
            {
                SensorParams.states[i] = 1;
            }
            int index;
            foreach (Tag tag in report)
            {
                index = Array.IndexOf(SensorParams.epcs, tag.Epc.ToString());
                if (index >= 0)
                {
                    //SensorParams.LST[index] = tag.LastSeenTime.Utc;
                    SensorParams.states[index] = 0;
                }
                tagcnt++;
                // Console.WriteLine("tagcnt {0},  state {1}, epc {2}", tagcnt,index, tag.Epc.ToString());

            }
            flag_report = 1;

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
                settings.Antennas.GetAntenna(SolutionConstants.antenna).IsEnabled = true;
                //settings.Antennas.GetAntenna(2).IsEnabled = true;


                // ReaderMode must be set to DenseReaderM8.
                //settings.ReaderMode = ReaderMode.DenseReaderM8;
                settings.ReaderMode = ReaderMode.MaxThroughput;
                //settings.Antennas.GetAntenna(1).TxPowerInDbm = 30;

                // Tell the reader not to send tag reports.
                // We will ask for them.
                settings.Report.Mode = ReportMode.WaitForQuery;

                // Apply the newly modified settings.
                reader.ApplySettings(settings);


                // Assign an event handler that will
                // be called when the tag report buffer is almost full.
                reader.ReportBufferWarning += OnReportBufferWarning;

                // Assign an event handler that will
                // be called when the tag report buffer has overflowed.
                reader.ReportBufferOverflow += OnReportBufferOverflow;

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

        

        static void OnReportBufferOverflow(ImpinjReader reader, ReportBufferOverflowEvent e)
        {
            Console.WriteLine("The tag report buffer has overflowed!");
        }

        static void OnReportBufferWarning(ImpinjReader reader, ReportBufferWarningEvent e)
        {
            Console.WriteLine("The tag report buffer is {0}% full!", e.PercentFull);
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
