using System.Diagnostics.Tracing;
using System.Runtime.Versioning;

namespace Neme.Extensions.FileSystem;

[EventSource(Name = "Neme-Extensions-FileSystem-FileIO")]
[SupportedOSPlatform("windows6.0.6000")]
internal sealed class FileIOEventSource : EventSource
{
    public static readonly FileIOEventSource Log = new();

    private FileIOEventSource() { }

    [Event(1, Level = EventLevel.Verbose, Message = "Enumerating volumes to find serial number 0x{0:X16}")]
    public void EnumeratingVolumes(ulong volumeSerialNumber)
    {
        if (IsEnabled(EventLevel.Verbose, EventKeywords.All))
            WriteEvent(1, volumeSerialNumber);
    }

    [Event(2, Level = EventLevel.Verbose, Message = "Volume handle cache hit for serial number 0x{0:X16}")]
    public void VolumeHandleCacheHit(ulong volumeSerialNumber)
    {
        if (IsEnabled(EventLevel.Verbose, EventKeywords.All))
            WriteEvent(2, volumeSerialNumber);
    }

    [Event(3, Level = EventLevel.Verbose, Message = "Volume handle cache miss for serial number 0x{0:X16}")]
    public void VolumeHandleCacheMiss(ulong volumeSerialNumber)
    {
        if (IsEnabled(EventLevel.Verbose, EventKeywords.All))
            WriteEvent(3, volumeSerialNumber);
    }

    [Event(4, Level = EventLevel.Verbose, Message = "Found volume with matching lower 32-bit serial 0x{0:X8}, verifying full 64-bit match")]
    public void VolumePartialMatch(uint serialNumberLower32)
    {
        if (IsEnabled(EventLevel.Verbose, EventKeywords.All))
            WriteEvent(4, serialNumberLower32);
    }

    [Event(5, Level = EventLevel.Informational, Message = "Volume found and verified: serial number 0x{0:X16}")]
    public void VolumeVerified(ulong volumeSerialNumber)
    {
        if (IsEnabled(EventLevel.Informational, EventKeywords.All))
            WriteEvent(5, volumeSerialNumber);
    }

    [Event(6, Level = EventLevel.Verbose, Message = "Volume serial mismatch: expected 0x{0:X16}, got 0x{1:X16}")]
    public void VolumeSerialMismatch(ulong expectedSerial, ulong actualSerial)
    {
        if (IsEnabled(EventLevel.Verbose, EventKeywords.All))
            WriteEvent(6, expectedSerial, actualSerial);
    }

    [Event(7, Level = EventLevel.Informational, Message = "Cached volume handle for serial number 0x{0:X16}")]
    public void VolumeHandleCached(ulong volumeSerialNumber)
    {
        if (IsEnabled(EventLevel.Informational, EventKeywords.All))
            WriteEvent(7, volumeSerialNumber);
    }

    [Event(8, Level = EventLevel.Warning, Message = "No volume found with serial number 0x{0:X16}")]
    public void VolumeNotFound(ulong volumeSerialNumber)
    {
        if (IsEnabled(EventLevel.Warning, EventKeywords.All))
            WriteEvent(8, volumeSerialNumber);
    }

    [Event(9, Level = EventLevel.Verbose, Message = "Opening file by ID: Volume=0x{0:X16}, FileIdLow=0x{1:X16}, FileIdHigh=0x{2:X16}")]
    public void OpeningFileById(ulong volumeSerialNumber, ulong fileIdLow, ulong fileIdHigh)
    {
        if (IsEnabled(EventLevel.Verbose, EventKeywords.All))
            WriteEvent(9, volumeSerialNumber, fileIdLow, fileIdHigh);
    }

    [Event(10, Level = EventLevel.Informational, Message = "Successfully opened file by ID: Volume=0x{0:X16}, FileIdLow=0x{1:X16}, FileIdHigh=0x{2:X16}")]
    public void FileOpenedById(ulong volumeSerialNumber, ulong fileIdLow, ulong fileIdHigh)
    {
        if (IsEnabled(EventLevel.Informational, EventKeywords.All))
            WriteEvent(10, volumeSerialNumber, fileIdLow, fileIdHigh);
    }

    [Event(11, Level = EventLevel.Error, Message = "Failed to get volume information for volume {0}: Error {1} - {2}")]
    public void VolumeInformationFailed(string volumePath, int errorCode, string errorMessage)
    {
        if (IsEnabled(EventLevel.Error, EventKeywords.All))
            WriteEvent(11, volumePath, errorCode, errorMessage);
    }

    [Event(12, Level = EventLevel.Error, Message = "Failed to open volume handle for path {0}: Error {1} - {2}")]
    public void VolumeHandleOpenFailed(string volumePath, int errorCode, string errorMessage)
    {
        if (IsEnabled(EventLevel.Error, EventKeywords.All))
            WriteEvent(12, volumePath, errorCode, errorMessage);
    }

    [Event(13, Level = EventLevel.Error, Message = "Failed to get file information by handle for volume {0}: Error {1} - {2}")]
    public void GetFileInformationFailed(string volumePath, int errorCode, string errorMessage)
    {
        if (IsEnabled(EventLevel.Error, EventKeywords.All))
            WriteEvent(13, volumePath, errorCode, errorMessage);
    }
}
