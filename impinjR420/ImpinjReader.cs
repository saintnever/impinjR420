#region Assembly Impinj.OctaneSdk, Version=2.2.0.240, Culture=neutral, PublicKeyToken=05c02f4d2efc38d8
// C:\Users\TX\Documents\Visual Studio 2015\Projects\impinjR420\impinjR420\lib\Impinj.OctaneSdk.dll
#endregion

using System;
using Org.LLRP.LTK.LLRPV1;

namespace Impinj.OctaneSdk
{
    //
    // Summary:
    //     The main class for controlling and configuring an Impinj RFID reader.
    public class ImpinjReader
    {
        //
        // Summary:
        //     Creates and initializes an ImpinjReader object.
        public ImpinjReader();
        //
        // Summary:
        //     Creates and initializes an ImpinjReader object.
        public ImpinjReader(string address, string name);

        //
        // Summary:
        //     Destroys an ImpinjReader object and frees resources.
        ~ImpinjReader();

        //
        // Summary:
        //     Contains the IP address or hostname of the reader.
        //
        // Remarks:
        //     Read only.
        public string Address { get; }
        //
        // Summary:
        //     The connection timeout in milliseconds. If a connection to the reader cannot
        //     be established in this time, an exception is thrown.
        public int ConnectTimeout { get; set; }
        //
        // Summary:
        //     Handle to the build in debug logging handler.
        public ImpinjReaderDebug Debug { get; }
        //
        // Summary:
        //     Indicates whether or not a connection to the reader exists.
        public bool IsConnected { get; }
        //
        // Summary:
        //     Returns whether or not the connected reader is a reader with direction or location
        //     role capabilities.
        public bool IsSpatialReader { get; }
        //
        // Summary:
        //     Returns whether the connected reader is an xArray or not.
        public bool IsXArray { get; }
        //
        // Summary:
        //     Returns whether the connected reader is an xSpan or not.
        public bool IsXSpan { get; }
        //
        [Obsolete("This property is no longer supported.", true)]
        public int LogLevel { get; set; }
        //
        // Summary:
        //     [Deprecated] Number of times to attempt a reader connection before an exception
        //     is thrown.
        [Obsolete("This property is no longer supported.", true)]
        public int MaxConnectionAttempts { get; set; }
        //
        // Summary:
        //     The message reply timeout.
        public int MessageTimeout { get; set; }
        //
        // Summary:
        //     Assigns a name to this reader. For example, "Dock Door #1 Reader". This name
        //     can be used to identify a particular reader in an application that controls multiple
        //     readers.
        public string Name { get; set; }
        //
        // Summary:
        //     Handle for establishing RShell communications with the reader.
        public RShellEngine RShell { get; }

        //
        // Summary:
        //     Event to provide notification when an AntennaChanged event occurs.
        public event AntennaEventHandler AntennaChanged;
        //
        // Summary:
        //     Event to provide notification of a completed asynchronous connection attempt.
        public event ConnectAsyncCompleteHandler ConnectAsyncComplete;
        //
        // Summary:
        //     Event to provide notification that the TCP/IP connection to the Impinj Reader
        //     has been lost.
        public event ConnectionLostHandler ConnectionLost;
        //
        // Summary:
        //     Event to provide notification of a diagnostic report. Only available on the xArray.
        public event DiagnosticsReportedHandler DiagnosticsReported;
        //
        // Summary:
        //     Event to provide notification of a direction report. Only available on the xArray.
        public event DirectionReportedHandler DirectionReported;
        //
        // Summary:
        //     Event to provide notification when a GPI port status changes.
        public event GpiChangedHandler GpiChanged;
        //
        // Summary:
        //     Event to provide notification that a keep alive TCP/IP packet was received from
        //     the reader.
        public event KeepaliveHandler KeepaliveReceived;
        //
        // Summary:
        //     Event to provide notification of a location report. Only available on the xArray.
        public event LocationReportedHandler LocationReported;
        //
        // Summary:
        //     Obsolete. Do not use.
        [Obsolete("This event is no longer supported.", true)]
        public event LoggingHandler Logging;
        //
        // Summary:
        //     Event to provide notification when the reader has started.
        public event ReaderStartedEventHandler ReaderStarted;
        //
        // Summary:
        //     Event to provide notification when the reader has started.
        public event ReaderStoppedEventHandler ReaderStopped;
        //
        // Summary:
        //     Event to provide notification when the report buffer on the reader could not
        //     hold more tag reports.
        public event ReportBufferOverflowEventHandler ReportBufferOverflow;
        //
        // Summary:
        //     Event to provide notification when the report buffer on the reader is nearly
        //     full.
        public event ReportBufferWarningEventHandler ReportBufferWarning;
        //
        // Summary:
        //     Event to provide notification that a tag operation has completed, including the
        //     results of the operation.
        public event TagOpCompleteHandler TagOpComplete;
        //
        // Summary:
        //     Event to provide notification when a tag report is available.
        public event TagsReportedHandler TagsReported;

