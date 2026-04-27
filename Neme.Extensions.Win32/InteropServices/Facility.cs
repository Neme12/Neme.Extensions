namespace Neme.Extensions.Win32.InteropServices;

#pragma warning disable CA1028 // Enum Storage should be Int32
public enum Facility : ushort
#pragma warning restore CA1028 // Enum Storage should be Int32
{
    // Values from https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-erref/0642cb2f-2075-4469-918c-4441e69c548a
#pragma warning disable CA1707 // Identifiers should not contain underscores
    NULL = 0,
    RPC = 1,
    DISPATCH = 2,
    STORAGE = 3,
    ITF = 4,
    WIN32 = 7,
    WINDOWS = 8,
    SECURITY = 9,
#pragma warning disable CA1069 // Enums values should not be duplicated
    SSPI = 9,
#pragma warning restore CA1069 // Enums values should not be duplicated
    CONTROL = 10,
    CERT = 11,
    INTERNET = 12,
    MEDIASERVER = 13,
    MSMQ = 14,
    SETUPAPI = 15,
    SCARD = 16,
    COMPLUS = 17,
    AAF = 18,
    URT = 19,
    ACS = 20,
    DPLAY = 21,
    UMI = 22,
    SXS = 23,
    WINDOWS_CE = 24,
    HTTP = 25,
    USERMODE_COMMONLOG = 26,
    USERMODE_FILTER_MANAGER = 31,
    BACKGROUNDCOPY = 32,
    CONFIGURATION = 33,
    STATE_MANAGEMENT = 34,
    METADIRECTORY = 35,
    WINDOWSUPDATE = 36,
    DIRECTORYSERVICE = 37,
    GRAPHICS = 38,
    SHELL = 39,
    TPM_SERVICES = 40,
    TPM_SOFTWARE = 41,
    PLA = 48,
    FVE = 49,
    FWP = 50,
    WINRM = 51,
    NDIS = 52,
    USERMODE_HYPERVISOR = 53,
    CMI = 54,
    USERMODE_VIRTUALIZATION = 55,
    USERMODE_VOLMGR = 56,
    BCD = 57,
    USERMODE_VHD = 58,
    SDIAG = 60,
    WEBSERVICES = 61,
    WINDOWS_DEFENDER = 80,
    OPC = 81,
#pragma warning restore CA1707 // Identifiers should not contain underscores
}