        //
        // Summary:
        //     Adds a sequence of tag operations (read, write, lock, kill) to the reader.
        //
        // Parameters:
        //   sequence:
        //     The sequence of operations to add to the reader.
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown if a zero length operations sequence is provided.
        public void AddOpSequence(TagOpSequence sequence);
        //
        // Summary:
        //     Applies the default settings to the reader.
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown if this method is called prior to establishing a connection with a reader.
        public void ApplyDefaultSettings();
        //
        // Summary:
        //     Obsolete method. Do not use.
        [Obsolete("This method has been renamed ApplyDefaultSettings.", true)]
        public Settings ApplyFactorySettings();
        //
        // Summary:
        //     Applies the provided settings to the reader.
        //
        // Parameters:
        //   settings:
        //     Settings to apply to the reader
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown when this method is called prior to establishing a connection with a reader,
        //     or if the settings are invalid.
        public void ApplySettings(Settings settings);
        //
        // Summary:
        //     Applies the provided settings to the reader.
        //
        // Parameters:
        //   setReaderConfigMessage:
        //     The set reader config LLRP LTK message to be sent
        //
        //   addRoSpecMessage:
        //     The add RO Spec LLRP LTK message to be sent
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown when this method is called prior to establishing a connection with a reader,
        //     or if the settings are invalid.
        public void ApplySettings(MSG_SET_READER_CONFIG setReaderConfigMessage, MSG_ADD_ROSPEC addRoSpecMessage);
        //
        // Summary:
        //     Build up an LTK add ROSpec message.
        //
        // Parameters:
        //   config:
        //     A settings object after which the message will be modeled.
        //
        // Returns:
        //     An LTK add ROSpec message.
        public MSG_ADD_ROSPEC BuildAddROSpecMessage(Settings config);
        //
        // Summary:
        //     Build up an LTK set reader config message.
        //
        // Parameters:
        //   config:
        //     A settings object after which the message will be modeled.
        //
        // Returns:
        //     An LTK set reader config message.
        public MSG_SET_READER_CONFIG BuildSetReaderConfigMessage(Settings config);
        //
        [Obsolete("This method is now obsolete. Use ApplyDefaultSettings or QueryDefaultSettings instead", true)]
        public void ClearSettings();
        //
        // Summary:
        //     Connect to an %Impinj RFID reader. The Address property must be set prior to
        //     calling this method.
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown when the connection attempt fails or if the Address property has not been
        //     set.
        public void Connect();
        //
        // Summary:
        //     Connect to an %Impinj RFID reader.
        //
        // Parameters:
        //   address:
        //     IP address or hostname of the target reader.
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown when the connection attempt fails.
        public void Connect(string address);
        //
        // Summary:
        //     Connect to an %Impinj RFID reader.
        //
        // Parameters:
        //   address:
        //     IP address or hostname of the target reader.
        //
        //   useTLS:
        //     Enable TLS encryption if "true".
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown when the connection attempt fails.
        public void Connect(string address, bool useTLS);
        //
        // Summary:
        //     Connect to an %Impinj RFID reader.
        //
        // Parameters:
        //   address:
        //     IP address or hostname of the target reader.
        //
        //   port:
        //     The port with which to connect.
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown when the connection attempt fails.
        public void Connect(string address, int port);
        //
        // Summary:
        //     Connect to an %Impinj RFID reader.
        //
        // Parameters:
        //   address:
        //     IP address or hostname of the target reader.
        //
        //   port:
        //     TCP/IP port number used by the target reader.
        //
        //   useTLS:
        //     Enable TLS encryption if "true".
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown when the connection attempt fails.
        public void Connect(string address, int port, bool useTLS);
        //
        // Summary:
        //     Connect to an %Impinj RFID reader asynchronously. An event will be raised when
        //     the connection attempt succeeds or fails. The Address property must be set prior
        //     to calling this method.
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown when the connection attempt fails or if the Address property has not been
        //     set.
        public void ConnectAsync();
        //
        // Summary:
        //     Connect to an %Impinj RFID reader asynchronously. An event will be raised when
        //     the connection attempt succeeds or fails.
        //
        // Parameters:
        //   address:
        //     IP address or hostname of the target reader.
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown when the connection attempt fails.
        public void ConnectAsync(string address);
        //
        // Summary:
        //     Connect to an %Impinj RFID reader asynchronously. An event will be raised when
        //     the connection attempt succeeds or fails.
        //
        // Parameters:
        //   address:
        //     IP address or hostname of the target reader.
        //
        //   useTLS:
        //     Enable TLS encryption if "true".
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown when the connection attempt fails.
        public void ConnectAsync(string address, bool useTLS);
        //
        // Summary:
        //     Connect to an %Impinj RFID reader asynchronously. An event will be raised when
        //     the connection attempt succeeds or fails.
        //
        // Parameters:
        //   address:
        //     IP address or hostname of the target reader.
        //
        //   port:
        //     TCP/IP port number used by the target reader.
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown when the connection attempt fails.
        public void ConnectAsync(string address, int port);
        //
        // Summary:
        //     Connect to an %Impinj RFID reader asynchronously. An event will be raised when
        //     the connection attempt succeeds or fails.
        //
        // Parameters:
        //   address:
        //     IP address or hostname of the target reader.
        //
        //   port:
        //     TCP/IP port number used by the target reader.
        //
        //   useTLS:
        //     Enable TLS encryption if "true".
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown when the connection attempt fails.
        public void ConnectAsync(string address, int port, bool useTLS);
        //
        // Summary:
        //     Deletes all tag operation sequences from the reader.
        public void DeleteAllOpSequences();
        //
        // Summary:
        //     Deletes a tag operation sequence from the reader.
        //
        // Parameters:
        //   sequenceId:
        //     The sequence ID of the operation sequence to delete.
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown if the referenced sequence does not exist on the reader.
        public void DeleteOpSequence(uint sequenceId);
        //
        // Summary:
        //     Closes the connection to the reader.
        public void Disconnect();
        //
        // Summary:
        //     Enables a tag operation sequence on the reader.
        //
        // Parameters:
        //   sequenceId:
        //     The sequence ID of the operation sequence to enable.
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown if the referenced sequence does not exist on the reader.
        public void EnableOpSequence(uint sequenceId);
        //
        // Parameters:
        //   accessParams:
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        [Obsolete("The use of this method is discouraged because it blocks program execution and performs poorly. We suggest creating a TagOpSequence instead.This method will be removed in a future release of the SDK.", false)]
        public KillTagResult KillTag(KillTagParams accessParams);
        //
        // Parameters:
        //   accessParams:
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        [Obsolete("The use of this method is discouraged because it blocks program execution and performs poorly. We suggest creating a TagOpSequence instead.This method will be removed in a future release of the SDK.", false)]
        public LockTagResult LockTag(LockTagParams accessParams);
        //
        // Parameters:
        //   accessParams:
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        [Obsolete("The use of this method is discouraged because it blocks program execution and performs poorly. We suggest creating a TagOpSequence instead.This method will be removed in a future release of the SDK.", false)]
        public ProgramAccessPasswordResult ProgramAccessPassword(ProgramAccessPasswordParams accessParams);
        //
        // Parameters:
        //   accessParams:
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        [Obsolete("The use of this method is discouraged because it blocks program execution and performs poorly. We suggest creating a TagOpSequence instead.This method will be removed in a future release of the SDK.", false)]
        public ProgramEpcResult ProgramEpc(ProgramEpcParams accessParams);
        //
        // Parameters:
        //   accessParams:
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        [Obsolete("The use of this method is discouraged because it blocks program execution and performs poorly. We suggest creating a TagOpSequence instead.This method will be removed in a future release of the SDK.", false)]
        public ProgramKillPasswordResult ProgramKillPassword(ProgramKillPasswordParams accessParams);
        //
        // Parameters:
        //   accessParams:
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        [Obsolete("The use of this method is discouraged because it blocks program execution and performs poorly. We suggest creating a TagOpSequence instead.This method will be removed in a future release of the SDK.", false)]
        public ProgramUserMemoryResult ProgramUserMemory(ProgramUserMemoryParams accessParams);
        //
        // Summary:
        //     Returns the reader default settings.
        //
        // Returns:
        //     The default settings of the reader.
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown when this method is called prior to establishing a connection with a reader.
        public Settings QueryDefaultSettings();
        //
        // Summary:
        //     Obsolete method. Do not use.
        [Obsolete("This method has been renamed QueryDefaultSettings.", true)]
        public Settings QueryFactorySettings();
        //
        // Summary:
        //     Queries the reader for a summary of the features that it supports.
        //
        // Returns:
        //     An object summarizing the features supported by the reader.
        public FeatureSet QueryFeatureSet();
        //
        // Summary:
        //     This function queries the reader for its current settings.
        //
        // Returns:
        //     An object containing the current reader settings
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown if a communication error occurs while talking with the reader, the reader
        //     has not been configured, or if the configuration was not applied by the SDK.
        public Settings QuerySettings();
        //
        // Summary:
        //     This function queries the reader for a summary of its current status.
        //
        // Returns:
        //     An object containing the current reader status.
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown if a communication error occurs while talking with the reader.
        public Status QueryStatus();
        //
        // Summary:
        //     Tells the reader to send all the tag reports it has buffered. The ReportMode
        //     should be set so that reader accumulates tag reads (WaitForQuery or BatchAfterStop).
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown when this method is called prior to establishing a connection with a reader.
        public void QueryTags();
        //
        // Parameters:
        //   seconds:
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        [Obsolete("The use of this method is discouraged because it blocks program execution and performs poorly. We suggest receiving tag reports asynchronously via an event.", false)]
        public TagReport QueryTags(double seconds);
        //
        // Parameters:
        //   accessParams:
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        [Obsolete("The use of this method is discouraged because it blocks program execution and performs poorly. We suggest creating a TagOpSequence instead.This method will be removed in a future release of the SDK.", false)]
        public ReadKillPasswordResult ReadKillPassword(ReadKillPasswordParams accessParams);
        //
        // Parameters:
        //   accessParams:
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        [Obsolete("The use of this method is discouraged because it blocks program execution and performs poorly. We suggest creating a TagOpSequence instead.This method will be removed in a future release of the SDK.", false)]
        public ReadTidMemoryResult ReadTidMemory(ReadTidMemoryParams accessParams);
        //
        // Parameters:
        //   accessParams:
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        [Obsolete("The use of this method is discouraged because it blocks program execution and performs poorly. We suggest creating a TagOpSequence instead.This method will be removed in a future release of the SDK.", false)]
        public ReadUserMemoryResult ReadUserMemory(ReadUserMemoryParams accessParams);
        //
        // Summary:
        //     This function tells the reader to resume sending reports and events. Messages
        //     in the queue on the reader may then be sent.
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown if a communication error occurs while talking with the reader.
        public void ResumeEventsAndReports();
        //
        // Summary:
        //     Instructs the Reader to save the current configuration to persistent storage.
        //     The saved parameters then become the Reader's power-on and reset settings.
        public void SaveSettings();
        //
        // Summary:
        //     This function is used to set the value of a GPO signal to the specified value.
        //     The output is set to high when state is true, and low when state is false.
        //
        // Parameters:
        //   port:
        //     GPO port to set
        //
        //   state:
        //     Value to set GPO to
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown if a communication error occurs while talking with the reader.
        public void SetGpo(ushort port, bool state);
        //
        // Summary:
        //     Starts the reader. Tag reports will be received asynchronously via an event.
        //
        // Exceptions:
        //   T:Impinj.OctaneSdk.OctaneSdkException:
        //     Thrown if the Start method is called before connecting to a reader.
        public void Start();
        //
        // Summary:
        //     Stops the reader. Tags will no longer be read.
        public void Stop();
        //
        // Summary:
        //     Turns the xArray beacon light off.
        public void TurnBeaconOff();
        //
        // Summary:
        //     Turns the xArray beacon light on.
        //
        // Parameters:
        //   duration:
        //     The period in milliseconds to blink the beacon light.
        public void TurnBeaconOn(ulong duration);

        //
        // Summary:
        //     Delegate declaration required to support declaration of AntennaChanged event.
        //     Internal use only - bind to the AntennaChanged event.
        //
        // Parameters:
        //   reader:
        //     ImpinjReader object.
        //
        //   e:
        //     AntennaEvent object.
        public delegate void AntennaEventHandler(ImpinjReader reader, AntennaEvent e);
        //
        // Summary:
        //     Delegate declaration required to support declaration of ConnectAsyncComplete
        //     event. Internal use only - bind to the ConnectAsyncComplete event.
        //
        // Parameters:
        //   reader:
        //     If the connection attempt was successful, this contains a connected ImpinjReader
        //     instance. If the connection failed, this contains a disconnected ImpinjReader
        //     instance with only the address set.
        //
        //   result:
        //     The result of the connection attempt.
        //
        //   errorMessage:
        //     An error message, if an error occurred
        public delegate void ConnectAsyncCompleteHandler(ImpinjReader reader, ConnectAsyncResult result, string errorMessage);
        //
        // Summary:
        //     Delegate declaration required to support declaration of ConnectionLostHandler
        //     event. Internal use only - bind to ConnectionLost event.
        //
        // Parameters:
        //   reader:
        //     ImpinjReader object
        public delegate void ConnectionLostHandler(ImpinjReader reader);
        //
        // Summary:
        //     Delegate declaration required to support declaration of DiagnosticsReported event.
        //     Internal use only - bind to the DiagnosticsReported event.
        //
        // Parameters:
        //   reader:
        //     The ImpinjReader that raised the event.
        //
        //   report:
        //     The diagnostic report.
        public delegate void DiagnosticsReportedHandler(ImpinjReader reader, DiagnosticReport report);
        //
        // Summary:
        //     Delegate declaration required to support declaration of DirectionReported event.
        //     Internal use only - bind to the DirectionReported event.
        //
        // Parameters:
        //   reader:
        //     The ImpinjReader that raised the event.
        //
        //   report:
        //     The direction report.
        public delegate void DirectionReportedHandler(ImpinjReader reader, DirectionReport report);
        //
        // Summary:
        //     Delegate declaration required to support declaration of GpiChanged event. Internal
        //     use only - bind to the GpiChanged event.
        //
        // Parameters:
        //   reader:
        //     ImpinjReader object.
        //
        //   e:
        //     GpiEvent object.
        public delegate void GpiChangedHandler(ImpinjReader reader, GpiEvent e);
        //
        // Summary:
        //     Delegate declaration required to support declaration of KeepaliveReceived event.
        //     Internal use only - bind to KeepaliveReceived event.
        //
        // Parameters:
        //   reader:
        //     ImpinjReader object
        public delegate void KeepaliveHandler(ImpinjReader reader);
        //
        // Summary:
        //     Delegate declaration required to support declaration of LocationReported event.
        //     Internal use only - bind to the LocationReported event.
        //
        // Parameters:
        //   reader:
        //     The ImpinjReader that raised the event.
        //
        //   report:
        //     The location report.
        public delegate void LocationReportedHandler(ImpinjReader reader, LocationReport report);
        //
        // Summary:
        //     Delegate declaration required to support declaration of Logging event. Internal
        //     use only - bind to Logging event.
        //
        // Parameters:
        //   reader:
        //     ImpinjReader
        public delegate void LoggingHandler(ImpinjReader reader);
        //
        // Summary:
        //     Delegate declaration required to support declaration of ReaderStarted event.
        //     Internal use only - bind to the ReaderStarted event.
        //
        // Parameters:
        //   reader:
        //     ImpinjReader object.
        //
        //   e:
        //     ReaderStarted object.
        public delegate void ReaderStartedEventHandler(ImpinjReader reader, ReaderStartedEvent e);
        //
        // Summary:
        //     Delegate declaration required to support declaration of ReaderStopped event.
        //     Internal use only - bind to the ReaderStopped event.
        //
        // Parameters:
        //   reader:
        //     ImpinjReader object.
        //
        //   e:
        //     ReaderStopped object.
        public delegate void ReaderStoppedEventHandler(ImpinjReader reader, ReaderStoppedEvent e);
        //
        // Summary:
        //     Delegate declaration required to support declaration of ReportBufferOverflow
        //     event. Internal use only - bind to the ReportBufferOverflow event.
        //
        // Parameters:
        //   reader:
        //     ImpinjReader object.
        //
        //   e:
        //     ReportBufferOverflowEvent object.
        public delegate void ReportBufferOverflowEventHandler(ImpinjReader reader, ReportBufferOverflowEvent e);
        //
        // Summary:
        //     Delegate declaration required to support declaration of ReportBufferWarning event.
        //     Internal use only - bind to the ReportBufferWarning event.
        //
        // Parameters:
        //   reader:
        //     ImpinjReader object.
        //
        //   e:
        //     ReportBufferWarningEvent object.
        public delegate void ReportBufferWarningEventHandler(ImpinjReader reader, ReportBufferWarningEvent e);
        //
        // Summary:
        //     Delegate declaration required to support declaration of TagOpComplete event.
        //     Internal use only - bind to the TagOpCompete event.
        //
        // Parameters:
        //   reader:
        //     ImpinjReader object.
        //
        //   results:
        //     TagOpReport object.
        public delegate void TagOpCompleteHandler(ImpinjReader reader, TagOpReport results);
        //
        // Summary:
        //     Delegate declaration required to support declaration of TagsReported event. Internal
        //     use only - bind to the TagsReported event.
        //
        // Parameters:
        //   reader:
        //     ImpinjReader object.
        //
        //   report:
        //     TagReport object.
        public delegate void TagsReportedHandler(ImpinjReader reader, TagReport report);
    }
}