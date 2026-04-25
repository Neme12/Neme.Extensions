using System.Collections.Frozen;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Neme.Extensions.InteropServices;

public readonly partial struct HResult
{
    private static readonly FrozenDictionary<int, (string name, string? description)> _constants =
        typeof(HResult)
            .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(p => p.PropertyType == typeof(HResult) && p.GetMethod is not null && p.SetMethod is null)
            .GroupBy(p => p.GetCustomAttribute<HResultConstantAttribute>()!.Value.Value)
            .ToFrozenDictionary(p => p.Key, p =>
            {
                var property = p.First();
                return (property.Name, property.GetCustomAttribute<DescriptionAttribute>()?.Description);
            });

#pragma warning disable CA1707 // Identifiers should not contain underscores

    // Array.from(temp1.children).map(s => `/// <summary>${s.children[1].innerText}</summary>\n[Description("""${s.children[1].innerText}""")]\n[HResultConstant(${s.children[2].innerText})]\npublic static HResult ${s.children[0].innerText} => new(${s.children[2].innerText});`).join('\n\n')
    #region Values from https://learn.microsoft.com/en-us/windows/win32/seccrypto/common-hresult-values
    /// <summary>Operation successful</summary>
    [Description("""Operation successful""")]
    [HResultConstant(0x00000000)]
    public static HResult S_OK => new(0x00000000);

    /// <summary>Operation aborted</summary>
    [Description("""Operation aborted""")]
    [HResultConstant(0x80004004)]
    public static HResult E_ABORT => new(0x80004004);

    /// <summary>General access denied error</summary>
    [Description("""General access denied error""")]
    [HResultConstant(0x80070005)]
    public static HResult E_ACCESSDENIED => new(0x80070005);

    /// <summary>Unspecified failure</summary>
    [Description("""Unspecified failure""")]
    [HResultConstant(0x80004005)]
    public static HResult E_FAIL => new(0x80004005);

    /// <summary>Handle that is not valid</summary>
    [Description("""Handle that is not valid""")]
    [HResultConstant(0x80070006)]
    public static HResult E_HANDLE => new(0x80070006);

    /// <summary>One or more arguments are not valid</summary>
    [Description("""One or more arguments are not valid""")]
    [HResultConstant(0x80070057)]
    public static HResult E_INVALIDARG => new(0x80070057);

    /// <summary>No such interface supported</summary>
    [Description("""No such interface supported""")]
    [HResultConstant(0x80004002)]
    public static HResult E_NOINTERFACE => new(0x80004002);

    /// <summary>Not implemented</summary>
    [Description("""Not implemented""")]
    [HResultConstant(0x80004001)]
    public static HResult E_NOTIMPL => new(0x80004001);

    /// <summary>Failed to allocate necessary memory</summary>
    [Description("""Failed to allocate necessary memory""")]
    [HResultConstant(0x8007000E)]
    public static HResult E_OUTOFMEMORY => new(0x8007000E);

    /// <summary>Pointer that is not valid</summary>
    [Description("""Pointer that is not valid""")]
    [HResultConstant(0x80004003)]
    public static HResult E_POINTER => new(0x80004003);

    /// <summary>Unexpected failure</summary>
    [Description("""Unexpected failure""")]
    [HResultConstant(0x8000FFFF)]
    public static HResult E_UNEXPECTED => new(0x8000FFFF);
    #endregion

    // Array.from(temp1.children).map(s => `/// <summary>${s.children[1].children[0].innerText}</summary>\n[Description("""${s.children[1].children[0].innerText}""")]\n[HResultConstant(${s.children[0].children[0].innerText})]\npublic static HResult ${s.children[0].children[1].innerText} => new(${s.children[0].children[0].innerText});`).join('\n\n')
    #region Values from https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-erref/705fb797-2175-4a90-b5a3-3918024b10b8
    /// <summary>The underlying file was converted to compound file format.</summary>
    [Description("""The underlying file was converted to compound file format.""")]
    [HResultConstant(0x00030200)]
    public static HResult STG_S_CONVERTED => new(0x00030200);

    /// <summary>The storage operation should block until more data is available.</summary>
    [Description("""The storage operation should block until more data is available.""")]
    [HResultConstant(0x00030201)]
    public static HResult STG_S_BLOCK => new(0x00030201);

    /// <summary>The storage operation should retry immediately.</summary>
    [Description("""The storage operation should retry immediately.""")]
    [HResultConstant(0x00030202)]
    public static HResult STG_S_RETRYNOW => new(0x00030202);

    /// <summary>The notified event sink will not influence the storage operation.</summary>
    [Description("""The notified event sink will not influence the storage operation.""")]
    [HResultConstant(0x00030203)]
    public static HResult STG_S_MONITORING => new(0x00030203);

    /// <summary>Multiple opens prevent consolidated (commit succeeded).</summary>
    [Description("""Multiple opens prevent consolidated (commit succeeded).""")]
    [HResultConstant(0x00030204)]
    public static HResult STG_S_MULTIPLEOPENS => new(0x00030204);

    /// <summary>Consolidation of the storage file failed (commit succeeded).</summary>
    [Description("""Consolidation of the storage file failed (commit succeeded).""")]
    [HResultConstant(0x00030205)]
    public static HResult STG_S_CONSOLIDATIONFAILED => new(0x00030205);

    /// <summary>Consolidation of the storage file is inappropriate (commit succeeded).</summary>
    [Description("""Consolidation of the storage file is inappropriate (commit succeeded).""")]
    [HResultConstant(0x00030206)]
    public static HResult STG_S_CANNOTCONSOLIDATE => new(0x00030206);

    /// <summary>Use the registry database to provide the requested information.</summary>
    [Description("""Use the registry database to provide the requested information.""")]
    [HResultConstant(0x00040000)]
    public static HResult OLE_S_USEREG => new(0x00040000);

    /// <summary>Success, but static.</summary>
    [Description("""Success, but static.""")]
    [HResultConstant(0x00040001)]
    public static HResult OLE_S_STATIC => new(0x00040001);

    /// <summary>Macintosh clipboard format.</summary>
    [Description("""Macintosh clipboard format.""")]
    [HResultConstant(0x00040002)]
    public static HResult OLE_S_MAC_CLIPFORMAT => new(0x00040002);

    /// <summary>Successful drop took place.</summary>
    [Description("""Successful drop took place.""")]
    [HResultConstant(0x00040100)]
    public static HResult DRAGDROP_S_DROP => new(0x00040100);

    /// <summary>Drag-drop operation canceled.</summary>
    [Description("""Drag-drop operation canceled.""")]
    [HResultConstant(0x00040101)]
    public static HResult DRAGDROP_S_CANCEL => new(0x00040101);

    /// <summary>Use the default cursor.</summary>
    [Description("""Use the default cursor.""")]
    [HResultConstant(0x00040102)]
    public static HResult DRAGDROP_S_USEDEFAULTCURSORS => new(0x00040102);

    /// <summary>Data has same FORMATETC.</summary>
    [Description("""Data has same FORMATETC.""")]
    [HResultConstant(0x00040130)]
    public static HResult DATA_S_SAMEFORMATETC => new(0x00040130);

    /// <summary>View is already frozen.</summary>
    [Description("""View is already frozen.""")]
    [HResultConstant(0x00040140)]
    public static HResult VIEW_S_ALREADY_FROZEN => new(0x00040140);

    /// <summary>FORMATETC not supported.</summary>
    [Description("""FORMATETC not supported.""")]
    [HResultConstant(0x00040170)]
    public static HResult CACHE_S_FORMATETC_NOTSUPPORTED => new(0x00040170);

    /// <summary>Same cache.</summary>
    [Description("""Same cache.""")]
    [HResultConstant(0x00040171)]
    public static HResult CACHE_S_SAMECACHE => new(0x00040171);

    /// <summary>Some caches are not updated.</summary>
    [Description("""Some caches are not updated.""")]
    [HResultConstant(0x00040172)]
    public static HResult CACHE_S_SOMECACHES_NOTUPDATED => new(0x00040172);

    /// <summary>Invalid verb for OLE object.</summary>
    [Description("""Invalid verb for OLE object.""")]
    [HResultConstant(0x00040180)]
    public static HResult OLEOBJ_S_INVALIDVERB => new(0x00040180);

    /// <summary>Verb number is valid but verb cannot be done now.</summary>
    [Description("""Verb number is valid but verb cannot be done now.""")]
    [HResultConstant(0x00040181)]
    public static HResult OLEOBJ_S_CANNOT_DOVERB_NOW => new(0x00040181);

    /// <summary>Invalid window handle passed.</summary>
    [Description("""Invalid window handle passed.""")]
    [HResultConstant(0x00040182)]
    public static HResult OLEOBJ_S_INVALIDHWND => new(0x00040182);

    /// <summary>Message is too long; some of it had to be truncated before displaying.</summary>
    [Description("""Message is too long; some of it had to be truncated before displaying.""")]
    [HResultConstant(0x000401A0)]
    public static HResult INPLACE_S_TRUNCATED => new(0x000401A0);

    /// <summary>Unable to convert OLESTREAM to IStorage.</summary>
    [Description("""Unable to convert OLESTREAM to IStorage.""")]
    [HResultConstant(0x000401C0)]
    public static HResult CONVERT10_S_NO_PRESENTATION => new(0x000401C0);

    /// <summary>Moniker reduced to itself.</summary>
    [Description("""Moniker reduced to itself.""")]
    [HResultConstant(0x000401E2)]
    public static HResult MK_S_REDUCED_TO_SELF => new(0x000401E2);

    /// <summary>Common prefix is this moniker.</summary>
    [Description("""Common prefix is this moniker.""")]
    [HResultConstant(0x000401E4)]
    public static HResult MK_S_ME => new(0x000401E4);

    /// <summary>Common prefix is input moniker.</summary>
    [Description("""Common prefix is input moniker.""")]
    [HResultConstant(0x000401E5)]
    public static HResult MK_S_HIM => new(0x000401E5);

    /// <summary>Common prefix is both monikers.</summary>
    [Description("""Common prefix is both monikers.""")]
    [HResultConstant(0x000401E6)]
    public static HResult MK_S_US => new(0x000401E6);

    /// <summary>Moniker is already registered in running object table.</summary>
    [Description("""Moniker is already registered in running object table.""")]
    [HResultConstant(0x000401E7)]
    public static HResult MK_S_MONIKERALREADYREGISTERED => new(0x000401E7);

    /// <summary>An event was able to invoke some, but not all, of the subscribers.</summary>
    [Description("""An event was able to invoke some, but not all, of the subscribers.""")]
    [HResultConstant(0x00040200)]
    public static HResult EVENT_S_SOME_SUBSCRIBERS_FAILED => new(0x00040200);

    /// <summary>An event was delivered, but there were no subscribers.</summary>
    [Description("""An event was delivered, but there were no subscribers.""")]
    [HResultConstant(0x00040202)]
    public static HResult EVENT_S_NOSUBSCRIBERS => new(0x00040202);

    /// <summary>The task is ready to run at its next scheduled time.</summary>
    [Description("""The task is ready to run at its next scheduled time.""")]
    [HResultConstant(0x00041300)]
    public static HResult SCHED_S_TASK_READY => new(0x00041300);

    /// <summary>The task is currently running.</summary>
    [Description("""The task is currently running.""")]
    [HResultConstant(0x00041301)]
    public static HResult SCHED_S_TASK_RUNNING => new(0x00041301);

    /// <summary>The task will not run at the scheduled times because it has been disabled.</summary>
    [Description("""The task will not run at the scheduled times because it has been disabled.""")]
    [HResultConstant(0x00041302)]
    public static HResult SCHED_S_TASK_DISABLED => new(0x00041302);

    /// <summary>The task has not yet run.</summary>
    [Description("""The task has not yet run.""")]
    [HResultConstant(0x00041303)]
    public static HResult SCHED_S_TASK_HAS_NOT_RUN => new(0x00041303);

    /// <summary>There are no more runs scheduled for this task.</summary>
    [Description("""There are no more runs scheduled for this task.""")]
    [HResultConstant(0x00041304)]
    public static HResult SCHED_S_TASK_NO_MORE_RUNS => new(0x00041304);

    /// <summary>One or more of the properties that are needed to run this task on a schedule have not been set.</summary>
    [Description("""One or more of the properties that are needed to run this task on a schedule have not been set.""")]
    [HResultConstant(0x00041305)]
    public static HResult SCHED_S_TASK_NOT_SCHEDULED => new(0x00041305);

    /// <summary>The last run of the task was terminated by the user.</summary>
    [Description("""The last run of the task was terminated by the user.""")]
    [HResultConstant(0x00041306)]
    public static HResult SCHED_S_TASK_TERMINATED => new(0x00041306);

    /// <summary>Either the task has no triggers, or the existing triggers are disabled or not set.</summary>
    [Description("""Either the task has no triggers, or the existing triggers are disabled or not set.""")]
    [HResultConstant(0x00041307)]
    public static HResult SCHED_S_TASK_NO_VALID_TRIGGERS => new(0x00041307);

    /// <summary>Event triggers do not have set run times.</summary>
    [Description("""Event triggers do not have set run times.""")]
    [HResultConstant(0x00041308)]
    public static HResult SCHED_S_EVENT_TRIGGER => new(0x00041308);

    /// <summary>The task is registered, but not all specified triggers will start the task.</summary>
    [Description("""The task is registered, but not all specified triggers will start the task.""")]
    [HResultConstant(0x0004131B)]
    public static HResult SCHED_S_SOME_TRIGGERS_FAILED => new(0x0004131B);

    /// <summary>The task is registered, but it might fail to start. Batch logon privilege needs to be enabled for the task principal.</summary>
    [Description("""The task is registered, but it might fail to start. Batch logon privilege needs to be enabled for the task principal.""")]
    [HResultConstant(0x0004131C)]
    public static HResult SCHED_S_BATCH_LOGON_PROBLEM => new(0x0004131C);

    /// <summary>An asynchronous operation was specified. The operation has begun, but its outcome is not known yet.</summary>
    [Description("""An asynchronous operation was specified. The operation has begun, but its outcome is not known yet.""")]
    [HResultConstant(0x0004D000)]
    public static HResult XACT_S_ASYNC => new(0x0004D000);

    /// <summary>The method call succeeded because the transaction was read-only.</summary>
    [Description("""The method call succeeded because the transaction was read-only.""")]
    [HResultConstant(0x0004D002)]
    public static HResult XACT_S_READONLY => new(0x0004D002);

    /// <summary>The transaction was successfully aborted. However, this is a coordinated transaction, and a number of enlisted resources were aborted outright because they could not support abort-retaining semantics.</summary>
    [Description("""The transaction was successfully aborted. However, this is a coordinated transaction, and a number of enlisted resources were aborted outright because they could not support abort-retaining semantics.""")]
    [HResultConstant(0x0004D003)]
    public static HResult XACT_S_SOMENORETAIN => new(0x0004D003);

    /// <summary>No changes were made during this call, but the sink wants another chance to look if any other sinks make further changes.</summary>
    [Description("""No changes were made during this call, but the sink wants another chance to look if any other sinks make further changes.""")]
    [HResultConstant(0x0004D004)]
    public static HResult XACT_S_OKINFORM => new(0x0004D004);

    /// <summary>The sink is content and wants the transaction to proceed. Changes were made to one or more resources during this call.</summary>
    [Description("""The sink is content and wants the transaction to proceed. Changes were made to one or more resources during this call.""")]
    [HResultConstant(0x0004D005)]
    public static HResult XACT_S_MADECHANGESCONTENT => new(0x0004D005);

    /// <summary>The sink is for the moment and wants the transaction to proceed, but if other changes are made following this return by other event sinks, this sink wants another chance to look.</summary>
    [Description("""The sink is for the moment and wants the transaction to proceed, but if other changes are made following this return by other event sinks, this sink wants another chance to look.""")]
    [HResultConstant(0x0004D006)]
    public static HResult XACT_S_MADECHANGESINFORM => new(0x0004D006);

    /// <summary>The transaction was successfully aborted. However, the abort was nonretaining.</summary>
    [Description("""The transaction was successfully aborted. However, the abort was nonretaining.""")]
    [HResultConstant(0x0004D007)]
    public static HResult XACT_S_ALLNORETAIN => new(0x0004D007);

    /// <summary>An abort operation was already in progress.</summary>
    [Description("""An abort operation was already in progress.""")]
    [HResultConstant(0x0004D008)]
    public static HResult XACT_S_ABORTING => new(0x0004D008);

    /// <summary>The resource manager has performed a single-phase commit of the transaction.</summary>
    [Description("""The resource manager has performed a single-phase commit of the transaction.""")]
    [HResultConstant(0x0004D009)]
    public static HResult XACT_S_SINGLEPHASE => new(0x0004D009);

    /// <summary>The local transaction has not aborted.</summary>
    [Description("""The local transaction has not aborted.""")]
    [HResultConstant(0x0004D00A)]
    public static HResult XACT_S_LOCALLY_OK => new(0x0004D00A);

    /// <summary>The resource manager has requested to be the coordinator (last resource manager) for the transaction.</summary>
    [Description("""The resource manager has requested to be the coordinator (last resource manager) for the transaction.""")]
    [HResultConstant(0x0004D010)]
    public static HResult XACT_S_LASTRESOURCEMANAGER => new(0x0004D010);

    /// <summary>Not all the requested interfaces were available.</summary>
    [Description("""Not all the requested interfaces were available.""")]
    [HResultConstant(0x00080012)]
    public static HResult CO_S_NOTALLINTERFACES => new(0x00080012);

    /// <summary>The specified machine name was not found in the cache.</summary>
    [Description("""The specified machine name was not found in the cache.""")]
    [HResultConstant(0x00080013)]
    public static HResult CO_S_MACHINENAMENOTFOUND => new(0x00080013);

    /// <summary>The function completed successfully, but it must be called again to complete the context.</summary>
    [Description("""The function completed successfully, but it must be called again to complete the context.""")]
    [HResultConstant(0x00090312)]
    public static HResult SEC_I_CONTINUE_NEEDED => new(0x00090312);

    /// <summary>The function completed successfully, but CompleteToken must be called.</summary>
    [Description("""The function completed successfully, but CompleteToken must be called.""")]
    [HResultConstant(0x00090313)]
    public static HResult SEC_I_COMPLETE_NEEDED => new(0x00090313);

    /// <summary>The function completed successfully, but both CompleteToken and this function must be called to complete the context.</summary>
    [Description("""The function completed successfully, but both CompleteToken and this function must be called to complete the context.""")]
    [HResultConstant(0x00090314)]
    public static HResult SEC_I_COMPLETE_AND_CONTINUE => new(0x00090314);

    /// <summary>The logon was completed, but no network authority was available. The logon was made using locally known information.</summary>
    [Description("""The logon was completed, but no network authority was available. The logon was made using locally known information.""")]
    [HResultConstant(0x00090315)]
    public static HResult SEC_I_LOCAL_LOGON => new(0x00090315);

    /// <summary>The context has expired and can no longer be used.</summary>
    [Description("""The context has expired and can no longer be used.""")]
    [HResultConstant(0x00090317)]
    public static HResult SEC_I_CONTEXT_EXPIRED => new(0x00090317);

    /// <summary>The credentials supplied were not complete and could not be verified. Additional information can be returned from the context.</summary>
    [Description("""The credentials supplied were not complete and could not be verified. Additional information can be returned from the context.""")]
    [HResultConstant(0x00090320)]
    public static HResult SEC_I_INCOMPLETE_CREDENTIALS => new(0x00090320);

    /// <summary>The context data must be renegotiated with the peer.</summary>
    [Description("""The context data must be renegotiated with the peer.""")]
    [HResultConstant(0x00090321)]
    public static HResult SEC_I_RENEGOTIATE => new(0x00090321);

    /// <summary>There is no LSA mode context associated with this context.</summary>
    [Description("""There is no LSA mode context associated with this context.""")]
    [HResultConstant(0x00090323)]
    public static HResult SEC_I_NO_LSA_CONTEXT => new(0x00090323);

    /// <summary>A signature operation must be performed before the user can authenticate.</summary>
    [Description("""A signature operation must be performed before the user can authenticate.""")]
    [HResultConstant(0x0009035C)]
    public static HResult SEC_I_SIGNATURE_NEEDED => new(0x0009035C);

    /// <summary>The protected data needs to be reprotected.</summary>
    [Description("""The protected data needs to be reprotected.""")]
    [HResultConstant(0x00091012)]
    public static HResult CRYPT_I_NEW_PROTECTION_REQUIRED => new(0x00091012);

    /// <summary>The requested operation is pending completion.</summary>
    [Description("""The requested operation is pending completion.""")]
    [HResultConstant(0x000D0000)]
    public static HResult NS_S_CALLPENDING => new(0x000D0000);

    /// <summary>The requested operation was aborted by the client.</summary>
    [Description("""The requested operation was aborted by the client.""")]
    [HResultConstant(0x000D0001)]
    public static HResult NS_S_CALLABORTED => new(0x000D0001);

    /// <summary>The stream was purposefully stopped before completion.</summary>
    [Description("""The stream was purposefully stopped before completion.""")]
    [HResultConstant(0x000D0002)]
    public static HResult NS_S_STREAM_TRUNCATED => new(0x000D0002);

    /// <summary>The requested operation has caused the source to rebuffer.</summary>
    [Description("""The requested operation has caused the source to rebuffer.""")]
    [HResultConstant(0x000D0BC8)]
    public static HResult NS_S_REBUFFERING => new(0x000D0BC8);

    /// <summary>The requested operation has caused the source to degrade codec quality.</summary>
    [Description("""The requested operation has caused the source to degrade codec quality.""")]
    [HResultConstant(0x000D0BC9)]
    public static HResult NS_S_DEGRADING_QUALITY => new(0x000D0BC9);

    /// <summary>The transcryptor object has reached end of file.</summary>
    [Description("""The transcryptor object has reached end of file.""")]
    [HResultConstant(0x000D0BDB)]
    public static HResult NS_S_TRANSCRYPTOR_EOF => new(0x000D0BDB);

    /// <summary>An upgrade is needed for the theme manager to correctly show this skin. Skin reports version: %.1f.</summary>
    [Description("""An upgrade is needed for the theme manager to correctly show this skin. Skin reports version: %.1f.""")]
    [HResultConstant(0x000D0FE8)]
    public static HResult NS_S_WMP_UI_VERSIONMISMATCH => new(0x000D0FE8);

    /// <summary>An error occurred in one of the UI components.</summary>
    [Description("""An error occurred in one of the UI components.""")]
    [HResultConstant(0x000D0FE9)]
    public static HResult NS_S_WMP_EXCEPTION => new(0x000D0FE9);

    /// <summary>Successfully loaded a GIF file.</summary>
    [Description("""Successfully loaded a GIF file.""")]
    [HResultConstant(0x000D1040)]
    public static HResult NS_S_WMP_LOADED_GIF_IMAGE => new(0x000D1040);

    /// <summary>Successfully loaded a PNG file.</summary>
    [Description("""Successfully loaded a PNG file.""")]
    [HResultConstant(0x000D1041)]
    public static HResult NS_S_WMP_LOADED_PNG_IMAGE => new(0x000D1041);

    /// <summary>Successfully loaded a BMP file.</summary>
    [Description("""Successfully loaded a BMP file.""")]
    [HResultConstant(0x000D1042)]
    public static HResult NS_S_WMP_LOADED_BMP_IMAGE => new(0x000D1042);

    /// <summary>Successfully loaded a JPG file.</summary>
    [Description("""Successfully loaded a JPG file.""")]
    [HResultConstant(0x000D1043)]
    public static HResult NS_S_WMP_LOADED_JPG_IMAGE => new(0x000D1043);

    /// <summary>Drop this frame.</summary>
    [Description("""Drop this frame.""")]
    [HResultConstant(0x000D104F)]
    public static HResult NS_S_WMG_FORCE_DROP_FRAME => new(0x000D104F);

    /// <summary>The specified stream has already been rendered.</summary>
    [Description("""The specified stream has already been rendered.""")]
    [HResultConstant(0x000D105F)]
    public static HResult NS_S_WMR_ALREADYRENDERED => new(0x000D105F);

    /// <summary>The specified type partially matches this pin type.</summary>
    [Description("""The specified type partially matches this pin type.""")]
    [HResultConstant(0x000D1060)]
    public static HResult NS_S_WMR_PINTYPEPARTIALMATCH => new(0x000D1060);

    /// <summary>The specified type fully matches this pin type.</summary>
    [Description("""The specified type fully matches this pin type.""")]
    [HResultConstant(0x000D1061)]
    public static HResult NS_S_WMR_PINTYPEFULLMATCH => new(0x000D1061);

    /// <summary>The timestamp is late compared to the current render position. Advise dropping this frame.</summary>
    [Description("""The timestamp is late compared to the current render position. Advise dropping this frame.""")]
    [HResultConstant(0x000D1066)]
    public static HResult NS_S_WMG_ADVISE_DROP_FRAME => new(0x000D1066);

    /// <summary>The timestamp is severely late compared to the current render position. Advise dropping everything up to the next key frame.</summary>
    [Description("""The timestamp is severely late compared to the current render position. Advise dropping everything up to the next key frame.""")]
    [HResultConstant(0x000D1067)]
    public static HResult NS_S_WMG_ADVISE_DROP_TO_KEYFRAME => new(0x000D1067);

    /// <summary>No burn rights. You will be prompted to buy burn rights when you try to burn this file to an audio CD.</summary>
    [Description("""No burn rights. You will be prompted to buy burn rights when you try to burn this file to an audio CD.""")]
    [HResultConstant(0x000D10DB)]
    public static HResult NS_S_NEED_TO_BUY_BURN_RIGHTS => new(0x000D10DB);

    /// <summary>Failed to clear playlist because it was aborted by user.</summary>
    [Description("""Failed to clear playlist because it was aborted by user.""")]
    [HResultConstant(0x000D10FE)]
    public static HResult NS_S_WMPCORE_PLAYLISTCLEARABORT => new(0x000D10FE);

    /// <summary>Failed to remove item in the playlist since it was aborted by user.</summary>
    [Description("""Failed to remove item in the playlist since it was aborted by user.""")]
    [HResultConstant(0x000D10FF)]
    public static HResult NS_S_WMPCORE_PLAYLISTREMOVEITEMABORT => new(0x000D10FF);

    /// <summary>Playlist is being generated asynchronously.</summary>
    [Description("""Playlist is being generated asynchronously.""")]
    [HResultConstant(0x000D1102)]
    public static HResult NS_S_WMPCORE_PLAYLIST_CREATION_PENDING => new(0x000D1102);

    /// <summary>Validation of the media is pending.</summary>
    [Description("""Validation of the media is pending.""")]
    [HResultConstant(0x000D1103)]
    public static HResult NS_S_WMPCORE_MEDIA_VALIDATION_PENDING => new(0x000D1103);

    /// <summary>Encountered more than one Repeat block during ASX processing.</summary>
    [Description("""Encountered more than one Repeat block during ASX processing.""")]
    [HResultConstant(0x000D1104)]
    public static HResult NS_S_WMPCORE_PLAYLIST_REPEAT_SECONDARY_SEGMENTS_IGNORED => new(0x000D1104);

    /// <summary>Current state of WMP disallows calling this method or property.</summary>
    [Description("""Current state of WMP disallows calling this method or property.""")]
    [HResultConstant(0x000D1105)]
    public static HResult NS_S_WMPCORE_COMMAND_NOT_AVAILABLE => new(0x000D1105);

    /// <summary>Name for the playlist has been auto generated.</summary>
    [Description("""Name for the playlist has been auto generated.""")]
    [HResultConstant(0x000D1106)]
    public static HResult NS_S_WMPCORE_PLAYLIST_NAME_AUTO_GENERATED => new(0x000D1106);

    /// <summary>The imported playlist does not contain all items from the original.</summary>
    [Description("""The imported playlist does not contain all items from the original.""")]
    [HResultConstant(0x000D1107)]
    public static HResult NS_S_WMPCORE_PLAYLIST_IMPORT_MISSING_ITEMS => new(0x000D1107);

    /// <summary>The M3U playlist has been ignored because it only contains one item.</summary>
    [Description("""The M3U playlist has been ignored because it only contains one item.""")]
    [HResultConstant(0x000D1108)]
    public static HResult NS_S_WMPCORE_PLAYLIST_COLLAPSED_TO_SINGLE_MEDIA => new(0x000D1108);

    /// <summary>The open for the child playlist associated with this media is pending.</summary>
    [Description("""The open for the child playlist associated with this media is pending.""")]
    [HResultConstant(0x000D1109)]
    public static HResult NS_S_WMPCORE_MEDIA_CHILD_PLAYLIST_OPEN_PENDING => new(0x000D1109);

    /// <summary>More nodes support the interface requested, but the array for returning them is full.</summary>
    [Description("""More nodes support the interface requested, but the array for returning them is full.""")]
    [HResultConstant(0x000D110A)]
    public static HResult NS_S_WMPCORE_MORE_NODES_AVAIABLE => new(0x000D110A);

    /// <summary>Backup or Restore successful!.</summary>
    [Description("""Backup or Restore successful!.""")]
    [HResultConstant(0x000D1135)]
    public static HResult NS_S_WMPBR_SUCCESS => new(0x000D1135);

    /// <summary>Transfer complete with limitations.</summary>
    [Description("""Transfer complete with limitations.""")]
    [HResultConstant(0x000D1136)]
    public static HResult NS_S_WMPBR_PARTIALSUCCESS => new(0x000D1136);

    /// <summary>Request to the effects control to change transparency status to transparent.</summary>
    [Description("""Request to the effects control to change transparency status to transparent.""")]
    [HResultConstant(0x000D1144)]
    public static HResult NS_S_WMPEFFECT_TRANSPARENT => new(0x000D1144);

    /// <summary>Request to the effects control to change transparency status to opaque.</summary>
    [Description("""Request to the effects control to change transparency status to opaque.""")]
    [HResultConstant(0x000D1145)]
    public static HResult NS_S_WMPEFFECT_OPAQUE => new(0x000D1145);

    /// <summary>The requested application pane is performing an operation and will not be released.</summary>
    [Description("""The requested application pane is performing an operation and will not be released.""")]
    [HResultConstant(0x000D114E)]
    public static HResult NS_S_OPERATION_PENDING => new(0x000D114E);

    /// <summary>The file is only available for purchase when you buy the entire album.</summary>
    [Description("""The file is only available for purchase when you buy the entire album.""")]
    [HResultConstant(0x000D1359)]
    public static HResult NS_S_TRACK_BUY_REQUIRES_ALBUM_PURCHASE => new(0x000D1359);

    /// <summary>There were problems completing the requested navigation. There are identifiers missing in the catalog.</summary>
    [Description("""There were problems completing the requested navigation. There are identifiers missing in the catalog.""")]
    [HResultConstant(0x000D135E)]
    public static HResult NS_S_NAVIGATION_COMPLETE_WITH_ERRORS => new(0x000D135E);

    /// <summary>Track already downloaded.</summary>
    [Description("""Track already downloaded.""")]
    [HResultConstant(0x000D1361)]
    public static HResult NS_S_TRACK_ALREADY_DOWNLOADED => new(0x000D1361);

    /// <summary>The publishing point successfully started, but one or more of the requested data writer plug-ins failed.</summary>
    [Description("""The publishing point successfully started, but one or more of the requested data writer plug-ins failed.""")]
    [HResultConstant(0x000D1519)]
    public static HResult NS_S_PUBLISHING_POINT_STARTED_WITH_FAILED_SINKS => new(0x000D1519);

    /// <summary>Status message: The license was acquired.</summary>
    [Description("""Status message: The license was acquired.""")]
    [HResultConstant(0x000D2726)]
    public static HResult NS_S_DRM_LICENSE_ACQUIRED => new(0x000D2726);

    /// <summary>Status message: The security upgrade has been completed.</summary>
    [Description("""Status message: The security upgrade has been completed.""")]
    [HResultConstant(0x000D2727)]
    public static HResult NS_S_DRM_INDIVIDUALIZED => new(0x000D2727);

    /// <summary>Status message: License monitoring has been canceled.</summary>
    [Description("""Status message: License monitoring has been canceled.""")]
    [HResultConstant(0x000D2746)]
    public static HResult NS_S_DRM_MONITOR_CANCELLED => new(0x000D2746);

    /// <summary>Status message: License acquisition has been canceled.</summary>
    [Description("""Status message: License acquisition has been canceled.""")]
    [HResultConstant(0x000D2747)]
    public static HResult NS_S_DRM_ACQUIRE_CANCELLED => new(0x000D2747);

    /// <summary>The track is burnable and had no playlist burn limit.</summary>
    [Description("""The track is burnable and had no playlist burn limit.""")]
    [HResultConstant(0x000D276E)]
    public static HResult NS_S_DRM_BURNABLE_TRACK => new(0x000D276E);

    /// <summary>The track is burnable but has a playlist burn limit.</summary>
    [Description("""The track is burnable but has a playlist burn limit.""")]
    [HResultConstant(0x000D276F)]
    public static HResult NS_S_DRM_BURNABLE_TRACK_WITH_PLAYLIST_RESTRICTION => new(0x000D276F);

    /// <summary>A security upgrade is required to perform the operation on this media file.</summary>
    [Description("""A security upgrade is required to perform the operation on this media file.""")]
    [HResultConstant(0x000D27DE)]
    public static HResult NS_S_DRM_NEEDS_INDIVIDUALIZATION => new(0x000D27DE);

    /// <summary>Installation was successful; however, some file cleanup is not complete. For best results, restart your computer.</summary>
    [Description("""Installation was successful; however, some file cleanup is not complete. For best results, restart your computer.""")]
    [HResultConstant(0x000D2AF8)]
    public static HResult NS_S_REBOOT_RECOMMENDED => new(0x000D2AF8);

    /// <summary>Installation was successful; however, some file cleanup is not complete. To continue, you must restart your computer.</summary>
    [Description("""Installation was successful; however, some file cleanup is not complete. To continue, you must restart your computer.""")]
    [HResultConstant(0x000D2AF9)]
    public static HResult NS_S_REBOOT_REQUIRED => new(0x000D2AF9);

    /// <summary>EOS hit during rewinding.</summary>
    [Description("""EOS hit during rewinding.""")]
    [HResultConstant(0x000D2F09)]
    public static HResult NS_S_EOSRECEDING => new(0x000D2F09);

    /// <summary>Internal.</summary>
    [Description("""Internal.""")]
    [HResultConstant(0x000D2F0D)]
    public static HResult NS_S_CHANGENOTICE => new(0x000D2F0D);

    /// <summary>The IO was completed by a filter.</summary>
    [Description("""The IO was completed by a filter.""")]
    [HResultConstant(0x001F0001)]
    public static HResult ERROR_FLT_IO_COMPLETE => new(0x001F0001);

    /// <summary>No mode is pinned on the specified VidPN source or target.</summary>
    [Description("""No mode is pinned on the specified VidPN source or target.""")]
    [HResultConstant(0x00262307)]
    public static HResult ERROR_GRAPHICS_MODE_NOT_PINNED => new(0x00262307);

    /// <summary>Specified mode set does not specify preference for one of its modes.</summary>
    [Description("""Specified mode set does not specify preference for one of its modes.""")]
    [HResultConstant(0x0026231E)]
    public static HResult ERROR_GRAPHICS_NO_PREFERRED_MODE => new(0x0026231E);

    /// <summary>Specified data set (for example, mode set, frequency range set, descriptor set, and topology) is empty.</summary>
    [Description("""Specified data set (for example, mode set, frequency range set, descriptor set, and topology) is empty.""")]
    [HResultConstant(0x0026234B)]
    public static HResult ERROR_GRAPHICS_DATASET_IS_EMPTY => new(0x0026234B);

    /// <summary>Specified data set (for example, mode set, frequency range set, descriptor set, and topology) does not contain any more elements.</summary>
    [Description("""Specified data set (for example, mode set, frequency range set, descriptor set, and topology) does not contain any more elements.""")]
    [HResultConstant(0x0026234C)]
    public static HResult ERROR_GRAPHICS_NO_MORE_ELEMENTS_IN_DATASET => new(0x0026234C);

    /// <summary>Specified content transformation is not pinned on the specified VidPN present path.</summary>
    [Description("""Specified content transformation is not pinned on the specified VidPN present path.""")]
    [HResultConstant(0x00262351)]
    public static HResult ERROR_GRAPHICS_PATH_CONTENT_GEOMETRY_TRANSFORMATION_NOT_PINNED => new(0x00262351);

    /// <summary>Property value will be ignored.</summary>
    [Description("""Property value will be ignored.""")]
    [HResultConstant(0x00300100)]
    public static HResult PLA_S_PROPERTY_IGNORED => new(0x00300100);

    /// <summary>The request will be completed later by a Network Driver Interface Specification (NDIS) status indication.</summary>
    [Description("""The request will be completed later by a Network Driver Interface Specification (NDIS) status indication.""")]
    [HResultConstant(0x00340001)]
    public static HResult ERROR_NDIS_INDICATION_REQUIRED => new(0x00340001);

    /// <summary>The VolumeSequenceNumber of a MOVE_NOTIFICATION request is incorrect.</summary>
    [Description("""The VolumeSequenceNumber of a MOVE_NOTIFICATION request is incorrect.""")]
    [HResultConstant(0x0DEAD100)]
    public static HResult TRK_S_OUT_OF_SYNC => new(0x0DEAD100);

    /// <summary>The VolumeID in a request was not found in the server's ServerVolumeTable.</summary>
    [Description("""The VolumeID in a request was not found in the server's ServerVolumeTable.""")]
    [HResultConstant(0x0DEAD102)]
    public static HResult TRK_VOLUME_NOT_FOUND => new(0x0DEAD102);

    /// <summary>A notification was sent to the LnkSvrMessage method, but the RequestMachine for the request was not the VolumeOwner for a VolumeID in the request.</summary>
    [Description("""A notification was sent to the LnkSvrMessage method, but the RequestMachine for the request was not the VolumeOwner for a VolumeID in the request.""")]
    [HResultConstant(0x0DEAD103)]
    public static HResult TRK_VOLUME_NOT_OWNED => new(0x0DEAD103);

    /// <summary>The server received a MOVE_NOTIFICATION request, but the FileTable size limit has already been reached.</summary>
    [Description("""The server received a MOVE_NOTIFICATION request, but the FileTable size limit has already been reached.""")]
    [HResultConstant(0x0DEAD107)]
    public static HResult TRK_S_NOTIFICATION_QUOTA_EXCEEDED => new(0x0DEAD107);

    /// <summary>The Title Server %1 is running.</summary>
    [Description("""The Title Server %1 is running.""")]
    [HResultConstant(0x400D004F)]
    public static HResult NS_I_TIGER_START => new(0x400D004F);

    /// <summary>Content Server %1 (%2) is starting.</summary>
    [Description("""Content Server %1 (%2) is starting.""")]
    [HResultConstant(0x400D0051)]
    public static HResult NS_I_CUB_START => new(0x400D0051);

    /// <summary>Content Server %1 (%2) is running.</summary>
    [Description("""Content Server %1 (%2) is running.""")]
    [HResultConstant(0x400D0052)]
    public static HResult NS_I_CUB_RUNNING => new(0x400D0052);

    /// <summary>Disk %1 ( %2 ) on Content Server %3, is running.</summary>
    [Description("""Disk %1 ( %2 ) on Content Server %3, is running.""")]
    [HResultConstant(0x400D0054)]
    public static HResult NS_I_DISK_START => new(0x400D0054);

    /// <summary>Started rebuilding disk %1 ( %2 ) on Content Server %3.</summary>
    [Description("""Started rebuilding disk %1 ( %2 ) on Content Server %3.""")]
    [HResultConstant(0x400D0056)]
    public static HResult NS_I_DISK_REBUILD_STARTED => new(0x400D0056);

    /// <summary>Finished rebuilding disk %1 ( %2 ) on Content Server %3.</summary>
    [Description("""Finished rebuilding disk %1 ( %2 ) on Content Server %3.""")]
    [HResultConstant(0x400D0057)]
    public static HResult NS_I_DISK_REBUILD_FINISHED => new(0x400D0057);

    /// <summary>Aborted rebuilding disk %1 ( %2 ) on Content Server %3.</summary>
    [Description("""Aborted rebuilding disk %1 ( %2 ) on Content Server %3.""")]
    [HResultConstant(0x400D0058)]
    public static HResult NS_I_DISK_REBUILD_ABORTED => new(0x400D0058);

    /// <summary>A NetShow administrator at network location %1 set the data stream limit to %2 streams.</summary>
    [Description("""A NetShow administrator at network location %1 set the data stream limit to %2 streams.""")]
    [HResultConstant(0x400D0059)]
    public static HResult NS_I_LIMIT_FUNNELS => new(0x400D0059);

    /// <summary>A NetShow administrator at network location %1 started disk %2.</summary>
    [Description("""A NetShow administrator at network location %1 started disk %2.""")]
    [HResultConstant(0x400D005A)]
    public static HResult NS_I_START_DISK => new(0x400D005A);

    /// <summary>A NetShow administrator at network location %1 stopped disk %2.</summary>
    [Description("""A NetShow administrator at network location %1 stopped disk %2.""")]
    [HResultConstant(0x400D005B)]
    public static HResult NS_I_STOP_DISK => new(0x400D005B);

    /// <summary>A NetShow administrator at network location %1 stopped Content Server %2.</summary>
    [Description("""A NetShow administrator at network location %1 stopped Content Server %2.""")]
    [HResultConstant(0x400D005C)]
    public static HResult NS_I_STOP_CUB => new(0x400D005C);

    /// <summary>A NetShow administrator at network location %1 aborted user session %2 from the system.</summary>
    [Description("""A NetShow administrator at network location %1 aborted user session %2 from the system.""")]
    [HResultConstant(0x400D005D)]
    public static HResult NS_I_KILL_USERSESSION => new(0x400D005D);

    /// <summary>A NetShow administrator at network location %1 aborted obsolete connection %2 from the system.</summary>
    [Description("""A NetShow administrator at network location %1 aborted obsolete connection %2 from the system.""")]
    [HResultConstant(0x400D005E)]
    public static HResult NS_I_KILL_CONNECTION => new(0x400D005E);

    /// <summary>A NetShow administrator at network location %1 started rebuilding disk %2.</summary>
    [Description("""A NetShow administrator at network location %1 started rebuilding disk %2.""")]
    [HResultConstant(0x400D005F)]
    public static HResult NS_I_REBUILD_DISK => new(0x400D005F);

    /// <summary>Event initialization failed, there will be no MCM events.</summary>
    [Description("""Event initialization failed, there will be no MCM events.""")]
    [HResultConstant(0x400D0069)]
    public static HResult MCMADM_I_NO_EVENTS => new(0x400D0069);

    /// <summary>The logging operation failed.</summary>
    [Description("""The logging operation failed.""")]
    [HResultConstant(0x400D006E)]
    public static HResult NS_I_LOGGING_FAILED => new(0x400D006E);

    /// <summary>A NetShow administrator at network location %1 set the maximum bandwidth limit to %2 bps.</summary>
    [Description("""A NetShow administrator at network location %1 set the maximum bandwidth limit to %2 bps.""")]
    [HResultConstant(0x400D0070)]
    public static HResult NS_I_LIMIT_BANDWIDTH => new(0x400D0070);

    /// <summary>Content Server %1 (%2) has established its link to Content Server %3.</summary>
    [Description("""Content Server %1 (%2) has established its link to Content Server %3.""")]
    [HResultConstant(0x400D0191)]
    public static HResult NS_I_CUB_UNFAIL_LINK => new(0x400D0191);

    /// <summary>Restripe operation has started.</summary>
    [Description("""Restripe operation has started.""")]
    [HResultConstant(0x400D0193)]
    public static HResult NS_I_RESTRIPE_START => new(0x400D0193);

    /// <summary>Restripe operation has completed.</summary>
    [Description("""Restripe operation has completed.""")]
    [HResultConstant(0x400D0194)]
    public static HResult NS_I_RESTRIPE_DONE => new(0x400D0194);

    /// <summary>Content disk %1 (%2) on Content Server %3 has been restriped out.</summary>
    [Description("""Content disk %1 (%2) on Content Server %3 has been restriped out.""")]
    [HResultConstant(0x400D0196)]
    public static HResult NS_I_RESTRIPE_DISK_OUT => new(0x400D0196);

    /// <summary>Content server %1 (%2) has been restriped out.</summary>
    [Description("""Content server %1 (%2) has been restriped out.""")]
    [HResultConstant(0x400D0197)]
    public static HResult NS_I_RESTRIPE_CUB_OUT => new(0x400D0197);

    /// <summary>Disk %1 ( %2 ) on Content Server %3, has been offlined.</summary>
    [Description("""Disk %1 ( %2 ) on Content Server %3, has been offlined.""")]
    [HResultConstant(0x400D0198)]
    public static HResult NS_I_DISK_STOP => new(0x400D0198);

    /// <summary>The playlist change occurred while receding.</summary>
    [Description("""The playlist change occurred while receding.""")]
    [HResultConstant(0x400D14BE)]
    public static HResult NS_I_PLAYLIST_CHANGE_RECEDING => new(0x400D14BE);

    /// <summary>The client is reconnected.</summary>
    [Description("""The client is reconnected.""")]
    [HResultConstant(0x400D2EFF)]
    public static HResult NS_I_RECONNECTED => new(0x400D2EFF);

    /// <summary>Forcing a switch to a pending header on start.</summary>
    [Description("""Forcing a switch to a pending header on start.""")]
    [HResultConstant(0x400D2F01)]
    public static HResult NS_I_NOLOG_STOP => new(0x400D2F01);

    /// <summary>There is already an existing packetizer plugin for the stream.</summary>
    [Description("""There is already an existing packetizer plugin for the stream.""")]
    [HResultConstant(0x400D2F03)]
    public static HResult NS_I_EXISTING_PACKETIZER => new(0x400D2F03);

    /// <summary>The proxy setting is manual.</summary>
    [Description("""The proxy setting is manual.""")]
    [HResultConstant(0x400D2F04)]
    public static HResult NS_I_MANUAL_PROXY => new(0x400D2F04);

    /// <summary>The kernel driver detected a version mismatch between it and the user mode driver.</summary>
    [Description("""The kernel driver detected a version mismatch between it and the user mode driver.""")]
    [HResultConstant(0x40262009)]
    public static HResult ERROR_GRAPHICS_DRIVER_MISMATCH => new(0x40262009);

    /// <summary>Child device presence was not reliably detected.</summary>
    [Description("""Child device presence was not reliably detected.""")]
    [HResultConstant(0x4026242F)]
    public static HResult ERROR_GRAPHICS_UNKNOWN_CHILD_STATUS => new(0x4026242F);

    /// <summary>Starting the lead-link adapter has been deferred temporarily.</summary>
    [Description("""Starting the lead-link adapter has been deferred temporarily.""")]
    [HResultConstant(0x40262437)]
    public static HResult ERROR_GRAPHICS_LEADLINK_START_DEFERRED => new(0x40262437);

    /// <summary>The display adapter is being polled for children too frequently at the same polling level.</summary>
    [Description("""The display adapter is being polled for children too frequently at the same polling level.""")]
    [HResultConstant(0x40262439)]
    public static HResult ERROR_GRAPHICS_POLLING_TOO_FREQUENTLY => new(0x40262439);

    /// <summary>Starting the adapter has been deferred temporarily.</summary>
    [Description("""Starting the adapter has been deferred temporarily.""")]
    [HResultConstant(0x4026243A)]
    public static HResult ERROR_GRAPHICS_START_DEFERRED => new(0x4026243A);

    /// <summary>The data necessary to complete this operation is not yet available.</summary>
    [Description("""The data necessary to complete this operation is not yet available.""")]
    [HResultConstant(0x8000000A)]
    public static HResult E_PENDING => new(0x8000000A);

    /// <summary>Thread local storage failure.</summary>
    [Description("""Thread local storage failure.""")]
    [HResultConstant(0x80004006)]
    public static HResult CO_E_INIT_TLS => new(0x80004006);

    /// <summary>Get shared memory allocator failure.</summary>
    [Description("""Get shared memory allocator failure.""")]
    [HResultConstant(0x80004007)]
    public static HResult CO_E_INIT_SHARED_ALLOCATOR => new(0x80004007);

    /// <summary>Get memory allocator failure.</summary>
    [Description("""Get memory allocator failure.""")]
    [HResultConstant(0x80004008)]
    public static HResult CO_E_INIT_MEMORY_ALLOCATOR => new(0x80004008);

    /// <summary>Unable to initialize class cache.</summary>
    [Description("""Unable to initialize class cache.""")]
    [HResultConstant(0x80004009)]
    public static HResult CO_E_INIT_CLASS_CACHE => new(0x80004009);

    /// <summary>Unable to initialize remote procedure call (RPC) services.</summary>
    [Description("""Unable to initialize remote procedure call (RPC) services.""")]
    [HResultConstant(0x8000400A)]
    public static HResult CO_E_INIT_RPC_CHANNEL => new(0x8000400A);

    /// <summary>Cannot set thread local storage channel control.</summary>
    [Description("""Cannot set thread local storage channel control.""")]
    [HResultConstant(0x8000400B)]
    public static HResult CO_E_INIT_TLS_SET_CHANNEL_CONTROL => new(0x8000400B);

    /// <summary>Could not allocate thread local storage channel control.</summary>
    [Description("""Could not allocate thread local storage channel control.""")]
    [HResultConstant(0x8000400C)]
    public static HResult CO_E_INIT_TLS_CHANNEL_CONTROL => new(0x8000400C);

    /// <summary>The user-supplied memory allocator is unacceptable.</summary>
    [Description("""The user-supplied memory allocator is unacceptable.""")]
    [HResultConstant(0x8000400D)]
    public static HResult CO_E_INIT_UNACCEPTED_USER_ALLOCATOR => new(0x8000400D);

    /// <summary>The OLE service mutex already exists.</summary>
    [Description("""The OLE service mutex already exists.""")]
    [HResultConstant(0x8000400E)]
    public static HResult CO_E_INIT_SCM_MUTEX_EXISTS => new(0x8000400E);

    /// <summary>The OLE service file mapping already exists.</summary>
    [Description("""The OLE service file mapping already exists.""")]
    [HResultConstant(0x8000400F)]
    public static HResult CO_E_INIT_SCM_FILE_MAPPING_EXISTS => new(0x8000400F);

    /// <summary>Unable to map view of file for OLE service.</summary>
    [Description("""Unable to map view of file for OLE service.""")]
    [HResultConstant(0x80004010)]
    public static HResult CO_E_INIT_SCM_MAP_VIEW_OF_FILE => new(0x80004010);

    /// <summary>Failure attempting to launch OLE service.</summary>
    [Description("""Failure attempting to launch OLE service.""")]
    [HResultConstant(0x80004011)]
    public static HResult CO_E_INIT_SCM_EXEC_FAILURE => new(0x80004011);

    /// <summary>There was an attempt to call CoInitialize a second time while single-threaded.</summary>
    [Description("""There was an attempt to call CoInitialize a second time while single-threaded.""")]
    [HResultConstant(0x80004012)]
    public static HResult CO_E_INIT_ONLY_SINGLE_THREADED => new(0x80004012);

    /// <summary>A Remote activation was necessary but was not allowed.</summary>
    [Description("""A Remote activation was necessary but was not allowed.""")]
    [HResultConstant(0x80004013)]
    public static HResult CO_E_CANT_REMOTE => new(0x80004013);

    /// <summary>A Remote activation was necessary, but the server name provided was invalid.</summary>
    [Description("""A Remote activation was necessary, but the server name provided was invalid.""")]
    [HResultConstant(0x80004014)]
    public static HResult CO_E_BAD_SERVER_NAME => new(0x80004014);

    /// <summary>The class is configured to run as a security ID different from the caller.</summary>
    [Description("""The class is configured to run as a security ID different from the caller.""")]
    [HResultConstant(0x80004015)]
    public static HResult CO_E_WRONG_SERVER_IDENTITY => new(0x80004015);

    /// <summary>Use of OLE1 services requiring Dynamic Data Exchange (DDE) Windows is disabled.</summary>
    [Description("""Use of OLE1 services requiring Dynamic Data Exchange (DDE) Windows is disabled.""")]
    [HResultConstant(0x80004016)]
    public static HResult CO_E_OLE1DDE_DISABLED => new(0x80004016);

    /// <summary>A RunAs specification must be <domain name>\<user name> or simply <user name>.</summary>
    [Description("""A RunAs specification must be <domain name>\<user name> or simply <user name>.""")]
    [HResultConstant(0x80004017)]
    public static HResult CO_E_RUNAS_SYNTAX => new(0x80004017);

    /// <summary>The server process could not be started. The path name might be incorrect.</summary>
    [Description("""The server process could not be started. The path name might be incorrect.""")]
    [HResultConstant(0x80004018)]
    public static HResult CO_E_CREATEPROCESS_FAILURE => new(0x80004018);

    /// <summary>The server process could not be started as the configured identity. The path name might be incorrect or unavailable.</summary>
    [Description("""The server process could not be started as the configured identity. The path name might be incorrect or unavailable.""")]
    [HResultConstant(0x80004019)]
    public static HResult CO_E_RUNAS_CREATEPROCESS_FAILURE => new(0x80004019);

    /// <summary>The server process could not be started because the configured identity is incorrect. Check the user name and password.</summary>
    [Description("""The server process could not be started because the configured identity is incorrect. Check the user name and password.""")]
    [HResultConstant(0x8000401A)]
    public static HResult CO_E_RUNAS_LOGON_FAILURE => new(0x8000401A);

    /// <summary>The client is not allowed to launch this server.</summary>
    [Description("""The client is not allowed to launch this server.""")]
    [HResultConstant(0x8000401B)]
    public static HResult CO_E_LAUNCH_PERMSSION_DENIED => new(0x8000401B);

    /// <summary>The service providing this server could not be started.</summary>
    [Description("""The service providing this server could not be started.""")]
    [HResultConstant(0x8000401C)]
    public static HResult CO_E_START_SERVICE_FAILURE => new(0x8000401C);

    /// <summary>This computer was unable to communicate with the computer providing the server.</summary>
    [Description("""This computer was unable to communicate with the computer providing the server.""")]
    [HResultConstant(0x8000401D)]
    public static HResult CO_E_REMOTE_COMMUNICATION_FAILURE => new(0x8000401D);

    /// <summary>The server did not respond after being launched.</summary>
    [Description("""The server did not respond after being launched.""")]
    [HResultConstant(0x8000401E)]
    public static HResult CO_E_SERVER_START_TIMEOUT => new(0x8000401E);

    /// <summary>The registration information for this server is inconsistent or incomplete.</summary>
    [Description("""The registration information for this server is inconsistent or incomplete.""")]
    [HResultConstant(0x8000401F)]
    public static HResult CO_E_CLSREG_INCONSISTENT => new(0x8000401F);

    /// <summary>The registration information for this interface is inconsistent or incomplete.</summary>
    [Description("""The registration information for this interface is inconsistent or incomplete.""")]
    [HResultConstant(0x80004020)]
    public static HResult CO_E_IIDREG_INCONSISTENT => new(0x80004020);

    /// <summary>The operation attempted is not supported.</summary>
    [Description("""The operation attempted is not supported.""")]
    [HResultConstant(0x80004021)]
    public static HResult CO_E_NOT_SUPPORTED => new(0x80004021);

    /// <summary>A DLL must be loaded.</summary>
    [Description("""A DLL must be loaded.""")]
    [HResultConstant(0x80004022)]
    public static HResult CO_E_RELOAD_DLL => new(0x80004022);

    /// <summary>A Microsoft Software Installer error was encountered.</summary>
    [Description("""A Microsoft Software Installer error was encountered.""")]
    [HResultConstant(0x80004023)]
    public static HResult CO_E_MSI_ERROR => new(0x80004023);

    /// <summary>The specified activation could not occur in the client context as specified.</summary>
    [Description("""The specified activation could not occur in the client context as specified.""")]
    [HResultConstant(0x80004024)]
    public static HResult CO_E_ATTEMPT_TO_CREATE_OUTSIDE_CLIENT_CONTEXT => new(0x80004024);

    /// <summary>Activations on the server are paused.</summary>
    [Description("""Activations on the server are paused.""")]
    [HResultConstant(0x80004025)]
    public static HResult CO_E_SERVER_PAUSED => new(0x80004025);

    /// <summary>Activations on the server are not paused.</summary>
    [Description("""Activations on the server are not paused.""")]
    [HResultConstant(0x80004026)]
    public static HResult CO_E_SERVER_NOT_PAUSED => new(0x80004026);

    /// <summary>The component or application containing the component has been disabled.</summary>
    [Description("""The component or application containing the component has been disabled.""")]
    [HResultConstant(0x80004027)]
    public static HResult CO_E_CLASS_DISABLED => new(0x80004027);

    /// <summary>The common language runtime is not available.</summary>
    [Description("""The common language runtime is not available.""")]
    [HResultConstant(0x80004028)]
    public static HResult CO_E_CLRNOTAVAILABLE => new(0x80004028);

    /// <summary>The thread-pool rejected the submitted asynchronous work.</summary>
    [Description("""The thread-pool rejected the submitted asynchronous work.""")]
    [HResultConstant(0x80004029)]
    public static HResult CO_E_ASYNC_WORK_REJECTED => new(0x80004029);

    /// <summary>The server started, but it did not finish initializing in a timely fashion.</summary>
    [Description("""The server started, but it did not finish initializing in a timely fashion.""")]
    [HResultConstant(0x8000402A)]
    public static HResult CO_E_SERVER_INIT_TIMEOUT => new(0x8000402A);

    /// <summary>Unable to complete the call because there is no COM+ security context inside IObjectControl.Activate.</summary>
    [Description("""Unable to complete the call because there is no COM+ security context inside IObjectControl.Activate.""")]
    [HResultConstant(0x8000402B)]
    public static HResult CO_E_NO_SECCTX_IN_ACTIVATE => new(0x8000402B);

    /// <summary>The provided tracker configuration is invalid.</summary>
    [Description("""The provided tracker configuration is invalid.""")]
    [HResultConstant(0x80004030)]
    public static HResult CO_E_TRACKER_CONFIG => new(0x80004030);

    /// <summary>The provided thread pool configuration is invalid.</summary>
    [Description("""The provided thread pool configuration is invalid.""")]
    [HResultConstant(0x80004031)]
    public static HResult CO_E_THREADPOOL_CONFIG => new(0x80004031);

    /// <summary>The provided side-by-side configuration is invalid.</summary>
    [Description("""The provided side-by-side configuration is invalid.""")]
    [HResultConstant(0x80004032)]
    public static HResult CO_E_SXS_CONFIG => new(0x80004032);

    /// <summary>The server principal name (SPN) obtained during security negotiation is malformed.</summary>
    [Description("""The server principal name (SPN) obtained during security negotiation is malformed.""")]
    [HResultConstant(0x80004033)]
    public static HResult CO_E_MALFORMED_SPN => new(0x80004033);

    /// <summary>Call was rejected by callee.</summary>
    [Description("""Call was rejected by callee.""")]
    [HResultConstant(0x80010001)]
    public static HResult RPC_E_CALL_REJECTED => new(0x80010001);

    /// <summary>Call was canceled by the message filter.</summary>
    [Description("""Call was canceled by the message filter.""")]
    [HResultConstant(0x80010002)]
    public static HResult RPC_E_CALL_CANCELED => new(0x80010002);

    /// <summary>The caller is dispatching an intertask SendMessage call and cannot call out via PostMessage.</summary>
    [Description("""The caller is dispatching an intertask SendMessage call and cannot call out via PostMessage.""")]
    [HResultConstant(0x80010003)]
    public static HResult RPC_E_CANTPOST_INSENDCALL => new(0x80010003);

    /// <summary>The caller is dispatching an asynchronous call and cannot make an outgoing call on behalf of this call.</summary>
    [Description("""The caller is dispatching an asynchronous call and cannot make an outgoing call on behalf of this call.""")]
    [HResultConstant(0x80010004)]
    public static HResult RPC_E_CANTCALLOUT_INASYNCCALL => new(0x80010004);

    /// <summary>It is illegal to call out while inside message filter.</summary>
    [Description("""It is illegal to call out while inside message filter.""")]
    [HResultConstant(0x80010005)]
    public static HResult RPC_E_CANTCALLOUT_INEXTERNALCALL => new(0x80010005);

    /// <summary>The connection terminated or is in a bogus state and can no longer be used. Other connections are still valid.</summary>
    [Description("""The connection terminated or is in a bogus state and can no longer be used. Other connections are still valid.""")]
    [HResultConstant(0x80010006)]
    public static HResult RPC_E_CONNECTION_TERMINATED => new(0x80010006);

    /// <summary>The callee (the server, not the server application) is not available and disappeared; all connections are invalid. The call might have executed.</summary>
    [Description("""The callee (the server, not the server application) is not available and disappeared; all connections are invalid. The call might have executed.""")]
    [HResultConstant(0x80010007)]
    public static HResult RPC_E_SERVER_DIED => new(0x80010007);

    /// <summary>The caller (client) disappeared while the callee (server) was processing a call.</summary>
    [Description("""The caller (client) disappeared while the callee (server) was processing a call.""")]
    [HResultConstant(0x80010008)]
    public static HResult RPC_E_CLIENT_DIED => new(0x80010008);

    /// <summary>The data packet with the marshaled parameter data is incorrect.</summary>
    [Description("""The data packet with the marshaled parameter data is incorrect.""")]
    [HResultConstant(0x80010009)]
    public static HResult RPC_E_INVALID_DATAPACKET => new(0x80010009);

    /// <summary>The call was not transmitted properly; the message queue was full and was not emptied after yielding.</summary>
    [Description("""The call was not transmitted properly; the message queue was full and was not emptied after yielding.""")]
    [HResultConstant(0x8001000A)]
    public static HResult RPC_E_CANTTRANSMIT_CALL => new(0x8001000A);

    /// <summary>The client RPC caller cannot marshal the parameter data due to errors (such as low memory).</summary>
    [Description("""The client RPC caller cannot marshal the parameter data due to errors (such as low memory).""")]
    [HResultConstant(0x8001000B)]
    public static HResult RPC_E_CLIENT_CANTMARSHAL_DATA => new(0x8001000B);

    /// <summary>The client RPC caller cannot unmarshal the return data due to errors (such as low memory).</summary>
    [Description("""The client RPC caller cannot unmarshal the return data due to errors (such as low memory).""")]
    [HResultConstant(0x8001000C)]
    public static HResult RPC_E_CLIENT_CANTUNMARSHAL_DATA => new(0x8001000C);

    /// <summary>The server RPC callee cannot marshal the return data due to errors (such as low memory).</summary>
    [Description("""The server RPC callee cannot marshal the return data due to errors (such as low memory).""")]
    [HResultConstant(0x8001000D)]
    public static HResult RPC_E_SERVER_CANTMARSHAL_DATA => new(0x8001000D);

    /// <summary>The server RPC callee cannot unmarshal the parameter data due to errors (such as low memory).</summary>
    [Description("""The server RPC callee cannot unmarshal the parameter data due to errors (such as low memory).""")]
    [HResultConstant(0x8001000E)]
    public static HResult RPC_E_SERVER_CANTUNMARSHAL_DATA => new(0x8001000E);

    /// <summary>Received data is invalid. The data might be server or client data.</summary>
    [Description("""Received data is invalid. The data might be server or client data.""")]
    [HResultConstant(0x8001000F)]
    public static HResult RPC_E_INVALID_DATA => new(0x8001000F);

    /// <summary>A particular parameter is invalid and cannot be (un)marshaled.</summary>
    [Description("""A particular parameter is invalid and cannot be (un)marshaled.""")]
    [HResultConstant(0x80010010)]
    public static HResult RPC_E_INVALID_PARAMETER => new(0x80010010);

    /// <summary>There is no second outgoing call on same channel in DDE conversation.</summary>
    [Description("""There is no second outgoing call on same channel in DDE conversation.""")]
    [HResultConstant(0x80010011)]
    public static HResult RPC_E_CANTCALLOUT_AGAIN => new(0x80010011);

    /// <summary>The callee (the server, not the server application) is not available and disappeared; all connections are invalid. The call did not execute.</summary>
    [Description("""The callee (the server, not the server application) is not available and disappeared; all connections are invalid. The call did not execute.""")]
    [HResultConstant(0x80010012)]
    public static HResult RPC_E_SERVER_DIED_DNE => new(0x80010012);

    /// <summary>System call failed.</summary>
    [Description("""System call failed.""")]
    [HResultConstant(0x80010100)]
    public static HResult RPC_E_SYS_CALL_FAILED => new(0x80010100);

    /// <summary>Could not allocate some required resource (such as memory or events)</summary>
    [Description("""Could not allocate some required resource (such as memory or events)""")]
    [HResultConstant(0x80010101)]
    public static HResult RPC_E_OUT_OF_RESOURCES => new(0x80010101);

    /// <summary>Attempted to make calls on more than one thread in single-threaded mode.</summary>
    [Description("""Attempted to make calls on more than one thread in single-threaded mode.""")]
    [HResultConstant(0x80010102)]
    public static HResult RPC_E_ATTEMPTED_MULTITHREAD => new(0x80010102);

    /// <summary>The requested interface is not registered on the server object.</summary>
    [Description("""The requested interface is not registered on the server object.""")]
    [HResultConstant(0x80010103)]
    public static HResult RPC_E_NOT_REGISTERED => new(0x80010103);

    /// <summary>RPC could not call the server or could not return the results of calling the server.</summary>
    [Description("""RPC could not call the server or could not return the results of calling the server.""")]
    [HResultConstant(0x80010104)]
    public static HResult RPC_E_FAULT => new(0x80010104);

    /// <summary>The server threw an exception.</summary>
    [Description("""The server threw an exception.""")]
    [HResultConstant(0x80010105)]
    public static HResult RPC_E_SERVERFAULT => new(0x80010105);

    /// <summary>Cannot change thread mode after it is set.</summary>
    [Description("""Cannot change thread mode after it is set.""")]
    [HResultConstant(0x80010106)]
    public static HResult RPC_E_CHANGED_MODE => new(0x80010106);

    /// <summary>The method called does not exist on the server.</summary>
    [Description("""The method called does not exist on the server.""")]
    [HResultConstant(0x80010107)]
    public static HResult RPC_E_INVALIDMETHOD => new(0x80010107);

    /// <summary>The object invoked has disconnected from its clients.</summary>
    [Description("""The object invoked has disconnected from its clients.""")]
    [HResultConstant(0x80010108)]
    public static HResult RPC_E_DISCONNECTED => new(0x80010108);

    /// <summary>The object invoked chose not to process the call now. Try again later.</summary>
    [Description("""The object invoked chose not to process the call now. Try again later.""")]
    [HResultConstant(0x80010109)]
    public static HResult RPC_E_RETRY => new(0x80010109);

    /// <summary>The message filter indicated that the application is busy.</summary>
    [Description("""The message filter indicated that the application is busy.""")]
    [HResultConstant(0x8001010A)]
    public static HResult RPC_E_SERVERCALL_RETRYLATER => new(0x8001010A);

    /// <summary>The message filter rejected the call.</summary>
    [Description("""The message filter rejected the call.""")]
    [HResultConstant(0x8001010B)]
    public static HResult RPC_E_SERVERCALL_REJECTED => new(0x8001010B);

    /// <summary>A call control interface was called with invalid data.</summary>
    [Description("""A call control interface was called with invalid data.""")]
    [HResultConstant(0x8001010C)]
    public static HResult RPC_E_INVALID_CALLDATA => new(0x8001010C);

    /// <summary>An outgoing call cannot be made because the application is dispatching an input-synchronous call.</summary>
    [Description("""An outgoing call cannot be made because the application is dispatching an input-synchronous call.""")]
    [HResultConstant(0x8001010D)]
    public static HResult RPC_E_CANTCALLOUT_ININPUTSYNCCALL => new(0x8001010D);

    /// <summary>The application called an interface that was marshaled for a different thread.</summary>
    [Description("""The application called an interface that was marshaled for a different thread.""")]
    [HResultConstant(0x8001010E)]
    public static HResult RPC_E_WRONG_THREAD => new(0x8001010E);

    /// <summary>CoInitialize has not been called on the current thread.</summary>
    [Description("""CoInitialize has not been called on the current thread.""")]
    [HResultConstant(0x8001010F)]
    public static HResult RPC_E_THREAD_NOT_INIT => new(0x8001010F);

    /// <summary>The version of OLE on the client and server machines does not match.</summary>
    [Description("""The version of OLE on the client and server machines does not match.""")]
    [HResultConstant(0x80010110)]
    public static HResult RPC_E_VERSION_MISMATCH => new(0x80010110);

    /// <summary>OLE received a packet with an invalid header.</summary>
    [Description("""OLE received a packet with an invalid header.""")]
    [HResultConstant(0x80010111)]
    public static HResult RPC_E_INVALID_HEADER => new(0x80010111);

    /// <summary>OLE received a packet with an invalid extension.</summary>
    [Description("""OLE received a packet with an invalid extension.""")]
    [HResultConstant(0x80010112)]
    public static HResult RPC_E_INVALID_EXTENSION => new(0x80010112);

    /// <summary>The requested object or interface does not exist.</summary>
    [Description("""The requested object or interface does not exist.""")]
    [HResultConstant(0x80010113)]
    public static HResult RPC_E_INVALID_IPID => new(0x80010113);

    /// <summary>The requested object does not exist.</summary>
    [Description("""The requested object does not exist.""")]
    [HResultConstant(0x80010114)]
    public static HResult RPC_E_INVALID_OBJECT => new(0x80010114);

    /// <summary>OLE has sent a request and is waiting for a reply.</summary>
    [Description("""OLE has sent a request and is waiting for a reply.""")]
    [HResultConstant(0x80010115)]
    public static HResult RPC_S_CALLPENDING => new(0x80010115);

    /// <summary>OLE is waiting before retrying a request.</summary>
    [Description("""OLE is waiting before retrying a request.""")]
    [HResultConstant(0x80010116)]
    public static HResult RPC_S_WAITONTIMER => new(0x80010116);

    /// <summary>Call context cannot be accessed after call completed.</summary>
    [Description("""Call context cannot be accessed after call completed.""")]
    [HResultConstant(0x80010117)]
    public static HResult RPC_E_CALL_COMPLETE => new(0x80010117);

    /// <summary>Impersonate on unsecure calls is not supported.</summary>
    [Description("""Impersonate on unsecure calls is not supported.""")]
    [HResultConstant(0x80010118)]
    public static HResult RPC_E_UNSECURE_CALL => new(0x80010118);

    /// <summary>Security must be initialized before any interfaces are marshaled or unmarshaled. It cannot be changed after initialized.</summary>
    [Description("""Security must be initialized before any interfaces are marshaled or unmarshaled. It cannot be changed after initialized.""")]
    [HResultConstant(0x80010119)]
    public static HResult RPC_E_TOO_LATE => new(0x80010119);

    /// <summary>No security packages are installed on this machine, the user is not logged on, or there are no compatible security packages between the client and server.</summary>
    [Description("""No security packages are installed on this machine, the user is not logged on, or there are no compatible security packages between the client and server.""")]
    [HResultConstant(0x8001011A)]
    public static HResult RPC_E_NO_GOOD_SECURITY_PACKAGES => new(0x8001011A);

    /// <summary>Access is denied.</summary>
    [Description("""Access is denied.""")]
    [HResultConstant(0x8001011B)]
    public static HResult RPC_E_ACCESS_DENIED => new(0x8001011B);

    /// <summary>Remote calls are not allowed for this process.</summary>
    [Description("""Remote calls are not allowed for this process.""")]
    [HResultConstant(0x8001011C)]
    public static HResult RPC_E_REMOTE_DISABLED => new(0x8001011C);

    /// <summary>The marshaled interface data packet (OBJREF) has an invalid or unknown format.</summary>
    [Description("""The marshaled interface data packet (OBJREF) has an invalid or unknown format.""")]
    [HResultConstant(0x8001011D)]
    public static HResult RPC_E_INVALID_OBJREF => new(0x8001011D);

    /// <summary>No context is associated with this call. This happens for some custom marshaled calls and on the client side of the call.</summary>
    [Description("""No context is associated with this call. This happens for some custom marshaled calls and on the client side of the call.""")]
    [HResultConstant(0x8001011E)]
    public static HResult RPC_E_NO_CONTEXT => new(0x8001011E);

    /// <summary>This operation returned because the time-out period expired.</summary>
    [Description("""This operation returned because the time-out period expired.""")]
    [HResultConstant(0x8001011F)]
    public static HResult RPC_E_TIMEOUT => new(0x8001011F);

    /// <summary>There are no synchronize objects to wait on.</summary>
    [Description("""There are no synchronize objects to wait on.""")]
    [HResultConstant(0x80010120)]
    public static HResult RPC_E_NO_SYNC => new(0x80010120);

    /// <summary>Full subject issuer chain Secure Sockets Layer (SSL) principal name expected from the server.</summary>
    [Description("""Full subject issuer chain Secure Sockets Layer (SSL) principal name expected from the server.""")]
    [HResultConstant(0x80010121)]
    public static HResult RPC_E_FULLSIC_REQUIRED => new(0x80010121);

    /// <summary>Principal name is not a valid Microsoft standard (msstd) name.</summary>
    [Description("""Principal name is not a valid Microsoft standard (msstd) name.""")]
    [HResultConstant(0x80010122)]
    public static HResult RPC_E_INVALID_STD_NAME => new(0x80010122);

    /// <summary>Unable to impersonate DCOM client.</summary>
    [Description("""Unable to impersonate DCOM client.""")]
    [HResultConstant(0x80010123)]
    public static HResult CO_E_FAILEDTOIMPERSONATE => new(0x80010123);

    /// <summary>Unable to obtain server's security context.</summary>
    [Description("""Unable to obtain server's security context.""")]
    [HResultConstant(0x80010124)]
    public static HResult CO_E_FAILEDTOGETSECCTX => new(0x80010124);

    /// <summary>Unable to open the access token of the current thread.</summary>
    [Description("""Unable to open the access token of the current thread.""")]
    [HResultConstant(0x80010125)]
    public static HResult CO_E_FAILEDTOOPENTHREADTOKEN => new(0x80010125);

    /// <summary>Unable to obtain user information from an access token.</summary>
    [Description("""Unable to obtain user information from an access token.""")]
    [HResultConstant(0x80010126)]
    public static HResult CO_E_FAILEDTOGETTOKENINFO => new(0x80010126);

    /// <summary>The client who called IAccessControl::IsAccessPermitted was not the trustee provided to the method.</summary>
    [Description("""The client who called IAccessControl::IsAccessPermitted was not the trustee provided to the method.""")]
    [HResultConstant(0x80010127)]
    public static HResult CO_E_TRUSTEEDOESNTMATCHCLIENT => new(0x80010127);

    /// <summary>Unable to obtain the client's security blanket.</summary>
    [Description("""Unable to obtain the client's security blanket.""")]
    [HResultConstant(0x80010128)]
    public static HResult CO_E_FAILEDTOQUERYCLIENTBLANKET => new(0x80010128);

    /// <summary>Unable to set a discretionary access control list (ACL) into a security descriptor.</summary>
    [Description("""Unable to set a discretionary access control list (ACL) into a security descriptor.""")]
    [HResultConstant(0x80010129)]
    public static HResult CO_E_FAILEDTOSETDACL => new(0x80010129);

    /// <summary>The system function AccessCheck returned false.</summary>
    [Description("""The system function AccessCheck returned false.""")]
    [HResultConstant(0x8001012A)]
    public static HResult CO_E_ACCESSCHECKFAILED => new(0x8001012A);

    /// <summary>Either NetAccessDel or NetAccessAdd returned an error code.</summary>
    [Description("""Either NetAccessDel or NetAccessAdd returned an error code.""")]
    [HResultConstant(0x8001012B)]
    public static HResult CO_E_NETACCESSAPIFAILED => new(0x8001012B);

    /// <summary>One of the trustee strings provided by the user did not conform to the <Domain>\<Name> syntax and it was not the *" string".</summary>
    [Description("""One of the trustee strings provided by the user did not conform to the <Domain>\<Name> syntax and it was not the *" string".""")]
    [HResultConstant(0x8001012C)]
    public static HResult CO_E_WRONGTRUSTEENAMESYNTAX => new(0x8001012C);

    /// <summary>One of the security identifiers provided by the user was invalid.</summary>
    [Description("""One of the security identifiers provided by the user was invalid.""")]
    [HResultConstant(0x8001012D)]
    public static HResult CO_E_INVALIDSID => new(0x8001012D);

    /// <summary>Unable to convert a wide character trustee string to a multiple-byte trustee string.</summary>
    [Description("""Unable to convert a wide character trustee string to a multiple-byte trustee string.""")]
    [HResultConstant(0x8001012E)]
    public static HResult CO_E_CONVERSIONFAILED => new(0x8001012E);

    /// <summary>Unable to find a security identifier that corresponds to a trustee string provided by the user.</summary>
    [Description("""Unable to find a security identifier that corresponds to a trustee string provided by the user.""")]
    [HResultConstant(0x8001012F)]
    public static HResult CO_E_NOMATCHINGSIDFOUND => new(0x8001012F);

    /// <summary>The system function LookupAccountSID failed.</summary>
    [Description("""The system function LookupAccountSID failed.""")]
    [HResultConstant(0x80010130)]
    public static HResult CO_E_LOOKUPACCSIDFAILED => new(0x80010130);

    /// <summary>Unable to find a trustee name that corresponds to a security identifier provided by the user.</summary>
    [Description("""Unable to find a trustee name that corresponds to a security identifier provided by the user.""")]
    [HResultConstant(0x80010131)]
    public static HResult CO_E_NOMATCHINGNAMEFOUND => new(0x80010131);

    /// <summary>The system function LookupAccountName failed.</summary>
    [Description("""The system function LookupAccountName failed.""")]
    [HResultConstant(0x80010132)]
    public static HResult CO_E_LOOKUPACCNAMEFAILED => new(0x80010132);

    /// <summary>Unable to set or reset a serialization handle.</summary>
    [Description("""Unable to set or reset a serialization handle.""")]
    [HResultConstant(0x80010133)]
    public static HResult CO_E_SETSERLHNDLFAILED => new(0x80010133);

    /// <summary>Unable to obtain the Windows directory.</summary>
    [Description("""Unable to obtain the Windows directory.""")]
    [HResultConstant(0x80010134)]
    public static HResult CO_E_FAILEDTOGETWINDIR => new(0x80010134);

    /// <summary>Path too long.</summary>
    [Description("""Path too long.""")]
    [HResultConstant(0x80010135)]
    public static HResult CO_E_PATHTOOLONG => new(0x80010135);

    /// <summary>Unable to generate a UUID.</summary>
    [Description("""Unable to generate a UUID.""")]
    [HResultConstant(0x80010136)]
    public static HResult CO_E_FAILEDTOGENUUID => new(0x80010136);

    /// <summary>Unable to create file.</summary>
    [Description("""Unable to create file.""")]
    [HResultConstant(0x80010137)]
    public static HResult CO_E_FAILEDTOCREATEFILE => new(0x80010137);

    /// <summary>Unable to close a serialization handle or a file handle.</summary>
    [Description("""Unable to close a serialization handle or a file handle.""")]
    [HResultConstant(0x80010138)]
    public static HResult CO_E_FAILEDTOCLOSEHANDLE => new(0x80010138);

    /// <summary>The number of access control entries (ACEs) in an ACL exceeds the system limit.</summary>
    [Description("""The number of access control entries (ACEs) in an ACL exceeds the system limit.""")]
    [HResultConstant(0x80010139)]
    public static HResult CO_E_EXCEEDSYSACLLIMIT => new(0x80010139);

    /// <summary>Not all the DENY_ACCESS ACEs are arranged in front of the GRANT_ACCESS ACEs in the stream.</summary>
    [Description("""Not all the DENY_ACCESS ACEs are arranged in front of the GRANT_ACCESS ACEs in the stream.""")]
    [HResultConstant(0x8001013A)]
    public static HResult CO_E_ACESINWRONGORDER => new(0x8001013A);

    /// <summary>The version of ACL format in the stream is not supported by this implementation of IAccessControl.</summary>
    [Description("""The version of ACL format in the stream is not supported by this implementation of IAccessControl.""")]
    [HResultConstant(0x8001013B)]
    public static HResult CO_E_INCOMPATIBLESTREAMVERSION => new(0x8001013B);

    /// <summary>Unable to open the access token of the server process.</summary>
    [Description("""Unable to open the access token of the server process.""")]
    [HResultConstant(0x8001013C)]
    public static HResult CO_E_FAILEDTOOPENPROCESSTOKEN => new(0x8001013C);

    /// <summary>Unable to decode the ACL in the stream provided by the user.</summary>
    [Description("""Unable to decode the ACL in the stream provided by the user.""")]
    [HResultConstant(0x8001013D)]
    public static HResult CO_E_DECODEFAILED => new(0x8001013D);

    /// <summary>The COM IAccessControl object is not initialized.</summary>
    [Description("""The COM IAccessControl object is not initialized.""")]
    [HResultConstant(0x8001013F)]
    public static HResult CO_E_ACNOTINITIALIZED => new(0x8001013F);

    /// <summary>Call Cancellation is disabled.</summary>
    [Description("""Call Cancellation is disabled.""")]
    [HResultConstant(0x80010140)]
    public static HResult CO_E_CANCEL_DISABLED => new(0x80010140);

    /// <summary>An internal error occurred.</summary>
    [Description("""An internal error occurred.""")]
    [HResultConstant(0x8001FFFF)]
    public static HResult RPC_E_UNEXPECTED => new(0x8001FFFF);

    /// <summary>Unknown interface.</summary>
    [Description("""Unknown interface.""")]
    [HResultConstant(0x80020001)]
    public static HResult DISP_E_UNKNOWNINTERFACE => new(0x80020001);

    /// <summary>Member not found.</summary>
    [Description("""Member not found.""")]
    [HResultConstant(0x80020003)]
    public static HResult DISP_E_MEMBERNOTFOUND => new(0x80020003);

    /// <summary>Parameter not found.</summary>
    [Description("""Parameter not found.""")]
    [HResultConstant(0x80020004)]
    public static HResult DISP_E_PARAMNOTFOUND => new(0x80020004);

    /// <summary>Type mismatch.</summary>
    [Description("""Type mismatch.""")]
    [HResultConstant(0x80020005)]
    public static HResult DISP_E_TYPEMISMATCH => new(0x80020005);

    /// <summary>Unknown name.</summary>
    [Description("""Unknown name.""")]
    [HResultConstant(0x80020006)]
    public static HResult DISP_E_UNKNOWNNAME => new(0x80020006);

    /// <summary>No named arguments.</summary>
    [Description("""No named arguments.""")]
    [HResultConstant(0x80020007)]
    public static HResult DISP_E_NONAMEDARGS => new(0x80020007);

    /// <summary>Bad variable type.</summary>
    [Description("""Bad variable type.""")]
    [HResultConstant(0x80020008)]
    public static HResult DISP_E_BADVARTYPE => new(0x80020008);

    /// <summary>Exception occurred.</summary>
    [Description("""Exception occurred.""")]
    [HResultConstant(0x80020009)]
    public static HResult DISP_E_EXCEPTION => new(0x80020009);

    /// <summary>Out of present range.</summary>
    [Description("""Out of present range.""")]
    [HResultConstant(0x8002000A)]
    public static HResult DISP_E_OVERFLOW => new(0x8002000A);

    /// <summary>Invalid index.</summary>
    [Description("""Invalid index.""")]
    [HResultConstant(0x8002000B)]
    public static HResult DISP_E_BADINDEX => new(0x8002000B);

    /// <summary>Unknown language.</summary>
    [Description("""Unknown language.""")]
    [HResultConstant(0x8002000C)]
    public static HResult DISP_E_UNKNOWNLCID => new(0x8002000C);

    /// <summary>Memory is locked.</summary>
    [Description("""Memory is locked.""")]
    [HResultConstant(0x8002000D)]
    public static HResult DISP_E_ARRAYISLOCKED => new(0x8002000D);

    /// <summary>Invalid number of parameters.</summary>
    [Description("""Invalid number of parameters.""")]
    [HResultConstant(0x8002000E)]
    public static HResult DISP_E_BADPARAMCOUNT => new(0x8002000E);

    /// <summary>Parameter not optional.</summary>
    [Description("""Parameter not optional.""")]
    [HResultConstant(0x8002000F)]
    public static HResult DISP_E_PARAMNOTOPTIONAL => new(0x8002000F);

    /// <summary>Invalid callee.</summary>
    [Description("""Invalid callee.""")]
    [HResultConstant(0x80020010)]
    public static HResult DISP_E_BADCALLEE => new(0x80020010);

    /// <summary>Does not support a collection.</summary>
    [Description("""Does not support a collection.""")]
    [HResultConstant(0x80020011)]
    public static HResult DISP_E_NOTACOLLECTION => new(0x80020011);

    /// <summary>Division by zero.</summary>
    [Description("""Division by zero.""")]
    [HResultConstant(0x80020012)]
    public static HResult DISP_E_DIVBYZERO => new(0x80020012);

    /// <summary>Buffer too small.</summary>
    [Description("""Buffer too small.""")]
    [HResultConstant(0x80020013)]
    public static HResult DISP_E_BUFFERTOOSMALL => new(0x80020013);

    /// <summary>Buffer too small.</summary>
    [Description("""Buffer too small.""")]
    [HResultConstant(0x80028016)]
    public static HResult TYPE_E_BUFFERTOOSMALL => new(0x80028016);

    /// <summary>Field name not defined in the record.</summary>
    [Description("""Field name not defined in the record.""")]
    [HResultConstant(0x80028017)]
    public static HResult TYPE_E_FIELDNOTFOUND => new(0x80028017);

    /// <summary>Old format or invalid type library.</summary>
    [Description("""Old format or invalid type library.""")]
    [HResultConstant(0x80028018)]
    public static HResult TYPE_E_INVDATAREAD => new(0x80028018);

    /// <summary>Old format or invalid type library.</summary>
    [Description("""Old format or invalid type library.""")]
    [HResultConstant(0x80028019)]
    public static HResult TYPE_E_UNSUPFORMAT => new(0x80028019);

    /// <summary>Error accessing the OLE registry.</summary>
    [Description("""Error accessing the OLE registry.""")]
    [HResultConstant(0x8002801C)]
    public static HResult TYPE_E_REGISTRYACCESS => new(0x8002801C);

    /// <summary>Library not registered.</summary>
    [Description("""Library not registered.""")]
    [HResultConstant(0x8002801D)]
    public static HResult TYPE_E_LIBNOTREGISTERED => new(0x8002801D);

    /// <summary>Bound to unknown type.</summary>
    [Description("""Bound to unknown type.""")]
    [HResultConstant(0x80028027)]
    public static HResult TYPE_E_UNDEFINEDTYPE => new(0x80028027);

    /// <summary>Qualified name disallowed.</summary>
    [Description("""Qualified name disallowed.""")]
    [HResultConstant(0x80028028)]
    public static HResult TYPE_E_QUALIFIEDNAMEDISALLOWED => new(0x80028028);

    /// <summary>Invalid forward reference, or reference to uncompiled type.</summary>
    [Description("""Invalid forward reference, or reference to uncompiled type.""")]
    [HResultConstant(0x80028029)]
    public static HResult TYPE_E_INVALIDSTATE => new(0x80028029);

    /// <summary>Type mismatch.</summary>
    [Description("""Type mismatch.""")]
    [HResultConstant(0x8002802A)]
    public static HResult TYPE_E_WRONGTYPEKIND => new(0x8002802A);

    /// <summary>Element not found.</summary>
    [Description("""Element not found.""")]
    [HResultConstant(0x8002802B)]
    public static HResult TYPE_E_ELEMENTNOTFOUND => new(0x8002802B);

    /// <summary>Ambiguous name.</summary>
    [Description("""Ambiguous name.""")]
    [HResultConstant(0x8002802C)]
    public static HResult TYPE_E_AMBIGUOUSNAME => new(0x8002802C);

    /// <summary>Name already exists in the library.</summary>
    [Description("""Name already exists in the library.""")]
    [HResultConstant(0x8002802D)]
    public static HResult TYPE_E_NAMECONFLICT => new(0x8002802D);

    /// <summary>Unknown language code identifier (LCID).</summary>
    [Description("""Unknown language code identifier (LCID).""")]
    [HResultConstant(0x8002802E)]
    public static HResult TYPE_E_UNKNOWNLCID => new(0x8002802E);

    /// <summary>Function not defined in specified DLL.</summary>
    [Description("""Function not defined in specified DLL.""")]
    [HResultConstant(0x8002802F)]
    public static HResult TYPE_E_DLLFUNCTIONNOTFOUND => new(0x8002802F);

    /// <summary>Wrong module kind for the operation.</summary>
    [Description("""Wrong module kind for the operation.""")]
    [HResultConstant(0x800288BD)]
    public static HResult TYPE_E_BADMODULEKIND => new(0x800288BD);

    /// <summary>Size cannot exceed 64 KB.</summary>
    [Description("""Size cannot exceed 64 KB.""")]
    [HResultConstant(0x800288C5)]
    public static HResult TYPE_E_SIZETOOBIG => new(0x800288C5);

    /// <summary>Duplicate ID in inheritance hierarchy.</summary>
    [Description("""Duplicate ID in inheritance hierarchy.""")]
    [HResultConstant(0x800288C6)]
    public static HResult TYPE_E_DUPLICATEID => new(0x800288C6);

    /// <summary>Incorrect inheritance depth in standard OLE hmember.</summary>
    [Description("""Incorrect inheritance depth in standard OLE hmember.""")]
    [HResultConstant(0x800288CF)]
    public static HResult TYPE_E_INVALIDID => new(0x800288CF);

    /// <summary>Type mismatch.</summary>
    [Description("""Type mismatch.""")]
    [HResultConstant(0x80028CA0)]
    public static HResult TYPE_E_TYPEMISMATCH => new(0x80028CA0);

    /// <summary>Invalid number of arguments.</summary>
    [Description("""Invalid number of arguments.""")]
    [HResultConstant(0x80028CA1)]
    public static HResult TYPE_E_OUTOFBOUNDS => new(0x80028CA1);

    /// <summary>I/O error.</summary>
    [Description("""I/O error.""")]
    [HResultConstant(0x80028CA2)]
    public static HResult TYPE_E_IOERROR => new(0x80028CA2);

    /// <summary>Error creating unique .tmp file.</summary>
    [Description("""Error creating unique .tmp file.""")]
    [HResultConstant(0x80028CA3)]
    public static HResult TYPE_E_CANTCREATETMPFILE => new(0x80028CA3);

    /// <summary>Error loading type library or DLL.</summary>
    [Description("""Error loading type library or DLL.""")]
    [HResultConstant(0x80029C4A)]
    public static HResult TYPE_E_CANTLOADLIBRARY => new(0x80029C4A);

    /// <summary>Inconsistent property functions.</summary>
    [Description("""Inconsistent property functions.""")]
    [HResultConstant(0x80029C83)]
    public static HResult TYPE_E_INCONSISTENTPROPFUNCS => new(0x80029C83);

    /// <summary>Circular dependency between types and modules.</summary>
    [Description("""Circular dependency between types and modules.""")]
    [HResultConstant(0x80029C84)]
    public static HResult TYPE_E_CIRCULARTYPE => new(0x80029C84);

    /// <summary>Unable to perform requested operation.</summary>
    [Description("""Unable to perform requested operation.""")]
    [HResultConstant(0x80030001)]
    public static HResult STG_E_INVALIDFUNCTION => new(0x80030001);

    /// <summary>%1 could not be found.</summary>
    [Description("""%1 could not be found.""")]
    [HResultConstant(0x80030002)]
    public static HResult STG_E_FILENOTFOUND => new(0x80030002);

    /// <summary>The path %1 could not be found.</summary>
    [Description("""The path %1 could not be found.""")]
    [HResultConstant(0x80030003)]
    public static HResult STG_E_PATHNOTFOUND => new(0x80030003);

    /// <summary>There are insufficient resources to open another file.</summary>
    [Description("""There are insufficient resources to open another file.""")]
    [HResultConstant(0x80030004)]
    public static HResult STG_E_TOOMANYOPENFILES => new(0x80030004);

    /// <summary>Access denied.</summary>
    [Description("""Access denied.""")]
    [HResultConstant(0x80030005)]
    public static HResult STG_E_ACCESSDENIED => new(0x80030005);

    /// <summary>Attempted an operation on an invalid object.</summary>
    [Description("""Attempted an operation on an invalid object.""")]
    [HResultConstant(0x80030006)]
    public static HResult STG_E_INVALIDHANDLE => new(0x80030006);

    /// <summary>There is insufficient memory available to complete operation.</summary>
    [Description("""There is insufficient memory available to complete operation.""")]
    [HResultConstant(0x80030008)]
    public static HResult STG_E_INSUFFICIENTMEMORY => new(0x80030008);

    /// <summary>Invalid pointer error.</summary>
    [Description("""Invalid pointer error.""")]
    [HResultConstant(0x80030009)]
    public static HResult STG_E_INVALIDPOINTER => new(0x80030009);

    /// <summary>There are no more entries to return.</summary>
    [Description("""There are no more entries to return.""")]
    [HResultConstant(0x80030012)]
    public static HResult STG_E_NOMOREFILES => new(0x80030012);

    /// <summary>Disk is write-protected.</summary>
    [Description("""Disk is write-protected.""")]
    [HResultConstant(0x80030013)]
    public static HResult STG_E_DISKISWRITEPROTECTED => new(0x80030013);

    /// <summary>An error occurred during a seek operation.</summary>
    [Description("""An error occurred during a seek operation.""")]
    [HResultConstant(0x80030019)]
    public static HResult STG_E_SEEKERROR => new(0x80030019);

    /// <summary>A disk error occurred during a write operation.</summary>
    [Description("""A disk error occurred during a write operation.""")]
    [HResultConstant(0x8003001D)]
    public static HResult STG_E_WRITEFAULT => new(0x8003001D);

    /// <summary>A disk error occurred during a read operation.</summary>
    [Description("""A disk error occurred during a read operation.""")]
    [HResultConstant(0x8003001E)]
    public static HResult STG_E_READFAULT => new(0x8003001E);

    /// <summary>A share violation has occurred.</summary>
    [Description("""A share violation has occurred.""")]
    [HResultConstant(0x80030020)]
    public static HResult STG_E_SHAREVIOLATION => new(0x80030020);

    /// <summary>A lock violation has occurred.</summary>
    [Description("""A lock violation has occurred.""")]
    [HResultConstant(0x80030021)]
    public static HResult STG_E_LOCKVIOLATION => new(0x80030021);

    /// <summary>%1 already exists.</summary>
    [Description("""%1 already exists.""")]
    [HResultConstant(0x80030050)]
    public static HResult STG_E_FILEALREADYEXISTS => new(0x80030050);

    /// <summary>Invalid parameter error.</summary>
    [Description("""Invalid parameter error.""")]
    [HResultConstant(0x80030057)]
    public static HResult STG_E_INVALIDPARAMETER => new(0x80030057);

    /// <summary>There is insufficient disk space to complete operation.</summary>
    [Description("""There is insufficient disk space to complete operation.""")]
    [HResultConstant(0x80030070)]
    public static HResult STG_E_MEDIUMFULL => new(0x80030070);

    /// <summary>Illegal write of non-simple property to simple property set.</summary>
    [Description("""Illegal write of non-simple property to simple property set.""")]
    [HResultConstant(0x800300F0)]
    public static HResult STG_E_PROPSETMISMATCHED => new(0x800300F0);

    /// <summary>An application programming interface (API) call exited abnormally.</summary>
    [Description("""An application programming interface (API) call exited abnormally.""")]
    [HResultConstant(0x800300FA)]
    public static HResult STG_E_ABNORMALAPIEXIT => new(0x800300FA);

    /// <summary>The file %1 is not a valid compound file.</summary>
    [Description("""The file %1 is not a valid compound file.""")]
    [HResultConstant(0x800300FB)]
    public static HResult STG_E_INVALIDHEADER => new(0x800300FB);

    /// <summary>The name %1 is not valid.</summary>
    [Description("""The name %1 is not valid.""")]
    [HResultConstant(0x800300FC)]
    public static HResult STG_E_INVALIDNAME => new(0x800300FC);

    /// <summary>An unexpected error occurred.</summary>
    [Description("""An unexpected error occurred.""")]
    [HResultConstant(0x800300FD)]
    public static HResult STG_E_UNKNOWN => new(0x800300FD);

    /// <summary>That function is not implemented.</summary>
    [Description("""That function is not implemented.""")]
    [HResultConstant(0x800300FE)]
    public static HResult STG_E_UNIMPLEMENTEDFUNCTION => new(0x800300FE);

    /// <summary>Invalid flag error.</summary>
    [Description("""Invalid flag error.""")]
    [HResultConstant(0x800300FF)]
    public static HResult STG_E_INVALIDFLAG => new(0x800300FF);

    /// <summary>Attempted to use an object that is busy.</summary>
    [Description("""Attempted to use an object that is busy.""")]
    [HResultConstant(0x80030100)]
    public static HResult STG_E_INUSE => new(0x80030100);

    /// <summary>The storage has been changed since the last commit.</summary>
    [Description("""The storage has been changed since the last commit.""")]
    [HResultConstant(0x80030101)]
    public static HResult STG_E_NOTCURRENT => new(0x80030101);

    /// <summary>Attempted to use an object that has ceased to exist.</summary>
    [Description("""Attempted to use an object that has ceased to exist.""")]
    [HResultConstant(0x80030102)]
    public static HResult STG_E_REVERTED => new(0x80030102);

    /// <summary>Cannot save.</summary>
    [Description("""Cannot save.""")]
    [HResultConstant(0x80030103)]
    public static HResult STG_E_CANTSAVE => new(0x80030103);

    /// <summary>The compound file %1 was produced with an incompatible version of storage.</summary>
    [Description("""The compound file %1 was produced with an incompatible version of storage.""")]
    [HResultConstant(0x80030104)]
    public static HResult STG_E_OLDFORMAT => new(0x80030104);

    /// <summary>The compound file %1 was produced with a newer version of storage.</summary>
    [Description("""The compound file %1 was produced with a newer version of storage.""")]
    [HResultConstant(0x80030105)]
    public static HResult STG_E_OLDDLL => new(0x80030105);

    /// <summary>Share.exe or equivalent is required for operation.</summary>
    [Description("""Share.exe or equivalent is required for operation.""")]
    [HResultConstant(0x80030106)]
    public static HResult STG_E_SHAREREQUIRED => new(0x80030106);

    /// <summary>Illegal operation called on non-file based storage.</summary>
    [Description("""Illegal operation called on non-file based storage.""")]
    [HResultConstant(0x80030107)]
    public static HResult STG_E_NOTFILEBASEDSTORAGE => new(0x80030107);

    /// <summary>Illegal operation called on object with extant marshalings.</summary>
    [Description("""Illegal operation called on object with extant marshalings.""")]
    [HResultConstant(0x80030108)]
    public static HResult STG_E_EXTANTMARSHALLINGS => new(0x80030108);

    /// <summary>The docfile has been corrupted.</summary>
    [Description("""The docfile has been corrupted.""")]
    [HResultConstant(0x80030109)]
    public static HResult STG_E_DOCFILECORRUPT => new(0x80030109);

    /// <summary>OLE32.DLL has been loaded at the wrong address.</summary>
    [Description("""OLE32.DLL has been loaded at the wrong address.""")]
    [HResultConstant(0x80030110)]
    public static HResult STG_E_BADBASEADDRESS => new(0x80030110);

    /// <summary>The compound file is too large for the current implementation.</summary>
    [Description("""The compound file is too large for the current implementation.""")]
    [HResultConstant(0x80030111)]
    public static HResult STG_E_DOCFILETOOLARGE => new(0x80030111);

    /// <summary>The compound file was not created with the STGM_SIMPLE flag.</summary>
    [Description("""The compound file was not created with the STGM_SIMPLE flag.""")]
    [HResultConstant(0x80030112)]
    public static HResult STG_E_NOTSIMPLEFORMAT => new(0x80030112);

    /// <summary>The file download was aborted abnormally. The file is incomplete.</summary>
    [Description("""The file download was aborted abnormally. The file is incomplete.""")]
    [HResultConstant(0x80030201)]
    public static HResult STG_E_INCOMPLETE => new(0x80030201);

    /// <summary>The file download has been terminated.</summary>
    [Description("""The file download has been terminated.""")]
    [HResultConstant(0x80030202)]
    public static HResult STG_E_TERMINATED => new(0x80030202);

    /// <summary>Generic Copy Protection Error.</summary>
    [Description("""Generic Copy Protection Error.""")]
    [HResultConstant(0x80030305)]
    public static HResult STG_E_STATUS_COPY_PROTECTION_FAILURE => new(0x80030305);

    /// <summary>Copy Protection Error—DVD CSS Authentication failed.</summary>
    [Description("""Copy Protection Error—DVD CSS Authentication failed.""")]
    [HResultConstant(0x80030306)]
    public static HResult STG_E_CSS_AUTHENTICATION_FAILURE => new(0x80030306);

    /// <summary>Copy Protection Error—The given sector does not have a valid CSS key.</summary>
    [Description("""Copy Protection Error—The given sector does not have a valid CSS key.""")]
    [HResultConstant(0x80030307)]
    public static HResult STG_E_CSS_KEY_NOT_PRESENT => new(0x80030307);

    /// <summary>Copy Protection Error—DVD session key not established.</summary>
    [Description("""Copy Protection Error—DVD session key not established.""")]
    [HResultConstant(0x80030308)]
    public static HResult STG_E_CSS_KEY_NOT_ESTABLISHED => new(0x80030308);

    /// <summary>Copy Protection Error—The read failed because the sector is encrypted.</summary>
    [Description("""Copy Protection Error—The read failed because the sector is encrypted.""")]
    [HResultConstant(0x80030309)]
    public static HResult STG_E_CSS_SCRAMBLED_SECTOR => new(0x80030309);

    /// <summary>Copy Protection Error—The current DVD's region does not correspond to the region setting of the drive.</summary>
    [Description("""Copy Protection Error—The current DVD's region does not correspond to the region setting of the drive.""")]
    [HResultConstant(0x8003030A)]
    public static HResult STG_E_CSS_REGION_MISMATCH => new(0x8003030A);

    /// <summary>Copy Protection Error—The drive's region setting might be permanent or the number of user resets has been exhausted.</summary>
    [Description("""Copy Protection Error—The drive's region setting might be permanent or the number of user resets has been exhausted.""")]
    [HResultConstant(0x8003030B)]
    public static HResult STG_E_RESETS_EXHAUSTED => new(0x8003030B);

    /// <summary>Invalid OLEVERB structure.</summary>
    [Description("""Invalid OLEVERB structure.""")]
    [HResultConstant(0x80040000)]
    public static HResult OLE_E_OLEVERB => new(0x80040000);

    /// <summary>Invalid advise flags.</summary>
    [Description("""Invalid advise flags.""")]
    [HResultConstant(0x80040001)]
    public static HResult OLE_E_ADVF => new(0x80040001);

    /// <summary>Cannot enumerate any more because the associated data is missing.</summary>
    [Description("""Cannot enumerate any more because the associated data is missing.""")]
    [HResultConstant(0x80040002)]
    public static HResult OLE_E_ENUM_NOMORE => new(0x80040002);

    /// <summary>This implementation does not take advises.</summary>
    [Description("""This implementation does not take advises.""")]
    [HResultConstant(0x80040003)]
    public static HResult OLE_E_ADVISENOTSUPPORTED => new(0x80040003);

    /// <summary>There is no connection for this connection ID.</summary>
    [Description("""There is no connection for this connection ID.""")]
    [HResultConstant(0x80040004)]
    public static HResult OLE_E_NOCONNECTION => new(0x80040004);

    /// <summary>Need to run the object to perform this operation.</summary>
    [Description("""Need to run the object to perform this operation.""")]
    [HResultConstant(0x80040005)]
    public static HResult OLE_E_NOTRUNNING => new(0x80040005);

    /// <summary>There is no cache to operate on.</summary>
    [Description("""There is no cache to operate on.""")]
    [HResultConstant(0x80040006)]
    public static HResult OLE_E_NOCACHE => new(0x80040006);

    /// <summary>Uninitialized object.</summary>
    [Description("""Uninitialized object.""")]
    [HResultConstant(0x80040007)]
    public static HResult OLE_E_BLANK => new(0x80040007);

    /// <summary>Linked object's source class has changed.</summary>
    [Description("""Linked object's source class has changed.""")]
    [HResultConstant(0x80040008)]
    public static HResult OLE_E_CLASSDIFF => new(0x80040008);

    /// <summary>Not able to get the moniker of the object.</summary>
    [Description("""Not able to get the moniker of the object.""")]
    [HResultConstant(0x80040009)]
    public static HResult OLE_E_CANT_GETMONIKER => new(0x80040009);

    /// <summary>Not able to bind to the source.</summary>
    [Description("""Not able to bind to the source.""")]
    [HResultConstant(0x8004000A)]
    public static HResult OLE_E_CANT_BINDTOSOURCE => new(0x8004000A);

    /// <summary>Object is static; operation not allowed.</summary>
    [Description("""Object is static; operation not allowed.""")]
    [HResultConstant(0x8004000B)]
    public static HResult OLE_E_STATIC => new(0x8004000B);

    /// <summary>User canceled out of the Save dialog box.</summary>
    [Description("""User canceled out of the Save dialog box.""")]
    [HResultConstant(0x8004000C)]
    public static HResult OLE_E_PROMPTSAVECANCELLED => new(0x8004000C);

    /// <summary>Invalid rectangle.</summary>
    [Description("""Invalid rectangle.""")]
    [HResultConstant(0x8004000D)]
    public static HResult OLE_E_INVALIDRECT => new(0x8004000D);

    /// <summary>compobj.dll is too old for the ole2.dll initialized.</summary>
    [Description("""compobj.dll is too old for the ole2.dll initialized.""")]
    [HResultConstant(0x8004000E)]
    public static HResult OLE_E_WRONGCOMPOBJ => new(0x8004000E);

    /// <summary>Invalid window handle.</summary>
    [Description("""Invalid window handle.""")]
    [HResultConstant(0x8004000F)]
    public static HResult OLE_E_INVALIDHWND => new(0x8004000F);

    /// <summary>Object is not in any of the inplace active states.</summary>
    [Description("""Object is not in any of the inplace active states.""")]
    [HResultConstant(0x80040010)]
    public static HResult OLE_E_NOT_INPLACEACTIVE => new(0x80040010);

    /// <summary>Not able to convert object.</summary>
    [Description("""Not able to convert object.""")]
    [HResultConstant(0x80040011)]
    public static HResult OLE_E_CANTCONVERT => new(0x80040011);

    /// <summary>Not able to perform the operation because object is not given storage yet.</summary>
    [Description("""Not able to perform the operation because object is not given storage yet.""")]
    [HResultConstant(0x80040012)]
    public static HResult OLE_E_NOSTORAGE => new(0x80040012);

    /// <summary>Invalid FORMATETC structure.</summary>
    [Description("""Invalid FORMATETC structure.""")]
    [HResultConstant(0x80040064)]
    public static HResult DV_E_FORMATETC => new(0x80040064);

    /// <summary>Invalid DVTARGETDEVICE structure.</summary>
    [Description("""Invalid DVTARGETDEVICE structure.""")]
    [HResultConstant(0x80040065)]
    public static HResult DV_E_DVTARGETDEVICE => new(0x80040065);

    /// <summary>Invalid STDGMEDIUM structure.</summary>
    [Description("""Invalid STDGMEDIUM structure.""")]
    [HResultConstant(0x80040066)]
    public static HResult DV_E_STGMEDIUM => new(0x80040066);

    /// <summary>Invalid STATDATA structure.</summary>
    [Description("""Invalid STATDATA structure.""")]
    [HResultConstant(0x80040067)]
    public static HResult DV_E_STATDATA => new(0x80040067);

    /// <summary>Invalid lindex.</summary>
    [Description("""Invalid lindex.""")]
    [HResultConstant(0x80040068)]
    public static HResult DV_E_LINDEX => new(0x80040068);

    /// <summary>Invalid TYMED structure.</summary>
    [Description("""Invalid TYMED structure.""")]
    [HResultConstant(0x80040069)]
    public static HResult DV_E_TYMED => new(0x80040069);

    /// <summary>Invalid clipboard format.</summary>
    [Description("""Invalid clipboard format.""")]
    [HResultConstant(0x8004006A)]
    public static HResult DV_E_CLIPFORMAT => new(0x8004006A);

    /// <summary>Invalid aspects.</summary>
    [Description("""Invalid aspects.""")]
    [HResultConstant(0x8004006B)]
    public static HResult DV_E_DVASPECT => new(0x8004006B);

    /// <summary>The tdSize parameter of the DVTARGETDEVICE structure is invalid.</summary>
    [Description("""The tdSize parameter of the DVTARGETDEVICE structure is invalid.""")]
    [HResultConstant(0x8004006C)]
    public static HResult DV_E_DVTARGETDEVICE_SIZE => new(0x8004006C);

    /// <summary>Object does not support IViewObject interface.</summary>
    [Description("""Object does not support IViewObject interface.""")]
    [HResultConstant(0x8004006D)]
    public static HResult DV_E_NOIVIEWOBJECT => new(0x8004006D);

    /// <summary>Trying to revoke a drop target that has not been registered.</summary>
    [Description("""Trying to revoke a drop target that has not been registered.""")]
    [HResultConstant(0x80040100)]
    public static HResult DRAGDROP_E_NOTREGISTERED => new(0x80040100);

    /// <summary>This window has already been registered as a drop target.</summary>
    [Description("""This window has already been registered as a drop target.""")]
    [HResultConstant(0x80040101)]
    public static HResult DRAGDROP_E_ALREADYREGISTERED => new(0x80040101);

    /// <summary>Invalid window handle.</summary>
    [Description("""Invalid window handle.""")]
    [HResultConstant(0x80040102)]
    public static HResult DRAGDROP_E_INVALIDHWND => new(0x80040102);

    /// <summary>Class does not support aggregation (or class object is remote).</summary>
    [Description("""Class does not support aggregation (or class object is remote).""")]
    [HResultConstant(0x80040110)]
    public static HResult CLASS_E_NOAGGREGATION => new(0x80040110);

    /// <summary>ClassFactory cannot supply requested class.</summary>
    [Description("""ClassFactory cannot supply requested class.""")]
    [HResultConstant(0x80040111)]
    public static HResult CLASS_E_CLASSNOTAVAILABLE => new(0x80040111);

    /// <summary>Class is not licensed for use.</summary>
    [Description("""Class is not licensed for use.""")]
    [HResultConstant(0x80040112)]
    public static HResult CLASS_E_NOTLICENSED => new(0x80040112);

    /// <summary>Error drawing view.</summary>
    [Description("""Error drawing view.""")]
    [HResultConstant(0x80040140)]
    public static HResult VIEW_E_DRAW => new(0x80040140);

    /// <summary>Could not read key from registry.</summary>
    [Description("""Could not read key from registry.""")]
    [HResultConstant(0x80040150)]
    public static HResult REGDB_E_READREGDB => new(0x80040150);

    /// <summary>Could not write key to registry.</summary>
    [Description("""Could not write key to registry.""")]
    [HResultConstant(0x80040151)]
    public static HResult REGDB_E_WRITEREGDB => new(0x80040151);

    /// <summary>Could not find the key in the registry.</summary>
    [Description("""Could not find the key in the registry.""")]
    [HResultConstant(0x80040152)]
    public static HResult REGDB_E_KEYMISSING => new(0x80040152);

    /// <summary>Invalid value for registry.</summary>
    [Description("""Invalid value for registry.""")]
    [HResultConstant(0x80040153)]
    public static HResult REGDB_E_INVALIDVALUE => new(0x80040153);

    /// <summary>Class not registered.</summary>
    [Description("""Class not registered.""")]
    [HResultConstant(0x80040154)]
    public static HResult REGDB_E_CLASSNOTREG => new(0x80040154);

    /// <summary>Interface not registered.</summary>
    [Description("""Interface not registered.""")]
    [HResultConstant(0x80040155)]
    public static HResult REGDB_E_IIDNOTREG => new(0x80040155);

    /// <summary>Threading model entry is not valid.</summary>
    [Description("""Threading model entry is not valid.""")]
    [HResultConstant(0x80040156)]
    public static HResult REGDB_E_BADTHREADINGMODEL => new(0x80040156);

    /// <summary>CATID does not exist.</summary>
    [Description("""CATID does not exist.""")]
    [HResultConstant(0x80040160)]
    public static HResult CAT_E_CATIDNOEXIST => new(0x80040160);

    /// <summary>Description not found.</summary>
    [Description("""Description not found.""")]
    [HResultConstant(0x80040161)]
    public static HResult CAT_E_NODESCRIPTION => new(0x80040161);

    /// <summary>No package in the software installation data in Active Directory meets this criteria.</summary>
    [Description("""No package in the software installation data in Active Directory meets this criteria.""")]
    [HResultConstant(0x80040164)]
    public static HResult CS_E_PACKAGE_NOTFOUND => new(0x80040164);

    /// <summary>Deleting this will break the referential integrity of the software installation data in Active Directory.</summary>
    [Description("""Deleting this will break the referential integrity of the software installation data in Active Directory.""")]
    [HResultConstant(0x80040165)]
    public static HResult CS_E_NOT_DELETABLE => new(0x80040165);

    /// <summary>The CLSID was not found in the software installation data in Active Directory.</summary>
    [Description("""The CLSID was not found in the software installation data in Active Directory.""")]
    [HResultConstant(0x80040166)]
    public static HResult CS_E_CLASS_NOTFOUND => new(0x80040166);

    /// <summary>The software installation data in Active Directory is corrupt.</summary>
    [Description("""The software installation data in Active Directory is corrupt.""")]
    [HResultConstant(0x80040167)]
    public static HResult CS_E_INVALID_VERSION => new(0x80040167);

    /// <summary>There is no software installation data in Active Directory.</summary>
    [Description("""There is no software installation data in Active Directory.""")]
    [HResultConstant(0x80040168)]
    public static HResult CS_E_NO_CLASSSTORE => new(0x80040168);

    /// <summary>There is no software installation data object in Active Directory.</summary>
    [Description("""There is no software installation data object in Active Directory.""")]
    [HResultConstant(0x80040169)]
    public static HResult CS_E_OBJECT_NOTFOUND => new(0x80040169);

    /// <summary>The software installation data object in Active Directory already exists.</summary>
    [Description("""The software installation data object in Active Directory already exists.""")]
    [HResultConstant(0x8004016A)]
    public static HResult CS_E_OBJECT_ALREADY_EXISTS => new(0x8004016A);

    /// <summary>The path to the software installation data in Active Directory is not correct.</summary>
    [Description("""The path to the software installation data in Active Directory is not correct.""")]
    [HResultConstant(0x8004016B)]
    public static HResult CS_E_INVALID_PATH => new(0x8004016B);

    /// <summary>A network error interrupted the operation.</summary>
    [Description("""A network error interrupted the operation.""")]
    [HResultConstant(0x8004016C)]
    public static HResult CS_E_NETWORK_ERROR => new(0x8004016C);

    /// <summary>The size of this object exceeds the maximum size set by the administrator.</summary>
    [Description("""The size of this object exceeds the maximum size set by the administrator.""")]
    [HResultConstant(0x8004016D)]
    public static HResult CS_E_ADMIN_LIMIT_EXCEEDED => new(0x8004016D);

    /// <summary>The schema for the software installation data in Active Directory does not match the required schema.</summary>
    [Description("""The schema for the software installation data in Active Directory does not match the required schema.""")]
    [HResultConstant(0x8004016E)]
    public static HResult CS_E_SCHEMA_MISMATCH => new(0x8004016E);

    /// <summary>An error occurred in the software installation data in Active Directory.</summary>
    [Description("""An error occurred in the software installation data in Active Directory.""")]
    [HResultConstant(0x8004016F)]
    public static HResult CS_E_INTERNAL_ERROR => new(0x8004016F);

    /// <summary>Cache not updated.</summary>
    [Description("""Cache not updated.""")]
    [HResultConstant(0x80040170)]
    public static HResult CACHE_E_NOCACHE_UPDATED => new(0x80040170);

    /// <summary>No verbs for OLE object.</summary>
    [Description("""No verbs for OLE object.""")]
    [HResultConstant(0x80040180)]
    public static HResult OLEOBJ_E_NOVERBS => new(0x80040180);

    /// <summary>Invalid verb for OLE object.</summary>
    [Description("""Invalid verb for OLE object.""")]
    [HResultConstant(0x80040181)]
    public static HResult OLEOBJ_E_INVALIDVERB => new(0x80040181);

    /// <summary>Undo is not available.</summary>
    [Description("""Undo is not available.""")]
    [HResultConstant(0x800401A0)]
    public static HResult INPLACE_E_NOTUNDOABLE => new(0x800401A0);

    /// <summary>Space for tools is not available.</summary>
    [Description("""Space for tools is not available.""")]
    [HResultConstant(0x800401A1)]
    public static HResult INPLACE_E_NOTOOLSPACE => new(0x800401A1);

    /// <summary>OLESTREAM Get method failed.</summary>
    [Description("""OLESTREAM Get method failed.""")]
    [HResultConstant(0x800401C0)]
    public static HResult CONVERT10_E_OLESTREAM_GET => new(0x800401C0);

    /// <summary>OLESTREAM Put method failed.</summary>
    [Description("""OLESTREAM Put method failed.""")]
    [HResultConstant(0x800401C1)]
    public static HResult CONVERT10_E_OLESTREAM_PUT => new(0x800401C1);

    /// <summary>Contents of the OLESTREAM not in correct format.</summary>
    [Description("""Contents of the OLESTREAM not in correct format.""")]
    [HResultConstant(0x800401C2)]
    public static HResult CONVERT10_E_OLESTREAM_FMT => new(0x800401C2);

    /// <summary>There was an error in a Windows GDI call while converting the bitmap to a device-independent bitmap (DIB).</summary>
    [Description("""There was an error in a Windows GDI call while converting the bitmap to a device-independent bitmap (DIB).""")]
    [HResultConstant(0x800401C3)]
    public static HResult CONVERT10_E_OLESTREAM_BITMAP_TO_DIB => new(0x800401C3);

    /// <summary>Contents of the IStorage not in correct format.</summary>
    [Description("""Contents of the IStorage not in correct format.""")]
    [HResultConstant(0x800401C4)]
    public static HResult CONVERT10_E_STG_FMT => new(0x800401C4);

    /// <summary>Contents of IStorage is missing one of the standard streams.</summary>
    [Description("""Contents of IStorage is missing one of the standard streams.""")]
    [HResultConstant(0x800401C5)]
    public static HResult CONVERT10_E_STG_NO_STD_STREAM => new(0x800401C5);

    /// <summary>There was an error in a Windows Graphics Device Interface (GDI) call while converting the DIB to a bitmap.</summary>
    [Description("""There was an error in a Windows Graphics Device Interface (GDI) call while converting the DIB to a bitmap.""")]
    [HResultConstant(0x800401C6)]
    public static HResult CONVERT10_E_STG_DIB_TO_BITMAP => new(0x800401C6);

    /// <summary>OpenClipboard failed.</summary>
    [Description("""OpenClipboard failed.""")]
    [HResultConstant(0x800401D0)]
    public static HResult CLIPBRD_E_CANT_OPEN => new(0x800401D0);

    /// <summary>EmptyClipboard failed.</summary>
    [Description("""EmptyClipboard failed.""")]
    [HResultConstant(0x800401D1)]
    public static HResult CLIPBRD_E_CANT_EMPTY => new(0x800401D1);

    /// <summary>SetClipboard failed.</summary>
    [Description("""SetClipboard failed.""")]
    [HResultConstant(0x800401D2)]
    public static HResult CLIPBRD_E_CANT_SET => new(0x800401D2);

    /// <summary>Data on clipboard is invalid.</summary>
    [Description("""Data on clipboard is invalid.""")]
    [HResultConstant(0x800401D3)]
    public static HResult CLIPBRD_E_BAD_DATA => new(0x800401D3);

    /// <summary>CloseClipboard failed.</summary>
    [Description("""CloseClipboard failed.""")]
    [HResultConstant(0x800401D4)]
    public static HResult CLIPBRD_E_CANT_CLOSE => new(0x800401D4);

    /// <summary>Moniker needs to be connected manually.</summary>
    [Description("""Moniker needs to be connected manually.""")]
    [HResultConstant(0x800401E0)]
    public static HResult MK_E_CONNECTMANUALLY => new(0x800401E0);

    /// <summary>Operation exceeded deadline.</summary>
    [Description("""Operation exceeded deadline.""")]
    [HResultConstant(0x800401E1)]
    public static HResult MK_E_EXCEEDEDDEADLINE => new(0x800401E1);

    /// <summary>Moniker needs to be generic.</summary>
    [Description("""Moniker needs to be generic.""")]
    [HResultConstant(0x800401E2)]
    public static HResult MK_E_NEEDGENERIC => new(0x800401E2);

    /// <summary>Operation unavailable.</summary>
    [Description("""Operation unavailable.""")]
    [HResultConstant(0x800401E3)]
    public static HResult MK_E_UNAVAILABLE => new(0x800401E3);

    /// <summary>Invalid syntax.</summary>
    [Description("""Invalid syntax.""")]
    [HResultConstant(0x800401E4)]
    public static HResult MK_E_SYNTAX => new(0x800401E4);

    /// <summary>No object for moniker.</summary>
    [Description("""No object for moniker.""")]
    [HResultConstant(0x800401E5)]
    public static HResult MK_E_NOOBJECT => new(0x800401E5);

    /// <summary>Bad extension for file.</summary>
    [Description("""Bad extension for file.""")]
    [HResultConstant(0x800401E6)]
    public static HResult MK_E_INVALIDEXTENSION => new(0x800401E6);

    /// <summary>Intermediate operation failed.</summary>
    [Description("""Intermediate operation failed.""")]
    [HResultConstant(0x800401E7)]
    public static HResult MK_E_INTERMEDIATEINTERFACENOTSUPPORTED => new(0x800401E7);

    /// <summary>Moniker is not bindable.</summary>
    [Description("""Moniker is not bindable.""")]
    [HResultConstant(0x800401E8)]
    public static HResult MK_E_NOTBINDABLE => new(0x800401E8);

    /// <summary>Moniker is not bound.</summary>
    [Description("""Moniker is not bound.""")]
    [HResultConstant(0x800401E9)]
    public static HResult MK_E_NOTBOUND => new(0x800401E9);

    /// <summary>Moniker cannot open file.</summary>
    [Description("""Moniker cannot open file.""")]
    [HResultConstant(0x800401EA)]
    public static HResult MK_E_CANTOPENFILE => new(0x800401EA);

    /// <summary>User input required for operation to succeed.</summary>
    [Description("""User input required for operation to succeed.""")]
    [HResultConstant(0x800401EB)]
    public static HResult MK_E_MUSTBOTHERUSER => new(0x800401EB);

    /// <summary>Moniker class has no inverse.</summary>
    [Description("""Moniker class has no inverse.""")]
    [HResultConstant(0x800401EC)]
    public static HResult MK_E_NOINVERSE => new(0x800401EC);

    /// <summary>Moniker does not refer to storage.</summary>
    [Description("""Moniker does not refer to storage.""")]
    [HResultConstant(0x800401ED)]
    public static HResult MK_E_NOSTORAGE => new(0x800401ED);

    /// <summary>No common prefix.</summary>
    [Description("""No common prefix.""")]
    [HResultConstant(0x800401EE)]
    public static HResult MK_E_NOPREFIX => new(0x800401EE);

    /// <summary>Moniker could not be enumerated.</summary>
    [Description("""Moniker could not be enumerated.""")]
    [HResultConstant(0x800401EF)]
    public static HResult MK_E_ENUMERATION_FAILED => new(0x800401EF);

    /// <summary>CoInitialize has not been called.</summary>
    [Description("""CoInitialize has not been called.""")]
    [HResultConstant(0x800401F0)]
    public static HResult CO_E_NOTINITIALIZED => new(0x800401F0);

    /// <summary>CoInitialize has already been called.</summary>
    [Description("""CoInitialize has already been called.""")]
    [HResultConstant(0x800401F1)]
    public static HResult CO_E_ALREADYINITIALIZED => new(0x800401F1);

    /// <summary>Class of object cannot be determined.</summary>
    [Description("""Class of object cannot be determined.""")]
    [HResultConstant(0x800401F2)]
    public static HResult CO_E_CANTDETERMINECLASS => new(0x800401F2);

    /// <summary>Invalid class string.</summary>
    [Description("""Invalid class string.""")]
    [HResultConstant(0x800401F3)]
    public static HResult CO_E_CLASSSTRING => new(0x800401F3);

    /// <summary>Invalid interface string.</summary>
    [Description("""Invalid interface string.""")]
    [HResultConstant(0x800401F4)]
    public static HResult CO_E_IIDSTRING => new(0x800401F4);

    /// <summary>Application not found.</summary>
    [Description("""Application not found.""")]
    [HResultConstant(0x800401F5)]
    public static HResult CO_E_APPNOTFOUND => new(0x800401F5);

    /// <summary>Application cannot be run more than once.</summary>
    [Description("""Application cannot be run more than once.""")]
    [HResultConstant(0x800401F6)]
    public static HResult CO_E_APPSINGLEUSE => new(0x800401F6);

    /// <summary>Some error in application.</summary>
    [Description("""Some error in application.""")]
    [HResultConstant(0x800401F7)]
    public static HResult CO_E_ERRORINAPP => new(0x800401F7);

    /// <summary>DLL for class not found.</summary>
    [Description("""DLL for class not found.""")]
    [HResultConstant(0x800401F8)]
    public static HResult CO_E_DLLNOTFOUND => new(0x800401F8);

    /// <summary>Error in the DLL.</summary>
    [Description("""Error in the DLL.""")]
    [HResultConstant(0x800401F9)]
    public static HResult CO_E_ERRORINDLL => new(0x800401F9);

    /// <summary>Wrong operating system or operating system version for application.</summary>
    [Description("""Wrong operating system or operating system version for application.""")]
    [HResultConstant(0x800401FA)]
    public static HResult CO_E_WRONGOSFORAPP => new(0x800401FA);

    /// <summary>Object is not registered.</summary>
    [Description("""Object is not registered.""")]
    [HResultConstant(0x800401FB)]
    public static HResult CO_E_OBJNOTREG => new(0x800401FB);

    /// <summary>Object is already registered.</summary>
    [Description("""Object is already registered.""")]
    [HResultConstant(0x800401FC)]
    public static HResult CO_E_OBJISREG => new(0x800401FC);

    /// <summary>Object is not connected to server.</summary>
    [Description("""Object is not connected to server.""")]
    [HResultConstant(0x800401FD)]
    public static HResult CO_E_OBJNOTCONNECTED => new(0x800401FD);

    /// <summary>Application was launched, but it did not register a class factory.</summary>
    [Description("""Application was launched, but it did not register a class factory.""")]
    [HResultConstant(0x800401FE)]
    public static HResult CO_E_APPDIDNTREG => new(0x800401FE);

    /// <summary>Object has been released.</summary>
    [Description("""Object has been released.""")]
    [HResultConstant(0x800401FF)]
    public static HResult CO_E_RELEASED => new(0x800401FF);

    /// <summary>An event was unable to invoke any of the subscribers.</summary>
    [Description("""An event was unable to invoke any of the subscribers.""")]
    [HResultConstant(0x80040201)]
    public static HResult EVENT_E_ALL_SUBSCRIBERS_FAILED => new(0x80040201);

    /// <summary>A syntax error occurred trying to evaluate a query string.</summary>
    [Description("""A syntax error occurred trying to evaluate a query string.""")]
    [HResultConstant(0x80040203)]
    public static HResult EVENT_E_QUERYSYNTAX => new(0x80040203);

    /// <summary>An invalid field name was used in a query string.</summary>
    [Description("""An invalid field name was used in a query string.""")]
    [HResultConstant(0x80040204)]
    public static HResult EVENT_E_QUERYFIELD => new(0x80040204);

    /// <summary>An unexpected exception was raised.</summary>
    [Description("""An unexpected exception was raised.""")]
    [HResultConstant(0x80040205)]
    public static HResult EVENT_E_INTERNALEXCEPTION => new(0x80040205);

    /// <summary>An unexpected internal error was detected.</summary>
    [Description("""An unexpected internal error was detected.""")]
    [HResultConstant(0x80040206)]
    public static HResult EVENT_E_INTERNALERROR => new(0x80040206);

    /// <summary>The owner security identifier (SID) on a per-user subscription does not exist.</summary>
    [Description("""The owner security identifier (SID) on a per-user subscription does not exist.""")]
    [HResultConstant(0x80040207)]
    public static HResult EVENT_E_INVALID_PER_USER_SID => new(0x80040207);

    /// <summary>A user-supplied component or subscriber raised an exception.</summary>
    [Description("""A user-supplied component or subscriber raised an exception.""")]
    [HResultConstant(0x80040208)]
    public static HResult EVENT_E_USER_EXCEPTION => new(0x80040208);

    /// <summary>An interface has too many methods to fire events from.</summary>
    [Description("""An interface has too many methods to fire events from.""")]
    [HResultConstant(0x80040209)]
    public static HResult EVENT_E_TOO_MANY_METHODS => new(0x80040209);

    /// <summary>A subscription cannot be stored unless its event class already exists.</summary>
    [Description("""A subscription cannot be stored unless its event class already exists.""")]
    [HResultConstant(0x8004020A)]
    public static HResult EVENT_E_MISSING_EVENTCLASS => new(0x8004020A);

    /// <summary>Not all the objects requested could be removed.</summary>
    [Description("""Not all the objects requested could be removed.""")]
    [HResultConstant(0x8004020B)]
    public static HResult EVENT_E_NOT_ALL_REMOVED => new(0x8004020B);

    /// <summary>COM+ is required for this operation, but it is not installed.</summary>
    [Description("""COM+ is required for this operation, but it is not installed.""")]
    [HResultConstant(0x8004020C)]
    public static HResult EVENT_E_COMPLUS_NOT_INSTALLED => new(0x8004020C);

    /// <summary>Cannot modify or delete an object that was not added using the COM+ Administrative SDK.</summary>
    [Description("""Cannot modify or delete an object that was not added using the COM+ Administrative SDK.""")]
    [HResultConstant(0x8004020D)]
    public static HResult EVENT_E_CANT_MODIFY_OR_DELETE_UNCONFIGURED_OBJECT => new(0x8004020D);

    /// <summary>Cannot modify or delete an object that was added using the COM+ Administrative SDK.</summary>
    [Description("""Cannot modify or delete an object that was added using the COM+ Administrative SDK.""")]
    [HResultConstant(0x8004020E)]
    public static HResult EVENT_E_CANT_MODIFY_OR_DELETE_CONFIGURED_OBJECT => new(0x8004020E);

    /// <summary>The event class for this subscription is in an invalid partition.</summary>
    [Description("""The event class for this subscription is in an invalid partition.""")]
    [HResultConstant(0x8004020F)]
    public static HResult EVENT_E_INVALID_EVENT_CLASS_PARTITION => new(0x8004020F);

    /// <summary>The owner of the PerUser subscription is not logged on to the system specified.</summary>
    [Description("""The owner of the PerUser subscription is not logged on to the system specified.""")]
    [HResultConstant(0x80040210)]
    public static HResult EVENT_E_PER_USER_SID_NOT_LOGGED_ON => new(0x80040210);

    /// <summary>Trigger not found.</summary>
    [Description("""Trigger not found.""")]
    [HResultConstant(0x80041309)]
    public static HResult SCHED_E_TRIGGER_NOT_FOUND => new(0x80041309);

    /// <summary>One or more of the properties that are needed to run this task have not been set.</summary>
    [Description("""One or more of the properties that are needed to run this task have not been set.""")]
    [HResultConstant(0x8004130A)]
    public static HResult SCHED_E_TASK_NOT_READY => new(0x8004130A);

    /// <summary>There is no running instance of the task.</summary>
    [Description("""There is no running instance of the task.""")]
    [HResultConstant(0x8004130B)]
    public static HResult SCHED_E_TASK_NOT_RUNNING => new(0x8004130B);

    /// <summary>The Task Scheduler service is not installed on this computer.</summary>
    [Description("""The Task Scheduler service is not installed on this computer.""")]
    [HResultConstant(0x8004130C)]
    public static HResult SCHED_E_SERVICE_NOT_INSTALLED => new(0x8004130C);

    /// <summary>The task object could not be opened.</summary>
    [Description("""The task object could not be opened.""")]
    [HResultConstant(0x8004130D)]
    public static HResult SCHED_E_CANNOT_OPEN_TASK => new(0x8004130D);

    /// <summary>The object is either an invalid task object or is not a task object.</summary>
    [Description("""The object is either an invalid task object or is not a task object.""")]
    [HResultConstant(0x8004130E)]
    public static HResult SCHED_E_INVALID_TASK => new(0x8004130E);

    /// <summary>No account information could be found in the Task Scheduler security database for the task indicated.</summary>
    [Description("""No account information could be found in the Task Scheduler security database for the task indicated.""")]
    [HResultConstant(0x8004130F)]
    public static HResult SCHED_E_ACCOUNT_INFORMATION_NOT_SET => new(0x8004130F);

    /// <summary>Unable to establish existence of the account specified.</summary>
    [Description("""Unable to establish existence of the account specified.""")]
    [HResultConstant(0x80041310)]
    public static HResult SCHED_E_ACCOUNT_NAME_NOT_FOUND => new(0x80041310);

    /// <summary>Corruption was detected in the Task Scheduler security database; the database has been reset.</summary>
    [Description("""Corruption was detected in the Task Scheduler security database; the database has been reset.""")]
    [HResultConstant(0x80041311)]
    public static HResult SCHED_E_ACCOUNT_DBASE_CORRUPT => new(0x80041311);

    /// <summary>Task Scheduler security services are available only on Windows NT operating system.</summary>
    [Description("""Task Scheduler security services are available only on Windows NT operating system.""")]
    [HResultConstant(0x80041312)]
    public static HResult SCHED_E_NO_SECURITY_SERVICES => new(0x80041312);

    /// <summary>The task object version is either unsupported or invalid.</summary>
    [Description("""The task object version is either unsupported or invalid.""")]
    [HResultConstant(0x80041313)]
    public static HResult SCHED_E_UNKNOWN_OBJECT_VERSION => new(0x80041313);

    /// <summary>The task has been configured with an unsupported combination of account settings and run-time options.</summary>
    [Description("""The task has been configured with an unsupported combination of account settings and run-time options.""")]
    [HResultConstant(0x80041314)]
    public static HResult SCHED_E_UNSUPPORTED_ACCOUNT_OPTION => new(0x80041314);

    /// <summary>The Task Scheduler service is not running.</summary>
    [Description("""The Task Scheduler service is not running.""")]
    [HResultConstant(0x80041315)]
    public static HResult SCHED_E_SERVICE_NOT_RUNNING => new(0x80041315);

    /// <summary>The task XML contains an unexpected node.</summary>
    [Description("""The task XML contains an unexpected node.""")]
    [HResultConstant(0x80041316)]
    public static HResult SCHED_E_UNEXPECTEDNODE => new(0x80041316);

    /// <summary>The task XML contains an element or attribute from an unexpected namespace.</summary>
    [Description("""The task XML contains an element or attribute from an unexpected namespace.""")]
    [HResultConstant(0x80041317)]
    public static HResult SCHED_E_NAMESPACE => new(0x80041317);

    /// <summary>The task XML contains a value that is incorrectly formatted or out of range.</summary>
    [Description("""The task XML contains a value that is incorrectly formatted or out of range.""")]
    [HResultConstant(0x80041318)]
    public static HResult SCHED_E_INVALIDVALUE => new(0x80041318);

    /// <summary>The task XML is missing a required element or attribute.</summary>
    [Description("""The task XML is missing a required element or attribute.""")]
    [HResultConstant(0x80041319)]
    public static HResult SCHED_E_MISSINGNODE => new(0x80041319);

    /// <summary>The task XML is malformed.</summary>
    [Description("""The task XML is malformed.""")]
    [HResultConstant(0x8004131A)]
    public static HResult SCHED_E_MALFORMEDXML => new(0x8004131A);

    /// <summary>The task XML contains too many nodes of the same type.</summary>
    [Description("""The task XML contains too many nodes of the same type.""")]
    [HResultConstant(0x8004131D)]
    public static HResult SCHED_E_TOO_MANY_NODES => new(0x8004131D);

    /// <summary>The task cannot be started after the trigger's end boundary.</summary>
    [Description("""The task cannot be started after the trigger's end boundary.""")]
    [HResultConstant(0x8004131E)]
    public static HResult SCHED_E_PAST_END_BOUNDARY => new(0x8004131E);

    /// <summary>An instance of this task is already running.</summary>
    [Description("""An instance of this task is already running.""")]
    [HResultConstant(0x8004131F)]
    public static HResult SCHED_E_ALREADY_RUNNING => new(0x8004131F);

    /// <summary>The task will not run because the user is not logged on.</summary>
    [Description("""The task will not run because the user is not logged on.""")]
    [HResultConstant(0x80041320)]
    public static HResult SCHED_E_USER_NOT_LOGGED_ON => new(0x80041320);

    /// <summary>The task image is corrupt or has been tampered with.</summary>
    [Description("""The task image is corrupt or has been tampered with.""")]
    [HResultConstant(0x80041321)]
    public static HResult SCHED_E_INVALID_TASK_HASH => new(0x80041321);

    /// <summary>The Task Scheduler service is not available.</summary>
    [Description("""The Task Scheduler service is not available.""")]
    [HResultConstant(0x80041322)]
    public static HResult SCHED_E_SERVICE_NOT_AVAILABLE => new(0x80041322);

    /// <summary>The Task Scheduler service is too busy to handle your request. Try again later.</summary>
    [Description("""The Task Scheduler service is too busy to handle your request. Try again later.""")]
    [HResultConstant(0x80041323)]
    public static HResult SCHED_E_SERVICE_TOO_BUSY => new(0x80041323);

    /// <summary>The Task Scheduler service attempted to run the task, but the task did not run due to one of the constraints in the task definition.</summary>
    [Description("""The Task Scheduler service attempted to run the task, but the task did not run due to one of the constraints in the task definition.""")]
    [HResultConstant(0x80041324)]
    public static HResult SCHED_E_TASK_ATTEMPTED => new(0x80041324);

    /// <summary>Another single phase resource manager has already been enlisted in this transaction.</summary>
    [Description("""Another single phase resource manager has already been enlisted in this transaction.""")]
    [HResultConstant(0x8004D000)]
    public static HResult XACT_E_ALREADYOTHERSINGLEPHASE => new(0x8004D000);

    /// <summary>A retaining commit or abort is not supported.</summary>
    [Description("""A retaining commit or abort is not supported.""")]
    [HResultConstant(0x8004D001)]
    public static HResult XACT_E_CANTRETAIN => new(0x8004D001);

    /// <summary>The transaction failed to commit for an unknown reason. The transaction was aborted.</summary>
    [Description("""The transaction failed to commit for an unknown reason. The transaction was aborted.""")]
    [HResultConstant(0x8004D002)]
    public static HResult XACT_E_COMMITFAILED => new(0x8004D002);

    /// <summary>Cannot call commit on this transaction object because the calling application did not initiate the transaction.</summary>
    [Description("""Cannot call commit on this transaction object because the calling application did not initiate the transaction.""")]
    [HResultConstant(0x8004D003)]
    public static HResult XACT_E_COMMITPREVENTED => new(0x8004D003);

    /// <summary>Instead of committing, the resource heuristically aborted.</summary>
    [Description("""Instead of committing, the resource heuristically aborted.""")]
    [HResultConstant(0x8004D004)]
    public static HResult XACT_E_HEURISTICABORT => new(0x8004D004);

    /// <summary>Instead of aborting, the resource heuristically committed.</summary>
    [Description("""Instead of aborting, the resource heuristically committed.""")]
    [HResultConstant(0x8004D005)]
    public static HResult XACT_E_HEURISTICCOMMIT => new(0x8004D005);

    /// <summary>Some of the states of the resource were committed while others were aborted, likely because of heuristic decisions.</summary>
    [Description("""Some of the states of the resource were committed while others were aborted, likely because of heuristic decisions.""")]
    [HResultConstant(0x8004D006)]
    public static HResult XACT_E_HEURISTICDAMAGE => new(0x8004D006);

    /// <summary>Some of the states of the resource might have been committed while others were aborted, likely because of heuristic decisions.</summary>
    [Description("""Some of the states of the resource might have been committed while others were aborted, likely because of heuristic decisions.""")]
    [HResultConstant(0x8004D007)]
    public static HResult XACT_E_HEURISTICDANGER => new(0x8004D007);

    /// <summary>The requested isolation level is not valid or supported.</summary>
    [Description("""The requested isolation level is not valid or supported.""")]
    [HResultConstant(0x8004D008)]
    public static HResult XACT_E_ISOLATIONLEVEL => new(0x8004D008);

    /// <summary>The transaction manager does not support an asynchronous operation for this method.</summary>
    [Description("""The transaction manager does not support an asynchronous operation for this method.""")]
    [HResultConstant(0x8004D009)]
    public static HResult XACT_E_NOASYNC => new(0x8004D009);

    /// <summary>Unable to enlist in the transaction.</summary>
    [Description("""Unable to enlist in the transaction.""")]
    [HResultConstant(0x8004D00A)]
    public static HResult XACT_E_NOENLIST => new(0x8004D00A);

    /// <summary>The requested semantics of retention of isolation across retaining commit and abort boundaries cannot be supported by this transaction implementation, or isoFlags was not equal to 0.</summary>
    [Description("""The requested semantics of retention of isolation across retaining commit and abort boundaries cannot be supported by this transaction implementation, or isoFlags was not equal to 0.""")]
    [HResultConstant(0x8004D00B)]
    public static HResult XACT_E_NOISORETAIN => new(0x8004D00B);

    /// <summary>There is no resource presently associated with this enlistment.</summary>
    [Description("""There is no resource presently associated with this enlistment.""")]
    [HResultConstant(0x8004D00C)]
    public static HResult XACT_E_NORESOURCE => new(0x8004D00C);

    /// <summary>The transaction failed to commit due to the failure of optimistic concurrency control in at least one of the resource managers.</summary>
    [Description("""The transaction failed to commit due to the failure of optimistic concurrency control in at least one of the resource managers.""")]
    [HResultConstant(0x8004D00D)]
    public static HResult XACT_E_NOTCURRENT => new(0x8004D00D);

    /// <summary>The transaction has already been implicitly or explicitly committed or aborted.</summary>
    [Description("""The transaction has already been implicitly or explicitly committed or aborted.""")]
    [HResultConstant(0x8004D00E)]
    public static HResult XACT_E_NOTRANSACTION => new(0x8004D00E);

    /// <summary>An invalid combination of flags was specified.</summary>
    [Description("""An invalid combination of flags was specified.""")]
    [HResultConstant(0x8004D00F)]
    public static HResult XACT_E_NOTSUPPORTED => new(0x8004D00F);

    /// <summary>The resource manager ID is not associated with this transaction or the transaction manager.</summary>
    [Description("""The resource manager ID is not associated with this transaction or the transaction manager.""")]
    [HResultConstant(0x8004D010)]
    public static HResult XACT_E_UNKNOWNRMGRID => new(0x8004D010);

    /// <summary>This method was called in the wrong state.</summary>
    [Description("""This method was called in the wrong state.""")]
    [HResultConstant(0x8004D011)]
    public static HResult XACT_E_WRONGSTATE => new(0x8004D011);

    /// <summary>The indicated unit of work does not match the unit of work expected by the resource manager.</summary>
    [Description("""The indicated unit of work does not match the unit of work expected by the resource manager.""")]
    [HResultConstant(0x8004D012)]
    public static HResult XACT_E_WRONGUOW => new(0x8004D012);

    /// <summary>An enlistment in a transaction already exists.</summary>
    [Description("""An enlistment in a transaction already exists.""")]
    [HResultConstant(0x8004D013)]
    public static HResult XACT_E_XTIONEXISTS => new(0x8004D013);

    /// <summary>An import object for the transaction could not be found.</summary>
    [Description("""An import object for the transaction could not be found.""")]
    [HResultConstant(0x8004D014)]
    public static HResult XACT_E_NOIMPORTOBJECT => new(0x8004D014);

    /// <summary>The transaction cookie is invalid.</summary>
    [Description("""The transaction cookie is invalid.""")]
    [HResultConstant(0x8004D015)]
    public static HResult XACT_E_INVALIDCOOKIE => new(0x8004D015);

    /// <summary>The transaction status is in doubt. A communication failure occurred, or a transaction manager or resource manager has failed.</summary>
    [Description("""The transaction status is in doubt. A communication failure occurred, or a transaction manager or resource manager has failed.""")]
    [HResultConstant(0x8004D016)]
    public static HResult XACT_E_INDOUBT => new(0x8004D016);

    /// <summary>A time-out was specified, but time-outs are not supported.</summary>
    [Description("""A time-out was specified, but time-outs are not supported.""")]
    [HResultConstant(0x8004D017)]
    public static HResult XACT_E_NOTIMEOUT => new(0x8004D017);

    /// <summary>The requested operation is already in progress for the transaction.</summary>
    [Description("""The requested operation is already in progress for the transaction.""")]
    [HResultConstant(0x8004D018)]
    public static HResult XACT_E_ALREADYINPROGRESS => new(0x8004D018);

    /// <summary>The transaction has already been aborted.</summary>
    [Description("""The transaction has already been aborted.""")]
    [HResultConstant(0x8004D019)]
    public static HResult XACT_E_ABORTED => new(0x8004D019);

    /// <summary>The Transaction Manager returned a log full error.</summary>
    [Description("""The Transaction Manager returned a log full error.""")]
    [HResultConstant(0x8004D01A)]
    public static HResult XACT_E_LOGFULL => new(0x8004D01A);

    /// <summary>The transaction manager is not available.</summary>
    [Description("""The transaction manager is not available.""")]
    [HResultConstant(0x8004D01B)]
    public static HResult XACT_E_TMNOTAVAILABLE => new(0x8004D01B);

    /// <summary>A connection with the transaction manager was lost.</summary>
    [Description("""A connection with the transaction manager was lost.""")]
    [HResultConstant(0x8004D01C)]
    public static HResult XACT_E_CONNECTION_DOWN => new(0x8004D01C);

    /// <summary>A request to establish a connection with the transaction manager was denied.</summary>
    [Description("""A request to establish a connection with the transaction manager was denied.""")]
    [HResultConstant(0x8004D01D)]
    public static HResult XACT_E_CONNECTION_DENIED => new(0x8004D01D);

    /// <summary>Resource manager reenlistment to determine transaction status timed out.</summary>
    [Description("""Resource manager reenlistment to determine transaction status timed out.""")]
    [HResultConstant(0x8004D01E)]
    public static HResult XACT_E_REENLISTTIMEOUT => new(0x8004D01E);

    /// <summary>The transaction manager failed to establish a connection with another Transaction Internet Protocol (TIP) transaction manager.</summary>
    [Description("""The transaction manager failed to establish a connection with another Transaction Internet Protocol (TIP) transaction manager.""")]
    [HResultConstant(0x8004D01F)]
    public static HResult XACT_E_TIP_CONNECT_FAILED => new(0x8004D01F);

    /// <summary>The transaction manager encountered a protocol error with another TIP transaction manager.</summary>
    [Description("""The transaction manager encountered a protocol error with another TIP transaction manager.""")]
    [HResultConstant(0x8004D020)]
    public static HResult XACT_E_TIP_PROTOCOL_ERROR => new(0x8004D020);

    /// <summary>The transaction manager could not propagate a transaction from another TIP transaction manager.</summary>
    [Description("""The transaction manager could not propagate a transaction from another TIP transaction manager.""")]
    [HResultConstant(0x8004D021)]
    public static HResult XACT_E_TIP_PULL_FAILED => new(0x8004D021);

    /// <summary>The transaction manager on the destination machine is not available.</summary>
    [Description("""The transaction manager on the destination machine is not available.""")]
    [HResultConstant(0x8004D022)]
    public static HResult XACT_E_DEST_TMNOTAVAILABLE => new(0x8004D022);

    /// <summary>The transaction manager has disabled its support for TIP.</summary>
    [Description("""The transaction manager has disabled its support for TIP.""")]
    [HResultConstant(0x8004D023)]
    public static HResult XACT_E_TIP_DISABLED => new(0x8004D023);

    /// <summary>The transaction manager has disabled its support for remote or network transactions.</summary>
    [Description("""The transaction manager has disabled its support for remote or network transactions.""")]
    [HResultConstant(0x8004D024)]
    public static HResult XACT_E_NETWORK_TX_DISABLED => new(0x8004D024);

    /// <summary>The partner transaction manager has disabled its support for remote or network transactions.</summary>
    [Description("""The partner transaction manager has disabled its support for remote or network transactions.""")]
    [HResultConstant(0x8004D025)]
    public static HResult XACT_E_PARTNER_NETWORK_TX_DISABLED => new(0x8004D025);

    /// <summary>The transaction manager has disabled its support for XA transactions.</summary>
    [Description("""The transaction manager has disabled its support for XA transactions.""")]
    [HResultConstant(0x8004D026)]
    public static HResult XACT_E_XA_TX_DISABLED => new(0x8004D026);

    /// <summary>Microsoft Distributed Transaction Coordinator (MSDTC) was unable to read its configuration information.</summary>
    [Description("""Microsoft Distributed Transaction Coordinator (MSDTC) was unable to read its configuration information.""")]
    [HResultConstant(0x8004D027)]
    public static HResult XACT_E_UNABLE_TO_READ_DTC_CONFIG => new(0x8004D027);

    /// <summary>MSDTC was unable to load the DTC proxy DLL.</summary>
    [Description("""MSDTC was unable to load the DTC proxy DLL.""")]
    [HResultConstant(0x8004D028)]
    public static HResult XACT_E_UNABLE_TO_LOAD_DTC_PROXY => new(0x8004D028);

    /// <summary>The local transaction has aborted.</summary>
    [Description("""The local transaction has aborted.""")]
    [HResultConstant(0x8004D029)]
    public static HResult XACT_E_ABORTING => new(0x8004D029);

    /// <summary>The specified CRM clerk was not found. It might have completed before it could be held.</summary>
    [Description("""The specified CRM clerk was not found. It might have completed before it could be held.""")]
    [HResultConstant(0x8004D080)]
    public static HResult XACT_E_CLERKNOTFOUND => new(0x8004D080);

    /// <summary>The specified CRM clerk does not exist.</summary>
    [Description("""The specified CRM clerk does not exist.""")]
    [HResultConstant(0x8004D081)]
    public static HResult XACT_E_CLERKEXISTS => new(0x8004D081);

    /// <summary>Recovery of the CRM log file is still in progress.</summary>
    [Description("""Recovery of the CRM log file is still in progress.""")]
    [HResultConstant(0x8004D082)]
    public static HResult XACT_E_RECOVERYINPROGRESS => new(0x8004D082);

    /// <summary>The transaction has completed, and the log records have been discarded from the log file. They are no longer available.</summary>
    [Description("""The transaction has completed, and the log records have been discarded from the log file. They are no longer available.""")]
    [HResultConstant(0x8004D083)]
    public static HResult XACT_E_TRANSACTIONCLOSED => new(0x8004D083);

    /// <summary>lsnToRead is outside of the current limits of the log</summary>
    [Description("""lsnToRead is outside of the current limits of the log""")]
    [HResultConstant(0x8004D084)]
    public static HResult XACT_E_INVALIDLSN => new(0x8004D084);

    /// <summary>The COM+ Compensating Resource Manager has records it wishes to replay.</summary>
    [Description("""The COM+ Compensating Resource Manager has records it wishes to replay.""")]
    [HResultConstant(0x8004D085)]
    public static HResult XACT_E_REPLAYREQUEST => new(0x8004D085);

    /// <summary>The request to connect to the specified transaction coordinator was denied.</summary>
    [Description("""The request to connect to the specified transaction coordinator was denied.""")]
    [HResultConstant(0x8004D100)]
    public static HResult XACT_E_CONNECTION_REQUEST_DENIED => new(0x8004D100);

    /// <summary>The maximum number of enlistments for the specified transaction has been reached.</summary>
    [Description("""The maximum number of enlistments for the specified transaction has been reached.""")]
    [HResultConstant(0x8004D101)]
    public static HResult XACT_E_TOOMANY_ENLISTMENTS => new(0x8004D101);

    /// <summary>A resource manager with the same identifier is already registered with the specified transaction coordinator.</summary>
    [Description("""A resource manager with the same identifier is already registered with the specified transaction coordinator.""")]
    [HResultConstant(0x8004D102)]
    public static HResult XACT_E_DUPLICATE_GUID => new(0x8004D102);

    /// <summary>The prepare request given was not eligible for single-phase optimizations.</summary>
    [Description("""The prepare request given was not eligible for single-phase optimizations.""")]
    [HResultConstant(0x8004D103)]
    public static HResult XACT_E_NOTSINGLEPHASE => new(0x8004D103);

    /// <summary>RecoveryComplete has already been called for the given resource manager.</summary>
    [Description("""RecoveryComplete has already been called for the given resource manager.""")]
    [HResultConstant(0x8004D104)]
    public static HResult XACT_E_RECOVERYALREADYDONE => new(0x8004D104);

    /// <summary>The interface call made was incorrect for the current state of the protocol.</summary>
    [Description("""The interface call made was incorrect for the current state of the protocol.""")]
    [HResultConstant(0x8004D105)]
    public static HResult XACT_E_PROTOCOL => new(0x8004D105);

    /// <summary>The xa_open call failed for the XA resource.</summary>
    [Description("""The xa_open call failed for the XA resource.""")]
    [HResultConstant(0x8004D106)]
    public static HResult XACT_E_RM_FAILURE => new(0x8004D106);

    /// <summary>The xa_recover call failed for the XA resource.</summary>
    [Description("""The xa_recover call failed for the XA resource.""")]
    [HResultConstant(0x8004D107)]
    public static HResult XACT_E_RECOVERY_FAILED => new(0x8004D107);

    /// <summary>The logical unit of work specified cannot be found.</summary>
    [Description("""The logical unit of work specified cannot be found.""")]
    [HResultConstant(0x8004D108)]
    public static HResult XACT_E_LU_NOT_FOUND => new(0x8004D108);

    /// <summary>The specified logical unit of work already exists.</summary>
    [Description("""The specified logical unit of work already exists.""")]
    [HResultConstant(0x8004D109)]
    public static HResult XACT_E_DUPLICATE_LU => new(0x8004D109);

    /// <summary>Subordinate creation failed. The specified logical unit of work was not connected.</summary>
    [Description("""Subordinate creation failed. The specified logical unit of work was not connected.""")]
    [HResultConstant(0x8004D10A)]
    public static HResult XACT_E_LU_NOT_CONNECTED => new(0x8004D10A);

    /// <summary>A transaction with the given identifier already exists.</summary>
    [Description("""A transaction with the given identifier already exists.""")]
    [HResultConstant(0x8004D10B)]
    public static HResult XACT_E_DUPLICATE_TRANSID => new(0x8004D10B);

    /// <summary>The resource is in use.</summary>
    [Description("""The resource is in use.""")]
    [HResultConstant(0x8004D10C)]
    public static HResult XACT_E_LU_BUSY => new(0x8004D10C);

    /// <summary>The LU Recovery process is down.</summary>
    [Description("""The LU Recovery process is down.""")]
    [HResultConstant(0x8004D10D)]
    public static HResult XACT_E_LU_NO_RECOVERY_PROCESS => new(0x8004D10D);

    /// <summary>The remote session was lost.</summary>
    [Description("""The remote session was lost.""")]
    [HResultConstant(0x8004D10E)]
    public static HResult XACT_E_LU_DOWN => new(0x8004D10E);

    /// <summary>The resource is currently recovering.</summary>
    [Description("""The resource is currently recovering.""")]
    [HResultConstant(0x8004D10F)]
    public static HResult XACT_E_LU_RECOVERING => new(0x8004D10F);

    /// <summary>There was a mismatch in driving recovery.</summary>
    [Description("""There was a mismatch in driving recovery.""")]
    [HResultConstant(0x8004D110)]
    public static HResult XACT_E_LU_RECOVERY_MISMATCH => new(0x8004D110);

    /// <summary>An error occurred with the XA resource.</summary>
    [Description("""An error occurred with the XA resource.""")]
    [HResultConstant(0x8004D111)]
    public static HResult XACT_E_RM_UNAVAILABLE => new(0x8004D111);

    /// <summary>The root transaction wanted to commit, but the transaction aborted.</summary>
    [Description("""The root transaction wanted to commit, but the transaction aborted.""")]
    [HResultConstant(0x8004E002)]
    public static HResult CONTEXT_E_ABORTED => new(0x8004E002);

    /// <summary>The COM+ component on which the method call was made has a transaction that has already aborted or is in the process of aborting.</summary>
    [Description("""The COM+ component on which the method call was made has a transaction that has already aborted or is in the process of aborting.""")]
    [HResultConstant(0x8004E003)]
    public static HResult CONTEXT_E_ABORTING => new(0x8004E003);

    /// <summary>There is no Microsoft Transaction Server (MTS) object context.</summary>
    [Description("""There is no Microsoft Transaction Server (MTS) object context.""")]
    [HResultConstant(0x8004E004)]
    public static HResult CONTEXT_E_NOCONTEXT => new(0x8004E004);

    /// <summary>The component is configured to use synchronization, and this method call would cause a deadlock to occur.</summary>
    [Description("""The component is configured to use synchronization, and this method call would cause a deadlock to occur.""")]
    [HResultConstant(0x8004E005)]
    public static HResult CONTEXT_E_WOULD_DEADLOCK => new(0x8004E005);

    /// <summary>The component is configured to use synchronization, and a thread has timed out waiting to enter the context.</summary>
    [Description("""The component is configured to use synchronization, and a thread has timed out waiting to enter the context.""")]
    [HResultConstant(0x8004E006)]
    public static HResult CONTEXT_E_SYNCH_TIMEOUT => new(0x8004E006);

    /// <summary>You made a method call on a COM+ component that has a transaction that has already committed or aborted.</summary>
    [Description("""You made a method call on a COM+ component that has a transaction that has already committed or aborted.""")]
    [HResultConstant(0x8004E007)]
    public static HResult CONTEXT_E_OLDREF => new(0x8004E007);

    /// <summary>The specified role was not configured for the application.</summary>
    [Description("""The specified role was not configured for the application.""")]
    [HResultConstant(0x8004E00C)]
    public static HResult CONTEXT_E_ROLENOTFOUND => new(0x8004E00C);

    /// <summary>COM+ was unable to talk to the MSDTC.</summary>
    [Description("""COM+ was unable to talk to the MSDTC.""")]
    [HResultConstant(0x8004E00F)]
    public static HResult CONTEXT_E_TMNOTAVAILABLE => new(0x8004E00F);

    /// <summary>An unexpected error occurred during COM+ activation.</summary>
    [Description("""An unexpected error occurred during COM+ activation.""")]
    [HResultConstant(0x8004E021)]
    public static HResult CO_E_ACTIVATIONFAILED => new(0x8004E021);

    /// <summary>COM+ activation failed. Check the event log for more information.</summary>
    [Description("""COM+ activation failed. Check the event log for more information.""")]
    [HResultConstant(0x8004E022)]
    public static HResult CO_E_ACTIVATIONFAILED_EVENTLOGGED => new(0x8004E022);

    /// <summary>COM+ activation failed due to a catalog or configuration error.</summary>
    [Description("""COM+ activation failed due to a catalog or configuration error.""")]
    [HResultConstant(0x8004E023)]
    public static HResult CO_E_ACTIVATIONFAILED_CATALOGERROR => new(0x8004E023);

    /// <summary>COM+ activation failed because the activation could not be completed in the specified amount of time.</summary>
    [Description("""COM+ activation failed because the activation could not be completed in the specified amount of time.""")]
    [HResultConstant(0x8004E024)]
    public static HResult CO_E_ACTIVATIONFAILED_TIMEOUT => new(0x8004E024);

    /// <summary>COM+ activation failed because an initialization function failed. Check the event log for more information.</summary>
    [Description("""COM+ activation failed because an initialization function failed. Check the event log for more information.""")]
    [HResultConstant(0x8004E025)]
    public static HResult CO_E_INITIALIZATIONFAILED => new(0x8004E025);

    /// <summary>The requested operation requires that just-in-time (JIT) be in the current context, and it is not.</summary>
    [Description("""The requested operation requires that just-in-time (JIT) be in the current context, and it is not.""")]
    [HResultConstant(0x8004E026)]
    public static HResult CONTEXT_E_NOJIT => new(0x8004E026);

    /// <summary>The requested operation requires that the current context have a transaction, and it does not.</summary>
    [Description("""The requested operation requires that the current context have a transaction, and it does not.""")]
    [HResultConstant(0x8004E027)]
    public static HResult CONTEXT_E_NOTRANSACTION => new(0x8004E027);

    /// <summary>The components threading model has changed after install into a COM+ application. Re-install component.</summary>
    [Description("""The components threading model has changed after install into a COM+ application. Re-install component.""")]
    [HResultConstant(0x8004E028)]
    public static HResult CO_E_THREADINGMODEL_CHANGED => new(0x8004E028);

    /// <summary>Internet Information Services (IIS) intrinsics not available. Start your work with IIS.</summary>
    [Description("""Internet Information Services (IIS) intrinsics not available. Start your work with IIS.""")]
    [HResultConstant(0x8004E029)]
    public static HResult CO_E_NOIISINTRINSICS => new(0x8004E029);

    /// <summary>An attempt to write a cookie failed.</summary>
    [Description("""An attempt to write a cookie failed.""")]
    [HResultConstant(0x8004E02A)]
    public static HResult CO_E_NOCOOKIES => new(0x8004E02A);

    /// <summary>An attempt to use a database generated a database-specific error.</summary>
    [Description("""An attempt to use a database generated a database-specific error.""")]
    [HResultConstant(0x8004E02B)]
    public static HResult CO_E_DBERROR => new(0x8004E02B);

    /// <summary>The COM+ component you created must use object pooling to work.</summary>
    [Description("""The COM+ component you created must use object pooling to work.""")]
    [HResultConstant(0x8004E02C)]
    public static HResult CO_E_NOTPOOLED => new(0x8004E02C);

    /// <summary>The COM+ component you created must use object construction to work correctly.</summary>
    [Description("""The COM+ component you created must use object construction to work correctly.""")]
    [HResultConstant(0x8004E02D)]
    public static HResult CO_E_NOTCONSTRUCTED => new(0x8004E02D);

    /// <summary>The COM+ component requires synchronization, and it is not configured for it.</summary>
    [Description("""The COM+ component requires synchronization, and it is not configured for it.""")]
    [HResultConstant(0x8004E02E)]
    public static HResult CO_E_NOSYNCHRONIZATION => new(0x8004E02E);

    /// <summary>The TxIsolation Level property for the COM+ component being created is stronger than the TxIsolationLevel for the root.</summary>
    [Description("""The TxIsolation Level property for the COM+ component being created is stronger than the TxIsolationLevel for the root.""")]
    [HResultConstant(0x8004E02F)]
    public static HResult CO_E_ISOLEVELMISMATCH => new(0x8004E02F);

    /// <summary>The component attempted to make a cross-context call between invocations of EnterTransactionScope and ExitTransactionScope. This is not allowed. Cross-context calls cannot be made while inside a transaction scope.</summary>
    [Description("""The component attempted to make a cross-context call between invocations of EnterTransactionScope and ExitTransactionScope. This is not allowed. Cross-context calls cannot be made while inside a transaction scope.""")]
    [HResultConstant(0x8004E030)]
    public static HResult CO_E_CALL_OUT_OF_TX_SCOPE_NOT_ALLOWED => new(0x8004E030);

    /// <summary>The component made a call to EnterTransactionScope, but did not make a corresponding call to ExitTransactionScope before returning.</summary>
    [Description("""The component made a call to EnterTransactionScope, but did not make a corresponding call to ExitTransactionScope before returning.""")]
    [HResultConstant(0x8004E031)]
    public static HResult CO_E_EXIT_TRANSACTION_SCOPE_NOT_CALLED => new(0x8004E031);

    /// <summary>The server cannot support a client request for a dynamic virtual channel.</summary>
    [Description("""The server cannot support a client request for a dynamic virtual channel.""")]
    [HResultConstant(0x80070032)]
    public static HResult ERROR_NOT_SUPPORTED => new(0x80070032);

    /// <summary>There is not enough space on the disk.</summary>
    [Description("""There is not enough space on the disk.""")]
    [HResultConstant(0x80070070)]
    public static HResult ERROR_DISK_FULL => new(0x80070070);

    /// <summary>Attempt to create a class object failed.</summary>
    [Description("""Attempt to create a class object failed.""")]
    [HResultConstant(0x80080001)]
    public static HResult CO_E_CLASS_CREATE_FAILED => new(0x80080001);

    /// <summary>OLE service could not bind object.</summary>
    [Description("""OLE service could not bind object.""")]
    [HResultConstant(0x80080002)]
    public static HResult CO_E_SCM_ERROR => new(0x80080002);

    /// <summary>RPC communication failed with OLE service.</summary>
    [Description("""RPC communication failed with OLE service.""")]
    [HResultConstant(0x80080003)]
    public static HResult CO_E_SCM_RPC_FAILURE => new(0x80080003);

    /// <summary>Bad path to object.</summary>
    [Description("""Bad path to object.""")]
    [HResultConstant(0x80080004)]
    public static HResult CO_E_BAD_PATH => new(0x80080004);

    /// <summary>Server execution failed.</summary>
    [Description("""Server execution failed.""")]
    [HResultConstant(0x80080005)]
    public static HResult CO_E_SERVER_EXEC_FAILURE => new(0x80080005);

    /// <summary>OLE service could not communicate with the object server.</summary>
    [Description("""OLE service could not communicate with the object server.""")]
    [HResultConstant(0x80080006)]
    public static HResult CO_E_OBJSRV_RPC_FAILURE => new(0x80080006);

    /// <summary>Moniker path could not be normalized.</summary>
    [Description("""Moniker path could not be normalized.""")]
    [HResultConstant(0x80080007)]
    public static HResult MK_E_NO_NORMALIZED => new(0x80080007);

    /// <summary>Object server is stopping when OLE service contacts it.</summary>
    [Description("""Object server is stopping when OLE service contacts it.""")]
    [HResultConstant(0x80080008)]
    public static HResult CO_E_SERVER_STOPPING => new(0x80080008);

    /// <summary>An invalid root block pointer was specified.</summary>
    [Description("""An invalid root block pointer was specified.""")]
    [HResultConstant(0x80080009)]
    public static HResult MEM_E_INVALID_ROOT => new(0x80080009);

    /// <summary>An allocation chain contained an invalid link pointer.</summary>
    [Description("""An allocation chain contained an invalid link pointer.""")]
    [HResultConstant(0x80080010)]
    public static HResult MEM_E_INVALID_LINK => new(0x80080010);

    /// <summary>The requested allocation size was too large.</summary>
    [Description("""The requested allocation size was too large.""")]
    [HResultConstant(0x80080011)]
    public static HResult MEM_E_INVALID_SIZE => new(0x80080011);

    /// <summary>The activation requires a display name to be present under the class identifier (CLSID) key.</summary>
    [Description("""The activation requires a display name to be present under the class identifier (CLSID) key.""")]
    [HResultConstant(0x80080015)]
    public static HResult CO_E_MISSING_DISPLAYNAME => new(0x80080015);

    /// <summary>The activation requires that the RunAs value for the application is Activate As Activator.</summary>
    [Description("""The activation requires that the RunAs value for the application is Activate As Activator.""")]
    [HResultConstant(0x80080016)]
    public static HResult CO_E_RUNAS_VALUE_MUST_BE_AAA => new(0x80080016);

    /// <summary>The class is not configured to support elevated activation.</summary>
    [Description("""The class is not configured to support elevated activation.""")]
    [HResultConstant(0x80080017)]
    public static HResult CO_E_ELEVATION_DISABLED => new(0x80080017);

    /// <summary>Bad UID.</summary>
    [Description("""Bad UID.""")]
    [HResultConstant(0x80090001)]
    public static HResult NTE_BAD_UID => new(0x80090001);

    /// <summary>Bad hash.</summary>
    [Description("""Bad hash.""")]
    [HResultConstant(0x80090002)]
    public static HResult NTE_BAD_HASH => new(0x80090002);

    /// <summary>Bad key.</summary>
    [Description("""Bad key.""")]
    [HResultConstant(0x80090003)]
    public static HResult NTE_BAD_KEY => new(0x80090003);

    /// <summary>Bad length.</summary>
    [Description("""Bad length.""")]
    [HResultConstant(0x80090004)]
    public static HResult NTE_BAD_LEN => new(0x80090004);

    /// <summary>Bad data.</summary>
    [Description("""Bad data.""")]
    [HResultConstant(0x80090005)]
    public static HResult NTE_BAD_DATA => new(0x80090005);

    /// <summary>Invalid signature.</summary>
    [Description("""Invalid signature.""")]
    [HResultConstant(0x80090006)]
    public static HResult NTE_BAD_SIGNATURE => new(0x80090006);

    /// <summary>Bad version of provider.</summary>
    [Description("""Bad version of provider.""")]
    [HResultConstant(0x80090007)]
    public static HResult NTE_BAD_VER => new(0x80090007);

    /// <summary>Invalid algorithm specified.</summary>
    [Description("""Invalid algorithm specified.""")]
    [HResultConstant(0x80090008)]
    public static HResult NTE_BAD_ALGID => new(0x80090008);

    /// <summary>Invalid flags specified.</summary>
    [Description("""Invalid flags specified.""")]
    [HResultConstant(0x80090009)]
    public static HResult NTE_BAD_FLAGS => new(0x80090009);

    /// <summary>Invalid type specified.</summary>
    [Description("""Invalid type specified.""")]
    [HResultConstant(0x8009000A)]
    public static HResult NTE_BAD_TYPE => new(0x8009000A);

    /// <summary>Key not valid for use in specified state.</summary>
    [Description("""Key not valid for use in specified state.""")]
    [HResultConstant(0x8009000B)]
    public static HResult NTE_BAD_KEY_STATE => new(0x8009000B);

    /// <summary>Hash not valid for use in specified state.</summary>
    [Description("""Hash not valid for use in specified state.""")]
    [HResultConstant(0x8009000C)]
    public static HResult NTE_BAD_HASH_STATE => new(0x8009000C);

    /// <summary>Key does not exist.</summary>
    [Description("""Key does not exist.""")]
    [HResultConstant(0x8009000D)]
    public static HResult NTE_NO_KEY => new(0x8009000D);

    /// <summary>Insufficient memory available for the operation.</summary>
    [Description("""Insufficient memory available for the operation.""")]
    [HResultConstant(0x8009000E)]
    public static HResult NTE_NO_MEMORY => new(0x8009000E);

    /// <summary>Object already exists.</summary>
    [Description("""Object already exists.""")]
    [HResultConstant(0x8009000F)]
    public static HResult NTE_EXISTS => new(0x8009000F);

    /// <summary>Access denied.</summary>
    [Description("""Access denied.""")]
    [HResultConstant(0x80090010)]
    public static HResult NTE_PERM => new(0x80090010);

    /// <summary>Object was not found.</summary>
    [Description("""Object was not found.""")]
    [HResultConstant(0x80090011)]
    public static HResult NTE_NOT_FOUND => new(0x80090011);

    /// <summary>Data already encrypted.</summary>
    [Description("""Data already encrypted.""")]
    [HResultConstant(0x80090012)]
    public static HResult NTE_DOUBLE_ENCRYPT => new(0x80090012);

    /// <summary>Invalid provider specified.</summary>
    [Description("""Invalid provider specified.""")]
    [HResultConstant(0x80090013)]
    public static HResult NTE_BAD_PROVIDER => new(0x80090013);

    /// <summary>Invalid provider type specified.</summary>
    [Description("""Invalid provider type specified.""")]
    [HResultConstant(0x80090014)]
    public static HResult NTE_BAD_PROV_TYPE => new(0x80090014);

    /// <summary>Provider's public key is invalid.</summary>
    [Description("""Provider's public key is invalid.""")]
    [HResultConstant(0x80090015)]
    public static HResult NTE_BAD_PUBLIC_KEY => new(0x80090015);

    /// <summary>Key set does not exist.</summary>
    [Description("""Key set does not exist.""")]
    [HResultConstant(0x80090016)]
    public static HResult NTE_BAD_KEYSET => new(0x80090016);

    /// <summary>Provider type not defined.</summary>
    [Description("""Provider type not defined.""")]
    [HResultConstant(0x80090017)]
    public static HResult NTE_PROV_TYPE_NOT_DEF => new(0x80090017);

    /// <summary>The provider type, as registered, is invalid.</summary>
    [Description("""The provider type, as registered, is invalid.""")]
    [HResultConstant(0x80090018)]
    public static HResult NTE_PROV_TYPE_ENTRY_BAD => new(0x80090018);

    /// <summary>The key set is not defined.</summary>
    [Description("""The key set is not defined.""")]
    [HResultConstant(0x80090019)]
    public static HResult NTE_KEYSET_NOT_DEF => new(0x80090019);

    /// <summary>The key set, as registered, is invalid.</summary>
    [Description("""The key set, as registered, is invalid.""")]
    [HResultConstant(0x8009001A)]
    public static HResult NTE_KEYSET_ENTRY_BAD => new(0x8009001A);

    /// <summary>Provider type does not match registered value.</summary>
    [Description("""Provider type does not match registered value.""")]
    [HResultConstant(0x8009001B)]
    public static HResult NTE_PROV_TYPE_NO_MATCH => new(0x8009001B);

    /// <summary>The digital signature file is corrupt.</summary>
    [Description("""The digital signature file is corrupt.""")]
    [HResultConstant(0x8009001C)]
    public static HResult NTE_SIGNATURE_FILE_BAD => new(0x8009001C);

    /// <summary>Provider DLL failed to initialize correctly.</summary>
    [Description("""Provider DLL failed to initialize correctly.""")]
    [HResultConstant(0x8009001D)]
    public static HResult NTE_PROVIDER_DLL_FAIL => new(0x8009001D);

    /// <summary>Provider DLL could not be found.</summary>
    [Description("""Provider DLL could not be found.""")]
    [HResultConstant(0x8009001E)]
    public static HResult NTE_PROV_DLL_NOT_FOUND => new(0x8009001E);

    /// <summary>The keyset parameter is invalid.</summary>
    [Description("""The keyset parameter is invalid.""")]
    [HResultConstant(0x8009001F)]
    public static HResult NTE_BAD_KEYSET_PARAM => new(0x8009001F);

    /// <summary>An internal error occurred.</summary>
    [Description("""An internal error occurred.""")]
    [HResultConstant(0x80090020)]
    public static HResult NTE_FAIL => new(0x80090020);

    /// <summary>A base error occurred.</summary>
    [Description("""A base error occurred.""")]
    [HResultConstant(0x80090021)]
    public static HResult NTE_SYS_ERR => new(0x80090021);

    /// <summary>Provider could not perform the action because the context was acquired as silent.</summary>
    [Description("""Provider could not perform the action because the context was acquired as silent.""")]
    [HResultConstant(0x80090022)]
    public static HResult NTE_SILENT_CONTEXT => new(0x80090022);

    /// <summary>The security token does not have storage space available for an additional container.</summary>
    [Description("""The security token does not have storage space available for an additional container.""")]
    [HResultConstant(0x80090023)]
    public static HResult NTE_TOKEN_KEYSET_STORAGE_FULL => new(0x80090023);

    /// <summary>The profile for the user is a temporary profile.</summary>
    [Description("""The profile for the user is a temporary profile.""")]
    [HResultConstant(0x80090024)]
    public static HResult NTE_TEMPORARY_PROFILE => new(0x80090024);

    /// <summary>The key parameters could not be set because the configuration service provider (CSP) uses fixed parameters.</summary>
    [Description("""The key parameters could not be set because the configuration service provider (CSP) uses fixed parameters.""")]
    [HResultConstant(0x80090025)]
    public static HResult NTE_FIXEDPARAMETER => new(0x80090025);

    /// <summary>The supplied handle is invalid.</summary>
    [Description("""The supplied handle is invalid.""")]
    [HResultConstant(0x80090026)]
    public static HResult NTE_INVALID_HANDLE => new(0x80090026);

    /// <summary>The parameter is incorrect.</summary>
    [Description("""The parameter is incorrect.""")]
    [HResultConstant(0x80090027)]
    public static HResult NTE_INVALID_PARAMETER => new(0x80090027);

    /// <summary>The buffer supplied to a function was too small.</summary>
    [Description("""The buffer supplied to a function was too small.""")]
    [HResultConstant(0x80090028)]
    public static HResult NTE_BUFFER_TOO_SMALL => new(0x80090028);

    /// <summary>The requested operation is not supported.</summary>
    [Description("""The requested operation is not supported.""")]
    [HResultConstant(0x80090029)]
    public static HResult NTE_NOT_SUPPORTED => new(0x80090029);

    /// <summary>No more data is available.</summary>
    [Description("""No more data is available.""")]
    [HResultConstant(0x8009002A)]
    public static HResult NTE_NO_MORE_ITEMS => new(0x8009002A);

    /// <summary>The supplied buffers overlap incorrectly.</summary>
    [Description("""The supplied buffers overlap incorrectly.""")]
    [HResultConstant(0x8009002B)]
    public static HResult NTE_BUFFERS_OVERLAP => new(0x8009002B);

    /// <summary>The specified data could not be decrypted.</summary>
    [Description("""The specified data could not be decrypted.""")]
    [HResultConstant(0x8009002C)]
    public static HResult NTE_DECRYPTION_FAILURE => new(0x8009002C);

    /// <summary>An internal consistency check failed.</summary>
    [Description("""An internal consistency check failed.""")]
    [HResultConstant(0x8009002D)]
    public static HResult NTE_INTERNAL_ERROR => new(0x8009002D);

    /// <summary>This operation requires input from the user.</summary>
    [Description("""This operation requires input from the user.""")]
    [HResultConstant(0x8009002E)]
    public static HResult NTE_UI_REQUIRED => new(0x8009002E);

    /// <summary>The cryptographic provider does not support Hash Message Authentication Code (HMAC).</summary>
    [Description("""The cryptographic provider does not support Hash Message Authentication Code (HMAC).""")]
    [HResultConstant(0x8009002F)]
    public static HResult NTE_HMAC_NOT_SUPPORTED => new(0x8009002F);

    /// <summary>Not enough memory is available to complete this request.</summary>
    [Description("""Not enough memory is available to complete this request.""")]
    [HResultConstant(0x80090300)]
    public static HResult SEC_E_INSUFFICIENT_MEMORY => new(0x80090300);

    /// <summary>The handle specified is invalid.</summary>
    [Description("""The handle specified is invalid.""")]
    [HResultConstant(0x80090301)]
    public static HResult SEC_E_INVALID_HANDLE => new(0x80090301);

    /// <summary>The function requested is not supported.</summary>
    [Description("""The function requested is not supported.""")]
    [HResultConstant(0x80090302)]
    public static HResult SEC_E_UNSUPPORTED_FUNCTION => new(0x80090302);

    /// <summary>The specified target is unknown or unreachable.</summary>
    [Description("""The specified target is unknown or unreachable.""")]
    [HResultConstant(0x80090303)]
    public static HResult SEC_E_TARGET_UNKNOWN => new(0x80090303);

    /// <summary>The Local Security Authority (LSA) cannot be contacted.</summary>
    [Description("""The Local Security Authority (LSA) cannot be contacted.""")]
    [HResultConstant(0x80090304)]
    public static HResult SEC_E_INTERNAL_ERROR => new(0x80090304);

    /// <summary>The requested security package does not exist.</summary>
    [Description("""The requested security package does not exist.""")]
    [HResultConstant(0x80090305)]
    public static HResult SEC_E_SECPKG_NOT_FOUND => new(0x80090305);

    /// <summary>The caller is not the owner of the desired credentials.</summary>
    [Description("""The caller is not the owner of the desired credentials.""")]
    [HResultConstant(0x80090306)]
    public static HResult SEC_E_NOT_OWNER => new(0x80090306);

    /// <summary>The security package failed to initialize and cannot be installed.</summary>
    [Description("""The security package failed to initialize and cannot be installed.""")]
    [HResultConstant(0x80090307)]
    public static HResult SEC_E_CANNOT_INSTALL => new(0x80090307);

    /// <summary>The token supplied to the function is invalid.</summary>
    [Description("""The token supplied to the function is invalid.""")]
    [HResultConstant(0x80090308)]
    public static HResult SEC_E_INVALID_TOKEN => new(0x80090308);

    /// <summary>The security package is not able to marshal the logon buffer, so the logon attempt has failed.</summary>
    [Description("""The security package is not able to marshal the logon buffer, so the logon attempt has failed.""")]
    [HResultConstant(0x80090309)]
    public static HResult SEC_E_CANNOT_PACK => new(0x80090309);

    /// <summary>The per-message quality of protection is not supported by the security package.</summary>
    [Description("""The per-message quality of protection is not supported by the security package.""")]
    [HResultConstant(0x8009030A)]
    public static HResult SEC_E_QOP_NOT_SUPPORTED => new(0x8009030A);

    /// <summary>The security context does not allow impersonation of the client.</summary>
    [Description("""The security context does not allow impersonation of the client.""")]
    [HResultConstant(0x8009030B)]
    public static HResult SEC_E_NO_IMPERSONATION => new(0x8009030B);

    /// <summary>The logon attempt failed.</summary>
    [Description("""The logon attempt failed.""")]
    [HResultConstant(0x8009030C)]
    public static HResult SEC_E_LOGON_DENIED => new(0x8009030C);

    /// <summary>The credentials supplied to the package were not recognized.</summary>
    [Description("""The credentials supplied to the package were not recognized.""")]
    [HResultConstant(0x8009030D)]
    public static HResult SEC_E_UNKNOWN_CREDENTIALS => new(0x8009030D);

    /// <summary>No credentials are available in the security package.</summary>
    [Description("""No credentials are available in the security package.""")]
    [HResultConstant(0x8009030E)]
    public static HResult SEC_E_NO_CREDENTIALS => new(0x8009030E);

    /// <summary>The message or signature supplied for verification has been altered.</summary>
    [Description("""The message or signature supplied for verification has been altered.""")]
    [HResultConstant(0x8009030F)]
    public static HResult SEC_E_MESSAGE_ALTERED => new(0x8009030F);

    /// <summary>The message supplied for verification is out of sequence.</summary>
    [Description("""The message supplied for verification is out of sequence.""")]
    [HResultConstant(0x80090310)]
    public static HResult SEC_E_OUT_OF_SEQUENCE => new(0x80090310);

    /// <summary>No authority could be contacted for authentication.</summary>
    [Description("""No authority could be contacted for authentication.""")]
    [HResultConstant(0x80090311)]
    public static HResult SEC_E_NO_AUTHENTICATING_AUTHORITY => new(0x80090311);

    /// <summary>The requested security package does not exist.</summary>
    [Description("""The requested security package does not exist.""")]
    [HResultConstant(0x80090316)]
    public static HResult SEC_E_BAD_PKGID => new(0x80090316);

    /// <summary>The context has expired and can no longer be used.</summary>
    [Description("""The context has expired and can no longer be used.""")]
    [HResultConstant(0x80090317)]
    public static HResult SEC_E_CONTEXT_EXPIRED => new(0x80090317);

    /// <summary>The supplied message is incomplete. The signature was not verified.</summary>
    [Description("""The supplied message is incomplete. The signature was not verified.""")]
    [HResultConstant(0x80090318)]
    public static HResult SEC_E_INCOMPLETE_MESSAGE => new(0x80090318);

    /// <summary>The credentials supplied were not complete and could not be verified. The context could not be initialized.</summary>
    [Description("""The credentials supplied were not complete and could not be verified. The context could not be initialized.""")]
    [HResultConstant(0x80090320)]
    public static HResult SEC_E_INCOMPLETE_CREDENTIALS => new(0x80090320);

    /// <summary>The buffers supplied to a function was too small.</summary>
    [Description("""The buffers supplied to a function was too small.""")]
    [HResultConstant(0x80090321)]
    public static HResult SEC_E_BUFFER_TOO_SMALL => new(0x80090321);

    /// <summary>The target principal name is incorrect.</summary>
    [Description("""The target principal name is incorrect.""")]
    [HResultConstant(0x80090322)]
    public static HResult SEC_E_WRONG_PRINCIPAL => new(0x80090322);

    /// <summary>The clocks on the client and server machines are skewed.</summary>
    [Description("""The clocks on the client and server machines are skewed.""")]
    [HResultConstant(0x80090324)]
    public static HResult SEC_E_TIME_SKEW => new(0x80090324);

    /// <summary>The certificate chain was issued by an authority that is not trusted.</summary>
    [Description("""The certificate chain was issued by an authority that is not trusted.""")]
    [HResultConstant(0x80090325)]
    public static HResult SEC_E_UNTRUSTED_ROOT => new(0x80090325);

    /// <summary>The message received was unexpected or badly formatted.</summary>
    [Description("""The message received was unexpected or badly formatted.""")]
    [HResultConstant(0x80090326)]
    public static HResult SEC_E_ILLEGAL_MESSAGE => new(0x80090326);

    /// <summary>An unknown error occurred while processing the certificate.</summary>
    [Description("""An unknown error occurred while processing the certificate.""")]
    [HResultConstant(0x80090327)]
    public static HResult SEC_E_CERT_UNKNOWN => new(0x80090327);

    /// <summary>The received certificate has expired.</summary>
    [Description("""The received certificate has expired.""")]
    [HResultConstant(0x80090328)]
    public static HResult SEC_E_CERT_EXPIRED => new(0x80090328);

    /// <summary>The specified data could not be encrypted.</summary>
    [Description("""The specified data could not be encrypted.""")]
    [HResultConstant(0x80090329)]
    public static HResult SEC_E_ENCRYPT_FAILURE => new(0x80090329);

    /// <summary>The specified data could not be decrypted.</summary>
    [Description("""The specified data could not be decrypted.""")]
    [HResultConstant(0x80090330)]
    public static HResult SEC_E_DECRYPT_FAILURE => new(0x80090330);

    /// <summary>The client and server cannot communicate because they do not possess a common algorithm.</summary>
    [Description("""The client and server cannot communicate because they do not possess a common algorithm.""")]
    [HResultConstant(0x80090331)]
    public static HResult SEC_E_ALGORITHM_MISMATCH => new(0x80090331);

    /// <summary>The security context could not be established due to a failure in the requested quality of service (for example, mutual authentication or delegation).</summary>
    [Description("""The security context could not be established due to a failure in the requested quality of service (for example, mutual authentication or delegation).""")]
    [HResultConstant(0x80090332)]
    public static HResult SEC_E_SECURITY_QOS_FAILED => new(0x80090332);

    /// <summary>A security context was deleted before the context was completed. This is considered a logon failure.</summary>
    [Description("""A security context was deleted before the context was completed. This is considered a logon failure.""")]
    [HResultConstant(0x80090333)]
    public static HResult SEC_E_UNFINISHED_CONTEXT_DELETED => new(0x80090333);

    /// <summary>The client is trying to negotiate a context and the server requires user-to-user but did not send a ticket granting ticket (TGT) reply.</summary>
    [Description("""The client is trying to negotiate a context and the server requires user-to-user but did not send a ticket granting ticket (TGT) reply.""")]
    [HResultConstant(0x80090334)]
    public static HResult SEC_E_NO_TGT_REPLY => new(0x80090334);

    /// <summary>Unable to accomplish the requested task because the local machine does not have an IP addresses.</summary>
    [Description("""Unable to accomplish the requested task because the local machine does not have an IP addresses.""")]
    [HResultConstant(0x80090335)]
    public static HResult SEC_E_NO_IP_ADDRESSES => new(0x80090335);

    /// <summary>The supplied credential handle does not match the credential associated with the security context.</summary>
    [Description("""The supplied credential handle does not match the credential associated with the security context.""")]
    [HResultConstant(0x80090336)]
    public static HResult SEC_E_WRONG_CREDENTIAL_HANDLE => new(0x80090336);

    /// <summary>The cryptographic system or checksum function is invalid because a required function is unavailable.</summary>
    [Description("""The cryptographic system or checksum function is invalid because a required function is unavailable.""")]
    [HResultConstant(0x80090337)]
    public static HResult SEC_E_CRYPTO_SYSTEM_INVALID => new(0x80090337);

    /// <summary>The number of maximum ticket referrals has been exceeded.</summary>
    [Description("""The number of maximum ticket referrals has been exceeded.""")]
    [HResultConstant(0x80090338)]
    public static HResult SEC_E_MAX_REFERRALS_EXCEEDED => new(0x80090338);

    /// <summary>The local machine must be a Kerberos domain controller (KDC), and it is not.</summary>
    [Description("""The local machine must be a Kerberos domain controller (KDC), and it is not.""")]
    [HResultConstant(0x80090339)]
    public static HResult SEC_E_MUST_BE_KDC => new(0x80090339);

    /// <summary>The other end of the security negotiation requires strong cryptographics, but it is not supported on the local machine.</summary>
    [Description("""The other end of the security negotiation requires strong cryptographics, but it is not supported on the local machine.""")]
    [HResultConstant(0x8009033A)]
    public static HResult SEC_E_STRONG_CRYPTO_NOT_SUPPORTED => new(0x8009033A);

    /// <summary>The KDC reply contained more than one principal name.</summary>
    [Description("""The KDC reply contained more than one principal name.""")]
    [HResultConstant(0x8009033B)]
    public static HResult SEC_E_TOO_MANY_PRINCIPALS => new(0x8009033B);

    /// <summary>Expected to find PA data for a hint of what etype to use, but it was not found.</summary>
    [Description("""Expected to find PA data for a hint of what etype to use, but it was not found.""")]
    [HResultConstant(0x8009033C)]
    public static HResult SEC_E_NO_PA_DATA => new(0x8009033C);

    /// <summary>The client certificate does not contain a valid user principal name (UPN), or does not match the client name in the logon request. Contact your administrator.</summary>
    [Description("""The client certificate does not contain a valid user principal name (UPN), or does not match the client name in the logon request. Contact your administrator.""")]
    [HResultConstant(0x8009033D)]
    public static HResult SEC_E_PKINIT_NAME_MISMATCH => new(0x8009033D);

    /// <summary>Smart card logon is required and was not used.</summary>
    [Description("""Smart card logon is required and was not used.""")]
    [HResultConstant(0x8009033E)]
    public static HResult SEC_E_SMARTCARD_LOGON_REQUIRED => new(0x8009033E);

    /// <summary>A system shutdown is in progress.</summary>
    [Description("""A system shutdown is in progress.""")]
    [HResultConstant(0x8009033F)]
    public static HResult SEC_E_SHUTDOWN_IN_PROGRESS => new(0x8009033F);

    /// <summary>An invalid request was sent to the KDC.</summary>
    [Description("""An invalid request was sent to the KDC.""")]
    [HResultConstant(0x80090340)]
    public static HResult SEC_E_KDC_INVALID_REQUEST => new(0x80090340);

    /// <summary>The KDC was unable to generate a referral for the service requested.</summary>
    [Description("""The KDC was unable to generate a referral for the service requested.""")]
    [HResultConstant(0x80090341)]
    public static HResult SEC_E_KDC_UNABLE_TO_REFER => new(0x80090341);

    /// <summary>The encryption type requested is not supported by the KDC.</summary>
    [Description("""The encryption type requested is not supported by the KDC.""")]
    [HResultConstant(0x80090342)]
    public static HResult SEC_E_KDC_UNKNOWN_ETYPE => new(0x80090342);

    /// <summary>An unsupported pre-authentication mechanism was presented to the Kerberos package.</summary>
    [Description("""An unsupported pre-authentication mechanism was presented to the Kerberos package.""")]
    [HResultConstant(0x80090343)]
    public static HResult SEC_E_UNSUPPORTED_PREAUTH => new(0x80090343);

    /// <summary>The requested operation cannot be completed. The computer must be trusted for delegation, and the current user account must be configured to allow delegation.</summary>
    [Description("""The requested operation cannot be completed. The computer must be trusted for delegation, and the current user account must be configured to allow delegation.""")]
    [HResultConstant(0x80090345)]
    public static HResult SEC_E_DELEGATION_REQUIRED => new(0x80090345);

    /// <summary>Client's supplied Security Support Provider Interface (SSPI) channel bindings were incorrect.</summary>
    [Description("""Client's supplied Security Support Provider Interface (SSPI) channel bindings were incorrect.""")]
    [HResultConstant(0x80090346)]
    public static HResult SEC_E_BAD_BINDINGS => new(0x80090346);

    /// <summary>The received certificate was mapped to multiple accounts.</summary>
    [Description("""The received certificate was mapped to multiple accounts.""")]
    [HResultConstant(0x80090347)]
    public static HResult SEC_E_MULTIPLE_ACCOUNTS => new(0x80090347);

    /// <summary>No Kerberos key was found.</summary>
    [Description("""No Kerberos key was found.""")]
    [HResultConstant(0x80090348)]
    public static HResult SEC_E_NO_KERB_KEY => new(0x80090348);

    /// <summary>The certificate is not valid for the requested usage.</summary>
    [Description("""The certificate is not valid for the requested usage.""")]
    [HResultConstant(0x80090349)]
    public static HResult SEC_E_CERT_WRONG_USAGE => new(0x80090349);

    /// <summary>The system detected a possible attempt to compromise security. Ensure that you can contact the server that authenticated you.</summary>
    [Description("""The system detected a possible attempt to compromise security. Ensure that you can contact the server that authenticated you.""")]
    [HResultConstant(0x80090350)]
    public static HResult SEC_E_DOWNGRADE_DETECTED => new(0x80090350);

    /// <summary>The smart card certificate used for authentication has been revoked. Contact your system administrator. The event log might contain additional information.</summary>
    [Description("""The smart card certificate used for authentication has been revoked. Contact your system administrator. The event log might contain additional information.""")]
    [HResultConstant(0x80090351)]
    public static HResult SEC_E_SMARTCARD_CERT_REVOKED => new(0x80090351);

    /// <summary>An untrusted certification authority (CA) was detected while processing the smart card certificate used for authentication. Contact your system administrator.</summary>
    [Description("""An untrusted certification authority (CA) was detected while processing the smart card certificate used for authentication. Contact your system administrator.""")]
    [HResultConstant(0x80090352)]
    public static HResult SEC_E_ISSUING_CA_UNTRUSTED => new(0x80090352);

    /// <summary>The revocation status of the smart card certificate used for authentication could not be determined. Contact your system administrator.</summary>
    [Description("""The revocation status of the smart card certificate used for authentication could not be determined. Contact your system administrator.""")]
    [HResultConstant(0x80090353)]
    public static HResult SEC_E_REVOCATION_OFFLINE_C => new(0x80090353);

    /// <summary>The smart card certificate used for authentication was not trusted. Contact your system administrator.</summary>
    [Description("""The smart card certificate used for authentication was not trusted. Contact your system administrator.""")]
    [HResultConstant(0x80090354)]
    public static HResult SEC_E_PKINIT_CLIENT_FAILURE => new(0x80090354);

    /// <summary>The smart card certificate used for authentication has expired. Contact your system administrator.</summary>
    [Description("""The smart card certificate used for authentication has expired. Contact your system administrator.""")]
    [HResultConstant(0x80090355)]
    public static HResult SEC_E_SMARTCARD_CERT_EXPIRED => new(0x80090355);

    /// <summary>The Kerberos subsystem encountered an error. A service for user protocol requests was made against a domain controller that does not support services for users.</summary>
    [Description("""The Kerberos subsystem encountered an error. A service for user protocol requests was made against a domain controller that does not support services for users.""")]
    [HResultConstant(0x80090356)]
    public static HResult SEC_E_NO_S4U_PROT_SUPPORT => new(0x80090356);

    /// <summary>An attempt was made by this server to make a Kerberos-constrained delegation request for a target outside the server's realm. This is not supported and indicates a misconfiguration on this server's allowed-to-delegate-to list. Contact your administrator.</summary>
    [Description("""An attempt was made by this server to make a Kerberos-constrained delegation request for a target outside the server's realm. This is not supported and indicates a misconfiguration on this server's allowed-to-delegate-to list. Contact your administrator.""")]
    [HResultConstant(0x80090357)]
    public static HResult SEC_E_CROSSREALM_DELEGATION_FAILURE => new(0x80090357);

    /// <summary>The revocation status of the domain controller certificate used for smart card authentication could not be determined. The system event log contains additional information. Contact your system administrator.</summary>
    [Description("""The revocation status of the domain controller certificate used for smart card authentication could not be determined. The system event log contains additional information. Contact your system administrator.""")]
    [HResultConstant(0x80090358)]
    public static HResult SEC_E_REVOCATION_OFFLINE_KDC => new(0x80090358);

    /// <summary>An untrusted CA was detected while processing the domain controller certificate used for authentication. The system event log contains additional information. Contact your system administrator.</summary>
    [Description("""An untrusted CA was detected while processing the domain controller certificate used for authentication. The system event log contains additional information. Contact your system administrator.""")]
    [HResultConstant(0x80090359)]
    public static HResult SEC_E_ISSUING_CA_UNTRUSTED_KDC => new(0x80090359);

    /// <summary>The domain controller certificate used for smart card logon has expired. Contact your system administrator with the contents of your system event log.</summary>
    [Description("""The domain controller certificate used for smart card logon has expired. Contact your system administrator with the contents of your system event log.""")]
    [HResultConstant(0x8009035A)]
    public static HResult SEC_E_KDC_CERT_EXPIRED => new(0x8009035A);

    /// <summary>The domain controller certificate used for smart card logon has been revoked. Contact your system administrator with the contents of your system event log.</summary>
    [Description("""The domain controller certificate used for smart card logon has been revoked. Contact your system administrator with the contents of your system event log.""")]
    [HResultConstant(0x8009035B)]
    public static HResult SEC_E_KDC_CERT_REVOKED => new(0x8009035B);

    /// <summary>One or more of the parameters passed to the function were invalid.</summary>
    [Description("""One or more of the parameters passed to the function were invalid.""")]
    [HResultConstant(0x8009035D)]
    public static HResult SEC_E_INVALID_PARAMETER => new(0x8009035D);

    /// <summary>The client policy does not allow credential delegation to the target server.</summary>
    [Description("""The client policy does not allow credential delegation to the target server.""")]
    [HResultConstant(0x8009035E)]
    public static HResult SEC_E_DELEGATION_POLICY => new(0x8009035E);

    /// <summary>The client policy does not allow credential delegation to the target server with NLTM only authentication.</summary>
    [Description("""The client policy does not allow credential delegation to the target server with NLTM only authentication.""")]
    [HResultConstant(0x8009035F)]
    public static HResult SEC_E_POLICY_NLTM_ONLY => new(0x8009035F);

    /// <summary>An error occurred while performing an operation on a cryptographic message.</summary>
    [Description("""An error occurred while performing an operation on a cryptographic message.""")]
    [HResultConstant(0x80091001)]
    public static HResult CRYPT_E_MSG_ERROR => new(0x80091001);

    /// <summary>Unknown cryptographic algorithm.</summary>
    [Description("""Unknown cryptographic algorithm.""")]
    [HResultConstant(0x80091002)]
    public static HResult CRYPT_E_UNKNOWN_ALGO => new(0x80091002);

    /// <summary>The object identifier is poorly formatted.</summary>
    [Description("""The object identifier is poorly formatted.""")]
    [HResultConstant(0x80091003)]
    public static HResult CRYPT_E_OID_FORMAT => new(0x80091003);

    /// <summary>Invalid cryptographic message type.</summary>
    [Description("""Invalid cryptographic message type.""")]
    [HResultConstant(0x80091004)]
    public static HResult CRYPT_E_INVALID_MSG_TYPE => new(0x80091004);

    /// <summary>Unexpected cryptographic message encoding.</summary>
    [Description("""Unexpected cryptographic message encoding.""")]
    [HResultConstant(0x80091005)]
    public static HResult CRYPT_E_UNEXPECTED_ENCODING => new(0x80091005);

    /// <summary>The cryptographic message does not contain an expected authenticated attribute.</summary>
    [Description("""The cryptographic message does not contain an expected authenticated attribute.""")]
    [HResultConstant(0x80091006)]
    public static HResult CRYPT_E_AUTH_ATTR_MISSING => new(0x80091006);

    /// <summary>The hash value is not correct.</summary>
    [Description("""The hash value is not correct.""")]
    [HResultConstant(0x80091007)]
    public static HResult CRYPT_E_HASH_VALUE => new(0x80091007);

    /// <summary>The index value is not valid.</summary>
    [Description("""The index value is not valid.""")]
    [HResultConstant(0x80091008)]
    public static HResult CRYPT_E_INVALID_INDEX => new(0x80091008);

    /// <summary>The content of the cryptographic message has already been decrypted.</summary>
    [Description("""The content of the cryptographic message has already been decrypted.""")]
    [HResultConstant(0x80091009)]
    public static HResult CRYPT_E_ALREADY_DECRYPTED => new(0x80091009);

    /// <summary>The content of the cryptographic message has not been decrypted yet.</summary>
    [Description("""The content of the cryptographic message has not been decrypted yet.""")]
    [HResultConstant(0x8009100A)]
    public static HResult CRYPT_E_NOT_DECRYPTED => new(0x8009100A);

    /// <summary>The enveloped-data message does not contain the specified recipient.</summary>
    [Description("""The enveloped-data message does not contain the specified recipient.""")]
    [HResultConstant(0x8009100B)]
    public static HResult CRYPT_E_RECIPIENT_NOT_FOUND => new(0x8009100B);

    /// <summary>Invalid control type.</summary>
    [Description("""Invalid control type.""")]
    [HResultConstant(0x8009100C)]
    public static HResult CRYPT_E_CONTROL_TYPE => new(0x8009100C);

    /// <summary>Invalid issuer or serial number.</summary>
    [Description("""Invalid issuer or serial number.""")]
    [HResultConstant(0x8009100D)]
    public static HResult CRYPT_E_ISSUER_SERIALNUMBER => new(0x8009100D);

    /// <summary>Cannot find the original signer.</summary>
    [Description("""Cannot find the original signer.""")]
    [HResultConstant(0x8009100E)]
    public static HResult CRYPT_E_SIGNER_NOT_FOUND => new(0x8009100E);

    /// <summary>The cryptographic message does not contain all of the requested attributes.</summary>
    [Description("""The cryptographic message does not contain all of the requested attributes.""")]
    [HResultConstant(0x8009100F)]
    public static HResult CRYPT_E_ATTRIBUTES_MISSING => new(0x8009100F);

    /// <summary>The streamed cryptographic message is not ready to return data.</summary>
    [Description("""The streamed cryptographic message is not ready to return data.""")]
    [HResultConstant(0x80091010)]
    public static HResult CRYPT_E_STREAM_MSG_NOT_READY => new(0x80091010);

    /// <summary>The streamed cryptographic message requires more data to complete the decode operation.</summary>
    [Description("""The streamed cryptographic message requires more data to complete the decode operation.""")]
    [HResultConstant(0x80091011)]
    public static HResult CRYPT_E_STREAM_INSUFFICIENT_DATA => new(0x80091011);

    /// <summary>The length specified for the output data was insufficient.</summary>
    [Description("""The length specified for the output data was insufficient.""")]
    [HResultConstant(0x80092001)]
    public static HResult CRYPT_E_BAD_LEN => new(0x80092001);

    /// <summary>An error occurred during the encode or decode operation.</summary>
    [Description("""An error occurred during the encode or decode operation.""")]
    [HResultConstant(0x80092002)]
    public static HResult CRYPT_E_BAD_ENCODE => new(0x80092002);

    /// <summary>An error occurred while reading or writing to a file.</summary>
    [Description("""An error occurred while reading or writing to a file.""")]
    [HResultConstant(0x80092003)]
    public static HResult CRYPT_E_FILE_ERROR => new(0x80092003);

    /// <summary>Cannot find object or property.</summary>
    [Description("""Cannot find object or property.""")]
    [HResultConstant(0x80092004)]
    public static HResult CRYPT_E_NOT_FOUND => new(0x80092004);

    /// <summary>The object or property already exists.</summary>
    [Description("""The object or property already exists.""")]
    [HResultConstant(0x80092005)]
    public static HResult CRYPT_E_EXISTS => new(0x80092005);

    /// <summary>No provider was specified for the store or object.</summary>
    [Description("""No provider was specified for the store or object.""")]
    [HResultConstant(0x80092006)]
    public static HResult CRYPT_E_NO_PROVIDER => new(0x80092006);

    /// <summary>The specified certificate is self-signed.</summary>
    [Description("""The specified certificate is self-signed.""")]
    [HResultConstant(0x80092007)]
    public static HResult CRYPT_E_SELF_SIGNED => new(0x80092007);

    /// <summary>The previous certificate or certificate revocation list (CRL) context was deleted.</summary>
    [Description("""The previous certificate or certificate revocation list (CRL) context was deleted.""")]
    [HResultConstant(0x80092008)]
    public static HResult CRYPT_E_DELETED_PREV => new(0x80092008);

    /// <summary>Cannot find the requested object.</summary>
    [Description("""Cannot find the requested object.""")]
    [HResultConstant(0x80092009)]
    public static HResult CRYPT_E_NO_MATCH => new(0x80092009);

    /// <summary>The certificate does not have a property that references a private key.</summary>
    [Description("""The certificate does not have a property that references a private key.""")]
    [HResultConstant(0x8009200A)]
    public static HResult CRYPT_E_UNEXPECTED_MSG_TYPE => new(0x8009200A);

    /// <summary>Cannot find the certificate and private key for decryption.</summary>
    [Description("""Cannot find the certificate and private key for decryption.""")]
    [HResultConstant(0x8009200B)]
    public static HResult CRYPT_E_NO_KEY_PROPERTY => new(0x8009200B);

    /// <summary>Cannot find the certificate and private key to use for decryption.</summary>
    [Description("""Cannot find the certificate and private key to use for decryption.""")]
    [HResultConstant(0x8009200C)]
    public static HResult CRYPT_E_NO_DECRYPT_CERT => new(0x8009200C);

    /// <summary>Not a cryptographic message or the cryptographic message is not formatted correctly.</summary>
    [Description("""Not a cryptographic message or the cryptographic message is not formatted correctly.""")]
    [HResultConstant(0x8009200D)]
    public static HResult CRYPT_E_BAD_MSG => new(0x8009200D);

    /// <summary>The signed cryptographic message does not have a signer for the specified signer index.</summary>
    [Description("""The signed cryptographic message does not have a signer for the specified signer index.""")]
    [HResultConstant(0x8009200E)]
    public static HResult CRYPT_E_NO_SIGNER => new(0x8009200E);

    /// <summary>Final closure is pending until additional frees or closes.</summary>
    [Description("""Final closure is pending until additional frees or closes.""")]
    [HResultConstant(0x8009200F)]
    public static HResult CRYPT_E_PENDING_CLOSE => new(0x8009200F);

    /// <summary>The certificate is revoked.</summary>
    [Description("""The certificate is revoked.""")]
    [HResultConstant(0x80092010)]
    public static HResult CRYPT_E_REVOKED => new(0x80092010);

    /// <summary>No DLL or exported function was found to verify revocation.</summary>
    [Description("""No DLL or exported function was found to verify revocation.""")]
    [HResultConstant(0x80092011)]
    public static HResult CRYPT_E_NO_REVOCATION_DLL => new(0x80092011);

    /// <summary>The revocation function was unable to check revocation for the certificate.</summary>
    [Description("""The revocation function was unable to check revocation for the certificate.""")]
    [HResultConstant(0x80092012)]
    public static HResult CRYPT_E_NO_REVOCATION_CHECK => new(0x80092012);

    /// <summary>The revocation function was unable to check revocation because the revocation server was offline.</summary>
    [Description("""The revocation function was unable to check revocation because the revocation server was offline.""")]
    [HResultConstant(0x80092013)]
    public static HResult CRYPT_E_REVOCATION_OFFLINE => new(0x80092013);

    /// <summary>The certificate is not in the revocation server's database.</summary>
    [Description("""The certificate is not in the revocation server's database.""")]
    [HResultConstant(0x80092014)]
    public static HResult CRYPT_E_NOT_IN_REVOCATION_DATABASE => new(0x80092014);

    /// <summary>The string contains a non-numeric character.</summary>
    [Description("""The string contains a non-numeric character.""")]
    [HResultConstant(0x80092020)]
    public static HResult CRYPT_E_INVALID_NUMERIC_STRING => new(0x80092020);

    /// <summary>The string contains a nonprintable character.</summary>
    [Description("""The string contains a nonprintable character.""")]
    [HResultConstant(0x80092021)]
    public static HResult CRYPT_E_INVALID_PRINTABLE_STRING => new(0x80092021);

    /// <summary>The string contains a character not in the 7-bit ASCII character set.</summary>
    [Description("""The string contains a character not in the 7-bit ASCII character set.""")]
    [HResultConstant(0x80092022)]
    public static HResult CRYPT_E_INVALID_IA5_STRING => new(0x80092022);

    /// <summary>The string contains an invalid X500 name attribute key, object identifier (OID), value, or delimiter.</summary>
    [Description("""The string contains an invalid X500 name attribute key, object identifier (OID), value, or delimiter.""")]
    [HResultConstant(0x80092023)]
    public static HResult CRYPT_E_INVALID_X500_STRING => new(0x80092023);

    /// <summary>The dwValueType for the CERT_NAME_VALUE is not one of the character strings. Most likely it is either a CERT_RDN_ENCODED_BLOB or CERT_TDN_OCTED_STRING.</summary>
    [Description("""The dwValueType for the CERT_NAME_VALUE is not one of the character strings. Most likely it is either a CERT_RDN_ENCODED_BLOB or CERT_TDN_OCTED_STRING.""")]
    [HResultConstant(0x80092024)]
    public static HResult CRYPT_E_NOT_CHAR_STRING => new(0x80092024);

    /// <summary>The Put operation cannot continue. The file needs to be resized. However, there is already a signature present. A complete signing operation must be done.</summary>
    [Description("""The Put operation cannot continue. The file needs to be resized. However, there is already a signature present. A complete signing operation must be done.""")]
    [HResultConstant(0x80092025)]
    public static HResult CRYPT_E_FILERESIZED => new(0x80092025);

    /// <summary>The cryptographic operation failed due to a local security option setting.</summary>
    [Description("""The cryptographic operation failed due to a local security option setting.""")]
    [HResultConstant(0x80092026)]
    public static HResult CRYPT_E_SECURITY_SETTINGS => new(0x80092026);

    /// <summary>No DLL or exported function was found to verify subject usage.</summary>
    [Description("""No DLL or exported function was found to verify subject usage.""")]
    [HResultConstant(0x80092027)]
    public static HResult CRYPT_E_NO_VERIFY_USAGE_DLL => new(0x80092027);

    /// <summary>The called function was unable to perform a usage check on the subject.</summary>
    [Description("""The called function was unable to perform a usage check on the subject.""")]
    [HResultConstant(0x80092028)]
    public static HResult CRYPT_E_NO_VERIFY_USAGE_CHECK => new(0x80092028);

    /// <summary>The called function was unable to complete the usage check because the server was offline.</summary>
    [Description("""The called function was unable to complete the usage check because the server was offline.""")]
    [HResultConstant(0x80092029)]
    public static HResult CRYPT_E_VERIFY_USAGE_OFFLINE => new(0x80092029);

    /// <summary>The subject was not found in a certificate trust list (CTL).</summary>
    [Description("""The subject was not found in a certificate trust list (CTL).""")]
    [HResultConstant(0x8009202A)]
    public static HResult CRYPT_E_NOT_IN_CTL => new(0x8009202A);

    /// <summary>None of the signers of the cryptographic message or certificate trust list is trusted.</summary>
    [Description("""None of the signers of the cryptographic message or certificate trust list is trusted.""")]
    [HResultConstant(0x8009202B)]
    public static HResult CRYPT_E_NO_TRUSTED_SIGNER => new(0x8009202B);

    /// <summary>The public key's algorithm parameters are missing.</summary>
    [Description("""The public key's algorithm parameters are missing.""")]
    [HResultConstant(0x8009202C)]
    public static HResult CRYPT_E_MISSING_PUBKEY_PARA => new(0x8009202C);

    /// <summary>OSS Certificate encode/decode error code base.</summary>
    [Description("""OSS Certificate encode/decode error code base.""")]
    [HResultConstant(0x80093000)]
    public static HResult CRYPT_E_OSS_ERROR => new(0x80093000);

    /// <summary>OSS ASN.1 Error: Output Buffer is too small.</summary>
    [Description("""OSS ASN.1 Error: Output Buffer is too small.""")]
    [HResultConstant(0x80093001)]
    public static HResult OSS_MORE_BUF => new(0x80093001);

    /// <summary>OSS ASN.1 Error: Signed integer is encoded as a unsigned integer.</summary>
    [Description("""OSS ASN.1 Error: Signed integer is encoded as a unsigned integer.""")]
    [HResultConstant(0x80093002)]
    public static HResult OSS_NEGATIVE_UINTEGER => new(0x80093002);

    /// <summary>OSS ASN.1 Error: Unknown ASN.1 data type.</summary>
    [Description("""OSS ASN.1 Error: Unknown ASN.1 data type.""")]
    [HResultConstant(0x80093003)]
    public static HResult OSS_PDU_RANGE => new(0x80093003);

    /// <summary>OSS ASN.1 Error: Output buffer is too small; the decoded data has been truncated.</summary>
    [Description("""OSS ASN.1 Error: Output buffer is too small; the decoded data has been truncated.""")]
    [HResultConstant(0x80093004)]
    public static HResult OSS_MORE_INPUT => new(0x80093004);

    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    [Description("""OSS ASN.1 Error: Invalid data.""")]
    [HResultConstant(0x80093005)]
    public static HResult OSS_DATA_ERROR => new(0x80093005);

    /// <summary>OSS ASN.1 Error: Invalid argument.</summary>
    [Description("""OSS ASN.1 Error: Invalid argument.""")]
    [HResultConstant(0x80093006)]
    public static HResult OSS_BAD_ARG => new(0x80093006);

    /// <summary>OSS ASN.1 Error: Encode/Decode version mismatch.</summary>
    [Description("""OSS ASN.1 Error: Encode/Decode version mismatch.""")]
    [HResultConstant(0x80093007)]
    public static HResult OSS_BAD_VERSION => new(0x80093007);

    /// <summary>OSS ASN.1 Error: Out of memory.</summary>
    [Description("""OSS ASN.1 Error: Out of memory.""")]
    [HResultConstant(0x80093008)]
    public static HResult OSS_OUT_MEMORY => new(0x80093008);

    /// <summary>OSS ASN.1 Error: Encode/Decode error.</summary>
    [Description("""OSS ASN.1 Error: Encode/Decode error.""")]
    [HResultConstant(0x80093009)]
    public static HResult OSS_PDU_MISMATCH => new(0x80093009);

    /// <summary>OSS ASN.1 Error: Internal error.</summary>
    [Description("""OSS ASN.1 Error: Internal error.""")]
    [HResultConstant(0x8009300A)]
    public static HResult OSS_LIMITED => new(0x8009300A);

    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    [Description("""OSS ASN.1 Error: Invalid data.""")]
    [HResultConstant(0x8009300B)]
    public static HResult OSS_BAD_PTR => new(0x8009300B);

    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    [Description("""OSS ASN.1 Error: Invalid data.""")]
    [HResultConstant(0x8009300C)]
    public static HResult OSS_BAD_TIME => new(0x8009300C);

    /// <summary>OSS ASN.1 Error: Unsupported BER indefinite-length encoding.</summary>
    [Description("""OSS ASN.1 Error: Unsupported BER indefinite-length encoding.""")]
    [HResultConstant(0x8009300D)]
    public static HResult OSS_INDEFINITE_NOT_SUPPORTED => new(0x8009300D);

    /// <summary>OSS ASN.1 Error: Access violation.</summary>
    [Description("""OSS ASN.1 Error: Access violation.""")]
    [HResultConstant(0x8009300E)]
    public static HResult OSS_MEM_ERROR => new(0x8009300E);

    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    [Description("""OSS ASN.1 Error: Invalid data.""")]
    [HResultConstant(0x8009300F)]
    public static HResult OSS_BAD_TABLE => new(0x8009300F);

    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    [Description("""OSS ASN.1 Error: Invalid data.""")]
    [HResultConstant(0x80093010)]
    public static HResult OSS_TOO_LONG => new(0x80093010);

    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    [Description("""OSS ASN.1 Error: Invalid data.""")]
    [HResultConstant(0x80093011)]
    public static HResult OSS_CONSTRAINT_VIOLATED => new(0x80093011);

    /// <summary>OSS ASN.1 Error: Internal error.</summary>
    [Description("""OSS ASN.1 Error: Internal error.""")]
    [HResultConstant(0x80093012)]
    public static HResult OSS_FATAL_ERROR => new(0x80093012);

    /// <summary>OSS ASN.1 Error: Multithreading conflict.</summary>
    [Description("""OSS ASN.1 Error: Multithreading conflict.""")]
    [HResultConstant(0x80093013)]
    public static HResult OSS_ACCESS_SERIALIZATION_ERROR => new(0x80093013);

    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    [Description("""OSS ASN.1 Error: Invalid data.""")]
    [HResultConstant(0x80093014)]
    public static HResult OSS_NULL_TBL => new(0x80093014);

    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    [Description("""OSS ASN.1 Error: Invalid data.""")]
    [HResultConstant(0x80093015)]
    public static HResult OSS_NULL_FCN => new(0x80093015);

    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    [Description("""OSS ASN.1 Error: Invalid data.""")]
    [HResultConstant(0x80093016)]
    public static HResult OSS_BAD_ENCRULES => new(0x80093016);

    /// <summary>OSS ASN.1 Error: Encode/Decode function not implemented.</summary>
    [Description("""OSS ASN.1 Error: Encode/Decode function not implemented.""")]
    [HResultConstant(0x80093017)]
    public static HResult OSS_UNAVAIL_ENCRULES => new(0x80093017);

    /// <summary>OSS ASN.1 Error: Trace file error.</summary>
    [Description("""OSS ASN.1 Error: Trace file error.""")]
    [HResultConstant(0x80093018)]
    public static HResult OSS_CANT_OPEN_TRACE_WINDOW => new(0x80093018);

    /// <summary>OSS ASN.1 Error: Function not implemented.</summary>
    [Description("""OSS ASN.1 Error: Function not implemented.""")]
    [HResultConstant(0x80093019)]
    public static HResult OSS_UNIMPLEMENTED => new(0x80093019);

    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    [Description("""OSS ASN.1 Error: Program link error.""")]
    [HResultConstant(0x8009301A)]
    public static HResult OSS_OID_DLL_NOT_LINKED => new(0x8009301A);

    /// <summary>OSS ASN.1 Error: Trace file error.</summary>
    [Description("""OSS ASN.1 Error: Trace file error.""")]
    [HResultConstant(0x8009301B)]
    public static HResult OSS_CANT_OPEN_TRACE_FILE => new(0x8009301B);

    /// <summary>OSS ASN.1 Error: Trace file error.</summary>
    [Description("""OSS ASN.1 Error: Trace file error.""")]
    [HResultConstant(0x8009301C)]
    public static HResult OSS_TRACE_FILE_ALREADY_OPEN => new(0x8009301C);

    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    [Description("""OSS ASN.1 Error: Invalid data.""")]
    [HResultConstant(0x8009301D)]
    public static HResult OSS_TABLE_MISMATCH => new(0x8009301D);

    /// <summary>OSS ASN.1 Error: Invalid data.</summary>
    [Description("""OSS ASN.1 Error: Invalid data.""")]
    [HResultConstant(0x8009301E)]
    public static HResult OSS_TYPE_NOT_SUPPORTED => new(0x8009301E);

    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    [Description("""OSS ASN.1 Error: Program link error.""")]
    [HResultConstant(0x8009301F)]
    public static HResult OSS_REAL_DLL_NOT_LINKED => new(0x8009301F);

    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    [Description("""OSS ASN.1 Error: Program link error.""")]
    [HResultConstant(0x80093020)]
    public static HResult OSS_REAL_CODE_NOT_LINKED => new(0x80093020);

    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    [Description("""OSS ASN.1 Error: Program link error.""")]
    [HResultConstant(0x80093021)]
    public static HResult OSS_OUT_OF_RANGE => new(0x80093021);

    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    [Description("""OSS ASN.1 Error: Program link error.""")]
    [HResultConstant(0x80093022)]
    public static HResult OSS_COPIER_DLL_NOT_LINKED => new(0x80093022);

    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    [Description("""OSS ASN.1 Error: Program link error.""")]
    [HResultConstant(0x80093023)]
    public static HResult OSS_CONSTRAINT_DLL_NOT_LINKED => new(0x80093023);

    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    [Description("""OSS ASN.1 Error: Program link error.""")]
    [HResultConstant(0x80093024)]
    public static HResult OSS_COMPARATOR_DLL_NOT_LINKED => new(0x80093024);

    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    [Description("""OSS ASN.1 Error: Program link error.""")]
    [HResultConstant(0x80093025)]
    public static HResult OSS_COMPARATOR_CODE_NOT_LINKED => new(0x80093025);

    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    [Description("""OSS ASN.1 Error: Program link error.""")]
    [HResultConstant(0x80093026)]
    public static HResult OSS_MEM_MGR_DLL_NOT_LINKED => new(0x80093026);

    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    [Description("""OSS ASN.1 Error: Program link error.""")]
    [HResultConstant(0x80093027)]
    public static HResult OSS_PDV_DLL_NOT_LINKED => new(0x80093027);

    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    [Description("""OSS ASN.1 Error: Program link error.""")]
    [HResultConstant(0x80093028)]
    public static HResult OSS_PDV_CODE_NOT_LINKED => new(0x80093028);

    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    [Description("""OSS ASN.1 Error: Program link error.""")]
    [HResultConstant(0x80093029)]
    public static HResult OSS_API_DLL_NOT_LINKED => new(0x80093029);

    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    [Description("""OSS ASN.1 Error: Program link error.""")]
    [HResultConstant(0x8009302A)]
    public static HResult OSS_BERDER_DLL_NOT_LINKED => new(0x8009302A);

    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    [Description("""OSS ASN.1 Error: Program link error.""")]
    [HResultConstant(0x8009302B)]
    public static HResult OSS_PER_DLL_NOT_LINKED => new(0x8009302B);

    /// <summary>OSS ASN.1 Error: Program link error.</summary>
    [Description("""OSS ASN.1 Error: Program link error.""")]
    [HResultConstant(0x8009302C)]
    public static HResult OSS_OPEN_TYPE_ERROR => new(0x8009302C);

    /// <summary>OSS ASN.1 Error: System resource error.</summary>
    [Description("""OSS ASN.1 Error: System resource error.""")]
    [HResultConstant(0x8009302D)]
    public static HResult OSS_MUTEX_NOT_CREATED => new(0x8009302D);

    /// <summary>OSS ASN.1 Error: Trace file error.</summary>
    [Description("""OSS ASN.1 Error: Trace file error.""")]
    [HResultConstant(0x8009302E)]
    public static HResult OSS_CANT_CLOSE_TRACE_FILE => new(0x8009302E);

    /// <summary>ASN1 Certificate encode/decode error code base.</summary>
    [Description("""ASN1 Certificate encode/decode error code base.""")]
    [HResultConstant(0x80093100)]
    public static HResult CRYPT_E_ASN1_ERROR => new(0x80093100);

    /// <summary>ASN1 internal encode or decode error.</summary>
    [Description("""ASN1 internal encode or decode error.""")]
    [HResultConstant(0x80093101)]
    public static HResult CRYPT_E_ASN1_INTERNAL => new(0x80093101);

    /// <summary>ASN1 unexpected end of data.</summary>
    [Description("""ASN1 unexpected end of data.""")]
    [HResultConstant(0x80093102)]
    public static HResult CRYPT_E_ASN1_EOD => new(0x80093102);

    /// <summary>ASN1 corrupted data.</summary>
    [Description("""ASN1 corrupted data.""")]
    [HResultConstant(0x80093103)]
    public static HResult CRYPT_E_ASN1_CORRUPT => new(0x80093103);

    /// <summary>ASN1 value too large.</summary>
    [Description("""ASN1 value too large.""")]
    [HResultConstant(0x80093104)]
    public static HResult CRYPT_E_ASN1_LARGE => new(0x80093104);

    /// <summary>ASN1 constraint violated.</summary>
    [Description("""ASN1 constraint violated.""")]
    [HResultConstant(0x80093105)]
    public static HResult CRYPT_E_ASN1_CONSTRAINT => new(0x80093105);

    /// <summary>ASN1 out of memory.</summary>
    [Description("""ASN1 out of memory.""")]
    [HResultConstant(0x80093106)]
    public static HResult CRYPT_E_ASN1_MEMORY => new(0x80093106);

    /// <summary>ASN1 buffer overflow.</summary>
    [Description("""ASN1 buffer overflow.""")]
    [HResultConstant(0x80093107)]
    public static HResult CRYPT_E_ASN1_OVERFLOW => new(0x80093107);

    /// <summary>ASN1 function not supported for this protocol data unit (PDU).</summary>
    [Description("""ASN1 function not supported for this protocol data unit (PDU).""")]
    [HResultConstant(0x80093108)]
    public static HResult CRYPT_E_ASN1_BADPDU => new(0x80093108);

    /// <summary>ASN1 bad arguments to function call.</summary>
    [Description("""ASN1 bad arguments to function call.""")]
    [HResultConstant(0x80093109)]
    public static HResult CRYPT_E_ASN1_BADARGS => new(0x80093109);

    /// <summary>ASN1 bad real value.</summary>
    [Description("""ASN1 bad real value.""")]
    [HResultConstant(0x8009310A)]
    public static HResult CRYPT_E_ASN1_BADREAL => new(0x8009310A);

    /// <summary>ASN1 bad tag value met.</summary>
    [Description("""ASN1 bad tag value met.""")]
    [HResultConstant(0x8009310B)]
    public static HResult CRYPT_E_ASN1_BADTAG => new(0x8009310B);

    /// <summary>ASN1 bad choice value.</summary>
    [Description("""ASN1 bad choice value.""")]
    [HResultConstant(0x8009310C)]
    public static HResult CRYPT_E_ASN1_CHOICE => new(0x8009310C);

    /// <summary>ASN1 bad encoding rule.</summary>
    [Description("""ASN1 bad encoding rule.""")]
    [HResultConstant(0x8009310D)]
    public static HResult CRYPT_E_ASN1_RULE => new(0x8009310D);

    /// <summary>ASN1 bad Unicode (UTF8).</summary>
    [Description("""ASN1 bad Unicode (UTF8).""")]
    [HResultConstant(0x8009310E)]
    public static HResult CRYPT_E_ASN1_UTF8 => new(0x8009310E);

    /// <summary>ASN1 bad PDU type.</summary>
    [Description("""ASN1 bad PDU type.""")]
    [HResultConstant(0x80093133)]
    public static HResult CRYPT_E_ASN1_PDU_TYPE => new(0x80093133);

    /// <summary>ASN1 not yet implemented.</summary>
    [Description("""ASN1 not yet implemented.""")]
    [HResultConstant(0x80093134)]
    public static HResult CRYPT_E_ASN1_NYI => new(0x80093134);

    /// <summary>ASN1 skipped unknown extensions.</summary>
    [Description("""ASN1 skipped unknown extensions.""")]
    [HResultConstant(0x80093201)]
    public static HResult CRYPT_E_ASN1_EXTENDED => new(0x80093201);

    /// <summary>ASN1 end of data expected.</summary>
    [Description("""ASN1 end of data expected.""")]
    [HResultConstant(0x80093202)]
    public static HResult CRYPT_E_ASN1_NOEOD => new(0x80093202);

    /// <summary>The request subject name is invalid or too long.</summary>
    [Description("""The request subject name is invalid or too long.""")]
    [HResultConstant(0x80094001)]
    public static HResult CERTSRV_E_BAD_REQUESTSUBJECT => new(0x80094001);

    /// <summary>The request does not exist.</summary>
    [Description("""The request does not exist.""")]
    [HResultConstant(0x80094002)]
    public static HResult CERTSRV_E_NO_REQUEST => new(0x80094002);

    /// <summary>The request's current status does not allow this operation.</summary>
    [Description("""The request's current status does not allow this operation.""")]
    [HResultConstant(0x80094003)]
    public static HResult CERTSRV_E_BAD_REQUESTSTATUS => new(0x80094003);

    /// <summary>The requested property value is empty.</summary>
    [Description("""The requested property value is empty.""")]
    [HResultConstant(0x80094004)]
    public static HResult CERTSRV_E_PROPERTY_EMPTY => new(0x80094004);

    /// <summary>The CA's certificate contains invalid data.</summary>
    [Description("""The CA's certificate contains invalid data.""")]
    [HResultConstant(0x80094005)]
    public static HResult CERTSRV_E_INVALID_CA_CERTIFICATE => new(0x80094005);

    /// <summary>Certificate service has been suspended for a database restore operation.</summary>
    [Description("""Certificate service has been suspended for a database restore operation.""")]
    [HResultConstant(0x80094006)]
    public static HResult CERTSRV_E_SERVER_SUSPENDED => new(0x80094006);

    /// <summary>The certificate contains an encoded length that is potentially incompatible with older enrollment software.</summary>
    [Description("""The certificate contains an encoded length that is potentially incompatible with older enrollment software.""")]
    [HResultConstant(0x80094007)]
    public static HResult CERTSRV_E_ENCODING_LENGTH => new(0x80094007);

    /// <summary>The operation is denied. The user has multiple roles assigned, and the CA is configured to enforce role separation.</summary>
    [Description("""The operation is denied. The user has multiple roles assigned, and the CA is configured to enforce role separation.""")]
    [HResultConstant(0x80094008)]
    public static HResult CERTSRV_E_ROLECONFLICT => new(0x80094008);

    /// <summary>The operation is denied. It can only be performed by a certificate manager that is allowed to manage certificates for the current requester.</summary>
    [Description("""The operation is denied. It can only be performed by a certificate manager that is allowed to manage certificates for the current requester.""")]
    [HResultConstant(0x80094009)]
    public static HResult CERTSRV_E_RESTRICTEDOFFICER => new(0x80094009);

    /// <summary>Cannot archive private key. The CA is not configured for key archival.</summary>
    [Description("""Cannot archive private key. The CA is not configured for key archival.""")]
    [HResultConstant(0x8009400A)]
    public static HResult CERTSRV_E_KEY_ARCHIVAL_NOT_CONFIGURED => new(0x8009400A);

    /// <summary>Cannot archive private key. The CA could not verify one or more key recovery certificates.</summary>
    [Description("""Cannot archive private key. The CA could not verify one or more key recovery certificates.""")]
    [HResultConstant(0x8009400B)]
    public static HResult CERTSRV_E_NO_VALID_KRA => new(0x8009400B);

    /// <summary>The request is incorrectly formatted. The encrypted private key must be in an unauthenticated attribute in an outermost signature.</summary>
    [Description("""The request is incorrectly formatted. The encrypted private key must be in an unauthenticated attribute in an outermost signature.""")]
    [HResultConstant(0x8009400C)]
    public static HResult CERTSRV_E_BAD_REQUEST_KEY_ARCHIVAL => new(0x8009400C);

    /// <summary>At least one security principal must have the permission to manage this CA.</summary>
    [Description("""At least one security principal must have the permission to manage this CA.""")]
    [HResultConstant(0x8009400D)]
    public static HResult CERTSRV_E_NO_CAADMIN_DEFINED => new(0x8009400D);

    /// <summary>The request contains an invalid renewal certificate attribute.</summary>
    [Description("""The request contains an invalid renewal certificate attribute.""")]
    [HResultConstant(0x8009400E)]
    public static HResult CERTSRV_E_BAD_RENEWAL_CERT_ATTRIBUTE => new(0x8009400E);

    /// <summary>An attempt was made to open a CA database session, but there are already too many active sessions. The server needs to be configured to allow additional sessions.</summary>
    [Description("""An attempt was made to open a CA database session, but there are already too many active sessions. The server needs to be configured to allow additional sessions.""")]
    [HResultConstant(0x8009400F)]
    public static HResult CERTSRV_E_NO_DB_SESSIONS => new(0x8009400F);

    /// <summary>A memory reference caused a data alignment fault.</summary>
    [Description("""A memory reference caused a data alignment fault.""")]
    [HResultConstant(0x80094010)]
    public static HResult CERTSRV_E_ALIGNMENT_FAULT => new(0x80094010);

    /// <summary>The permissions on this CA do not allow the current user to enroll for certificates.</summary>
    [Description("""The permissions on this CA do not allow the current user to enroll for certificates.""")]
    [HResultConstant(0x80094011)]
    public static HResult CERTSRV_E_ENROLL_DENIED => new(0x80094011);

    /// <summary>The permissions on the certificate template do not allow the current user to enroll for this type of certificate.</summary>
    [Description("""The permissions on the certificate template do not allow the current user to enroll for this type of certificate.""")]
    [HResultConstant(0x80094012)]
    public static HResult CERTSRV_E_TEMPLATE_DENIED => new(0x80094012);

    /// <summary>The contacted domain controller cannot support signed Lightweight Directory Access Protocol (LDAP) traffic. Update the domain controller or configure Certificate Services to use SSL for Active Directory access.</summary>
    [Description("""The contacted domain controller cannot support signed Lightweight Directory Access Protocol (LDAP) traffic. Update the domain controller or configure Certificate Services to use SSL for Active Directory access.""")]
    [HResultConstant(0x80094013)]
    public static HResult CERTSRV_E_DOWNLEVEL_DC_SSL_OR_UPGRADE => new(0x80094013);

    /// <summary>The requested certificate template is not supported by this CA.</summary>
    [Description("""The requested certificate template is not supported by this CA.""")]
    [HResultConstant(0x80094800)]
    public static HResult CERTSRV_E_UNSUPPORTED_CERT_TYPE => new(0x80094800);

    /// <summary>The request contains no certificate template information.</summary>
    [Description("""The request contains no certificate template information.""")]
    [HResultConstant(0x80094801)]
    public static HResult CERTSRV_E_NO_CERT_TYPE => new(0x80094801);

    /// <summary>The request contains conflicting template information.</summary>
    [Description("""The request contains conflicting template information.""")]
    [HResultConstant(0x80094802)]
    public static HResult CERTSRV_E_TEMPLATE_CONFLICT => new(0x80094802);

    /// <summary>The request is missing a required Subject Alternate name extension.</summary>
    [Description("""The request is missing a required Subject Alternate name extension.""")]
    [HResultConstant(0x80094803)]
    public static HResult CERTSRV_E_SUBJECT_ALT_NAME_REQUIRED => new(0x80094803);

    /// <summary>The request is missing a required private key for archival by the server.</summary>
    [Description("""The request is missing a required private key for archival by the server.""")]
    [HResultConstant(0x80094804)]
    public static HResult CERTSRV_E_ARCHIVED_KEY_REQUIRED => new(0x80094804);

    /// <summary>The request is missing a required SMIME capabilities extension.</summary>
    [Description("""The request is missing a required SMIME capabilities extension.""")]
    [HResultConstant(0x80094805)]
    public static HResult CERTSRV_E_SMIME_REQUIRED => new(0x80094805);

    /// <summary>The request was made on behalf of a subject other than the caller. The certificate template must be configured to require at least one signature to authorize the request.</summary>
    [Description("""The request was made on behalf of a subject other than the caller. The certificate template must be configured to require at least one signature to authorize the request.""")]
    [HResultConstant(0x80094806)]
    public static HResult CERTSRV_E_BAD_RENEWAL_SUBJECT => new(0x80094806);

    /// <summary>The request template version is newer than the supported template version.</summary>
    [Description("""The request template version is newer than the supported template version.""")]
    [HResultConstant(0x80094807)]
    public static HResult CERTSRV_E_BAD_TEMPLATE_VERSION => new(0x80094807);

    /// <summary>The template is missing a required signature policy attribute.</summary>
    [Description("""The template is missing a required signature policy attribute.""")]
    [HResultConstant(0x80094808)]
    public static HResult CERTSRV_E_TEMPLATE_POLICY_REQUIRED => new(0x80094808);

    /// <summary>The request is missing required signature policy information.</summary>
    [Description("""The request is missing required signature policy information.""")]
    [HResultConstant(0x80094809)]
    public static HResult CERTSRV_E_SIGNATURE_POLICY_REQUIRED => new(0x80094809);

    /// <summary>The request is missing one or more required signatures.</summary>
    [Description("""The request is missing one or more required signatures.""")]
    [HResultConstant(0x8009480A)]
    public static HResult CERTSRV_E_SIGNATURE_COUNT => new(0x8009480A);

    /// <summary>One or more signatures did not include the required application or issuance policies. The request is missing one or more required valid signatures.</summary>
    [Description("""One or more signatures did not include the required application or issuance policies. The request is missing one or more required valid signatures.""")]
    [HResultConstant(0x8009480B)]
    public static HResult CERTSRV_E_SIGNATURE_REJECTED => new(0x8009480B);

    /// <summary>The request is missing one or more required signature issuance policies.</summary>
    [Description("""The request is missing one or more required signature issuance policies.""")]
    [HResultConstant(0x8009480C)]
    public static HResult CERTSRV_E_ISSUANCE_POLICY_REQUIRED => new(0x8009480C);

    /// <summary>The UPN is unavailable and cannot be added to the Subject Alternate name.</summary>
    [Description("""The UPN is unavailable and cannot be added to the Subject Alternate name.""")]
    [HResultConstant(0x8009480D)]
    public static HResult CERTSRV_E_SUBJECT_UPN_REQUIRED => new(0x8009480D);

    /// <summary>The Active Directory GUID is unavailable and cannot be added to the Subject Alternate name.</summary>
    [Description("""The Active Directory GUID is unavailable and cannot be added to the Subject Alternate name.""")]
    [HResultConstant(0x8009480E)]
    public static HResult CERTSRV_E_SUBJECT_DIRECTORY_GUID_REQUIRED => new(0x8009480E);

    /// <summary>The Domain Name System (DNS) name is unavailable and cannot be added to the Subject Alternate name.</summary>
    [Description("""The Domain Name System (DNS) name is unavailable and cannot be added to the Subject Alternate name.""")]
    [HResultConstant(0x8009480F)]
    public static HResult CERTSRV_E_SUBJECT_DNS_REQUIRED => new(0x8009480F);

    /// <summary>The request includes a private key for archival by the server, but key archival is not enabled for the specified certificate template.</summary>
    [Description("""The request includes a private key for archival by the server, but key archival is not enabled for the specified certificate template.""")]
    [HResultConstant(0x80094810)]
    public static HResult CERTSRV_E_ARCHIVED_KEY_UNEXPECTED => new(0x80094810);

    /// <summary>The public key does not meet the minimum size required by the specified certificate template.</summary>
    [Description("""The public key does not meet the minimum size required by the specified certificate template.""")]
    [HResultConstant(0x80094811)]
    public static HResult CERTSRV_E_KEY_LENGTH => new(0x80094811);

    /// <summary>The email name is unavailable and cannot be added to the Subject or Subject Alternate name.</summary>
    [Description("""The email name is unavailable and cannot be added to the Subject or Subject Alternate name.""")]
    [HResultConstant(0x80094812)]
    public static HResult CERTSRV_E_SUBJECT_EMAIL_REQUIRED => new(0x80094812);

    /// <summary>One or more certificate templates to be enabled on this CA could not be found.</summary>
    [Description("""One or more certificate templates to be enabled on this CA could not be found.""")]
    [HResultConstant(0x80094813)]
    public static HResult CERTSRV_E_UNKNOWN_CERT_TYPE => new(0x80094813);

    /// <summary>The certificate template renewal period is longer than the certificate validity period. The template should be reconfigured or the CA certificate renewed.</summary>
    [Description("""The certificate template renewal period is longer than the certificate validity period. The template should be reconfigured or the CA certificate renewed.""")]
    [HResultConstant(0x80094814)]
    public static HResult CERTSRV_E_CERT_TYPE_OVERLAP => new(0x80094814);

    /// <summary>The certificate template requires too many return authorization (RA) signatures. Only one RA signature is allowed.</summary>
    [Description("""The certificate template requires too many return authorization (RA) signatures. Only one RA signature is allowed.""")]
    [HResultConstant(0x80094815)]
    public static HResult CERTSRV_E_TOO_MANY_SIGNATURES => new(0x80094815);

    /// <summary>The key used in a renewal request does not match one of the certificates being renewed.</summary>
    [Description("""The key used in a renewal request does not match one of the certificates being renewed.""")]
    [HResultConstant(0x80094816)]
    public static HResult CERTSRV_E_RENEWAL_BAD_PUBLIC_KEY => new(0x80094816);

    /// <summary>The endorsement key certificate is not valid.</summary>
    [Description("""The endorsement key certificate is not valid.""")]
    [HResultConstant(0x80094817)]
    public static HResult CERTSRV_E_INVALID_EK => new(0x80094817);

    /// <summary>Key attestation did not succeed.</summary>
    [Description("""Key attestation did not succeed.""")]
    [HResultConstant(0x8009481A)]
    public static HResult CERTSRV_E_KEY_ATTESTATION => new(0x8009481A);

    /// <summary>The key is not exportable.</summary>
    [Description("""The key is not exportable.""")]
    [HResultConstant(0x80095000)]
    public static HResult XENROLL_E_KEY_NOT_EXPORTABLE => new(0x80095000);

    /// <summary>You cannot add the root CA certificate into your local store.</summary>
    [Description("""You cannot add the root CA certificate into your local store.""")]
    [HResultConstant(0x80095001)]
    public static HResult XENROLL_E_CANNOT_ADD_ROOT_CERT => new(0x80095001);

    /// <summary>The key archival hash attribute was not found in the response.</summary>
    [Description("""The key archival hash attribute was not found in the response.""")]
    [HResultConstant(0x80095002)]
    public static HResult XENROLL_E_RESPONSE_KA_HASH_NOT_FOUND => new(0x80095002);

    /// <summary>An unexpected key archival hash attribute was found in the response.</summary>
    [Description("""An unexpected key archival hash attribute was found in the response.""")]
    [HResultConstant(0x80095003)]
    public static HResult XENROLL_E_RESPONSE_UNEXPECTED_KA_HASH => new(0x80095003);

    /// <summary>There is a key archival hash mismatch between the request and the response.</summary>
    [Description("""There is a key archival hash mismatch between the request and the response.""")]
    [HResultConstant(0x80095004)]
    public static HResult XENROLL_E_RESPONSE_KA_HASH_MISMATCH => new(0x80095004);

    /// <summary>Signing certificate cannot include SMIME extension.</summary>
    [Description("""Signing certificate cannot include SMIME extension.""")]
    [HResultConstant(0x80095005)]
    public static HResult XENROLL_E_KEYSPEC_SMIME_MISMATCH => new(0x80095005);

    /// <summary>A system-level error occurred while verifying trust.</summary>
    [Description("""A system-level error occurred while verifying trust.""")]
    [HResultConstant(0x80096001)]
    public static HResult TRUST_E_SYSTEM_ERROR => new(0x80096001);

    /// <summary>The certificate for the signer of the message is invalid or not found.</summary>
    [Description("""The certificate for the signer of the message is invalid or not found.""")]
    [HResultConstant(0x80096002)]
    public static HResult TRUST_E_NO_SIGNER_CERT => new(0x80096002);

    /// <summary>One of the counter signatures was invalid.</summary>
    [Description("""One of the counter signatures was invalid.""")]
    [HResultConstant(0x80096003)]
    public static HResult TRUST_E_COUNTER_SIGNER => new(0x80096003);

    /// <summary>The signature of the certificate cannot be verified.</summary>
    [Description("""The signature of the certificate cannot be verified.""")]
    [HResultConstant(0x80096004)]
    public static HResult TRUST_E_CERT_SIGNATURE => new(0x80096004);

    /// <summary>The time-stamp signature or certificate could not be verified or is malformed.</summary>
    [Description("""The time-stamp signature or certificate could not be verified or is malformed.""")]
    [HResultConstant(0x80096005)]
    public static HResult TRUST_E_TIME_STAMP => new(0x80096005);

    /// <summary>The digital signature of the object did not verify.</summary>
    [Description("""The digital signature of the object did not verify.""")]
    [HResultConstant(0x80096010)]
    public static HResult TRUST_E_BAD_DIGEST => new(0x80096010);

    /// <summary>A certificate's basic constraint extension has not been observed.</summary>
    [Description("""A certificate's basic constraint extension has not been observed.""")]
    [HResultConstant(0x80096019)]
    public static HResult TRUST_E_BASIC_CONSTRAINTS => new(0x80096019);

    /// <summary>The certificate does not meet or contain the Authenticode financial extensions.</summary>
    [Description("""The certificate does not meet or contain the Authenticode financial extensions.""")]
    [HResultConstant(0x8009601E)]
    public static HResult TRUST_E_FINANCIAL_CRITERIA => new(0x8009601E);

    /// <summary>Tried to reference a part of the file outside the proper range.</summary>
    [Description("""Tried to reference a part of the file outside the proper range.""")]
    [HResultConstant(0x80097001)]
    public static HResult MSSIPOTF_E_OUTOFMEMRANGE => new(0x80097001);

    /// <summary>Could not retrieve an object from the file.</summary>
    [Description("""Could not retrieve an object from the file.""")]
    [HResultConstant(0x80097002)]
    public static HResult MSSIPOTF_E_CANTGETOBJECT => new(0x80097002);

    /// <summary>Could not find the head table in the file.</summary>
    [Description("""Could not find the head table in the file.""")]
    [HResultConstant(0x80097003)]
    public static HResult MSSIPOTF_E_NOHEADTABLE => new(0x80097003);

    /// <summary>The magic number in the head table is incorrect.</summary>
    [Description("""The magic number in the head table is incorrect.""")]
    [HResultConstant(0x80097004)]
    public static HResult MSSIPOTF_E_BAD_MAGICNUMBER => new(0x80097004);

    /// <summary>The offset table has incorrect values.</summary>
    [Description("""The offset table has incorrect values.""")]
    [HResultConstant(0x80097005)]
    public static HResult MSSIPOTF_E_BAD_OFFSET_TABLE => new(0x80097005);

    /// <summary>Duplicate table tags or the tags are out of alphabetical order.</summary>
    [Description("""Duplicate table tags or the tags are out of alphabetical order.""")]
    [HResultConstant(0x80097006)]
    public static HResult MSSIPOTF_E_TABLE_TAGORDER => new(0x80097006);

    /// <summary>A table does not start on a long word boundary.</summary>
    [Description("""A table does not start on a long word boundary.""")]
    [HResultConstant(0x80097007)]
    public static HResult MSSIPOTF_E_TABLE_LONGWORD => new(0x80097007);

    /// <summary>First table does not appear after header information.</summary>
    [Description("""First table does not appear after header information.""")]
    [HResultConstant(0x80097008)]
    public static HResult MSSIPOTF_E_BAD_FIRST_TABLE_PLACEMENT => new(0x80097008);

    /// <summary>Two or more tables overlap.</summary>
    [Description("""Two or more tables overlap.""")]
    [HResultConstant(0x80097009)]
    public static HResult MSSIPOTF_E_TABLES_OVERLAP => new(0x80097009);

    /// <summary>Too many pad bytes between tables, or pad bytes are not 0.</summary>
    [Description("""Too many pad bytes between tables, or pad bytes are not 0.""")]
    [HResultConstant(0x8009700A)]
    public static HResult MSSIPOTF_E_TABLE_PADBYTES => new(0x8009700A);

    /// <summary>File is too small to contain the last table.</summary>
    [Description("""File is too small to contain the last table.""")]
    [HResultConstant(0x8009700B)]
    public static HResult MSSIPOTF_E_FILETOOSMALL => new(0x8009700B);

    /// <summary>A table checksum is incorrect.</summary>
    [Description("""A table checksum is incorrect.""")]
    [HResultConstant(0x8009700C)]
    public static HResult MSSIPOTF_E_TABLE_CHECKSUM => new(0x8009700C);

    /// <summary>The file checksum is incorrect.</summary>
    [Description("""The file checksum is incorrect.""")]
    [HResultConstant(0x8009700D)]
    public static HResult MSSIPOTF_E_FILE_CHECKSUM => new(0x8009700D);

    /// <summary>The signature does not have the correct attributes for the policy.</summary>
    [Description("""The signature does not have the correct attributes for the policy.""")]
    [HResultConstant(0x80097010)]
    public static HResult MSSIPOTF_E_FAILED_POLICY => new(0x80097010);

    /// <summary>The file did not pass the hints check.</summary>
    [Description("""The file did not pass the hints check.""")]
    [HResultConstant(0x80097011)]
    public static HResult MSSIPOTF_E_FAILED_HINTS_CHECK => new(0x80097011);

    /// <summary>The file is not an OpenType file.</summary>
    [Description("""The file is not an OpenType file.""")]
    [HResultConstant(0x80097012)]
    public static HResult MSSIPOTF_E_NOT_OPENTYPE => new(0x80097012);

    /// <summary>Failed on a file operation (such as open, map, read, or write).</summary>
    [Description("""Failed on a file operation (such as open, map, read, or write).""")]
    [HResultConstant(0x80097013)]
    public static HResult MSSIPOTF_E_FILE => new(0x80097013);

    /// <summary>A call to a CryptoAPI function failed.</summary>
    [Description("""A call to a CryptoAPI function failed.""")]
    [HResultConstant(0x80097014)]
    public static HResult MSSIPOTF_E_CRYPT => new(0x80097014);

    /// <summary>There is a bad version number in the file.</summary>
    [Description("""There is a bad version number in the file.""")]
    [HResultConstant(0x80097015)]
    public static HResult MSSIPOTF_E_BADVERSION => new(0x80097015);

    /// <summary>The structure of the DSIG table is incorrect.</summary>
    [Description("""The structure of the DSIG table is incorrect.""")]
    [HResultConstant(0x80097016)]
    public static HResult MSSIPOTF_E_DSIG_STRUCTURE => new(0x80097016);

    /// <summary>A check failed in a partially constant table.</summary>
    [Description("""A check failed in a partially constant table.""")]
    [HResultConstant(0x80097017)]
    public static HResult MSSIPOTF_E_PCONST_CHECK => new(0x80097017);

    /// <summary>Some kind of structural error.</summary>
    [Description("""Some kind of structural error.""")]
    [HResultConstant(0x80097018)]
    public static HResult MSSIPOTF_E_STRUCTURE => new(0x80097018);

    /// <summary>The requested credential requires confirmation.</summary>
    [Description("""The requested credential requires confirmation.""")]
    [HResultConstant(0x80097019)]
    public static HResult ERROR_CRED_REQUIRES_CONFIRMATION => new(0x80097019);

    /// <summary>Unknown trust provider.</summary>
    [Description("""Unknown trust provider.""")]
    [HResultConstant(0x800B0001)]
    public static HResult TRUST_E_PROVIDER_UNKNOWN => new(0x800B0001);

    /// <summary>The trust verification action specified is not supported by the specified trust provider.</summary>
    [Description("""The trust verification action specified is not supported by the specified trust provider.""")]
    [HResultConstant(0x800B0002)]
    public static HResult TRUST_E_ACTION_UNKNOWN => new(0x800B0002);

    /// <summary>The form specified for the subject is not one supported or known by the specified trust provider.</summary>
    [Description("""The form specified for the subject is not one supported or known by the specified trust provider.""")]
    [HResultConstant(0x800B0003)]
    public static HResult TRUST_E_SUBJECT_FORM_UNKNOWN => new(0x800B0003);

    /// <summary>The subject is not trusted for the specified action.</summary>
    [Description("""The subject is not trusted for the specified action.""")]
    [HResultConstant(0x800B0004)]
    public static HResult TRUST_E_SUBJECT_NOT_TRUSTED => new(0x800B0004);

    /// <summary>Error due to problem in ASN.1 encoding process.</summary>
    [Description("""Error due to problem in ASN.1 encoding process.""")]
    [HResultConstant(0x800B0005)]
    public static HResult DIGSIG_E_ENCODE => new(0x800B0005);

    /// <summary>Error due to problem in ASN.1 decoding process.</summary>
    [Description("""Error due to problem in ASN.1 decoding process.""")]
    [HResultConstant(0x800B0006)]
    public static HResult DIGSIG_E_DECODE => new(0x800B0006);

    /// <summary>Reading/writing extensions where attributes are appropriate, and vice versa.</summary>
    [Description("""Reading/writing extensions where attributes are appropriate, and vice versa.""")]
    [HResultConstant(0x800B0007)]
    public static HResult DIGSIG_E_EXTENSIBILITY => new(0x800B0007);

    /// <summary>Unspecified cryptographic failure.</summary>
    [Description("""Unspecified cryptographic failure.""")]
    [HResultConstant(0x800B0008)]
    public static HResult DIGSIG_E_CRYPTO => new(0x800B0008);

    /// <summary>The size of the data could not be determined.</summary>
    [Description("""The size of the data could not be determined.""")]
    [HResultConstant(0x800B0009)]
    public static HResult PERSIST_E_SIZEDEFINITE => new(0x800B0009);

    /// <summary>The size of the indefinite-sized data could not be determined.</summary>
    [Description("""The size of the indefinite-sized data could not be determined.""")]
    [HResultConstant(0x800B000A)]
    public static HResult PERSIST_E_SIZEINDEFINITE => new(0x800B000A);

    /// <summary>This object does not read and write self-sizing data.</summary>
    [Description("""This object does not read and write self-sizing data.""")]
    [HResultConstant(0x800B000B)]
    public static HResult PERSIST_E_NOTSELFSIZING => new(0x800B000B);

    /// <summary>No signature was present in the subject.</summary>
    [Description("""No signature was present in the subject.""")]
    [HResultConstant(0x800B0100)]
    public static HResult TRUST_E_NOSIGNATURE => new(0x800B0100);

    /// <summary>A required certificate is not within its validity period when verifying against the current system clock or the time stamp in the signed file.</summary>
    [Description("""A required certificate is not within its validity period when verifying against the current system clock or the time stamp in the signed file.""")]
    [HResultConstant(0x800B0101)]
    public static HResult CERT_E_EXPIRED => new(0x800B0101);

    /// <summary>The validity periods of the certification chain do not nest correctly.</summary>
    [Description("""The validity periods of the certification chain do not nest correctly.""")]
    [HResultConstant(0x800B0102)]
    public static HResult CERT_E_VALIDITYPERIODNESTING => new(0x800B0102);

    /// <summary>A certificate that can only be used as an end entity is being used as a CA or vice versa.</summary>
    [Description("""A certificate that can only be used as an end entity is being used as a CA or vice versa.""")]
    [HResultConstant(0x800B0103)]
    public static HResult CERT_E_ROLE => new(0x800B0103);

    /// <summary>A path length constraint in the certification chain has been violated.</summary>
    [Description("""A path length constraint in the certification chain has been violated.""")]
    [HResultConstant(0x800B0104)]
    public static HResult CERT_E_PATHLENCONST => new(0x800B0104);

    /// <summary>A certificate contains an unknown extension that is marked "critical".</summary>
    [Description("""A certificate contains an unknown extension that is marked "critical".""")]
    [HResultConstant(0x800B0105)]
    public static HResult CERT_E_CRITICAL => new(0x800B0105);

    /// <summary>A certificate is being used for a purpose other than the ones specified by its CA.</summary>
    [Description("""A certificate is being used for a purpose other than the ones specified by its CA.""")]
    [HResultConstant(0x800B0106)]
    public static HResult CERT_E_PURPOSE => new(0x800B0106);

    /// <summary>A parent of a given certificate did not issue that child certificate.</summary>
    [Description("""A parent of a given certificate did not issue that child certificate.""")]
    [HResultConstant(0x800B0107)]
    public static HResult CERT_E_ISSUERCHAINING => new(0x800B0107);

    /// <summary>A certificate is missing or has an empty value for an important field, such as a subject or issuer name.</summary>
    [Description("""A certificate is missing or has an empty value for an important field, such as a subject or issuer name.""")]
    [HResultConstant(0x800B0108)]
    public static HResult CERT_E_MALFORMED => new(0x800B0108);

    /// <summary>A certificate chain processed, but terminated in a root certificate that is not trusted by the trust provider.</summary>
    [Description("""A certificate chain processed, but terminated in a root certificate that is not trusted by the trust provider.""")]
    [HResultConstant(0x800B0109)]
    public static HResult CERT_E_UNTRUSTEDROOT => new(0x800B0109);

    /// <summary>A certificate chain could not be built to a trusted root authority.</summary>
    [Description("""A certificate chain could not be built to a trusted root authority.""")]
    [HResultConstant(0x800B010A)]
    public static HResult CERT_E_CHAINING => new(0x800B010A);

    /// <summary>Generic trust failure.</summary>
    [Description("""Generic trust failure.""")]
    [HResultConstant(0x800B010B)]
    public static HResult TRUST_E_FAIL => new(0x800B010B);

    /// <summary>A certificate was explicitly revoked by its issuer. If the certificate is Microsoft Windows PCA 2010, then the driver was signed by a certificate no longer recognized by Windows.<3></summary>
    [Description("""A certificate was explicitly revoked by its issuer. If the certificate is Microsoft Windows PCA 2010, then the driver was signed by a certificate no longer recognized by Windows.<3>""")]
    [HResultConstant(0x800B010C)]
    public static HResult CERT_E_REVOKED => new(0x800B010C);

    /// <summary>The certification path terminates with the test root that is not trusted with the current policy settings.</summary>
    [Description("""The certification path terminates with the test root that is not trusted with the current policy settings.""")]
    [HResultConstant(0x800B010D)]
    public static HResult CERT_E_UNTRUSTEDTESTROOT => new(0x800B010D);

    /// <summary>The revocation process could not continue—the certificates could not be checked.</summary>
    [Description("""The revocation process could not continue—the certificates could not be checked.""")]
    [HResultConstant(0x800B010E)]
    public static HResult CERT_E_REVOCATION_FAILURE => new(0x800B010E);

    /// <summary>The certificate's CN name does not match the passed value.</summary>
    [Description("""The certificate's CN name does not match the passed value.""")]
    [HResultConstant(0x800B010F)]
    public static HResult CERT_E_CN_NO_MATCH => new(0x800B010F);

    /// <summary>The certificate is not valid for the requested usage.</summary>
    [Description("""The certificate is not valid for the requested usage.""")]
    [HResultConstant(0x800B0110)]
    public static HResult CERT_E_WRONG_USAGE => new(0x800B0110);

    /// <summary>The certificate was explicitly marked as untrusted by the user.</summary>
    [Description("""The certificate was explicitly marked as untrusted by the user.""")]
    [HResultConstant(0x800B0111)]
    public static HResult TRUST_E_EXPLICIT_DISTRUST => new(0x800B0111);

    /// <summary>A certification chain processed correctly, but one of the CA certificates is not trusted by the policy provider.</summary>
    [Description("""A certification chain processed correctly, but one of the CA certificates is not trusted by the policy provider.""")]
    [HResultConstant(0x800B0112)]
    public static HResult CERT_E_UNTRUSTEDCA => new(0x800B0112);

    /// <summary>The certificate has invalid policy.</summary>
    [Description("""The certificate has invalid policy.""")]
    [HResultConstant(0x800B0113)]
    public static HResult CERT_E_INVALID_POLICY => new(0x800B0113);

    /// <summary>The certificate has an invalid name. The name is not included in the permitted list or is explicitly excluded.</summary>
    [Description("""The certificate has an invalid name. The name is not included in the permitted list or is explicitly excluded.""")]
    [HResultConstant(0x800B0114)]
    public static HResult CERT_E_INVALID_NAME => new(0x800B0114);

    /// <summary>The maximum filebitrate value specified is greater than the server's configured maximum bandwidth.</summary>
    [Description("""The maximum filebitrate value specified is greater than the server's configured maximum bandwidth.""")]
    [HResultConstant(0x800D0003)]
    public static HResult NS_W_SERVER_BANDWIDTH_LIMIT => new(0x800D0003);

    /// <summary>The maximum bandwidth value specified is less than the maximum filebitrate.</summary>
    [Description("""The maximum bandwidth value specified is less than the maximum filebitrate.""")]
    [HResultConstant(0x800D0004)]
    public static HResult NS_W_FILE_BANDWIDTH_LIMIT => new(0x800D0004);

    /// <summary>Unknown %1 event encountered.</summary>
    [Description("""Unknown %1 event encountered.""")]
    [HResultConstant(0x800D0060)]
    public static HResult NS_W_UNKNOWN_EVENT => new(0x800D0060);

    /// <summary>Disk %1 ( %2 ) on Content Server %3, will be failed because it is catatonic.</summary>
    [Description("""Disk %1 ( %2 ) on Content Server %3, will be failed because it is catatonic.""")]
    [HResultConstant(0x800D0199)]
    public static HResult NS_I_CATATONIC_FAILURE => new(0x800D0199);

    /// <summary>Disk %1 ( %2 ) on Content Server %3, auto online from catatonic state.</summary>
    [Description("""Disk %1 ( %2 ) on Content Server %3, auto online from catatonic state.""")]
    [HResultConstant(0x800D019A)]
    public static HResult NS_I_CATATONIC_AUTO_UNFAIL => new(0x800D019A);

    /// <summary>A non-empty line was encountered in the INF before the start of a section.</summary>
    [Description("""A non-empty line was encountered in the INF before the start of a section.""")]
    [HResultConstant(0x800F0000)]
    public static HResult SPAPI_E_EXPECTED_SECTION_NAME => new(0x800F0000);

    /// <summary>A section name marker in the information file (INF) is not complete or does not exist on a line by itself.</summary>
    [Description("""A section name marker in the information file (INF) is not complete or does not exist on a line by itself.""")]
    [HResultConstant(0x800F0001)]
    public static HResult SPAPI_E_BAD_SECTION_NAME_LINE => new(0x800F0001);

    /// <summary>An INF section was encountered whose name exceeds the maximum section name length.</summary>
    [Description("""An INF section was encountered whose name exceeds the maximum section name length.""")]
    [HResultConstant(0x800F0002)]
    public static HResult SPAPI_E_SECTION_NAME_TOO_LONG => new(0x800F0002);

    /// <summary>The syntax of the INF is invalid.</summary>
    [Description("""The syntax of the INF is invalid.""")]
    [HResultConstant(0x800F0003)]
    public static HResult SPAPI_E_GENERAL_SYNTAX => new(0x800F0003);

    /// <summary>The style of the INF is different than what was requested.</summary>
    [Description("""The style of the INF is different than what was requested.""")]
    [HResultConstant(0x800F0100)]
    public static HResult SPAPI_E_WRONG_INF_STYLE => new(0x800F0100);

    /// <summary>The required section was not found in the INF.</summary>
    [Description("""The required section was not found in the INF.""")]
    [HResultConstant(0x800F0101)]
    public static HResult SPAPI_E_SECTION_NOT_FOUND => new(0x800F0101);

    /// <summary>The required line was not found in the INF.</summary>
    [Description("""The required line was not found in the INF.""")]
    [HResultConstant(0x800F0102)]
    public static HResult SPAPI_E_LINE_NOT_FOUND => new(0x800F0102);

    /// <summary>The files affected by the installation of this file queue have not been backed up for uninstall.</summary>
    [Description("""The files affected by the installation of this file queue have not been backed up for uninstall.""")]
    [HResultConstant(0x800F0103)]
    public static HResult SPAPI_E_NO_BACKUP => new(0x800F0103);

    /// <summary>The INF or the device information set or element does not have an associated install class.</summary>
    [Description("""The INF or the device information set or element does not have an associated install class.""")]
    [HResultConstant(0x800F0200)]
    public static HResult SPAPI_E_NO_ASSOCIATED_CLASS => new(0x800F0200);

    /// <summary>The INF or the device information set or element does not match the specified install class.</summary>
    [Description("""The INF or the device information set or element does not match the specified install class.""")]
    [HResultConstant(0x800F0201)]
    public static HResult SPAPI_E_CLASS_MISMATCH => new(0x800F0201);

    /// <summary>An existing device was found that is a duplicate of the device being manually installed.</summary>
    [Description("""An existing device was found that is a duplicate of the device being manually installed.""")]
    [HResultConstant(0x800F0202)]
    public static HResult SPAPI_E_DUPLICATE_FOUND => new(0x800F0202);

    /// <summary>There is no driver selected for the device information set or element.</summary>
    [Description("""There is no driver selected for the device information set or element.""")]
    [HResultConstant(0x800F0203)]
    public static HResult SPAPI_E_NO_DRIVER_SELECTED => new(0x800F0203);

    /// <summary>The requested device registry key does not exist.</summary>
    [Description("""The requested device registry key does not exist.""")]
    [HResultConstant(0x800F0204)]
    public static HResult SPAPI_E_KEY_DOES_NOT_EXIST => new(0x800F0204);

    /// <summary>The device instance name is invalid.</summary>
    [Description("""The device instance name is invalid.""")]
    [HResultConstant(0x800F0205)]
    public static HResult SPAPI_E_INVALID_DEVINST_NAME => new(0x800F0205);

    /// <summary>The install class is not present or is invalid.</summary>
    [Description("""The install class is not present or is invalid.""")]
    [HResultConstant(0x800F0206)]
    public static HResult SPAPI_E_INVALID_CLASS => new(0x800F0206);

    /// <summary>The device instance cannot be created because it already exists.</summary>
    [Description("""The device instance cannot be created because it already exists.""")]
    [HResultConstant(0x800F0207)]
    public static HResult SPAPI_E_DEVINST_ALREADY_EXISTS => new(0x800F0207);

    /// <summary>The operation cannot be performed on a device information element that has not been registered.</summary>
    [Description("""The operation cannot be performed on a device information element that has not been registered.""")]
    [HResultConstant(0x800F0208)]
    public static HResult SPAPI_E_DEVINFO_NOT_REGISTERED => new(0x800F0208);

    /// <summary>The device property code is invalid.</summary>
    [Description("""The device property code is invalid.""")]
    [HResultConstant(0x800F0209)]
    public static HResult SPAPI_E_INVALID_REG_PROPERTY => new(0x800F0209);

    /// <summary>The INF from which a driver list is to be built does not exist.</summary>
    [Description("""The INF from which a driver list is to be built does not exist.""")]
    [HResultConstant(0x800F020A)]
    public static HResult SPAPI_E_NO_INF => new(0x800F020A);

    /// <summary>The device instance does not exist in the hardware tree.</summary>
    [Description("""The device instance does not exist in the hardware tree.""")]
    [HResultConstant(0x800F020B)]
    public static HResult SPAPI_E_NO_SUCH_DEVINST => new(0x800F020B);

    /// <summary>The icon representing this install class cannot be loaded.</summary>
    [Description("""The icon representing this install class cannot be loaded.""")]
    [HResultConstant(0x800F020C)]
    public static HResult SPAPI_E_CANT_LOAD_CLASS_ICON => new(0x800F020C);

    /// <summary>The class installer registry entry is invalid.</summary>
    [Description("""The class installer registry entry is invalid.""")]
    [HResultConstant(0x800F020D)]
    public static HResult SPAPI_E_INVALID_CLASS_INSTALLER => new(0x800F020D);

    /// <summary>The class installer has indicated that the default action should be performed for this installation request.</summary>
    [Description("""The class installer has indicated that the default action should be performed for this installation request.""")]
    [HResultConstant(0x800F020E)]
    public static HResult SPAPI_E_DI_DO_DEFAULT => new(0x800F020E);

    /// <summary>The operation does not require any files to be copied.</summary>
    [Description("""The operation does not require any files to be copied.""")]
    [HResultConstant(0x800F020F)]
    public static HResult SPAPI_E_DI_NOFILECOPY => new(0x800F020F);

    /// <summary>The specified hardware profile does not exist.</summary>
    [Description("""The specified hardware profile does not exist.""")]
    [HResultConstant(0x800F0210)]
    public static HResult SPAPI_E_INVALID_HWPROFILE => new(0x800F0210);

    /// <summary>There is no device information element currently selected for this device information set.</summary>
    [Description("""There is no device information element currently selected for this device information set.""")]
    [HResultConstant(0x800F0211)]
    public static HResult SPAPI_E_NO_DEVICE_SELECTED => new(0x800F0211);

    /// <summary>The operation cannot be performed because the device information set is locked.</summary>
    [Description("""The operation cannot be performed because the device information set is locked.""")]
    [HResultConstant(0x800F0212)]
    public static HResult SPAPI_E_DEVINFO_LIST_LOCKED => new(0x800F0212);

    /// <summary>The operation cannot be performed because the device information element is locked.</summary>
    [Description("""The operation cannot be performed because the device information element is locked.""")]
    [HResultConstant(0x800F0213)]
    public static HResult SPAPI_E_DEVINFO_DATA_LOCKED => new(0x800F0213);

    /// <summary>The specified path does not contain any applicable device INFs.</summary>
    [Description("""The specified path does not contain any applicable device INFs.""")]
    [HResultConstant(0x800F0214)]
    public static HResult SPAPI_E_DI_BAD_PATH => new(0x800F0214);

    /// <summary>No class installer parameters have been set for the device information set or element.</summary>
    [Description("""No class installer parameters have been set for the device information set or element.""")]
    [HResultConstant(0x800F0215)]
    public static HResult SPAPI_E_NO_CLASSINSTALL_PARAMS => new(0x800F0215);

    /// <summary>The operation cannot be performed because the file queue is locked.</summary>
    [Description("""The operation cannot be performed because the file queue is locked.""")]
    [HResultConstant(0x800F0216)]
    public static HResult SPAPI_E_FILEQUEUE_LOCKED => new(0x800F0216);

    /// <summary>A service installation section in this INF is invalid.</summary>
    [Description("""A service installation section in this INF is invalid.""")]
    [HResultConstant(0x800F0217)]
    public static HResult SPAPI_E_BAD_SERVICE_INSTALLSECT => new(0x800F0217);

    /// <summary>There is no class driver list for the device information element.</summary>
    [Description("""There is no class driver list for the device information element.""")]
    [HResultConstant(0x800F0218)]
    public static HResult SPAPI_E_NO_CLASS_DRIVER_LIST => new(0x800F0218);

    /// <summary>The installation failed because a function driver was not specified for this device instance.</summary>
    [Description("""The installation failed because a function driver was not specified for this device instance.""")]
    [HResultConstant(0x800F0219)]
    public static HResult SPAPI_E_NO_ASSOCIATED_SERVICE => new(0x800F0219);

    /// <summary>There is presently no default device interface designated for this interface class.</summary>
    [Description("""There is presently no default device interface designated for this interface class.""")]
    [HResultConstant(0x800F021A)]
    public static HResult SPAPI_E_NO_DEFAULT_DEVICE_INTERFACE => new(0x800F021A);

    /// <summary>The operation cannot be performed because the device interface is currently active.</summary>
    [Description("""The operation cannot be performed because the device interface is currently active.""")]
    [HResultConstant(0x800F021B)]
    public static HResult SPAPI_E_DEVICE_INTERFACE_ACTIVE => new(0x800F021B);

    /// <summary>The operation cannot be performed because the device interface has been removed from the system.</summary>
    [Description("""The operation cannot be performed because the device interface has been removed from the system.""")]
    [HResultConstant(0x800F021C)]
    public static HResult SPAPI_E_DEVICE_INTERFACE_REMOVED => new(0x800F021C);

    /// <summary>An interface installation section in this INF is invalid.</summary>
    [Description("""An interface installation section in this INF is invalid.""")]
    [HResultConstant(0x800F021D)]
    public static HResult SPAPI_E_BAD_INTERFACE_INSTALLSECT => new(0x800F021D);

    /// <summary>This interface class does not exist in the system.</summary>
    [Description("""This interface class does not exist in the system.""")]
    [HResultConstant(0x800F021E)]
    public static HResult SPAPI_E_NO_SUCH_INTERFACE_CLASS => new(0x800F021E);

    /// <summary>The reference string supplied for this interface device is invalid.</summary>
    [Description("""The reference string supplied for this interface device is invalid.""")]
    [HResultConstant(0x800F021F)]
    public static HResult SPAPI_E_INVALID_REFERENCE_STRING => new(0x800F021F);

    /// <summary>The specified machine name does not conform to Universal Naming Convention (UNCs).</summary>
    [Description("""The specified machine name does not conform to Universal Naming Convention (UNCs).""")]
    [HResultConstant(0x800F0220)]
    public static HResult SPAPI_E_INVALID_MACHINENAME => new(0x800F0220);

    /// <summary>A general remote communication error occurred.</summary>
    [Description("""A general remote communication error occurred.""")]
    [HResultConstant(0x800F0221)]
    public static HResult SPAPI_E_REMOTE_COMM_FAILURE => new(0x800F0221);

    /// <summary>The machine selected for remote communication is not available at this time.</summary>
    [Description("""The machine selected for remote communication is not available at this time.""")]
    [HResultConstant(0x800F0222)]
    public static HResult SPAPI_E_MACHINE_UNAVAILABLE => new(0x800F0222);

    /// <summary>The Plug and Play service is not available on the remote machine.</summary>
    [Description("""The Plug and Play service is not available on the remote machine.""")]
    [HResultConstant(0x800F0223)]
    public static HResult SPAPI_E_NO_CONFIGMGR_SERVICES => new(0x800F0223);

    /// <summary>The property page provider registry entry is invalid.</summary>
    [Description("""The property page provider registry entry is invalid.""")]
    [HResultConstant(0x800F0224)]
    public static HResult SPAPI_E_INVALID_PROPPAGE_PROVIDER => new(0x800F0224);

    /// <summary>The requested device interface is not present in the system.</summary>
    [Description("""The requested device interface is not present in the system.""")]
    [HResultConstant(0x800F0225)]
    public static HResult SPAPI_E_NO_SUCH_DEVICE_INTERFACE => new(0x800F0225);

    /// <summary>The device's co-installer has additional work to perform after installation is complete.</summary>
    [Description("""The device's co-installer has additional work to perform after installation is complete.""")]
    [HResultConstant(0x800F0226)]
    public static HResult SPAPI_E_DI_POSTPROCESSING_REQUIRED => new(0x800F0226);

    /// <summary>The device's co-installer is invalid.</summary>
    [Description("""The device's co-installer is invalid.""")]
    [HResultConstant(0x800F0227)]
    public static HResult SPAPI_E_INVALID_COINSTALLER => new(0x800F0227);

    /// <summary>There are no compatible drivers for this device.</summary>
    [Description("""There are no compatible drivers for this device.""")]
    [HResultConstant(0x800F0228)]
    public static HResult SPAPI_E_NO_COMPAT_DRIVERS => new(0x800F0228);

    /// <summary>There is no icon that represents this device or device type.</summary>
    [Description("""There is no icon that represents this device or device type.""")]
    [HResultConstant(0x800F0229)]
    public static HResult SPAPI_E_NO_DEVICE_ICON => new(0x800F0229);

    /// <summary>A logical configuration specified in this INF is invalid.</summary>
    [Description("""A logical configuration specified in this INF is invalid.""")]
    [HResultConstant(0x800F022A)]
    public static HResult SPAPI_E_INVALID_INF_LOGCONFIG => new(0x800F022A);

    /// <summary>The class installer has denied the request to install or upgrade this device.</summary>
    [Description("""The class installer has denied the request to install or upgrade this device.""")]
    [HResultConstant(0x800F022B)]
    public static HResult SPAPI_E_DI_DONT_INSTALL => new(0x800F022B);

    /// <summary>One of the filter drivers installed for this device is invalid.</summary>
    [Description("""One of the filter drivers installed for this device is invalid.""")]
    [HResultConstant(0x800F022C)]
    public static HResult SPAPI_E_INVALID_FILTER_DRIVER => new(0x800F022C);

    /// <summary>The driver selected for this device does not support Windows XP operating system.</summary>
    [Description("""The driver selected for this device does not support Windows XP operating system.""")]
    [HResultConstant(0x800F022D)]
    public static HResult SPAPI_E_NON_WINDOWS_NT_DRIVER => new(0x800F022D);

    /// <summary>The driver selected for this device does not support Windows.</summary>
    [Description("""The driver selected for this device does not support Windows.""")]
    [HResultConstant(0x800F022E)]
    public static HResult SPAPI_E_NON_WINDOWS_DRIVER => new(0x800F022E);

    /// <summary>The third-party INF does not contain digital signature information.</summary>
    [Description("""The third-party INF does not contain digital signature information.""")]
    [HResultConstant(0x800F022F)]
    public static HResult SPAPI_E_NO_CATALOG_FOR_OEM_INF => new(0x800F022F);

    /// <summary>An invalid attempt was made to use a device installation file queue for verification of digital signatures relative to other platforms.</summary>
    [Description("""An invalid attempt was made to use a device installation file queue for verification of digital signatures relative to other platforms.""")]
    [HResultConstant(0x800F0230)]
    public static HResult SPAPI_E_DEVINSTALL_QUEUE_NONNATIVE => new(0x800F0230);

    /// <summary>The device cannot be disabled.</summary>
    [Description("""The device cannot be disabled.""")]
    [HResultConstant(0x800F0231)]
    public static HResult SPAPI_E_NOT_DISABLEABLE => new(0x800F0231);

    /// <summary>The device could not be dynamically removed.</summary>
    [Description("""The device could not be dynamically removed.""")]
    [HResultConstant(0x800F0232)]
    public static HResult SPAPI_E_CANT_REMOVE_DEVINST => new(0x800F0232);

    /// <summary>Cannot copy to specified target.</summary>
    [Description("""Cannot copy to specified target.""")]
    [HResultConstant(0x800F0233)]
    public static HResult SPAPI_E_INVALID_TARGET => new(0x800F0233);

    /// <summary>Driver is not intended for this platform.</summary>
    [Description("""Driver is not intended for this platform.""")]
    [HResultConstant(0x800F0234)]
    public static HResult SPAPI_E_DRIVER_NONNATIVE => new(0x800F0234);

    /// <summary>Operation not allowed in WOW64.</summary>
    [Description("""Operation not allowed in WOW64.""")]
    [HResultConstant(0x800F0235)]
    public static HResult SPAPI_E_IN_WOW64 => new(0x800F0235);

    /// <summary>The operation involving unsigned file copying was rolled back, so that a system restore point could be set.</summary>
    [Description("""The operation involving unsigned file copying was rolled back, so that a system restore point could be set.""")]
    [HResultConstant(0x800F0236)]
    public static HResult SPAPI_E_SET_SYSTEM_RESTORE_POINT => new(0x800F0236);

    /// <summary>An INF was copied into the Windows INF directory in an improper manner.</summary>
    [Description("""An INF was copied into the Windows INF directory in an improper manner.""")]
    [HResultConstant(0x800F0237)]
    public static HResult SPAPI_E_INCORRECTLY_COPIED_INF => new(0x800F0237);

    /// <summary>The Security Configuration Editor (SCE) APIs have been disabled on this embedded product.</summary>
    [Description("""The Security Configuration Editor (SCE) APIs have been disabled on this embedded product.""")]
    [HResultConstant(0x800F0238)]
    public static HResult SPAPI_E_SCE_DISABLED => new(0x800F0238);

    /// <summary>An unknown exception was encountered.</summary>
    [Description("""An unknown exception was encountered.""")]
    [HResultConstant(0x800F0239)]
    public static HResult SPAPI_E_UNKNOWN_EXCEPTION => new(0x800F0239);

    /// <summary>A problem was encountered when accessing the Plug and Play registry database.</summary>
    [Description("""A problem was encountered when accessing the Plug and Play registry database.""")]
    [HResultConstant(0x800F023A)]
    public static HResult SPAPI_E_PNP_REGISTRY_ERROR => new(0x800F023A);

    /// <summary>The requested operation is not supported for a remote machine.</summary>
    [Description("""The requested operation is not supported for a remote machine.""")]
    [HResultConstant(0x800F023B)]
    public static HResult SPAPI_E_REMOTE_REQUEST_UNSUPPORTED => new(0x800F023B);

    /// <summary>The specified file is not an installed original equipment manufacturer (OEM) INF.</summary>
    [Description("""The specified file is not an installed original equipment manufacturer (OEM) INF.""")]
    [HResultConstant(0x800F023C)]
    public static HResult SPAPI_E_NOT_AN_INSTALLED_OEM_INF => new(0x800F023C);

    /// <summary>One or more devices are presently installed using the specified INF.</summary>
    [Description("""One or more devices are presently installed using the specified INF.""")]
    [HResultConstant(0x800F023D)]
    public static HResult SPAPI_E_INF_IN_USE_BY_DEVICES => new(0x800F023D);

    /// <summary>The requested device install operation is obsolete.</summary>
    [Description("""The requested device install operation is obsolete.""")]
    [HResultConstant(0x800F023E)]
    public static HResult SPAPI_E_DI_FUNCTION_OBSOLETE => new(0x800F023E);

    /// <summary>A file could not be verified because it does not have an associated catalog signed via Authenticode.</summary>
    [Description("""A file could not be verified because it does not have an associated catalog signed via Authenticode.""")]
    [HResultConstant(0x800F023F)]
    public static HResult SPAPI_E_NO_AUTHENTICODE_CATALOG => new(0x800F023F);

    /// <summary>Authenticode signature verification is not supported for the specified INF.</summary>
    [Description("""Authenticode signature verification is not supported for the specified INF.""")]
    [HResultConstant(0x800F0240)]
    public static HResult SPAPI_E_AUTHENTICODE_DISALLOWED => new(0x800F0240);

    /// <summary>The INF was signed with an Authenticode catalog from a trusted publisher.</summary>
    [Description("""The INF was signed with an Authenticode catalog from a trusted publisher.""")]
    [HResultConstant(0x800F0241)]
    public static HResult SPAPI_E_AUTHENTICODE_TRUSTED_PUBLISHER => new(0x800F0241);

    /// <summary>The publisher of an Authenticode-signed catalog has not yet been established as trusted.</summary>
    [Description("""The publisher of an Authenticode-signed catalog has not yet been established as trusted.""")]
    [HResultConstant(0x800F0242)]
    public static HResult SPAPI_E_AUTHENTICODE_TRUST_NOT_ESTABLISHED => new(0x800F0242);

    /// <summary>The publisher of an Authenticode-signed catalog was not established as trusted.</summary>
    [Description("""The publisher of an Authenticode-signed catalog was not established as trusted.""")]
    [HResultConstant(0x800F0243)]
    public static HResult SPAPI_E_AUTHENTICODE_PUBLISHER_NOT_TRUSTED => new(0x800F0243);

    /// <summary>The software was tested for compliance with Windows logo requirements on a different version of Windows and might not be compatible with this version.</summary>
    [Description("""The software was tested for compliance with Windows logo requirements on a different version of Windows and might not be compatible with this version.""")]
    [HResultConstant(0x800F0244)]
    public static HResult SPAPI_E_SIGNATURE_OSATTRIBUTE_MISMATCH => new(0x800F0244);

    /// <summary>The file can be validated only by a catalog signed via Authenticode.</summary>
    [Description("""The file can be validated only by a catalog signed via Authenticode.""")]
    [HResultConstant(0x800F0245)]
    public static HResult SPAPI_E_ONLY_VALIDATE_VIA_AUTHENTICODE => new(0x800F0245);

    /// <summary>One of the installers for this device cannot perform the installation at this time.</summary>
    [Description("""One of the installers for this device cannot perform the installation at this time.""")]
    [HResultConstant(0x800F0246)]
    public static HResult SPAPI_E_DEVICE_INSTALLER_NOT_READY => new(0x800F0246);

    /// <summary>A problem was encountered while attempting to add the driver to the store.</summary>
    [Description("""A problem was encountered while attempting to add the driver to the store.""")]
    [HResultConstant(0x800F0247)]
    public static HResult SPAPI_E_DRIVER_STORE_ADD_FAILED => new(0x800F0247);

    /// <summary>The installation of this device is forbidden by system policy. Contact your system administrator.</summary>
    [Description("""The installation of this device is forbidden by system policy. Contact your system administrator.""")]
    [HResultConstant(0x800F0248)]
    public static HResult SPAPI_E_DEVICE_INSTALL_BLOCKED => new(0x800F0248);

    /// <summary>The installation of this driver is forbidden by system policy. Contact your system administrator.</summary>
    [Description("""The installation of this driver is forbidden by system policy. Contact your system administrator.""")]
    [HResultConstant(0x800F0249)]
    public static HResult SPAPI_E_DRIVER_INSTALL_BLOCKED => new(0x800F0249);

    /// <summary>The specified INF is the wrong type for this operation.</summary>
    [Description("""The specified INF is the wrong type for this operation.""")]
    [HResultConstant(0x800F024A)]
    public static HResult SPAPI_E_WRONG_INF_TYPE => new(0x800F024A);

    /// <summary>The hash for the file is not present in the specified catalog file. The file is likely corrupt or the victim of tampering.</summary>
    [Description("""The hash for the file is not present in the specified catalog file. The file is likely corrupt or the victim of tampering.""")]
    [HResultConstant(0x800F024B)]
    public static HResult SPAPI_E_FILE_HASH_NOT_IN_CATALOG => new(0x800F024B);

    /// <summary>A problem was encountered while attempting to delete the driver from the store.</summary>
    [Description("""A problem was encountered while attempting to delete the driver from the store.""")]
    [HResultConstant(0x800F024C)]
    public static HResult SPAPI_E_DRIVER_STORE_DELETE_FAILED => new(0x800F024C);

    /// <summary>An unrecoverable stack overflow was encountered.</summary>
    [Description("""An unrecoverable stack overflow was encountered.""")]
    [HResultConstant(0x800F0300)]
    public static HResult SPAPI_E_UNRECOVERABLE_STACK_OVERFLOW => new(0x800F0300);

    /// <summary>No installed components were detected.</summary>
    [Description("""No installed components were detected.""")]
    [HResultConstant(0x800F1000)]
    public static HResult SPAPI_E_ERROR_NOT_INSTALLED => new(0x800F1000);

    /// <summary>An internal consistency check failed.</summary>
    [Description("""An internal consistency check failed.""")]
    [HResultConstant(0x80100001)]
    public static HResult SCARD_F_INTERNAL_ERROR => new(0x80100001);

    /// <summary>The action was canceled by an SCardCancel request.</summary>
    [Description("""The action was canceled by an SCardCancel request.""")]
    [HResultConstant(0x80100002)]
    public static HResult SCARD_E_CANCELLED => new(0x80100002);

    /// <summary>The supplied handle was invalid.</summary>
    [Description("""The supplied handle was invalid.""")]
    [HResultConstant(0x80100003)]
    public static HResult SCARD_E_INVALID_HANDLE => new(0x80100003);

    /// <summary>One or more of the supplied parameters could not be properly interpreted.</summary>
    [Description("""One or more of the supplied parameters could not be properly interpreted.""")]
    [HResultConstant(0x80100004)]
    public static HResult SCARD_E_INVALID_PARAMETER => new(0x80100004);

    /// <summary>Registry startup information is missing or invalid.</summary>
    [Description("""Registry startup information is missing or invalid.""")]
    [HResultConstant(0x80100005)]
    public static HResult SCARD_E_INVALID_TARGET => new(0x80100005);

    /// <summary>Not enough memory available to complete this command.</summary>
    [Description("""Not enough memory available to complete this command.""")]
    [HResultConstant(0x80100006)]
    public static HResult SCARD_E_NO_MEMORY => new(0x80100006);

    /// <summary>An internal consistency timer has expired.</summary>
    [Description("""An internal consistency timer has expired.""")]
    [HResultConstant(0x80100007)]
    public static HResult SCARD_F_WAITED_TOO_LONG => new(0x80100007);

    /// <summary>The data buffer to receive returned data is too small for the returned data.</summary>
    [Description("""The data buffer to receive returned data is too small for the returned data.""")]
    [HResultConstant(0x80100008)]
    public static HResult SCARD_E_INSUFFICIENT_BUFFER => new(0x80100008);

    /// <summary>The specified reader name is not recognized.</summary>
    [Description("""The specified reader name is not recognized.""")]
    [HResultConstant(0x80100009)]
    public static HResult SCARD_E_UNKNOWN_READER => new(0x80100009);

    /// <summary>The user-specified time-out value has expired.</summary>
    [Description("""The user-specified time-out value has expired.""")]
    [HResultConstant(0x8010000A)]
    public static HResult SCARD_E_TIMEOUT => new(0x8010000A);

    /// <summary>The smart card cannot be accessed because of other connections outstanding.</summary>
    [Description("""The smart card cannot be accessed because of other connections outstanding.""")]
    [HResultConstant(0x8010000B)]
    public static HResult SCARD_E_SHARING_VIOLATION => new(0x8010000B);

    /// <summary>The operation requires a smart card, but no smart card is currently in the device.</summary>
    [Description("""The operation requires a smart card, but no smart card is currently in the device.""")]
    [HResultConstant(0x8010000C)]
    public static HResult SCARD_E_NO_SMARTCARD => new(0x8010000C);

    /// <summary>The specified smart card name is not recognized.</summary>
    [Description("""The specified smart card name is not recognized.""")]
    [HResultConstant(0x8010000D)]
    public static HResult SCARD_E_UNKNOWN_CARD => new(0x8010000D);

    /// <summary>The system could not dispose of the media in the requested manner.</summary>
    [Description("""The system could not dispose of the media in the requested manner.""")]
    [HResultConstant(0x8010000E)]
    public static HResult SCARD_E_CANT_DISPOSE => new(0x8010000E);

    /// <summary>The requested protocols are incompatible with the protocol currently in use with the smart card.</summary>
    [Description("""The requested protocols are incompatible with the protocol currently in use with the smart card.""")]
    [HResultConstant(0x8010000F)]
    public static HResult SCARD_E_PROTO_MISMATCH => new(0x8010000F);

    /// <summary>The reader or smart card is not ready to accept commands.</summary>
    [Description("""The reader or smart card is not ready to accept commands.""")]
    [HResultConstant(0x80100010)]
    public static HResult SCARD_E_NOT_READY => new(0x80100010);

    /// <summary>One or more of the supplied parameters values could not be properly interpreted.</summary>
    [Description("""One or more of the supplied parameters values could not be properly interpreted.""")]
    [HResultConstant(0x80100011)]
    public static HResult SCARD_E_INVALID_VALUE => new(0x80100011);

    /// <summary>The action was canceled by the system, presumably to log off or shut down.</summary>
    [Description("""The action was canceled by the system, presumably to log off or shut down.""")]
    [HResultConstant(0x80100012)]
    public static HResult SCARD_E_SYSTEM_CANCELLED => new(0x80100012);

    /// <summary>An internal communications error has been detected.</summary>
    [Description("""An internal communications error has been detected.""")]
    [HResultConstant(0x80100013)]
    public static HResult SCARD_F_COMM_ERROR => new(0x80100013);

    /// <summary>An internal error has been detected, but the source is unknown.</summary>
    [Description("""An internal error has been detected, but the source is unknown.""")]
    [HResultConstant(0x80100014)]
    public static HResult SCARD_F_UNKNOWN_ERROR => new(0x80100014);

    /// <summary>An automatic terminal recognition (ATR) obtained from the registry is not a valid ATR string.</summary>
    [Description("""An automatic terminal recognition (ATR) obtained from the registry is not a valid ATR string.""")]
    [HResultConstant(0x80100015)]
    public static HResult SCARD_E_INVALID_ATR => new(0x80100015);

    /// <summary>An attempt was made to end a nonexistent transaction.</summary>
    [Description("""An attempt was made to end a nonexistent transaction.""")]
    [HResultConstant(0x80100016)]
    public static HResult SCARD_E_NOT_TRANSACTED => new(0x80100016);

    /// <summary>The specified reader is not currently available for use.</summary>
    [Description("""The specified reader is not currently available for use.""")]
    [HResultConstant(0x80100017)]
    public static HResult SCARD_E_READER_UNAVAILABLE => new(0x80100017);

    /// <summary>The operation has been aborted to allow the server application to exit.</summary>
    [Description("""The operation has been aborted to allow the server application to exit.""")]
    [HResultConstant(0x80100018)]
    public static HResult SCARD_P_SHUTDOWN => new(0x80100018);

    /// <summary>The peripheral component interconnect (PCI) Receive buffer was too small.</summary>
    [Description("""The peripheral component interconnect (PCI) Receive buffer was too small.""")]
    [HResultConstant(0x80100019)]
    public static HResult SCARD_E_PCI_TOO_SMALL => new(0x80100019);

    /// <summary>The reader driver does not meet minimal requirements for support.</summary>
    [Description("""The reader driver does not meet minimal requirements for support.""")]
    [HResultConstant(0x8010001A)]
    public static HResult SCARD_E_READER_UNSUPPORTED => new(0x8010001A);

    /// <summary>The reader driver did not produce a unique reader name.</summary>
    [Description("""The reader driver did not produce a unique reader name.""")]
    [HResultConstant(0x8010001B)]
    public static HResult SCARD_E_DUPLICATE_READER => new(0x8010001B);

    /// <summary>The smart card does not meet minimal requirements for support.</summary>
    [Description("""The smart card does not meet minimal requirements for support.""")]
    [HResultConstant(0x8010001C)]
    public static HResult SCARD_E_CARD_UNSUPPORTED => new(0x8010001C);

    /// <summary>The smart card resource manager is not running.</summary>
    [Description("""The smart card resource manager is not running.""")]
    [HResultConstant(0x8010001D)]
    public static HResult SCARD_E_NO_SERVICE => new(0x8010001D);

    /// <summary>The smart card resource manager has shut down.</summary>
    [Description("""The smart card resource manager has shut down.""")]
    [HResultConstant(0x8010001E)]
    public static HResult SCARD_E_SERVICE_STOPPED => new(0x8010001E);

    /// <summary>An unexpected card error has occurred.</summary>
    [Description("""An unexpected card error has occurred.""")]
    [HResultConstant(0x8010001F)]
    public static HResult SCARD_E_UNEXPECTED => new(0x8010001F);

    /// <summary>No primary provider can be found for the smart card.</summary>
    [Description("""No primary provider can be found for the smart card.""")]
    [HResultConstant(0x80100020)]
    public static HResult SCARD_E_ICC_INSTALLATION => new(0x80100020);

    /// <summary>The requested order of object creation is not supported.</summary>
    [Description("""The requested order of object creation is not supported.""")]
    [HResultConstant(0x80100021)]
    public static HResult SCARD_E_ICC_CREATEORDER => new(0x80100021);

    /// <summary>This smart card does not support the requested feature.</summary>
    [Description("""This smart card does not support the requested feature.""")]
    [HResultConstant(0x80100022)]
    public static HResult SCARD_E_UNSUPPORTED_FEATURE => new(0x80100022);

    /// <summary>The identified directory does not exist in the smart card.</summary>
    [Description("""The identified directory does not exist in the smart card.""")]
    [HResultConstant(0x80100023)]
    public static HResult SCARD_E_DIR_NOT_FOUND => new(0x80100023);

    /// <summary>The identified file does not exist in the smart card.</summary>
    [Description("""The identified file does not exist in the smart card.""")]
    [HResultConstant(0x80100024)]
    public static HResult SCARD_E_FILE_NOT_FOUND => new(0x80100024);

    /// <summary>The supplied path does not represent a smart card directory.</summary>
    [Description("""The supplied path does not represent a smart card directory.""")]
    [HResultConstant(0x80100025)]
    public static HResult SCARD_E_NO_DIR => new(0x80100025);

    /// <summary>The supplied path does not represent a smart card file.</summary>
    [Description("""The supplied path does not represent a smart card file.""")]
    [HResultConstant(0x80100026)]
    public static HResult SCARD_E_NO_FILE => new(0x80100026);

    /// <summary>Access is denied to this file.</summary>
    [Description("""Access is denied to this file.""")]
    [HResultConstant(0x80100027)]
    public static HResult SCARD_E_NO_ACCESS => new(0x80100027);

    /// <summary>The smart card does not have enough memory to store the information.</summary>
    [Description("""The smart card does not have enough memory to store the information.""")]
    [HResultConstant(0x80100028)]
    public static HResult SCARD_E_WRITE_TOO_MANY => new(0x80100028);

    /// <summary>There was an error trying to set the smart card file object pointer.</summary>
    [Description("""There was an error trying to set the smart card file object pointer.""")]
    [HResultConstant(0x80100029)]
    public static HResult SCARD_E_BAD_SEEK => new(0x80100029);

    /// <summary>The supplied PIN is incorrect.</summary>
    [Description("""The supplied PIN is incorrect.""")]
    [HResultConstant(0x8010002A)]
    public static HResult SCARD_E_INVALID_CHV => new(0x8010002A);

    /// <summary>An unrecognized error code was returned from a layered component.</summary>
    [Description("""An unrecognized error code was returned from a layered component.""")]
    [HResultConstant(0x8010002B)]
    public static HResult SCARD_E_UNKNOWN_RES_MNG => new(0x8010002B);

    /// <summary>The requested certificate does not exist.</summary>
    [Description("""The requested certificate does not exist.""")]
    [HResultConstant(0x8010002C)]
    public static HResult SCARD_E_NO_SUCH_CERTIFICATE => new(0x8010002C);

    /// <summary>The requested certificate could not be obtained.</summary>
    [Description("""The requested certificate could not be obtained.""")]
    [HResultConstant(0x8010002D)]
    public static HResult SCARD_E_CERTIFICATE_UNAVAILABLE => new(0x8010002D);

    /// <summary>Cannot find a smart card reader.</summary>
    [Description("""Cannot find a smart card reader.""")]
    [HResultConstant(0x8010002E)]
    public static HResult SCARD_E_NO_READERS_AVAILABLE => new(0x8010002E);

    /// <summary>A communications error with the smart card has been detected. Retry the operation.</summary>
    [Description("""A communications error with the smart card has been detected. Retry the operation.""")]
    [HResultConstant(0x8010002F)]
    public static HResult SCARD_E_COMM_DATA_LOST => new(0x8010002F);

    /// <summary>The requested key container does not exist on the smart card.</summary>
    [Description("""The requested key container does not exist on the smart card.""")]
    [HResultConstant(0x80100030)]
    public static HResult SCARD_E_NO_KEY_CONTAINER => new(0x80100030);

    /// <summary>The smart card resource manager is too busy to complete this operation.</summary>
    [Description("""The smart card resource manager is too busy to complete this operation.""")]
    [HResultConstant(0x80100031)]
    public static HResult SCARD_E_SERVER_TOO_BUSY => new(0x80100031);

    /// <summary>The reader cannot communicate with the smart card, due to ATR configuration conflicts.</summary>
    [Description("""The reader cannot communicate with the smart card, due to ATR configuration conflicts.""")]
    [HResultConstant(0x80100065)]
    public static HResult SCARD_W_UNSUPPORTED_CARD => new(0x80100065);

    /// <summary>The smart card is not responding to a reset.</summary>
    [Description("""The smart card is not responding to a reset.""")]
    [HResultConstant(0x80100066)]
    public static HResult SCARD_W_UNRESPONSIVE_CARD => new(0x80100066);

    /// <summary>Power has been removed from the smart card, so that further communication is not possible.</summary>
    [Description("""Power has been removed from the smart card, so that further communication is not possible.""")]
    [HResultConstant(0x80100067)]
    public static HResult SCARD_W_UNPOWERED_CARD => new(0x80100067);

    /// <summary>The smart card has been reset, so any shared state information is invalid.</summary>
    [Description("""The smart card has been reset, so any shared state information is invalid.""")]
    [HResultConstant(0x80100068)]
    public static HResult SCARD_W_RESET_CARD => new(0x80100068);

    /// <summary>The smart card has been removed, so that further communication is not possible.</summary>
    [Description("""The smart card has been removed, so that further communication is not possible.""")]
    [HResultConstant(0x80100069)]
    public static HResult SCARD_W_REMOVED_CARD => new(0x80100069);

    /// <summary>Access was denied because of a security violation.</summary>
    [Description("""Access was denied because of a security violation.""")]
    [HResultConstant(0x8010006A)]
    public static HResult SCARD_W_SECURITY_VIOLATION => new(0x8010006A);

    /// <summary>The card cannot be accessed because the wrong PIN was presented.</summary>
    [Description("""The card cannot be accessed because the wrong PIN was presented.""")]
    [HResultConstant(0x8010006B)]
    public static HResult SCARD_W_WRONG_CHV => new(0x8010006B);

    /// <summary>The card cannot be accessed because the maximum number of PIN entry attempts has been reached.</summary>
    [Description("""The card cannot be accessed because the maximum number of PIN entry attempts has been reached.""")]
    [HResultConstant(0x8010006C)]
    public static HResult SCARD_W_CHV_BLOCKED => new(0x8010006C);

    /// <summary>The end of the smart card file has been reached.</summary>
    [Description("""The end of the smart card file has been reached.""")]
    [HResultConstant(0x8010006D)]
    public static HResult SCARD_W_EOF => new(0x8010006D);

    /// <summary>The action was canceled by the user.</summary>
    [Description("""The action was canceled by the user.""")]
    [HResultConstant(0x8010006E)]
    public static HResult SCARD_W_CANCELLED_BY_USER => new(0x8010006E);

    /// <summary>No PIN was presented to the smart card.</summary>
    [Description("""No PIN was presented to the smart card.""")]
    [HResultConstant(0x8010006F)]
    public static HResult SCARD_W_CARD_NOT_AUTHENTICATED => new(0x8010006F);

    /// <summary>Errors occurred accessing one or more objects—the ErrorInfo collection contains more detail.</summary>
    [Description("""Errors occurred accessing one or more objects—the ErrorInfo collection contains more detail.""")]
    [HResultConstant(0x80110401)]
    public static HResult COMADMIN_E_OBJECTERRORS => new(0x80110401);

    /// <summary>One or more of the object's properties are missing or invalid.</summary>
    [Description("""One or more of the object's properties are missing or invalid.""")]
    [HResultConstant(0x80110402)]
    public static HResult COMADMIN_E_OBJECTINVALID => new(0x80110402);

    /// <summary>The object was not found in the catalog.</summary>
    [Description("""The object was not found in the catalog.""")]
    [HResultConstant(0x80110403)]
    public static HResult COMADMIN_E_KEYMISSING => new(0x80110403);

    /// <summary>The object is already registered.</summary>
    [Description("""The object is already registered.""")]
    [HResultConstant(0x80110404)]
    public static HResult COMADMIN_E_ALREADYINSTALLED => new(0x80110404);

    /// <summary>An error occurred writing to the application file.</summary>
    [Description("""An error occurred writing to the application file.""")]
    [HResultConstant(0x80110407)]
    public static HResult COMADMIN_E_APP_FILE_WRITEFAIL => new(0x80110407);

    /// <summary>An error occurred reading the application file.</summary>
    [Description("""An error occurred reading the application file.""")]
    [HResultConstant(0x80110408)]
    public static HResult COMADMIN_E_APP_FILE_READFAIL => new(0x80110408);

    /// <summary>Invalid version number in application file.</summary>
    [Description("""Invalid version number in application file.""")]
    [HResultConstant(0x80110409)]
    public static HResult COMADMIN_E_APP_FILE_VERSION => new(0x80110409);

    /// <summary>The file path is invalid.</summary>
    [Description("""The file path is invalid.""")]
    [HResultConstant(0x8011040A)]
    public static HResult COMADMIN_E_BADPATH => new(0x8011040A);

    /// <summary>The application is already installed.</summary>
    [Description("""The application is already installed.""")]
    [HResultConstant(0x8011040B)]
    public static HResult COMADMIN_E_APPLICATIONEXISTS => new(0x8011040B);

    /// <summary>The role already exists.</summary>
    [Description("""The role already exists.""")]
    [HResultConstant(0x8011040C)]
    public static HResult COMADMIN_E_ROLEEXISTS => new(0x8011040C);

    /// <summary>An error occurred copying the file.</summary>
    [Description("""An error occurred copying the file.""")]
    [HResultConstant(0x8011040D)]
    public static HResult COMADMIN_E_CANTCOPYFILE => new(0x8011040D);

    /// <summary>One or more users are not valid.</summary>
    [Description("""One or more users are not valid.""")]
    [HResultConstant(0x8011040F)]
    public static HResult COMADMIN_E_NOUSER => new(0x8011040F);

    /// <summary>One or more users in the application file are not valid.</summary>
    [Description("""One or more users in the application file are not valid.""")]
    [HResultConstant(0x80110410)]
    public static HResult COMADMIN_E_INVALIDUSERIDS => new(0x80110410);

    /// <summary>The component's CLSID is missing or corrupt.</summary>
    [Description("""The component's CLSID is missing or corrupt.""")]
    [HResultConstant(0x80110411)]
    public static HResult COMADMIN_E_NOREGISTRYCLSID => new(0x80110411);

    /// <summary>The component's programmatic ID is missing or corrupt.</summary>
    [Description("""The component's programmatic ID is missing or corrupt.""")]
    [HResultConstant(0x80110412)]
    public static HResult COMADMIN_E_BADREGISTRYPROGID => new(0x80110412);

    /// <summary>Unable to set required authentication level for update request.</summary>
    [Description("""Unable to set required authentication level for update request.""")]
    [HResultConstant(0x80110413)]
    public static HResult COMADMIN_E_AUTHENTICATIONLEVEL => new(0x80110413);

    /// <summary>The identity or password set on the application is not valid.</summary>
    [Description("""The identity or password set on the application is not valid.""")]
    [HResultConstant(0x80110414)]
    public static HResult COMADMIN_E_USERPASSWDNOTVALID => new(0x80110414);

    /// <summary>Application file CLSIDs or instance identifiers (IIDs) do not match corresponding DLLs.</summary>
    [Description("""Application file CLSIDs or instance identifiers (IIDs) do not match corresponding DLLs.""")]
    [HResultConstant(0x80110418)]
    public static HResult COMADMIN_E_CLSIDORIIDMISMATCH => new(0x80110418);

    /// <summary>Interface information is either missing or changed.</summary>
    [Description("""Interface information is either missing or changed.""")]
    [HResultConstant(0x80110419)]
    public static HResult COMADMIN_E_REMOTEINTERFACE => new(0x80110419);

    /// <summary>DllRegisterServer failed on component install.</summary>
    [Description("""DllRegisterServer failed on component install.""")]
    [HResultConstant(0x8011041A)]
    public static HResult COMADMIN_E_DLLREGISTERSERVER => new(0x8011041A);

    /// <summary>No server file share available.</summary>
    [Description("""No server file share available.""")]
    [HResultConstant(0x8011041B)]
    public static HResult COMADMIN_E_NOSERVERSHARE => new(0x8011041B);

    /// <summary>DLL could not be loaded.</summary>
    [Description("""DLL could not be loaded.""")]
    [HResultConstant(0x8011041D)]
    public static HResult COMADMIN_E_DLLLOADFAILED => new(0x8011041D);

    /// <summary>The registered TypeLib ID is not valid.</summary>
    [Description("""The registered TypeLib ID is not valid.""")]
    [HResultConstant(0x8011041E)]
    public static HResult COMADMIN_E_BADREGISTRYLIBID => new(0x8011041E);

    /// <summary>Application install directory not found.</summary>
    [Description("""Application install directory not found.""")]
    [HResultConstant(0x8011041F)]
    public static HResult COMADMIN_E_APPDIRNOTFOUND => new(0x8011041F);

    /// <summary>Errors occurred while in the component registrar.</summary>
    [Description("""Errors occurred while in the component registrar.""")]
    [HResultConstant(0x80110423)]
    public static HResult COMADMIN_E_REGISTRARFAILED => new(0x80110423);

    /// <summary>The file does not exist.</summary>
    [Description("""The file does not exist.""")]
    [HResultConstant(0x80110424)]
    public static HResult COMADMIN_E_COMPFILE_DOESNOTEXIST => new(0x80110424);

    /// <summary>The DLL could not be loaded.</summary>
    [Description("""The DLL could not be loaded.""")]
    [HResultConstant(0x80110425)]
    public static HResult COMADMIN_E_COMPFILE_LOADDLLFAIL => new(0x80110425);

    /// <summary>GetClassObject failed in the DLL.</summary>
    [Description("""GetClassObject failed in the DLL.""")]
    [HResultConstant(0x80110426)]
    public static HResult COMADMIN_E_COMPFILE_GETCLASSOBJ => new(0x80110426);

    /// <summary>The DLL does not support the components listed in the TypeLib.</summary>
    [Description("""The DLL does not support the components listed in the TypeLib.""")]
    [HResultConstant(0x80110427)]
    public static HResult COMADMIN_E_COMPFILE_CLASSNOTAVAIL => new(0x80110427);

    /// <summary>The TypeLib could not be loaded.</summary>
    [Description("""The TypeLib could not be loaded.""")]
    [HResultConstant(0x80110428)]
    public static HResult COMADMIN_E_COMPFILE_BADTLB => new(0x80110428);

    /// <summary>The file does not contain components or component information.</summary>
    [Description("""The file does not contain components or component information.""")]
    [HResultConstant(0x80110429)]
    public static HResult COMADMIN_E_COMPFILE_NOTINSTALLABLE => new(0x80110429);

    /// <summary>Changes to this object and its subobjects have been disabled.</summary>
    [Description("""Changes to this object and its subobjects have been disabled.""")]
    [HResultConstant(0x8011042A)]
    public static HResult COMADMIN_E_NOTCHANGEABLE => new(0x8011042A);

    /// <summary>The delete function has been disabled for this object.</summary>
    [Description("""The delete function has been disabled for this object.""")]
    [HResultConstant(0x8011042B)]
    public static HResult COMADMIN_E_NOTDELETEABLE => new(0x8011042B);

    /// <summary>The server catalog version is not supported.</summary>
    [Description("""The server catalog version is not supported.""")]
    [HResultConstant(0x8011042C)]
    public static HResult COMADMIN_E_SESSION => new(0x8011042C);

    /// <summary>The component move was disallowed because the source or destination application is either a system application or currently locked against changes.</summary>
    [Description("""The component move was disallowed because the source or destination application is either a system application or currently locked against changes.""")]
    [HResultConstant(0x8011042D)]
    public static HResult COMADMIN_E_COMP_MOVE_LOCKED => new(0x8011042D);

    /// <summary>The component move failed because the destination application no longer exists.</summary>
    [Description("""The component move failed because the destination application no longer exists.""")]
    [HResultConstant(0x8011042E)]
    public static HResult COMADMIN_E_COMP_MOVE_BAD_DEST => new(0x8011042E);

    /// <summary>The system was unable to register the TypeLib.</summary>
    [Description("""The system was unable to register the TypeLib.""")]
    [HResultConstant(0x80110430)]
    public static HResult COMADMIN_E_REGISTERTLB => new(0x80110430);

    /// <summary>This operation cannot be performed on the system application.</summary>
    [Description("""This operation cannot be performed on the system application.""")]
    [HResultConstant(0x80110433)]
    public static HResult COMADMIN_E_SYSTEMAPP => new(0x80110433);

    /// <summary>The component registrar referenced in this file is not available.</summary>
    [Description("""The component registrar referenced in this file is not available.""")]
    [HResultConstant(0x80110434)]
    public static HResult COMADMIN_E_COMPFILE_NOREGISTRAR => new(0x80110434);

    /// <summary>A component in the same DLL is already installed.</summary>
    [Description("""A component in the same DLL is already installed.""")]
    [HResultConstant(0x80110435)]
    public static HResult COMADMIN_E_COREQCOMPINSTALLED => new(0x80110435);

    /// <summary>The service is not installed.</summary>
    [Description("""The service is not installed.""")]
    [HResultConstant(0x80110436)]
    public static HResult COMADMIN_E_SERVICENOTINSTALLED => new(0x80110436);

    /// <summary>One or more property settings are either invalid or in conflict with each other.</summary>
    [Description("""One or more property settings are either invalid or in conflict with each other.""")]
    [HResultConstant(0x80110437)]
    public static HResult COMADMIN_E_PROPERTYSAVEFAILED => new(0x80110437);

    /// <summary>The object you are attempting to add or rename already exists.</summary>
    [Description("""The object you are attempting to add or rename already exists.""")]
    [HResultConstant(0x80110438)]
    public static HResult COMADMIN_E_OBJECTEXISTS => new(0x80110438);

    /// <summary>The component already exists.</summary>
    [Description("""The component already exists.""")]
    [HResultConstant(0x80110439)]
    public static HResult COMADMIN_E_COMPONENTEXISTS => new(0x80110439);

    /// <summary>The registration file is corrupt.</summary>
    [Description("""The registration file is corrupt.""")]
    [HResultConstant(0x8011043B)]
    public static HResult COMADMIN_E_REGFILE_CORRUPT => new(0x8011043B);

    /// <summary>The property value is too large.</summary>
    [Description("""The property value is too large.""")]
    [HResultConstant(0x8011043C)]
    public static HResult COMADMIN_E_PROPERTY_OVERFLOW => new(0x8011043C);

    /// <summary>Object was not found in registry.</summary>
    [Description("""Object was not found in registry.""")]
    [HResultConstant(0x8011043E)]
    public static HResult COMADMIN_E_NOTINREGISTRY => new(0x8011043E);

    /// <summary>This object cannot be pooled.</summary>
    [Description("""This object cannot be pooled.""")]
    [HResultConstant(0x8011043F)]
    public static HResult COMADMIN_E_OBJECTNOTPOOLABLE => new(0x8011043F);

    /// <summary>A CLSID with the same GUID as the new application ID is already installed on this machine.</summary>
    [Description("""A CLSID with the same GUID as the new application ID is already installed on this machine.""")]
    [HResultConstant(0x80110446)]
    public static HResult COMADMIN_E_APPLID_MATCHES_CLSID => new(0x80110446);

    /// <summary>A role assigned to a component, interface, or method did not exist in the application.</summary>
    [Description("""A role assigned to a component, interface, or method did not exist in the application.""")]
    [HResultConstant(0x80110447)]
    public static HResult COMADMIN_E_ROLE_DOES_NOT_EXIST => new(0x80110447);

    /// <summary>You must have components in an application to start the application.</summary>
    [Description("""You must have components in an application to start the application.""")]
    [HResultConstant(0x80110448)]
    public static HResult COMADMIN_E_START_APP_NEEDS_COMPONENTS => new(0x80110448);

    /// <summary>This operation is not enabled on this platform.</summary>
    [Description("""This operation is not enabled on this platform.""")]
    [HResultConstant(0x80110449)]
    public static HResult COMADMIN_E_REQUIRES_DIFFERENT_PLATFORM => new(0x80110449);

    /// <summary>Application proxy is not exportable.</summary>
    [Description("""Application proxy is not exportable.""")]
    [HResultConstant(0x8011044A)]
    public static HResult COMADMIN_E_CAN_NOT_EXPORT_APP_PROXY => new(0x8011044A);

    /// <summary>Failed to start application because it is either a library application or an application proxy.</summary>
    [Description("""Failed to start application because it is either a library application or an application proxy.""")]
    [HResultConstant(0x8011044B)]
    public static HResult COMADMIN_E_CAN_NOT_START_APP => new(0x8011044B);

    /// <summary>System application is not exportable.</summary>
    [Description("""System application is not exportable.""")]
    [HResultConstant(0x8011044C)]
    public static HResult COMADMIN_E_CAN_NOT_EXPORT_SYS_APP => new(0x8011044C);

    /// <summary>Cannot subscribe to this component (the component might have been imported).</summary>
    [Description("""Cannot subscribe to this component (the component might have been imported).""")]
    [HResultConstant(0x8011044D)]
    public static HResult COMADMIN_E_CANT_SUBSCRIBE_TO_COMPONENT => new(0x8011044D);

    /// <summary>An event class cannot also be a subscriber component.</summary>
    [Description("""An event class cannot also be a subscriber component.""")]
    [HResultConstant(0x8011044E)]
    public static HResult COMADMIN_E_EVENTCLASS_CANT_BE_SUBSCRIBER => new(0x8011044E);

    /// <summary>Library applications and application proxies are incompatible.</summary>
    [Description("""Library applications and application proxies are incompatible.""")]
    [HResultConstant(0x8011044F)]
    public static HResult COMADMIN_E_LIB_APP_PROXY_INCOMPATIBLE => new(0x8011044F);

    /// <summary>This function is valid for the base partition only.</summary>
    [Description("""This function is valid for the base partition only.""")]
    [HResultConstant(0x80110450)]
    public static HResult COMADMIN_E_BASE_PARTITION_ONLY => new(0x80110450);

    /// <summary>You cannot start an application that has been disabled.</summary>
    [Description("""You cannot start an application that has been disabled.""")]
    [HResultConstant(0x80110451)]
    public static HResult COMADMIN_E_START_APP_DISABLED => new(0x80110451);

    /// <summary>The specified partition name is already in use on this computer.</summary>
    [Description("""The specified partition name is already in use on this computer.""")]
    [HResultConstant(0x80110457)]
    public static HResult COMADMIN_E_CAT_DUPLICATE_PARTITION_NAME => new(0x80110457);

    /// <summary>The specified partition name is invalid. Check that the name contains at least one visible character.</summary>
    [Description("""The specified partition name is invalid. Check that the name contains at least one visible character.""")]
    [HResultConstant(0x80110458)]
    public static HResult COMADMIN_E_CAT_INVALID_PARTITION_NAME => new(0x80110458);

    /// <summary>The partition cannot be deleted because it is the default partition for one or more users.</summary>
    [Description("""The partition cannot be deleted because it is the default partition for one or more users.""")]
    [HResultConstant(0x80110459)]
    public static HResult COMADMIN_E_CAT_PARTITION_IN_USE => new(0x80110459);

    /// <summary>The partition cannot be exported because one or more components in the partition have the same file name.</summary>
    [Description("""The partition cannot be exported because one or more components in the partition have the same file name.""")]
    [HResultConstant(0x8011045A)]
    public static HResult COMADMIN_E_FILE_PARTITION_DUPLICATE_FILES => new(0x8011045A);

    /// <summary>Applications that contain one or more imported components cannot be installed into a nonbase partition.</summary>
    [Description("""Applications that contain one or more imported components cannot be installed into a nonbase partition.""")]
    [HResultConstant(0x8011045B)]
    public static HResult COMADMIN_E_CAT_IMPORTED_COMPONENTS_NOT_ALLOWED => new(0x8011045B);

    /// <summary>The application name is not unique and cannot be resolved to an application ID.</summary>
    [Description("""The application name is not unique and cannot be resolved to an application ID.""")]
    [HResultConstant(0x8011045C)]
    public static HResult COMADMIN_E_AMBIGUOUS_APPLICATION_NAME => new(0x8011045C);

    /// <summary>The partition name is not unique and cannot be resolved to a partition ID.</summary>
    [Description("""The partition name is not unique and cannot be resolved to a partition ID.""")]
    [HResultConstant(0x8011045D)]
    public static HResult COMADMIN_E_AMBIGUOUS_PARTITION_NAME => new(0x8011045D);

    /// <summary>The COM+ registry database has not been initialized.</summary>
    [Description("""The COM+ registry database has not been initialized.""")]
    [HResultConstant(0x80110472)]
    public static HResult COMADMIN_E_REGDB_NOTINITIALIZED => new(0x80110472);

    /// <summary>The COM+ registry database is not open.</summary>
    [Description("""The COM+ registry database is not open.""")]
    [HResultConstant(0x80110473)]
    public static HResult COMADMIN_E_REGDB_NOTOPEN => new(0x80110473);

    /// <summary>The COM+ registry database detected a system error.</summary>
    [Description("""The COM+ registry database detected a system error.""")]
    [HResultConstant(0x80110474)]
    public static HResult COMADMIN_E_REGDB_SYSTEMERR => new(0x80110474);

    /// <summary>The COM+ registry database is already running.</summary>
    [Description("""The COM+ registry database is already running.""")]
    [HResultConstant(0x80110475)]
    public static HResult COMADMIN_E_REGDB_ALREADYRUNNING => new(0x80110475);

    /// <summary>This version of the COM+ registry database cannot be migrated.</summary>
    [Description("""This version of the COM+ registry database cannot be migrated.""")]
    [HResultConstant(0x80110480)]
    public static HResult COMADMIN_E_MIG_VERSIONNOTSUPPORTED => new(0x80110480);

    /// <summary>The schema version to be migrated could not be found in the COM+ registry database.</summary>
    [Description("""The schema version to be migrated could not be found in the COM+ registry database.""")]
    [HResultConstant(0x80110481)]
    public static HResult COMADMIN_E_MIG_SCHEMANOTFOUND => new(0x80110481);

    /// <summary>There was a type mismatch between binaries.</summary>
    [Description("""There was a type mismatch between binaries.""")]
    [HResultConstant(0x80110482)]
    public static HResult COMADMIN_E_CAT_BITNESSMISMATCH => new(0x80110482);

    /// <summary>A binary of unknown or invalid type was provided.</summary>
    [Description("""A binary of unknown or invalid type was provided.""")]
    [HResultConstant(0x80110483)]
    public static HResult COMADMIN_E_CAT_UNACCEPTABLEBITNESS => new(0x80110483);

    /// <summary>There was a type mismatch between a binary and an application.</summary>
    [Description("""There was a type mismatch between a binary and an application.""")]
    [HResultConstant(0x80110484)]
    public static HResult COMADMIN_E_CAT_WRONGAPPBITNESS => new(0x80110484);

    /// <summary>The application cannot be paused or resumed.</summary>
    [Description("""The application cannot be paused or resumed.""")]
    [HResultConstant(0x80110485)]
    public static HResult COMADMIN_E_CAT_PAUSE_RESUME_NOT_SUPPORTED => new(0x80110485);

    /// <summary>The COM+ catalog server threw an exception during execution.</summary>
    [Description("""The COM+ catalog server threw an exception during execution.""")]
    [HResultConstant(0x80110486)]
    public static HResult COMADMIN_E_CAT_SERVERFAULT => new(0x80110486);

    /// <summary>Only COM+ applications marked "queued" can be invoked using the "queue" moniker.</summary>
    [Description("""Only COM+ applications marked "queued" can be invoked using the "queue" moniker.""")]
    [HResultConstant(0x80110600)]
    public static HResult COMQC_E_APPLICATION_NOT_QUEUED => new(0x80110600);

    /// <summary>At least one interface must be marked "queued" to create a queued component instance with the "queue" moniker.</summary>
    [Description("""At least one interface must be marked "queued" to create a queued component instance with the "queue" moniker.""")]
    [HResultConstant(0x80110601)]
    public static HResult COMQC_E_NO_QUEUEABLE_INTERFACES => new(0x80110601);

    /// <summary>Message Queuing is required for the requested operation and is not installed.</summary>
    [Description("""Message Queuing is required for the requested operation and is not installed.""")]
    [HResultConstant(0x80110602)]
    public static HResult COMQC_E_QUEUING_SERVICE_NOT_AVAILABLE => new(0x80110602);

    /// <summary>Unable to marshal an interface that does not support IPersistStream.</summary>
    [Description("""Unable to marshal an interface that does not support IPersistStream.""")]
    [HResultConstant(0x80110603)]
    public static HResult COMQC_E_NO_IPERSISTSTREAM => new(0x80110603);

    /// <summary>The message is improperly formatted or was damaged in transit.</summary>
    [Description("""The message is improperly formatted or was damaged in transit.""")]
    [HResultConstant(0x80110604)]
    public static HResult COMQC_E_BAD_MESSAGE => new(0x80110604);

    /// <summary>An unauthenticated message was received by an application that accepts only authenticated messages.</summary>
    [Description("""An unauthenticated message was received by an application that accepts only authenticated messages.""")]
    [HResultConstant(0x80110605)]
    public static HResult COMQC_E_UNAUTHENTICATED => new(0x80110605);

    /// <summary>The message was requeued or moved by a user not in the QC Trusted User "role".</summary>
    [Description("""The message was requeued or moved by a user not in the QC Trusted User "role".""")]
    [HResultConstant(0x80110606)]
    public static HResult COMQC_E_UNTRUSTED_ENQUEUER => new(0x80110606);

    /// <summary>Cannot create a duplicate resource of type Distributed Transaction Coordinator.</summary>
    [Description("""Cannot create a duplicate resource of type Distributed Transaction Coordinator.""")]
    [HResultConstant(0x80110701)]
    public static HResult MSDTC_E_DUPLICATE_RESOURCE => new(0x80110701);

    /// <summary>One of the objects being inserted or updated does not belong to a valid parent collection.</summary>
    [Description("""One of the objects being inserted or updated does not belong to a valid parent collection.""")]
    [HResultConstant(0x80110808)]
    public static HResult COMADMIN_E_OBJECT_PARENT_MISSING => new(0x80110808);

    /// <summary>One of the specified objects cannot be found.</summary>
    [Description("""One of the specified objects cannot be found.""")]
    [HResultConstant(0x80110809)]
    public static HResult COMADMIN_E_OBJECT_DOES_NOT_EXIST => new(0x80110809);

    /// <summary>The specified application is not currently running.</summary>
    [Description("""The specified application is not currently running.""")]
    [HResultConstant(0x8011080A)]
    public static HResult COMADMIN_E_APP_NOT_RUNNING => new(0x8011080A);

    /// <summary>The partitions specified are not valid.</summary>
    [Description("""The partitions specified are not valid.""")]
    [HResultConstant(0x8011080B)]
    public static HResult COMADMIN_E_INVALID_PARTITION => new(0x8011080B);

    /// <summary>COM+ applications that run as Windows NT service cannot be pooled or recycled.</summary>
    [Description("""COM+ applications that run as Windows NT service cannot be pooled or recycled.""")]
    [HResultConstant(0x8011080D)]
    public static HResult COMADMIN_E_SVCAPP_NOT_POOLABLE_OR_RECYCLABLE => new(0x8011080D);

    /// <summary>One or more users are already assigned to a local partition set.</summary>
    [Description("""One or more users are already assigned to a local partition set.""")]
    [HResultConstant(0x8011080E)]
    public static HResult COMADMIN_E_USER_IN_SET => new(0x8011080E);

    /// <summary>Library applications cannot be recycled.</summary>
    [Description("""Library applications cannot be recycled.""")]
    [HResultConstant(0x8011080F)]
    public static HResult COMADMIN_E_CANTRECYCLELIBRARYAPPS => new(0x8011080F);

    /// <summary>Applications running as Windows NT services cannot be recycled.</summary>
    [Description("""Applications running as Windows NT services cannot be recycled.""")]
    [HResultConstant(0x80110811)]
    public static HResult COMADMIN_E_CANTRECYCLESERVICEAPPS => new(0x80110811);

    /// <summary>The process has already been recycled.</summary>
    [Description("""The process has already been recycled.""")]
    [HResultConstant(0x80110812)]
    public static HResult COMADMIN_E_PROCESSALREADYRECYCLED => new(0x80110812);

    /// <summary>A paused process cannot be recycled.</summary>
    [Description("""A paused process cannot be recycled.""")]
    [HResultConstant(0x80110813)]
    public static HResult COMADMIN_E_PAUSEDPROCESSMAYNOTBERECYCLED => new(0x80110813);

    /// <summary>Library applications cannot be Windows NT services.</summary>
    [Description("""Library applications cannot be Windows NT services.""")]
    [HResultConstant(0x80110814)]
    public static HResult COMADMIN_E_CANTMAKEINPROCSERVICE => new(0x80110814);

    /// <summary>The ProgID provided to the copy operation is invalid. The ProgID is in use by another registered CLSID.</summary>
    [Description("""The ProgID provided to the copy operation is invalid. The ProgID is in use by another registered CLSID.""")]
    [HResultConstant(0x80110815)]
    public static HResult COMADMIN_E_PROGIDINUSEBYCLSID => new(0x80110815);

    /// <summary>The partition specified as the default is not a member of the partition set.</summary>
    [Description("""The partition specified as the default is not a member of the partition set.""")]
    [HResultConstant(0x80110816)]
    public static HResult COMADMIN_E_DEFAULT_PARTITION_NOT_IN_SET => new(0x80110816);

    /// <summary>A recycled process cannot be paused.</summary>
    [Description("""A recycled process cannot be paused.""")]
    [HResultConstant(0x80110817)]
    public static HResult COMADMIN_E_RECYCLEDPROCESSMAYNOTBEPAUSED => new(0x80110817);

    /// <summary>Access to the specified partition is denied.</summary>
    [Description("""Access to the specified partition is denied.""")]
    [HResultConstant(0x80110818)]
    public static HResult COMADMIN_E_PARTITION_ACCESSDENIED => new(0x80110818);

    /// <summary>Only application files (*.msi files) can be installed into partitions.</summary>
    [Description("""Only application files (*.msi files) can be installed into partitions.""")]
    [HResultConstant(0x80110819)]
    public static HResult COMADMIN_E_PARTITION_MSI_ONLY => new(0x80110819);

    /// <summary>Applications containing one or more legacy components cannot be exported to 1.0 format.</summary>
    [Description("""Applications containing one or more legacy components cannot be exported to 1.0 format.""")]
    [HResultConstant(0x8011081A)]
    public static HResult COMADMIN_E_LEGACYCOMPS_NOT_ALLOWED_IN_1_0_FORMAT => new(0x8011081A);

    /// <summary>Legacy components cannot exist in nonbase partitions.</summary>
    [Description("""Legacy components cannot exist in nonbase partitions.""")]
    [HResultConstant(0x8011081B)]
    public static HResult COMADMIN_E_LEGACYCOMPS_NOT_ALLOWED_IN_NONBASE_PARTITIONS => new(0x8011081B);

    /// <summary>A component cannot be moved (or copied) from the System Application, an application proxy, or a nonchangeable application.</summary>
    [Description("""A component cannot be moved (or copied) from the System Application, an application proxy, or a nonchangeable application.""")]
    [HResultConstant(0x8011081C)]
    public static HResult COMADMIN_E_COMP_MOVE_SOURCE => new(0x8011081C);

    /// <summary>A component cannot be moved (or copied) to the System Application, an application proxy or a nonchangeable application.</summary>
    [Description("""A component cannot be moved (or copied) to the System Application, an application proxy or a nonchangeable application.""")]
    [HResultConstant(0x8011081D)]
    public static HResult COMADMIN_E_COMP_MOVE_DEST => new(0x8011081D);

    /// <summary>A private component cannot be moved (or copied) to a library application or to the base partition.</summary>
    [Description("""A private component cannot be moved (or copied) to a library application or to the base partition.""")]
    [HResultConstant(0x8011081E)]
    public static HResult COMADMIN_E_COMP_MOVE_PRIVATE => new(0x8011081E);

    /// <summary>The Base Application Partition exists in all partition sets and cannot be removed.</summary>
    [Description("""The Base Application Partition exists in all partition sets and cannot be removed.""")]
    [HResultConstant(0x8011081F)]
    public static HResult COMADMIN_E_BASEPARTITION_REQUIRED_IN_SET => new(0x8011081F);

    /// <summary>Alas, Event Class components cannot be aliased.</summary>
    [Description("""Alas, Event Class components cannot be aliased.""")]
    [HResultConstant(0x80110820)]
    public static HResult COMADMIN_E_CANNOT_ALIAS_EVENTCLASS => new(0x80110820);

    /// <summary>Access is denied because the component is private.</summary>
    [Description("""Access is denied because the component is private.""")]
    [HResultConstant(0x80110821)]
    public static HResult COMADMIN_E_PRIVATE_ACCESSDENIED => new(0x80110821);

    /// <summary>The specified SAFER level is invalid.</summary>
    [Description("""The specified SAFER level is invalid.""")]
    [HResultConstant(0x80110822)]
    public static HResult COMADMIN_E_SAFERINVALID => new(0x80110822);

    /// <summary>The specified user cannot write to the system registry.</summary>
    [Description("""The specified user cannot write to the system registry.""")]
    [HResultConstant(0x80110823)]
    public static HResult COMADMIN_E_REGISTRY_ACCESSDENIED => new(0x80110823);

    /// <summary>COM+ partitions are currently disabled.</summary>
    [Description("""COM+ partitions are currently disabled.""")]
    [HResultConstant(0x80110824)]
    public static HResult COMADMIN_E_PARTITIONS_DISABLED => new(0x80110824);

    /// <summary>A handler was not defined by the filter for this operation.</summary>
    [Description("""A handler was not defined by the filter for this operation.""")]
    [HResultConstant(0x801F0001)]
    public static HResult ERROR_FLT_NO_HANDLER_DEFINED => new(0x801F0001);

    /// <summary>A context is already defined for this object.</summary>
    [Description("""A context is already defined for this object.""")]
    [HResultConstant(0x801F0002)]
    public static HResult ERROR_FLT_CONTEXT_ALREADY_DEFINED => new(0x801F0002);

    /// <summary>Asynchronous requests are not valid for this operation.</summary>
    [Description("""Asynchronous requests are not valid for this operation.""")]
    [HResultConstant(0x801F0003)]
    public static HResult ERROR_FLT_INVALID_ASYNCHRONOUS_REQUEST => new(0x801F0003);

    /// <summary>Disallow the Fast IO path for this operation.</summary>
    [Description("""Disallow the Fast IO path for this operation.""")]
    [HResultConstant(0x801F0004)]
    public static HResult ERROR_FLT_DISALLOW_FAST_IO => new(0x801F0004);

    /// <summary>An invalid name request was made. The name requested cannot be retrieved at this time.</summary>
    [Description("""An invalid name request was made. The name requested cannot be retrieved at this time.""")]
    [HResultConstant(0x801F0005)]
    public static HResult ERROR_FLT_INVALID_NAME_REQUEST => new(0x801F0005);

    /// <summary>Posting this operation to a worker thread for further processing is not safe at this time because it could lead to a system deadlock.</summary>
    [Description("""Posting this operation to a worker thread for further processing is not safe at this time because it could lead to a system deadlock.""")]
    [HResultConstant(0x801F0006)]
    public static HResult ERROR_FLT_NOT_SAFE_TO_POST_OPERATION => new(0x801F0006);

    /// <summary>The Filter Manager was not initialized when a filter tried to register. Be sure that the Filter Manager is being loaded as a driver.</summary>
    [Description("""The Filter Manager was not initialized when a filter tried to register. Be sure that the Filter Manager is being loaded as a driver.""")]
    [HResultConstant(0x801F0007)]
    public static HResult ERROR_FLT_NOT_INITIALIZED => new(0x801F0007);

    /// <summary>The filter is not ready for attachment to volumes because it has not finished initializing (FltStartFiltering has not been called).</summary>
    [Description("""The filter is not ready for attachment to volumes because it has not finished initializing (FltStartFiltering has not been called).""")]
    [HResultConstant(0x801F0008)]
    public static HResult ERROR_FLT_FILTER_NOT_READY => new(0x801F0008);

    /// <summary>The filter must clean up any operation-specific context at this time because it is being removed from the system before the operation is completed by the lower drivers.</summary>
    [Description("""The filter must clean up any operation-specific context at this time because it is being removed from the system before the operation is completed by the lower drivers.""")]
    [HResultConstant(0x801F0009)]
    public static HResult ERROR_FLT_POST_OPERATION_CLEANUP => new(0x801F0009);

    /// <summary>The Filter Manager had an internal error from which it cannot recover; therefore, the operation has been failed. This is usually the result of a filter returning an invalid value from a preoperation callback.</summary>
    [Description("""The Filter Manager had an internal error from which it cannot recover; therefore, the operation has been failed. This is usually the result of a filter returning an invalid value from a preoperation callback.""")]
    [HResultConstant(0x801F000A)]
    public static HResult ERROR_FLT_INTERNAL_ERROR => new(0x801F000A);

    /// <summary>The object specified for this action is in the process of being deleted; therefore, the action requested cannot be completed at this time.</summary>
    [Description("""The object specified for this action is in the process of being deleted; therefore, the action requested cannot be completed at this time.""")]
    [HResultConstant(0x801F000B)]
    public static HResult ERROR_FLT_DELETING_OBJECT => new(0x801F000B);

    /// <summary>Nonpaged pool must be used for this type of context.</summary>
    [Description("""Nonpaged pool must be used for this type of context.""")]
    [HResultConstant(0x801F000C)]
    public static HResult ERROR_FLT_MUST_BE_NONPAGED_POOL => new(0x801F000C);

    /// <summary>A duplicate handler definition has been provided for an operation.</summary>
    [Description("""A duplicate handler definition has been provided for an operation.""")]
    [HResultConstant(0x801F000D)]
    public static HResult ERROR_FLT_DUPLICATE_ENTRY => new(0x801F000D);

    /// <summary>The callback data queue has been disabled.</summary>
    [Description("""The callback data queue has been disabled.""")]
    [HResultConstant(0x801F000E)]
    public static HResult ERROR_FLT_CBDQ_DISABLED => new(0x801F000E);

    /// <summary>Do not attach the filter to the volume at this time.</summary>
    [Description("""Do not attach the filter to the volume at this time.""")]
    [HResultConstant(0x801F000F)]
    public static HResult ERROR_FLT_DO_NOT_ATTACH => new(0x801F000F);

    /// <summary>Do not detach the filter from the volume at this time.</summary>
    [Description("""Do not detach the filter from the volume at this time.""")]
    [HResultConstant(0x801F0010)]
    public static HResult ERROR_FLT_DO_NOT_DETACH => new(0x801F0010);

    /// <summary>An instance already exists at this altitude on the volume specified.</summary>
    [Description("""An instance already exists at this altitude on the volume specified.""")]
    [HResultConstant(0x801F0011)]
    public static HResult ERROR_FLT_INSTANCE_ALTITUDE_COLLISION => new(0x801F0011);

    /// <summary>An instance already exists with this name on the volume specified.</summary>
    [Description("""An instance already exists with this name on the volume specified.""")]
    [HResultConstant(0x801F0012)]
    public static HResult ERROR_FLT_INSTANCE_NAME_COLLISION => new(0x801F0012);

    /// <summary>The system could not find the filter specified.</summary>
    [Description("""The system could not find the filter specified.""")]
    [HResultConstant(0x801F0013)]
    public static HResult ERROR_FLT_FILTER_NOT_FOUND => new(0x801F0013);

    /// <summary>The system could not find the volume specified.</summary>
    [Description("""The system could not find the volume specified.""")]
    [HResultConstant(0x801F0014)]
    public static HResult ERROR_FLT_VOLUME_NOT_FOUND => new(0x801F0014);

    /// <summary>The system could not find the instance specified.</summary>
    [Description("""The system could not find the instance specified.""")]
    [HResultConstant(0x801F0015)]
    public static HResult ERROR_FLT_INSTANCE_NOT_FOUND => new(0x801F0015);

    /// <summary>No registered context allocation definition was found for the given request.</summary>
    [Description("""No registered context allocation definition was found for the given request.""")]
    [HResultConstant(0x801F0016)]
    public static HResult ERROR_FLT_CONTEXT_ALLOCATION_NOT_FOUND => new(0x801F0016);

    /// <summary>An invalid parameter was specified during context registration.</summary>
    [Description("""An invalid parameter was specified during context registration.""")]
    [HResultConstant(0x801F0017)]
    public static HResult ERROR_FLT_INVALID_CONTEXT_REGISTRATION => new(0x801F0017);

    /// <summary>The name requested was not found in the Filter Manager name cache and could not be retrieved from the file system.</summary>
    [Description("""The name requested was not found in the Filter Manager name cache and could not be retrieved from the file system.""")]
    [HResultConstant(0x801F0018)]
    public static HResult ERROR_FLT_NAME_CACHE_MISS => new(0x801F0018);

    /// <summary>The requested device object does not exist for the given volume.</summary>
    [Description("""The requested device object does not exist for the given volume.""")]
    [HResultConstant(0x801F0019)]
    public static HResult ERROR_FLT_NO_DEVICE_OBJECT => new(0x801F0019);

    /// <summary>The specified volume is already mounted.</summary>
    [Description("""The specified volume is already mounted.""")]
    [HResultConstant(0x801F001A)]
    public static HResult ERROR_FLT_VOLUME_ALREADY_MOUNTED => new(0x801F001A);

    /// <summary>The specified Transaction Context is already enlisted in a transaction.</summary>
    [Description("""The specified Transaction Context is already enlisted in a transaction.""")]
    [HResultConstant(0x801F001B)]
    public static HResult ERROR_FLT_ALREADY_ENLISTED => new(0x801F001B);

    /// <summary>The specified context is already attached to another object.</summary>
    [Description("""The specified context is already attached to another object.""")]
    [HResultConstant(0x801F001C)]
    public static HResult ERROR_FLT_CONTEXT_ALREADY_LINKED => new(0x801F001C);

    /// <summary>No waiter is present for the filter's reply to this message.</summary>
    [Description("""No waiter is present for the filter's reply to this message.""")]
    [HResultConstant(0x801F0020)]
    public static HResult ERROR_FLT_NO_WAITER_FOR_REPLY => new(0x801F0020);

    /// <summary>{Display Driver Stopped Responding} The %hs display driver has stopped working normally. Save your work and reboot the system to restore full display functionality. The next time you reboot the machine a dialog will be displayed giving you a chance to report this failure to Microsoft.</summary>
    [Description("""{Display Driver Stopped Responding} The %hs display driver has stopped working normally. Save your work and reboot the system to restore full display functionality. The next time you reboot the machine a dialog will be displayed giving you a chance to report this failure to Microsoft.""")]
    [HResultConstant(0x80260001)]
    public static HResult ERROR_HUNG_DISPLAY_DRIVER_THREAD => new(0x80260001);

    /// <summary>Monitor descriptor could not be obtained.</summary>
    [Description("""Monitor descriptor could not be obtained.""")]
    [HResultConstant(0x80261001)]
    public static HResult ERROR_MONITOR_NO_DESCRIPTOR => new(0x80261001);

    /// <summary>Format of the obtained monitor descriptor is not supported by this release.</summary>
    [Description("""Format of the obtained monitor descriptor is not supported by this release.""")]
    [HResultConstant(0x80261002)]
    public static HResult ERROR_MONITOR_UNKNOWN_DESCRIPTOR_FORMAT => new(0x80261002);

    /// <summary>{Desktop Composition is Disabled} The operation could not be completed because desktop composition is disabled.</summary>
    [Description("""{Desktop Composition is Disabled} The operation could not be completed because desktop composition is disabled.""")]
    [HResultConstant(0x80263001)]
    public static HResult DWM_E_COMPOSITIONDISABLED => new(0x80263001);

    /// <summary>{Some Desktop Composition APIs Are Not Supported While Remoting} Some desktop composition APIs are not supported while remoting. The operation is not supported while running in a remote session.</summary>
    [Description("""{Some Desktop Composition APIs Are Not Supported While Remoting} Some desktop composition APIs are not supported while remoting. The operation is not supported while running in a remote session.""")]
    [HResultConstant(0x80263002)]
    public static HResult DWM_E_REMOTING_NOT_SUPPORTED => new(0x80263002);

    /// <summary>{No DWM Redirection Surface is Available} The Desktop Window Manager (DWM) was unable to provide a redirection surface to complete the DirectX present.</summary>
    [Description("""{No DWM Redirection Surface is Available} The Desktop Window Manager (DWM) was unable to provide a redirection surface to complete the DirectX present.""")]
    [HResultConstant(0x80263003)]
    public static HResult DWM_E_NO_REDIRECTION_SURFACE_AVAILABLE => new(0x80263003);

    /// <summary>{DWM Is Not Queuing Presents for the Specified Window} The window specified is not currently using queued presents.</summary>
    [Description("""{DWM Is Not Queuing Presents for the Specified Window} The window specified is not currently using queued presents.""")]
    [HResultConstant(0x80263004)]
    public static HResult DWM_E_NOT_QUEUING_PRESENTS => new(0x80263004);

    /// <summary>This is an error mask to convert Trusted Platform Module (TPM) hardware errors to Win32 errors.</summary>
    [Description("""This is an error mask to convert Trusted Platform Module (TPM) hardware errors to Win32 errors.""")]
    [HResultConstant(0x80280000)]
    public static HResult TPM_E_ERROR_MASK => new(0x80280000);

    /// <summary>Authentication failed.</summary>
    [Description("""Authentication failed.""")]
    [HResultConstant(0x80280001)]
    public static HResult TPM_E_AUTHFAIL => new(0x80280001);

    /// <summary>The index to a Platform Configuration Register (PCR), DIR, or other register is incorrect.</summary>
    [Description("""The index to a Platform Configuration Register (PCR), DIR, or other register is incorrect.""")]
    [HResultConstant(0x80280002)]
    public static HResult TPM_E_BADINDEX => new(0x80280002);

    /// <summary>One or more parameters are bad.</summary>
    [Description("""One or more parameters are bad.""")]
    [HResultConstant(0x80280003)]
    public static HResult TPM_E_BAD_PARAMETER => new(0x80280003);

    /// <summary>An operation completed successfully but the auditing of that operation failed.</summary>
    [Description("""An operation completed successfully but the auditing of that operation failed.""")]
    [HResultConstant(0x80280004)]
    public static HResult TPM_E_AUDITFAILURE => new(0x80280004);

    /// <summary>The clear disable flag is set and all clear operations now require physical access.</summary>
    [Description("""The clear disable flag is set and all clear operations now require physical access.""")]
    [HResultConstant(0x80280005)]
    public static HResult TPM_E_CLEAR_DISABLED => new(0x80280005);

    /// <summary>The TPM is deactivated.</summary>
    [Description("""The TPM is deactivated.""")]
    [HResultConstant(0x80280006)]
    public static HResult TPM_E_DEACTIVATED => new(0x80280006);

    /// <summary>The TPM is disabled.</summary>
    [Description("""The TPM is disabled.""")]
    [HResultConstant(0x80280007)]
    public static HResult TPM_E_DISABLED => new(0x80280007);

    /// <summary>The target command has been disabled.</summary>
    [Description("""The target command has been disabled.""")]
    [HResultConstant(0x80280008)]
    public static HResult TPM_E_DISABLED_CMD => new(0x80280008);

    /// <summary>The operation failed.</summary>
    [Description("""The operation failed.""")]
    [HResultConstant(0x80280009)]
    public static HResult TPM_E_FAIL => new(0x80280009);

    /// <summary>The ordinal was unknown or inconsistent.</summary>
    [Description("""The ordinal was unknown or inconsistent.""")]
    [HResultConstant(0x8028000A)]
    public static HResult TPM_E_BAD_ORDINAL => new(0x8028000A);

    /// <summary>The ability to install an owner is disabled.</summary>
    [Description("""The ability to install an owner is disabled.""")]
    [HResultConstant(0x8028000B)]
    public static HResult TPM_E_INSTALL_DISABLED => new(0x8028000B);

    /// <summary>The key handle cannot be interpreted.</summary>
    [Description("""The key handle cannot be interpreted.""")]
    [HResultConstant(0x8028000C)]
    public static HResult TPM_E_INVALID_KEYHANDLE => new(0x8028000C);

    /// <summary>The key handle points to an invalid key.</summary>
    [Description("""The key handle points to an invalid key.""")]
    [HResultConstant(0x8028000D)]
    public static HResult TPM_E_KEYNOTFOUND => new(0x8028000D);

    /// <summary>Unacceptable encryption scheme.</summary>
    [Description("""Unacceptable encryption scheme.""")]
    [HResultConstant(0x8028000E)]
    public static HResult TPM_E_INAPPROPRIATE_ENC => new(0x8028000E);

    /// <summary>Migration authorization failed.</summary>
    [Description("""Migration authorization failed.""")]
    [HResultConstant(0x8028000F)]
    public static HResult TPM_E_MIGRATEFAIL => new(0x8028000F);

    /// <summary>PCR information could not be interpreted.</summary>
    [Description("""PCR information could not be interpreted.""")]
    [HResultConstant(0x80280010)]
    public static HResult TPM_E_INVALID_PCR_INFO => new(0x80280010);

    /// <summary>No room to load key.</summary>
    [Description("""No room to load key.""")]
    [HResultConstant(0x80280011)]
    public static HResult TPM_E_NOSPACE => new(0x80280011);

    /// <summary>There is no storage root key (SRK) set.</summary>
    [Description("""There is no storage root key (SRK) set.""")]
    [HResultConstant(0x80280012)]
    public static HResult TPM_E_NOSRK => new(0x80280012);

    /// <summary>An encrypted blob is invalid or was not created by this TPM.</summary>
    [Description("""An encrypted blob is invalid or was not created by this TPM.""")]
    [HResultConstant(0x80280013)]
    public static HResult TPM_E_NOTSEALED_BLOB => new(0x80280013);

    /// <summary>There is already an owner.</summary>
    [Description("""There is already an owner.""")]
    [HResultConstant(0x80280014)]
    public static HResult TPM_E_OWNER_SET => new(0x80280014);

    /// <summary>The TPM has insufficient internal resources to perform the requested action.</summary>
    [Description("""The TPM has insufficient internal resources to perform the requested action.""")]
    [HResultConstant(0x80280015)]
    public static HResult TPM_E_RESOURCES => new(0x80280015);

    /// <summary>A random string was too short.</summary>
    [Description("""A random string was too short.""")]
    [HResultConstant(0x80280016)]
    public static HResult TPM_E_SHORTRANDOM => new(0x80280016);

    /// <summary>The TPM does not have the space to perform the operation.</summary>
    [Description("""The TPM does not have the space to perform the operation.""")]
    [HResultConstant(0x80280017)]
    public static HResult TPM_E_SIZE => new(0x80280017);

    /// <summary>The named PCR value does not match the current PCR value.</summary>
    [Description("""The named PCR value does not match the current PCR value.""")]
    [HResultConstant(0x80280018)]
    public static HResult TPM_E_WRONGPCRVAL => new(0x80280018);

    /// <summary>The paramSize argument to the command has the incorrect value.</summary>
    [Description("""The paramSize argument to the command has the incorrect value.""")]
    [HResultConstant(0x80280019)]
    public static HResult TPM_E_BAD_PARAM_SIZE => new(0x80280019);

    /// <summary>There is no existing SHA-1 thread.</summary>
    [Description("""There is no existing SHA-1 thread.""")]
    [HResultConstant(0x8028001A)]
    public static HResult TPM_E_SHA_THREAD => new(0x8028001A);

    /// <summary>The calculation is unable to proceed because the existing SHA-1 thread has already encountered an error.</summary>
    [Description("""The calculation is unable to proceed because the existing SHA-1 thread has already encountered an error.""")]
    [HResultConstant(0x8028001B)]
    public static HResult TPM_E_SHA_ERROR => new(0x8028001B);

    /// <summary>Self-test has failed and the TPM has shut down.</summary>
    [Description("""Self-test has failed and the TPM has shut down.""")]
    [HResultConstant(0x8028001C)]
    public static HResult TPM_E_FAILEDSELFTEST => new(0x8028001C);

    /// <summary>The authorization for the second key in a two-key function failed authorization.</summary>
    [Description("""The authorization for the second key in a two-key function failed authorization.""")]
    [HResultConstant(0x8028001D)]
    public static HResult TPM_E_AUTH2FAIL => new(0x8028001D);

    /// <summary>The tag value sent to for a command is invalid.</summary>
    [Description("""The tag value sent to for a command is invalid.""")]
    [HResultConstant(0x8028001E)]
    public static HResult TPM_E_BADTAG => new(0x8028001E);

    /// <summary>An I/O error occurred transmitting information to the TPM.</summary>
    [Description("""An I/O error occurred transmitting information to the TPM.""")]
    [HResultConstant(0x8028001F)]
    public static HResult TPM_E_IOERROR => new(0x8028001F);

    /// <summary>The encryption process had a problem.</summary>
    [Description("""The encryption process had a problem.""")]
    [HResultConstant(0x80280020)]
    public static HResult TPM_E_ENCRYPT_ERROR => new(0x80280020);

    /// <summary>The decryption process did not complete.</summary>
    [Description("""The decryption process did not complete.""")]
    [HResultConstant(0x80280021)]
    public static HResult TPM_E_DECRYPT_ERROR => new(0x80280021);

    /// <summary>An invalid handle was used.</summary>
    [Description("""An invalid handle was used.""")]
    [HResultConstant(0x80280022)]
    public static HResult TPM_E_INVALID_AUTHHANDLE => new(0x80280022);

    /// <summary>The TPM does not have an endorsement key (EK) installed.</summary>
    [Description("""The TPM does not have an endorsement key (EK) installed.""")]
    [HResultConstant(0x80280023)]
    public static HResult TPM_E_NO_ENDORSEMENT => new(0x80280023);

    /// <summary>The usage of a key is not allowed.</summary>
    [Description("""The usage of a key is not allowed.""")]
    [HResultConstant(0x80280024)]
    public static HResult TPM_E_INVALID_KEYUSAGE => new(0x80280024);

    /// <summary>The submitted entity type is not allowed.</summary>
    [Description("""The submitted entity type is not allowed.""")]
    [HResultConstant(0x80280025)]
    public static HResult TPM_E_WRONG_ENTITYTYPE => new(0x80280025);

    /// <summary>The command was received in the wrong sequence relative to TPM_Init and a subsequent TPM_Startup.</summary>
    [Description("""The command was received in the wrong sequence relative to TPM_Init and a subsequent TPM_Startup.""")]
    [HResultConstant(0x80280026)]
    public static HResult TPM_E_INVALID_POSTINIT => new(0x80280026);

    /// <summary>Signed data cannot include additional DER information.</summary>
    [Description("""Signed data cannot include additional DER information.""")]
    [HResultConstant(0x80280027)]
    public static HResult TPM_E_INAPPROPRIATE_SIG => new(0x80280027);

    /// <summary>The key properties in TPM_KEY_PARMs are not supported by this TPM.</summary>
    [Description("""The key properties in TPM_KEY_PARMs are not supported by this TPM.""")]
    [HResultConstant(0x80280028)]
    public static HResult TPM_E_BAD_KEY_PROPERTY => new(0x80280028);

    /// <summary>The migration properties of this key are incorrect.</summary>
    [Description("""The migration properties of this key are incorrect.""")]
    [HResultConstant(0x80280029)]
    public static HResult TPM_E_BAD_MIGRATION => new(0x80280029);

    /// <summary>The signature or encryption scheme for this key is incorrect or not permitted in this situation.</summary>
    [Description("""The signature or encryption scheme for this key is incorrect or not permitted in this situation.""")]
    [HResultConstant(0x8028002A)]
    public static HResult TPM_E_BAD_SCHEME => new(0x8028002A);

    /// <summary>The size of the data (or blob) parameter is bad or inconsistent with the referenced key.</summary>
    [Description("""The size of the data (or blob) parameter is bad or inconsistent with the referenced key.""")]
    [HResultConstant(0x8028002B)]
    public static HResult TPM_E_BAD_DATASIZE => new(0x8028002B);

    /// <summary>A mode parameter is bad, such as capArea or subCapArea for TPM_GetCapability, physicalPresence parameter for TPM_PhysicalPresence, or migrationType for TPM_CreateMigrationBlob.</summary>
    [Description("""A mode parameter is bad, such as capArea or subCapArea for TPM_GetCapability, physicalPresence parameter for TPM_PhysicalPresence, or migrationType for TPM_CreateMigrationBlob.""")]
    [HResultConstant(0x8028002C)]
    public static HResult TPM_E_BAD_MODE => new(0x8028002C);

    /// <summary>Either the physicalPresence or physicalPresenceLock bits have the wrong value.</summary>
    [Description("""Either the physicalPresence or physicalPresenceLock bits have the wrong value.""")]
    [HResultConstant(0x8028002D)]
    public static HResult TPM_E_BAD_PRESENCE => new(0x8028002D);

    /// <summary>The TPM cannot perform this version of the capability.</summary>
    [Description("""The TPM cannot perform this version of the capability.""")]
    [HResultConstant(0x8028002E)]
    public static HResult TPM_E_BAD_VERSION => new(0x8028002E);

    /// <summary>The TPM does not allow for wrapped transport sessions.</summary>
    [Description("""The TPM does not allow for wrapped transport sessions.""")]
    [HResultConstant(0x8028002F)]
    public static HResult TPM_E_NO_WRAP_TRANSPORT => new(0x8028002F);

    /// <summary>TPM audit construction failed and the underlying command was returning a failure code also.</summary>
    [Description("""TPM audit construction failed and the underlying command was returning a failure code also.""")]
    [HResultConstant(0x80280030)]
    public static HResult TPM_E_AUDITFAIL_UNSUCCESSFUL => new(0x80280030);

    /// <summary>TPM audit construction failed and the underlying command was returning success.</summary>
    [Description("""TPM audit construction failed and the underlying command was returning success.""")]
    [HResultConstant(0x80280031)]
    public static HResult TPM_E_AUDITFAIL_SUCCESSFUL => new(0x80280031);

    /// <summary>Attempt to reset a PCR that does not have the resettable attribute.</summary>
    [Description("""Attempt to reset a PCR that does not have the resettable attribute.""")]
    [HResultConstant(0x80280032)]
    public static HResult TPM_E_NOTRESETABLE => new(0x80280032);

    /// <summary>Attempt to reset a PCR register that requires locality and the locality modifier not part of command transport.</summary>
    [Description("""Attempt to reset a PCR register that requires locality and the locality modifier not part of command transport.""")]
    [HResultConstant(0x80280033)]
    public static HResult TPM_E_NOTLOCAL => new(0x80280033);

    /// <summary>Make identity blob not properly typed.</summary>
    [Description("""Make identity blob not properly typed.""")]
    [HResultConstant(0x80280034)]
    public static HResult TPM_E_BAD_TYPE => new(0x80280034);

    /// <summary>When saving context identified resource type does not match actual resource.</summary>
    [Description("""When saving context identified resource type does not match actual resource.""")]
    [HResultConstant(0x80280035)]
    public static HResult TPM_E_INVALID_RESOURCE => new(0x80280035);

    /// <summary>The TPM is attempting to execute a command only available when in Federal Information Processing Standards (FIPS) mode.</summary>
    [Description("""The TPM is attempting to execute a command only available when in Federal Information Processing Standards (FIPS) mode.""")]
    [HResultConstant(0x80280036)]
    public static HResult TPM_E_NOTFIPS => new(0x80280036);

    /// <summary>The command is attempting to use an invalid family ID.</summary>
    [Description("""The command is attempting to use an invalid family ID.""")]
    [HResultConstant(0x80280037)]
    public static HResult TPM_E_INVALID_FAMILY => new(0x80280037);

    /// <summary>The permission to manipulate the NV storage is not available.</summary>
    [Description("""The permission to manipulate the NV storage is not available.""")]
    [HResultConstant(0x80280038)]
    public static HResult TPM_E_NO_NV_PERMISSION => new(0x80280038);

    /// <summary>The operation requires a signed command.</summary>
    [Description("""The operation requires a signed command.""")]
    [HResultConstant(0x80280039)]
    public static HResult TPM_E_REQUIRES_SIGN => new(0x80280039);

    /// <summary>Wrong operation to load an NV key.</summary>
    [Description("""Wrong operation to load an NV key.""")]
    [HResultConstant(0x8028003A)]
    public static HResult TPM_E_KEY_NOTSUPPORTED => new(0x8028003A);

    /// <summary>NV_LoadKey blob requires both owner and blob authorization.</summary>
    [Description("""NV_LoadKey blob requires both owner and blob authorization.""")]
    [HResultConstant(0x8028003B)]
    public static HResult TPM_E_AUTH_CONFLICT => new(0x8028003B);

    /// <summary>The NV area is locked and not writable.</summary>
    [Description("""The NV area is locked and not writable.""")]
    [HResultConstant(0x8028003C)]
    public static HResult TPM_E_AREA_LOCKED => new(0x8028003C);

    /// <summary>The locality is incorrect for the attempted operation.</summary>
    [Description("""The locality is incorrect for the attempted operation.""")]
    [HResultConstant(0x8028003D)]
    public static HResult TPM_E_BAD_LOCALITY => new(0x8028003D);

    /// <summary>The NV area is read-only and cannot be written to.</summary>
    [Description("""The NV area is read-only and cannot be written to.""")]
    [HResultConstant(0x8028003E)]
    public static HResult TPM_E_READ_ONLY => new(0x8028003E);

    /// <summary>There is no protection on the write to the NV area.</summary>
    [Description("""There is no protection on the write to the NV area.""")]
    [HResultConstant(0x8028003F)]
    public static HResult TPM_E_PER_NOWRITE => new(0x8028003F);

    /// <summary>The family count value does not match.</summary>
    [Description("""The family count value does not match.""")]
    [HResultConstant(0x80280040)]
    public static HResult TPM_E_FAMILYCOUNT => new(0x80280040);

    /// <summary>The NV area has already been written to.</summary>
    [Description("""The NV area has already been written to.""")]
    [HResultConstant(0x80280041)]
    public static HResult TPM_E_WRITE_LOCKED => new(0x80280041);

    /// <summary>The NV area attributes conflict.</summary>
    [Description("""The NV area attributes conflict.""")]
    [HResultConstant(0x80280042)]
    public static HResult TPM_E_BAD_ATTRIBUTES => new(0x80280042);

    /// <summary>The structure tag and version are invalid or inconsistent.</summary>
    [Description("""The structure tag and version are invalid or inconsistent.""")]
    [HResultConstant(0x80280043)]
    public static HResult TPM_E_INVALID_STRUCTURE => new(0x80280043);

    /// <summary>The key is under control of the TPM owner and can only be evicted by the TPM owner.</summary>
    [Description("""The key is under control of the TPM owner and can only be evicted by the TPM owner.""")]
    [HResultConstant(0x80280044)]
    public static HResult TPM_E_KEY_OWNER_CONTROL => new(0x80280044);

    /// <summary>The counter handle is incorrect.</summary>
    [Description("""The counter handle is incorrect.""")]
    [HResultConstant(0x80280045)]
    public static HResult TPM_E_BAD_COUNTER => new(0x80280045);

    /// <summary>The write is not a complete write of the area.</summary>
    [Description("""The write is not a complete write of the area.""")]
    [HResultConstant(0x80280046)]
    public static HResult TPM_E_NOT_FULLWRITE => new(0x80280046);

    /// <summary>The gap between saved context counts is too large.</summary>
    [Description("""The gap between saved context counts is too large.""")]
    [HResultConstant(0x80280047)]
    public static HResult TPM_E_CONTEXT_GAP => new(0x80280047);

    /// <summary>The maximum number of NV writes without an owner has been exceeded.</summary>
    [Description("""The maximum number of NV writes without an owner has been exceeded.""")]
    [HResultConstant(0x80280048)]
    public static HResult TPM_E_MAXNVWRITES => new(0x80280048);

    /// <summary>No operator AuthData value is set.</summary>
    [Description("""No operator AuthData value is set.""")]
    [HResultConstant(0x80280049)]
    public static HResult TPM_E_NOOPERATOR => new(0x80280049);

    /// <summary>The resource pointed to by context is not loaded.</summary>
    [Description("""The resource pointed to by context is not loaded.""")]
    [HResultConstant(0x8028004A)]
    public static HResult TPM_E_RESOURCEMISSING => new(0x8028004A);

    /// <summary>The delegate administration is locked.</summary>
    [Description("""The delegate administration is locked.""")]
    [HResultConstant(0x8028004B)]
    public static HResult TPM_E_DELEGATE_LOCK => new(0x8028004B);

    /// <summary>Attempt to manage a family other then the delegated family.</summary>
    [Description("""Attempt to manage a family other then the delegated family.""")]
    [HResultConstant(0x8028004C)]
    public static HResult TPM_E_DELEGATE_FAMILY => new(0x8028004C);

    /// <summary>Delegation table management not enabled.</summary>
    [Description("""Delegation table management not enabled.""")]
    [HResultConstant(0x8028004D)]
    public static HResult TPM_E_DELEGATE_ADMIN => new(0x8028004D);

    /// <summary>There was a command executed outside an exclusive transport session.</summary>
    [Description("""There was a command executed outside an exclusive transport session.""")]
    [HResultConstant(0x8028004E)]
    public static HResult TPM_E_TRANSPORT_NOTEXCLUSIVE => new(0x8028004E);

    /// <summary>Attempt to context save an owner evict controlled key.</summary>
    [Description("""Attempt to context save an owner evict controlled key.""")]
    [HResultConstant(0x8028004F)]
    public static HResult TPM_E_OWNER_CONTROL => new(0x8028004F);

    /// <summary>The DAA command has no resources available to execute the command.</summary>
    [Description("""The DAA command has no resources available to execute the command.""")]
    [HResultConstant(0x80280050)]
    public static HResult TPM_E_DAA_RESOURCES => new(0x80280050);

    /// <summary>The consistency check on DAA parameter inputData0 has failed.</summary>
    [Description("""The consistency check on DAA parameter inputData0 has failed.""")]
    [HResultConstant(0x80280051)]
    public static HResult TPM_E_DAA_INPUT_DATA0 => new(0x80280051);

    /// <summary>The consistency check on DAA parameter inputData1 has failed.</summary>
    [Description("""The consistency check on DAA parameter inputData1 has failed.""")]
    [HResultConstant(0x80280052)]
    public static HResult TPM_E_DAA_INPUT_DATA1 => new(0x80280052);

    /// <summary>The consistency check on DAA_issuerSettings has failed.</summary>
    [Description("""The consistency check on DAA_issuerSettings has failed.""")]
    [HResultConstant(0x80280053)]
    public static HResult TPM_E_DAA_ISSUER_SETTINGS => new(0x80280053);

    /// <summary>The consistency check on DAA_tpmSpecific has failed.</summary>
    [Description("""The consistency check on DAA_tpmSpecific has failed.""")]
    [HResultConstant(0x80280054)]
    public static HResult TPM_E_DAA_TPM_SETTINGS => new(0x80280054);

    /// <summary>The atomic process indicated by the submitted DAA command is not the expected process.</summary>
    [Description("""The atomic process indicated by the submitted DAA command is not the expected process.""")]
    [HResultConstant(0x80280055)]
    public static HResult TPM_E_DAA_STAGE => new(0x80280055);

    /// <summary>The issuer's validity check has detected an inconsistency.</summary>
    [Description("""The issuer's validity check has detected an inconsistency.""")]
    [HResultConstant(0x80280056)]
    public static HResult TPM_E_DAA_ISSUER_VALIDITY => new(0x80280056);

    /// <summary>The consistency check on w has failed.</summary>
    [Description("""The consistency check on w has failed.""")]
    [HResultConstant(0x80280057)]
    public static HResult TPM_E_DAA_WRONG_W => new(0x80280057);

    /// <summary>The handle is incorrect.</summary>
    [Description("""The handle is incorrect.""")]
    [HResultConstant(0x80280058)]
    public static HResult TPM_E_BAD_HANDLE => new(0x80280058);

    /// <summary>Delegation is not correct.</summary>
    [Description("""Delegation is not correct.""")]
    [HResultConstant(0x80280059)]
    public static HResult TPM_E_BAD_DELEGATE => new(0x80280059);

    /// <summary>The context blob is invalid.</summary>
    [Description("""The context blob is invalid.""")]
    [HResultConstant(0x8028005A)]
    public static HResult TPM_E_BADCONTEXT => new(0x8028005A);

    /// <summary>Too many contexts held by the TPM.</summary>
    [Description("""Too many contexts held by the TPM.""")]
    [HResultConstant(0x8028005B)]
    public static HResult TPM_E_TOOMANYCONTEXTS => new(0x8028005B);

    /// <summary>Migration authority signature validation failure.</summary>
    [Description("""Migration authority signature validation failure.""")]
    [HResultConstant(0x8028005C)]
    public static HResult TPM_E_MA_TICKET_SIGNATURE => new(0x8028005C);

    /// <summary>Migration destination not authenticated.</summary>
    [Description("""Migration destination not authenticated.""")]
    [HResultConstant(0x8028005D)]
    public static HResult TPM_E_MA_DESTINATION => new(0x8028005D);

    /// <summary>Migration source incorrect.</summary>
    [Description("""Migration source incorrect.""")]
    [HResultConstant(0x8028005E)]
    public static HResult TPM_E_MA_SOURCE => new(0x8028005E);

    /// <summary>Incorrect migration authority.</summary>
    [Description("""Incorrect migration authority.""")]
    [HResultConstant(0x8028005F)]
    public static HResult TPM_E_MA_AUTHORITY => new(0x8028005F);

    /// <summary>Attempt to revoke the EK and the EK is not revocable.</summary>
    [Description("""Attempt to revoke the EK and the EK is not revocable.""")]
    [HResultConstant(0x80280061)]
    public static HResult TPM_E_PERMANENTEK => new(0x80280061);

    /// <summary>Bad signature of CMK ticket.</summary>
    [Description("""Bad signature of CMK ticket.""")]
    [HResultConstant(0x80280062)]
    public static HResult TPM_E_BAD_SIGNATURE => new(0x80280062);

    /// <summary>There is no room in the context list for additional contexts.</summary>
    [Description("""There is no room in the context list for additional contexts.""")]
    [HResultConstant(0x80280063)]
    public static HResult TPM_E_NOCONTEXTSPACE => new(0x80280063);

    /// <summary>The command was blocked.</summary>
    [Description("""The command was blocked.""")]
    [HResultConstant(0x80280400)]
    public static HResult TPM_E_COMMAND_BLOCKED => new(0x80280400);

    /// <summary>The specified handle was not found.</summary>
    [Description("""The specified handle was not found.""")]
    [HResultConstant(0x80280401)]
    public static HResult TPM_E_INVALID_HANDLE => new(0x80280401);

    /// <summary>The TPM returned a duplicate handle and the command needs to be resubmitted.</summary>
    [Description("""The TPM returned a duplicate handle and the command needs to be resubmitted.""")]
    [HResultConstant(0x80280402)]
    public static HResult TPM_E_DUPLICATE_VHANDLE => new(0x80280402);

    /// <summary>The command within the transport was blocked.</summary>
    [Description("""The command within the transport was blocked.""")]
    [HResultConstant(0x80280403)]
    public static HResult TPM_E_EMBEDDED_COMMAND_BLOCKED => new(0x80280403);

    /// <summary>The command within the transport is not supported.</summary>
    [Description("""The command within the transport is not supported.""")]
    [HResultConstant(0x80280404)]
    public static HResult TPM_E_EMBEDDED_COMMAND_UNSUPPORTED => new(0x80280404);

    /// <summary>The TPM is too busy to respond to the command immediately, but the command could be resubmitted at a later time.</summary>
    [Description("""The TPM is too busy to respond to the command immediately, but the command could be resubmitted at a later time.""")]
    [HResultConstant(0x80280800)]
    public static HResult TPM_E_RETRY => new(0x80280800);

    /// <summary>SelfTestFull has not been run.</summary>
    [Description("""SelfTestFull has not been run.""")]
    [HResultConstant(0x80280801)]
    public static HResult TPM_E_NEEDS_SELFTEST => new(0x80280801);

    /// <summary>The TPM is currently executing a full self-test.</summary>
    [Description("""The TPM is currently executing a full self-test.""")]
    [HResultConstant(0x80280802)]
    public static HResult TPM_E_DOING_SELFTEST => new(0x80280802);

    /// <summary>The TPM is defending against dictionary attacks and is in a time-out period.</summary>
    [Description("""The TPM is defending against dictionary attacks and is in a time-out period.""")]
    [HResultConstant(0x80280803)]
    public static HResult TPM_E_DEFEND_LOCK_RUNNING => new(0x80280803);

    /// <summary>An internal software error has been detected.</summary>
    [Description("""An internal software error has been detected.""")]
    [HResultConstant(0x80284001)]
    public static HResult TBS_E_INTERNAL_ERROR => new(0x80284001);

    /// <summary>One or more input parameters are bad.</summary>
    [Description("""One or more input parameters are bad.""")]
    [HResultConstant(0x80284002)]
    public static HResult TBS_E_BAD_PARAMETER => new(0x80284002);

    /// <summary>A specified output pointer is bad.</summary>
    [Description("""A specified output pointer is bad.""")]
    [HResultConstant(0x80284003)]
    public static HResult TBS_E_INVALID_OUTPUT_POINTER => new(0x80284003);

    /// <summary>The specified context handle does not refer to a valid context.</summary>
    [Description("""The specified context handle does not refer to a valid context.""")]
    [HResultConstant(0x80284004)]
    public static HResult TBS_E_INVALID_CONTEXT => new(0x80284004);

    /// <summary>A specified output buffer is too small.</summary>
    [Description("""A specified output buffer is too small.""")]
    [HResultConstant(0x80284005)]
    public static HResult TBS_E_INSUFFICIENT_BUFFER => new(0x80284005);

    /// <summary>An error occurred while communicating with the TPM.</summary>
    [Description("""An error occurred while communicating with the TPM.""")]
    [HResultConstant(0x80284006)]
    public static HResult TBS_E_IOERROR => new(0x80284006);

    /// <summary>One or more context parameters are invalid.</summary>
    [Description("""One or more context parameters are invalid.""")]
    [HResultConstant(0x80284007)]
    public static HResult TBS_E_INVALID_CONTEXT_PARAM => new(0x80284007);

    /// <summary>The TPM Base Services (TBS) is not running and could not be started.</summary>
    [Description("""The TPM Base Services (TBS) is not running and could not be started.""")]
    [HResultConstant(0x80284008)]
    public static HResult TBS_E_SERVICE_NOT_RUNNING => new(0x80284008);

    /// <summary>A new context could not be created because there are too many open contexts.</summary>
    [Description("""A new context could not be created because there are too many open contexts.""")]
    [HResultConstant(0x80284009)]
    public static HResult TBS_E_TOO_MANY_TBS_CONTEXTS => new(0x80284009);

    /// <summary>A new virtual resource could not be created because there are too many open virtual resources.</summary>
    [Description("""A new virtual resource could not be created because there are too many open virtual resources.""")]
    [HResultConstant(0x8028400A)]
    public static HResult TBS_E_TOO_MANY_RESOURCES => new(0x8028400A);

    /// <summary>The TBS service has been started but is not yet running.</summary>
    [Description("""The TBS service has been started but is not yet running.""")]
    [HResultConstant(0x8028400B)]
    public static HResult TBS_E_SERVICE_START_PENDING => new(0x8028400B);

    /// <summary>The physical presence interface is not supported.</summary>
    [Description("""The physical presence interface is not supported.""")]
    [HResultConstant(0x8028400C)]
    public static HResult TBS_E_PPI_NOT_SUPPORTED => new(0x8028400C);

    /// <summary>The command was canceled.</summary>
    [Description("""The command was canceled.""")]
    [HResultConstant(0x8028400D)]
    public static HResult TBS_E_COMMAND_CANCELED => new(0x8028400D);

    /// <summary>The input or output buffer is too large.</summary>
    [Description("""The input or output buffer is too large.""")]
    [HResultConstant(0x8028400E)]
    public static HResult TBS_E_BUFFER_TOO_LARGE => new(0x8028400E);

    /// <summary>The command buffer is not in the correct state.</summary>
    [Description("""The command buffer is not in the correct state.""")]
    [HResultConstant(0x80290100)]
    public static HResult TPMAPI_E_INVALID_STATE => new(0x80290100);

    /// <summary>The command buffer does not contain enough data to satisfy the request.</summary>
    [Description("""The command buffer does not contain enough data to satisfy the request.""")]
    [HResultConstant(0x80290101)]
    public static HResult TPMAPI_E_NOT_ENOUGH_DATA => new(0x80290101);

    /// <summary>The command buffer cannot contain any more data.</summary>
    [Description("""The command buffer cannot contain any more data.""")]
    [HResultConstant(0x80290102)]
    public static HResult TPMAPI_E_TOO_MUCH_DATA => new(0x80290102);

    /// <summary>One or more output parameters was null or invalid.</summary>
    [Description("""One or more output parameters was null or invalid.""")]
    [HResultConstant(0x80290103)]
    public static HResult TPMAPI_E_INVALID_OUTPUT_POINTER => new(0x80290103);

    /// <summary>One or more input parameters are invalid.</summary>
    [Description("""One or more input parameters are invalid.""")]
    [HResultConstant(0x80290104)]
    public static HResult TPMAPI_E_INVALID_PARAMETER => new(0x80290104);

    /// <summary>Not enough memory was available to satisfy the request.</summary>
    [Description("""Not enough memory was available to satisfy the request.""")]
    [HResultConstant(0x80290105)]
    public static HResult TPMAPI_E_OUT_OF_MEMORY => new(0x80290105);

    /// <summary>The specified buffer was too small.</summary>
    [Description("""The specified buffer was too small.""")]
    [HResultConstant(0x80290106)]
    public static HResult TPMAPI_E_BUFFER_TOO_SMALL => new(0x80290106);

    /// <summary>An internal error was detected.</summary>
    [Description("""An internal error was detected.""")]
    [HResultConstant(0x80290107)]
    public static HResult TPMAPI_E_INTERNAL_ERROR => new(0x80290107);

    /// <summary>The caller does not have the appropriate rights to perform the requested operation.</summary>
    [Description("""The caller does not have the appropriate rights to perform the requested operation.""")]
    [HResultConstant(0x80290108)]
    public static HResult TPMAPI_E_ACCESS_DENIED => new(0x80290108);

    /// <summary>The specified authorization information was invalid.</summary>
    [Description("""The specified authorization information was invalid.""")]
    [HResultConstant(0x80290109)]
    public static HResult TPMAPI_E_AUTHORIZATION_FAILED => new(0x80290109);

    /// <summary>The specified context handle was not valid.</summary>
    [Description("""The specified context handle was not valid.""")]
    [HResultConstant(0x8029010A)]
    public static HResult TPMAPI_E_INVALID_CONTEXT_HANDLE => new(0x8029010A);

    /// <summary>An error occurred while communicating with the TBS.</summary>
    [Description("""An error occurred while communicating with the TBS.""")]
    [HResultConstant(0x8029010B)]
    public static HResult TPMAPI_E_TBS_COMMUNICATION_ERROR => new(0x8029010B);

    /// <summary>The TPM returned an unexpected result.</summary>
    [Description("""The TPM returned an unexpected result.""")]
    [HResultConstant(0x8029010C)]
    public static HResult TPMAPI_E_TPM_COMMAND_ERROR => new(0x8029010C);

    /// <summary>The message was too large for the encoding scheme.</summary>
    [Description("""The message was too large for the encoding scheme.""")]
    [HResultConstant(0x8029010D)]
    public static HResult TPMAPI_E_MESSAGE_TOO_LARGE => new(0x8029010D);

    /// <summary>The encoding in the binary large object (BLOB) was not recognized.</summary>
    [Description("""The encoding in the binary large object (BLOB) was not recognized.""")]
    [HResultConstant(0x8029010E)]
    public static HResult TPMAPI_E_INVALID_ENCODING => new(0x8029010E);

    /// <summary>The key size is not valid.</summary>
    [Description("""The key size is not valid.""")]
    [HResultConstant(0x8029010F)]
    public static HResult TPMAPI_E_INVALID_KEY_SIZE => new(0x8029010F);

    /// <summary>The encryption operation failed.</summary>
    [Description("""The encryption operation failed.""")]
    [HResultConstant(0x80290110)]
    public static HResult TPMAPI_E_ENCRYPTION_FAILED => new(0x80290110);

    /// <summary>The key parameters structure was not valid.</summary>
    [Description("""The key parameters structure was not valid.""")]
    [HResultConstant(0x80290111)]
    public static HResult TPMAPI_E_INVALID_KEY_PARAMS => new(0x80290111);

    /// <summary>The requested supplied data does not appear to be a valid migration authorization BLOB.</summary>
    [Description("""The requested supplied data does not appear to be a valid migration authorization BLOB.""")]
    [HResultConstant(0x80290112)]
    public static HResult TPMAPI_E_INVALID_MIGRATION_AUTHORIZATION_BLOB => new(0x80290112);

    /// <summary>The specified PCR index was invalid.</summary>
    [Description("""The specified PCR index was invalid.""")]
    [HResultConstant(0x80290113)]
    public static HResult TPMAPI_E_INVALID_PCR_INDEX => new(0x80290113);

    /// <summary>The data given does not appear to be a valid delegate BLOB.</summary>
    [Description("""The data given does not appear to be a valid delegate BLOB.""")]
    [HResultConstant(0x80290114)]
    public static HResult TPMAPI_E_INVALID_DELEGATE_BLOB => new(0x80290114);

    /// <summary>One or more of the specified context parameters was not valid.</summary>
    [Description("""One or more of the specified context parameters was not valid.""")]
    [HResultConstant(0x80290115)]
    public static HResult TPMAPI_E_INVALID_CONTEXT_PARAMS => new(0x80290115);

    /// <summary>The data given does not appear to be a valid key BLOB.</summary>
    [Description("""The data given does not appear to be a valid key BLOB.""")]
    [HResultConstant(0x80290116)]
    public static HResult TPMAPI_E_INVALID_KEY_BLOB => new(0x80290116);

    /// <summary>The specified PCR data was invalid.</summary>
    [Description("""The specified PCR data was invalid.""")]
    [HResultConstant(0x80290117)]
    public static HResult TPMAPI_E_INVALID_PCR_DATA => new(0x80290117);

    /// <summary>The format of the owner authorization data was invalid.</summary>
    [Description("""The format of the owner authorization data was invalid.""")]
    [HResultConstant(0x80290118)]
    public static HResult TPMAPI_E_INVALID_OWNER_AUTH => new(0x80290118);

    /// <summary>The specified buffer was too small.</summary>
    [Description("""The specified buffer was too small.""")]
    [HResultConstant(0x80290200)]
    public static HResult TBSIMP_E_BUFFER_TOO_SMALL => new(0x80290200);

    /// <summary>The context could not be cleaned up.</summary>
    [Description("""The context could not be cleaned up.""")]
    [HResultConstant(0x80290201)]
    public static HResult TBSIMP_E_CLEANUP_FAILED => new(0x80290201);

    /// <summary>The specified context handle is invalid.</summary>
    [Description("""The specified context handle is invalid.""")]
    [HResultConstant(0x80290202)]
    public static HResult TBSIMP_E_INVALID_CONTEXT_HANDLE => new(0x80290202);

    /// <summary>An invalid context parameter was specified.</summary>
    [Description("""An invalid context parameter was specified.""")]
    [HResultConstant(0x80290203)]
    public static HResult TBSIMP_E_INVALID_CONTEXT_PARAM => new(0x80290203);

    /// <summary>An error occurred while communicating with the TPM.</summary>
    [Description("""An error occurred while communicating with the TPM.""")]
    [HResultConstant(0x80290204)]
    public static HResult TBSIMP_E_TPM_ERROR => new(0x80290204);

    /// <summary>No entry with the specified key was found.</summary>
    [Description("""No entry with the specified key was found.""")]
    [HResultConstant(0x80290205)]
    public static HResult TBSIMP_E_HASH_BAD_KEY => new(0x80290205);

    /// <summary>The specified virtual handle matches a virtual handle already in use.</summary>
    [Description("""The specified virtual handle matches a virtual handle already in use.""")]
    [HResultConstant(0x80290206)]
    public static HResult TBSIMP_E_DUPLICATE_VHANDLE => new(0x80290206);

    /// <summary>The pointer to the returned handle location was null or invalid.</summary>
    [Description("""The pointer to the returned handle location was null or invalid.""")]
    [HResultConstant(0x80290207)]
    public static HResult TBSIMP_E_INVALID_OUTPUT_POINTER => new(0x80290207);

    /// <summary>One or more parameters are invalid.</summary>
    [Description("""One or more parameters are invalid.""")]
    [HResultConstant(0x80290208)]
    public static HResult TBSIMP_E_INVALID_PARAMETER => new(0x80290208);

    /// <summary>The RPC subsystem could not be initialized.</summary>
    [Description("""The RPC subsystem could not be initialized.""")]
    [HResultConstant(0x80290209)]
    public static HResult TBSIMP_E_RPC_INIT_FAILED => new(0x80290209);

    /// <summary>The TBS scheduler is not running.</summary>
    [Description("""The TBS scheduler is not running.""")]
    [HResultConstant(0x8029020A)]
    public static HResult TBSIMP_E_SCHEDULER_NOT_RUNNING => new(0x8029020A);

    /// <summary>The command was canceled.</summary>
    [Description("""The command was canceled.""")]
    [HResultConstant(0x8029020B)]
    public static HResult TBSIMP_E_COMMAND_CANCELED => new(0x8029020B);

    /// <summary>There was not enough memory to fulfill the request.</summary>
    [Description("""There was not enough memory to fulfill the request.""")]
    [HResultConstant(0x8029020C)]
    public static HResult TBSIMP_E_OUT_OF_MEMORY => new(0x8029020C);

    /// <summary>The specified list is empty, or the iteration has reached the end of the list.</summary>
    [Description("""The specified list is empty, or the iteration has reached the end of the list.""")]
    [HResultConstant(0x8029020D)]
    public static HResult TBSIMP_E_LIST_NO_MORE_ITEMS => new(0x8029020D);

    /// <summary>The specified item was not found in the list.</summary>
    [Description("""The specified item was not found in the list.""")]
    [HResultConstant(0x8029020E)]
    public static HResult TBSIMP_E_LIST_NOT_FOUND => new(0x8029020E);

    /// <summary>The TPM does not have enough space to load the requested resource.</summary>
    [Description("""The TPM does not have enough space to load the requested resource.""")]
    [HResultConstant(0x8029020F)]
    public static HResult TBSIMP_E_NOT_ENOUGH_SPACE => new(0x8029020F);

    /// <summary>There are too many TPM contexts in use.</summary>
    [Description("""There are too many TPM contexts in use.""")]
    [HResultConstant(0x80290210)]
    public static HResult TBSIMP_E_NOT_ENOUGH_TPM_CONTEXTS => new(0x80290210);

    /// <summary>The TPM command failed.</summary>
    [Description("""The TPM command failed.""")]
    [HResultConstant(0x80290211)]
    public static HResult TBSIMP_E_COMMAND_FAILED => new(0x80290211);

    /// <summary>The TBS does not recognize the specified ordinal.</summary>
    [Description("""The TBS does not recognize the specified ordinal.""")]
    [HResultConstant(0x80290212)]
    public static HResult TBSIMP_E_UNKNOWN_ORDINAL => new(0x80290212);

    /// <summary>The requested resource is no longer available.</summary>
    [Description("""The requested resource is no longer available.""")]
    [HResultConstant(0x80290213)]
    public static HResult TBSIMP_E_RESOURCE_EXPIRED => new(0x80290213);

    /// <summary>The resource type did not match.</summary>
    [Description("""The resource type did not match.""")]
    [HResultConstant(0x80290214)]
    public static HResult TBSIMP_E_INVALID_RESOURCE => new(0x80290214);

    /// <summary>No resources can be unloaded.</summary>
    [Description("""No resources can be unloaded.""")]
    [HResultConstant(0x80290215)]
    public static HResult TBSIMP_E_NOTHING_TO_UNLOAD => new(0x80290215);

    /// <summary>No new entries can be added to the hash table.</summary>
    [Description("""No new entries can be added to the hash table.""")]
    [HResultConstant(0x80290216)]
    public static HResult TBSIMP_E_HASH_TABLE_FULL => new(0x80290216);

    /// <summary>A new TBS context could not be created because there are too many open contexts.</summary>
    [Description("""A new TBS context could not be created because there are too many open contexts.""")]
    [HResultConstant(0x80290217)]
    public static HResult TBSIMP_E_TOO_MANY_TBS_CONTEXTS => new(0x80290217);

    /// <summary>A new virtual resource could not be created because there are too many open virtual resources.</summary>
    [Description("""A new virtual resource could not be created because there are too many open virtual resources.""")]
    [HResultConstant(0x80290218)]
    public static HResult TBSIMP_E_TOO_MANY_RESOURCES => new(0x80290218);

    /// <summary>The physical presence interface is not supported.</summary>
    [Description("""The physical presence interface is not supported.""")]
    [HResultConstant(0x80290219)]
    public static HResult TBSIMP_E_PPI_NOT_SUPPORTED => new(0x80290219);

    /// <summary>TBS is not compatible with the version of TPM found on the system.</summary>
    [Description("""TBS is not compatible with the version of TPM found on the system.""")]
    [HResultConstant(0x8029021A)]
    public static HResult TBSIMP_E_TPM_INCOMPATIBLE => new(0x8029021A);

    /// <summary>A general error was detected when attempting to acquire the BIOS response to a physical presence command.</summary>
    [Description("""A general error was detected when attempting to acquire the BIOS response to a physical presence command.""")]
    [HResultConstant(0x80290300)]
    public static HResult TPM_E_PPI_ACPI_FAILURE => new(0x80290300);

    /// <summary>The user failed to confirm the TPM operation request.</summary>
    [Description("""The user failed to confirm the TPM operation request.""")]
    [HResultConstant(0x80290301)]
    public static HResult TPM_E_PPI_USER_ABORT => new(0x80290301);

    /// <summary>The BIOS failure prevented the successful execution of the requested TPM operation (for example, invalid TPM operation request, BIOS communication error with the TPM).</summary>
    [Description("""The BIOS failure prevented the successful execution of the requested TPM operation (for example, invalid TPM operation request, BIOS communication error with the TPM).""")]
    [HResultConstant(0x80290302)]
    public static HResult TPM_E_PPI_BIOS_FAILURE => new(0x80290302);

    /// <summary>The BIOS does not support the physical presence interface.</summary>
    [Description("""The BIOS does not support the physical presence interface.""")]
    [HResultConstant(0x80290303)]
    public static HResult TPM_E_PPI_NOT_SUPPORTED => new(0x80290303);

    /// <summary>A Data Collector Set was not found.</summary>
    [Description("""A Data Collector Set was not found.""")]
    [HResultConstant(0x80300002)]
    public static HResult PLA_E_DCS_NOT_FOUND => new(0x80300002);

    /// <summary>Unable to start Data Collector Set because there are too many folders.</summary>
    [Description("""Unable to start Data Collector Set because there are too many folders.""")]
    [HResultConstant(0x80300045)]
    public static HResult PLA_E_TOO_MANY_FOLDERS => new(0x80300045);

    /// <summary>Not enough free disk space to start Data Collector Set.</summary>
    [Description("""Not enough free disk space to start Data Collector Set.""")]
    [HResultConstant(0x80300070)]
    public static HResult PLA_E_NO_MIN_DISK => new(0x80300070);

    /// <summary>Data Collector Set is in use.</summary>
    [Description("""Data Collector Set is in use.""")]
    [HResultConstant(0x803000AA)]
    public static HResult PLA_E_DCS_IN_USE => new(0x803000AA);

    /// <summary>Data Collector Set already exists.</summary>
    [Description("""Data Collector Set already exists.""")]
    [HResultConstant(0x803000B7)]
    public static HResult PLA_E_DCS_ALREADY_EXISTS => new(0x803000B7);

    /// <summary>Property value conflict.</summary>
    [Description("""Property value conflict.""")]
    [HResultConstant(0x80300101)]
    public static HResult PLA_E_PROPERTY_CONFLICT => new(0x80300101);

    /// <summary>The current configuration for this Data Collector Set requires that it contain exactly one Data Collector.</summary>
    [Description("""The current configuration for this Data Collector Set requires that it contain exactly one Data Collector.""")]
    [HResultConstant(0x80300102)]
    public static HResult PLA_E_DCS_SINGLETON_REQUIRED => new(0x80300102);

    /// <summary>A user account is required to commit the current Data Collector Set properties.</summary>
    [Description("""A user account is required to commit the current Data Collector Set properties.""")]
    [HResultConstant(0x80300103)]
    public static HResult PLA_E_CREDENTIALS_REQUIRED => new(0x80300103);

    /// <summary>Data Collector Set is not running.</summary>
    [Description("""Data Collector Set is not running.""")]
    [HResultConstant(0x80300104)]
    public static HResult PLA_E_DCS_NOT_RUNNING => new(0x80300104);

    /// <summary>A conflict was detected in the list of include and exclude APIs. Do not specify the same API in both the include list and the exclude list.</summary>
    [Description("""A conflict was detected in the list of include and exclude APIs. Do not specify the same API in both the include list and the exclude list.""")]
    [HResultConstant(0x80300105)]
    public static HResult PLA_E_CONFLICT_INCL_EXCL_API => new(0x80300105);

    /// <summary>The executable path specified refers to a network share or UNC path.</summary>
    [Description("""The executable path specified refers to a network share or UNC path.""")]
    [HResultConstant(0x80300106)]
    public static HResult PLA_E_NETWORK_EXE_NOT_VALID => new(0x80300106);

    /// <summary>The executable path specified is already configured for API tracing.</summary>
    [Description("""The executable path specified is already configured for API tracing.""")]
    [HResultConstant(0x80300107)]
    public static HResult PLA_E_EXE_ALREADY_CONFIGURED => new(0x80300107);

    /// <summary>The executable path specified does not exist. Verify that the specified path is correct.</summary>
    [Description("""The executable path specified does not exist. Verify that the specified path is correct.""")]
    [HResultConstant(0x80300108)]
    public static HResult PLA_E_EXE_PATH_NOT_VALID => new(0x80300108);

    /// <summary>Data Collector already exists.</summary>
    [Description("""Data Collector already exists.""")]
    [HResultConstant(0x80300109)]
    public static HResult PLA_E_DC_ALREADY_EXISTS => new(0x80300109);

    /// <summary>The wait for the Data Collector Set start notification has timed out.</summary>
    [Description("""The wait for the Data Collector Set start notification has timed out.""")]
    [HResultConstant(0x8030010A)]
    public static HResult PLA_E_DCS_START_WAIT_TIMEOUT => new(0x8030010A);

    /// <summary>The wait for the Data Collector to start has timed out.</summary>
    [Description("""The wait for the Data Collector to start has timed out.""")]
    [HResultConstant(0x8030010B)]
    public static HResult PLA_E_DC_START_WAIT_TIMEOUT => new(0x8030010B);

    /// <summary>The wait for the report generation tool to finish has timed out.</summary>
    [Description("""The wait for the report generation tool to finish has timed out.""")]
    [HResultConstant(0x8030010C)]
    public static HResult PLA_E_REPORT_WAIT_TIMEOUT => new(0x8030010C);

    /// <summary>Duplicate items are not allowed.</summary>
    [Description("""Duplicate items are not allowed.""")]
    [HResultConstant(0x8030010D)]
    public static HResult PLA_E_NO_DUPLICATES => new(0x8030010D);

    /// <summary>When specifying the executable to trace, you must specify a full path to the executable and not just a file name.</summary>
    [Description("""When specifying the executable to trace, you must specify a full path to the executable and not just a file name.""")]
    [HResultConstant(0x8030010E)]
    public static HResult PLA_E_EXE_FULL_PATH_REQUIRED => new(0x8030010E);

    /// <summary>The session name provided is invalid.</summary>
    [Description("""The session name provided is invalid.""")]
    [HResultConstant(0x8030010F)]
    public static HResult PLA_E_INVALID_SESSION_NAME => new(0x8030010F);

    /// <summary>The Event Log channel Microsoft-Windows-Diagnosis-PLA/Operational must be enabled to perform this operation.</summary>
    [Description("""The Event Log channel Microsoft-Windows-Diagnosis-PLA/Operational must be enabled to perform this operation.""")]
    [HResultConstant(0x80300110)]
    public static HResult PLA_E_PLA_CHANNEL_NOT_ENABLED => new(0x80300110);

    /// <summary>The Event Log channel Microsoft-Windows-TaskScheduler must be enabled to perform this operation.</summary>
    [Description("""The Event Log channel Microsoft-Windows-TaskScheduler must be enabled to perform this operation.""")]
    [HResultConstant(0x80300111)]
    public static HResult PLA_E_TASKSCHED_CHANNEL_NOT_ENABLED => new(0x80300111);

    /// <summary>The volume must be unlocked before it can be used.</summary>
    [Description("""The volume must be unlocked before it can be used.""")]
    [HResultConstant(0x80310000)]
    public static HResult FVE_E_LOCKED_VOLUME => new(0x80310000);

    /// <summary>The volume is fully decrypted and no key is available.</summary>
    [Description("""The volume is fully decrypted and no key is available.""")]
    [HResultConstant(0x80310001)]
    public static HResult FVE_E_NOT_ENCRYPTED => new(0x80310001);

    /// <summary>The firmware does not support using a TPM during boot.</summary>
    [Description("""The firmware does not support using a TPM during boot.""")]
    [HResultConstant(0x80310002)]
    public static HResult FVE_E_NO_TPM_BIOS => new(0x80310002);

    /// <summary>The firmware does not use a TPM to perform initial program load (IPL) measurement.</summary>
    [Description("""The firmware does not use a TPM to perform initial program load (IPL) measurement.""")]
    [HResultConstant(0x80310003)]
    public static HResult FVE_E_NO_MBR_METRIC => new(0x80310003);

    /// <summary>The master boot record (MBR) is not TPM-aware.</summary>
    [Description("""The master boot record (MBR) is not TPM-aware.""")]
    [HResultConstant(0x80310004)]
    public static HResult FVE_E_NO_BOOTSECTOR_METRIC => new(0x80310004);

    /// <summary>The BOOTMGR is not being measured by the TPM.</summary>
    [Description("""The BOOTMGR is not being measured by the TPM.""")]
    [HResultConstant(0x80310005)]
    public static HResult FVE_E_NO_BOOTMGR_METRIC => new(0x80310005);

    /// <summary>The BOOTMGR component does not perform expected TPM measurements.</summary>
    [Description("""The BOOTMGR component does not perform expected TPM measurements.""")]
    [HResultConstant(0x80310006)]
    public static HResult FVE_E_WRONG_BOOTMGR => new(0x80310006);

    /// <summary>No secure key protection mechanism has been defined.</summary>
    [Description("""No secure key protection mechanism has been defined.""")]
    [HResultConstant(0x80310007)]
    public static HResult FVE_E_SECURE_KEY_REQUIRED => new(0x80310007);

    /// <summary>This volume has not been provisioned for encryption.</summary>
    [Description("""This volume has not been provisioned for encryption.""")]
    [HResultConstant(0x80310008)]
    public static HResult FVE_E_NOT_ACTIVATED => new(0x80310008);

    /// <summary>Requested action was denied by the full-volume encryption (FVE) control engine.</summary>
    [Description("""Requested action was denied by the full-volume encryption (FVE) control engine.""")]
    [HResultConstant(0x80310009)]
    public static HResult FVE_E_ACTION_NOT_ALLOWED => new(0x80310009);

    /// <summary>The Active Directory forest does not contain the required attributes and classes to host FVE or TPM information.</summary>
    [Description("""The Active Directory forest does not contain the required attributes and classes to host FVE or TPM information.""")]
    [HResultConstant(0x8031000A)]
    public static HResult FVE_E_AD_SCHEMA_NOT_INSTALLED => new(0x8031000A);

    /// <summary>The type of data obtained from Active Directory was not expected.</summary>
    [Description("""The type of data obtained from Active Directory was not expected.""")]
    [HResultConstant(0x8031000B)]
    public static HResult FVE_E_AD_INVALID_DATATYPE => new(0x8031000B);

    /// <summary>The size of the data obtained from Active Directory was not expected.</summary>
    [Description("""The size of the data obtained from Active Directory was not expected.""")]
    [HResultConstant(0x8031000C)]
    public static HResult FVE_E_AD_INVALID_DATASIZE => new(0x8031000C);

    /// <summary>The attribute read from Active Directory has no (zero) values.</summary>
    [Description("""The attribute read from Active Directory has no (zero) values.""")]
    [HResultConstant(0x8031000D)]
    public static HResult FVE_E_AD_NO_VALUES => new(0x8031000D);

    /// <summary>The attribute was not set.</summary>
    [Description("""The attribute was not set.""")]
    [HResultConstant(0x8031000E)]
    public static HResult FVE_E_AD_ATTR_NOT_SET => new(0x8031000E);

    /// <summary>The specified GUID could not be found.</summary>
    [Description("""The specified GUID could not be found.""")]
    [HResultConstant(0x8031000F)]
    public static HResult FVE_E_AD_GUID_NOT_FOUND => new(0x8031000F);

    /// <summary>The control block for the encrypted volume is not valid.</summary>
    [Description("""The control block for the encrypted volume is not valid.""")]
    [HResultConstant(0x80310010)]
    public static HResult FVE_E_BAD_INFORMATION => new(0x80310010);

    /// <summary>Not enough free space remaining on volume to allow encryption.</summary>
    [Description("""Not enough free space remaining on volume to allow encryption.""")]
    [HResultConstant(0x80310011)]
    public static HResult FVE_E_TOO_SMALL => new(0x80310011);

    /// <summary>The volume cannot be encrypted because it is required to boot the operating system.</summary>
    [Description("""The volume cannot be encrypted because it is required to boot the operating system.""")]
    [HResultConstant(0x80310012)]
    public static HResult FVE_E_SYSTEM_VOLUME => new(0x80310012);

    /// <summary>The volume cannot be encrypted because the file system is not supported.</summary>
    [Description("""The volume cannot be encrypted because the file system is not supported.""")]
    [HResultConstant(0x80310013)]
    public static HResult FVE_E_FAILED_WRONG_FS => new(0x80310013);

    /// <summary>The file system is inconsistent. Run CHKDSK.</summary>
    [Description("""The file system is inconsistent. Run CHKDSK.""")]
    [HResultConstant(0x80310014)]
    public static HResult FVE_E_FAILED_BAD_FS => new(0x80310014);

    /// <summary>This volume cannot be encrypted.</summary>
    [Description("""This volume cannot be encrypted.""")]
    [HResultConstant(0x80310015)]
    public static HResult FVE_E_NOT_SUPPORTED => new(0x80310015);

    /// <summary>Data supplied is malformed.</summary>
    [Description("""Data supplied is malformed.""")]
    [HResultConstant(0x80310016)]
    public static HResult FVE_E_BAD_DATA => new(0x80310016);

    /// <summary>Volume is not bound to the system.</summary>
    [Description("""Volume is not bound to the system.""")]
    [HResultConstant(0x80310017)]
    public static HResult FVE_E_VOLUME_NOT_BOUND => new(0x80310017);

    /// <summary>TPM must be owned before a volume can be bound to it.</summary>
    [Description("""TPM must be owned before a volume can be bound to it.""")]
    [HResultConstant(0x80310018)]
    public static HResult FVE_E_TPM_NOT_OWNED => new(0x80310018);

    /// <summary>The volume specified is not a data volume.</summary>
    [Description("""The volume specified is not a data volume.""")]
    [HResultConstant(0x80310019)]
    public static HResult FVE_E_NOT_DATA_VOLUME => new(0x80310019);

    /// <summary>The buffer supplied to a function was insufficient to contain the returned data.</summary>
    [Description("""The buffer supplied to a function was insufficient to contain the returned data.""")]
    [HResultConstant(0x8031001A)]
    public static HResult FVE_E_AD_INSUFFICIENT_BUFFER => new(0x8031001A);

    /// <summary>A read operation failed while converting the volume.</summary>
    [Description("""A read operation failed while converting the volume.""")]
    [HResultConstant(0x8031001B)]
    public static HResult FVE_E_CONV_READ => new(0x8031001B);

    /// <summary>A write operation failed while converting the volume.</summary>
    [Description("""A write operation failed while converting the volume.""")]
    [HResultConstant(0x8031001C)]
    public static HResult FVE_E_CONV_WRITE => new(0x8031001C);

    /// <summary>One or more key protection mechanisms are required for this volume.</summary>
    [Description("""One or more key protection mechanisms are required for this volume.""")]
    [HResultConstant(0x8031001D)]
    public static HResult FVE_E_KEY_REQUIRED => new(0x8031001D);

    /// <summary>Cluster configurations are not supported.</summary>
    [Description("""Cluster configurations are not supported.""")]
    [HResultConstant(0x8031001E)]
    public static HResult FVE_E_CLUSTERING_NOT_SUPPORTED => new(0x8031001E);

    /// <summary>The volume is already bound to the system.</summary>
    [Description("""The volume is already bound to the system.""")]
    [HResultConstant(0x8031001F)]
    public static HResult FVE_E_VOLUME_BOUND_ALREADY => new(0x8031001F);

    /// <summary>The boot OS volume is not being protected via FVE.</summary>
    [Description("""The boot OS volume is not being protected via FVE.""")]
    [HResultConstant(0x80310020)]
    public static HResult FVE_E_OS_NOT_PROTECTED => new(0x80310020);

    /// <summary>All protection mechanisms are effectively disabled (clear key exists).</summary>
    [Description("""All protection mechanisms are effectively disabled (clear key exists).""")]
    [HResultConstant(0x80310021)]
    public static HResult FVE_E_PROTECTION_DISABLED => new(0x80310021);

    /// <summary>A recovery key protection mechanism is required.</summary>
    [Description("""A recovery key protection mechanism is required.""")]
    [HResultConstant(0x80310022)]
    public static HResult FVE_E_RECOVERY_KEY_REQUIRED => new(0x80310022);

    /// <summary>This volume cannot be bound to a TPM.</summary>
    [Description("""This volume cannot be bound to a TPM.""")]
    [HResultConstant(0x80310023)]
    public static HResult FVE_E_FOREIGN_VOLUME => new(0x80310023);

    /// <summary>The control block for the encrypted volume was updated by another thread. Try again.</summary>
    [Description("""The control block for the encrypted volume was updated by another thread. Try again.""")]
    [HResultConstant(0x80310024)]
    public static HResult FVE_E_OVERLAPPED_UPDATE => new(0x80310024);

    /// <summary>The SRK authentication of the TPM is not zero and, therefore, is not compatible.</summary>
    [Description("""The SRK authentication of the TPM is not zero and, therefore, is not compatible.""")]
    [HResultConstant(0x80310025)]
    public static HResult FVE_E_TPM_SRK_AUTH_NOT_ZERO => new(0x80310025);

    /// <summary>The volume encryption algorithm cannot be used on this sector size.</summary>
    [Description("""The volume encryption algorithm cannot be used on this sector size.""")]
    [HResultConstant(0x80310026)]
    public static HResult FVE_E_FAILED_SECTOR_SIZE => new(0x80310026);

    /// <summary>BitLocker recovery authentication failed.</summary>
    [Description("""BitLocker recovery authentication failed.""")]
    [HResultConstant(0x80310027)]
    public static HResult FVE_E_FAILED_AUTHENTICATION => new(0x80310027);

    /// <summary>The volume specified is not the boot OS volume.</summary>
    [Description("""The volume specified is not the boot OS volume.""")]
    [HResultConstant(0x80310028)]
    public static HResult FVE_E_NOT_OS_VOLUME => new(0x80310028);

    /// <summary>Auto-unlock information for data volumes is present on the boot OS volume.</summary>
    [Description("""Auto-unlock information for data volumes is present on the boot OS volume.""")]
    [HResultConstant(0x80310029)]
    public static HResult FVE_E_AUTOUNLOCK_ENABLED => new(0x80310029);

    /// <summary>The system partition boot sector does not perform TPM measurements.</summary>
    [Description("""The system partition boot sector does not perform TPM measurements.""")]
    [HResultConstant(0x8031002A)]
    public static HResult FVE_E_WRONG_BOOTSECTOR => new(0x8031002A);

    /// <summary>The system partition file system must be NTFS.</summary>
    [Description("""The system partition file system must be NTFS.""")]
    [HResultConstant(0x8031002B)]
    public static HResult FVE_E_WRONG_SYSTEM_FS => new(0x8031002B);

    /// <summary>Group policy requires a recovery password before encryption can begin.</summary>
    [Description("""Group policy requires a recovery password before encryption can begin.""")]
    [HResultConstant(0x8031002C)]
    public static HResult FVE_E_POLICY_PASSWORD_REQUIRED => new(0x8031002C);

    /// <summary>The volume encryption algorithm and key cannot be set on an encrypted volume.</summary>
    [Description("""The volume encryption algorithm and key cannot be set on an encrypted volume.""")]
    [HResultConstant(0x8031002D)]
    public static HResult FVE_E_CANNOT_SET_FVEK_ENCRYPTED => new(0x8031002D);

    /// <summary>A key must be specified before encryption can begin.</summary>
    [Description("""A key must be specified before encryption can begin.""")]
    [HResultConstant(0x8031002E)]
    public static HResult FVE_E_CANNOT_ENCRYPT_NO_KEY => new(0x8031002E);

    /// <summary>A bootable CD/DVD is in the system. Remove the CD/DVD and reboot the system.</summary>
    [Description("""A bootable CD/DVD is in the system. Remove the CD/DVD and reboot the system.""")]
    [HResultConstant(0x80310030)]
    public static HResult FVE_E_BOOTABLE_CDDVD => new(0x80310030);

    /// <summary>An instance of this key protector already exists on the volume.</summary>
    [Description("""An instance of this key protector already exists on the volume.""")]
    [HResultConstant(0x80310031)]
    public static HResult FVE_E_PROTECTOR_EXISTS => new(0x80310031);

    /// <summary>The file cannot be saved to a relative path.</summary>
    [Description("""The file cannot be saved to a relative path.""")]
    [HResultConstant(0x80310032)]
    public static HResult FVE_E_RELATIVE_PATH => new(0x80310032);

    /// <summary>The callout does not exist.</summary>
    [Description("""The callout does not exist.""")]
    [HResultConstant(0x80320001)]
    public static HResult FWP_E_CALLOUT_NOT_FOUND => new(0x80320001);

    /// <summary>The filter condition does not exist.</summary>
    [Description("""The filter condition does not exist.""")]
    [HResultConstant(0x80320002)]
    public static HResult FWP_E_CONDITION_NOT_FOUND => new(0x80320002);

    /// <summary>The filter does not exist.</summary>
    [Description("""The filter does not exist.""")]
    [HResultConstant(0x80320003)]
    public static HResult FWP_E_FILTER_NOT_FOUND => new(0x80320003);

    /// <summary>The layer does not exist.</summary>
    [Description("""The layer does not exist.""")]
    [HResultConstant(0x80320004)]
    public static HResult FWP_E_LAYER_NOT_FOUND => new(0x80320004);

    /// <summary>The provider does not exist.</summary>
    [Description("""The provider does not exist.""")]
    [HResultConstant(0x80320005)]
    public static HResult FWP_E_PROVIDER_NOT_FOUND => new(0x80320005);

    /// <summary>The provider context does not exist.</summary>
    [Description("""The provider context does not exist.""")]
    [HResultConstant(0x80320006)]
    public static HResult FWP_E_PROVIDER_CONTEXT_NOT_FOUND => new(0x80320006);

    /// <summary>The sublayer does not exist.</summary>
    [Description("""The sublayer does not exist.""")]
    [HResultConstant(0x80320007)]
    public static HResult FWP_E_SUBLAYER_NOT_FOUND => new(0x80320007);

    /// <summary>The object does not exist.</summary>
    [Description("""The object does not exist.""")]
    [HResultConstant(0x80320008)]
    public static HResult FWP_E_NOT_FOUND => new(0x80320008);

    /// <summary>An object with that GUID or LUID already exists.</summary>
    [Description("""An object with that GUID or LUID already exists.""")]
    [HResultConstant(0x80320009)]
    public static HResult FWP_E_ALREADY_EXISTS => new(0x80320009);

    /// <summary>The object is referenced by other objects and, therefore, cannot be deleted.</summary>
    [Description("""The object is referenced by other objects and, therefore, cannot be deleted.""")]
    [HResultConstant(0x8032000A)]
    public static HResult FWP_E_IN_USE => new(0x8032000A);

    /// <summary>The call is not allowed from within a dynamic session.</summary>
    [Description("""The call is not allowed from within a dynamic session.""")]
    [HResultConstant(0x8032000B)]
    public static HResult FWP_E_DYNAMIC_SESSION_IN_PROGRESS => new(0x8032000B);

    /// <summary>The call was made from the wrong session and, therefore, cannot be completed.</summary>
    [Description("""The call was made from the wrong session and, therefore, cannot be completed.""")]
    [HResultConstant(0x8032000C)]
    public static HResult FWP_E_WRONG_SESSION => new(0x8032000C);

    /// <summary>The call must be made from within an explicit transaction.</summary>
    [Description("""The call must be made from within an explicit transaction.""")]
    [HResultConstant(0x8032000D)]
    public static HResult FWP_E_NO_TXN_IN_PROGRESS => new(0x8032000D);

    /// <summary>The call is not allowed from within an explicit transaction.</summary>
    [Description("""The call is not allowed from within an explicit transaction.""")]
    [HResultConstant(0x8032000E)]
    public static HResult FWP_E_TXN_IN_PROGRESS => new(0x8032000E);

    /// <summary>The explicit transaction has been forcibly canceled.</summary>
    [Description("""The explicit transaction has been forcibly canceled.""")]
    [HResultConstant(0x8032000F)]
    public static HResult FWP_E_TXN_ABORTED => new(0x8032000F);

    /// <summary>The session has been canceled.</summary>
    [Description("""The session has been canceled.""")]
    [HResultConstant(0x80320010)]
    public static HResult FWP_E_SESSION_ABORTED => new(0x80320010);

    /// <summary>The call is not allowed from within a read-only transaction.</summary>
    [Description("""The call is not allowed from within a read-only transaction.""")]
    [HResultConstant(0x80320011)]
    public static HResult FWP_E_INCOMPATIBLE_TXN => new(0x80320011);

    /// <summary>The call timed out while waiting to acquire the transaction lock.</summary>
    [Description("""The call timed out while waiting to acquire the transaction lock.""")]
    [HResultConstant(0x80320012)]
    public static HResult FWP_E_TIMEOUT => new(0x80320012);

    /// <summary>Collection of network diagnostic events is disabled.</summary>
    [Description("""Collection of network diagnostic events is disabled.""")]
    [HResultConstant(0x80320013)]
    public static HResult FWP_E_NET_EVENTS_DISABLED => new(0x80320013);

    /// <summary>The operation is not supported by the specified layer.</summary>
    [Description("""The operation is not supported by the specified layer.""")]
    [HResultConstant(0x80320014)]
    public static HResult FWP_E_INCOMPATIBLE_LAYER => new(0x80320014);

    /// <summary>The call is allowed for kernel-mode callers only.</summary>
    [Description("""The call is allowed for kernel-mode callers only.""")]
    [HResultConstant(0x80320015)]
    public static HResult FWP_E_KM_CLIENTS_ONLY => new(0x80320015);

    /// <summary>The call tried to associate two objects with incompatible lifetimes.</summary>
    [Description("""The call tried to associate two objects with incompatible lifetimes.""")]
    [HResultConstant(0x80320016)]
    public static HResult FWP_E_LIFETIME_MISMATCH => new(0x80320016);

    /// <summary>The object is built in and, therefore, cannot be deleted.</summary>
    [Description("""The object is built in and, therefore, cannot be deleted.""")]
    [HResultConstant(0x80320017)]
    public static HResult FWP_E_BUILTIN_OBJECT => new(0x80320017);

    /// <summary>The maximum number of boot-time filters has been reached.</summary>
    [Description("""The maximum number of boot-time filters has been reached.""")]
    [HResultConstant(0x80320018)]
    public static HResult FWP_E_TOO_MANY_BOOTTIME_FILTERS => new(0x80320018);

    /// <summary>A notification could not be delivered because a message queue is at its maximum capacity.</summary>
    [Description("""A notification could not be delivered because a message queue is at its maximum capacity.""")]
    [HResultConstant(0x80320019)]
    public static HResult FWP_E_NOTIFICATION_DROPPED => new(0x80320019);

    /// <summary>The traffic parameters do not match those for the security association context.</summary>
    [Description("""The traffic parameters do not match those for the security association context.""")]
    [HResultConstant(0x8032001A)]
    public static HResult FWP_E_TRAFFIC_MISMATCH => new(0x8032001A);

    /// <summary>The call is not allowed for the current security association state.</summary>
    [Description("""The call is not allowed for the current security association state.""")]
    [HResultConstant(0x8032001B)]
    public static HResult FWP_E_INCOMPATIBLE_SA_STATE => new(0x8032001B);

    /// <summary>A required pointer is null.</summary>
    [Description("""A required pointer is null.""")]
    [HResultConstant(0x8032001C)]
    public static HResult FWP_E_NULL_POINTER => new(0x8032001C);

    /// <summary>An enumerator is not valid.</summary>
    [Description("""An enumerator is not valid.""")]
    [HResultConstant(0x8032001D)]
    public static HResult FWP_E_INVALID_ENUMERATOR => new(0x8032001D);

    /// <summary>The flags field contains an invalid value.</summary>
    [Description("""The flags field contains an invalid value.""")]
    [HResultConstant(0x8032001E)]
    public static HResult FWP_E_INVALID_FLAGS => new(0x8032001E);

    /// <summary>A network mask is not valid.</summary>
    [Description("""A network mask is not valid.""")]
    [HResultConstant(0x8032001F)]
    public static HResult FWP_E_INVALID_NET_MASK => new(0x8032001F);

    /// <summary>An FWP_RANGE is not valid.</summary>
    [Description("""An FWP_RANGE is not valid.""")]
    [HResultConstant(0x80320020)]
    public static HResult FWP_E_INVALID_RANGE => new(0x80320020);

    /// <summary>The time interval is not valid.</summary>
    [Description("""The time interval is not valid.""")]
    [HResultConstant(0x80320021)]
    public static HResult FWP_E_INVALID_INTERVAL => new(0x80320021);

    /// <summary>An array that must contain at least one element that is zero-length.</summary>
    [Description("""An array that must contain at least one element that is zero-length.""")]
    [HResultConstant(0x80320022)]
    public static HResult FWP_E_ZERO_LENGTH_ARRAY => new(0x80320022);

    /// <summary>The displayData.name field cannot be null.</summary>
    [Description("""The displayData.name field cannot be null.""")]
    [HResultConstant(0x80320023)]
    public static HResult FWP_E_NULL_DISPLAY_NAME => new(0x80320023);

    /// <summary>The action type is not one of the allowed action types for a filter.</summary>
    [Description("""The action type is not one of the allowed action types for a filter.""")]
    [HResultConstant(0x80320024)]
    public static HResult FWP_E_INVALID_ACTION_TYPE => new(0x80320024);

    /// <summary>The filter weight is not valid.</summary>
    [Description("""The filter weight is not valid.""")]
    [HResultConstant(0x80320025)]
    public static HResult FWP_E_INVALID_WEIGHT => new(0x80320025);

    /// <summary>A filter condition contains a match type that is not compatible with the operands.</summary>
    [Description("""A filter condition contains a match type that is not compatible with the operands.""")]
    [HResultConstant(0x80320026)]
    public static HResult FWP_E_MATCH_TYPE_MISMATCH => new(0x80320026);

    /// <summary>An FWP_VALUE or FWPM_CONDITION_VALUE is of the wrong type.</summary>
    [Description("""An FWP_VALUE or FWPM_CONDITION_VALUE is of the wrong type.""")]
    [HResultConstant(0x80320027)]
    public static HResult FWP_E_TYPE_MISMATCH => new(0x80320027);

    /// <summary>An integer value is outside the allowed range.</summary>
    [Description("""An integer value is outside the allowed range.""")]
    [HResultConstant(0x80320028)]
    public static HResult FWP_E_OUT_OF_BOUNDS => new(0x80320028);

    /// <summary>A reserved field is nonzero.</summary>
    [Description("""A reserved field is nonzero.""")]
    [HResultConstant(0x80320029)]
    public static HResult FWP_E_RESERVED => new(0x80320029);

    /// <summary>A filter cannot contain multiple conditions operating on a single field.</summary>
    [Description("""A filter cannot contain multiple conditions operating on a single field.""")]
    [HResultConstant(0x8032002A)]
    public static HResult FWP_E_DUPLICATE_CONDITION => new(0x8032002A);

    /// <summary>A policy cannot contain the same keying module more than once.</summary>
    [Description("""A policy cannot contain the same keying module more than once.""")]
    [HResultConstant(0x8032002B)]
    public static HResult FWP_E_DUPLICATE_KEYMOD => new(0x8032002B);

    /// <summary>The action type is not compatible with the layer.</summary>
    [Description("""The action type is not compatible with the layer.""")]
    [HResultConstant(0x8032002C)]
    public static HResult FWP_E_ACTION_INCOMPATIBLE_WITH_LAYER => new(0x8032002C);

    /// <summary>The action type is not compatible with the sublayer.</summary>
    [Description("""The action type is not compatible with the sublayer.""")]
    [HResultConstant(0x8032002D)]
    public static HResult FWP_E_ACTION_INCOMPATIBLE_WITH_SUBLAYER => new(0x8032002D);

    /// <summary>The raw context or the provider context is not compatible with the layer.</summary>
    [Description("""The raw context or the provider context is not compatible with the layer.""")]
    [HResultConstant(0x8032002E)]
    public static HResult FWP_E_CONTEXT_INCOMPATIBLE_WITH_LAYER => new(0x8032002E);

    /// <summary>The raw context or the provider context is not compatible with the callout.</summary>
    [Description("""The raw context or the provider context is not compatible with the callout.""")]
    [HResultConstant(0x8032002F)]
    public static HResult FWP_E_CONTEXT_INCOMPATIBLE_WITH_CALLOUT => new(0x8032002F);

    /// <summary>The authentication method is not compatible with the policy type.</summary>
    [Description("""The authentication method is not compatible with the policy type.""")]
    [HResultConstant(0x80320030)]
    public static HResult FWP_E_INCOMPATIBLE_AUTH_METHOD => new(0x80320030);

    /// <summary>The Diffie-Hellman group is not compatible with the policy type.</summary>
    [Description("""The Diffie-Hellman group is not compatible with the policy type.""")]
    [HResultConstant(0x80320031)]
    public static HResult FWP_E_INCOMPATIBLE_DH_GROUP => new(0x80320031);

    /// <summary>An Internet Key Exchange (IKE) policy cannot contain an Extended Mode policy.</summary>
    [Description("""An Internet Key Exchange (IKE) policy cannot contain an Extended Mode policy.""")]
    [HResultConstant(0x80320032)]
    public static HResult FWP_E_EM_NOT_SUPPORTED => new(0x80320032);

    /// <summary>The enumeration template or subscription will never match any objects.</summary>
    [Description("""The enumeration template or subscription will never match any objects.""")]
    [HResultConstant(0x80320033)]
    public static HResult FWP_E_NEVER_MATCH => new(0x80320033);

    /// <summary>The provider context is of the wrong type.</summary>
    [Description("""The provider context is of the wrong type.""")]
    [HResultConstant(0x80320034)]
    public static HResult FWP_E_PROVIDER_CONTEXT_MISMATCH => new(0x80320034);

    /// <summary>The parameter is incorrect.</summary>
    [Description("""The parameter is incorrect.""")]
    [HResultConstant(0x80320035)]
    public static HResult FWP_E_INVALID_PARAMETER => new(0x80320035);

    /// <summary>The maximum number of sublayers has been reached.</summary>
    [Description("""The maximum number of sublayers has been reached.""")]
    [HResultConstant(0x80320036)]
    public static HResult FWP_E_TOO_MANY_SUBLAYERS => new(0x80320036);

    /// <summary>The notification function for a callout returned an error.</summary>
    [Description("""The notification function for a callout returned an error.""")]
    [HResultConstant(0x80320037)]
    public static HResult FWP_E_CALLOUT_NOTIFICATION_FAILED => new(0x80320037);

    /// <summary>The IPsec authentication configuration is not compatible with the authentication type.</summary>
    [Description("""The IPsec authentication configuration is not compatible with the authentication type.""")]
    [HResultConstant(0x80320038)]
    public static HResult FWP_E_INCOMPATIBLE_AUTH_CONFIG => new(0x80320038);

    /// <summary>The IPsec cipher configuration is not compatible with the cipher type.</summary>
    [Description("""The IPsec cipher configuration is not compatible with the cipher type.""")]
    [HResultConstant(0x80320039)]
    public static HResult FWP_E_INCOMPATIBLE_CIPHER_CONFIG => new(0x80320039);

    /// <summary>The binding to the network interface is being closed.</summary>
    [Description("""The binding to the network interface is being closed.""")]
    [HResultConstant(0x80340002)]
    public static HResult ERROR_NDIS_INTERFACE_CLOSING => new(0x80340002);

    /// <summary>An invalid version was specified.</summary>
    [Description("""An invalid version was specified.""")]
    [HResultConstant(0x80340004)]
    public static HResult ERROR_NDIS_BAD_VERSION => new(0x80340004);

    /// <summary>An invalid characteristics table was used.</summary>
    [Description("""An invalid characteristics table was used.""")]
    [HResultConstant(0x80340005)]
    public static HResult ERROR_NDIS_BAD_CHARACTERISTICS => new(0x80340005);

    /// <summary>Failed to find the network interface, or the network interface is not ready.</summary>
    [Description("""Failed to find the network interface, or the network interface is not ready.""")]
    [HResultConstant(0x80340006)]
    public static HResult ERROR_NDIS_ADAPTER_NOT_FOUND => new(0x80340006);

    /// <summary>Failed to open the network interface.</summary>
    [Description("""Failed to open the network interface.""")]
    [HResultConstant(0x80340007)]
    public static HResult ERROR_NDIS_OPEN_FAILED => new(0x80340007);

    /// <summary>The network interface has encountered an internal unrecoverable failure.</summary>
    [Description("""The network interface has encountered an internal unrecoverable failure.""")]
    [HResultConstant(0x80340008)]
    public static HResult ERROR_NDIS_DEVICE_FAILED => new(0x80340008);

    /// <summary>The multicast list on the network interface is full.</summary>
    [Description("""The multicast list on the network interface is full.""")]
    [HResultConstant(0x80340009)]
    public static HResult ERROR_NDIS_MULTICAST_FULL => new(0x80340009);

    /// <summary>An attempt was made to add a duplicate multicast address to the list.</summary>
    [Description("""An attempt was made to add a duplicate multicast address to the list.""")]
    [HResultConstant(0x8034000A)]
    public static HResult ERROR_NDIS_MULTICAST_EXISTS => new(0x8034000A);

    /// <summary>At attempt was made to remove a multicast address that was never added.</summary>
    [Description("""At attempt was made to remove a multicast address that was never added.""")]
    [HResultConstant(0x8034000B)]
    public static HResult ERROR_NDIS_MULTICAST_NOT_FOUND => new(0x8034000B);

    /// <summary>The network interface aborted the request.</summary>
    [Description("""The network interface aborted the request.""")]
    [HResultConstant(0x8034000C)]
    public static HResult ERROR_NDIS_REQUEST_ABORTED => new(0x8034000C);

    /// <summary>The network interface cannot process the request because it is being reset.</summary>
    [Description("""The network interface cannot process the request because it is being reset.""")]
    [HResultConstant(0x8034000D)]
    public static HResult ERROR_NDIS_RESET_IN_PROGRESS => new(0x8034000D);

    /// <summary>An attempt was made to send an invalid packet on a network interface.</summary>
    [Description("""An attempt was made to send an invalid packet on a network interface.""")]
    [HResultConstant(0x8034000F)]
    public static HResult ERROR_NDIS_INVALID_PACKET => new(0x8034000F);

    /// <summary>The specified request is not a valid operation for the target device.</summary>
    [Description("""The specified request is not a valid operation for the target device.""")]
    [HResultConstant(0x80340010)]
    public static HResult ERROR_NDIS_INVALID_DEVICE_REQUEST => new(0x80340010);

    /// <summary>The network interface is not ready to complete this operation.</summary>
    [Description("""The network interface is not ready to complete this operation.""")]
    [HResultConstant(0x80340011)]
    public static HResult ERROR_NDIS_ADAPTER_NOT_READY => new(0x80340011);

    /// <summary>The length of the buffer submitted for this operation is not valid.</summary>
    [Description("""The length of the buffer submitted for this operation is not valid.""")]
    [HResultConstant(0x80340014)]
    public static HResult ERROR_NDIS_INVALID_LENGTH => new(0x80340014);

    /// <summary>The data used for this operation is not valid.</summary>
    [Description("""The data used for this operation is not valid.""")]
    [HResultConstant(0x80340015)]
    public static HResult ERROR_NDIS_INVALID_DATA => new(0x80340015);

    /// <summary>The length of the buffer submitted for this operation is too small.</summary>
    [Description("""The length of the buffer submitted for this operation is too small.""")]
    [HResultConstant(0x80340016)]
    public static HResult ERROR_NDIS_BUFFER_TOO_SHORT => new(0x80340016);

    /// <summary>The network interface does not support this OID.</summary>
    [Description("""The network interface does not support this OID.""")]
    [HResultConstant(0x80340017)]
    public static HResult ERROR_NDIS_INVALID_OID => new(0x80340017);

    /// <summary>The network interface has been removed.</summary>
    [Description("""The network interface has been removed.""")]
    [HResultConstant(0x80340018)]
    public static HResult ERROR_NDIS_ADAPTER_REMOVED => new(0x80340018);

    /// <summary>The network interface does not support this media type.</summary>
    [Description("""The network interface does not support this media type.""")]
    [HResultConstant(0x80340019)]
    public static HResult ERROR_NDIS_UNSUPPORTED_MEDIA => new(0x80340019);

    /// <summary>An attempt was made to remove a token ring group address that is in use by other components.</summary>
    [Description("""An attempt was made to remove a token ring group address that is in use by other components.""")]
    [HResultConstant(0x8034001A)]
    public static HResult ERROR_NDIS_GROUP_ADDRESS_IN_USE => new(0x8034001A);

    /// <summary>An attempt was made to map a file that cannot be found.</summary>
    [Description("""An attempt was made to map a file that cannot be found.""")]
    [HResultConstant(0x8034001B)]
    public static HResult ERROR_NDIS_FILE_NOT_FOUND => new(0x8034001B);

    /// <summary>An error occurred while the NDIS tried to map the file.</summary>
    [Description("""An error occurred while the NDIS tried to map the file.""")]
    [HResultConstant(0x8034001C)]
    public static HResult ERROR_NDIS_ERROR_READING_FILE => new(0x8034001C);

    /// <summary>An attempt was made to map a file that is already mapped.</summary>
    [Description("""An attempt was made to map a file that is already mapped.""")]
    [HResultConstant(0x8034001D)]
    public static HResult ERROR_NDIS_ALREADY_MAPPED => new(0x8034001D);

    /// <summary>An attempt to allocate a hardware resource failed because the resource is used by another component.</summary>
    [Description("""An attempt to allocate a hardware resource failed because the resource is used by another component.""")]
    [HResultConstant(0x8034001E)]
    public static HResult ERROR_NDIS_RESOURCE_CONFLICT => new(0x8034001E);

    /// <summary>The I/O operation failed because network media is disconnected or the wireless access point is out of range.</summary>
    [Description("""The I/O operation failed because network media is disconnected or the wireless access point is out of range.""")]
    [HResultConstant(0x8034001F)]
    public static HResult ERROR_NDIS_MEDIA_DISCONNECTED => new(0x8034001F);

    /// <summary>The network address used in the request is invalid.</summary>
    [Description("""The network address used in the request is invalid.""")]
    [HResultConstant(0x80340022)]
    public static HResult ERROR_NDIS_INVALID_ADDRESS => new(0x80340022);

    /// <summary>The offload operation on the network interface has been paused.</summary>
    [Description("""The offload operation on the network interface has been paused.""")]
    [HResultConstant(0x8034002A)]
    public static HResult ERROR_NDIS_PAUSED => new(0x8034002A);

    /// <summary>The network interface was not found.</summary>
    [Description("""The network interface was not found.""")]
    [HResultConstant(0x8034002B)]
    public static HResult ERROR_NDIS_INTERFACE_NOT_FOUND => new(0x8034002B);

    /// <summary>The revision number specified in the structure is not supported.</summary>
    [Description("""The revision number specified in the structure is not supported.""")]
    [HResultConstant(0x8034002C)]
    public static HResult ERROR_NDIS_UNSUPPORTED_REVISION => new(0x8034002C);

    /// <summary>The specified port does not exist on this network interface.</summary>
    [Description("""The specified port does not exist on this network interface.""")]
    [HResultConstant(0x8034002D)]
    public static HResult ERROR_NDIS_INVALID_PORT => new(0x8034002D);

    /// <summary>The current state of the specified port on this network interface does not support the requested operation.</summary>
    [Description("""The current state of the specified port on this network interface does not support the requested operation.""")]
    [HResultConstant(0x8034002E)]
    public static HResult ERROR_NDIS_INVALID_PORT_STATE => new(0x8034002E);

    /// <summary>The network interface does not support this request.</summary>
    [Description("""The network interface does not support this request.""")]
    [HResultConstant(0x803400BB)]
    public static HResult ERROR_NDIS_NOT_SUPPORTED => new(0x803400BB);

    /// <summary>The wireless local area network (LAN) interface is in auto-configuration mode and does not support the requested parameter change operation.</summary>
    [Description("""The wireless local area network (LAN) interface is in auto-configuration mode and does not support the requested parameter change operation.""")]
    [HResultConstant(0x80342000)]
    public static HResult ERROR_NDIS_DOT11_AUTO_CONFIG_ENABLED => new(0x80342000);

    /// <summary>The wireless LAN interface is busy and cannot perform the requested operation.</summary>
    [Description("""The wireless LAN interface is busy and cannot perform the requested operation.""")]
    [HResultConstant(0x80342001)]
    public static HResult ERROR_NDIS_DOT11_MEDIA_IN_USE => new(0x80342001);

    /// <summary>The wireless LAN interface is shutting down and does not support the requested operation.</summary>
    [Description("""The wireless LAN interface is shutting down and does not support the requested operation.""")]
    [HResultConstant(0x80342002)]
    public static HResult ERROR_NDIS_DOT11_POWER_STATE_INVALID => new(0x80342002);

    /// <summary>A requested object was not found.</summary>
    [Description("""A requested object was not found.""")]
    [HResultConstant(0x8DEAD01B)]
    public static HResult TRK_E_NOT_FOUND => new(0x8DEAD01B);

    /// <summary>The server received a CREATE_VOLUME subrequest of a SYNC_VOLUMES request, but the ServerVolumeTable size limit for the RequestMachine has already been reached.</summary>
    [Description("""The server received a CREATE_VOLUME subrequest of a SYNC_VOLUMES request, but the ServerVolumeTable size limit for the RequestMachine has already been reached.""")]
    [HResultConstant(0x8DEAD01C)]
    public static HResult TRK_E_VOLUME_QUOTA_EXCEEDED => new(0x8DEAD01C);

    /// <summary>The server is busy, and the client should retry the request at a later time.</summary>
    [Description("""The server is busy, and the client should retry the request at a later time.""")]
    [HResultConstant(0x8DEAD01E)]
    public static HResult TRK_SERVER_TOO_BUSY => new(0x8DEAD01E);

    /// <summary>The specified event is currently not being audited.</summary>
    [Description("""The specified event is currently not being audited.""")]
    [HResultConstant(0xC0090001)]
    public static HResult ERROR_AUDITING_DISABLED => new(0xC0090001);

    /// <summary>The SID filtering operation removed all SIDs.</summary>
    [Description("""The SID filtering operation removed all SIDs.""")]
    [HResultConstant(0xC0090002)]
    public static HResult ERROR_ALL_SIDS_FILTERED => new(0xC0090002);

    /// <summary>Business rule scripts are disabled for the calling application.</summary>
    [Description("""Business rule scripts are disabled for the calling application.""")]
    [HResultConstant(0xC0090003)]
    public static HResult ERROR_BIZRULES_NOT_ENABLED => new(0xC0090003);

    /// <summary>There is no connection established with the Windows Media server. The operation failed.</summary>
    [Description("""There is no connection established with the Windows Media server. The operation failed.""")]
    [HResultConstant(0xC00D0005)]
    public static HResult NS_E_NOCONNECTION => new(0xC00D0005);

    /// <summary>Unable to establish a connection to the server.</summary>
    [Description("""Unable to establish a connection to the server.""")]
    [HResultConstant(0xC00D0006)]
    public static HResult NS_E_CANNOTCONNECT => new(0xC00D0006);

    /// <summary>Unable to destroy the title.</summary>
    [Description("""Unable to destroy the title.""")]
    [HResultConstant(0xC00D0007)]
    public static HResult NS_E_CANNOTDESTROYTITLE => new(0xC00D0007);

    /// <summary>Unable to rename the title.</summary>
    [Description("""Unable to rename the title.""")]
    [HResultConstant(0xC00D0008)]
    public static HResult NS_E_CANNOTRENAMETITLE => new(0xC00D0008);

    /// <summary>Unable to offline disk.</summary>
    [Description("""Unable to offline disk.""")]
    [HResultConstant(0xC00D0009)]
    public static HResult NS_E_CANNOTOFFLINEDISK => new(0xC00D0009);

    /// <summary>Unable to online disk.</summary>
    [Description("""Unable to online disk.""")]
    [HResultConstant(0xC00D000A)]
    public static HResult NS_E_CANNOTONLINEDISK => new(0xC00D000A);

    /// <summary>There is no file parser registered for this type of file.</summary>
    [Description("""There is no file parser registered for this type of file.""")]
    [HResultConstant(0xC00D000B)]
    public static HResult NS_E_NOREGISTEREDWALKER => new(0xC00D000B);

    /// <summary>There is no data connection established.</summary>
    [Description("""There is no data connection established.""")]
    [HResultConstant(0xC00D000C)]
    public static HResult NS_E_NOFUNNEL => new(0xC00D000C);

    /// <summary>Failed to load the local play DLL.</summary>
    [Description("""Failed to load the local play DLL.""")]
    [HResultConstant(0xC00D000D)]
    public static HResult NS_E_NO_LOCALPLAY => new(0xC00D000D);

    /// <summary>The network is busy.</summary>
    [Description("""The network is busy.""")]
    [HResultConstant(0xC00D000E)]
    public static HResult NS_E_NETWORK_BUSY => new(0xC00D000E);

    /// <summary>The server session limit was exceeded.</summary>
    [Description("""The server session limit was exceeded.""")]
    [HResultConstant(0xC00D000F)]
    public static HResult NS_E_TOO_MANY_SESS => new(0xC00D000F);

    /// <summary>The network connection already exists.</summary>
    [Description("""The network connection already exists.""")]
    [HResultConstant(0xC00D0010)]
    public static HResult NS_E_ALREADY_CONNECTED => new(0xC00D0010);

    /// <summary>Index %1 is invalid.</summary>
    [Description("""Index %1 is invalid.""")]
    [HResultConstant(0xC00D0011)]
    public static HResult NS_E_INVALID_INDEX => new(0xC00D0011);

    /// <summary>There is no protocol or protocol version supported by both the client and the server.</summary>
    [Description("""There is no protocol or protocol version supported by both the client and the server.""")]
    [HResultConstant(0xC00D0012)]
    public static HResult NS_E_PROTOCOL_MISMATCH => new(0xC00D0012);

    /// <summary>The server, a computer set up to offer multimedia content to other computers, could not handle your request for multimedia content in a timely manner. Please try again later.</summary>
    [Description("""The server, a computer set up to offer multimedia content to other computers, could not handle your request for multimedia content in a timely manner. Please try again later.""")]
    [HResultConstant(0xC00D0013)]
    public static HResult NS_E_TIMEOUT => new(0xC00D0013);

    /// <summary>Error writing to the network.</summary>
    [Description("""Error writing to the network.""")]
    [HResultConstant(0xC00D0014)]
    public static HResult NS_E_NET_WRITE => new(0xC00D0014);

    /// <summary>Error reading from the network.</summary>
    [Description("""Error reading from the network.""")]
    [HResultConstant(0xC00D0015)]
    public static HResult NS_E_NET_READ => new(0xC00D0015);

    /// <summary>Error writing to a disk.</summary>
    [Description("""Error writing to a disk.""")]
    [HResultConstant(0xC00D0016)]
    public static HResult NS_E_DISK_WRITE => new(0xC00D0016);

    /// <summary>Error reading from a disk.</summary>
    [Description("""Error reading from a disk.""")]
    [HResultConstant(0xC00D0017)]
    public static HResult NS_E_DISK_READ => new(0xC00D0017);

    /// <summary>Error writing to a file.</summary>
    [Description("""Error writing to a file.""")]
    [HResultConstant(0xC00D0018)]
    public static HResult NS_E_FILE_WRITE => new(0xC00D0018);

    /// <summary>Error reading from a file.</summary>
    [Description("""Error reading from a file.""")]
    [HResultConstant(0xC00D0019)]
    public static HResult NS_E_FILE_READ => new(0xC00D0019);

    /// <summary>The system cannot find the file specified.</summary>
    [Description("""The system cannot find the file specified.""")]
    [HResultConstant(0xC00D001A)]
    public static HResult NS_E_FILE_NOT_FOUND => new(0xC00D001A);

    /// <summary>The file already exists.</summary>
    [Description("""The file already exists.""")]
    [HResultConstant(0xC00D001B)]
    public static HResult NS_E_FILE_EXISTS => new(0xC00D001B);

    /// <summary>The file name, directory name, or volume label syntax is incorrect.</summary>
    [Description("""The file name, directory name, or volume label syntax is incorrect.""")]
    [HResultConstant(0xC00D001C)]
    public static HResult NS_E_INVALID_NAME => new(0xC00D001C);

    /// <summary>Failed to open a file.</summary>
    [Description("""Failed to open a file.""")]
    [HResultConstant(0xC00D001D)]
    public static HResult NS_E_FILE_OPEN_FAILED => new(0xC00D001D);

    /// <summary>Unable to allocate a file.</summary>
    [Description("""Unable to allocate a file.""")]
    [HResultConstant(0xC00D001E)]
    public static HResult NS_E_FILE_ALLOCATION_FAILED => new(0xC00D001E);

    /// <summary>Unable to initialize a file.</summary>
    [Description("""Unable to initialize a file.""")]
    [HResultConstant(0xC00D001F)]
    public static HResult NS_E_FILE_INIT_FAILED => new(0xC00D001F);

    /// <summary>Unable to play a file.</summary>
    [Description("""Unable to play a file.""")]
    [HResultConstant(0xC00D0020)]
    public static HResult NS_E_FILE_PLAY_FAILED => new(0xC00D0020);

    /// <summary>Could not set the disk UID.</summary>
    [Description("""Could not set the disk UID.""")]
    [HResultConstant(0xC00D0021)]
    public static HResult NS_E_SET_DISK_UID_FAILED => new(0xC00D0021);

    /// <summary>An error was induced for testing purposes.</summary>
    [Description("""An error was induced for testing purposes.""")]
    [HResultConstant(0xC00D0022)]
    public static HResult NS_E_INDUCED => new(0xC00D0022);

    /// <summary>Two Content Servers failed to communicate.</summary>
    [Description("""Two Content Servers failed to communicate.""")]
    [HResultConstant(0xC00D0023)]
    public static HResult NS_E_CCLINK_DOWN => new(0xC00D0023);

    /// <summary>An unknown error occurred.</summary>
    [Description("""An unknown error occurred.""")]
    [HResultConstant(0xC00D0024)]
    public static HResult NS_E_INTERNAL => new(0xC00D0024);

    /// <summary>The requested resource is in use.</summary>
    [Description("""The requested resource is in use.""")]
    [HResultConstant(0xC00D0025)]
    public static HResult NS_E_BUSY => new(0xC00D0025);

    /// <summary>The specified protocol is not recognized. Be sure that the file name and syntax, such as slashes, are correct for the protocol.</summary>
    [Description("""The specified protocol is not recognized. Be sure that the file name and syntax, such as slashes, are correct for the protocol.""")]
    [HResultConstant(0xC00D0026)]
    public static HResult NS_E_UNRECOGNIZED_STREAM_TYPE => new(0xC00D0026);

    /// <summary>The network service provider failed.</summary>
    [Description("""The network service provider failed.""")]
    [HResultConstant(0xC00D0027)]
    public static HResult NS_E_NETWORK_SERVICE_FAILURE => new(0xC00D0027);

    /// <summary>An attempt to acquire a network resource failed.</summary>
    [Description("""An attempt to acquire a network resource failed.""")]
    [HResultConstant(0xC00D0028)]
    public static HResult NS_E_NETWORK_RESOURCE_FAILURE => new(0xC00D0028);

    /// <summary>The network connection has failed.</summary>
    [Description("""The network connection has failed.""")]
    [HResultConstant(0xC00D0029)]
    public static HResult NS_E_CONNECTION_FAILURE => new(0xC00D0029);

    /// <summary>The session is being terminated locally.</summary>
    [Description("""The session is being terminated locally.""")]
    [HResultConstant(0xC00D002A)]
    public static HResult NS_E_SHUTDOWN => new(0xC00D002A);

    /// <summary>The request is invalid in the current state.</summary>
    [Description("""The request is invalid in the current state.""")]
    [HResultConstant(0xC00D002B)]
    public static HResult NS_E_INVALID_REQUEST => new(0xC00D002B);

    /// <summary>There is insufficient bandwidth available to fulfill the request.</summary>
    [Description("""There is insufficient bandwidth available to fulfill the request.""")]
    [HResultConstant(0xC00D002C)]
    public static HResult NS_E_INSUFFICIENT_BANDWIDTH => new(0xC00D002C);

    /// <summary>The disk is not rebuilding.</summary>
    [Description("""The disk is not rebuilding.""")]
    [HResultConstant(0xC00D002D)]
    public static HResult NS_E_NOT_REBUILDING => new(0xC00D002D);

    /// <summary>An operation requested for a particular time could not be carried out on schedule.</summary>
    [Description("""An operation requested for a particular time could not be carried out on schedule.""")]
    [HResultConstant(0xC00D002E)]
    public static HResult NS_E_LATE_OPERATION => new(0xC00D002E);

    /// <summary>Invalid or corrupt data was encountered.</summary>
    [Description("""Invalid or corrupt data was encountered.""")]
    [HResultConstant(0xC00D002F)]
    public static HResult NS_E_INVALID_DATA => new(0xC00D002F);

    /// <summary>The bandwidth required to stream a file is higher than the maximum file bandwidth allowed on the server.</summary>
    [Description("""The bandwidth required to stream a file is higher than the maximum file bandwidth allowed on the server.""")]
    [HResultConstant(0xC00D0030)]
    public static HResult NS_E_FILE_BANDWIDTH_LIMIT => new(0xC00D0030);

    /// <summary>The client cannot have any more files open simultaneously.</summary>
    [Description("""The client cannot have any more files open simultaneously.""")]
    [HResultConstant(0xC00D0031)]
    public static HResult NS_E_OPEN_FILE_LIMIT => new(0xC00D0031);

    /// <summary>The server received invalid data from the client on the control connection.</summary>
    [Description("""The server received invalid data from the client on the control connection.""")]
    [HResultConstant(0xC00D0032)]
    public static HResult NS_E_BAD_CONTROL_DATA => new(0xC00D0032);

    /// <summary>There is no stream available.</summary>
    [Description("""There is no stream available.""")]
    [HResultConstant(0xC00D0033)]
    public static HResult NS_E_NO_STREAM => new(0xC00D0033);

    /// <summary>There is no more data in the stream.</summary>
    [Description("""There is no more data in the stream.""")]
    [HResultConstant(0xC00D0034)]
    public static HResult NS_E_STREAM_END => new(0xC00D0034);

    /// <summary>The specified server could not be found.</summary>
    [Description("""The specified server could not be found.""")]
    [HResultConstant(0xC00D0035)]
    public static HResult NS_E_SERVER_NOT_FOUND => new(0xC00D0035);

    /// <summary>The specified name is already in use.</summary>
    [Description("""The specified name is already in use.""")]
    [HResultConstant(0xC00D0036)]
    public static HResult NS_E_DUPLICATE_NAME => new(0xC00D0036);

    /// <summary>The specified address is already in use.</summary>
    [Description("""The specified address is already in use.""")]
    [HResultConstant(0xC00D0037)]
    public static HResult NS_E_DUPLICATE_ADDRESS => new(0xC00D0037);

    /// <summary>The specified address is not a valid multicast address.</summary>
    [Description("""The specified address is not a valid multicast address.""")]
    [HResultConstant(0xC00D0038)]
    public static HResult NS_E_BAD_MULTICAST_ADDRESS => new(0xC00D0038);

    /// <summary>The specified adapter address is invalid.</summary>
    [Description("""The specified adapter address is invalid.""")]
    [HResultConstant(0xC00D0039)]
    public static HResult NS_E_BAD_ADAPTER_ADDRESS => new(0xC00D0039);

    /// <summary>The specified delivery mode is invalid.</summary>
    [Description("""The specified delivery mode is invalid.""")]
    [HResultConstant(0xC00D003A)]
    public static HResult NS_E_BAD_DELIVERY_MODE => new(0xC00D003A);

    /// <summary>The specified station does not exist.</summary>
    [Description("""The specified station does not exist.""")]
    [HResultConstant(0xC00D003B)]
    public static HResult NS_E_INVALID_CHANNEL => new(0xC00D003B);

    /// <summary>The specified stream does not exist.</summary>
    [Description("""The specified stream does not exist.""")]
    [HResultConstant(0xC00D003C)]
    public static HResult NS_E_INVALID_STREAM => new(0xC00D003C);

    /// <summary>The specified archive could not be opened.</summary>
    [Description("""The specified archive could not be opened.""")]
    [HResultConstant(0xC00D003D)]
    public static HResult NS_E_INVALID_ARCHIVE => new(0xC00D003D);

    /// <summary>The system cannot find any titles on the server.</summary>
    [Description("""The system cannot find any titles on the server.""")]
    [HResultConstant(0xC00D003E)]
    public static HResult NS_E_NOTITLES => new(0xC00D003E);

    /// <summary>The system cannot find the client specified.</summary>
    [Description("""The system cannot find the client specified.""")]
    [HResultConstant(0xC00D003F)]
    public static HResult NS_E_INVALID_CLIENT => new(0xC00D003F);

    /// <summary>The Blackhole Address is not initialized.</summary>
    [Description("""The Blackhole Address is not initialized.""")]
    [HResultConstant(0xC00D0040)]
    public static HResult NS_E_INVALID_BLACKHOLE_ADDRESS => new(0xC00D0040);

    /// <summary>The station does not support the stream format.</summary>
    [Description("""The station does not support the stream format.""")]
    [HResultConstant(0xC00D0041)]
    public static HResult NS_E_INCOMPATIBLE_FORMAT => new(0xC00D0041);

    /// <summary>The specified key is not valid.</summary>
    [Description("""The specified key is not valid.""")]
    [HResultConstant(0xC00D0042)]
    public static HResult NS_E_INVALID_KEY => new(0xC00D0042);

    /// <summary>The specified port is not valid.</summary>
    [Description("""The specified port is not valid.""")]
    [HResultConstant(0xC00D0043)]
    public static HResult NS_E_INVALID_PORT => new(0xC00D0043);

    /// <summary>The specified TTL is not valid.</summary>
    [Description("""The specified TTL is not valid.""")]
    [HResultConstant(0xC00D0044)]
    public static HResult NS_E_INVALID_TTL => new(0xC00D0044);

    /// <summary>The request to fast forward or rewind could not be fulfilled.</summary>
    [Description("""The request to fast forward or rewind could not be fulfilled.""")]
    [HResultConstant(0xC00D0045)]
    public static HResult NS_E_STRIDE_REFUSED => new(0xC00D0045);

    /// <summary>Unable to load the appropriate file parser.</summary>
    [Description("""Unable to load the appropriate file parser.""")]
    [HResultConstant(0xC00D0046)]
    public static HResult NS_E_MMSAUTOSERVER_CANTFINDWALKER => new(0xC00D0046);

    /// <summary>Cannot exceed the maximum bandwidth limit.</summary>
    [Description("""Cannot exceed the maximum bandwidth limit.""")]
    [HResultConstant(0xC00D0047)]
    public static HResult NS_E_MAX_BITRATE => new(0xC00D0047);

    /// <summary>Invalid value for LogFilePeriod.</summary>
    [Description("""Invalid value for LogFilePeriod.""")]
    [HResultConstant(0xC00D0048)]
    public static HResult NS_E_LOGFILEPERIOD => new(0xC00D0048);

    /// <summary>Cannot exceed the maximum client limit.</summary>
    [Description("""Cannot exceed the maximum client limit.""")]
    [HResultConstant(0xC00D0049)]
    public static HResult NS_E_MAX_CLIENTS => new(0xC00D0049);

    /// <summary>The maximum log file size has been reached.</summary>
    [Description("""The maximum log file size has been reached.""")]
    [HResultConstant(0xC00D004A)]
    public static HResult NS_E_LOG_FILE_SIZE => new(0xC00D004A);

    /// <summary>Cannot exceed the maximum file rate.</summary>
    [Description("""Cannot exceed the maximum file rate.""")]
    [HResultConstant(0xC00D004B)]
    public static HResult NS_E_MAX_FILERATE => new(0xC00D004B);

    /// <summary>Unknown file type.</summary>
    [Description("""Unknown file type.""")]
    [HResultConstant(0xC00D004C)]
    public static HResult NS_E_WALKER_UNKNOWN => new(0xC00D004C);

    /// <summary>The specified file, %1, cannot be loaded onto the specified server, %2.</summary>
    [Description("""The specified file, %1, cannot be loaded onto the specified server, %2.""")]
    [HResultConstant(0xC00D004D)]
    public static HResult NS_E_WALKER_SERVER => new(0xC00D004D);

    /// <summary>There was a usage error with file parser.</summary>
    [Description("""There was a usage error with file parser.""")]
    [HResultConstant(0xC00D004E)]
    public static HResult NS_E_WALKER_USAGE => new(0xC00D004E);

    /// <summary>The Title Server %1 has failed.</summary>
    [Description("""The Title Server %1 has failed.""")]
    [HResultConstant(0xC00D0050)]
    public static HResult NS_E_TIGER_FAIL => new(0xC00D0050);

    /// <summary>Content Server %1 (%2) has failed.</summary>
    [Description("""Content Server %1 (%2) has failed.""")]
    [HResultConstant(0xC00D0053)]
    public static HResult NS_E_CUB_FAIL => new(0xC00D0053);

    /// <summary>Disk %1 ( %2 ) on Content Server %3, has failed.</summary>
    [Description("""Disk %1 ( %2 ) on Content Server %3, has failed.""")]
    [HResultConstant(0xC00D0055)]
    public static HResult NS_E_DISK_FAIL => new(0xC00D0055);

    /// <summary>The NetShow data stream limit of %1 streams was reached.</summary>
    [Description("""The NetShow data stream limit of %1 streams was reached.""")]
    [HResultConstant(0xC00D0060)]
    public static HResult NS_E_MAX_FUNNELS_ALERT => new(0xC00D0060);

    /// <summary>The NetShow Video Server was unable to allocate a %1 block file named %2.</summary>
    [Description("""The NetShow Video Server was unable to allocate a %1 block file named %2.""")]
    [HResultConstant(0xC00D0061)]
    public static HResult NS_E_ALLOCATE_FILE_FAIL => new(0xC00D0061);

    /// <summary>A Content Server was unable to page a block.</summary>
    [Description("""A Content Server was unable to page a block.""")]
    [HResultConstant(0xC00D0062)]
    public static HResult NS_E_PAGING_ERROR => new(0xC00D0062);

    /// <summary>Disk %1 has unrecognized control block version %2.</summary>
    [Description("""Disk %1 has unrecognized control block version %2.""")]
    [HResultConstant(0xC00D0063)]
    public static HResult NS_E_BAD_BLOCK0_VERSION => new(0xC00D0063);

    /// <summary>Disk %1 has incorrect uid %2.</summary>
    [Description("""Disk %1 has incorrect uid %2.""")]
    [HResultConstant(0xC00D0064)]
    public static HResult NS_E_BAD_DISK_UID => new(0xC00D0064);

    /// <summary>Disk %1 has unsupported file system major version %2.</summary>
    [Description("""Disk %1 has unsupported file system major version %2.""")]
    [HResultConstant(0xC00D0065)]
    public static HResult NS_E_BAD_FSMAJOR_VERSION => new(0xC00D0065);

    /// <summary>Disk %1 has bad stamp number in control block.</summary>
    [Description("""Disk %1 has bad stamp number in control block.""")]
    [HResultConstant(0xC00D0066)]
    public static HResult NS_E_BAD_STAMPNUMBER => new(0xC00D0066);

    /// <summary>Disk %1 is partially reconstructed.</summary>
    [Description("""Disk %1 is partially reconstructed.""")]
    [HResultConstant(0xC00D0067)]
    public static HResult NS_E_PARTIALLY_REBUILT_DISK => new(0xC00D0067);

    /// <summary>EnactPlan gives up.</summary>
    [Description("""EnactPlan gives up.""")]
    [HResultConstant(0xC00D0068)]
    public static HResult NS_E_ENACTPLAN_GIVEUP => new(0xC00D0068);

    /// <summary>The key was not found in the registry.</summary>
    [Description("""The key was not found in the registry.""")]
    [HResultConstant(0xC00D006A)]
    public static HResult MCMADM_E_REGKEY_NOT_FOUND => new(0xC00D006A);

    /// <summary>The publishing point cannot be started because the server does not have the appropriate stream formats. Use the Multicast Announcement Wizard to create a new announcement for this publishing point.</summary>
    [Description("""The publishing point cannot be started because the server does not have the appropriate stream formats. Use the Multicast Announcement Wizard to create a new announcement for this publishing point.""")]
    [HResultConstant(0xC00D006B)]
    public static HResult NS_E_NO_FORMATS => new(0xC00D006B);

    /// <summary>No reference URLs were found in an ASX file.</summary>
    [Description("""No reference URLs were found in an ASX file.""")]
    [HResultConstant(0xC00D006C)]
    public static HResult NS_E_NO_REFERENCES => new(0xC00D006C);

    /// <summary>Error opening wave device, the device might be in use.</summary>
    [Description("""Error opening wave device, the device might be in use.""")]
    [HResultConstant(0xC00D006D)]
    public static HResult NS_E_WAVE_OPEN => new(0xC00D006D);

    /// <summary>Unable to establish a connection to the NetShow event monitor service.</summary>
    [Description("""Unable to establish a connection to the NetShow event monitor service.""")]
    [HResultConstant(0xC00D006F)]
    public static HResult NS_E_CANNOTCONNECTEVENTS => new(0xC00D006F);

    /// <summary>No device driver is present on the system.</summary>
    [Description("""No device driver is present on the system.""")]
    [HResultConstant(0xC00D0071)]
    public static HResult NS_E_NO_DEVICE => new(0xC00D0071);

    /// <summary>No specified device driver is present.</summary>
    [Description("""No specified device driver is present.""")]
    [HResultConstant(0xC00D0072)]
    public static HResult NS_E_NO_SPECIFIED_DEVICE => new(0xC00D0072);

    /// <summary>Netshow Events Monitor is not operational and has been disconnected.</summary>
    [Description("""Netshow Events Monitor is not operational and has been disconnected.""")]
    [HResultConstant(0xC00D00C8)]
    public static HResult NS_E_MONITOR_GIVEUP => new(0xC00D00C8);

    /// <summary>Disk %1 is remirrored.</summary>
    [Description("""Disk %1 is remirrored.""")]
    [HResultConstant(0xC00D00C9)]
    public static HResult NS_E_REMIRRORED_DISK => new(0xC00D00C9);

    /// <summary>Insufficient data found.</summary>
    [Description("""Insufficient data found.""")]
    [HResultConstant(0xC00D00CA)]
    public static HResult NS_E_INSUFFICIENT_DATA => new(0xC00D00CA);

    /// <summary>1 failed in file %2 line %3.</summary>
    [Description("""1 failed in file %2 line %3.""")]
    [HResultConstant(0xC00D00CB)]
    public static HResult NS_E_ASSERT => new(0xC00D00CB);

    /// <summary>The specified adapter name is invalid.</summary>
    [Description("""The specified adapter name is invalid.""")]
    [HResultConstant(0xC00D00CC)]
    public static HResult NS_E_BAD_ADAPTER_NAME => new(0xC00D00CC);

    /// <summary>The application is not licensed for this feature.</summary>
    [Description("""The application is not licensed for this feature.""")]
    [HResultConstant(0xC00D00CD)]
    public static HResult NS_E_NOT_LICENSED => new(0xC00D00CD);

    /// <summary>Unable to contact the server.</summary>
    [Description("""Unable to contact the server.""")]
    [HResultConstant(0xC00D00CE)]
    public static HResult NS_E_NO_SERVER_CONTACT => new(0xC00D00CE);

    /// <summary>Maximum number of titles exceeded.</summary>
    [Description("""Maximum number of titles exceeded.""")]
    [HResultConstant(0xC00D00CF)]
    public static HResult NS_E_TOO_MANY_TITLES => new(0xC00D00CF);

    /// <summary>Maximum size of a title exceeded.</summary>
    [Description("""Maximum size of a title exceeded.""")]
    [HResultConstant(0xC00D00D0)]
    public static HResult NS_E_TITLE_SIZE_EXCEEDED => new(0xC00D00D0);

    /// <summary>UDP protocol not enabled. Not trying %1!ls!.</summary>
    [Description("""UDP protocol not enabled. Not trying %1!ls!.""")]
    [HResultConstant(0xC00D00D1)]
    public static HResult NS_E_UDP_DISABLED => new(0xC00D00D1);

    /// <summary>TCP protocol not enabled. Not trying %1!ls!.</summary>
    [Description("""TCP protocol not enabled. Not trying %1!ls!.""")]
    [HResultConstant(0xC00D00D2)]
    public static HResult NS_E_TCP_DISABLED => new(0xC00D00D2);

    /// <summary>HTTP protocol not enabled. Not trying %1!ls!.</summary>
    [Description("""HTTP protocol not enabled. Not trying %1!ls!.""")]
    [HResultConstant(0xC00D00D3)]
    public static HResult NS_E_HTTP_DISABLED => new(0xC00D00D3);

    /// <summary>The product license has expired.</summary>
    [Description("""The product license has expired.""")]
    [HResultConstant(0xC00D00D4)]
    public static HResult NS_E_LICENSE_EXPIRED => new(0xC00D00D4);

    /// <summary>Source file exceeds the per title maximum bitrate. See NetShow Theater documentation for more information.</summary>
    [Description("""Source file exceeds the per title maximum bitrate. See NetShow Theater documentation for more information.""")]
    [HResultConstant(0xC00D00D5)]
    public static HResult NS_E_TITLE_BITRATE => new(0xC00D00D5);

    /// <summary>The program name cannot be empty.</summary>
    [Description("""The program name cannot be empty.""")]
    [HResultConstant(0xC00D00D6)]
    public static HResult NS_E_EMPTY_PROGRAM_NAME => new(0xC00D00D6);

    /// <summary>Station %1 does not exist.</summary>
    [Description("""Station %1 does not exist.""")]
    [HResultConstant(0xC00D00D7)]
    public static HResult NS_E_MISSING_CHANNEL => new(0xC00D00D7);

    /// <summary>You need to define at least one station before this operation can complete.</summary>
    [Description("""You need to define at least one station before this operation can complete.""")]
    [HResultConstant(0xC00D00D8)]
    public static HResult NS_E_NO_CHANNELS => new(0xC00D00D8);

    /// <summary>The index specified is invalid.</summary>
    [Description("""The index specified is invalid.""")]
    [HResultConstant(0xC00D00D9)]
    public static HResult NS_E_INVALID_INDEX2 => new(0xC00D00D9);

    /// <summary>Content Server %1 (%2) has failed its link to Content Server %3.</summary>
    [Description("""Content Server %1 (%2) has failed its link to Content Server %3.""")]
    [HResultConstant(0xC00D0190)]
    public static HResult NS_E_CUB_FAIL_LINK => new(0xC00D0190);

    /// <summary>Content Server %1 (%2) has incorrect uid %3.</summary>
    [Description("""Content Server %1 (%2) has incorrect uid %3.""")]
    [HResultConstant(0xC00D0192)]
    public static HResult NS_E_BAD_CUB_UID => new(0xC00D0192);

    /// <summary>Server unreliable because multiple components failed.</summary>
    [Description("""Server unreliable because multiple components failed.""")]
    [HResultConstant(0xC00D0195)]
    public static HResult NS_E_GLITCH_MODE => new(0xC00D0195);

    /// <summary>Content Server %1 (%2) is unable to communicate with the Media System Network Protocol.</summary>
    [Description("""Content Server %1 (%2) is unable to communicate with the Media System Network Protocol.""")]
    [HResultConstant(0xC00D019B)]
    public static HResult NS_E_NO_MEDIA_PROTOCOL => new(0xC00D019B);

    /// <summary>Nothing to do.</summary>
    [Description("""Nothing to do.""")]
    [HResultConstant(0xC00D07F1)]
    public static HResult NS_E_NOTHING_TO_DO => new(0xC00D07F1);

    /// <summary>Not receiving data from the server.</summary>
    [Description("""Not receiving data from the server.""")]
    [HResultConstant(0xC00D07F2)]
    public static HResult NS_E_NO_MULTICAST => new(0xC00D07F2);

    /// <summary>The input media format is invalid.</summary>
    [Description("""The input media format is invalid.""")]
    [HResultConstant(0xC00D0BB8)]
    public static HResult NS_E_INVALID_INPUT_FORMAT => new(0xC00D0BB8);

    /// <summary>The MSAudio codec is not installed on this system.</summary>
    [Description("""The MSAudio codec is not installed on this system.""")]
    [HResultConstant(0xC00D0BB9)]
    public static HResult NS_E_MSAUDIO_NOT_INSTALLED => new(0xC00D0BB9);

    /// <summary>An unexpected error occurred with the MSAudio codec.</summary>
    [Description("""An unexpected error occurred with the MSAudio codec.""")]
    [HResultConstant(0xC00D0BBA)]
    public static HResult NS_E_UNEXPECTED_MSAUDIO_ERROR => new(0xC00D0BBA);

    /// <summary>The output media format is invalid.</summary>
    [Description("""The output media format is invalid.""")]
    [HResultConstant(0xC00D0BBB)]
    public static HResult NS_E_INVALID_OUTPUT_FORMAT => new(0xC00D0BBB);

    /// <summary>The object must be fully configured before audio samples can be processed.</summary>
    [Description("""The object must be fully configured before audio samples can be processed.""")]
    [HResultConstant(0xC00D0BBC)]
    public static HResult NS_E_NOT_CONFIGURED => new(0xC00D0BBC);

    /// <summary>You need a license to perform the requested operation on this media file.</summary>
    [Description("""You need a license to perform the requested operation on this media file.""")]
    [HResultConstant(0xC00D0BBD)]
    public static HResult NS_E_PROTECTED_CONTENT => new(0xC00D0BBD);

    /// <summary>You need a license to perform the requested operation on this media file.</summary>
    [Description("""You need a license to perform the requested operation on this media file.""")]
    [HResultConstant(0xC00D0BBE)]
    public static HResult NS_E_LICENSE_REQUIRED => new(0xC00D0BBE);

    /// <summary>This media file is corrupted or invalid. Contact the content provider for a new file.</summary>
    [Description("""This media file is corrupted or invalid. Contact the content provider for a new file.""")]
    [HResultConstant(0xC00D0BBF)]
    public static HResult NS_E_TAMPERED_CONTENT => new(0xC00D0BBF);

    /// <summary>The license for this media file has expired. Get a new license or contact the content provider for further assistance.</summary>
    [Description("""The license for this media file has expired. Get a new license or contact the content provider for further assistance.""")]
    [HResultConstant(0xC00D0BC0)]
    public static HResult NS_E_LICENSE_OUTOFDATE => new(0xC00D0BC0);

    /// <summary>You are not allowed to open this file. Contact the content provider for further assistance.</summary>
    [Description("""You are not allowed to open this file. Contact the content provider for further assistance.""")]
    [HResultConstant(0xC00D0BC1)]
    public static HResult NS_E_LICENSE_INCORRECT_RIGHTS => new(0xC00D0BC1);

    /// <summary>The requested audio codec is not installed on this system.</summary>
    [Description("""The requested audio codec is not installed on this system.""")]
    [HResultConstant(0xC00D0BC2)]
    public static HResult NS_E_AUDIO_CODEC_NOT_INSTALLED => new(0xC00D0BC2);

    /// <summary>An unexpected error occurred with the audio codec.</summary>
    [Description("""An unexpected error occurred with the audio codec.""")]
    [HResultConstant(0xC00D0BC3)]
    public static HResult NS_E_AUDIO_CODEC_ERROR => new(0xC00D0BC3);

    /// <summary>The requested video codec is not installed on this system.</summary>
    [Description("""The requested video codec is not installed on this system.""")]
    [HResultConstant(0xC00D0BC4)]
    public static HResult NS_E_VIDEO_CODEC_NOT_INSTALLED => new(0xC00D0BC4);

    /// <summary>An unexpected error occurred with the video codec.</summary>
    [Description("""An unexpected error occurred with the video codec.""")]
    [HResultConstant(0xC00D0BC5)]
    public static HResult NS_E_VIDEO_CODEC_ERROR => new(0xC00D0BC5);

    /// <summary>The Profile is invalid.</summary>
    [Description("""The Profile is invalid.""")]
    [HResultConstant(0xC00D0BC6)]
    public static HResult NS_E_INVALIDPROFILE => new(0xC00D0BC6);

    /// <summary>A new version of the SDK is needed to play the requested content.</summary>
    [Description("""A new version of the SDK is needed to play the requested content.""")]
    [HResultConstant(0xC00D0BC7)]
    public static HResult NS_E_INCOMPATIBLE_VERSION => new(0xC00D0BC7);

    /// <summary>The requested URL is not available in offline mode.</summary>
    [Description("""The requested URL is not available in offline mode.""")]
    [HResultConstant(0xC00D0BCA)]
    public static HResult NS_E_OFFLINE_MODE => new(0xC00D0BCA);

    /// <summary>The requested URL cannot be accessed because there is no network connection.</summary>
    [Description("""The requested URL cannot be accessed because there is no network connection.""")]
    [HResultConstant(0xC00D0BCB)]
    public static HResult NS_E_NOT_CONNECTED => new(0xC00D0BCB);

    /// <summary>The encoding process was unable to keep up with the amount of supplied data.</summary>
    [Description("""The encoding process was unable to keep up with the amount of supplied data.""")]
    [HResultConstant(0xC00D0BCC)]
    public static HResult NS_E_TOO_MUCH_DATA => new(0xC00D0BCC);

    /// <summary>The given property is not supported.</summary>
    [Description("""The given property is not supported.""")]
    [HResultConstant(0xC00D0BCD)]
    public static HResult NS_E_UNSUPPORTED_PROPERTY => new(0xC00D0BCD);

    /// <summary>Windows Media Player cannot copy the files to the CD because they are 8-bit. Convert the files to 16-bit, 44-kHz stereo files by using Sound Recorder or another audio-processing program, and then try again.</summary>
    [Description("""Windows Media Player cannot copy the files to the CD because they are 8-bit. Convert the files to 16-bit, 44-kHz stereo files by using Sound Recorder or another audio-processing program, and then try again.""")]
    [HResultConstant(0xC00D0BCE)]
    public static HResult NS_E_8BIT_WAVE_UNSUPPORTED => new(0xC00D0BCE);

    /// <summary>There are no more samples in the current range.</summary>
    [Description("""There are no more samples in the current range.""")]
    [HResultConstant(0xC00D0BCF)]
    public static HResult NS_E_NO_MORE_SAMPLES => new(0xC00D0BCF);

    /// <summary>The given sampling rate is invalid.</summary>
    [Description("""The given sampling rate is invalid.""")]
    [HResultConstant(0xC00D0BD0)]
    public static HResult NS_E_INVALID_SAMPLING_RATE => new(0xC00D0BD0);

    /// <summary>The given maximum packet size is too small to accommodate this profile.)</summary>
    [Description("""The given maximum packet size is too small to accommodate this profile.)""")]
    [HResultConstant(0xC00D0BD1)]
    public static HResult NS_E_MAX_PACKET_SIZE_TOO_SMALL => new(0xC00D0BD1);

    /// <summary>The packet arrived too late to be of use.</summary>
    [Description("""The packet arrived too late to be of use.""")]
    [HResultConstant(0xC00D0BD2)]
    public static HResult NS_E_LATE_PACKET => new(0xC00D0BD2);

    /// <summary>The packet is a duplicate of one received before.</summary>
    [Description("""The packet is a duplicate of one received before.""")]
    [HResultConstant(0xC00D0BD3)]
    public static HResult NS_E_DUPLICATE_PACKET => new(0xC00D0BD3);

    /// <summary>Supplied buffer is too small.</summary>
    [Description("""Supplied buffer is too small.""")]
    [HResultConstant(0xC00D0BD4)]
    public static HResult NS_E_SDK_BUFFERTOOSMALL => new(0xC00D0BD4);

    /// <summary>The wrong number of preprocessing passes was used for the stream's output type.</summary>
    [Description("""The wrong number of preprocessing passes was used for the stream's output type.""")]
    [HResultConstant(0xC00D0BD5)]
    public static HResult NS_E_INVALID_NUM_PASSES => new(0xC00D0BD5);

    /// <summary>An attempt was made to add, modify, or delete a read only attribute.</summary>
    [Description("""An attempt was made to add, modify, or delete a read only attribute.""")]
    [HResultConstant(0xC00D0BD6)]
    public static HResult NS_E_ATTRIBUTE_READ_ONLY => new(0xC00D0BD6);

    /// <summary>An attempt was made to add attribute that is not allowed for the given media type.</summary>
    [Description("""An attempt was made to add attribute that is not allowed for the given media type.""")]
    [HResultConstant(0xC00D0BD7)]
    public static HResult NS_E_ATTRIBUTE_NOT_ALLOWED => new(0xC00D0BD7);

    /// <summary>The EDL provided is invalid.</summary>
    [Description("""The EDL provided is invalid.""")]
    [HResultConstant(0xC00D0BD8)]
    public static HResult NS_E_INVALID_EDL => new(0xC00D0BD8);

    /// <summary>The Data Unit Extension data was too large to be used.</summary>
    [Description("""The Data Unit Extension data was too large to be used.""")]
    [HResultConstant(0xC00D0BD9)]
    public static HResult NS_E_DATA_UNIT_EXTENSION_TOO_LARGE => new(0xC00D0BD9);

    /// <summary>An unexpected error occurred with a DMO codec.</summary>
    [Description("""An unexpected error occurred with a DMO codec.""")]
    [HResultConstant(0xC00D0BDA)]
    public static HResult NS_E_CODEC_DMO_ERROR => new(0xC00D0BDA);

    /// <summary>This feature has been disabled by group policy.</summary>
    [Description("""This feature has been disabled by group policy.""")]
    [HResultConstant(0xC00D0BDC)]
    public static HResult NS_E_FEATURE_DISABLED_BY_GROUP_POLICY => new(0xC00D0BDC);

    /// <summary>This feature is disabled in this SKU.</summary>
    [Description("""This feature is disabled in this SKU.""")]
    [HResultConstant(0xC00D0BDD)]
    public static HResult NS_E_FEATURE_DISABLED_IN_SKU => new(0xC00D0BDD);

    /// <summary>There is no CD in the CD drive. Insert a CD, and then try again.</summary>
    [Description("""There is no CD in the CD drive. Insert a CD, and then try again.""")]
    [HResultConstant(0xC00D0FA0)]
    public static HResult NS_E_NO_CD => new(0xC00D0FA0);

    /// <summary>Windows Media Player could not use digital playback to play the CD. To switch to analog playback, on the Tools menu, click Options, and then click the Devices tab. Double-click the CD drive, and then in the Playback area, click Analog. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player could not use digital playback to play the CD. To switch to analog playback, on the Tools menu, click Options, and then click the Devices tab. Double-click the CD drive, and then in the Playback area, click Analog. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D0FA1)]
    public static HResult NS_E_CANT_READ_DIGITAL => new(0xC00D0FA1);

    /// <summary>Windows Media Player no longer detects a connected portable device. Reconnect your portable device, and then try synchronizing the file again.</summary>
    [Description("""Windows Media Player no longer detects a connected portable device. Reconnect your portable device, and then try synchronizing the file again.""")]
    [HResultConstant(0xC00D0FA2)]
    public static HResult NS_E_DEVICE_DISCONNECTED => new(0xC00D0FA2);

    /// <summary>Windows Media Player cannot play the file. The portable device does not support the specified file type.</summary>
    [Description("""Windows Media Player cannot play the file. The portable device does not support the specified file type.""")]
    [HResultConstant(0xC00D0FA3)]
    public static HResult NS_E_DEVICE_NOT_SUPPORT_FORMAT => new(0xC00D0FA3);

    /// <summary>Windows Media Player could not use digital playback to play the CD. The Player has automatically switched the CD drive to analog playback. To switch back to digital CD playback, use the Devices tab. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player could not use digital playback to play the CD. The Player has automatically switched the CD drive to analog playback. To switch back to digital CD playback, use the Devices tab. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D0FA4)]
    public static HResult NS_E_SLOW_READ_DIGITAL => new(0xC00D0FA4);

    /// <summary>An invalid line error occurred in the mixer.</summary>
    [Description("""An invalid line error occurred in the mixer.""")]
    [HResultConstant(0xC00D0FA5)]
    public static HResult NS_E_MIXER_INVALID_LINE => new(0xC00D0FA5);

    /// <summary>An invalid control error occurred in the mixer.</summary>
    [Description("""An invalid control error occurred in the mixer.""")]
    [HResultConstant(0xC00D0FA6)]
    public static HResult NS_E_MIXER_INVALID_CONTROL => new(0xC00D0FA6);

    /// <summary>An invalid value error occurred in the mixer.</summary>
    [Description("""An invalid value error occurred in the mixer.""")]
    [HResultConstant(0xC00D0FA7)]
    public static HResult NS_E_MIXER_INVALID_VALUE => new(0xC00D0FA7);

    /// <summary>An unrecognized MMRESULT occurred in the mixer.</summary>
    [Description("""An unrecognized MMRESULT occurred in the mixer.""")]
    [HResultConstant(0xC00D0FA8)]
    public static HResult NS_E_MIXER_UNKNOWN_MMRESULT => new(0xC00D0FA8);

    /// <summary>User has stopped the operation.</summary>
    [Description("""User has stopped the operation.""")]
    [HResultConstant(0xC00D0FA9)]
    public static HResult NS_E_USER_STOP => new(0xC00D0FA9);

    /// <summary>Windows Media Player cannot rip the track because a compatible MP3 encoder is not installed on your computer. Install a compatible MP3 encoder or choose a different format to rip to (such as Windows Media Audio).</summary>
    [Description("""Windows Media Player cannot rip the track because a compatible MP3 encoder is not installed on your computer. Install a compatible MP3 encoder or choose a different format to rip to (such as Windows Media Audio).""")]
    [HResultConstant(0xC00D0FAA)]
    public static HResult NS_E_MP3_FORMAT_NOT_FOUND => new(0xC00D0FAA);

    /// <summary>Windows Media Player cannot read the CD. The disc might be dirty or damaged. Turn on error correction, and then try again.</summary>
    [Description("""Windows Media Player cannot read the CD. The disc might be dirty or damaged. Turn on error correction, and then try again.""")]
    [HResultConstant(0xC00D0FAB)]
    public static HResult NS_E_CD_READ_ERROR_NO_CORRECTION => new(0xC00D0FAB);

    /// <summary>Windows Media Player cannot read the CD. The disc might be dirty or damaged or the CD drive might be malfunctioning.</summary>
    [Description("""Windows Media Player cannot read the CD. The disc might be dirty or damaged or the CD drive might be malfunctioning.""")]
    [HResultConstant(0xC00D0FAC)]
    public static HResult NS_E_CD_READ_ERROR => new(0xC00D0FAC);

    /// <summary>For best performance, do not play CD tracks while ripping them.</summary>
    [Description("""For best performance, do not play CD tracks while ripping them.""")]
    [HResultConstant(0xC00D0FAD)]
    public static HResult NS_E_CD_SLOW_COPY => new(0xC00D0FAD);

    /// <summary>It is not possible to directly burn tracks from one CD to another CD. You must first rip the tracks from the CD to your computer, and then burn the files to a blank CD.</summary>
    [Description("""It is not possible to directly burn tracks from one CD to another CD. You must first rip the tracks from the CD to your computer, and then burn the files to a blank CD.""")]
    [HResultConstant(0xC00D0FAE)]
    public static HResult NS_E_CD_COPYTO_CD => new(0xC00D0FAE);

    /// <summary>Could not open a sound mixer driver.</summary>
    [Description("""Could not open a sound mixer driver.""")]
    [HResultConstant(0xC00D0FAF)]
    public static HResult NS_E_MIXER_NODRIVER => new(0xC00D0FAF);

    /// <summary>Windows Media Player cannot rip tracks from the CD correctly because the CD drive settings in Device Manager do not match the CD drive settings in the Player.</summary>
    [Description("""Windows Media Player cannot rip tracks from the CD correctly because the CD drive settings in Device Manager do not match the CD drive settings in the Player.""")]
    [HResultConstant(0xC00D0FB0)]
    public static HResult NS_E_REDBOOK_ENABLED_WHILE_COPYING => new(0xC00D0FB0);

    /// <summary>Windows Media Player is busy reading the CD.</summary>
    [Description("""Windows Media Player is busy reading the CD.""")]
    [HResultConstant(0xC00D0FB1)]
    public static HResult NS_E_CD_REFRESH => new(0xC00D0FB1);

    /// <summary>Windows Media Player could not use digital playback to play the CD. The Player has automatically switched the CD drive to analog playback. To switch back to digital CD playback, use the Devices tab. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player could not use digital playback to play the CD. The Player has automatically switched the CD drive to analog playback. To switch back to digital CD playback, use the Devices tab. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D0FB2)]
    public static HResult NS_E_CD_DRIVER_PROBLEM => new(0xC00D0FB2);

    /// <summary>Windows Media Player could not use digital playback to play the CD. The Player has automatically switched the CD drive to analog playback. To switch back to digital CD playback, use the Devices tab. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player could not use digital playback to play the CD. The Player has automatically switched the CD drive to analog playback. To switch back to digital CD playback, use the Devices tab. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D0FB3)]
    public static HResult NS_E_WONT_DO_DIGITAL => new(0xC00D0FB3);

    /// <summary>A call was made to GetParseError on the XML parser but there was no error to retrieve.</summary>
    [Description("""A call was made to GetParseError on the XML parser but there was no error to retrieve.""")]
    [HResultConstant(0xC00D0FB4)]
    public static HResult NS_E_WMPXML_NOERROR => new(0xC00D0FB4);

    /// <summary>The XML Parser ran out of data while parsing.</summary>
    [Description("""The XML Parser ran out of data while parsing.""")]
    [HResultConstant(0xC00D0FB5)]
    public static HResult NS_E_WMPXML_ENDOFDATA => new(0xC00D0FB5);

    /// <summary>A generic parse error occurred in the XML parser but no information is available.</summary>
    [Description("""A generic parse error occurred in the XML parser but no information is available.""")]
    [HResultConstant(0xC00D0FB6)]
    public static HResult NS_E_WMPXML_PARSEERROR => new(0xC00D0FB6);

    /// <summary>A call get GetNamedAttribute or GetNamedAttributeIndex on the XML parser resulted in the index not being found.</summary>
    [Description("""A call get GetNamedAttribute or GetNamedAttributeIndex on the XML parser resulted in the index not being found.""")]
    [HResultConstant(0xC00D0FB7)]
    public static HResult NS_E_WMPXML_ATTRIBUTENOTFOUND => new(0xC00D0FB7);

    /// <summary>A call was made go GetNamedPI on the XML parser, but the requested Processing Instruction was not found.</summary>
    [Description("""A call was made go GetNamedPI on the XML parser, but the requested Processing Instruction was not found.""")]
    [HResultConstant(0xC00D0FB8)]
    public static HResult NS_E_WMPXML_PINOTFOUND => new(0xC00D0FB8);

    /// <summary>Persist was called on the XML parser, but the parser has no data to persist.</summary>
    [Description("""Persist was called on the XML parser, but the parser has no data to persist.""")]
    [HResultConstant(0xC00D0FB9)]
    public static HResult NS_E_WMPXML_EMPTYDOC => new(0xC00D0FB9);

    /// <summary>This file path is already in the library.</summary>
    [Description("""This file path is already in the library.""")]
    [HResultConstant(0xC00D0FBA)]
    public static HResult NS_E_WMP_PATH_ALREADY_IN_LIBRARY => new(0xC00D0FBA);

    /// <summary>Windows Media Player is already searching for files to add to your library. Wait for the current process to finish before attempting to search again.</summary>
    [Description("""Windows Media Player is already searching for files to add to your library. Wait for the current process to finish before attempting to search again.""")]
    [HResultConstant(0xC00D0FBE)]
    public static HResult NS_E_WMP_FILESCANALREADYSTARTED => new(0xC00D0FBE);

    /// <summary>Windows Media Player is unable to find the media you are looking for.</summary>
    [Description("""Windows Media Player is unable to find the media you are looking for.""")]
    [HResultConstant(0xC00D0FBF)]
    public static HResult NS_E_WMP_HME_INVALIDOBJECTID => new(0xC00D0FBF);

    /// <summary>A component of Windows Media Player is out-of-date. If you are running a pre-release version of Windows, try upgrading to a more recent version.</summary>
    [Description("""A component of Windows Media Player is out-of-date. If you are running a pre-release version of Windows, try upgrading to a more recent version.""")]
    [HResultConstant(0xC00D0FC0)]
    public static HResult NS_E_WMP_MF_CODE_EXPIRED => new(0xC00D0FC0);

    /// <summary>This container does not support search on items.</summary>
    [Description("""This container does not support search on items.""")]
    [HResultConstant(0xC00D0FC1)]
    public static HResult NS_E_WMP_HME_NOTSEARCHABLEFORITEMS => new(0xC00D0FC1);

    /// <summary>Windows Media Player encountered a problem while adding one or more files to the library. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player encountered a problem while adding one or more files to the library. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D0FC7)]
    public static HResult NS_E_WMP_ADDTOLIBRARY_FAILED => new(0xC00D0FC7);

    /// <summary>A Windows API call failed but no error information was available.</summary>
    [Description("""A Windows API call failed but no error information was available.""")]
    [HResultConstant(0xC00D0FC8)]
    public static HResult NS_E_WMP_WINDOWSAPIFAILURE => new(0xC00D0FC8);

    /// <summary>This file does not have burn rights. If you obtained this file from an online store, go to the online store to get burn rights.</summary>
    [Description("""This file does not have burn rights. If you obtained this file from an online store, go to the online store to get burn rights.""")]
    [HResultConstant(0xC00D0FC9)]
    public static HResult NS_E_WMP_RECORDING_NOT_ALLOWED => new(0xC00D0FC9);

    /// <summary>Windows Media Player no longer detects a connected portable device. Reconnect your portable device, and then try to sync the file again.</summary>
    [Description("""Windows Media Player no longer detects a connected portable device. Reconnect your portable device, and then try to sync the file again.""")]
    [HResultConstant(0xC00D0FCA)]
    public static HResult NS_E_DEVICE_NOT_READY => new(0xC00D0FCA);

    /// <summary>Windows Media Player cannot play the file because it is corrupted.</summary>
    [Description("""Windows Media Player cannot play the file because it is corrupted.""")]
    [HResultConstant(0xC00D0FCB)]
    public static HResult NS_E_DAMAGED_FILE => new(0xC00D0FCB);

    /// <summary>Windows Media Player encountered an error while attempting to access information in the library. Try restarting the Player.</summary>
    [Description("""Windows Media Player encountered an error while attempting to access information in the library. Try restarting the Player.""")]
    [HResultConstant(0xC00D0FCC)]
    public static HResult NS_E_MPDB_GENERIC => new(0xC00D0FCC);

    /// <summary>The file cannot be added to the library because it is smaller than the "Skip files smaller than" setting. To add the file, change the setting on the Library tab. For additional assistance, click Web Help.</summary>
    [Description("""The file cannot be added to the library because it is smaller than the "Skip files smaller than" setting. To add the file, change the setting on the Library tab. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D0FCD)]
    public static HResult NS_E_FILE_FAILED_CHECKS => new(0xC00D0FCD);

    /// <summary>Windows Media Player cannot create the library. You must be logged on as an administrator or a member of the Administrators group to install the Player. For more information, contact your system administrator.</summary>
    [Description("""Windows Media Player cannot create the library. You must be logged on as an administrator or a member of the Administrators group to install the Player. For more information, contact your system administrator.""")]
    [HResultConstant(0xC00D0FCE)]
    public static HResult NS_E_MEDIA_LIBRARY_FAILED => new(0xC00D0FCE);

    /// <summary>The file is already in use. Close other programs that might be using the file, or stop playing the file, and then try again.</summary>
    [Description("""The file is already in use. Close other programs that might be using the file, or stop playing the file, and then try again.""")]
    [HResultConstant(0xC00D0FCF)]
    public static HResult NS_E_SHARING_VIOLATION => new(0xC00D0FCF);

    /// <summary>Windows Media Player has encountered an unknown error.</summary>
    [Description("""Windows Media Player has encountered an unknown error.""")]
    [HResultConstant(0xC00D0FD0)]
    public static HResult NS_E_NO_ERROR_STRING_FOUND => new(0xC00D0FD0);

    /// <summary>The Windows Media Player ActiveX control cannot connect to remote media services, but will continue with local media services.</summary>
    [Description("""The Windows Media Player ActiveX control cannot connect to remote media services, but will continue with local media services.""")]
    [HResultConstant(0xC00D0FD1)]
    public static HResult NS_E_WMPOCX_NO_REMOTE_CORE => new(0xC00D0FD1);

    /// <summary>The requested method or property is not available because the Windows Media Player ActiveX control has not been properly activated.</summary>
    [Description("""The requested method or property is not available because the Windows Media Player ActiveX control has not been properly activated.""")]
    [HResultConstant(0xC00D0FD2)]
    public static HResult NS_E_WMPOCX_NO_ACTIVE_CORE => new(0xC00D0FD2);

    /// <summary>The Windows Media Player ActiveX control is not running in remote mode.</summary>
    [Description("""The Windows Media Player ActiveX control is not running in remote mode.""")]
    [HResultConstant(0xC00D0FD3)]
    public static HResult NS_E_WMPOCX_NOT_RUNNING_REMOTELY => new(0xC00D0FD3);

    /// <summary>An error occurred while trying to get the remote Windows Media Player window.</summary>
    [Description("""An error occurred while trying to get the remote Windows Media Player window.""")]
    [HResultConstant(0xC00D0FD4)]
    public static HResult NS_E_WMPOCX_NO_REMOTE_WINDOW => new(0xC00D0FD4);

    /// <summary>Windows Media Player has encountered an unknown error.</summary>
    [Description("""Windows Media Player has encountered an unknown error.""")]
    [HResultConstant(0xC00D0FD5)]
    public static HResult NS_E_WMPOCX_ERRORMANAGERNOTAVAILABLE => new(0xC00D0FD5);

    /// <summary>Windows Media Player was not closed properly. A damaged or incompatible plug-in might have caused the problem to occur. As a precaution, all optional plug-ins have been disabled.</summary>
    [Description("""Windows Media Player was not closed properly. A damaged or incompatible plug-in might have caused the problem to occur. As a precaution, all optional plug-ins have been disabled.""")]
    [HResultConstant(0xC00D0FD6)]
    public static HResult NS_E_PLUGIN_NOTSHUTDOWN => new(0xC00D0FD6);

    /// <summary>Windows Media Player cannot find the specified path. Verify that the path is typed correctly. If it is, the path does not exist in the specified location, or the computer where the path is located is not available.</summary>
    [Description("""Windows Media Player cannot find the specified path. Verify that the path is typed correctly. If it is, the path does not exist in the specified location, or the computer where the path is located is not available.""")]
    [HResultConstant(0xC00D0FD7)]
    public static HResult NS_E_WMP_CANNOT_FIND_FOLDER => new(0xC00D0FD7);

    /// <summary>Windows Media Player cannot save a file that is being streamed.</summary>
    [Description("""Windows Media Player cannot save a file that is being streamed.""")]
    [HResultConstant(0xC00D0FD8)]
    public static HResult NS_E_WMP_STREAMING_RECORDING_NOT_ALLOWED => new(0xC00D0FD8);

    /// <summary>Windows Media Player cannot find the selected plug-in. The Player will try to remove it from the menu. To use this plug-in, install it again.</summary>
    [Description("""Windows Media Player cannot find the selected plug-in. The Player will try to remove it from the menu. To use this plug-in, install it again.""")]
    [HResultConstant(0xC00D0FD9)]
    public static HResult NS_E_WMP_PLUGINDLL_NOTFOUND => new(0xC00D0FD9);

    /// <summary>Action requires input from the user.</summary>
    [Description("""Action requires input from the user.""")]
    [HResultConstant(0xC00D0FDA)]
    public static HResult NS_E_NEED_TO_ASK_USER => new(0xC00D0FDA);

    /// <summary>The Windows Media Player ActiveX control must be in a docked state for this action to be performed.</summary>
    [Description("""The Windows Media Player ActiveX control must be in a docked state for this action to be performed.""")]
    [HResultConstant(0xC00D0FDB)]
    public static HResult NS_E_WMPOCX_PLAYER_NOT_DOCKED => new(0xC00D0FDB);

    /// <summary>The Windows Media Player external object is not ready.</summary>
    [Description("""The Windows Media Player external object is not ready.""")]
    [HResultConstant(0xC00D0FDC)]
    public static HResult NS_E_WMP_EXTERNAL_NOTREADY => new(0xC00D0FDC);

    /// <summary>Windows Media Player cannot perform the requested action. Your computer's time and date might not be set correctly.</summary>
    [Description("""Windows Media Player cannot perform the requested action. Your computer's time and date might not be set correctly.""")]
    [HResultConstant(0xC00D0FDD)]
    public static HResult NS_E_WMP_MLS_STALE_DATA => new(0xC00D0FDD);

    /// <summary>The control (%s) does not support creation of sub-controls, yet (%d) sub-controls have been specified.</summary>
    [Description("""The control (%s) does not support creation of sub-controls, yet (%d) sub-controls have been specified.""")]
    [HResultConstant(0xC00D0FDE)]
    public static HResult NS_E_WMP_UI_SUBCONTROLSNOTSUPPORTED => new(0xC00D0FDE);

    /// <summary>Version mismatch: (%.1f required, %.1f found).</summary>
    [Description("""Version mismatch: (%.1f required, %.1f found).""")]
    [HResultConstant(0xC00D0FDF)]
    public static HResult NS_E_WMP_UI_VERSIONMISMATCH => new(0xC00D0FDF);

    /// <summary>The layout manager was given valid XML that wasn't a theme file.</summary>
    [Description("""The layout manager was given valid XML that wasn't a theme file.""")]
    [HResultConstant(0xC00D0FE0)]
    public static HResult NS_E_WMP_UI_NOTATHEMEFILE => new(0xC00D0FE0);

    /// <summary>The %s subelement could not be found on the %s object.</summary>
    [Description("""The %s subelement could not be found on the %s object.""")]
    [HResultConstant(0xC00D0FE1)]
    public static HResult NS_E_WMP_UI_SUBELEMENTNOTFOUND => new(0xC00D0FE1);

    /// <summary>An error occurred parsing the version tag. Valid version tags are of the form: <?wmp version='1.0'?>.</summary>
    [Description("""An error occurred parsing the version tag. Valid version tags are of the form: <?wmp version='1.0'?>.""")]
    [HResultConstant(0xC00D0FE2)]
    public static HResult NS_E_WMP_UI_VERSIONPARSE => new(0xC00D0FE2);

    /// <summary>The view specified in for the 'currentViewID' property (%s) was not found in this theme file.</summary>
    [Description("""The view specified in for the 'currentViewID' property (%s) was not found in this theme file.""")]
    [HResultConstant(0xC00D0FE3)]
    public static HResult NS_E_WMP_UI_VIEWIDNOTFOUND => new(0xC00D0FE3);

    /// <summary>This error used internally for hit testing.</summary>
    [Description("""This error used internally for hit testing.""")]
    [HResultConstant(0xC00D0FE4)]
    public static HResult NS_E_WMP_UI_PASSTHROUGH => new(0xC00D0FE4);

    /// <summary>Attributes were specified for the %s object, but the object was not available to send them to.</summary>
    [Description("""Attributes were specified for the %s object, but the object was not available to send them to.""")]
    [HResultConstant(0xC00D0FE5)]
    public static HResult NS_E_WMP_UI_OBJECTNOTFOUND => new(0xC00D0FE5);

    /// <summary>The %s event already has a handler, the second handler was ignored.</summary>
    [Description("""The %s event already has a handler, the second handler was ignored.""")]
    [HResultConstant(0xC00D0FE6)]
    public static HResult NS_E_WMP_UI_SECONDHANDLER => new(0xC00D0FE6);

    /// <summary>No .wms file found in skin archive.</summary>
    [Description("""No .wms file found in skin archive.""")]
    [HResultConstant(0xC00D0FE7)]
    public static HResult NS_E_WMP_UI_NOSKININZIP => new(0xC00D0FE7);

    /// <summary>Windows Media Player encountered a problem while downloading the file. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player encountered a problem while downloading the file. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D0FEA)]
    public static HResult NS_E_WMP_URLDOWNLOADFAILED => new(0xC00D0FEA);

    /// <summary>The Windows Media Player ActiveX control cannot load the requested uiMode and cannot roll back to the existing uiMode.</summary>
    [Description("""The Windows Media Player ActiveX control cannot load the requested uiMode and cannot roll back to the existing uiMode.""")]
    [HResultConstant(0xC00D0FEB)]
    public static HResult NS_E_WMPOCX_UNABLE_TO_LOAD_SKIN => new(0xC00D0FEB);

    /// <summary>Windows Media Player encountered a problem with the skin file. The skin file might not be valid.</summary>
    [Description("""Windows Media Player encountered a problem with the skin file. The skin file might not be valid.""")]
    [HResultConstant(0xC00D0FEC)]
    public static HResult NS_E_WMP_INVALID_SKIN => new(0xC00D0FEC);

    /// <summary>Windows Media Player cannot send the link because your email program is not responding. Verify that your email program is configured properly, and then try again. For more information about email, see Windows Help.</summary>
    [Description("""Windows Media Player cannot send the link because your email program is not responding. Verify that your email program is configured properly, and then try again. For more information about email, see Windows Help.""")]
    [HResultConstant(0xC00D0FED)]
    public static HResult NS_E_WMP_SENDMAILFAILED => new(0xC00D0FED);

    /// <summary>Windows Media Player cannot switch to full mode because your computer administrator has locked this skin.</summary>
    [Description("""Windows Media Player cannot switch to full mode because your computer administrator has locked this skin.""")]
    [HResultConstant(0xC00D0FEE)]
    public static HResult NS_E_WMP_LOCKEDINSKINMODE => new(0xC00D0FEE);

    /// <summary>Windows Media Player encountered a problem while saving the file. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player encountered a problem while saving the file. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D0FEF)]
    public static HResult NS_E_WMP_FAILED_TO_SAVE_FILE => new(0xC00D0FEF);

    /// <summary>Windows Media Player cannot overwrite a read-only file. Try using a different file name.</summary>
    [Description("""Windows Media Player cannot overwrite a read-only file. Try using a different file name.""")]
    [HResultConstant(0xC00D0FF0)]
    public static HResult NS_E_WMP_SAVEAS_READONLY => new(0xC00D0FF0);

    /// <summary>Windows Media Player encountered a problem while creating or saving the playlist. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player encountered a problem while creating or saving the playlist. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D0FF1)]
    public static HResult NS_E_WMP_FAILED_TO_SAVE_PLAYLIST => new(0xC00D0FF1);

    /// <summary>Windows Media Player cannot open the Windows Media Download file. The file might be damaged.</summary>
    [Description("""Windows Media Player cannot open the Windows Media Download file. The file might be damaged.""")]
    [HResultConstant(0xC00D0FF2)]
    public static HResult NS_E_WMP_FAILED_TO_OPEN_WMD => new(0xC00D0FF2);

    /// <summary>The file cannot be added to the library because it is a protected DVR-MS file. This content cannot be played back by Windows Media Player.</summary>
    [Description("""The file cannot be added to the library because it is a protected DVR-MS file. This content cannot be played back by Windows Media Player.""")]
    [HResultConstant(0xC00D0FF3)]
    public static HResult NS_E_WMP_CANT_PLAY_PROTECTED => new(0xC00D0FF3);

    /// <summary>Media sharing has been turned off because a required Windows setting or component has changed. For additional assistance, click Web Help.</summary>
    [Description("""Media sharing has been turned off because a required Windows setting or component has changed. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D0FF4)]
    public static HResult NS_E_SHARING_STATE_OUT_OF_SYNC => new(0xC00D0FF4);

    /// <summary>Exclusive Services launch failed because the Windows Media Player is already running.</summary>
    [Description("""Exclusive Services launch failed because the Windows Media Player is already running.""")]
    [HResultConstant(0xC00D0FFA)]
    public static HResult NS_E_WMPOCX_REMOTE_PLAYER_ALREADY_RUNNING => new(0xC00D0FFA);

    /// <summary>JPG Images are not recommended for use as a mappingImage.</summary>
    [Description("""JPG Images are not recommended for use as a mappingImage.""")]
    [HResultConstant(0xC00D1004)]
    public static HResult NS_E_WMP_RBC_JPGMAPPINGIMAGE => new(0xC00D1004);

    /// <summary>JPG Images are not recommended when using a transparencyColor.</summary>
    [Description("""JPG Images are not recommended when using a transparencyColor.""")]
    [HResultConstant(0xC00D1005)]
    public static HResult NS_E_WMP_JPGTRANSPARENCY => new(0xC00D1005);

    /// <summary>The Max property cannot be less than Min property.</summary>
    [Description("""The Max property cannot be less than Min property.""")]
    [HResultConstant(0xC00D1009)]
    public static HResult NS_E_WMP_INVALID_MAX_VAL => new(0xC00D1009);

    /// <summary>The Min property cannot be greater than Max property.</summary>
    [Description("""The Min property cannot be greater than Max property.""")]
    [HResultConstant(0xC00D100A)]
    public static HResult NS_E_WMP_INVALID_MIN_VAL => new(0xC00D100A);

    /// <summary>JPG Images are not recommended for use as a positionImage.</summary>
    [Description("""JPG Images are not recommended for use as a positionImage.""")]
    [HResultConstant(0xC00D100E)]
    public static HResult NS_E_WMP_CS_JPGPOSITIONIMAGE => new(0xC00D100E);

    /// <summary>The (%s) image's size is not evenly divisible by the positionImage's size.</summary>
    [Description("""The (%s) image's size is not evenly divisible by the positionImage's size.""")]
    [HResultConstant(0xC00D100F)]
    public static HResult NS_E_WMP_CS_NOTEVENLYDIVISIBLE => new(0xC00D100F);

    /// <summary>The ZIP reader opened a file and its signature did not match that of the ZIP files.</summary>
    [Description("""The ZIP reader opened a file and its signature did not match that of the ZIP files.""")]
    [HResultConstant(0xC00D1018)]
    public static HResult NS_E_WMPZIP_NOTAZIPFILE => new(0xC00D1018);

    /// <summary>The ZIP reader has detected that the file is corrupted.</summary>
    [Description("""The ZIP reader has detected that the file is corrupted.""")]
    [HResultConstant(0xC00D1019)]
    public static HResult NS_E_WMPZIP_CORRUPT => new(0xC00D1019);

    /// <summary>GetFileStream, SaveToFile, or SaveTemp file was called on the ZIP reader with a file name that was not found in the ZIP file.</summary>
    [Description("""GetFileStream, SaveToFile, or SaveTemp file was called on the ZIP reader with a file name that was not found in the ZIP file.""")]
    [HResultConstant(0xC00D101A)]
    public static HResult NS_E_WMPZIP_FILENOTFOUND => new(0xC00D101A);

    /// <summary>Image type not supported.</summary>
    [Description("""Image type not supported.""")]
    [HResultConstant(0xC00D1022)]
    public static HResult NS_E_WMP_IMAGE_FILETYPE_UNSUPPORTED => new(0xC00D1022);

    /// <summary>Image file might be corrupt.</summary>
    [Description("""Image file might be corrupt.""")]
    [HResultConstant(0xC00D1023)]
    public static HResult NS_E_WMP_IMAGE_INVALID_FORMAT => new(0xC00D1023);

    /// <summary>Unexpected end of file. GIF file might be corrupt.</summary>
    [Description("""Unexpected end of file. GIF file might be corrupt.""")]
    [HResultConstant(0xC00D1024)]
    public static HResult NS_E_WMP_GIF_UNEXPECTED_ENDOFFILE => new(0xC00D1024);

    /// <summary>Invalid GIF file.</summary>
    [Description("""Invalid GIF file.""")]
    [HResultConstant(0xC00D1025)]
    public static HResult NS_E_WMP_GIF_INVALID_FORMAT => new(0xC00D1025);

    /// <summary>Invalid GIF version. Only 87a or 89a supported.</summary>
    [Description("""Invalid GIF version. Only 87a or 89a supported.""")]
    [HResultConstant(0xC00D1026)]
    public static HResult NS_E_WMP_GIF_BAD_VERSION_NUMBER => new(0xC00D1026);

    /// <summary>No images found in GIF file.</summary>
    [Description("""No images found in GIF file.""")]
    [HResultConstant(0xC00D1027)]
    public static HResult NS_E_WMP_GIF_NO_IMAGE_IN_FILE => new(0xC00D1027);

    /// <summary>Invalid PNG image file format.</summary>
    [Description("""Invalid PNG image file format.""")]
    [HResultConstant(0xC00D1028)]
    public static HResult NS_E_WMP_PNG_INVALIDFORMAT => new(0xC00D1028);

    /// <summary>PNG bitdepth not supported.</summary>
    [Description("""PNG bitdepth not supported.""")]
    [HResultConstant(0xC00D1029)]
    public static HResult NS_E_WMP_PNG_UNSUPPORTED_BITDEPTH => new(0xC00D1029);

    /// <summary>Compression format defined in PNG file not supported,</summary>
    [Description("""Compression format defined in PNG file not supported,""")]
    [HResultConstant(0xC00D102A)]
    public static HResult NS_E_WMP_PNG_UNSUPPORTED_COMPRESSION => new(0xC00D102A);

    /// <summary>Filter method defined in PNG file not supported.</summary>
    [Description("""Filter method defined in PNG file not supported.""")]
    [HResultConstant(0xC00D102B)]
    public static HResult NS_E_WMP_PNG_UNSUPPORTED_FILTER => new(0xC00D102B);

    /// <summary>Interlace method defined in PNG file not supported.</summary>
    [Description("""Interlace method defined in PNG file not supported.""")]
    [HResultConstant(0xC00D102C)]
    public static HResult NS_E_WMP_PNG_UNSUPPORTED_INTERLACE => new(0xC00D102C);

    /// <summary>Bad CRC in PNG file.</summary>
    [Description("""Bad CRC in PNG file.""")]
    [HResultConstant(0xC00D102D)]
    public static HResult NS_E_WMP_PNG_UNSUPPORTED_BAD_CRC => new(0xC00D102D);

    /// <summary>Invalid bitmask in BMP file.</summary>
    [Description("""Invalid bitmask in BMP file.""")]
    [HResultConstant(0xC00D102E)]
    public static HResult NS_E_WMP_BMP_INVALID_BITMASK => new(0xC00D102E);

    /// <summary>Topdown DIB not supported.</summary>
    [Description("""Topdown DIB not supported.""")]
    [HResultConstant(0xC00D102F)]
    public static HResult NS_E_WMP_BMP_TOPDOWN_DIB_UNSUPPORTED => new(0xC00D102F);

    /// <summary>Bitmap could not be created.</summary>
    [Description("""Bitmap could not be created.""")]
    [HResultConstant(0xC00D1030)]
    public static HResult NS_E_WMP_BMP_BITMAP_NOT_CREATED => new(0xC00D1030);

    /// <summary>Compression format defined in BMP not supported.</summary>
    [Description("""Compression format defined in BMP not supported.""")]
    [HResultConstant(0xC00D1031)]
    public static HResult NS_E_WMP_BMP_COMPRESSION_UNSUPPORTED => new(0xC00D1031);

    /// <summary>Invalid Bitmap format.</summary>
    [Description("""Invalid Bitmap format.""")]
    [HResultConstant(0xC00D1032)]
    public static HResult NS_E_WMP_BMP_INVALID_FORMAT => new(0xC00D1032);

    /// <summary>JPEG Arithmetic coding not supported.</summary>
    [Description("""JPEG Arithmetic coding not supported.""")]
    [HResultConstant(0xC00D1033)]
    public static HResult NS_E_WMP_JPG_JERR_ARITHCODING_NOTIMPL => new(0xC00D1033);

    /// <summary>Invalid JPEG format.</summary>
    [Description("""Invalid JPEG format.""")]
    [HResultConstant(0xC00D1034)]
    public static HResult NS_E_WMP_JPG_INVALID_FORMAT => new(0xC00D1034);

    /// <summary>Invalid JPEG format.</summary>
    [Description("""Invalid JPEG format.""")]
    [HResultConstant(0xC00D1035)]
    public static HResult NS_E_WMP_JPG_BAD_DCTSIZE => new(0xC00D1035);

    /// <summary>Internal version error. Unexpected JPEG library version.</summary>
    [Description("""Internal version error. Unexpected JPEG library version.""")]
    [HResultConstant(0xC00D1036)]
    public static HResult NS_E_WMP_JPG_BAD_VERSION_NUMBER => new(0xC00D1036);

    /// <summary>Internal JPEG Library error. Unsupported JPEG data precision.</summary>
    [Description("""Internal JPEG Library error. Unsupported JPEG data precision.""")]
    [HResultConstant(0xC00D1037)]
    public static HResult NS_E_WMP_JPG_BAD_PRECISION => new(0xC00D1037);

    /// <summary>JPEG CCIR601 not supported.</summary>
    [Description("""JPEG CCIR601 not supported.""")]
    [HResultConstant(0xC00D1038)]
    public static HResult NS_E_WMP_JPG_CCIR601_NOTIMPL => new(0xC00D1038);

    /// <summary>No image found in JPEG file.</summary>
    [Description("""No image found in JPEG file.""")]
    [HResultConstant(0xC00D1039)]
    public static HResult NS_E_WMP_JPG_NO_IMAGE_IN_FILE => new(0xC00D1039);

    /// <summary>Could not read JPEG file.</summary>
    [Description("""Could not read JPEG file.""")]
    [HResultConstant(0xC00D103A)]
    public static HResult NS_E_WMP_JPG_READ_ERROR => new(0xC00D103A);

    /// <summary>JPEG Fractional sampling not supported.</summary>
    [Description("""JPEG Fractional sampling not supported.""")]
    [HResultConstant(0xC00D103B)]
    public static HResult NS_E_WMP_JPG_FRACT_SAMPLE_NOTIMPL => new(0xC00D103B);

    /// <summary>JPEG image too large. Maximum image size supported is 65500 X 65500.</summary>
    [Description("""JPEG image too large. Maximum image size supported is 65500 X 65500.""")]
    [HResultConstant(0xC00D103C)]
    public static HResult NS_E_WMP_JPG_IMAGE_TOO_BIG => new(0xC00D103C);

    /// <summary>Unexpected end of file reached in JPEG file.</summary>
    [Description("""Unexpected end of file reached in JPEG file.""")]
    [HResultConstant(0xC00D103D)]
    public static HResult NS_E_WMP_JPG_UNEXPECTED_ENDOFFILE => new(0xC00D103D);

    /// <summary>Unsupported JPEG SOF marker found.</summary>
    [Description("""Unsupported JPEG SOF marker found.""")]
    [HResultConstant(0xC00D103E)]
    public static HResult NS_E_WMP_JPG_SOF_UNSUPPORTED => new(0xC00D103E);

    /// <summary>Unknown JPEG marker found.</summary>
    [Description("""Unknown JPEG marker found.""")]
    [HResultConstant(0xC00D103F)]
    public static HResult NS_E_WMP_JPG_UNKNOWN_MARKER => new(0xC00D103F);

    /// <summary>Windows Media Player cannot display the picture file. The player either does not support the picture type or the picture is corrupted.</summary>
    [Description("""Windows Media Player cannot display the picture file. The player either does not support the picture type or the picture is corrupted.""")]
    [HResultConstant(0xC00D1044)]
    public static HResult NS_E_WMP_FAILED_TO_OPEN_IMAGE => new(0xC00D1044);

    /// <summary>Windows Media Player cannot compute a Digital Audio Id for the song. It is too short.</summary>
    [Description("""Windows Media Player cannot compute a Digital Audio Id for the song. It is too short.""")]
    [HResultConstant(0xC00D1049)]
    public static HResult NS_E_WMP_DAI_SONGTOOSHORT => new(0xC00D1049);

    /// <summary>Windows Media Player cannot play the file at the requested speed.</summary>
    [Description("""Windows Media Player cannot play the file at the requested speed.""")]
    [HResultConstant(0xC00D104A)]
    public static HResult NS_E_WMG_RATEUNAVAILABLE => new(0xC00D104A);

    /// <summary>The rendering or digital signal processing plug-in cannot be instantiated.</summary>
    [Description("""The rendering or digital signal processing plug-in cannot be instantiated.""")]
    [HResultConstant(0xC00D104B)]
    public static HResult NS_E_WMG_PLUGINUNAVAILABLE => new(0xC00D104B);

    /// <summary>The file cannot be queued for seamless playback.</summary>
    [Description("""The file cannot be queued for seamless playback.""")]
    [HResultConstant(0xC00D104C)]
    public static HResult NS_E_WMG_CANNOTQUEUE => new(0xC00D104C);

    /// <summary>Windows Media Player cannot download media usage rights for a file in the playlist.</summary>
    [Description("""Windows Media Player cannot download media usage rights for a file in the playlist.""")]
    [HResultConstant(0xC00D104D)]
    public static HResult NS_E_WMG_PREROLLLICENSEACQUISITIONNOTALLOWED => new(0xC00D104D);

    /// <summary>Windows Media Player encountered an error while trying to queue a file.</summary>
    [Description("""Windows Media Player encountered an error while trying to queue a file.""")]
    [HResultConstant(0xC00D104E)]
    public static HResult NS_E_WMG_UNEXPECTEDPREROLLSTATUS => new(0xC00D104E);

    /// <summary>Windows Media Player cannot play the protected file. The Player cannot verify that the connection to your video card is secure. Try installing an updated device driver for your video card.</summary>
    [Description("""Windows Media Player cannot play the protected file. The Player cannot verify that the connection to your video card is secure. Try installing an updated device driver for your video card.""")]
    [HResultConstant(0xC00D1051)]
    public static HResult NS_E_WMG_INVALID_COPP_CERTIFICATE => new(0xC00D1051);

    /// <summary>Windows Media Player cannot play the protected file. The Player detected that the connection to your hardware might not be secure.</summary>
    [Description("""Windows Media Player cannot play the protected file. The Player detected that the connection to your hardware might not be secure.""")]
    [HResultConstant(0xC00D1052)]
    public static HResult NS_E_WMG_COPP_SECURITY_INVALID => new(0xC00D1052);

    /// <summary>Windows Media Player output link protection is unsupported on this system.</summary>
    [Description("""Windows Media Player output link protection is unsupported on this system.""")]
    [HResultConstant(0xC00D1053)]
    public static HResult NS_E_WMG_COPP_UNSUPPORTED => new(0xC00D1053);

    /// <summary>Operation attempted in an invalid graph state.</summary>
    [Description("""Operation attempted in an invalid graph state.""")]
    [HResultConstant(0xC00D1054)]
    public static HResult NS_E_WMG_INVALIDSTATE => new(0xC00D1054);

    /// <summary>A renderer cannot be inserted in a stream while one already exists.</summary>
    [Description("""A renderer cannot be inserted in a stream while one already exists.""")]
    [HResultConstant(0xC00D1055)]
    public static HResult NS_E_WMG_SINKALREADYEXISTS => new(0xC00D1055);

    /// <summary>The Windows Media SDK interface needed to complete the operation does not exist at this time.</summary>
    [Description("""The Windows Media SDK interface needed to complete the operation does not exist at this time.""")]
    [HResultConstant(0xC00D1056)]
    public static HResult NS_E_WMG_NOSDKINTERFACE => new(0xC00D1056);

    /// <summary>Windows Media Player cannot play a portion of the file because it requires a codec that either could not be downloaded or that is not supported by the Player.</summary>
    [Description("""Windows Media Player cannot play a portion of the file because it requires a codec that either could not be downloaded or that is not supported by the Player.""")]
    [HResultConstant(0xC00D1057)]
    public static HResult NS_E_WMG_NOTALLOUTPUTSRENDERED => new(0xC00D1057);

    /// <summary>File transfer streams are not allowed in the standalone Player.</summary>
    [Description("""File transfer streams are not allowed in the standalone Player.""")]
    [HResultConstant(0xC00D1058)]
    public static HResult NS_E_WMG_FILETRANSFERNOTALLOWED => new(0xC00D1058);

    /// <summary>Windows Media Player cannot play the file. The Player does not support the format you are trying to play.</summary>
    [Description("""Windows Media Player cannot play the file. The Player does not support the format you are trying to play.""")]
    [HResultConstant(0xC00D1059)]
    public static HResult NS_E_WMR_UNSUPPORTEDSTREAM => new(0xC00D1059);

    /// <summary>An operation was attempted on a pin that does not exist in the DirectShow filter graph.</summary>
    [Description("""An operation was attempted on a pin that does not exist in the DirectShow filter graph.""")]
    [HResultConstant(0xC00D105A)]
    public static HResult NS_E_WMR_PINNOTFOUND => new(0xC00D105A);

    /// <summary>Specified operation cannot be completed while waiting for a media format change from the SDK.</summary>
    [Description("""Specified operation cannot be completed while waiting for a media format change from the SDK.""")]
    [HResultConstant(0xC00D105B)]
    public static HResult NS_E_WMR_WAITINGONFORMATSWITCH => new(0xC00D105B);

    /// <summary>Specified operation cannot be completed because the source filter does not exist.</summary>
    [Description("""Specified operation cannot be completed because the source filter does not exist.""")]
    [HResultConstant(0xC00D105C)]
    public static HResult NS_E_WMR_NOSOURCEFILTER => new(0xC00D105C);

    /// <summary>The specified type does not match this pin.</summary>
    [Description("""The specified type does not match this pin.""")]
    [HResultConstant(0xC00D105D)]
    public static HResult NS_E_WMR_PINTYPENOMATCH => new(0xC00D105D);

    /// <summary>The WMR Source Filter does not have a callback available.</summary>
    [Description("""The WMR Source Filter does not have a callback available.""")]
    [HResultConstant(0xC00D105E)]
    public static HResult NS_E_WMR_NOCALLBACKAVAILABLE => new(0xC00D105E);

    /// <summary>The specified property has not been set on this sample.</summary>
    [Description("""The specified property has not been set on this sample.""")]
    [HResultConstant(0xC00D1062)]
    public static HResult NS_E_WMR_SAMPLEPROPERTYNOTSET => new(0xC00D1062);

    /// <summary>A plug-in is required to correctly play the file. To determine if the plug-in is available to download, click Web Help.</summary>
    [Description("""A plug-in is required to correctly play the file. To determine if the plug-in is available to download, click Web Help.""")]
    [HResultConstant(0xC00D1063)]
    public static HResult NS_E_WMR_CANNOT_RENDER_BINARY_STREAM => new(0xC00D1063);

    /// <summary>Windows Media Player cannot play the file because your media usage rights are corrupted. If you previously backed up your media usage rights, try restoring them.</summary>
    [Description("""Windows Media Player cannot play the file because your media usage rights are corrupted. If you previously backed up your media usage rights, try restoring them.""")]
    [HResultConstant(0xC00D1064)]
    public static HResult NS_E_WMG_LICENSE_TAMPERED => new(0xC00D1064);

    /// <summary>Windows Media Player cannot play protected files that contain binary streams.</summary>
    [Description("""Windows Media Player cannot play protected files that contain binary streams.""")]
    [HResultConstant(0xC00D1065)]
    public static HResult NS_E_WMR_WILLNOT_RENDER_BINARY_STREAM => new(0xC00D1065);

    /// <summary>Windows Media Player cannot play the playlist because it is not valid.</summary>
    [Description("""Windows Media Player cannot play the playlist because it is not valid.""")]
    [HResultConstant(0xC00D1068)]
    public static HResult NS_E_WMX_UNRECOGNIZED_PLAYLIST_FORMAT => new(0xC00D1068);

    /// <summary>Windows Media Player cannot play the playlist because it is not valid.</summary>
    [Description("""Windows Media Player cannot play the playlist because it is not valid.""")]
    [HResultConstant(0xC00D1069)]
    public static HResult NS_E_ASX_INVALIDFORMAT => new(0xC00D1069);

    /// <summary>A later version of Windows Media Player might be required to play this playlist.</summary>
    [Description("""A later version of Windows Media Player might be required to play this playlist.""")]
    [HResultConstant(0xC00D106A)]
    public static HResult NS_E_ASX_INVALIDVERSION => new(0xC00D106A);

    /// <summary>The format of a REPEAT loop within the current playlist file is not valid.</summary>
    [Description("""The format of a REPEAT loop within the current playlist file is not valid.""")]
    [HResultConstant(0xC00D106B)]
    public static HResult NS_E_ASX_INVALID_REPEAT_BLOCK => new(0xC00D106B);

    /// <summary>Windows Media Player cannot save the playlist because it does not contain any items.</summary>
    [Description("""Windows Media Player cannot save the playlist because it does not contain any items.""")]
    [HResultConstant(0xC00D106C)]
    public static HResult NS_E_ASX_NOTHING_TO_WRITE => new(0xC00D106C);

    /// <summary>Windows Media Player cannot play the playlist because it is not valid.</summary>
    [Description("""Windows Media Player cannot play the playlist because it is not valid.""")]
    [HResultConstant(0xC00D106D)]
    public static HResult NS_E_URLLIST_INVALIDFORMAT => new(0xC00D106D);

    /// <summary>The specified attribute does not exist.</summary>
    [Description("""The specified attribute does not exist.""")]
    [HResultConstant(0xC00D106E)]
    public static HResult NS_E_WMX_ATTRIBUTE_DOES_NOT_EXIST => new(0xC00D106E);

    /// <summary>The specified attribute already exists.</summary>
    [Description("""The specified attribute already exists.""")]
    [HResultConstant(0xC00D106F)]
    public static HResult NS_E_WMX_ATTRIBUTE_ALREADY_EXISTS => new(0xC00D106F);

    /// <summary>Cannot retrieve the specified attribute.</summary>
    [Description("""Cannot retrieve the specified attribute.""")]
    [HResultConstant(0xC00D1070)]
    public static HResult NS_E_WMX_ATTRIBUTE_UNRETRIEVABLE => new(0xC00D1070);

    /// <summary>The specified item does not exist in the current playlist.</summary>
    [Description("""The specified item does not exist in the current playlist.""")]
    [HResultConstant(0xC00D1071)]
    public static HResult NS_E_WMX_ITEM_DOES_NOT_EXIST => new(0xC00D1071);

    /// <summary>Items of the specified type cannot be created within the current playlist.</summary>
    [Description("""Items of the specified type cannot be created within the current playlist.""")]
    [HResultConstant(0xC00D1072)]
    public static HResult NS_E_WMX_ITEM_TYPE_ILLEGAL => new(0xC00D1072);

    /// <summary>The specified item cannot be set in the current playlist.</summary>
    [Description("""The specified item cannot be set in the current playlist.""")]
    [HResultConstant(0xC00D1073)]
    public static HResult NS_E_WMX_ITEM_UNSETTABLE => new(0xC00D1073);

    /// <summary>Windows Media Player cannot perform the requested action because the playlist does not contain any items.</summary>
    [Description("""Windows Media Player cannot perform the requested action because the playlist does not contain any items.""")]
    [HResultConstant(0xC00D1074)]
    public static HResult NS_E_WMX_PLAYLIST_EMPTY => new(0xC00D1074);

    /// <summary>The specified auto playlist contains a filter type that is either not valid or is not installed on this computer.</summary>
    [Description("""The specified auto playlist contains a filter type that is either not valid or is not installed on this computer.""")]
    [HResultConstant(0xC00D1075)]
    public static HResult NS_E_MLS_SMARTPLAYLIST_FILTER_NOT_REGISTERED => new(0xC00D1075);

    /// <summary>Windows Media Player cannot play the file because the associated playlist contains too many nested playlists.</summary>
    [Description("""Windows Media Player cannot play the file because the associated playlist contains too many nested playlists.""")]
    [HResultConstant(0xC00D1076)]
    public static HResult NS_E_WMX_INVALID_FORMAT_OVER_NESTING => new(0xC00D1076);

    /// <summary>Windows Media Player cannot find the file. Verify that the path is typed correctly. If it is, the file might not exist in the specified location, or the computer where the file is stored might not be available.</summary>
    [Description("""Windows Media Player cannot find the file. Verify that the path is typed correctly. If it is, the file might not exist in the specified location, or the computer where the file is stored might not be available.""")]
    [HResultConstant(0xC00D107C)]
    public static HResult NS_E_WMPCORE_NOSOURCEURLSTRING => new(0xC00D107C);

    /// <summary>Failed to create the Global Interface Table.</summary>
    [Description("""Failed to create the Global Interface Table.""")]
    [HResultConstant(0xC00D107D)]
    public static HResult NS_E_WMPCORE_COCREATEFAILEDFORGITOBJECT => new(0xC00D107D);

    /// <summary>Failed to get the marshaled graph event handler interface.</summary>
    [Description("""Failed to get the marshaled graph event handler interface.""")]
    [HResultConstant(0xC00D107E)]
    public static HResult NS_E_WMPCORE_FAILEDTOGETMARSHALLEDEVENTHANDLERINTERFACE => new(0xC00D107E);

    /// <summary>Buffer is too small for copying media type.</summary>
    [Description("""Buffer is too small for copying media type.""")]
    [HResultConstant(0xC00D107F)]
    public static HResult NS_E_WMPCORE_BUFFERTOOSMALL => new(0xC00D107F);

    /// <summary>The current state of the Player does not allow this operation.</summary>
    [Description("""The current state of the Player does not allow this operation.""")]
    [HResultConstant(0xC00D1080)]
    public static HResult NS_E_WMPCORE_UNAVAILABLE => new(0xC00D1080);

    /// <summary>The playlist manager does not understand the current play mode (for example, shuffle or normal).</summary>
    [Description("""The playlist manager does not understand the current play mode (for example, shuffle or normal).""")]
    [HResultConstant(0xC00D1081)]
    public static HResult NS_E_WMPCORE_INVALIDPLAYLISTMODE => new(0xC00D1081);

    /// <summary>Windows Media Player cannot play the file because it is not in the current playlist.</summary>
    [Description("""Windows Media Player cannot play the file because it is not in the current playlist.""")]
    [HResultConstant(0xC00D1086)]
    public static HResult NS_E_WMPCORE_ITEMNOTINPLAYLIST => new(0xC00D1086);

    /// <summary>There are no items in the playlist. Add items to the playlist, and then try again.</summary>
    [Description("""There are no items in the playlist. Add items to the playlist, and then try again.""")]
    [HResultConstant(0xC00D1087)]
    public static HResult NS_E_WMPCORE_PLAYLISTEMPTY => new(0xC00D1087);

    /// <summary>The web page cannot be displayed because no web browser is installed on your computer.</summary>
    [Description("""The web page cannot be displayed because no web browser is installed on your computer.""")]
    [HResultConstant(0xC00D1088)]
    public static HResult NS_E_WMPCORE_NOBROWSER => new(0xC00D1088);

    /// <summary>Windows Media Player cannot find the specified file. Verify the path is typed correctly. If it is, the file does not exist in the specified location, or the computer where the file is stored is not available.</summary>
    [Description("""Windows Media Player cannot find the specified file. Verify the path is typed correctly. If it is, the file does not exist in the specified location, or the computer where the file is stored is not available.""")]
    [HResultConstant(0xC00D1089)]
    public static HResult NS_E_WMPCORE_UNRECOGNIZED_MEDIA_URL => new(0xC00D1089);

    /// <summary>Graph with the specified URL was not found in the prerolled graph list.</summary>
    [Description("""Graph with the specified URL was not found in the prerolled graph list.""")]
    [HResultConstant(0xC00D108A)]
    public static HResult NS_E_WMPCORE_GRAPH_NOT_IN_LIST => new(0xC00D108A);

    /// <summary>Windows Media Player cannot perform the requested operation because there is only one item in the playlist.</summary>
    [Description("""Windows Media Player cannot perform the requested operation because there is only one item in the playlist.""")]
    [HResultConstant(0xC00D108B)]
    public static HResult NS_E_WMPCORE_PLAYLIST_EMPTY_OR_SINGLE_MEDIA => new(0xC00D108B);

    /// <summary>An error sink was never registered for the calling object.</summary>
    [Description("""An error sink was never registered for the calling object.""")]
    [HResultConstant(0xC00D108C)]
    public static HResult NS_E_WMPCORE_ERRORSINKNOTREGISTERED => new(0xC00D108C);

    /// <summary>The error manager is not available to respond to errors.</summary>
    [Description("""The error manager is not available to respond to errors.""")]
    [HResultConstant(0xC00D108D)]
    public static HResult NS_E_WMPCORE_ERRORMANAGERNOTAVAILABLE => new(0xC00D108D);

    /// <summary>The Web Help URL cannot be opened.</summary>
    [Description("""The Web Help URL cannot be opened.""")]
    [HResultConstant(0xC00D108E)]
    public static HResult NS_E_WMPCORE_WEBHELPFAILED => new(0xC00D108E);

    /// <summary>Could not resume playing next item in playlist.</summary>
    [Description("""Could not resume playing next item in playlist.""")]
    [HResultConstant(0xC00D108F)]
    public static HResult NS_E_WMPCORE_MEDIA_ERROR_RESUME_FAILED => new(0xC00D108F);

    /// <summary>Windows Media Player cannot play the file because the associated playlist does not contain any items or the playlist is not valid.</summary>
    [Description("""Windows Media Player cannot play the file because the associated playlist does not contain any items or the playlist is not valid.""")]
    [HResultConstant(0xC00D1090)]
    public static HResult NS_E_WMPCORE_NO_REF_IN_ENTRY => new(0xC00D1090);

    /// <summary>An empty string for playlist attribute name was found.</summary>
    [Description("""An empty string for playlist attribute name was found.""")]
    [HResultConstant(0xC00D1091)]
    public static HResult NS_E_WMPCORE_WMX_LIST_ATTRIBUTE_NAME_EMPTY => new(0xC00D1091);

    /// <summary>A playlist attribute name that is not valid was found.</summary>
    [Description("""A playlist attribute name that is not valid was found.""")]
    [HResultConstant(0xC00D1092)]
    public static HResult NS_E_WMPCORE_WMX_LIST_ATTRIBUTE_NAME_ILLEGAL => new(0xC00D1092);

    /// <summary>An empty string for a playlist attribute value was found.</summary>
    [Description("""An empty string for a playlist attribute value was found.""")]
    [HResultConstant(0xC00D1093)]
    public static HResult NS_E_WMPCORE_WMX_LIST_ATTRIBUTE_VALUE_EMPTY => new(0xC00D1093);

    /// <summary>An illegal value for a playlist attribute was found.</summary>
    [Description("""An illegal value for a playlist attribute was found.""")]
    [HResultConstant(0xC00D1094)]
    public static HResult NS_E_WMPCORE_WMX_LIST_ATTRIBUTE_VALUE_ILLEGAL => new(0xC00D1094);

    /// <summary>An empty string for a playlist item attribute name was found.</summary>
    [Description("""An empty string for a playlist item attribute name was found.""")]
    [HResultConstant(0xC00D1095)]
    public static HResult NS_E_WMPCORE_WMX_LIST_ITEM_ATTRIBUTE_NAME_EMPTY => new(0xC00D1095);

    /// <summary>An illegal value for a playlist item attribute name was found.</summary>
    [Description("""An illegal value for a playlist item attribute name was found.""")]
    [HResultConstant(0xC00D1096)]
    public static HResult NS_E_WMPCORE_WMX_LIST_ITEM_ATTRIBUTE_NAME_ILLEGAL => new(0xC00D1096);

    /// <summary>An illegal value for a playlist item attribute was found.</summary>
    [Description("""An illegal value for a playlist item attribute was found.""")]
    [HResultConstant(0xC00D1097)]
    public static HResult NS_E_WMPCORE_WMX_LIST_ITEM_ATTRIBUTE_VALUE_EMPTY => new(0xC00D1097);

    /// <summary>The playlist does not contain any items.</summary>
    [Description("""The playlist does not contain any items.""")]
    [HResultConstant(0xC00D1098)]
    public static HResult NS_E_WMPCORE_LIST_ENTRY_NO_REF => new(0xC00D1098);

    /// <summary>Windows Media Player cannot play the file. The file is either corrupted or the Player does not support the format you are trying to play.</summary>
    [Description("""Windows Media Player cannot play the file. The file is either corrupted or the Player does not support the format you are trying to play.""")]
    [HResultConstant(0xC00D1099)]
    public static HResult NS_E_WMPCORE_MISNAMED_FILE => new(0xC00D1099);

    /// <summary>The codec downloaded for this file does not appear to be properly signed, so it cannot be installed.</summary>
    [Description("""The codec downloaded for this file does not appear to be properly signed, so it cannot be installed.""")]
    [HResultConstant(0xC00D109A)]
    public static HResult NS_E_WMPCORE_CODEC_NOT_TRUSTED => new(0xC00D109A);

    /// <summary>Windows Media Player cannot play the file. One or more codecs required to play the file could not be found.</summary>
    [Description("""Windows Media Player cannot play the file. One or more codecs required to play the file could not be found.""")]
    [HResultConstant(0xC00D109B)]
    public static HResult NS_E_WMPCORE_CODEC_NOT_FOUND => new(0xC00D109B);

    /// <summary>Windows Media Player cannot play the file because a required codec is not installed on your computer. To try downloading the codec, turn on the "Download codecs automatically" option.</summary>
    [Description("""Windows Media Player cannot play the file because a required codec is not installed on your computer. To try downloading the codec, turn on the "Download codecs automatically" option.""")]
    [HResultConstant(0xC00D109C)]
    public static HResult NS_E_WMPCORE_CODEC_DOWNLOAD_NOT_ALLOWED => new(0xC00D109C);

    /// <summary>Windows Media Player encountered a problem while downloading the playlist. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player encountered a problem while downloading the playlist. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D109D)]
    public static HResult NS_E_WMPCORE_ERROR_DOWNLOADING_PLAYLIST => new(0xC00D109D);

    /// <summary>Failed to build the playlist.</summary>
    [Description("""Failed to build the playlist.""")]
    [HResultConstant(0xC00D109E)]
    public static HResult NS_E_WMPCORE_FAILED_TO_BUILD_PLAYLIST => new(0xC00D109E);

    /// <summary>Playlist has no alternates to switch into.</summary>
    [Description("""Playlist has no alternates to switch into.""")]
    [HResultConstant(0xC00D109F)]
    public static HResult NS_E_WMPCORE_PLAYLIST_ITEM_ALTERNATE_NONE => new(0xC00D109F);

    /// <summary>No more playlist alternates available to switch to.</summary>
    [Description("""No more playlist alternates available to switch to.""")]
    [HResultConstant(0xC00D10A0)]
    public static HResult NS_E_WMPCORE_PLAYLIST_ITEM_ALTERNATE_EXHAUSTED => new(0xC00D10A0);

    /// <summary>Could not find the name of the alternate playlist to switch into.</summary>
    [Description("""Could not find the name of the alternate playlist to switch into.""")]
    [HResultConstant(0xC00D10A1)]
    public static HResult NS_E_WMPCORE_PLAYLIST_ITEM_ALTERNATE_NAME_NOT_FOUND => new(0xC00D10A1);

    /// <summary>Failed to switch to an alternate for this media.</summary>
    [Description("""Failed to switch to an alternate for this media.""")]
    [HResultConstant(0xC00D10A2)]
    public static HResult NS_E_WMPCORE_PLAYLIST_ITEM_ALTERNATE_MORPH_FAILED => new(0xC00D10A2);

    /// <summary>Failed to initialize an alternate for the media.</summary>
    [Description("""Failed to initialize an alternate for the media.""")]
    [HResultConstant(0xC00D10A3)]
    public static HResult NS_E_WMPCORE_PLAYLIST_ITEM_ALTERNATE_INIT_FAILED => new(0xC00D10A3);

    /// <summary>No URL specified for the roll over Refs in the playlist file.</summary>
    [Description("""No URL specified for the roll over Refs in the playlist file.""")]
    [HResultConstant(0xC00D10A4)]
    public static HResult NS_E_WMPCORE_MEDIA_ALTERNATE_REF_EMPTY => new(0xC00D10A4);

    /// <summary>Encountered a playlist with no name.</summary>
    [Description("""Encountered a playlist with no name.""")]
    [HResultConstant(0xC00D10A5)]
    public static HResult NS_E_WMPCORE_PLAYLIST_NO_EVENT_NAME => new(0xC00D10A5);

    /// <summary>A required attribute in the event block of the playlist was not found.</summary>
    [Description("""A required attribute in the event block of the playlist was not found.""")]
    [HResultConstant(0xC00D10A6)]
    public static HResult NS_E_WMPCORE_PLAYLIST_EVENT_ATTRIBUTE_ABSENT => new(0xC00D10A6);

    /// <summary>No items were found in the event block of the playlist.</summary>
    [Description("""No items were found in the event block of the playlist.""")]
    [HResultConstant(0xC00D10A7)]
    public static HResult NS_E_WMPCORE_PLAYLIST_EVENT_EMPTY => new(0xC00D10A7);

    /// <summary>No playlist was found while returning from a nested playlist.</summary>
    [Description("""No playlist was found while returning from a nested playlist.""")]
    [HResultConstant(0xC00D10A8)]
    public static HResult NS_E_WMPCORE_PLAYLIST_STACK_EMPTY => new(0xC00D10A8);

    /// <summary>The media item is not active currently.</summary>
    [Description("""The media item is not active currently.""")]
    [HResultConstant(0xC00D10A9)]
    public static HResult NS_E_WMPCORE_CURRENT_MEDIA_NOT_ACTIVE => new(0xC00D10A9);

    /// <summary>Windows Media Player cannot perform the requested action because you chose to cancel it.</summary>
    [Description("""Windows Media Player cannot perform the requested action because you chose to cancel it.""")]
    [HResultConstant(0xC00D10AB)]
    public static HResult NS_E_WMPCORE_USER_CANCEL => new(0xC00D10AB);

    /// <summary>Windows Media Player encountered a problem with the playlist. The format of the playlist is not valid.</summary>
    [Description("""Windows Media Player encountered a problem with the playlist. The format of the playlist is not valid.""")]
    [HResultConstant(0xC00D10AC)]
    public static HResult NS_E_WMPCORE_PLAYLIST_REPEAT_EMPTY => new(0xC00D10AC);

    /// <summary>Media object corresponding to start of a playlist repeat block was not found.</summary>
    [Description("""Media object corresponding to start of a playlist repeat block was not found.""")]
    [HResultConstant(0xC00D10AD)]
    public static HResult NS_E_WMPCORE_PLAYLIST_REPEAT_START_MEDIA_NONE => new(0xC00D10AD);

    /// <summary>Media object corresponding to the end of a playlist repeat block was not found.</summary>
    [Description("""Media object corresponding to the end of a playlist repeat block was not found.""")]
    [HResultConstant(0xC00D10AE)]
    public static HResult NS_E_WMPCORE_PLAYLIST_REPEAT_END_MEDIA_NONE => new(0xC00D10AE);

    /// <summary>The playlist URL supplied to the playlist manager is not valid.</summary>
    [Description("""The playlist URL supplied to the playlist manager is not valid.""")]
    [HResultConstant(0xC00D10AF)]
    public static HResult NS_E_WMPCORE_INVALID_PLAYLIST_URL => new(0xC00D10AF);

    /// <summary>Windows Media Player cannot play the file because it is corrupted.</summary>
    [Description("""Windows Media Player cannot play the file because it is corrupted.""")]
    [HResultConstant(0xC00D10B0)]
    public static HResult NS_E_WMPCORE_MISMATCHED_RUNTIME => new(0xC00D10B0);

    /// <summary>Windows Media Player cannot add the playlist to the library because the playlist does not contain any items.</summary>
    [Description("""Windows Media Player cannot add the playlist to the library because the playlist does not contain any items.""")]
    [HResultConstant(0xC00D10B1)]
    public static HResult NS_E_WMPCORE_PLAYLIST_IMPORT_FAILED_NO_ITEMS => new(0xC00D10B1);

    /// <summary>An error has occurred that could prevent the changing of the video contrast on this media.</summary>
    [Description("""An error has occurred that could prevent the changing of the video contrast on this media.""")]
    [HResultConstant(0xC00D10B2)]
    public static HResult NS_E_WMPCORE_VIDEO_TRANSFORM_FILTER_INSERTION => new(0xC00D10B2);

    /// <summary>Windows Media Player cannot play the file. If the file is located on the Internet, connect to the Internet. If the file is located on a removable storage card, insert the storage card.</summary>
    [Description("""Windows Media Player cannot play the file. If the file is located on the Internet, connect to the Internet. If the file is located on a removable storage card, insert the storage card.""")]
    [HResultConstant(0xC00D10B3)]
    public static HResult NS_E_WMPCORE_MEDIA_UNAVAILABLE => new(0xC00D10B3);

    /// <summary>The playlist contains an ENTRYREF for which no href was parsed. Check the syntax of playlist file.</summary>
    [Description("""The playlist contains an ENTRYREF for which no href was parsed. Check the syntax of playlist file.""")]
    [HResultConstant(0xC00D10B4)]
    public static HResult NS_E_WMPCORE_WMX_ENTRYREF_NO_REF => new(0xC00D10B4);

    /// <summary>Windows Media Player cannot play any items in the playlist. To find information about the problem, click the Now Playing tab, and then click the icon next to each file in the List pane.</summary>
    [Description("""Windows Media Player cannot play any items in the playlist. To find information about the problem, click the Now Playing tab, and then click the icon next to each file in the List pane.""")]
    [HResultConstant(0xC00D10B5)]
    public static HResult NS_E_WMPCORE_NO_PLAYABLE_MEDIA_IN_PLAYLIST => new(0xC00D10B5);

    /// <summary>Windows Media Player cannot play some or all of the items in the playlist because the playlist is nested.</summary>
    [Description("""Windows Media Player cannot play some or all of the items in the playlist because the playlist is nested.""")]
    [HResultConstant(0xC00D10B6)]
    public static HResult NS_E_WMPCORE_PLAYLIST_EMPTY_NESTED_PLAYLIST_SKIPPED_ITEMS => new(0xC00D10B6);

    /// <summary>Windows Media Player cannot play the file at this time. Try again later.</summary>
    [Description("""Windows Media Player cannot play the file at this time. Try again later.""")]
    [HResultConstant(0xC00D10B7)]
    public static HResult NS_E_WMPCORE_BUSY => new(0xC00D10B7);

    /// <summary>There is no child playlist available for this media item at this time.</summary>
    [Description("""There is no child playlist available for this media item at this time.""")]
    [HResultConstant(0xC00D10B8)]
    public static HResult NS_E_WMPCORE_MEDIA_CHILD_PLAYLIST_UNAVAILABLE => new(0xC00D10B8);

    /// <summary>There is no child playlist for this media item.</summary>
    [Description("""There is no child playlist for this media item.""")]
    [HResultConstant(0xC00D10B9)]
    public static HResult NS_E_WMPCORE_MEDIA_NO_CHILD_PLAYLIST => new(0xC00D10B9);

    /// <summary>Windows Media Player cannot find the file. The link from the item in the library to its associated digital media file might be broken. To fix the problem, try repairing the link or removing the item from the library.</summary>
    [Description("""Windows Media Player cannot find the file. The link from the item in the library to its associated digital media file might be broken. To fix the problem, try repairing the link or removing the item from the library.""")]
    [HResultConstant(0xC00D10BA)]
    public static HResult NS_E_WMPCORE_FILE_NOT_FOUND => new(0xC00D10BA);

    /// <summary>The temporary file was not found.</summary>
    [Description("""The temporary file was not found.""")]
    [HResultConstant(0xC00D10BB)]
    public static HResult NS_E_WMPCORE_TEMP_FILE_NOT_FOUND => new(0xC00D10BB);

    /// <summary>Windows Media Player cannot sync the file because the device needs to be updated.</summary>
    [Description("""Windows Media Player cannot sync the file because the device needs to be updated.""")]
    [HResultConstant(0xC00D10BC)]
    public static HResult NS_E_WMDM_REVOKED => new(0xC00D10BC);

    /// <summary>Windows Media Player cannot play the video because there is a problem with your video card.</summary>
    [Description("""Windows Media Player cannot play the video because there is a problem with your video card.""")]
    [HResultConstant(0xC00D10BD)]
    public static HResult NS_E_DDRAW_GENERIC => new(0xC00D10BD);

    /// <summary>Windows Media Player failed to change the screen mode for full-screen video playback.</summary>
    [Description("""Windows Media Player failed to change the screen mode for full-screen video playback.""")]
    [HResultConstant(0xC00D10BE)]
    public static HResult NS_E_DISPLAY_MODE_CHANGE_FAILED => new(0xC00D10BE);

    /// <summary>Windows Media Player cannot play one or more files. For additional information, right-click an item that cannot be played, and then click Error Details.</summary>
    [Description("""Windows Media Player cannot play one or more files. For additional information, right-click an item that cannot be played, and then click Error Details.""")]
    [HResultConstant(0xC00D10BF)]
    public static HResult NS_E_PLAYLIST_CONTAINS_ERRORS => new(0xC00D10BF);

    /// <summary>Cannot change the proxy name if the proxy setting is not set to custom.</summary>
    [Description("""Cannot change the proxy name if the proxy setting is not set to custom.""")]
    [HResultConstant(0xC00D10C0)]
    public static HResult NS_E_CHANGING_PROXY_NAME => new(0xC00D10C0);

    /// <summary>Cannot change the proxy port if the proxy setting is not set to custom.</summary>
    [Description("""Cannot change the proxy port if the proxy setting is not set to custom.""")]
    [HResultConstant(0xC00D10C1)]
    public static HResult NS_E_CHANGING_PROXY_PORT => new(0xC00D10C1);

    /// <summary>Cannot change the proxy exception list if the proxy setting is not set to custom.</summary>
    [Description("""Cannot change the proxy exception list if the proxy setting is not set to custom.""")]
    [HResultConstant(0xC00D10C2)]
    public static HResult NS_E_CHANGING_PROXY_EXCEPTIONLIST => new(0xC00D10C2);

    /// <summary>Cannot change the proxy bypass flag if the proxy setting is not set to custom.</summary>
    [Description("""Cannot change the proxy bypass flag if the proxy setting is not set to custom.""")]
    [HResultConstant(0xC00D10C3)]
    public static HResult NS_E_CHANGING_PROXYBYPASS => new(0xC00D10C3);

    /// <summary>Cannot find the specified protocol.</summary>
    [Description("""Cannot find the specified protocol.""")]
    [HResultConstant(0xC00D10C4)]
    public static HResult NS_E_CHANGING_PROXY_PROTOCOL_NOT_FOUND => new(0xC00D10C4);

    /// <summary>Cannot change the language settings. Either the graph has no audio or the audio only supports one language.</summary>
    [Description("""Cannot change the language settings. Either the graph has no audio or the audio only supports one language.""")]
    [HResultConstant(0xC00D10C5)]
    public static HResult NS_E_GRAPH_NOAUDIOLANGUAGE => new(0xC00D10C5);

    /// <summary>The graph has no audio language selected.</summary>
    [Description("""The graph has no audio language selected.""")]
    [HResultConstant(0xC00D10C6)]
    public static HResult NS_E_GRAPH_NOAUDIOLANGUAGESELECTED => new(0xC00D10C6);

    /// <summary>This is not a media CD.</summary>
    [Description("""This is not a media CD.""")]
    [HResultConstant(0xC00D10C7)]
    public static HResult NS_E_CORECD_NOTAMEDIACD => new(0xC00D10C7);

    /// <summary>Windows Media Player cannot play the file because the URL is too long.</summary>
    [Description("""Windows Media Player cannot play the file because the URL is too long.""")]
    [HResultConstant(0xC00D10C8)]
    public static HResult NS_E_WMPCORE_MEDIA_URL_TOO_LONG => new(0xC00D10C8);

    /// <summary>To play the selected item, you must install the Macromedia Flash Player. To download the Macromedia Flash Player, go to the Adobe website.</summary>
    [Description("""To play the selected item, you must install the Macromedia Flash Player. To download the Macromedia Flash Player, go to the Adobe website.""")]
    [HResultConstant(0xC00D10C9)]
    public static HResult NS_E_WMPFLASH_CANT_FIND_COM_SERVER => new(0xC00D10C9);

    /// <summary>To play the selected item, you must install a later version of the Macromedia Flash Player. To download the Macromedia Flash Player, go to the Adobe website.</summary>
    [Description("""To play the selected item, you must install a later version of the Macromedia Flash Player. To download the Macromedia Flash Player, go to the Adobe website.""")]
    [HResultConstant(0xC00D10CA)]
    public static HResult NS_E_WMPFLASH_INCOMPATIBLEVERSION => new(0xC00D10CA);

    /// <summary>Windows Media Player cannot play the file because your Internet security settings prohibit the use of ActiveX controls.</summary>
    [Description("""Windows Media Player cannot play the file because your Internet security settings prohibit the use of ActiveX controls.""")]
    [HResultConstant(0xC00D10CB)]
    public static HResult NS_E_WMPOCXGRAPH_IE_DISALLOWS_ACTIVEX_CONTROLS => new(0xC00D10CB);

    /// <summary>The use of this method requires an existing reference to the Player object.</summary>
    [Description("""The use of this method requires an existing reference to the Player object.""")]
    [HResultConstant(0xC00D10CC)]
    public static HResult NS_E_NEED_CORE_REFERENCE => new(0xC00D10CC);

    /// <summary>Windows Media Player cannot play the CD. The disc might be dirty or damaged.</summary>
    [Description("""Windows Media Player cannot play the CD. The disc might be dirty or damaged.""")]
    [HResultConstant(0xC00D10CD)]
    public static HResult NS_E_MEDIACD_READ_ERROR => new(0xC00D10CD);

    /// <summary>Windows Media Player cannot play the file because your Internet security settings prohibit the use of ActiveX controls.</summary>
    [Description("""Windows Media Player cannot play the file because your Internet security settings prohibit the use of ActiveX controls.""")]
    [HResultConstant(0xC00D10CE)]
    public static HResult NS_E_IE_DISALLOWS_ACTIVEX_CONTROLS => new(0xC00D10CE);

    /// <summary>Flash playback has been turned off in Windows Media Player.</summary>
    [Description("""Flash playback has been turned off in Windows Media Player.""")]
    [HResultConstant(0xC00D10CF)]
    public static HResult NS_E_FLASH_PLAYBACK_NOT_ALLOWED => new(0xC00D10CF);

    /// <summary>Windows Media Player cannot rip the CD because a valid rip location cannot be created.</summary>
    [Description("""Windows Media Player cannot rip the CD because a valid rip location cannot be created.""")]
    [HResultConstant(0xC00D10D0)]
    public static HResult NS_E_UNABLE_TO_CREATE_RIP_LOCATION => new(0xC00D10D0);

    /// <summary>Windows Media Player cannot play the file because a required codec is not installed on your computer.</summary>
    [Description("""Windows Media Player cannot play the file because a required codec is not installed on your computer.""")]
    [HResultConstant(0xC00D10D1)]
    public static HResult NS_E_WMPCORE_SOME_CODECS_MISSING => new(0xC00D10D1);

    /// <summary>Windows Media Player cannot rip one or more tracks from the CD.</summary>
    [Description("""Windows Media Player cannot rip one or more tracks from the CD.""")]
    [HResultConstant(0xC00D10D2)]
    public static HResult NS_E_WMP_RIP_FAILED => new(0xC00D10D2);

    /// <summary>Windows Media Player encountered a problem while ripping the track from the CD. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player encountered a problem while ripping the track from the CD. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D10D3)]
    public static HResult NS_E_WMP_FAILED_TO_RIP_TRACK => new(0xC00D10D3);

    /// <summary>Windows Media Player encountered a problem while erasing the disc. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player encountered a problem while erasing the disc. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D10D4)]
    public static HResult NS_E_WMP_ERASE_FAILED => new(0xC00D10D4);

    /// <summary>Windows Media Player encountered a problem while formatting the device. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player encountered a problem while formatting the device. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D10D5)]
    public static HResult NS_E_WMP_FORMAT_FAILED => new(0xC00D10D5);

    /// <summary>This file cannot be burned to a CD because it is not located on your computer.</summary>
    [Description("""This file cannot be burned to a CD because it is not located on your computer.""")]
    [HResultConstant(0xC00D10D6)]
    public static HResult NS_E_WMP_CANNOT_BURN_NON_LOCAL_FILE => new(0xC00D10D6);

    /// <summary>It is not possible to burn this file type to an audio CD. Windows Media Player can burn the following file types to an audio CD: WMA, MP3, or WAV.</summary>
    [Description("""It is not possible to burn this file type to an audio CD. Windows Media Player can burn the following file types to an audio CD: WMA, MP3, or WAV.""")]
    [HResultConstant(0xC00D10D7)]
    public static HResult NS_E_WMP_FILE_TYPE_CANNOT_BURN_TO_AUDIO_CD => new(0xC00D10D7);

    /// <summary>This file is too large to fit on a disc.</summary>
    [Description("""This file is too large to fit on a disc.""")]
    [HResultConstant(0xC00D10D8)]
    public static HResult NS_E_WMP_FILE_DOES_NOT_FIT_ON_CD => new(0xC00D10D8);

    /// <summary>It is not possible to determine if this file can fit on a disc because Windows Media Player cannot detect the length of the file. Playing the file before burning might enable the Player to detect the file length.</summary>
    [Description("""It is not possible to determine if this file can fit on a disc because Windows Media Player cannot detect the length of the file. Playing the file before burning might enable the Player to detect the file length.""")]
    [HResultConstant(0xC00D10D9)]
    public static HResult NS_E_WMP_FILE_NO_DURATION => new(0xC00D10D9);

    /// <summary>Windows Media Player encountered a problem while burning the file to the disc. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player encountered a problem while burning the file to the disc. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D10DA)]
    public static HResult NS_E_PDA_FAILED_TO_BURN => new(0xC00D10DA);

    /// <summary>Windows Media Player cannot burn the audio CD because some items in the list that you chose to buy could not be downloaded from the online store.</summary>
    [Description("""Windows Media Player cannot burn the audio CD because some items in the list that you chose to buy could not be downloaded from the online store.""")]
    [HResultConstant(0xC00D10DC)]
    public static HResult NS_E_FAILED_DOWNLOAD_ABORT_BURN => new(0xC00D10DC);

    /// <summary>Windows Media Player cannot play the file. Try using Windows Update or Device Manager to update the device drivers for your audio and video cards. For information about using Windows Update or Device Manager, see Windows Help.</summary>
    [Description("""Windows Media Player cannot play the file. Try using Windows Update or Device Manager to update the device drivers for your audio and video cards. For information about using Windows Update or Device Manager, see Windows Help.""")]
    [HResultConstant(0xC00D10DD)]
    public static HResult NS_E_WMPCORE_DEVICE_DRIVERS_MISSING => new(0xC00D10DD);

    /// <summary>Windows Media Player has detected that you are not connected to the Internet. Connect to the Internet, and then try again.</summary>
    [Description("""Windows Media Player has detected that you are not connected to the Internet. Connect to the Internet, and then try again.""")]
    [HResultConstant(0xC00D1126)]
    public static HResult NS_E_WMPIM_USEROFFLINE => new(0xC00D1126);

    /// <summary>The attempt to connect to the Internet was canceled.</summary>
    [Description("""The attempt to connect to the Internet was canceled.""")]
    [HResultConstant(0xC00D1127)]
    public static HResult NS_E_WMPIM_USERCANCELED => new(0xC00D1127);

    /// <summary>The attempt to connect to the Internet failed.</summary>
    [Description("""The attempt to connect to the Internet failed.""")]
    [HResultConstant(0xC00D1128)]
    public static HResult NS_E_WMPIM_DIALUPFAILED => new(0xC00D1128);

    /// <summary>Windows Media Player has encountered an unknown network error.</summary>
    [Description("""Windows Media Player has encountered an unknown network error.""")]
    [HResultConstant(0xC00D1129)]
    public static HResult NS_E_WINSOCK_ERROR_STRING => new(0xC00D1129);

    /// <summary>No window is currently listening to Backup and Restore events.</summary>
    [Description("""No window is currently listening to Backup and Restore events.""")]
    [HResultConstant(0xC00D1130)]
    public static HResult NS_E_WMPBR_NOLISTENER => new(0xC00D1130);

    /// <summary>Your media usage rights were not backed up because the backup was canceled.</summary>
    [Description("""Your media usage rights were not backed up because the backup was canceled.""")]
    [HResultConstant(0xC00D1131)]
    public static HResult NS_E_WMPBR_BACKUPCANCEL => new(0xC00D1131);

    /// <summary>Your media usage rights were not restored because the restoration was canceled.</summary>
    [Description("""Your media usage rights were not restored because the restoration was canceled.""")]
    [HResultConstant(0xC00D1132)]
    public static HResult NS_E_WMPBR_RESTORECANCEL => new(0xC00D1132);

    /// <summary>An error occurred while backing up or restoring your media usage rights. A required web page cannot be displayed.</summary>
    [Description("""An error occurred while backing up or restoring your media usage rights. A required web page cannot be displayed.""")]
    [HResultConstant(0xC00D1133)]
    public static HResult NS_E_WMPBR_ERRORWITHURL => new(0xC00D1133);

    /// <summary>Your media usage rights were not backed up because the backup was canceled.</summary>
    [Description("""Your media usage rights were not backed up because the backup was canceled.""")]
    [HResultConstant(0xC00D1134)]
    public static HResult NS_E_WMPBR_NAMECOLLISION => new(0xC00D1134);

    /// <summary>Windows Media Player cannot restore your media usage rights from the specified location. Choose another location, and then try again.</summary>
    [Description("""Windows Media Player cannot restore your media usage rights from the specified location. Choose another location, and then try again.""")]
    [HResultConstant(0xC00D1137)]
    public static HResult NS_E_WMPBR_DRIVE_INVALID => new(0xC00D1137);

    /// <summary>Windows Media Player cannot backup or restore your media usage rights.</summary>
    [Description("""Windows Media Player cannot backup or restore your media usage rights.""")]
    [HResultConstant(0xC00D1138)]
    public static HResult NS_E_WMPBR_BACKUPRESTOREFAILED => new(0xC00D1138);

    /// <summary>Windows Media Player cannot add the file to the library.</summary>
    [Description("""Windows Media Player cannot add the file to the library.""")]
    [HResultConstant(0xC00D1158)]
    public static HResult NS_E_WMP_CONVERT_FILE_FAILED => new(0xC00D1158);

    /// <summary>Windows Media Player cannot add the file to the library because the content provider prohibits it. For assistance, contact the company that provided the file.</summary>
    [Description("""Windows Media Player cannot add the file to the library because the content provider prohibits it. For assistance, contact the company that provided the file.""")]
    [HResultConstant(0xC00D1159)]
    public static HResult NS_E_WMP_CONVERT_NO_RIGHTS_ERRORURL => new(0xC00D1159);

    /// <summary>Windows Media Player cannot add the file to the library because the content provider prohibits it. For assistance, contact the company that provided the file.</summary>
    [Description("""Windows Media Player cannot add the file to the library because the content provider prohibits it. For assistance, contact the company that provided the file.""")]
    [HResultConstant(0xC00D115A)]
    public static HResult NS_E_WMP_CONVERT_NO_RIGHTS_NOERRORURL => new(0xC00D115A);

    /// <summary>Windows Media Player cannot add the file to the library. The file might not be valid.</summary>
    [Description("""Windows Media Player cannot add the file to the library. The file might not be valid.""")]
    [HResultConstant(0xC00D115B)]
    public static HResult NS_E_WMP_CONVERT_FILE_CORRUPT => new(0xC00D115B);

    /// <summary>Windows Media Player cannot add the file to the library. The plug-in required to add the file is not installed properly. For assistance, click Web Help to display the website of the company that provided the file.</summary>
    [Description("""Windows Media Player cannot add the file to the library. The plug-in required to add the file is not installed properly. For assistance, click Web Help to display the website of the company that provided the file.""")]
    [HResultConstant(0xC00D115C)]
    public static HResult NS_E_WMP_CONVERT_PLUGIN_UNAVAILABLE_ERRORURL => new(0xC00D115C);

    /// <summary>Windows Media Player cannot add the file to the library. The plug-in required to add the file is not installed properly. For assistance, contact the company that provided the file.</summary>
    [Description("""Windows Media Player cannot add the file to the library. The plug-in required to add the file is not installed properly. For assistance, contact the company that provided the file.""")]
    [HResultConstant(0xC00D115D)]
    public static HResult NS_E_WMP_CONVERT_PLUGIN_UNAVAILABLE_NOERRORURL => new(0xC00D115D);

    /// <summary>Windows Media Player cannot add the file to the library. The plug-in required to add the file is not installed properly. For assistance, contact the company that provided the file.</summary>
    [Description("""Windows Media Player cannot add the file to the library. The plug-in required to add the file is not installed properly. For assistance, contact the company that provided the file.""")]
    [HResultConstant(0xC00D115E)]
    public static HResult NS_E_WMP_CONVERT_PLUGIN_UNKNOWN_FILE_OWNER => new(0xC00D115E);

    /// <summary>Windows Media Player cannot play this DVD. Try installing an updated driver for your video card or obtaining a newer video card.</summary>
    [Description("""Windows Media Player cannot play this DVD. Try installing an updated driver for your video card or obtaining a newer video card.""")]
    [HResultConstant(0xC00D1160)]
    public static HResult NS_E_DVD_DISC_COPY_PROTECT_OUTPUT_NS => new(0xC00D1160);

    /// <summary>This DVD's resolution exceeds the maximum allowed by your component video outputs. Try reducing your screen resolution to 640 x 480, or turn off analog component outputs and use a VGA connection to your monitor.</summary>
    [Description("""This DVD's resolution exceeds the maximum allowed by your component video outputs. Try reducing your screen resolution to 640 x 480, or turn off analog component outputs and use a VGA connection to your monitor.""")]
    [HResultConstant(0xC00D1161)]
    public static HResult NS_E_DVD_DISC_COPY_PROTECT_OUTPUT_FAILED => new(0xC00D1161);

    /// <summary>Windows Media Player cannot display subtitles or highlights in DVD menus. Reinstall the DVD decoder or contact the DVD drive manufacturer to obtain an updated decoder.</summary>
    [Description("""Windows Media Player cannot display subtitles or highlights in DVD menus. Reinstall the DVD decoder or contact the DVD drive manufacturer to obtain an updated decoder.""")]
    [HResultConstant(0xC00D1162)]
    public static HResult NS_E_DVD_NO_SUBPICTURE_STREAM => new(0xC00D1162);

    /// <summary>Windows Media Player cannot play this DVD because there is a problem with digital copy protection between your DVD drive, decoder, and video card. Try installing an updated driver for your video card.</summary>
    [Description("""Windows Media Player cannot play this DVD because there is a problem with digital copy protection between your DVD drive, decoder, and video card. Try installing an updated driver for your video card.""")]
    [HResultConstant(0xC00D1163)]
    public static HResult NS_E_DVD_COPY_PROTECT => new(0xC00D1163);

    /// <summary>Windows Media Player cannot play the DVD. The disc was created in a manner that the Player does not support.</summary>
    [Description("""Windows Media Player cannot play the DVD. The disc was created in a manner that the Player does not support.""")]
    [HResultConstant(0xC00D1164)]
    public static HResult NS_E_DVD_AUTHORING_PROBLEM => new(0xC00D1164);

    /// <summary>Windows Media Player cannot play the DVD because the disc prohibits playback in your region of the world. You must obtain a disc that is intended for your geographic region.</summary>
    [Description("""Windows Media Player cannot play the DVD because the disc prohibits playback in your region of the world. You must obtain a disc that is intended for your geographic region.""")]
    [HResultConstant(0xC00D1165)]
    public static HResult NS_E_DVD_INVALID_DISC_REGION => new(0xC00D1165);

    /// <summary>Windows Media Player cannot play the DVD because your video card does not support DVD playback.</summary>
    [Description("""Windows Media Player cannot play the DVD because your video card does not support DVD playback.""")]
    [HResultConstant(0xC00D1166)]
    public static HResult NS_E_DVD_COMPATIBLE_VIDEO_CARD => new(0xC00D1166);

    /// <summary>Windows Media Player cannot play this DVD because it is not possible to turn on analog copy protection on the output display. Try installing an updated driver for your video card.</summary>
    [Description("""Windows Media Player cannot play this DVD because it is not possible to turn on analog copy protection on the output display. Try installing an updated driver for your video card.""")]
    [HResultConstant(0xC00D1167)]
    public static HResult NS_E_DVD_MACROVISION => new(0xC00D1167);

    /// <summary>Windows Media Player cannot play the DVD because the region assigned to your DVD drive does not match the region assigned to your DVD decoder.</summary>
    [Description("""Windows Media Player cannot play the DVD because the region assigned to your DVD drive does not match the region assigned to your DVD decoder.""")]
    [HResultConstant(0xC00D1168)]
    public static HResult NS_E_DVD_SYSTEM_DECODER_REGION => new(0xC00D1168);

    /// <summary>Windows Media Player cannot play the DVD because the disc prohibits playback in your region of the world. You must obtain a disc that is intended for your geographic region.</summary>
    [Description("""Windows Media Player cannot play the DVD because the disc prohibits playback in your region of the world. You must obtain a disc that is intended for your geographic region.""")]
    [HResultConstant(0xC00D1169)]
    public static HResult NS_E_DVD_DISC_DECODER_REGION => new(0xC00D1169);

    /// <summary>Windows Media Player cannot play DVD video. You might need to adjust your Windows display settings. Open display settings in Control Panel, and then try lowering your screen resolution and color quality settings.</summary>
    [Description("""Windows Media Player cannot play DVD video. You might need to adjust your Windows display settings. Open display settings in Control Panel, and then try lowering your screen resolution and color quality settings.""")]
    [HResultConstant(0xC00D116A)]
    public static HResult NS_E_DVD_NO_VIDEO_STREAM => new(0xC00D116A);

    /// <summary>Windows Media Player cannot play DVD audio. Verify that your sound card is set up correctly, and then try again.</summary>
    [Description("""Windows Media Player cannot play DVD audio. Verify that your sound card is set up correctly, and then try again.""")]
    [HResultConstant(0xC00D116B)]
    public static HResult NS_E_DVD_NO_AUDIO_STREAM => new(0xC00D116B);

    /// <summary>Windows Media Player cannot play DVD video. Close any open files and quit any other programs, and then try again. If the problem persists, restart your computer.</summary>
    [Description("""Windows Media Player cannot play DVD video. Close any open files and quit any other programs, and then try again. If the problem persists, restart your computer.""")]
    [HResultConstant(0xC00D116C)]
    public static HResult NS_E_DVD_GRAPH_BUILDING => new(0xC00D116C);

    /// <summary>Windows Media Player cannot play the DVD because a compatible DVD decoder is not installed on your computer.</summary>
    [Description("""Windows Media Player cannot play the DVD because a compatible DVD decoder is not installed on your computer.""")]
    [HResultConstant(0xC00D116D)]
    public static HResult NS_E_DVD_NO_DECODER => new(0xC00D116D);

    /// <summary>Windows Media Player cannot play the scene because it has a parental rating higher than the rating that you are authorized to view.</summary>
    [Description("""Windows Media Player cannot play the scene because it has a parental rating higher than the rating that you are authorized to view.""")]
    [HResultConstant(0xC00D116E)]
    public static HResult NS_E_DVD_PARENTAL => new(0xC00D116E);

    /// <summary>Windows Media Player cannot skip to the requested location on the DVD.</summary>
    [Description("""Windows Media Player cannot skip to the requested location on the DVD.""")]
    [HResultConstant(0xC00D116F)]
    public static HResult NS_E_DVD_CANNOT_JUMP => new(0xC00D116F);

    /// <summary>Windows Media Player cannot play the DVD because it is currently in use by another program. Quit the other program that is using the DVD, and then try again.</summary>
    [Description("""Windows Media Player cannot play the DVD because it is currently in use by another program. Quit the other program that is using the DVD, and then try again.""")]
    [HResultConstant(0xC00D1170)]
    public static HResult NS_E_DVD_DEVICE_CONTENTION => new(0xC00D1170);

    /// <summary>Windows Media Player cannot play DVD video. You might need to adjust your Windows display settings. Open display settings in Control Panel, and then try lowering your screen resolution and color quality settings.</summary>
    [Description("""Windows Media Player cannot play DVD video. You might need to adjust your Windows display settings. Open display settings in Control Panel, and then try lowering your screen resolution and color quality settings.""")]
    [HResultConstant(0xC00D1171)]
    public static HResult NS_E_DVD_NO_VIDEO_MEMORY => new(0xC00D1171);

    /// <summary>Windows Media Player cannot rip the DVD because it is copy protected.</summary>
    [Description("""Windows Media Player cannot rip the DVD because it is copy protected.""")]
    [HResultConstant(0xC00D1172)]
    public static HResult NS_E_DVD_CANNOT_COPY_PROTECTED => new(0xC00D1172);

    /// <summary>One of more of the required properties has not been set.</summary>
    [Description("""One of more of the required properties has not been set.""")]
    [HResultConstant(0xC00D1173)]
    public static HResult NS_E_DVD_REQUIRED_PROPERTY_NOT_SET => new(0xC00D1173);

    /// <summary>The specified title and/or chapter number does not exist on this DVD.</summary>
    [Description("""The specified title and/or chapter number does not exist on this DVD.""")]
    [HResultConstant(0xC00D1174)]
    public static HResult NS_E_DVD_INVALID_TITLE_CHAPTER => new(0xC00D1174);

    /// <summary>Windows Media Player cannot burn the files because the Player cannot find a burner. If the burner is connected properly, try using Windows Update to install the latest device driver.</summary>
    [Description("""Windows Media Player cannot burn the files because the Player cannot find a burner. If the burner is connected properly, try using Windows Update to install the latest device driver.""")]
    [HResultConstant(0xC00D1176)]
    public static HResult NS_E_NO_CD_BURNER => new(0xC00D1176);

    /// <summary>Windows Media Player does not detect storage media in the selected device. Insert storage media into the device, and then try again.</summary>
    [Description("""Windows Media Player does not detect storage media in the selected device. Insert storage media into the device, and then try again.""")]
    [HResultConstant(0xC00D1177)]
    public static HResult NS_E_DEVICE_IS_NOT_READY => new(0xC00D1177);

    /// <summary>Windows Media Player cannot sync this file. The Player might not support the file type.</summary>
    [Description("""Windows Media Player cannot sync this file. The Player might not support the file type.""")]
    [HResultConstant(0xC00D1178)]
    public static HResult NS_E_PDA_UNSUPPORTED_FORMAT => new(0xC00D1178);

    /// <summary>Windows Media Player does not detect a portable device. Connect your portable device, and then try again.</summary>
    [Description("""Windows Media Player does not detect a portable device. Connect your portable device, and then try again.""")]
    [HResultConstant(0xC00D1179)]
    public static HResult NS_E_NO_PDA => new(0xC00D1179);

    /// <summary>Windows Media Player encountered an error while communicating with the device. The storage card on the device might be full, the device might be turned off, or the device might not allow playlists or folders to be created on it.</summary>
    [Description("""Windows Media Player encountered an error while communicating with the device. The storage card on the device might be full, the device might be turned off, or the device might not allow playlists or folders to be created on it.""")]
    [HResultConstant(0xC00D117A)]
    public static HResult NS_E_PDA_UNSPECIFIED_ERROR => new(0xC00D117A);

    /// <summary>Windows Media Player encountered an error while burning a CD.</summary>
    [Description("""Windows Media Player encountered an error while burning a CD.""")]
    [HResultConstant(0xC00D117B)]
    public static HResult NS_E_MEMSTORAGE_BAD_DATA => new(0xC00D117B);

    /// <summary>Windows Media Player encountered an error while communicating with a portable device or CD drive.</summary>
    [Description("""Windows Media Player encountered an error while communicating with a portable device or CD drive.""")]
    [HResultConstant(0xC00D117C)]
    public static HResult NS_E_PDA_FAIL_SELECT_DEVICE => new(0xC00D117C);

    /// <summary>Windows Media Player cannot open the WAV file.</summary>
    [Description("""Windows Media Player cannot open the WAV file.""")]
    [HResultConstant(0xC00D117D)]
    public static HResult NS_E_PDA_FAIL_READ_WAVE_FILE => new(0xC00D117D);

    /// <summary>Windows Media Player failed to burn all the files to the CD. Select a slower recording speed, and then try again.</summary>
    [Description("""Windows Media Player failed to burn all the files to the CD. Select a slower recording speed, and then try again.""")]
    [HResultConstant(0xC00D117E)]
    public static HResult NS_E_IMAPI_LOSSOFSTREAMING => new(0xC00D117E);

    /// <summary>There is not enough storage space on the portable device to complete this operation. Delete some unneeded files on the portable device, and then try again.</summary>
    [Description("""There is not enough storage space on the portable device to complete this operation. Delete some unneeded files on the portable device, and then try again.""")]
    [HResultConstant(0xC00D117F)]
    public static HResult NS_E_PDA_DEVICE_FULL => new(0xC00D117F);

    /// <summary>Windows Media Player cannot burn the files. Verify that your burner is connected properly, and then try again. If the problem persists, reinstall the Player.</summary>
    [Description("""Windows Media Player cannot burn the files. Verify that your burner is connected properly, and then try again. If the problem persists, reinstall the Player.""")]
    [HResultConstant(0xC00D1180)]
    public static HResult NS_E_FAIL_LAUNCH_ROXIO_PLUGIN => new(0xC00D1180);

    /// <summary>Windows Media Player did not sync some files to the device because there is not enough storage space on the device.</summary>
    [Description("""Windows Media Player did not sync some files to the device because there is not enough storage space on the device.""")]
    [HResultConstant(0xC00D1181)]
    public static HResult NS_E_PDA_DEVICE_FULL_IN_SESSION => new(0xC00D1181);

    /// <summary>The disc in the burner is not valid. Insert a blank disc into the burner, and then try again.</summary>
    [Description("""The disc in the burner is not valid. Insert a blank disc into the burner, and then try again.""")]
    [HResultConstant(0xC00D1182)]
    public static HResult NS_E_IMAPI_MEDIUM_INVALIDTYPE => new(0xC00D1182);

    /// <summary>Windows Media Player cannot perform the requested action because the device does not support sync.</summary>
    [Description("""Windows Media Player cannot perform the requested action because the device does not support sync.""")]
    [HResultConstant(0xC00D1183)]
    public static HResult NS_E_PDA_MANUALDEVICE => new(0xC00D1183);

    /// <summary>To perform the requested action, you must first set up sync with the device.</summary>
    [Description("""To perform the requested action, you must first set up sync with the device.""")]
    [HResultConstant(0xC00D1184)]
    public static HResult NS_E_PDA_PARTNERSHIPNOTEXIST => new(0xC00D1184);

    /// <summary>You have already created sync partnerships with 16 devices. To create a new sync partnership, you must first end an existing partnership.</summary>
    [Description("""You have already created sync partnerships with 16 devices. To create a new sync partnership, you must first end an existing partnership.""")]
    [HResultConstant(0xC00D1185)]
    public static HResult NS_E_PDA_CANNOT_CREATE_ADDITIONAL_SYNC_RELATIONSHIP => new(0xC00D1185);

    /// <summary>Windows Media Player cannot sync the file because protected files cannot be converted to the required quality level or file format.</summary>
    [Description("""Windows Media Player cannot sync the file because protected files cannot be converted to the required quality level or file format.""")]
    [HResultConstant(0xC00D1186)]
    public static HResult NS_E_PDA_NO_TRANSCODE_OF_DRM => new(0xC00D1186);

    /// <summary>The folder that stores converted files is full. Either empty the folder or increase its size, and then try again.</summary>
    [Description("""The folder that stores converted files is full. Either empty the folder or increase its size, and then try again.""")]
    [HResultConstant(0xC00D1187)]
    public static HResult NS_E_PDA_TRANSCODECACHEFULL => new(0xC00D1187);

    /// <summary>There are too many files with the same name in the folder on the device. Change the file name or sync to a different folder.</summary>
    [Description("""There are too many files with the same name in the folder on the device. Change the file name or sync to a different folder.""")]
    [HResultConstant(0xC00D1188)]
    public static HResult NS_E_PDA_TOO_MANY_FILE_COLLISIONS => new(0xC00D1188);

    /// <summary>Windows Media Player cannot convert the file to the format required by the device.</summary>
    [Description("""Windows Media Player cannot convert the file to the format required by the device.""")]
    [HResultConstant(0xC00D1189)]
    public static HResult NS_E_PDA_CANNOT_TRANSCODE => new(0xC00D1189);

    /// <summary>You have reached the maximum number of files your device allows in a folder. If your device supports playback from subfolders, try creating subfolders on the device and storing some files in them.</summary>
    [Description("""You have reached the maximum number of files your device allows in a folder. If your device supports playback from subfolders, try creating subfolders on the device and storing some files in them.""")]
    [HResultConstant(0xC00D118A)]
    public static HResult NS_E_PDA_TOO_MANY_FILES_IN_DIRECTORY => new(0xC00D118A);

    /// <summary>Windows Media Player is already trying to start the Device Setup Wizard.</summary>
    [Description("""Windows Media Player is already trying to start the Device Setup Wizard.""")]
    [HResultConstant(0xC00D118B)]
    public static HResult NS_E_PROCESSINGSHOWSYNCWIZARD => new(0xC00D118B);

    /// <summary>Windows Media Player cannot convert this file format. If an updated version of the codec used to compress this file is available, install it and then try to sync the file again.</summary>
    [Description("""Windows Media Player cannot convert this file format. If an updated version of the codec used to compress this file is available, install it and then try to sync the file again.""")]
    [HResultConstant(0xC00D118C)]
    public static HResult NS_E_PDA_TRANSCODE_NOT_PERMITTED => new(0xC00D118C);

    /// <summary>Windows Media Player is busy setting up devices. Try again later.</summary>
    [Description("""Windows Media Player is busy setting up devices. Try again later.""")]
    [HResultConstant(0xC00D118D)]
    public static HResult NS_E_PDA_INITIALIZINGDEVICES => new(0xC00D118D);

    /// <summary>Your device is using an outdated driver that is no longer supported by Windows Media Player. For additional assistance, click Web Help.</summary>
    [Description("""Your device is using an outdated driver that is no longer supported by Windows Media Player. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D118E)]
    public static HResult NS_E_PDA_OBSOLETE_SP => new(0xC00D118E);

    /// <summary>Windows Media Player cannot sync the file because a file with the same name already exists on the device. Change the file name or try to sync the file to a different folder.</summary>
    [Description("""Windows Media Player cannot sync the file because a file with the same name already exists on the device. Change the file name or try to sync the file to a different folder.""")]
    [HResultConstant(0xC00D118F)]
    public static HResult NS_E_PDA_TITLE_COLLISION => new(0xC00D118F);

    /// <summary>Automatic and manual sync have been turned off temporarily. To sync to a device, restart Windows Media Player.</summary>
    [Description("""Automatic and manual sync have been turned off temporarily. To sync to a device, restart Windows Media Player.""")]
    [HResultConstant(0xC00D1190)]
    public static HResult NS_E_PDA_DEVICESUPPORTDISABLED => new(0xC00D1190);

    /// <summary>This device is not available. Connect the device to the computer, and then try again.</summary>
    [Description("""This device is not available. Connect the device to the computer, and then try again.""")]
    [HResultConstant(0xC00D1191)]
    public static HResult NS_E_PDA_NO_LONGER_AVAILABLE => new(0xC00D1191);

    /// <summary>Windows Media Player cannot sync the file because an error occurred while converting the file to another quality level or format. If the problem persists, remove the file from the list of files to sync.</summary>
    [Description("""Windows Media Player cannot sync the file because an error occurred while converting the file to another quality level or format. If the problem persists, remove the file from the list of files to sync.""")]
    [HResultConstant(0xC00D1192)]
    public static HResult NS_E_PDA_ENCODER_NOT_RESPONDING => new(0xC00D1192);

    /// <summary>Windows Media Player cannot sync the file to your device. The file might be stored in a location that is not supported. Copy the file from its current location to your hard disk, add it to your library, and then try to sync the file again.</summary>
    [Description("""Windows Media Player cannot sync the file to your device. The file might be stored in a location that is not supported. Copy the file from its current location to your hard disk, add it to your library, and then try to sync the file again.""")]
    [HResultConstant(0xC00D1193)]
    public static HResult NS_E_PDA_CANNOT_SYNC_FROM_LOCATION => new(0xC00D1193);

    /// <summary>Windows Media Player cannot open the specified URL. Verify that the Player is configured to use all available protocols, and then try again.</summary>
    [Description("""Windows Media Player cannot open the specified URL. Verify that the Player is configured to use all available protocols, and then try again.""")]
    [HResultConstant(0xC00D1194)]
    public static HResult NS_E_WMP_PROTOCOL_PROBLEM => new(0xC00D1194);

    /// <summary>Windows Media Player cannot perform the requested action because there is not enough storage space on your computer. Delete some unneeded files on your hard disk, and then try again.</summary>
    [Description("""Windows Media Player cannot perform the requested action because there is not enough storage space on your computer. Delete some unneeded files on your hard disk, and then try again.""")]
    [HResultConstant(0xC00D1195)]
    public static HResult NS_E_WMP_NO_DISK_SPACE => new(0xC00D1195);

    /// <summary>The server denied access to the file. Verify that you are using the correct user name and password.</summary>
    [Description("""The server denied access to the file. Verify that you are using the correct user name and password.""")]
    [HResultConstant(0xC00D1196)]
    public static HResult NS_E_WMP_LOGON_FAILURE => new(0xC00D1196);

    /// <summary>Windows Media Player cannot find the file. If you are trying to play, burn, or sync an item that is in your library, the item might point to a file that has been moved, renamed, or deleted.</summary>
    [Description("""Windows Media Player cannot find the file. If you are trying to play, burn, or sync an item that is in your library, the item might point to a file that has been moved, renamed, or deleted.""")]
    [HResultConstant(0xC00D1197)]
    public static HResult NS_E_WMP_CANNOT_FIND_FILE => new(0xC00D1197);

    /// <summary>Windows Media Player cannot connect to the server. The server name might not be correct, the server might not be available, or your proxy settings might not be correct.</summary>
    [Description("""Windows Media Player cannot connect to the server. The server name might not be correct, the server might not be available, or your proxy settings might not be correct.""")]
    [HResultConstant(0xC00D1198)]
    public static HResult NS_E_WMP_SERVER_INACCESSIBLE => new(0xC00D1198);

    /// <summary>Windows Media Player cannot play the file. The Player might not support the file type or might not support the codec that was used to compress the file.</summary>
    [Description("""Windows Media Player cannot play the file. The Player might not support the file type or might not support the codec that was used to compress the file.""")]
    [HResultConstant(0xC00D1199)]
    public static HResult NS_E_WMP_UNSUPPORTED_FORMAT => new(0xC00D1199);

    /// <summary>Windows Media Player cannot play the file. The Player might not support the file type or a required codec might not be installed on your computer.</summary>
    [Description("""Windows Media Player cannot play the file. The Player might not support the file type or a required codec might not be installed on your computer.""")]
    [HResultConstant(0xC00D119A)]
    public static HResult NS_E_WMP_DSHOW_UNSUPPORTED_FORMAT => new(0xC00D119A);

    /// <summary>Windows Media Player cannot create the playlist because the name already exists. Type a different playlist name.</summary>
    [Description("""Windows Media Player cannot create the playlist because the name already exists. Type a different playlist name.""")]
    [HResultConstant(0xC00D119B)]
    public static HResult NS_E_WMP_PLAYLIST_EXISTS => new(0xC00D119B);

    /// <summary>Windows Media Player cannot delete the playlist because it contains items that are not digital media files. Any digital media files in the playlist were deleted.</summary>
    [Description("""Windows Media Player cannot delete the playlist because it contains items that are not digital media files. Any digital media files in the playlist were deleted.""")]
    [HResultConstant(0xC00D119C)]
    public static HResult NS_E_WMP_NONMEDIA_FILES => new(0xC00D119C);

    /// <summary>The playlist cannot be opened because it is stored in a shared folder on another computer. If possible, move the playlist to the playlists folder on your computer.</summary>
    [Description("""The playlist cannot be opened because it is stored in a shared folder on another computer. If possible, move the playlist to the playlists folder on your computer.""")]
    [HResultConstant(0xC00D119D)]
    public static HResult NS_E_WMP_INVALID_ASX => new(0xC00D119D);

    /// <summary>Windows Media Player is already in use. Stop playing any items, close all Player dialog boxes, and then try again.</summary>
    [Description("""Windows Media Player is already in use. Stop playing any items, close all Player dialog boxes, and then try again.""")]
    [HResultConstant(0xC00D119E)]
    public static HResult NS_E_WMP_ALREADY_IN_USE => new(0xC00D119E);

    /// <summary>Windows Media Player encountered an error while burning. Verify that the burner is connected properly and that the disc is clean and not damaged.</summary>
    [Description("""Windows Media Player encountered an error while burning. Verify that the burner is connected properly and that the disc is clean and not damaged.""")]
    [HResultConstant(0xC00D119F)]
    public static HResult NS_E_WMP_IMAPI_FAILURE => new(0xC00D119F);

    /// <summary>Windows Media Player has encountered an unknown error with your portable device. Reconnect your portable device, and then try again.</summary>
    [Description("""Windows Media Player has encountered an unknown error with your portable device. Reconnect your portable device, and then try again.""")]
    [HResultConstant(0xC00D11A0)]
    public static HResult NS_E_WMP_WMDM_FAILURE => new(0xC00D11A0);

    /// <summary>A codec is required to play this file. To determine if this codec is available to download from the web, click Web Help.</summary>
    [Description("""A codec is required to play this file. To determine if this codec is available to download from the web, click Web Help.""")]
    [HResultConstant(0xC00D11A1)]
    public static HResult NS_E_WMP_CODEC_NEEDED_WITH_4CC => new(0xC00D11A1);

    /// <summary>An audio codec is needed to play this file. To determine if this codec is available to download from the web, click Web Help.</summary>
    [Description("""An audio codec is needed to play this file. To determine if this codec is available to download from the web, click Web Help.""")]
    [HResultConstant(0xC00D11A2)]
    public static HResult NS_E_WMP_CODEC_NEEDED_WITH_FORMATTAG => new(0xC00D11A2);

    /// <summary>To play the file, you must install the latest Windows service pack. To install the service pack from the Windows Update website, click Web Help.</summary>
    [Description("""To play the file, you must install the latest Windows service pack. To install the service pack from the Windows Update website, click Web Help.""")]
    [HResultConstant(0xC00D11A3)]
    public static HResult NS_E_WMP_MSSAP_NOT_AVAILABLE => new(0xC00D11A3);

    /// <summary>Windows Media Player no longer detects a portable device. Reconnect your portable device, and then try again.</summary>
    [Description("""Windows Media Player no longer detects a portable device. Reconnect your portable device, and then try again.""")]
    [HResultConstant(0xC00D11A4)]
    public static HResult NS_E_WMP_WMDM_INTERFACEDEAD => new(0xC00D11A4);

    /// <summary>Windows Media Player cannot sync the file because the portable device does not support protected files.</summary>
    [Description("""Windows Media Player cannot sync the file because the portable device does not support protected files.""")]
    [HResultConstant(0xC00D11A5)]
    public static HResult NS_E_WMP_WMDM_NOTCERTIFIED => new(0xC00D11A5);

    /// <summary>This file does not have sync rights. If you obtained this file from an online store, go to the online store to get sync rights.</summary>
    [Description("""This file does not have sync rights. If you obtained this file from an online store, go to the online store to get sync rights.""")]
    [HResultConstant(0xC00D11A6)]
    public static HResult NS_E_WMP_WMDM_LICENSE_NOTEXIST => new(0xC00D11A6);

    /// <summary>Windows Media Player cannot sync the file because the sync rights have expired. Go to the content provider's online store to get new sync rights.</summary>
    [Description("""Windows Media Player cannot sync the file because the sync rights have expired. Go to the content provider's online store to get new sync rights.""")]
    [HResultConstant(0xC00D11A7)]
    public static HResult NS_E_WMP_WMDM_LICENSE_EXPIRED => new(0xC00D11A7);

    /// <summary>The portable device is already in use. Wait until the current task finishes or quit other programs that might be using the portable device, and then try again.</summary>
    [Description("""The portable device is already in use. Wait until the current task finishes or quit other programs that might be using the portable device, and then try again.""")]
    [HResultConstant(0xC00D11A8)]
    public static HResult NS_E_WMP_WMDM_BUSY => new(0xC00D11A8);

    /// <summary>Windows Media Player cannot sync the file because the content provider or device prohibits it. You might be able to resolve this problem by going to the content provider's online store to get sync rights.</summary>
    [Description("""Windows Media Player cannot sync the file because the content provider or device prohibits it. You might be able to resolve this problem by going to the content provider's online store to get sync rights.""")]
    [HResultConstant(0xC00D11A9)]
    public static HResult NS_E_WMP_WMDM_NORIGHTS => new(0xC00D11A9);

    /// <summary>The content provider has not granted you the right to sync this file. Go to the content provider's online store to get sync rights.</summary>
    [Description("""The content provider has not granted you the right to sync this file. Go to the content provider's online store to get sync rights.""")]
    [HResultConstant(0xC00D11AA)]
    public static HResult NS_E_WMP_WMDM_INCORRECT_RIGHTS => new(0xC00D11AA);

    /// <summary>Windows Media Player cannot burn the files to the CD. Verify that the disc is clean and not damaged. If necessary, select a slower recording speed or try a different brand of blank discs.</summary>
    [Description("""Windows Media Player cannot burn the files to the CD. Verify that the disc is clean and not damaged. If necessary, select a slower recording speed or try a different brand of blank discs.""")]
    [HResultConstant(0xC00D11AB)]
    public static HResult NS_E_WMP_IMAPI_GENERIC => new(0xC00D11AB);

    /// <summary>Windows Media Player cannot burn the files. Verify that the burner is connected properly, and then try again.</summary>
    [Description("""Windows Media Player cannot burn the files. Verify that the burner is connected properly, and then try again.""")]
    [HResultConstant(0xC00D11AD)]
    public static HResult NS_E_WMP_IMAPI_DEVICE_NOTPRESENT => new(0xC00D11AD);

    /// <summary>Windows Media Player cannot burn the files. Verify that the burner is connected properly and that the disc is clean and not damaged. If the burner is already in use, wait until the current task finishes or quit other programs that might be using the burner.</summary>
    [Description("""Windows Media Player cannot burn the files. Verify that the burner is connected properly and that the disc is clean and not damaged. If the burner is already in use, wait until the current task finishes or quit other programs that might be using the burner.""")]
    [HResultConstant(0xC00D11AE)]
    public static HResult NS_E_WMP_IMAPI_DEVICE_BUSY => new(0xC00D11AE);

    /// <summary>Windows Media Player cannot burn the files to the CD.</summary>
    [Description("""Windows Media Player cannot burn the files to the CD.""")]
    [HResultConstant(0xC00D11AF)]
    public static HResult NS_E_WMP_IMAPI_LOSS_OF_STREAMING => new(0xC00D11AF);

    /// <summary>Windows Media Player cannot play the file. The server might not be available or there might be a problem with your network or firewall settings.</summary>
    [Description("""Windows Media Player cannot play the file. The server might not be available or there might be a problem with your network or firewall settings.""")]
    [HResultConstant(0xC00D11B0)]
    public static HResult NS_E_WMP_SERVER_UNAVAILABLE => new(0xC00D11B0);

    /// <summary>Windows Media Player encountered a problem while playing the file. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player encountered a problem while playing the file. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D11B1)]
    public static HResult NS_E_WMP_FILE_OPEN_FAILED => new(0xC00D11B1);

    /// <summary>Windows Media Player must connect to the Internet to verify the file's media usage rights. Connect to the Internet, and then try again.</summary>
    [Description("""Windows Media Player must connect to the Internet to verify the file's media usage rights. Connect to the Internet, and then try again.""")]
    [HResultConstant(0xC00D11B2)]
    public static HResult NS_E_WMP_VERIFY_ONLINE => new(0xC00D11B2);

    /// <summary>Windows Media Player cannot play the file because a network error occurred. The server might not be available. Verify that you are connected to the network and that your proxy settings are correct.</summary>
    [Description("""Windows Media Player cannot play the file because a network error occurred. The server might not be available. Verify that you are connected to the network and that your proxy settings are correct.""")]
    [HResultConstant(0xC00D11B3)]
    public static HResult NS_E_WMP_SERVER_NOT_RESPONDING => new(0xC00D11B3);

    /// <summary>Windows Media Player cannot restore your media usage rights because it could not find any backed up rights on your computer.</summary>
    [Description("""Windows Media Player cannot restore your media usage rights because it could not find any backed up rights on your computer.""")]
    [HResultConstant(0xC00D11B4)]
    public static HResult NS_E_WMP_DRM_CORRUPT_BACKUP => new(0xC00D11B4);

    /// <summary>Windows Media Player cannot download media usage rights because the server is not available (for example, the server might be busy or not online).</summary>
    [Description("""Windows Media Player cannot download media usage rights because the server is not available (for example, the server might be busy or not online).""")]
    [HResultConstant(0xC00D11B5)]
    public static HResult NS_E_WMP_DRM_LICENSE_SERVER_UNAVAILABLE => new(0xC00D11B5);

    /// <summary>Windows Media Player cannot play the file. A network firewall might be preventing the Player from opening the file by using the UDP transport protocol. If you typed a URL in the Open URL dialog box, try using a different transport protocol (for example, "http:").</summary>
    [Description("""Windows Media Player cannot play the file. A network firewall might be preventing the Player from opening the file by using the UDP transport protocol. If you typed a URL in the Open URL dialog box, try using a different transport protocol (for example, "http:").""")]
    [HResultConstant(0xC00D11B6)]
    public static HResult NS_E_WMP_NETWORK_FIREWALL => new(0xC00D11B6);

    /// <summary>Insert the removable media, and then try again.</summary>
    [Description("""Insert the removable media, and then try again.""")]
    [HResultConstant(0xC00D11B7)]
    public static HResult NS_E_WMP_NO_REMOVABLE_MEDIA => new(0xC00D11B7);

    /// <summary>Windows Media Player cannot play the file because the proxy server is not responding. The proxy server might be temporarily unavailable or your Player proxy settings might not be valid.</summary>
    [Description("""Windows Media Player cannot play the file because the proxy server is not responding. The proxy server might be temporarily unavailable or your Player proxy settings might not be valid.""")]
    [HResultConstant(0xC00D11B8)]
    public static HResult NS_E_WMP_PROXY_CONNECT_TIMEOUT => new(0xC00D11B8);

    /// <summary>To play the file, you might need to install a later version of Windows Media Player. On the Help menu, click Check for Updates, and then follow the instructions. For additional assistance, click Web Help.</summary>
    [Description("""To play the file, you might need to install a later version of Windows Media Player. On the Help menu, click Check for Updates, and then follow the instructions. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D11B9)]
    public static HResult NS_E_WMP_NEED_UPGRADE => new(0xC00D11B9);

    /// <summary>Windows Media Player cannot play the file because there is a problem with your sound device. There might not be a sound device installed on your computer, it might be in use by another program, or it might not be functioning properly.</summary>
    [Description("""Windows Media Player cannot play the file because there is a problem with your sound device. There might not be a sound device installed on your computer, it might be in use by another program, or it might not be functioning properly.""")]
    [HResultConstant(0xC00D11BA)]
    public static HResult NS_E_WMP_AUDIO_HW_PROBLEM => new(0xC00D11BA);

    /// <summary>Windows Media Player cannot play the file because the specified protocol is not supported. If you typed a URL in the Open URL dialog box, try using a different transport protocol (for example, "http:" or "rtsp:").</summary>
    [Description("""Windows Media Player cannot play the file because the specified protocol is not supported. If you typed a URL in the Open URL dialog box, try using a different transport protocol (for example, "http:" or "rtsp:").""")]
    [HResultConstant(0xC00D11BB)]
    public static HResult NS_E_WMP_INVALID_PROTOCOL => new(0xC00D11BB);

    /// <summary>Windows Media Player cannot add the file to the library because the file format is not supported.</summary>
    [Description("""Windows Media Player cannot add the file to the library because the file format is not supported.""")]
    [HResultConstant(0xC00D11BC)]
    public static HResult NS_E_WMP_INVALID_LIBRARY_ADD => new(0xC00D11BC);

    /// <summary>Windows Media Player cannot play the file because the specified protocol is not supported. If you typed a URL in the Open URL dialog box, try using a different transport protocol (for example, "mms:").</summary>
    [Description("""Windows Media Player cannot play the file because the specified protocol is not supported. If you typed a URL in the Open URL dialog box, try using a different transport protocol (for example, "mms:").""")]
    [HResultConstant(0xC00D11BD)]
    public static HResult NS_E_WMP_MMS_NOT_SUPPORTED => new(0xC00D11BD);

    /// <summary>Windows Media Player cannot play the file because there are no streaming protocols selected. Select one or more protocols, and then try again.</summary>
    [Description("""Windows Media Player cannot play the file because there are no streaming protocols selected. Select one or more protocols, and then try again.""")]
    [HResultConstant(0xC00D11BE)]
    public static HResult NS_E_WMP_NO_PROTOCOLS_SELECTED => new(0xC00D11BE);

    /// <summary>Windows Media Player cannot switch to Full Screen. You might need to adjust your Windows display settings. Open display settings in Control Panel, and then try setting Hardware acceleration to Full.</summary>
    [Description("""Windows Media Player cannot switch to Full Screen. You might need to adjust your Windows display settings. Open display settings in Control Panel, and then try setting Hardware acceleration to Full.""")]
    [HResultConstant(0xC00D11BF)]
    public static HResult NS_E_WMP_GOFULLSCREEN_FAILED => new(0xC00D11BF);

    /// <summary>Windows Media Player cannot play the file because a network error occurred. The server might not be available (for example, the server is busy or not online) or you might not be connected to the network.</summary>
    [Description("""Windows Media Player cannot play the file because a network error occurred. The server might not be available (for example, the server is busy or not online) or you might not be connected to the network.""")]
    [HResultConstant(0xC00D11C0)]
    public static HResult NS_E_WMP_NETWORK_ERROR => new(0xC00D11C0);

    /// <summary>Windows Media Player cannot play the file because the server is not responding. Verify that you are connected to the network, and then try again later.</summary>
    [Description("""Windows Media Player cannot play the file because the server is not responding. Verify that you are connected to the network, and then try again later.""")]
    [HResultConstant(0xC00D11C1)]
    public static HResult NS_E_WMP_CONNECT_TIMEOUT => new(0xC00D11C1);

    /// <summary>Windows Media Player cannot play the file because the multicast protocol is not enabled. On the Tools menu, click Options, click the Network tab, and then select the Multicast check box. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player cannot play the file because the multicast protocol is not enabled. On the Tools menu, click Options, click the Network tab, and then select the Multicast check box. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D11C2)]
    public static HResult NS_E_WMP_MULTICAST_DISABLED => new(0xC00D11C2);

    /// <summary>Windows Media Player cannot play the file because a network problem occurred. Verify that you are connected to the network, and then try again later.</summary>
    [Description("""Windows Media Player cannot play the file because a network problem occurred. Verify that you are connected to the network, and then try again later.""")]
    [HResultConstant(0xC00D11C3)]
    public static HResult NS_E_WMP_SERVER_DNS_TIMEOUT => new(0xC00D11C3);

    /// <summary>Windows Media Player cannot play the file because the network proxy server cannot be found. Verify that your proxy settings are correct, and then try again.</summary>
    [Description("""Windows Media Player cannot play the file because the network proxy server cannot be found. Verify that your proxy settings are correct, and then try again.""")]
    [HResultConstant(0xC00D11C4)]
    public static HResult NS_E_WMP_PROXY_NOT_FOUND => new(0xC00D11C4);

    /// <summary>Windows Media Player cannot play the file because it is corrupted.</summary>
    [Description("""Windows Media Player cannot play the file because it is corrupted.""")]
    [HResultConstant(0xC00D11C5)]
    public static HResult NS_E_WMP_TAMPERED_CONTENT => new(0xC00D11C5);

    /// <summary>Your computer is running low on memory. Quit other programs, and then try again.</summary>
    [Description("""Your computer is running low on memory. Quit other programs, and then try again.""")]
    [HResultConstant(0xC00D11C6)]
    public static HResult NS_E_WMP_OUTOFMEMORY => new(0xC00D11C6);

    /// <summary>Windows Media Player cannot play, burn, rip, or sync the file because a required audio codec is not installed on your computer.</summary>
    [Description("""Windows Media Player cannot play, burn, rip, or sync the file because a required audio codec is not installed on your computer.""")]
    [HResultConstant(0xC00D11C7)]
    public static HResult NS_E_WMP_AUDIO_CODEC_NOT_INSTALLED => new(0xC00D11C7);

    /// <summary>Windows Media Player cannot play the file because the required video codec is not installed on your computer.</summary>
    [Description("""Windows Media Player cannot play the file because the required video codec is not installed on your computer.""")]
    [HResultConstant(0xC00D11C8)]
    public static HResult NS_E_WMP_VIDEO_CODEC_NOT_INSTALLED => new(0xC00D11C8);

    /// <summary>Windows Media Player cannot burn the files. If the burner is busy, wait for the current task to finish. If necessary, verify that the burner is connected properly and that you have installed the latest device driver.</summary>
    [Description("""Windows Media Player cannot burn the files. If the burner is busy, wait for the current task to finish. If necessary, verify that the burner is connected properly and that you have installed the latest device driver.""")]
    [HResultConstant(0xC00D11C9)]
    public static HResult NS_E_WMP_IMAPI_DEVICE_INVALIDTYPE => new(0xC00D11C9);

    /// <summary>Windows Media Player cannot play the protected file because there is a problem with your sound device. Try installing a new device driver or use a different sound device.</summary>
    [Description("""Windows Media Player cannot play the protected file because there is a problem with your sound device. Try installing a new device driver or use a different sound device.""")]
    [HResultConstant(0xC00D11CA)]
    public static HResult NS_E_WMP_DRM_DRIVER_AUTH_FAILURE => new(0xC00D11CA);

    /// <summary>Windows Media Player encountered a network error. Restart the Player.</summary>
    [Description("""Windows Media Player encountered a network error. Restart the Player.""")]
    [HResultConstant(0xC00D11CB)]
    public static HResult NS_E_WMP_NETWORK_RESOURCE_FAILURE => new(0xC00D11CB);

    /// <summary>Windows Media Player is not installed properly. Reinstall the Player.</summary>
    [Description("""Windows Media Player is not installed properly. Reinstall the Player.""")]
    [HResultConstant(0xC00D11CC)]
    public static HResult NS_E_WMP_UPGRADE_APPLICATION => new(0xC00D11CC);

    /// <summary>Windows Media Player encountered an unknown error. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player encountered an unknown error. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D11CD)]
    public static HResult NS_E_WMP_UNKNOWN_ERROR => new(0xC00D11CD);

    /// <summary>Windows Media Player cannot play the file because the required codec is not valid.</summary>
    [Description("""Windows Media Player cannot play the file because the required codec is not valid.""")]
    [HResultConstant(0xC00D11CE)]
    public static HResult NS_E_WMP_INVALID_KEY => new(0xC00D11CE);

    /// <summary>The CD drive is in use by another user. Wait for the task to complete, and then try again.</summary>
    [Description("""The CD drive is in use by another user. Wait for the task to complete, and then try again.""")]
    [HResultConstant(0xC00D11CF)]
    public static HResult NS_E_WMP_CD_ANOTHER_USER => new(0xC00D11CF);

    /// <summary>Windows Media Player cannot play, sync, or burn the protected file because a problem occurred with the Windows Media Digital Rights Management (DRM) system. You might need to connect to the Internet to update your DRM components. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player cannot play, sync, or burn the protected file because a problem occurred with the Windows Media Digital Rights Management (DRM) system. You might need to connect to the Internet to update your DRM components. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D11D0)]
    public static HResult NS_E_WMP_DRM_NEEDS_AUTHORIZATION => new(0xC00D11D0);

    /// <summary>Windows Media Player cannot play the file because there might be a problem with your sound or video device. Try installing an updated device driver.</summary>
    [Description("""Windows Media Player cannot play the file because there might be a problem with your sound or video device. Try installing an updated device driver.""")]
    [HResultConstant(0xC00D11D1)]
    public static HResult NS_E_WMP_BAD_DRIVER => new(0xC00D11D1);

    /// <summary>Windows Media Player cannot access the file. The file might be in use, you might not have access to the computer where the file is stored, or your proxy settings might not be correct.</summary>
    [Description("""Windows Media Player cannot access the file. The file might be in use, you might not have access to the computer where the file is stored, or your proxy settings might not be correct.""")]
    [HResultConstant(0xC00D11D2)]
    public static HResult NS_E_WMP_ACCESS_DENIED => new(0xC00D11D2);

    /// <summary>The content provider prohibits this action. Go to the content provider's online store to get new media usage rights.</summary>
    [Description("""The content provider prohibits this action. Go to the content provider's online store to get new media usage rights.""")]
    [HResultConstant(0xC00D11D3)]
    public static HResult NS_E_WMP_LICENSE_RESTRICTS => new(0xC00D11D3);

    /// <summary>Windows Media Player cannot perform the requested action at this time.</summary>
    [Description("""Windows Media Player cannot perform the requested action at this time.""")]
    [HResultConstant(0xC00D11D4)]
    public static HResult NS_E_WMP_INVALID_REQUEST => new(0xC00D11D4);

    /// <summary>Windows Media Player cannot burn the files because there is not enough free disk space to store the temporary files. Delete some unneeded files on your hard disk, and then try again.</summary>
    [Description("""Windows Media Player cannot burn the files because there is not enough free disk space to store the temporary files. Delete some unneeded files on your hard disk, and then try again.""")]
    [HResultConstant(0xC00D11D5)]
    public static HResult NS_E_WMP_CD_STASH_NO_SPACE => new(0xC00D11D5);

    /// <summary>Your media usage rights have become corrupted or are no longer valid. This might happen if you have replaced hardware components in your computer.</summary>
    [Description("""Your media usage rights have become corrupted or are no longer valid. This might happen if you have replaced hardware components in your computer.""")]
    [HResultConstant(0xC00D11D6)]
    public static HResult NS_E_WMP_DRM_NEW_HARDWARE => new(0xC00D11D6);

    /// <summary>The required Windows Media Digital Rights Management (DRM) component cannot be validated. You might be able resolve the problem by reinstalling the Player.</summary>
    [Description("""The required Windows Media Digital Rights Management (DRM) component cannot be validated. You might be able resolve the problem by reinstalling the Player.""")]
    [HResultConstant(0xC00D11D7)]
    public static HResult NS_E_WMP_DRM_INVALID_SIG => new(0xC00D11D7);

    /// <summary>You have exceeded your restore limit for the day. Try restoring your media usage rights tomorrow.</summary>
    [Description("""You have exceeded your restore limit for the day. Try restoring your media usage rights tomorrow.""")]
    [HResultConstant(0xC00D11D8)]
    public static HResult NS_E_WMP_DRM_CANNOT_RESTORE => new(0xC00D11D8);

    /// <summary>Some files might not fit on the CD. The required space cannot be calculated accurately because some files might be missing duration information. To ensure the calculation is accurate, play the files that are missing duration information.</summary>
    [Description("""Some files might not fit on the CD. The required space cannot be calculated accurately because some files might be missing duration information. To ensure the calculation is accurate, play the files that are missing duration information.""")]
    [HResultConstant(0xC00D11D9)]
    public static HResult NS_E_WMP_BURN_DISC_OVERFLOW => new(0xC00D11D9);

    /// <summary>Windows Media Player cannot verify the file's media usage rights. If you obtained this file from an online store, go to the online store to get the necessary rights.</summary>
    [Description("""Windows Media Player cannot verify the file's media usage rights. If you obtained this file from an online store, go to the online store to get the necessary rights.""")]
    [HResultConstant(0xC00D11DA)]
    public static HResult NS_E_WMP_DRM_GENERIC_LICENSE_FAILURE => new(0xC00D11DA);

    /// <summary>It is not possible to sync because this device's internal clock is not set correctly. To set the clock, select the option to set the device clock on the Privacy tab of the Options dialog box, connect to the Internet, and then sync the device again. For additional assistance, click Web Help.</summary>
    [Description("""It is not possible to sync because this device's internal clock is not set correctly. To set the clock, select the option to set the device clock on the Privacy tab of the Options dialog box, connect to the Internet, and then sync the device again. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D11DB)]
    public static HResult NS_E_WMP_DRM_NO_SECURE_CLOCK => new(0xC00D11DB);

    /// <summary>Windows Media Player cannot play, burn, rip, or sync the protected file because you do not have the appropriate rights.</summary>
    [Description("""Windows Media Player cannot play, burn, rip, or sync the protected file because you do not have the appropriate rights.""")]
    [HResultConstant(0xC00D11DC)]
    public static HResult NS_E_WMP_DRM_NO_RIGHTS => new(0xC00D11DC);

    /// <summary>Windows Media Player encountered an error during upgrade.</summary>
    [Description("""Windows Media Player encountered an error during upgrade.""")]
    [HResultConstant(0xC00D11DD)]
    public static HResult NS_E_WMP_DRM_INDIV_FAILED => new(0xC00D11DD);

    /// <summary>Windows Media Player cannot connect to the server because it is not accepting any new connections. This could be because it has reached its maximum connection limit. Please try again later.</summary>
    [Description("""Windows Media Player cannot connect to the server because it is not accepting any new connections. This could be because it has reached its maximum connection limit. Please try again later.""")]
    [HResultConstant(0xC00D11DE)]
    public static HResult NS_E_WMP_SERVER_NONEWCONNECTIONS => new(0xC00D11DE);

    /// <summary>A number of queued files cannot be played. To find information about the problem, click the Now Playing tab, and then click the icon next to each file in the List pane.</summary>
    [Description("""A number of queued files cannot be played. To find information about the problem, click the Now Playing tab, and then click the icon next to each file in the List pane.""")]
    [HResultConstant(0xC00D11DF)]
    public static HResult NS_E_WMP_MULTIPLE_ERROR_IN_PLAYLIST => new(0xC00D11DF);

    /// <summary>Windows Media Player encountered an error while erasing the rewritable CD or DVD. Verify that the CD or DVD burner is connected properly and that the disc is clean and not damaged.</summary>
    [Description("""Windows Media Player encountered an error while erasing the rewritable CD or DVD. Verify that the CD or DVD burner is connected properly and that the disc is clean and not damaged.""")]
    [HResultConstant(0xC00D11E0)]
    public static HResult NS_E_WMP_IMAPI2_ERASE_FAIL => new(0xC00D11E0);

    /// <summary>Windows Media Player cannot erase the rewritable CD or DVD. Verify that the CD or DVD burner is connected properly and that the disc is clean and not damaged. If the burner is already in use, wait until the current task finishes or quit other programs that might be using the burner.</summary>
    [Description("""Windows Media Player cannot erase the rewritable CD or DVD. Verify that the CD or DVD burner is connected properly and that the disc is clean and not damaged. If the burner is already in use, wait until the current task finishes or quit other programs that might be using the burner.""")]
    [HResultConstant(0xC00D11E1)]
    public static HResult NS_E_WMP_IMAPI2_ERASE_DEVICE_BUSY => new(0xC00D11E1);

    /// <summary>A Windows Media Digital Rights Management (DRM) component encountered a problem. If you are trying to use a file that you obtained from an online store, try going to the online store and getting the appropriate usage rights.</summary>
    [Description("""A Windows Media Digital Rights Management (DRM) component encountered a problem. If you are trying to use a file that you obtained from an online store, try going to the online store and getting the appropriate usage rights.""")]
    [HResultConstant(0xC00D11E2)]
    public static HResult NS_E_WMP_DRM_COMPONENT_FAILURE => new(0xC00D11E2);

    /// <summary>It is not possible to obtain device's certificate. Please contact the device manufacturer for a firmware update or for other steps to resolve this problem.</summary>
    [Description("""It is not possible to obtain device's certificate. Please contact the device manufacturer for a firmware update or for other steps to resolve this problem.""")]
    [HResultConstant(0xC00D11E3)]
    public static HResult NS_E_WMP_DRM_NO_DEVICE_CERT => new(0xC00D11E3);

    /// <summary>Windows Media Player encountered an error when connecting to the server. The security information from the server could not be validated.</summary>
    [Description("""Windows Media Player encountered an error when connecting to the server. The security information from the server could not be validated.""")]
    [HResultConstant(0xC00D11E4)]
    public static HResult NS_E_WMP_SERVER_SECURITY_ERROR => new(0xC00D11E4);

    /// <summary>An audio device was disconnected or reconfigured. Verify that the audio device is connected, and then try to play the item again.</summary>
    [Description("""An audio device was disconnected or reconfigured. Verify that the audio device is connected, and then try to play the item again.""")]
    [HResultConstant(0xC00D11E5)]
    public static HResult NS_E_WMP_AUDIO_DEVICE_LOST => new(0xC00D11E5);

    /// <summary>Windows Media Player could not complete burning because the disc is not compatible with your drive. Try inserting a different kind of recordable media or use a disc that supports a write speed that is compatible with your drive.</summary>
    [Description("""Windows Media Player could not complete burning because the disc is not compatible with your drive. Try inserting a different kind of recordable media or use a disc that supports a write speed that is compatible with your drive.""")]
    [HResultConstant(0xC00D11E6)]
    public static HResult NS_E_WMP_IMAPI_MEDIA_INCOMPATIBLE => new(0xC00D11E6);

    /// <summary>Windows Media Player cannot save the sync settings because your device is full. Delete some unneeded files on your device and then try again.</summary>
    [Description("""Windows Media Player cannot save the sync settings because your device is full. Delete some unneeded files on your device and then try again.""")]
    [HResultConstant(0xC00D11EE)]
    public static HResult NS_E_SYNCWIZ_DEVICE_FULL => new(0xC00D11EE);

    /// <summary>It is not possible to change sync settings at this time. Try again later.</summary>
    [Description("""It is not possible to change sync settings at this time. Try again later.""")]
    [HResultConstant(0xC00D11EF)]
    public static HResult NS_E_SYNCWIZ_CANNOT_CHANGE_SETTINGS => new(0xC00D11EF);

    /// <summary>Windows Media Player cannot delete these files currently. If the Player is synchronizing, wait until it is complete and then try again.</summary>
    [Description("""Windows Media Player cannot delete these files currently. If the Player is synchronizing, wait until it is complete and then try again.""")]
    [HResultConstant(0xC00D11F0)]
    public static HResult NS_E_TRANSCODE_DELETECACHEERROR => new(0xC00D11F0);

    /// <summary>Windows Media Player could not use digital mode to read the CD. The Player has automatically switched the CD drive to analog mode. To switch back to digital mode, use the Devices tab. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player could not use digital mode to read the CD. The Player has automatically switched the CD drive to analog mode. To switch back to digital mode, use the Devices tab. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D11F8)]
    public static HResult NS_E_CD_NO_BUFFERS_READ => new(0xC00D11F8);

    /// <summary>No CD track was specified for playback.</summary>
    [Description("""No CD track was specified for playback.""")]
    [HResultConstant(0xC00D11F9)]
    public static HResult NS_E_CD_EMPTY_TRACK_QUEUE => new(0xC00D11F9);

    /// <summary>The CD filter was not able to create the CD reader.</summary>
    [Description("""The CD filter was not able to create the CD reader.""")]
    [HResultConstant(0xC00D11FA)]
    public static HResult NS_E_CD_NO_READER => new(0xC00D11FA);

    /// <summary>Invalid ISRC code.</summary>
    [Description("""Invalid ISRC code.""")]
    [HResultConstant(0xC00D11FB)]
    public static HResult NS_E_CD_ISRC_INVALID => new(0xC00D11FB);

    /// <summary>Invalid Media Catalog Number.</summary>
    [Description("""Invalid Media Catalog Number.""")]
    [HResultConstant(0xC00D11FC)]
    public static HResult NS_E_CD_MEDIA_CATALOG_NUMBER_INVALID => new(0xC00D11FC);

    /// <summary>Windows Media Player cannot play audio CDs correctly because the CD drive is slow and error correction is turned on. To increase performance, turn off playback error correction for this drive.</summary>
    [Description("""Windows Media Player cannot play audio CDs correctly because the CD drive is slow and error correction is turned on. To increase performance, turn off playback error correction for this drive.""")]
    [HResultConstant(0xC00D11FD)]
    public static HResult NS_E_SLOW_READ_DIGITAL_WITH_ERRORCORRECTION => new(0xC00D11FD);

    /// <summary>Windows Media Player cannot estimate the CD drive's playback speed because the CD track is too short.</summary>
    [Description("""Windows Media Player cannot estimate the CD drive's playback speed because the CD track is too short.""")]
    [HResultConstant(0xC00D11FE)]
    public static HResult NS_E_CD_SPEEDDETECT_NOT_ENOUGH_READS => new(0xC00D11FE);

    /// <summary>Cannot queue the CD track because queuing is not enabled.</summary>
    [Description("""Cannot queue the CD track because queuing is not enabled.""")]
    [HResultConstant(0xC00D11FF)]
    public static HResult NS_E_CD_QUEUEING_DISABLED => new(0xC00D11FF);

    /// <summary>Windows Media Player cannot download additional media usage rights until the current download is complete.</summary>
    [Description("""Windows Media Player cannot download additional media usage rights until the current download is complete.""")]
    [HResultConstant(0xC00D1202)]
    public static HResult NS_E_WMP_DRM_ACQUIRING_LICENSE => new(0xC00D1202);

    /// <summary>The media usage rights for this file have expired or are no longer valid. If you obtained the file from an online store, sign in to the store, and then try again.</summary>
    [Description("""The media usage rights for this file have expired or are no longer valid. If you obtained the file from an online store, sign in to the store, and then try again.""")]
    [HResultConstant(0xC00D1203)]
    public static HResult NS_E_WMP_DRM_LICENSE_EXPIRED => new(0xC00D1203);

    /// <summary>Windows Media Player cannot download the media usage rights for the file. If you obtained the file from an online store, sign in to the store, and then try again.</summary>
    [Description("""Windows Media Player cannot download the media usage rights for the file. If you obtained the file from an online store, sign in to the store, and then try again.""")]
    [HResultConstant(0xC00D1204)]
    public static HResult NS_E_WMP_DRM_LICENSE_NOTACQUIRED => new(0xC00D1204);

    /// <summary>The media usage rights for this file are not yet valid. To see when they will become valid, right-click the file in the library, click Properties, and then click the Media Usage Rights tab.</summary>
    [Description("""The media usage rights for this file are not yet valid. To see when they will become valid, right-click the file in the library, click Properties, and then click the Media Usage Rights tab.""")]
    [HResultConstant(0xC00D1205)]
    public static HResult NS_E_WMP_DRM_LICENSE_NOTENABLED => new(0xC00D1205);

    /// <summary>The media usage rights for this file are not valid. If you obtained this file from an online store, contact the store for assistance.</summary>
    [Description("""The media usage rights for this file are not valid. If you obtained this file from an online store, contact the store for assistance.""")]
    [HResultConstant(0xC00D1206)]
    public static HResult NS_E_WMP_DRM_LICENSE_UNUSABLE => new(0xC00D1206);

    /// <summary>The content provider has revoked the media usage rights for this file. If you obtained this file from an online store, ask the store if a new version of the file is available.</summary>
    [Description("""The content provider has revoked the media usage rights for this file. If you obtained this file from an online store, ask the store if a new version of the file is available.""")]
    [HResultConstant(0xC00D1207)]
    public static HResult NS_E_WMP_DRM_LICENSE_CONTENT_REVOKED => new(0xC00D1207);

    /// <summary>The media usage rights for this file require a feature that is not supported in your current version of Windows Media Player or your current version of Windows. Try installing the latest version of the Player. If you obtained this file from an online store, contact the store for further assistance.</summary>
    [Description("""The media usage rights for this file require a feature that is not supported in your current version of Windows Media Player or your current version of Windows. Try installing the latest version of the Player. If you obtained this file from an online store, contact the store for further assistance.""")]
    [HResultConstant(0xC00D1208)]
    public static HResult NS_E_WMP_DRM_LICENSE_NOSAP => new(0xC00D1208);

    /// <summary>Windows Media Player cannot download media usage rights at this time. Try again later.</summary>
    [Description("""Windows Media Player cannot download media usage rights at this time. Try again later.""")]
    [HResultConstant(0xC00D1209)]
    public static HResult NS_E_WMP_DRM_UNABLE_TO_ACQUIRE_LICENSE => new(0xC00D1209);

    /// <summary>Windows Media Player cannot play, burn, or sync the file because the media usage rights are missing. If you obtained the file from an online store, sign in to the store, and then try again.</summary>
    [Description("""Windows Media Player cannot play, burn, or sync the file because the media usage rights are missing. If you obtained the file from an online store, sign in to the store, and then try again.""")]
    [HResultConstant(0xC00D120A)]
    public static HResult NS_E_WMP_LICENSE_REQUIRED => new(0xC00D120A);

    /// <summary>Windows Media Player cannot play, burn, or sync the file because the media usage rights are missing. If you obtained the file from an online store, sign in to the store, and then try again.</summary>
    [Description("""Windows Media Player cannot play, burn, or sync the file because the media usage rights are missing. If you obtained the file from an online store, sign in to the store, and then try again.""")]
    [HResultConstant(0xC00D120B)]
    public static HResult NS_E_WMP_PROTECTED_CONTENT => new(0xC00D120B);

    /// <summary>Windows Media Player cannot read a policy. This can occur when the policy does not exist in the registry or when the registry cannot be read.</summary>
    [Description("""Windows Media Player cannot read a policy. This can occur when the policy does not exist in the registry or when the registry cannot be read.""")]
    [HResultConstant(0xC00D122A)]
    public static HResult NS_E_WMP_POLICY_VALUE_NOT_CONFIGURED => new(0xC00D122A);

    /// <summary>Windows Media Player cannot sync content streamed directly from the Internet. If possible, download the file to your computer, and then try to sync the file.</summary>
    [Description("""Windows Media Player cannot sync content streamed directly from the Internet. If possible, download the file to your computer, and then try to sync the file.""")]
    [HResultConstant(0xC00D1234)]
    public static HResult NS_E_PDA_CANNOT_SYNC_FROM_INTERNET => new(0xC00D1234);

    /// <summary>This playlist is not valid or is corrupted. Create a new playlist using Windows Media Player, then sync the new playlist instead.</summary>
    [Description("""This playlist is not valid or is corrupted. Create a new playlist using Windows Media Player, then sync the new playlist instead.""")]
    [HResultConstant(0xC00D1235)]
    public static HResult NS_E_PDA_CANNOT_SYNC_INVALID_PLAYLIST => new(0xC00D1235);

    /// <summary>Windows Media Player encountered a problem while synchronizing the file to the device. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player encountered a problem while synchronizing the file to the device. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D1236)]
    public static HResult NS_E_PDA_FAILED_TO_SYNCHRONIZE_FILE => new(0xC00D1236);

    /// <summary>Windows Media Player encountered an error while synchronizing to the device.</summary>
    [Description("""Windows Media Player encountered an error while synchronizing to the device.""")]
    [HResultConstant(0xC00D1237)]
    public static HResult NS_E_PDA_SYNC_FAILED => new(0xC00D1237);

    /// <summary>Windows Media Player cannot delete a file from the device.</summary>
    [Description("""Windows Media Player cannot delete a file from the device.""")]
    [HResultConstant(0xC00D1238)]
    public static HResult NS_E_PDA_DELETE_FAILED => new(0xC00D1238);

    /// <summary>Windows Media Player cannot copy a file from the device to your library.</summary>
    [Description("""Windows Media Player cannot copy a file from the device to your library.""")]
    [HResultConstant(0xC00D1239)]
    public static HResult NS_E_PDA_FAILED_TO_RETRIEVE_FILE => new(0xC00D1239);

    /// <summary>Windows Media Player cannot communicate with the device because the device is not responding. Try reconnecting the device, resetting the device, or contacting the device manufacturer for updated firmware.</summary>
    [Description("""Windows Media Player cannot communicate with the device because the device is not responding. Try reconnecting the device, resetting the device, or contacting the device manufacturer for updated firmware.""")]
    [HResultConstant(0xC00D123A)]
    public static HResult NS_E_PDA_DEVICE_NOT_RESPONDING => new(0xC00D123A);

    /// <summary>Windows Media Player cannot sync the picture to the device because a problem occurred while converting the file to another quality level or format. The original file might be damaged or corrupted.</summary>
    [Description("""Windows Media Player cannot sync the picture to the device because a problem occurred while converting the file to another quality level or format. The original file might be damaged or corrupted.""")]
    [HResultConstant(0xC00D123B)]
    public static HResult NS_E_PDA_FAILED_TO_TRANSCODE_PHOTO => new(0xC00D123B);

    /// <summary>Windows Media Player cannot convert the file. The file might have been encrypted by the Encrypted File System (EFS). Try decrypting the file first and then synchronizing it. For information about how to decrypt a file, see Windows Help and Support.</summary>
    [Description("""Windows Media Player cannot convert the file. The file might have been encrypted by the Encrypted File System (EFS). Try decrypting the file first and then synchronizing it. For information about how to decrypt a file, see Windows Help and Support.""")]
    [HResultConstant(0xC00D123C)]
    public static HResult NS_E_PDA_FAILED_TO_ENCRYPT_TRANSCODED_FILE => new(0xC00D123C);

    /// <summary>Your device requires that this file be converted in order to play on the device. However, the device either does not support playing audio, or Windows Media Player cannot convert the file to an audio format that is supported by the device.</summary>
    [Description("""Your device requires that this file be converted in order to play on the device. However, the device either does not support playing audio, or Windows Media Player cannot convert the file to an audio format that is supported by the device.""")]
    [HResultConstant(0xC00D123D)]
    public static HResult NS_E_PDA_CANNOT_TRANSCODE_TO_AUDIO => new(0xC00D123D);

    /// <summary>Your device requires that this file be converted in order to play on the device. However, the device either does not support playing video, or Windows Media Player cannot convert the file to a video format that is supported by the device.</summary>
    [Description("""Your device requires that this file be converted in order to play on the device. However, the device either does not support playing video, or Windows Media Player cannot convert the file to a video format that is supported by the device.""")]
    [HResultConstant(0xC00D123E)]
    public static HResult NS_E_PDA_CANNOT_TRANSCODE_TO_VIDEO => new(0xC00D123E);

    /// <summary>Your device requires that this file be converted in order to play on the device. However, the device either does not support displaying pictures, or Windows Media Player cannot convert the file to a picture format that is supported by the device.</summary>
    [Description("""Your device requires that this file be converted in order to play on the device. However, the device either does not support displaying pictures, or Windows Media Player cannot convert the file to a picture format that is supported by the device.""")]
    [HResultConstant(0xC00D123F)]
    public static HResult NS_E_PDA_CANNOT_TRANSCODE_TO_IMAGE => new(0xC00D123F);

    /// <summary>Windows Media Player cannot sync the file to your computer because the file name is too long. Try renaming the file on the device.</summary>
    [Description("""Windows Media Player cannot sync the file to your computer because the file name is too long. Try renaming the file on the device.""")]
    [HResultConstant(0xC00D1240)]
    public static HResult NS_E_PDA_RETRIEVED_FILE_FILENAME_TOO_LONG => new(0xC00D1240);

    /// <summary>Windows Media Player cannot sync the file because the device is not responding. This typically occurs when there is a problem with the device firmware. For additional assistance, click Web Help.</summary>
    [Description("""Windows Media Player cannot sync the file because the device is not responding. This typically occurs when there is a problem with the device firmware. For additional assistance, click Web Help.""")]
    [HResultConstant(0xC00D1241)]
    public static HResult NS_E_PDA_CEWMDM_DRM_ERROR => new(0xC00D1241);

    /// <summary>Incomplete playlist.</summary>
    [Description("""Incomplete playlist.""")]
    [HResultConstant(0xC00D1242)]
    public static HResult NS_E_INCOMPLETE_PLAYLIST => new(0xC00D1242);

    /// <summary>It is not possible to perform the requested action because sync is in progress. You can either stop sync or wait for it to complete, and then try again.</summary>
    [Description("""It is not possible to perform the requested action because sync is in progress. You can either stop sync or wait for it to complete, and then try again.""")]
    [HResultConstant(0xC00D1243)]
    public static HResult NS_E_PDA_SYNC_RUNNING => new(0xC00D1243);

    /// <summary>Windows Media Player cannot sync the subscription content because you are not signed in to the online store that provided it. Sign in to the online store, and then try again.</summary>
    [Description("""Windows Media Player cannot sync the subscription content because you are not signed in to the online store that provided it. Sign in to the online store, and then try again.""")]
    [HResultConstant(0xC00D1244)]
    public static HResult NS_E_PDA_SYNC_LOGIN_ERROR => new(0xC00D1244);

    /// <summary>Windows Media Player cannot convert the file to the format required by the device. One or more codecs required to convert the file could not be found.</summary>
    [Description("""Windows Media Player cannot convert the file to the format required by the device. One or more codecs required to convert the file could not be found.""")]
    [HResultConstant(0xC00D1245)]
    public static HResult NS_E_PDA_TRANSCODE_CODEC_NOT_FOUND => new(0xC00D1245);

    /// <summary>It is not possible to sync subscription files to this device.</summary>
    [Description("""It is not possible to sync subscription files to this device.""")]
    [HResultConstant(0xC00D1246)]
    public static HResult NS_E_CANNOT_SYNC_DRM_TO_NON_JANUS_DEVICE => new(0xC00D1246);

    /// <summary>Your device is operating slowly or is not responding. Until the device responds, it is not possible to sync again. To return the device to normal operation, try disconnecting it from the computer or resetting it.</summary>
    [Description("""Your device is operating slowly or is not responding. Until the device responds, it is not possible to sync again. To return the device to normal operation, try disconnecting it from the computer or resetting it.""")]
    [HResultConstant(0xC00D1247)]
    public static HResult NS_E_CANNOT_SYNC_PREVIOUS_SYNC_RUNNING => new(0xC00D1247);

    /// <summary>The Windows Media Player download manager cannot function properly because the Player main window cannot be found. Try restarting the Player.</summary>
    [Description("""The Windows Media Player download manager cannot function properly because the Player main window cannot be found. Try restarting the Player.""")]
    [HResultConstant(0xC00D125C)]
    public static HResult NS_E_WMP_HWND_NOTFOUND => new(0xC00D125C);

    /// <summary>Windows Media Player encountered a download that has the wrong number of files. This might occur if another program is trying to create jobs with the same signature as the Player.</summary>
    [Description("""Windows Media Player encountered a download that has the wrong number of files. This might occur if another program is trying to create jobs with the same signature as the Player.""")]
    [HResultConstant(0xC00D125D)]
    public static HResult NS_E_BKGDOWNLOAD_WRONG_NO_FILES => new(0xC00D125D);

    /// <summary>Windows Media Player tried to complete a download that was already canceled. The file will not be available.</summary>
    [Description("""Windows Media Player tried to complete a download that was already canceled. The file will not be available.""")]
    [HResultConstant(0xC00D125E)]
    public static HResult NS_E_BKGDOWNLOAD_COMPLETECANCELLEDJOB => new(0xC00D125E);

    /// <summary>Windows Media Player tried to cancel a download that was already completed. The file will not be removed.</summary>
    [Description("""Windows Media Player tried to cancel a download that was already completed. The file will not be removed.""")]
    [HResultConstant(0xC00D125F)]
    public static HResult NS_E_BKGDOWNLOAD_CANCELCOMPLETEDJOB => new(0xC00D125F);

    /// <summary>Windows Media Player is trying to access a download that is not valid.</summary>
    [Description("""Windows Media Player is trying to access a download that is not valid.""")]
    [HResultConstant(0xC00D1260)]
    public static HResult NS_E_BKGDOWNLOAD_NOJOBPOINTER => new(0xC00D1260);

    /// <summary>This download was not created by Windows Media Player.</summary>
    [Description("""This download was not created by Windows Media Player.""")]
    [HResultConstant(0xC00D1261)]
    public static HResult NS_E_BKGDOWNLOAD_INVALIDJOBSIGNATURE => new(0xC00D1261);

    /// <summary>The Windows Media Player download manager cannot create a temporary file name. This might occur if the path is not valid or if the disk is full.</summary>
    [Description("""The Windows Media Player download manager cannot create a temporary file name. This might occur if the path is not valid or if the disk is full.""")]
    [HResultConstant(0xC00D1262)]
    public static HResult NS_E_BKGDOWNLOAD_FAILED_TO_CREATE_TEMPFILE => new(0xC00D1262);

    /// <summary>The Windows Media Player download manager plug-in cannot start. This might occur if the system is out of resources.</summary>
    [Description("""The Windows Media Player download manager plug-in cannot start. This might occur if the system is out of resources.""")]
    [HResultConstant(0xC00D1263)]
    public static HResult NS_E_BKGDOWNLOAD_PLUGIN_FAILEDINITIALIZE => new(0xC00D1263);

    /// <summary>The Windows Media Player download manager cannot move the file.</summary>
    [Description("""The Windows Media Player download manager cannot move the file.""")]
    [HResultConstant(0xC00D1264)]
    public static HResult NS_E_BKGDOWNLOAD_PLUGIN_FAILEDTOMOVEFILE => new(0xC00D1264);

    /// <summary>The Windows Media Player download manager cannot perform a task because the system has no resources to allocate.</summary>
    [Description("""The Windows Media Player download manager cannot perform a task because the system has no resources to allocate.""")]
    [HResultConstant(0xC00D1265)]
    public static HResult NS_E_BKGDOWNLOAD_CALLFUNCFAILED => new(0xC00D1265);

    /// <summary>The Windows Media Player download manager cannot perform a task because the task took too long to run.</summary>
    [Description("""The Windows Media Player download manager cannot perform a task because the task took too long to run.""")]
    [HResultConstant(0xC00D1266)]
    public static HResult NS_E_BKGDOWNLOAD_CALLFUNCTIMEOUT => new(0xC00D1266);

    /// <summary>The Windows Media Player download manager cannot perform a task because the Player is terminating the service. The task will be recovered when the Player restarts.</summary>
    [Description("""The Windows Media Player download manager cannot perform a task because the Player is terminating the service. The task will be recovered when the Player restarts.""")]
    [HResultConstant(0xC00D1267)]
    public static HResult NS_E_BKGDOWNLOAD_CALLFUNCENDED => new(0xC00D1267);

    /// <summary>The Windows Media Player download manager cannot expand a WMD file. The file will be deleted and the operation will not be completed successfully.</summary>
    [Description("""The Windows Media Player download manager cannot expand a WMD file. The file will be deleted and the operation will not be completed successfully.""")]
    [HResultConstant(0xC00D1268)]
    public static HResult NS_E_BKGDOWNLOAD_WMDUNPACKFAILED => new(0xC00D1268);

    /// <summary>The Windows Media Player download manager cannot start. This might occur if the system is out of resources.</summary>
    [Description("""The Windows Media Player download manager cannot start. This might occur if the system is out of resources.""")]
    [HResultConstant(0xC00D1269)]
    public static HResult NS_E_BKGDOWNLOAD_FAILEDINITIALIZE => new(0xC00D1269);

    /// <summary>Windows Media Player cannot access a required functionality. This might occur if the wrong system files or Player DLLs are loaded.</summary>
    [Description("""Windows Media Player cannot access a required functionality. This might occur if the wrong system files or Player DLLs are loaded.""")]
    [HResultConstant(0xC00D126A)]
    public static HResult NS_E_INTERFACE_NOT_REGISTERED_IN_GIT => new(0xC00D126A);

    /// <summary>Windows Media Player cannot get the file name of the requested download. The requested download will be canceled.</summary>
    [Description("""Windows Media Player cannot get the file name of the requested download. The requested download will be canceled.""")]
    [HResultConstant(0xC00D126B)]
    public static HResult NS_E_BKGDOWNLOAD_INVALID_FILE_NAME => new(0xC00D126B);

    /// <summary>Windows Media Player encountered an error while downloading an image.</summary>
    [Description("""Windows Media Player encountered an error while downloading an image.""")]
    [HResultConstant(0xC00D128E)]
    public static HResult NS_E_IMAGE_DOWNLOAD_FAILED => new(0xC00D128E);

    /// <summary>Windows Media Player cannot update your media usage rights because the Player cannot verify the list of activated users of this computer.</summary>
    [Description("""Windows Media Player cannot update your media usage rights because the Player cannot verify the list of activated users of this computer.""")]
    [HResultConstant(0xC00D12C0)]
    public static HResult NS_E_WMP_UDRM_NOUSERLIST => new(0xC00D12C0);

    /// <summary>Windows Media Player is trying to acquire media usage rights for a file that is no longer being used. Rights acquisition will stop.</summary>
    [Description("""Windows Media Player is trying to acquire media usage rights for a file that is no longer being used. Rights acquisition will stop.""")]
    [HResultConstant(0xC00D12C1)]
    public static HResult NS_E_WMP_DRM_NOT_ACQUIRING => new(0xC00D12C1);

    /// <summary>The parameter is not valid.</summary>
    [Description("""The parameter is not valid.""")]
    [HResultConstant(0xC00D12F2)]
    public static HResult NS_E_WMP_BSTR_TOO_LONG => new(0xC00D12F2);

    /// <summary>The state is not valid for this request.</summary>
    [Description("""The state is not valid for this request.""")]
    [HResultConstant(0xC00D12FC)]
    public static HResult NS_E_WMP_AUTOPLAY_INVALID_STATE => new(0xC00D12FC);

    /// <summary>Windows Media Player cannot play this file until you complete the software component upgrade. After the component has been upgraded, try to play the file again.</summary>
    [Description("""Windows Media Player cannot play this file until you complete the software component upgrade. After the component has been upgraded, try to play the file again.""")]
    [HResultConstant(0xC00D1306)]
    public static HResult NS_E_WMP_COMPONENT_REVOKED => new(0xC00D1306);

    /// <summary>The URL is not safe for the operation specified.</summary>
    [Description("""The URL is not safe for the operation specified.""")]
    [HResultConstant(0xC00D1324)]
    public static HResult NS_E_CURL_NOTSAFE => new(0xC00D1324);

    /// <summary>The URL contains one or more characters that are not valid.</summary>
    [Description("""The URL contains one or more characters that are not valid.""")]
    [HResultConstant(0xC00D1325)]
    public static HResult NS_E_CURL_INVALIDCHAR => new(0xC00D1325);

    /// <summary>The URL contains a host name that is not valid.</summary>
    [Description("""The URL contains a host name that is not valid.""")]
    [HResultConstant(0xC00D1326)]
    public static HResult NS_E_CURL_INVALIDHOSTNAME => new(0xC00D1326);

    /// <summary>The URL contains a path that is not valid.</summary>
    [Description("""The URL contains a path that is not valid.""")]
    [HResultConstant(0xC00D1327)]
    public static HResult NS_E_CURL_INVALIDPATH => new(0xC00D1327);

    /// <summary>The URL contains a scheme that is not valid.</summary>
    [Description("""The URL contains a scheme that is not valid.""")]
    [HResultConstant(0xC00D1328)]
    public static HResult NS_E_CURL_INVALIDSCHEME => new(0xC00D1328);

    /// <summary>The URL is not valid.</summary>
    [Description("""The URL is not valid.""")]
    [HResultConstant(0xC00D1329)]
    public static HResult NS_E_CURL_INVALIDURL => new(0xC00D1329);

    /// <summary>Windows Media Player cannot play the file. If you clicked a link on a web page, the link might not be valid.</summary>
    [Description("""Windows Media Player cannot play the file. If you clicked a link on a web page, the link might not be valid.""")]
    [HResultConstant(0xC00D132B)]
    public static HResult NS_E_CURL_CANTWALK => new(0xC00D132B);

    /// <summary>The URL port is not valid.</summary>
    [Description("""The URL port is not valid.""")]
    [HResultConstant(0xC00D132C)]
    public static HResult NS_E_CURL_INVALIDPORT => new(0xC00D132C);

    /// <summary>The URL is not a directory.</summary>
    [Description("""The URL is not a directory.""")]
    [HResultConstant(0xC00D132D)]
    public static HResult NS_E_CURLHELPER_NOTADIRECTORY => new(0xC00D132D);

    /// <summary>The URL is not a file.</summary>
    [Description("""The URL is not a file.""")]
    [HResultConstant(0xC00D132E)]
    public static HResult NS_E_CURLHELPER_NOTAFILE => new(0xC00D132E);

    /// <summary>The URL contains characters that cannot be decoded. The URL might be truncated or incomplete.</summary>
    [Description("""The URL contains characters that cannot be decoded. The URL might be truncated or incomplete.""")]
    [HResultConstant(0xC00D132F)]
    public static HResult NS_E_CURL_CANTDECODE => new(0xC00D132F);

    /// <summary>The specified URL is not a relative URL.</summary>
    [Description("""The specified URL is not a relative URL.""")]
    [HResultConstant(0xC00D1330)]
    public static HResult NS_E_CURLHELPER_NOTRELATIVE => new(0xC00D1330);

    /// <summary>The buffer is smaller than the size specified.</summary>
    [Description("""The buffer is smaller than the size specified.""")]
    [HResultConstant(0xC00D1331)]
    public static HResult NS_E_CURL_INVALIDBUFFERSIZE => new(0xC00D1331);

    /// <summary>The content provider has not granted you the right to play this file. Go to the content provider's online store to get play rights.</summary>
    [Description("""The content provider has not granted you the right to play this file. Go to the content provider's online store to get play rights.""")]
    [HResultConstant(0xC00D1356)]
    public static HResult NS_E_SUBSCRIPTIONSERVICE_PLAYBACK_DISALLOWED => new(0xC00D1356);

    /// <summary>Windows Media Player cannot purchase or download content from multiple online stores.</summary>
    [Description("""Windows Media Player cannot purchase or download content from multiple online stores.""")]
    [HResultConstant(0xC00D1357)]
    public static HResult NS_E_CANNOT_BUY_OR_DOWNLOAD_FROM_MULTIPLE_SERVICES => new(0xC00D1357);

    /// <summary>The file cannot be purchased or downloaded. The file might not be available from the online store.</summary>
    [Description("""The file cannot be purchased or downloaded. The file might not be available from the online store.""")]
    [HResultConstant(0xC00D1358)]
    public static HResult NS_E_CANNOT_BUY_OR_DOWNLOAD_CONTENT => new(0xC00D1358);

    /// <summary>The provider of this file cannot be identified.</summary>
    [Description("""The provider of this file cannot be identified.""")]
    [HResultConstant(0xC00D135A)]
    public static HResult NS_E_NOT_CONTENT_PARTNER_TRACK => new(0xC00D135A);

    /// <summary>The file is only available for download when you buy the entire album.</summary>
    [Description("""The file is only available for download when you buy the entire album.""")]
    [HResultConstant(0xC00D135B)]
    public static HResult NS_E_TRACK_DOWNLOAD_REQUIRES_ALBUM_PURCHASE => new(0xC00D135B);

    /// <summary>You must buy the file before you can download it.</summary>
    [Description("""You must buy the file before you can download it.""")]
    [HResultConstant(0xC00D135C)]
    public static HResult NS_E_TRACK_DOWNLOAD_REQUIRES_PURCHASE => new(0xC00D135C);

    /// <summary>You have exceeded the maximum number of files that can be purchased in a single transaction.</summary>
    [Description("""You have exceeded the maximum number of files that can be purchased in a single transaction.""")]
    [HResultConstant(0xC00D135D)]
    public static HResult NS_E_TRACK_PURCHASE_MAXIMUM_EXCEEDED => new(0xC00D135D);

    /// <summary>Windows Media Player cannot sign in to the online store. Verify that you are using the correct user name and password. If the problem persists, the store might be temporarily unavailable.</summary>
    [Description("""Windows Media Player cannot sign in to the online store. Verify that you are using the correct user name and password. If the problem persists, the store might be temporarily unavailable.""")]
    [HResultConstant(0xC00D135F)]
    public static HResult NS_E_SUBSCRIPTIONSERVICE_LOGIN_FAILED => new(0xC00D135F);

    /// <summary>Windows Media Player cannot download this item because the server is not responding. The server might be temporarily unavailable or the Internet connection might be lost.</summary>
    [Description("""Windows Media Player cannot download this item because the server is not responding. The server might be temporarily unavailable or the Internet connection might be lost.""")]
    [HResultConstant(0xC00D1360)]
    public static HResult NS_E_SUBSCRIPTIONSERVICE_DOWNLOAD_TIMEOUT => new(0xC00D1360);

    /// <summary>Content Partner still initializing.</summary>
    [Description("""Content Partner still initializing.""")]
    [HResultConstant(0xC00D1362)]
    public static HResult NS_E_CONTENT_PARTNER_STILL_INITIALIZING => new(0xC00D1362);

    /// <summary>The folder could not be opened. The folder might have been moved or deleted.</summary>
    [Description("""The folder could not be opened. The folder might have been moved or deleted.""")]
    [HResultConstant(0xC00D1363)]
    public static HResult NS_E_OPEN_CONTAINING_FOLDER_FAILED => new(0xC00D1363);

    /// <summary>Windows Media Player could not add all of the images to the file because the images exceeded the 7 megabyte (MB) limit.</summary>
    [Description("""Windows Media Player could not add all of the images to the file because the images exceeded the 7 megabyte (MB) limit.""")]
    [HResultConstant(0xC00D136A)]
    public static HResult NS_E_ADVANCEDEDIT_TOO_MANY_PICTURES => new(0xC00D136A);

    /// <summary>The client redirected to another server.</summary>
    [Description("""The client redirected to another server.""")]
    [HResultConstant(0xC00D1388)]
    public static HResult NS_E_REDIRECT => new(0xC00D1388);

    /// <summary>The streaming media description is no longer current.</summary>
    [Description("""The streaming media description is no longer current.""")]
    [HResultConstant(0xC00D1389)]
    public static HResult NS_E_STALE_PRESENTATION => new(0xC00D1389);

    /// <summary>It is not possible to create a persistent namespace node under a transient parent node.</summary>
    [Description("""It is not possible to create a persistent namespace node under a transient parent node.""")]
    [HResultConstant(0xC00D138A)]
    public static HResult NS_E_NAMESPACE_WRONG_PERSIST => new(0xC00D138A);

    /// <summary>It is not possible to store a value in a namespace node that has a different value type.</summary>
    [Description("""It is not possible to store a value in a namespace node that has a different value type.""")]
    [HResultConstant(0xC00D138B)]
    public static HResult NS_E_NAMESPACE_WRONG_TYPE => new(0xC00D138B);

    /// <summary>It is not possible to remove the root namespace node.</summary>
    [Description("""It is not possible to remove the root namespace node.""")]
    [HResultConstant(0xC00D138C)]
    public static HResult NS_E_NAMESPACE_NODE_CONFLICT => new(0xC00D138C);

    /// <summary>The specified namespace node could not be found.</summary>
    [Description("""The specified namespace node could not be found.""")]
    [HResultConstant(0xC00D138D)]
    public static HResult NS_E_NAMESPACE_NODE_NOT_FOUND => new(0xC00D138D);

    /// <summary>The buffer supplied to hold namespace node string is too small.</summary>
    [Description("""The buffer supplied to hold namespace node string is too small.""")]
    [HResultConstant(0xC00D138E)]
    public static HResult NS_E_NAMESPACE_BUFFER_TOO_SMALL => new(0xC00D138E);

    /// <summary>The callback list on a namespace node is at the maximum size.</summary>
    [Description("""The callback list on a namespace node is at the maximum size.""")]
    [HResultConstant(0xC00D138F)]
    public static HResult NS_E_NAMESPACE_TOO_MANY_CALLBACKS => new(0xC00D138F);

    /// <summary>It is not possible to register an already-registered callback on a namespace node.</summary>
    [Description("""It is not possible to register an already-registered callback on a namespace node.""")]
    [HResultConstant(0xC00D1390)]
    public static HResult NS_E_NAMESPACE_DUPLICATE_CALLBACK => new(0xC00D1390);

    /// <summary>Cannot find the callback in the namespace when attempting to remove the callback.</summary>
    [Description("""Cannot find the callback in the namespace when attempting to remove the callback.""")]
    [HResultConstant(0xC00D1391)]
    public static HResult NS_E_NAMESPACE_CALLBACK_NOT_FOUND => new(0xC00D1391);

    /// <summary>The namespace node name exceeds the allowed maximum length.</summary>
    [Description("""The namespace node name exceeds the allowed maximum length.""")]
    [HResultConstant(0xC00D1392)]
    public static HResult NS_E_NAMESPACE_NAME_TOO_LONG => new(0xC00D1392);

    /// <summary>Cannot create a namespace node that already exists.</summary>
    [Description("""Cannot create a namespace node that already exists.""")]
    [HResultConstant(0xC00D1393)]
    public static HResult NS_E_NAMESPACE_DUPLICATE_NAME => new(0xC00D1393);

    /// <summary>The namespace node name cannot be a null string.</summary>
    [Description("""The namespace node name cannot be a null string.""")]
    [HResultConstant(0xC00D1394)]
    public static HResult NS_E_NAMESPACE_EMPTY_NAME => new(0xC00D1394);

    /// <summary>Finding a child namespace node by index failed because the index exceeded the number of children.</summary>
    [Description("""Finding a child namespace node by index failed because the index exceeded the number of children.""")]
    [HResultConstant(0xC00D1395)]
    public static HResult NS_E_NAMESPACE_INDEX_TOO_LARGE => new(0xC00D1395);

    /// <summary>The namespace node name is invalid.</summary>
    [Description("""The namespace node name is invalid.""")]
    [HResultConstant(0xC00D1396)]
    public static HResult NS_E_NAMESPACE_BAD_NAME => new(0xC00D1396);

    /// <summary>It is not possible to store a value in a namespace node that has a different security type.</summary>
    [Description("""It is not possible to store a value in a namespace node that has a different security type.""")]
    [HResultConstant(0xC00D1397)]
    public static HResult NS_E_NAMESPACE_WRONG_SECURITY => new(0xC00D1397);

    /// <summary>The archive request conflicts with other requests in progress.</summary>
    [Description("""The archive request conflicts with other requests in progress.""")]
    [HResultConstant(0xC00D13EC)]
    public static HResult NS_E_CACHE_ARCHIVE_CONFLICT => new(0xC00D13EC);

    /// <summary>The specified origin server cannot be found.</summary>
    [Description("""The specified origin server cannot be found.""")]
    [HResultConstant(0xC00D13ED)]
    public static HResult NS_E_CACHE_ORIGIN_SERVER_NOT_FOUND => new(0xC00D13ED);

    /// <summary>The specified origin server is not responding.</summary>
    [Description("""The specified origin server is not responding.""")]
    [HResultConstant(0xC00D13EE)]
    public static HResult NS_E_CACHE_ORIGIN_SERVER_TIMEOUT => new(0xC00D13EE);

    /// <summary>The internal code for HTTP status code 412 Precondition Failed due to not broadcast type.</summary>
    [Description("""The internal code for HTTP status code 412 Precondition Failed due to not broadcast type.""")]
    [HResultConstant(0xC00D13EF)]
    public static HResult NS_E_CACHE_NOT_BROADCAST => new(0xC00D13EF);

    /// <summary>The internal code for HTTP status code 403 Forbidden due to not cacheable.</summary>
    [Description("""The internal code for HTTP status code 403 Forbidden due to not cacheable.""")]
    [HResultConstant(0xC00D13F0)]
    public static HResult NS_E_CACHE_CANNOT_BE_CACHED => new(0xC00D13F0);

    /// <summary>The internal code for HTTP status code 304 Not Modified.</summary>
    [Description("""The internal code for HTTP status code 304 Not Modified.""")]
    [HResultConstant(0xC00D13F1)]
    public static HResult NS_E_CACHE_NOT_MODIFIED => new(0xC00D13F1);

    /// <summary>It is not possible to remove a cache or proxy publishing point.</summary>
    [Description("""It is not possible to remove a cache or proxy publishing point.""")]
    [HResultConstant(0xC00D1450)]
    public static HResult NS_E_CANNOT_REMOVE_PUBLISHING_POINT => new(0xC00D1450);

    /// <summary>It is not possible to remove the last instance of a type of plug-in.</summary>
    [Description("""It is not possible to remove the last instance of a type of plug-in.""")]
    [HResultConstant(0xC00D1451)]
    public static HResult NS_E_CANNOT_REMOVE_PLUGIN => new(0xC00D1451);

    /// <summary>Cache and proxy publishing points do not support this property or method.</summary>
    [Description("""Cache and proxy publishing points do not support this property or method.""")]
    [HResultConstant(0xC00D1452)]
    public static HResult NS_E_WRONG_PUBLISHING_POINT_TYPE => new(0xC00D1452);

    /// <summary>The plug-in does not support the specified load type.</summary>
    [Description("""The plug-in does not support the specified load type.""")]
    [HResultConstant(0xC00D1453)]
    public static HResult NS_E_UNSUPPORTED_LOAD_TYPE => new(0xC00D1453);

    /// <summary>The plug-in does not support any load types. The plug-in must support at least one load type.</summary>
    [Description("""The plug-in does not support any load types. The plug-in must support at least one load type.""")]
    [HResultConstant(0xC00D1454)]
    public static HResult NS_E_INVALID_PLUGIN_LOAD_TYPE_CONFIGURATION => new(0xC00D1454);

    /// <summary>The publishing point name is invalid.</summary>
    [Description("""The publishing point name is invalid.""")]
    [HResultConstant(0xC00D1455)]
    public static HResult NS_E_INVALID_PUBLISHING_POINT_NAME => new(0xC00D1455);

    /// <summary>Only one multicast data writer plug-in can be enabled for a publishing point.</summary>
    [Description("""Only one multicast data writer plug-in can be enabled for a publishing point.""")]
    [HResultConstant(0xC00D1456)]
    public static HResult NS_E_TOO_MANY_MULTICAST_SINKS => new(0xC00D1456);

    /// <summary>The requested operation cannot be completed while the publishing point is started.</summary>
    [Description("""The requested operation cannot be completed while the publishing point is started.""")]
    [HResultConstant(0xC00D1457)]
    public static HResult NS_E_PUBLISHING_POINT_INVALID_REQUEST_WHILE_STARTED => new(0xC00D1457);

    /// <summary>A multicast data writer plug-in must be enabled in order for this operation to be completed.</summary>
    [Description("""A multicast data writer plug-in must be enabled in order for this operation to be completed.""")]
    [HResultConstant(0xC00D1458)]
    public static HResult NS_E_MULTICAST_PLUGIN_NOT_ENABLED => new(0xC00D1458);

    /// <summary>This feature requires Windows Server 2003, Enterprise Edition.</summary>
    [Description("""This feature requires Windows Server 2003, Enterprise Edition.""")]
    [HResultConstant(0xC00D1459)]
    public static HResult NS_E_INVALID_OPERATING_SYSTEM_VERSION => new(0xC00D1459);

    /// <summary>The requested operation cannot be completed because the specified publishing point has been removed.</summary>
    [Description("""The requested operation cannot be completed because the specified publishing point has been removed.""")]
    [HResultConstant(0xC00D145A)]
    public static HResult NS_E_PUBLISHING_POINT_REMOVED => new(0xC00D145A);

    /// <summary>Push publishing points are started when the encoder starts pushing the stream. This publishing point cannot be started by the server administrator.</summary>
    [Description("""Push publishing points are started when the encoder starts pushing the stream. This publishing point cannot be started by the server administrator.""")]
    [HResultConstant(0xC00D145B)]
    public static HResult NS_E_INVALID_PUSH_PUBLISHING_POINT_START_REQUEST => new(0xC00D145B);

    /// <summary>The specified language is not supported.</summary>
    [Description("""The specified language is not supported.""")]
    [HResultConstant(0xC00D145C)]
    public static HResult NS_E_UNSUPPORTED_LANGUAGE => new(0xC00D145C);

    /// <summary>Windows Media Services will only run on Windows Server 2003, Standard Edition and Windows Server 2003, Enterprise Edition.</summary>
    [Description("""Windows Media Services will only run on Windows Server 2003, Standard Edition and Windows Server 2003, Enterprise Edition.""")]
    [HResultConstant(0xC00D145D)]
    public static HResult NS_E_WRONG_OS_VERSION => new(0xC00D145D);

    /// <summary>The operation cannot be completed because the publishing point has been stopped.</summary>
    [Description("""The operation cannot be completed because the publishing point has been stopped.""")]
    [HResultConstant(0xC00D145E)]
    public static HResult NS_E_PUBLISHING_POINT_STOPPED => new(0xC00D145E);

    /// <summary>The playlist entry is already playing.</summary>
    [Description("""The playlist entry is already playing.""")]
    [HResultConstant(0xC00D14B4)]
    public static HResult NS_E_PLAYLIST_ENTRY_ALREADY_PLAYING => new(0xC00D14B4);

    /// <summary>The playlist or directory you are requesting does not contain content.</summary>
    [Description("""The playlist or directory you are requesting does not contain content.""")]
    [HResultConstant(0xC00D14B5)]
    public static HResult NS_E_EMPTY_PLAYLIST => new(0xC00D14B5);

    /// <summary>The server was unable to parse the requested playlist file.</summary>
    [Description("""The server was unable to parse the requested playlist file.""")]
    [HResultConstant(0xC00D14B6)]
    public static HResult NS_E_PLAYLIST_PARSE_FAILURE => new(0xC00D14B6);

    /// <summary>The requested operation is not supported for this type of playlist entry.</summary>
    [Description("""The requested operation is not supported for this type of playlist entry.""")]
    [HResultConstant(0xC00D14B7)]
    public static HResult NS_E_PLAYLIST_UNSUPPORTED_ENTRY => new(0xC00D14B7);

    /// <summary>Cannot jump to a playlist entry that is not inserted in the playlist.</summary>
    [Description("""Cannot jump to a playlist entry that is not inserted in the playlist.""")]
    [HResultConstant(0xC00D14B8)]
    public static HResult NS_E_PLAYLIST_ENTRY_NOT_IN_PLAYLIST => new(0xC00D14B8);

    /// <summary>Cannot seek to the desired playlist entry.</summary>
    [Description("""Cannot seek to the desired playlist entry.""")]
    [HResultConstant(0xC00D14B9)]
    public static HResult NS_E_PLAYLIST_ENTRY_SEEK => new(0xC00D14B9);

    /// <summary>Cannot play recursive playlist.</summary>
    [Description("""Cannot play recursive playlist.""")]
    [HResultConstant(0xC00D14BA)]
    public static HResult NS_E_PLAYLIST_RECURSIVE_PLAYLISTS => new(0xC00D14BA);

    /// <summary>The number of nested playlists exceeded the limit the server can handle.</summary>
    [Description("""The number of nested playlists exceeded the limit the server can handle.""")]
    [HResultConstant(0xC00D14BB)]
    public static HResult NS_E_PLAYLIST_TOO_MANY_NESTED_PLAYLISTS => new(0xC00D14BB);

    /// <summary>Cannot execute the requested operation because the playlist has been shut down by the Media Server.</summary>
    [Description("""Cannot execute the requested operation because the playlist has been shut down by the Media Server.""")]
    [HResultConstant(0xC00D14BC)]
    public static HResult NS_E_PLAYLIST_SHUTDOWN => new(0xC00D14BC);

    /// <summary>The playlist has ended while receding.</summary>
    [Description("""The playlist has ended while receding.""")]
    [HResultConstant(0xC00D14BD)]
    public static HResult NS_E_PLAYLIST_END_RECEDING => new(0xC00D14BD);

    /// <summary>The data path does not have an associated data writer plug-in.</summary>
    [Description("""The data path does not have an associated data writer plug-in.""")]
    [HResultConstant(0xC00D1518)]
    public static HResult NS_E_DATAPATH_NO_SINK => new(0xC00D1518);

    /// <summary>The specified push template is invalid.</summary>
    [Description("""The specified push template is invalid.""")]
    [HResultConstant(0xC00D151A)]
    public static HResult NS_E_INVALID_PUSH_TEMPLATE => new(0xC00D151A);

    /// <summary>The specified push publishing point is invalid.</summary>
    [Description("""The specified push publishing point is invalid.""")]
    [HResultConstant(0xC00D151B)]
    public static HResult NS_E_INVALID_PUSH_PUBLISHING_POINT => new(0xC00D151B);

    /// <summary>The requested operation cannot be performed because the server or publishing point is in a critical error state.</summary>
    [Description("""The requested operation cannot be performed because the server or publishing point is in a critical error state.""")]
    [HResultConstant(0xC00D151C)]
    public static HResult NS_E_CRITICAL_ERROR => new(0xC00D151C);

    /// <summary>The content cannot be played because the server is not currently accepting connections. Try connecting at a later time.</summary>
    [Description("""The content cannot be played because the server is not currently accepting connections. Try connecting at a later time.""")]
    [HResultConstant(0xC00D151D)]
    public static HResult NS_E_NO_NEW_CONNECTIONS => new(0xC00D151D);

    /// <summary>The version of this playlist is not supported by the server.</summary>
    [Description("""The version of this playlist is not supported by the server.""")]
    [HResultConstant(0xC00D151E)]
    public static HResult NS_E_WSX_INVALID_VERSION => new(0xC00D151E);

    /// <summary>The command does not apply to the current media header user by a server component.</summary>
    [Description("""The command does not apply to the current media header user by a server component.""")]
    [HResultConstant(0xC00D151F)]
    public static HResult NS_E_HEADER_MISMATCH => new(0xC00D151F);

    /// <summary>The specified publishing point name is already in use.</summary>
    [Description("""The specified publishing point name is already in use.""")]
    [HResultConstant(0xC00D1520)]
    public static HResult NS_E_PUSH_DUPLICATE_PUBLISHING_POINT_NAME => new(0xC00D1520);

    /// <summary>There is no script engine available for this file.</summary>
    [Description("""There is no script engine available for this file.""")]
    [HResultConstant(0xC00D157C)]
    public static HResult NS_E_NO_SCRIPT_ENGINE => new(0xC00D157C);

    /// <summary>The plug-in has reported an error. See the Troubleshooting tab or the NT Application Event Log for details.</summary>
    [Description("""The plug-in has reported an error. See the Troubleshooting tab or the NT Application Event Log for details.""")]
    [HResultConstant(0xC00D157D)]
    public static HResult NS_E_PLUGIN_ERROR_REPORTED => new(0xC00D157D);

    /// <summary>No enabled data source plug-in is available to access the requested content.</summary>
    [Description("""No enabled data source plug-in is available to access the requested content.""")]
    [HResultConstant(0xC00D157E)]
    public static HResult NS_E_SOURCE_PLUGIN_NOT_FOUND => new(0xC00D157E);

    /// <summary>No enabled playlist parser plug-in is available to access the requested content.</summary>
    [Description("""No enabled playlist parser plug-in is available to access the requested content.""")]
    [HResultConstant(0xC00D157F)]
    public static HResult NS_E_PLAYLIST_PLUGIN_NOT_FOUND => new(0xC00D157F);

    /// <summary>The data source plug-in does not support enumeration.</summary>
    [Description("""The data source plug-in does not support enumeration.""")]
    [HResultConstant(0xC00D1580)]
    public static HResult NS_E_DATA_SOURCE_ENUMERATION_NOT_SUPPORTED => new(0xC00D1580);

    /// <summary>The server cannot stream the selected file because it is either damaged or corrupt. Select a different file.</summary>
    [Description("""The server cannot stream the selected file because it is either damaged or corrupt. Select a different file.""")]
    [HResultConstant(0xC00D1581)]
    public static HResult NS_E_MEDIA_PARSER_INVALID_FORMAT => new(0xC00D1581);

    /// <summary>The plug-in cannot be enabled because a compatible script debugger is not installed on this system. Install a script debugger, or disable the script debugger option on the general tab of the plug-in's properties page and try again.</summary>
    [Description("""The plug-in cannot be enabled because a compatible script debugger is not installed on this system. Install a script debugger, or disable the script debugger option on the general tab of the plug-in's properties page and try again.""")]
    [HResultConstant(0xC00D1582)]
    public static HResult NS_E_SCRIPT_DEBUGGER_NOT_INSTALLED => new(0xC00D1582);

    /// <summary>The plug-in cannot be loaded because it requires Windows Server 2003, Enterprise Edition.</summary>
    [Description("""The plug-in cannot be loaded because it requires Windows Server 2003, Enterprise Edition.""")]
    [HResultConstant(0xC00D1583)]
    public static HResult NS_E_FEATURE_REQUIRES_ENTERPRISE_SERVER => new(0xC00D1583);

    /// <summary>Another wizard is currently running. Please close the other wizard or wait until it finishes before attempting to run this wizard again.</summary>
    [Description("""Another wizard is currently running. Please close the other wizard or wait until it finishes before attempting to run this wizard again.""")]
    [HResultConstant(0xC00D1584)]
    public static HResult NS_E_WIZARD_RUNNING => new(0xC00D1584);

    /// <summary>Invalid log URL. Multicast logging URL must look like "http://servername/isapibackend.dll".</summary>
    [Description("""Invalid log URL. Multicast logging URL must look like "http://servername/isapibackend.dll".""")]
    [HResultConstant(0xC00D1585)]
    public static HResult NS_E_INVALID_LOG_URL => new(0xC00D1585);

    /// <summary>Invalid MTU specified. The valid range for maximum packet size is between 36 and 65507 bytes.</summary>
    [Description("""Invalid MTU specified. The valid range for maximum packet size is between 36 and 65507 bytes.""")]
    [HResultConstant(0xC00D1586)]
    public static HResult NS_E_INVALID_MTU_RANGE => new(0xC00D1586);

    /// <summary>Invalid play statistics for logging.</summary>
    [Description("""Invalid play statistics for logging.""")]
    [HResultConstant(0xC00D1587)]
    public static HResult NS_E_INVALID_PLAY_STATISTICS => new(0xC00D1587);

    /// <summary>The log needs to be skipped.</summary>
    [Description("""The log needs to be skipped.""")]
    [HResultConstant(0xC00D1588)]
    public static HResult NS_E_LOG_NEED_TO_BE_SKIPPED => new(0xC00D1588);

    /// <summary>The size of the data exceeded the limit the WMS HTTP Download Data Source plugin can handle.</summary>
    [Description("""The size of the data exceeded the limit the WMS HTTP Download Data Source plugin can handle.""")]
    [HResultConstant(0xC00D1589)]
    public static HResult NS_E_HTTP_TEXT_DATACONTAINER_SIZE_LIMIT_EXCEEDED => new(0xC00D1589);

    /// <summary>One usage of each socket address (protocol/network address/port) is permitted. Verify that other services or applications are not attempting to use the same port and then try to enable the plug-in again.</summary>
    [Description("""One usage of each socket address (protocol/network address/port) is permitted. Verify that other services or applications are not attempting to use the same port and then try to enable the plug-in again.""")]
    [HResultConstant(0xC00D158A)]
    public static HResult NS_E_PORT_IN_USE => new(0xC00D158A);

    /// <summary>One usage of each socket address (protocol/network address/port) is permitted. Verify that other services (such as IIS) or applications are not attempting to use the same port and then try to enable the plug-in again.</summary>
    [Description("""One usage of each socket address (protocol/network address/port) is permitted. Verify that other services (such as IIS) or applications are not attempting to use the same port and then try to enable the plug-in again.""")]
    [HResultConstant(0xC00D158B)]
    public static HResult NS_E_PORT_IN_USE_HTTP => new(0xC00D158B);

    /// <summary>The WMS HTTP Download Data Source plugin was unable to receive the remote server's response.</summary>
    [Description("""The WMS HTTP Download Data Source plugin was unable to receive the remote server's response.""")]
    [HResultConstant(0xC00D158C)]
    public static HResult NS_E_HTTP_TEXT_DATACONTAINER_INVALID_SERVER_RESPONSE => new(0xC00D158C);

    /// <summary>The archive plug-in has reached its quota.</summary>
    [Description("""The archive plug-in has reached its quota.""")]
    [HResultConstant(0xC00D158D)]
    public static HResult NS_E_ARCHIVE_REACH_QUOTA => new(0xC00D158D);

    /// <summary>The archive plug-in aborted because the source was from broadcast.</summary>
    [Description("""The archive plug-in aborted because the source was from broadcast.""")]
    [HResultConstant(0xC00D158E)]
    public static HResult NS_E_ARCHIVE_ABORT_DUE_TO_BCAST => new(0xC00D158E);

    /// <summary>The archive plug-in detected an interrupt in the source.</summary>
    [Description("""The archive plug-in detected an interrupt in the source.""")]
    [HResultConstant(0xC00D158F)]
    public static HResult NS_E_ARCHIVE_GAP_DETECTED => new(0xC00D158F);

    /// <summary>The system cannot find the file specified.</summary>
    [Description("""The system cannot find the file specified.""")]
    [HResultConstant(0xC00D1590)]
    public static HResult NS_E_AUTHORIZATION_FILE_NOT_FOUND => new(0xC00D1590);

    /// <summary>The mark-in time should be greater than 0 and less than the mark-out time.</summary>
    [Description("""The mark-in time should be greater than 0 and less than the mark-out time.""")]
    [HResultConstant(0xC00D1B58)]
    public static HResult NS_E_BAD_MARKIN => new(0xC00D1B58);

    /// <summary>The mark-out time should be greater than the mark-in time and less than the file duration.</summary>
    [Description("""The mark-out time should be greater than the mark-in time and less than the file duration.""")]
    [HResultConstant(0xC00D1B59)]
    public static HResult NS_E_BAD_MARKOUT => new(0xC00D1B59);

    /// <summary>No matching media type is found in the source %1.</summary>
    [Description("""No matching media type is found in the source %1.""")]
    [HResultConstant(0xC00D1B5A)]
    public static HResult NS_E_NOMATCHING_MEDIASOURCE => new(0xC00D1B5A);

    /// <summary>The specified source type is not supported.</summary>
    [Description("""The specified source type is not supported.""")]
    [HResultConstant(0xC00D1B5B)]
    public static HResult NS_E_UNSUPPORTED_SOURCETYPE => new(0xC00D1B5B);

    /// <summary>It is not possible to specify more than one audio input.</summary>
    [Description("""It is not possible to specify more than one audio input.""")]
    [HResultConstant(0xC00D1B5C)]
    public static HResult NS_E_TOO_MANY_AUDIO => new(0xC00D1B5C);

    /// <summary>It is not possible to specify more than two video inputs.</summary>
    [Description("""It is not possible to specify more than two video inputs.""")]
    [HResultConstant(0xC00D1B5D)]
    public static HResult NS_E_TOO_MANY_VIDEO => new(0xC00D1B5D);

    /// <summary>No matching element is found in the list.</summary>
    [Description("""No matching element is found in the list.""")]
    [HResultConstant(0xC00D1B5E)]
    public static HResult NS_E_NOMATCHING_ELEMENT => new(0xC00D1B5E);

    /// <summary>The profile's media types must match the media types defined for the session.</summary>
    [Description("""The profile's media types must match the media types defined for the session.""")]
    [HResultConstant(0xC00D1B5F)]
    public static HResult NS_E_MISMATCHED_MEDIACONTENT => new(0xC00D1B5F);

    /// <summary>It is not possible to remove an active source while encoding.</summary>
    [Description("""It is not possible to remove an active source while encoding.""")]
    [HResultConstant(0xC00D1B60)]
    public static HResult NS_E_CANNOT_DELETE_ACTIVE_SOURCEGROUP => new(0xC00D1B60);

    /// <summary>It is not possible to open the specified audio capture device because it is currently in use.</summary>
    [Description("""It is not possible to open the specified audio capture device because it is currently in use.""")]
    [HResultConstant(0xC00D1B61)]
    public static HResult NS_E_AUDIODEVICE_BUSY => new(0xC00D1B61);

    /// <summary>It is not possible to open the specified audio capture device because an unexpected error has occurred.</summary>
    [Description("""It is not possible to open the specified audio capture device because an unexpected error has occurred.""")]
    [HResultConstant(0xC00D1B62)]
    public static HResult NS_E_AUDIODEVICE_UNEXPECTED => new(0xC00D1B62);

    /// <summary>The audio capture device does not support the specified audio format.</summary>
    [Description("""The audio capture device does not support the specified audio format.""")]
    [HResultConstant(0xC00D1B63)]
    public static HResult NS_E_AUDIODEVICE_BADFORMAT => new(0xC00D1B63);

    /// <summary>It is not possible to open the specified video capture device because it is currently in use.</summary>
    [Description("""It is not possible to open the specified video capture device because it is currently in use.""")]
    [HResultConstant(0xC00D1B64)]
    public static HResult NS_E_VIDEODEVICE_BUSY => new(0xC00D1B64);

    /// <summary>It is not possible to open the specified video capture device because an unexpected error has occurred.</summary>
    [Description("""It is not possible to open the specified video capture device because an unexpected error has occurred.""")]
    [HResultConstant(0xC00D1B65)]
    public static HResult NS_E_VIDEODEVICE_UNEXPECTED => new(0xC00D1B65);

    /// <summary>This operation is not allowed while encoding.</summary>
    [Description("""This operation is not allowed while encoding.""")]
    [HResultConstant(0xC00D1B66)]
    public static HResult NS_E_INVALIDCALL_WHILE_ENCODER_RUNNING => new(0xC00D1B66);

    /// <summary>No profile is set for the source.</summary>
    [Description("""No profile is set for the source.""")]
    [HResultConstant(0xC00D1B67)]
    public static HResult NS_E_NO_PROFILE_IN_SOURCEGROUP => new(0xC00D1B67);

    /// <summary>The video capture driver returned an unrecoverable error. It is now in an unstable state.</summary>
    [Description("""The video capture driver returned an unrecoverable error. It is now in an unstable state.""")]
    [HResultConstant(0xC00D1B68)]
    public static HResult NS_E_VIDEODRIVER_UNSTABLE => new(0xC00D1B68);

    /// <summary>It was not possible to start the video device.</summary>
    [Description("""It was not possible to start the video device.""")]
    [HResultConstant(0xC00D1B69)]
    public static HResult NS_E_VIDCAPSTARTFAILED => new(0xC00D1B69);

    /// <summary>The video source does not support the requested output format or color depth.</summary>
    [Description("""The video source does not support the requested output format or color depth.""")]
    [HResultConstant(0xC00D1B6A)]
    public static HResult NS_E_VIDSOURCECOMPRESSION => new(0xC00D1B6A);

    /// <summary>The video source does not support the requested capture size.</summary>
    [Description("""The video source does not support the requested capture size.""")]
    [HResultConstant(0xC00D1B6B)]
    public static HResult NS_E_VIDSOURCESIZE => new(0xC00D1B6B);

    /// <summary>It was not possible to obtain output information from the video compressor.</summary>
    [Description("""It was not possible to obtain output information from the video compressor.""")]
    [HResultConstant(0xC00D1B6C)]
    public static HResult NS_E_ICMQUERYFORMAT => new(0xC00D1B6C);

    /// <summary>It was not possible to create a video capture window.</summary>
    [Description("""It was not possible to create a video capture window.""")]
    [HResultConstant(0xC00D1B6D)]
    public static HResult NS_E_VIDCAPCREATEWINDOW => new(0xC00D1B6D);

    /// <summary>There is already a stream active on this video device.</summary>
    [Description("""There is already a stream active on this video device.""")]
    [HResultConstant(0xC00D1B6E)]
    public static HResult NS_E_VIDCAPDRVINUSE => new(0xC00D1B6E);

    /// <summary>No media format is set in source.</summary>
    [Description("""No media format is set in source.""")]
    [HResultConstant(0xC00D1B6F)]
    public static HResult NS_E_NO_MEDIAFORMAT_IN_SOURCE => new(0xC00D1B6F);

    /// <summary>Cannot find a valid output stream from the source.</summary>
    [Description("""Cannot find a valid output stream from the source.""")]
    [HResultConstant(0xC00D1B70)]
    public static HResult NS_E_NO_VALID_OUTPUT_STREAM => new(0xC00D1B70);

    /// <summary>It was not possible to find a valid source plug-in for the specified source.</summary>
    [Description("""It was not possible to find a valid source plug-in for the specified source.""")]
    [HResultConstant(0xC00D1B71)]
    public static HResult NS_E_NO_VALID_SOURCE_PLUGIN => new(0xC00D1B71);

    /// <summary>No source is currently active.</summary>
    [Description("""No source is currently active.""")]
    [HResultConstant(0xC00D1B72)]
    public static HResult NS_E_NO_ACTIVE_SOURCEGROUP => new(0xC00D1B72);

    /// <summary>No script stream is set in the current source.</summary>
    [Description("""No script stream is set in the current source.""")]
    [HResultConstant(0xC00D1B73)]
    public static HResult NS_E_NO_SCRIPT_STREAM => new(0xC00D1B73);

    /// <summary>This operation is not allowed while archiving.</summary>
    [Description("""This operation is not allowed while archiving.""")]
    [HResultConstant(0xC00D1B74)]
    public static HResult NS_E_INVALIDCALL_WHILE_ARCHIVAL_RUNNING => new(0xC00D1B74);

    /// <summary>The setting for the maximum packet size is not valid.</summary>
    [Description("""The setting for the maximum packet size is not valid.""")]
    [HResultConstant(0xC00D1B75)]
    public static HResult NS_E_INVALIDPACKETSIZE => new(0xC00D1B75);

    /// <summary>The plug-in CLSID specified is not valid.</summary>
    [Description("""The plug-in CLSID specified is not valid.""")]
    [HResultConstant(0xC00D1B76)]
    public static HResult NS_E_PLUGIN_CLSID_INVALID => new(0xC00D1B76);

    /// <summary>This archive type is not supported.</summary>
    [Description("""This archive type is not supported.""")]
    [HResultConstant(0xC00D1B77)]
    public static HResult NS_E_UNSUPPORTED_ARCHIVETYPE => new(0xC00D1B77);

    /// <summary>This archive operation is not supported.</summary>
    [Description("""This archive operation is not supported.""")]
    [HResultConstant(0xC00D1B78)]
    public static HResult NS_E_UNSUPPORTED_ARCHIVEOPERATION => new(0xC00D1B78);

    /// <summary>The local archive file name was not set.</summary>
    [Description("""The local archive file name was not set.""")]
    [HResultConstant(0xC00D1B79)]
    public static HResult NS_E_ARCHIVE_FILENAME_NOTSET => new(0xC00D1B79);

    /// <summary>The source is not yet prepared.</summary>
    [Description("""The source is not yet prepared.""")]
    [HResultConstant(0xC00D1B7A)]
    public static HResult NS_E_SOURCEGROUP_NOTPREPARED => new(0xC00D1B7A);

    /// <summary>Profiles on the sources do not match.</summary>
    [Description("""Profiles on the sources do not match.""")]
    [HResultConstant(0xC00D1B7B)]
    public static HResult NS_E_PROFILE_MISMATCH => new(0xC00D1B7B);

    /// <summary>The specified crop values are not valid.</summary>
    [Description("""The specified crop values are not valid.""")]
    [HResultConstant(0xC00D1B7C)]
    public static HResult NS_E_INCORRECTCLIPSETTINGS => new(0xC00D1B7C);

    /// <summary>No statistics are available at this time.</summary>
    [Description("""No statistics are available at this time.""")]
    [HResultConstant(0xC00D1B7D)]
    public static HResult NS_E_NOSTATSAVAILABLE => new(0xC00D1B7D);

    /// <summary>The encoder is not archiving.</summary>
    [Description("""The encoder is not archiving.""")]
    [HResultConstant(0xC00D1B7E)]
    public static HResult NS_E_NOTARCHIVING => new(0xC00D1B7E);

    /// <summary>This operation is only allowed during encoding.</summary>
    [Description("""This operation is only allowed during encoding.""")]
    [HResultConstant(0xC00D1B7F)]
    public static HResult NS_E_INVALIDCALL_WHILE_ENCODER_STOPPED => new(0xC00D1B7F);

    /// <summary>This SourceGroupCollection doesn't contain any SourceGroups.</summary>
    [Description("""This SourceGroupCollection doesn't contain any SourceGroups.""")]
    [HResultConstant(0xC00D1B80)]
    public static HResult NS_E_NOSOURCEGROUPS => new(0xC00D1B80);

    /// <summary>This source does not have a frame rate of 30 fps. Therefore, it is not possible to apply the inverse telecine filter to the source.</summary>
    [Description("""This source does not have a frame rate of 30 fps. Therefore, it is not possible to apply the inverse telecine filter to the source.""")]
    [HResultConstant(0xC00D1B81)]
    public static HResult NS_E_INVALIDINPUTFPS => new(0xC00D1B81);

    /// <summary>It is not possible to display your source or output video in the Video panel.</summary>
    [Description("""It is not possible to display your source or output video in the Video panel.""")]
    [HResultConstant(0xC00D1B82)]
    public static HResult NS_E_NO_DATAVIEW_SUPPORT => new(0xC00D1B82);

    /// <summary>One or more codecs required to open this content could not be found.</summary>
    [Description("""One or more codecs required to open this content could not be found.""")]
    [HResultConstant(0xC00D1B83)]
    public static HResult NS_E_CODEC_UNAVAILABLE => new(0xC00D1B83);

    /// <summary>The archive file has the same name as an input file. Change one of the names before continuing.</summary>
    [Description("""The archive file has the same name as an input file. Change one of the names before continuing.""")]
    [HResultConstant(0xC00D1B84)]
    public static HResult NS_E_ARCHIVE_SAME_AS_INPUT => new(0xC00D1B84);

    /// <summary>The source has not been set up completely.</summary>
    [Description("""The source has not been set up completely.""")]
    [HResultConstant(0xC00D1B85)]
    public static HResult NS_E_SOURCE_NOTSPECIFIED => new(0xC00D1B85);

    /// <summary>It is not possible to apply time compression to a broadcast session.</summary>
    [Description("""It is not possible to apply time compression to a broadcast session.""")]
    [HResultConstant(0xC00D1B86)]
    public static HResult NS_E_NO_REALTIME_TIMECOMPRESSION => new(0xC00D1B86);

    /// <summary>It is not possible to open this device.</summary>
    [Description("""It is not possible to open this device.""")]
    [HResultConstant(0xC00D1B87)]
    public static HResult NS_E_UNSUPPORTED_ENCODER_DEVICE => new(0xC00D1B87);

    /// <summary>It is not possible to start encoding because the display size or color has changed since the current session was defined. Restore the previous settings or create a new session.</summary>
    [Description("""It is not possible to start encoding because the display size or color has changed since the current session was defined. Restore the previous settings or create a new session.""")]
    [HResultConstant(0xC00D1B88)]
    public static HResult NS_E_UNEXPECTED_DISPLAY_SETTINGS => new(0xC00D1B88);

    /// <summary>No audio data has been received for several seconds. Check the audio source and restart the encoder.</summary>
    [Description("""No audio data has been received for several seconds. Check the audio source and restart the encoder.""")]
    [HResultConstant(0xC00D1B89)]
    public static HResult NS_E_NO_AUDIODATA => new(0xC00D1B89);

    /// <summary>One or all of the specified sources are not working properly. Check that the sources are configured correctly.</summary>
    [Description("""One or all of the specified sources are not working properly. Check that the sources are configured correctly.""")]
    [HResultConstant(0xC00D1B8A)]
    public static HResult NS_E_INPUTSOURCE_PROBLEM => new(0xC00D1B8A);

    /// <summary>The supplied configuration file is not supported by this version of the encoder.</summary>
    [Description("""The supplied configuration file is not supported by this version of the encoder.""")]
    [HResultConstant(0xC00D1B8B)]
    public static HResult NS_E_WME_VERSION_MISMATCH => new(0xC00D1B8B);

    /// <summary>It is not possible to use image preprocessing with live encoding.</summary>
    [Description("""It is not possible to use image preprocessing with live encoding.""")]
    [HResultConstant(0xC00D1B8C)]
    public static HResult NS_E_NO_REALTIME_PREPROCESS => new(0xC00D1B8C);

    /// <summary>It is not possible to use two-pass encoding when the source is set to loop.</summary>
    [Description("""It is not possible to use two-pass encoding when the source is set to loop.""")]
    [HResultConstant(0xC00D1B8D)]
    public static HResult NS_E_NO_REPEAT_PREPROCESS => new(0xC00D1B8D);

    /// <summary>It is not possible to pause encoding during a broadcast.</summary>
    [Description("""It is not possible to pause encoding during a broadcast.""")]
    [HResultConstant(0xC00D1B8E)]
    public static HResult NS_E_CANNOT_PAUSE_LIVEBROADCAST => new(0xC00D1B8E);

    /// <summary>A DRM profile has not been set for the current session.</summary>
    [Description("""A DRM profile has not been set for the current session.""")]
    [HResultConstant(0xC00D1B8F)]
    public static HResult NS_E_DRM_PROFILE_NOT_SET => new(0xC00D1B8F);

    /// <summary>The profile ID is already used by a DRM profile. Specify a different profile ID.</summary>
    [Description("""The profile ID is already used by a DRM profile. Specify a different profile ID.""")]
    [HResultConstant(0xC00D1B90)]
    public static HResult NS_E_DUPLICATE_DRMPROFILE => new(0xC00D1B90);

    /// <summary>The setting of the selected device does not support control for playing back tapes.</summary>
    [Description("""The setting of the selected device does not support control for playing back tapes.""")]
    [HResultConstant(0xC00D1B91)]
    public static HResult NS_E_INVALID_DEVICE => new(0xC00D1B91);

    /// <summary>You must specify a mixed voice and audio mode in order to use an optimization definition file.</summary>
    [Description("""You must specify a mixed voice and audio mode in order to use an optimization definition file.""")]
    [HResultConstant(0xC00D1B92)]
    public static HResult NS_E_SPEECHEDL_ON_NON_MIXEDMODE => new(0xC00D1B92);

    /// <summary>The specified password is too long. Type a password with fewer than 8 characters.</summary>
    [Description("""The specified password is too long. Type a password with fewer than 8 characters.""")]
    [HResultConstant(0xC00D1B93)]
    public static HResult NS_E_DRM_PASSWORD_TOO_LONG => new(0xC00D1B93);

    /// <summary>It is not possible to seek to the specified mark-in point.</summary>
    [Description("""It is not possible to seek to the specified mark-in point.""")]
    [HResultConstant(0xC00D1B94)]
    public static HResult NS_E_DEVCONTROL_FAILED_SEEK => new(0xC00D1B94);

    /// <summary>When you choose to maintain the interlacing in your video, the output video size must match the input video size.</summary>
    [Description("""When you choose to maintain the interlacing in your video, the output video size must match the input video size.""")]
    [HResultConstant(0xC00D1B95)]
    public static HResult NS_E_INTERLACE_REQUIRE_SAMESIZE => new(0xC00D1B95);

    /// <summary>Only one device control plug-in can control a device.</summary>
    [Description("""Only one device control plug-in can control a device.""")]
    [HResultConstant(0xC00D1B96)]
    public static HResult NS_E_TOO_MANY_DEVICECONTROL => new(0xC00D1B96);

    /// <summary>You must also enable storing content to hard disk temporarily in order to use two-pass encoding with the input device.</summary>
    [Description("""You must also enable storing content to hard disk temporarily in order to use two-pass encoding with the input device.""")]
    [HResultConstant(0xC00D1B97)]
    public static HResult NS_E_NO_MULTIPASS_FOR_LIVEDEVICE => new(0xC00D1B97);

    /// <summary>An audience is missing from the output stream configuration.</summary>
    [Description("""An audience is missing from the output stream configuration.""")]
    [HResultConstant(0xC00D1B98)]
    public static HResult NS_E_MISSING_AUDIENCE => new(0xC00D1B98);

    /// <summary>All audiences in the output tree must have the same content type.</summary>
    [Description("""All audiences in the output tree must have the same content type.""")]
    [HResultConstant(0xC00D1B99)]
    public static HResult NS_E_AUDIENCE_CONTENTTYPE_MISMATCH => new(0xC00D1B99);

    /// <summary>A source index is missing from the output stream configuration.</summary>
    [Description("""A source index is missing from the output stream configuration.""")]
    [HResultConstant(0xC00D1B9A)]
    public static HResult NS_E_MISSING_SOURCE_INDEX => new(0xC00D1B9A);

    /// <summary>The same source index in different audiences should have the same number of languages.</summary>
    [Description("""The same source index in different audiences should have the same number of languages.""")]
    [HResultConstant(0xC00D1B9B)]
    public static HResult NS_E_NUM_LANGUAGE_MISMATCH => new(0xC00D1B9B);

    /// <summary>The same source index in different audiences should have the same languages.</summary>
    [Description("""The same source index in different audiences should have the same languages.""")]
    [HResultConstant(0xC00D1B9C)]
    public static HResult NS_E_LANGUAGE_MISMATCH => new(0xC00D1B9C);

    /// <summary>The same source index in different audiences should use the same VBR encoding mode.</summary>
    [Description("""The same source index in different audiences should use the same VBR encoding mode.""")]
    [HResultConstant(0xC00D1B9D)]
    public static HResult NS_E_VBRMODE_MISMATCH => new(0xC00D1B9D);

    /// <summary>The bit rate index specified is not valid.</summary>
    [Description("""The bit rate index specified is not valid.""")]
    [HResultConstant(0xC00D1B9E)]
    public static HResult NS_E_INVALID_INPUT_AUDIENCE_INDEX => new(0xC00D1B9E);

    /// <summary>The specified language is not valid.</summary>
    [Description("""The specified language is not valid.""")]
    [HResultConstant(0xC00D1B9F)]
    public static HResult NS_E_INVALID_INPUT_LANGUAGE => new(0xC00D1B9F);

    /// <summary>The specified source type is not valid.</summary>
    [Description("""The specified source type is not valid.""")]
    [HResultConstant(0xC00D1BA0)]
    public static HResult NS_E_INVALID_INPUT_STREAM => new(0xC00D1BA0);

    /// <summary>The source must be a mono channel .wav file.</summary>
    [Description("""The source must be a mono channel .wav file.""")]
    [HResultConstant(0xC00D1BA1)]
    public static HResult NS_E_EXPECT_MONO_WAV_INPUT => new(0xC00D1BA1);

    /// <summary>All the source .wav files must have the same format.</summary>
    [Description("""All the source .wav files must have the same format.""")]
    [HResultConstant(0xC00D1BA2)]
    public static HResult NS_E_INPUT_WAVFORMAT_MISMATCH => new(0xC00D1BA2);

    /// <summary>The hard disk being used for temporary storage of content has reached the minimum allowed disk space. Create more space on the hard disk and restart encoding.</summary>
    [Description("""The hard disk being used for temporary storage of content has reached the minimum allowed disk space. Create more space on the hard disk and restart encoding.""")]
    [HResultConstant(0xC00D1BA3)]
    public static HResult NS_E_RECORDQ_DISK_FULL => new(0xC00D1BA3);

    /// <summary>It is not possible to apply the inverse telecine feature to PAL content.</summary>
    [Description("""It is not possible to apply the inverse telecine feature to PAL content.""")]
    [HResultConstant(0xC00D1BA4)]
    public static HResult NS_E_NO_PAL_INVERSE_TELECINE => new(0xC00D1BA4);

    /// <summary>A capture device in the current active source is no longer available.</summary>
    [Description("""A capture device in the current active source is no longer available.""")]
    [HResultConstant(0xC00D1BA5)]
    public static HResult NS_E_ACTIVE_SG_DEVICE_DISCONNECTED => new(0xC00D1BA5);

    /// <summary>A device used in the current active source for device control is no longer available.</summary>
    [Description("""A device used in the current active source for device control is no longer available.""")]
    [HResultConstant(0xC00D1BA6)]
    public static HResult NS_E_ACTIVE_SG_DEVICE_CONTROL_DISCONNECTED => new(0xC00D1BA6);

    /// <summary>No frames have been submitted to the analyzer for analysis.</summary>
    [Description("""No frames have been submitted to the analyzer for analysis.""")]
    [HResultConstant(0xC00D1BA7)]
    public static HResult NS_E_NO_FRAMES_SUBMITTED_TO_ANALYZER => new(0xC00D1BA7);

    /// <summary>The source video does not support time codes.</summary>
    [Description("""The source video does not support time codes.""")]
    [HResultConstant(0xC00D1BA8)]
    public static HResult NS_E_INPUT_DOESNOT_SUPPORT_SMPTE => new(0xC00D1BA8);

    /// <summary>It is not possible to generate a time code when there are multiple sources in a session.</summary>
    [Description("""It is not possible to generate a time code when there are multiple sources in a session.""")]
    [HResultConstant(0xC00D1BA9)]
    public static HResult NS_E_NO_SMPTE_WITH_MULTIPLE_SOURCEGROUPS => new(0xC00D1BA9);

    /// <summary>The voice codec optimization definition file cannot be found or is corrupted.</summary>
    [Description("""The voice codec optimization definition file cannot be found or is corrupted.""")]
    [HResultConstant(0xC00D1BAA)]
    public static HResult NS_E_BAD_CONTENTEDL => new(0xC00D1BAA);

    /// <summary>The same source index in different audiences should have the same interlace mode.</summary>
    [Description("""The same source index in different audiences should have the same interlace mode.""")]
    [HResultConstant(0xC00D1BAB)]
    public static HResult NS_E_INTERLACEMODE_MISMATCH => new(0xC00D1BAB);

    /// <summary>The same source index in different audiences should have the same nonsquare pixel mode.</summary>
    [Description("""The same source index in different audiences should have the same nonsquare pixel mode.""")]
    [HResultConstant(0xC00D1BAC)]
    public static HResult NS_E_NONSQUAREPIXELMODE_MISMATCH => new(0xC00D1BAC);

    /// <summary>The same source index in different audiences should have the same time code mode.</summary>
    [Description("""The same source index in different audiences should have the same time code mode.""")]
    [HResultConstant(0xC00D1BAD)]
    public static HResult NS_E_SMPTEMODE_MISMATCH => new(0xC00D1BAD);

    /// <summary>Either the end of the tape has been reached or there is no tape. Check the device and tape.</summary>
    [Description("""Either the end of the tape has been reached or there is no tape. Check the device and tape.""")]
    [HResultConstant(0xC00D1BAE)]
    public static HResult NS_E_END_OF_TAPE => new(0xC00D1BAE);

    /// <summary>No audio or video input has been specified.</summary>
    [Description("""No audio or video input has been specified.""")]
    [HResultConstant(0xC00D1BAF)]
    public static HResult NS_E_NO_MEDIA_IN_AUDIENCE => new(0xC00D1BAF);

    /// <summary>The profile must contain a bit rate.</summary>
    [Description("""The profile must contain a bit rate.""")]
    [HResultConstant(0xC00D1BB0)]
    public static HResult NS_E_NO_AUDIENCES => new(0xC00D1BB0);

    /// <summary>You must specify at least one audio stream to be compatible with Windows Media Player 7.1.</summary>
    [Description("""You must specify at least one audio stream to be compatible with Windows Media Player 7.1.""")]
    [HResultConstant(0xC00D1BB1)]
    public static HResult NS_E_NO_AUDIO_COMPAT => new(0xC00D1BB1);

    /// <summary>Using a VBR encoding mode is not compatible with Windows Media Player 7.1.</summary>
    [Description("""Using a VBR encoding mode is not compatible with Windows Media Player 7.1.""")]
    [HResultConstant(0xC00D1BB2)]
    public static HResult NS_E_INVALID_VBR_COMPAT => new(0xC00D1BB2);

    /// <summary>You must specify a profile name.</summary>
    [Description("""You must specify a profile name.""")]
    [HResultConstant(0xC00D1BB3)]
    public static HResult NS_E_NO_PROFILE_NAME => new(0xC00D1BB3);

    /// <summary>It is not possible to use a VBR encoding mode with uncompressed audio or video.</summary>
    [Description("""It is not possible to use a VBR encoding mode with uncompressed audio or video.""")]
    [HResultConstant(0xC00D1BB4)]
    public static HResult NS_E_INVALID_VBR_WITH_UNCOMP => new(0xC00D1BB4);

    /// <summary>It is not possible to use MBR encoding with VBR encoding.</summary>
    [Description("""It is not possible to use MBR encoding with VBR encoding.""")]
    [HResultConstant(0xC00D1BB5)]
    public static HResult NS_E_MULTIPLE_VBR_AUDIENCES => new(0xC00D1BB5);

    /// <summary>It is not possible to mix uncompressed and compressed content in a session.</summary>
    [Description("""It is not possible to mix uncompressed and compressed content in a session.""")]
    [HResultConstant(0xC00D1BB6)]
    public static HResult NS_E_UNCOMP_COMP_COMBINATION => new(0xC00D1BB6);

    /// <summary>All audiences must use the same audio codec.</summary>
    [Description("""All audiences must use the same audio codec.""")]
    [HResultConstant(0xC00D1BB7)]
    public static HResult NS_E_MULTIPLE_AUDIO_CODECS => new(0xC00D1BB7);

    /// <summary>All audiences should use the same audio format to be compatible with Windows Media Player 7.1.</summary>
    [Description("""All audiences should use the same audio format to be compatible with Windows Media Player 7.1.""")]
    [HResultConstant(0xC00D1BB8)]
    public static HResult NS_E_MULTIPLE_AUDIO_FORMATS => new(0xC00D1BB8);

    /// <summary>The audio bit rate for an audience with a higher total bit rate must be greater than one with a lower total bit rate.</summary>
    [Description("""The audio bit rate for an audience with a higher total bit rate must be greater than one with a lower total bit rate.""")]
    [HResultConstant(0xC00D1BB9)]
    public static HResult NS_E_AUDIO_BITRATE_STEPDOWN => new(0xC00D1BB9);

    /// <summary>The audio peak bit rate setting is not valid.</summary>
    [Description("""The audio peak bit rate setting is not valid.""")]
    [HResultConstant(0xC00D1BBA)]
    public static HResult NS_E_INVALID_AUDIO_PEAKRATE => new(0xC00D1BBA);

    /// <summary>The audio peak bit rate setting must be greater than the audio bit rate setting.</summary>
    [Description("""The audio peak bit rate setting must be greater than the audio bit rate setting.""")]
    [HResultConstant(0xC00D1BBB)]
    public static HResult NS_E_INVALID_AUDIO_PEAKRATE_2 => new(0xC00D1BBB);

    /// <summary>The setting for the maximum buffer size for audio is not valid.</summary>
    [Description("""The setting for the maximum buffer size for audio is not valid.""")]
    [HResultConstant(0xC00D1BBC)]
    public static HResult NS_E_INVALID_AUDIO_BUFFERMAX => new(0xC00D1BBC);

    /// <summary>All audiences must use the same video codec.</summary>
    [Description("""All audiences must use the same video codec.""")]
    [HResultConstant(0xC00D1BBD)]
    public static HResult NS_E_MULTIPLE_VIDEO_CODECS => new(0xC00D1BBD);

    /// <summary>All audiences should use the same video size to be compatible with Windows Media Player 7.1.</summary>
    [Description("""All audiences should use the same video size to be compatible with Windows Media Player 7.1.""")]
    [HResultConstant(0xC00D1BBE)]
    public static HResult NS_E_MULTIPLE_VIDEO_SIZES => new(0xC00D1BBE);

    /// <summary>The video bit rate setting is not valid.</summary>
    [Description("""The video bit rate setting is not valid.""")]
    [HResultConstant(0xC00D1BBF)]
    public static HResult NS_E_INVALID_VIDEO_BITRATE => new(0xC00D1BBF);

    /// <summary>The video bit rate for an audience with a higher total bit rate must be greater than one with a lower total bit rate.</summary>
    [Description("""The video bit rate for an audience with a higher total bit rate must be greater than one with a lower total bit rate.""")]
    [HResultConstant(0xC00D1BC0)]
    public static HResult NS_E_VIDEO_BITRATE_STEPDOWN => new(0xC00D1BC0);

    /// <summary>The video peak bit rate setting is not valid.</summary>
    [Description("""The video peak bit rate setting is not valid.""")]
    [HResultConstant(0xC00D1BC1)]
    public static HResult NS_E_INVALID_VIDEO_PEAKRATE => new(0xC00D1BC1);

    /// <summary>The video peak bit rate setting must be greater than the video bit rate setting.</summary>
    [Description("""The video peak bit rate setting must be greater than the video bit rate setting.""")]
    [HResultConstant(0xC00D1BC2)]
    public static HResult NS_E_INVALID_VIDEO_PEAKRATE_2 => new(0xC00D1BC2);

    /// <summary>The video width setting is not valid.</summary>
    [Description("""The video width setting is not valid.""")]
    [HResultConstant(0xC00D1BC3)]
    public static HResult NS_E_INVALID_VIDEO_WIDTH => new(0xC00D1BC3);

    /// <summary>The video height setting is not valid.</summary>
    [Description("""The video height setting is not valid.""")]
    [HResultConstant(0xC00D1BC4)]
    public static HResult NS_E_INVALID_VIDEO_HEIGHT => new(0xC00D1BC4);

    /// <summary>The video frame rate setting is not valid.</summary>
    [Description("""The video frame rate setting is not valid.""")]
    [HResultConstant(0xC00D1BC5)]
    public static HResult NS_E_INVALID_VIDEO_FPS => new(0xC00D1BC5);

    /// <summary>The video key frame setting is not valid.</summary>
    [Description("""The video key frame setting is not valid.""")]
    [HResultConstant(0xC00D1BC6)]
    public static HResult NS_E_INVALID_VIDEO_KEYFRAME => new(0xC00D1BC6);

    /// <summary>The video image quality setting is not valid.</summary>
    [Description("""The video image quality setting is not valid.""")]
    [HResultConstant(0xC00D1BC7)]
    public static HResult NS_E_INVALID_VIDEO_IQUALITY => new(0xC00D1BC7);

    /// <summary>The video codec quality setting is not valid.</summary>
    [Description("""The video codec quality setting is not valid.""")]
    [HResultConstant(0xC00D1BC8)]
    public static HResult NS_E_INVALID_VIDEO_CQUALITY => new(0xC00D1BC8);

    /// <summary>The video buffer setting is not valid.</summary>
    [Description("""The video buffer setting is not valid.""")]
    [HResultConstant(0xC00D1BC9)]
    public static HResult NS_E_INVALID_VIDEO_BUFFER => new(0xC00D1BC9);

    /// <summary>The setting for the maximum buffer size for video is not valid.</summary>
    [Description("""The setting for the maximum buffer size for video is not valid.""")]
    [HResultConstant(0xC00D1BCA)]
    public static HResult NS_E_INVALID_VIDEO_BUFFERMAX => new(0xC00D1BCA);

    /// <summary>The value of the video maximum buffer size setting must be greater than the video buffer size setting.</summary>
    [Description("""The value of the video maximum buffer size setting must be greater than the video buffer size setting.""")]
    [HResultConstant(0xC00D1BCB)]
    public static HResult NS_E_INVALID_VIDEO_BUFFERMAX_2 => new(0xC00D1BCB);

    /// <summary>The alignment of the video width is not valid.</summary>
    [Description("""The alignment of the video width is not valid.""")]
    [HResultConstant(0xC00D1BCC)]
    public static HResult NS_E_INVALID_VIDEO_WIDTH_ALIGN => new(0xC00D1BCC);

    /// <summary>The alignment of the video height is not valid.</summary>
    [Description("""The alignment of the video height is not valid.""")]
    [HResultConstant(0xC00D1BCD)]
    public static HResult NS_E_INVALID_VIDEO_HEIGHT_ALIGN => new(0xC00D1BCD);

    /// <summary>All bit rates must have the same script bit rate.</summary>
    [Description("""All bit rates must have the same script bit rate.""")]
    [HResultConstant(0xC00D1BCE)]
    public static HResult NS_E_MULTIPLE_SCRIPT_BITRATES => new(0xC00D1BCE);

    /// <summary>The script bit rate specified is not valid.</summary>
    [Description("""The script bit rate specified is not valid.""")]
    [HResultConstant(0xC00D1BCF)]
    public static HResult NS_E_INVALID_SCRIPT_BITRATE => new(0xC00D1BCF);

    /// <summary>All bit rates must have the same file transfer bit rate.</summary>
    [Description("""All bit rates must have the same file transfer bit rate.""")]
    [HResultConstant(0xC00D1BD0)]
    public static HResult NS_E_MULTIPLE_FILE_BITRATES => new(0xC00D1BD0);

    /// <summary>The file transfer bit rate is not valid.</summary>
    [Description("""The file transfer bit rate is not valid.""")]
    [HResultConstant(0xC00D1BD1)]
    public static HResult NS_E_INVALID_FILE_BITRATE => new(0xC00D1BD1);

    /// <summary>All audiences in a profile should either be same as input or have video width and height specified.</summary>
    [Description("""All audiences in a profile should either be same as input or have video width and height specified.""")]
    [HResultConstant(0xC00D1BD2)]
    public static HResult NS_E_SAME_AS_INPUT_COMBINATION => new(0xC00D1BD2);

    /// <summary>This source type does not support looping.</summary>
    [Description("""This source type does not support looping.""")]
    [HResultConstant(0xC00D1BD3)]
    public static HResult NS_E_SOURCE_CANNOT_LOOP => new(0xC00D1BD3);

    /// <summary>The fold-down value needs to be between -144 and 0.</summary>
    [Description("""The fold-down value needs to be between -144 and 0.""")]
    [HResultConstant(0xC00D1BD4)]
    public static HResult NS_E_INVALID_FOLDDOWN_COEFFICIENTS => new(0xC00D1BD4);

    /// <summary>The specified DRM profile does not exist in the system.</summary>
    [Description("""The specified DRM profile does not exist in the system.""")]
    [HResultConstant(0xC00D1BD5)]
    public static HResult NS_E_DRMPROFILE_NOTFOUND => new(0xC00D1BD5);

    /// <summary>The specified time code is not valid.</summary>
    [Description("""The specified time code is not valid.""")]
    [HResultConstant(0xC00D1BD6)]
    public static HResult NS_E_INVALID_TIMECODE => new(0xC00D1BD6);

    /// <summary>It is not possible to apply time compression to a video-only session.</summary>
    [Description("""It is not possible to apply time compression to a video-only session.""")]
    [HResultConstant(0xC00D1BD7)]
    public static HResult NS_E_NO_AUDIO_TIMECOMPRESSION => new(0xC00D1BD7);

    /// <summary>It is not possible to apply time compression to a session that is using two-pass encoding.</summary>
    [Description("""It is not possible to apply time compression to a session that is using two-pass encoding.""")]
    [HResultConstant(0xC00D1BD8)]
    public static HResult NS_E_NO_TWOPASS_TIMECOMPRESSION => new(0xC00D1BD8);

    /// <summary>It is not possible to generate a time code for an audio-only session.</summary>
    [Description("""It is not possible to generate a time code for an audio-only session.""")]
    [HResultConstant(0xC00D1BD9)]
    public static HResult NS_E_TIMECODE_REQUIRES_VIDEOSTREAM => new(0xC00D1BD9);

    /// <summary>It is not possible to generate a time code when you are encoding content at multiple bit rates.</summary>
    [Description("""It is not possible to generate a time code when you are encoding content at multiple bit rates.""")]
    [HResultConstant(0xC00D1BDA)]
    public static HResult NS_E_NO_MBR_WITH_TIMECODE => new(0xC00D1BDA);

    /// <summary>The video codec selected does not support maintaining interlacing in video.</summary>
    [Description("""The video codec selected does not support maintaining interlacing in video.""")]
    [HResultConstant(0xC00D1BDB)]
    public static HResult NS_E_INVALID_INTERLACEMODE => new(0xC00D1BDB);

    /// <summary>Maintaining interlacing in video is not compatible with Windows Media Player 7.1.</summary>
    [Description("""Maintaining interlacing in video is not compatible with Windows Media Player 7.1.""")]
    [HResultConstant(0xC00D1BDC)]
    public static HResult NS_E_INVALID_INTERLACE_COMPAT => new(0xC00D1BDC);

    /// <summary>Allowing nonsquare pixel output is not compatible with Windows Media Player 7.1.</summary>
    [Description("""Allowing nonsquare pixel output is not compatible with Windows Media Player 7.1.""")]
    [HResultConstant(0xC00D1BDD)]
    public static HResult NS_E_INVALID_NONSQUAREPIXEL_COMPAT => new(0xC00D1BDD);

    /// <summary>Only capture devices can be used with device control.</summary>
    [Description("""Only capture devices can be used with device control.""")]
    [HResultConstant(0xC00D1BDE)]
    public static HResult NS_E_INVALID_SOURCE_WITH_DEVICE_CONTROL => new(0xC00D1BDE);

    /// <summary>It is not possible to generate the stream format file if you are using quality-based VBR encoding for the audio or video stream. Instead use the Windows Media file generated after encoding to create the announcement file.</summary>
    [Description("""It is not possible to generate the stream format file if you are using quality-based VBR encoding for the audio or video stream. Instead use the Windows Media file generated after encoding to create the announcement file.""")]
    [HResultConstant(0xC00D1BDF)]
    public static HResult NS_E_CANNOT_GENERATE_BROADCAST_INFO_FOR_QUALITYVBR => new(0xC00D1BDF);

    /// <summary>It is not possible to create a DRM profile because the maximum number of profiles has been reached. You must delete some DRM profiles before creating new ones.</summary>
    [Description("""It is not possible to create a DRM profile because the maximum number of profiles has been reached. You must delete some DRM profiles before creating new ones.""")]
    [HResultConstant(0xC00D1BE0)]
    public static HResult NS_E_EXCEED_MAX_DRM_PROFILE_LIMIT => new(0xC00D1BE0);

    /// <summary>The device is in an unstable state. Check that the device is functioning properly and a tape is in place.</summary>
    [Description("""The device is in an unstable state. Check that the device is functioning properly and a tape is in place.""")]
    [HResultConstant(0xC00D1BE1)]
    public static HResult NS_E_DEVICECONTROL_UNSTABLE => new(0xC00D1BE1);

    /// <summary>The pixel aspect ratio value must be between 1 and 255.</summary>
    [Description("""The pixel aspect ratio value must be between 1 and 255.""")]
    [HResultConstant(0xC00D1BE2)]
    public static HResult NS_E_INVALID_PIXEL_ASPECT_RATIO => new(0xC00D1BE2);

    /// <summary>All streams with different languages in the same audience must have same properties.</summary>
    [Description("""All streams with different languages in the same audience must have same properties.""")]
    [HResultConstant(0xC00D1BE3)]
    public static HResult NS_E_AUDIENCE__LANGUAGE_CONTENTTYPE_MISMATCH => new(0xC00D1BE3);

    /// <summary>The profile must contain at least one audio or video stream.</summary>
    [Description("""The profile must contain at least one audio or video stream.""")]
    [HResultConstant(0xC00D1BE4)]
    public static HResult NS_E_INVALID_PROFILE_CONTENTTYPE => new(0xC00D1BE4);

    /// <summary>The transform plug-in could not be found.</summary>
    [Description("""The transform plug-in could not be found.""")]
    [HResultConstant(0xC00D1BE5)]
    public static HResult NS_E_TRANSFORM_PLUGIN_NOT_FOUND => new(0xC00D1BE5);

    /// <summary>The transform plug-in is not valid. It might be damaged or you might not have the required permissions to access the plug-in.</summary>
    [Description("""The transform plug-in is not valid. It might be damaged or you might not have the required permissions to access the plug-in.""")]
    [HResultConstant(0xC00D1BE6)]
    public static HResult NS_E_TRANSFORM_PLUGIN_INVALID => new(0xC00D1BE6);

    /// <summary>To use two-pass encoding, you must enable device control and setup an edit decision list (EDL) that has at least one entry.</summary>
    [Description("""To use two-pass encoding, you must enable device control and setup an edit decision list (EDL) that has at least one entry.""")]
    [HResultConstant(0xC00D1BE7)]
    public static HResult NS_E_EDL_REQUIRED_FOR_DEVICE_MULTIPASS => new(0xC00D1BE7);

    /// <summary>When you choose to maintain the interlacing in your video, the output video size must be a multiple of 4.</summary>
    [Description("""When you choose to maintain the interlacing in your video, the output video size must be a multiple of 4.""")]
    [HResultConstant(0xC00D1BE8)]
    public static HResult NS_E_INVALID_VIDEO_WIDTH_FOR_INTERLACED_ENCODING => new(0xC00D1BE8);

    /// <summary>Markin/Markout is unsupported with this source type.</summary>
    [Description("""Markin/Markout is unsupported with this source type.""")]
    [HResultConstant(0xC00D1BE9)]
    public static HResult NS_E_MARKIN_UNSUPPORTED => new(0xC00D1BE9);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact product support for this application.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact product support for this application.""")]
    [HResultConstant(0xC00D2711)]
    public static HResult NS_E_DRM_INVALID_APPLICATION => new(0xC00D2711);

    /// <summary>License storage is not working. Contact Microsoft product support.</summary>
    [Description("""License storage is not working. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2712)]
    public static HResult NS_E_DRM_LICENSE_STORE_ERROR => new(0xC00D2712);

    /// <summary>Secure storage is not working. Contact Microsoft product support.</summary>
    [Description("""Secure storage is not working. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2713)]
    public static HResult NS_E_DRM_SECURE_STORE_ERROR => new(0xC00D2713);

    /// <summary>License acquisition did not work. Acquire a new license or contact the content provider for further assistance.</summary>
    [Description("""License acquisition did not work. Acquire a new license or contact the content provider for further assistance.""")]
    [HResultConstant(0xC00D2714)]
    public static HResult NS_E_DRM_LICENSE_STORE_SAVE_ERROR => new(0xC00D2714);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2715)]
    public static HResult NS_E_DRM_SECURE_STORE_UNLOCK_ERROR => new(0xC00D2715);

    /// <summary>The media file is corrupted. Contact the content provider to get a new file.</summary>
    [Description("""The media file is corrupted. Contact the content provider to get a new file.""")]
    [HResultConstant(0xC00D2716)]
    public static HResult NS_E_DRM_INVALID_CONTENT => new(0xC00D2716);

    /// <summary>The license is corrupted. Acquire a new license.</summary>
    [Description("""The license is corrupted. Acquire a new license.""")]
    [HResultConstant(0xC00D2717)]
    public static HResult NS_E_DRM_UNABLE_TO_OPEN_LICENSE => new(0xC00D2717);

    /// <summary>The license is corrupted or invalid. Acquire a new license</summary>
    [Description("""The license is corrupted or invalid. Acquire a new license""")]
    [HResultConstant(0xC00D2718)]
    public static HResult NS_E_DRM_INVALID_LICENSE => new(0xC00D2718);

    /// <summary>Licenses cannot be copied from one computer to another. Use License Management to transfer licenses, or get a new license for the media file.</summary>
    [Description("""Licenses cannot be copied from one computer to another. Use License Management to transfer licenses, or get a new license for the media file.""")]
    [HResultConstant(0xC00D2719)]
    public static HResult NS_E_DRM_INVALID_MACHINE => new(0xC00D2719);

    /// <summary>License storage is not working. Contact Microsoft product support.</summary>
    [Description("""License storage is not working. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D271B)]
    public static HResult NS_E_DRM_ENUM_LICENSE_FAILED => new(0xC00D271B);

    /// <summary>The media file is corrupted. Contact the content provider to get a new file.</summary>
    [Description("""The media file is corrupted. Contact the content provider to get a new file.""")]
    [HResultConstant(0xC00D271C)]
    public static HResult NS_E_DRM_INVALID_LICENSE_REQUEST => new(0xC00D271C);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D271D)]
    public static HResult NS_E_DRM_UNABLE_TO_INITIALIZE => new(0xC00D271D);

    /// <summary>The license could not be acquired. Try again later.</summary>
    [Description("""The license could not be acquired. Try again later.""")]
    [HResultConstant(0xC00D271E)]
    public static HResult NS_E_DRM_UNABLE_TO_ACQUIRE_LICENSE => new(0xC00D271E);

    /// <summary>License acquisition did not work. Acquire a new license or contact the content provider for further assistance.</summary>
    [Description("""License acquisition did not work. Acquire a new license or contact the content provider for further assistance.""")]
    [HResultConstant(0xC00D271F)]
    public static HResult NS_E_DRM_INVALID_LICENSE_ACQUIRED => new(0xC00D271F);

    /// <summary>The requested operation cannot be performed on this file.</summary>
    [Description("""The requested operation cannot be performed on this file.""")]
    [HResultConstant(0xC00D2720)]
    public static HResult NS_E_DRM_NO_RIGHTS => new(0xC00D2720);

    /// <summary>The requested action cannot be performed because a problem occurred with the Windows Media Digital Rights Management (DRM) components on your computer.</summary>
    [Description("""The requested action cannot be performed because a problem occurred with the Windows Media Digital Rights Management (DRM) components on your computer.""")]
    [HResultConstant(0xC00D2721)]
    public static HResult NS_E_DRM_KEY_ERROR => new(0xC00D2721);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2722)]
    public static HResult NS_E_DRM_ENCRYPT_ERROR => new(0xC00D2722);

    /// <summary>The media file is corrupted. Contact the content provider to get a new file.</summary>
    [Description("""The media file is corrupted. Contact the content provider to get a new file.""")]
    [HResultConstant(0xC00D2723)]
    public static HResult NS_E_DRM_DECRYPT_ERROR => new(0xC00D2723);

    /// <summary>The license is corrupted. Acquire a new license.</summary>
    [Description("""The license is corrupted. Acquire a new license.""")]
    [HResultConstant(0xC00D2725)]
    public static HResult NS_E_DRM_LICENSE_INVALID_XML => new(0xC00D2725);

    /// <summary>A security upgrade is required to perform the operation on this media file.</summary>
    [Description("""A security upgrade is required to perform the operation on this media file.""")]
    [HResultConstant(0xC00D2728)]
    public static HResult NS_E_DRM_NEEDS_INDIVIDUALIZATION => new(0xC00D2728);

    /// <summary>You already have the latest security components. No upgrade is necessary at this time.</summary>
    [Description("""You already have the latest security components. No upgrade is necessary at this time.""")]
    [HResultConstant(0xC00D2729)]
    public static HResult NS_E_DRM_ALREADY_INDIVIDUALIZED => new(0xC00D2729);

    /// <summary>The application cannot perform this action. Contact product support for this application.</summary>
    [Description("""The application cannot perform this action. Contact product support for this application.""")]
    [HResultConstant(0xC00D272A)]
    public static HResult NS_E_DRM_ACTION_NOT_QUERIED => new(0xC00D272A);

    /// <summary>You cannot begin a new license acquisition process until the current one has been completed.</summary>
    [Description("""You cannot begin a new license acquisition process until the current one has been completed.""")]
    [HResultConstant(0xC00D272B)]
    public static HResult NS_E_DRM_ACQUIRING_LICENSE => new(0xC00D272B);

    /// <summary>You cannot begin a new security upgrade until the current one has been completed.</summary>
    [Description("""You cannot begin a new security upgrade until the current one has been completed.""")]
    [HResultConstant(0xC00D272C)]
    public static HResult NS_E_DRM_INDIVIDUALIZING => new(0xC00D272C);

    /// <summary>Failure in Backup-Restore.</summary>
    [Description("""Failure in Backup-Restore.""")]
    [HResultConstant(0xC00D272D)]
    public static HResult NS_E_BACKUP_RESTORE_FAILURE => new(0xC00D272D);

    /// <summary>Bad Request ID in Backup-Restore.</summary>
    [Description("""Bad Request ID in Backup-Restore.""")]
    [HResultConstant(0xC00D272E)]
    public static HResult NS_E_BACKUP_RESTORE_BAD_REQUEST_ID => new(0xC00D272E);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D272F)]
    public static HResult NS_E_DRM_PARAMETERS_MISMATCHED => new(0xC00D272F);

    /// <summary>A license cannot be created for this media file. Reinstall the application.</summary>
    [Description("""A license cannot be created for this media file. Reinstall the application.""")]
    [HResultConstant(0xC00D2730)]
    public static HResult NS_E_DRM_UNABLE_TO_CREATE_LICENSE_OBJECT => new(0xC00D2730);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2731)]
    public static HResult NS_E_DRM_UNABLE_TO_CREATE_INDI_OBJECT => new(0xC00D2731);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2732)]
    public static HResult NS_E_DRM_UNABLE_TO_CREATE_ENCRYPT_OBJECT => new(0xC00D2732);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2733)]
    public static HResult NS_E_DRM_UNABLE_TO_CREATE_DECRYPT_OBJECT => new(0xC00D2733);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2734)]
    public static HResult NS_E_DRM_UNABLE_TO_CREATE_PROPERTIES_OBJECT => new(0xC00D2734);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2735)]
    public static HResult NS_E_DRM_UNABLE_TO_CREATE_BACKUP_OBJECT => new(0xC00D2735);

    /// <summary>The security upgrade failed. Try again later.</summary>
    [Description("""The security upgrade failed. Try again later.""")]
    [HResultConstant(0xC00D2736)]
    public static HResult NS_E_DRM_INDIVIDUALIZE_ERROR => new(0xC00D2736);

    /// <summary>License storage is not working. Contact Microsoft product support.</summary>
    [Description("""License storage is not working. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2737)]
    public static HResult NS_E_DRM_LICENSE_OPEN_ERROR => new(0xC00D2737);

    /// <summary>License storage is not working. Contact Microsoft product support.</summary>
    [Description("""License storage is not working. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2738)]
    public static HResult NS_E_DRM_LICENSE_CLOSE_ERROR => new(0xC00D2738);

    /// <summary>License storage is not working. Contact Microsoft product support.</summary>
    [Description("""License storage is not working. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2739)]
    public static HResult NS_E_DRM_GET_LICENSE_ERROR => new(0xC00D2739);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D273A)]
    public static HResult NS_E_DRM_QUERY_ERROR => new(0xC00D273A);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact product support for this application.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact product support for this application.""")]
    [HResultConstant(0xC00D273B)]
    public static HResult NS_E_DRM_REPORT_ERROR => new(0xC00D273B);

    /// <summary>License storage is not working. Contact Microsoft product support.</summary>
    [Description("""License storage is not working. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D273C)]
    public static HResult NS_E_DRM_GET_LICENSESTRING_ERROR => new(0xC00D273C);

    /// <summary>The media file is corrupted. Contact the content provider to get a new file.</summary>
    [Description("""The media file is corrupted. Contact the content provider to get a new file.""")]
    [HResultConstant(0xC00D273D)]
    public static HResult NS_E_DRM_GET_CONTENTSTRING_ERROR => new(0xC00D273D);

    /// <summary>A problem has occurred in the Digital Rights Management component. Try again later.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Try again later.""")]
    [HResultConstant(0xC00D273E)]
    public static HResult NS_E_DRM_MONITOR_ERROR => new(0xC00D273E);

    /// <summary>The application has made an invalid call to the Digital Rights Management component. Contact product support for this application.</summary>
    [Description("""The application has made an invalid call to the Digital Rights Management component. Contact product support for this application.""")]
    [HResultConstant(0xC00D273F)]
    public static HResult NS_E_DRM_UNABLE_TO_SET_PARAMETER => new(0xC00D273F);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2740)]
    public static HResult NS_E_DRM_INVALID_APPDATA => new(0xC00D2740);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact product support for this application.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact product support for this application.""")]
    [HResultConstant(0xC00D2741)]
    public static HResult NS_E_DRM_INVALID_APPDATA_VERSION => new(0xC00D2741);

    /// <summary>Licenses are already backed up in this location.</summary>
    [Description("""Licenses are already backed up in this location.""")]
    [HResultConstant(0xC00D2742)]
    public static HResult NS_E_DRM_BACKUP_EXISTS => new(0xC00D2742);

    /// <summary>One or more backed-up licenses are missing or corrupt.</summary>
    [Description("""One or more backed-up licenses are missing or corrupt.""")]
    [HResultConstant(0xC00D2743)]
    public static HResult NS_E_DRM_BACKUP_CORRUPT => new(0xC00D2743);

    /// <summary>You cannot begin a new backup process until the current process has been completed.</summary>
    [Description("""You cannot begin a new backup process until the current process has been completed.""")]
    [HResultConstant(0xC00D2744)]
    public static HResult NS_E_DRM_BACKUPRESTORE_BUSY => new(0xC00D2744);

    /// <summary>Bad Data sent to Backup-Restore.</summary>
    [Description("""Bad Data sent to Backup-Restore.""")]
    [HResultConstant(0xC00D2745)]
    public static HResult NS_E_BACKUP_RESTORE_BAD_DATA => new(0xC00D2745);

    /// <summary>The license is invalid. Contact the content provider for further assistance.</summary>
    [Description("""The license is invalid. Contact the content provider for further assistance.""")]
    [HResultConstant(0xC00D2748)]
    public static HResult NS_E_DRM_LICENSE_UNUSABLE => new(0xC00D2748);

    /// <summary>A required property was not set by the application. Contact product support for this application.</summary>
    [Description("""A required property was not set by the application. Contact product support for this application.""")]
    [HResultConstant(0xC00D2749)]
    public static HResult NS_E_DRM_INVALID_PROPERTY => new(0xC00D2749);

    /// <summary>A problem has occurred in the Digital Rights Management component of this application. Try to acquire a license again.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component of this application. Try to acquire a license again.""")]
    [HResultConstant(0xC00D274A)]
    public static HResult NS_E_DRM_SECURE_STORE_NOT_FOUND => new(0xC00D274A);

    /// <summary>A license cannot be found for this media file. Use License Management to transfer a license for this file from the original computer, or acquire a new license.</summary>
    [Description("""A license cannot be found for this media file. Use License Management to transfer a license for this file from the original computer, or acquire a new license.""")]
    [HResultConstant(0xC00D274B)]
    public static HResult NS_E_DRM_CACHED_CONTENT_ERROR => new(0xC00D274B);

    /// <summary>A problem occurred during the security upgrade. Try again later.</summary>
    [Description("""A problem occurred during the security upgrade. Try again later.""")]
    [HResultConstant(0xC00D274C)]
    public static HResult NS_E_DRM_INDIVIDUALIZATION_INCOMPLETE => new(0xC00D274C);

    /// <summary>Certified driver components are required to play this media file. Contact Windows Update to see whether updated drivers are available for your hardware.</summary>
    [Description("""Certified driver components are required to play this media file. Contact Windows Update to see whether updated drivers are available for your hardware.""")]
    [HResultConstant(0xC00D274D)]
    public static HResult NS_E_DRM_DRIVER_AUTH_FAILURE => new(0xC00D274D);

    /// <summary>One or more of the Secure Audio Path components were not found or an entry point in those components was not found.</summary>
    [Description("""One or more of the Secure Audio Path components were not found or an entry point in those components was not found.""")]
    [HResultConstant(0xC00D274E)]
    public static HResult NS_E_DRM_NEED_UPGRADE_MSSAP => new(0xC00D274E);

    /// <summary>Status message: Reopen the file.</summary>
    [Description("""Status message: Reopen the file.""")]
    [HResultConstant(0xC00D274F)]
    public static HResult NS_E_DRM_REOPEN_CONTENT => new(0xC00D274F);

    /// <summary>Certain driver functionality is required to play this media file. Contact Windows Update to see whether updated drivers are available for your hardware.</summary>
    [Description("""Certain driver functionality is required to play this media file. Contact Windows Update to see whether updated drivers are available for your hardware.""")]
    [HResultConstant(0xC00D2750)]
    public static HResult NS_E_DRM_DRIVER_DIGIOUT_FAILURE => new(0xC00D2750);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2751)]
    public static HResult NS_E_DRM_INVALID_SECURESTORE_PASSWORD => new(0xC00D2751);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2752)]
    public static HResult NS_E_DRM_APPCERT_REVOKED => new(0xC00D2752);

    /// <summary>You cannot restore your license(s).</summary>
    [Description("""You cannot restore your license(s).""")]
    [HResultConstant(0xC00D2753)]
    public static HResult NS_E_DRM_RESTORE_FRAUD => new(0xC00D2753);

    /// <summary>The licenses for your media files are corrupted. Contact Microsoft product support.</summary>
    [Description("""The licenses for your media files are corrupted. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2754)]
    public static HResult NS_E_DRM_HARDWARE_INCONSISTENT => new(0xC00D2754);

    /// <summary>To transfer this media file, you must upgrade the application.</summary>
    [Description("""To transfer this media file, you must upgrade the application.""")]
    [HResultConstant(0xC00D2755)]
    public static HResult NS_E_DRM_SDMI_TRIGGER => new(0xC00D2755);

    /// <summary>You cannot make any more copies of this media file.</summary>
    [Description("""You cannot make any more copies of this media file.""")]
    [HResultConstant(0xC00D2756)]
    public static HResult NS_E_DRM_SDMI_NOMORECOPIES => new(0xC00D2756);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2757)]
    public static HResult NS_E_DRM_UNABLE_TO_CREATE_HEADER_OBJECT => new(0xC00D2757);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2758)]
    public static HResult NS_E_DRM_UNABLE_TO_CREATE_KEYS_OBJECT => new(0xC00D2758);

    /// <summary>Unable to obtain license.</summary>
    [Description("""Unable to obtain license.""")]
    [HResultConstant(0xC00D2759)]
    public static HResult NS_E_DRM_LICENSE_NOTACQUIRED => new(0xC00D2759);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D275A)]
    public static HResult NS_E_DRM_UNABLE_TO_CREATE_CODING_OBJECT => new(0xC00D275A);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D275B)]
    public static HResult NS_E_DRM_UNABLE_TO_CREATE_STATE_DATA_OBJECT => new(0xC00D275B);

    /// <summary>The buffer supplied is not sufficient.</summary>
    [Description("""The buffer supplied is not sufficient.""")]
    [HResultConstant(0xC00D275C)]
    public static HResult NS_E_DRM_BUFFER_TOO_SMALL => new(0xC00D275C);

    /// <summary>The property requested is not supported.</summary>
    [Description("""The property requested is not supported.""")]
    [HResultConstant(0xC00D275D)]
    public static HResult NS_E_DRM_UNSUPPORTED_PROPERTY => new(0xC00D275D);

    /// <summary>The specified server cannot perform the requested operation.</summary>
    [Description("""The specified server cannot perform the requested operation.""")]
    [HResultConstant(0xC00D275E)]
    public static HResult NS_E_DRM_ERROR_BAD_NET_RESP => new(0xC00D275E);

    /// <summary>Some of the licenses could not be stored.</summary>
    [Description("""Some of the licenses could not be stored.""")]
    [HResultConstant(0xC00D275F)]
    public static HResult NS_E_DRM_STORE_NOTALLSTORED => new(0xC00D275F);

    /// <summary>The Digital Rights Management security upgrade component could not be validated. Contact Microsoft product support.</summary>
    [Description("""The Digital Rights Management security upgrade component could not be validated. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2760)]
    public static HResult NS_E_DRM_SECURITY_COMPONENT_SIGNATURE_INVALID => new(0xC00D2760);

    /// <summary>Invalid or corrupt data was encountered.</summary>
    [Description("""Invalid or corrupt data was encountered.""")]
    [HResultConstant(0xC00D2761)]
    public static HResult NS_E_DRM_INVALID_DATA => new(0xC00D2761);

    /// <summary>The Windows Media Digital Rights Management system cannot perform the requested action because your computer or network administrator has enabled the group policy Prevent Windows Media DRM Internet Access. For assistance, contact your administrator.</summary>
    [Description("""The Windows Media Digital Rights Management system cannot perform the requested action because your computer or network administrator has enabled the group policy Prevent Windows Media DRM Internet Access. For assistance, contact your administrator.""")]
    [HResultConstant(0xC00D2762)]
    public static HResult NS_E_DRM_POLICY_DISABLE_ONLINE => new(0xC00D2762);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2763)]
    public static HResult NS_E_DRM_UNABLE_TO_CREATE_AUTHENTICATION_OBJECT => new(0xC00D2763);

    /// <summary>Not all of the necessary properties for DRM have been set.</summary>
    [Description("""Not all of the necessary properties for DRM have been set.""")]
    [HResultConstant(0xC00D2764)]
    public static HResult NS_E_DRM_NOT_CONFIGURED => new(0xC00D2764);

    /// <summary>The portable device does not have the security required to copy protected files to it. To obtain the additional security, try to copy the file to your portable device again. When a message appears, click OK.</summary>
    [Description("""The portable device does not have the security required to copy protected files to it. To obtain the additional security, try to copy the file to your portable device again. When a message appears, click OK.""")]
    [HResultConstant(0xC00D2765)]
    public static HResult NS_E_DRM_DEVICE_ACTIVATION_CANCELED => new(0xC00D2765);

    /// <summary>Too many resets in Backup-Restore.</summary>
    [Description("""Too many resets in Backup-Restore.""")]
    [HResultConstant(0xC00D2766)]
    public static HResult NS_E_BACKUP_RESTORE_TOO_MANY_RESETS => new(0xC00D2766);

    /// <summary>Running this process under a debugger while using DRM content is not allowed.</summary>
    [Description("""Running this process under a debugger while using DRM content is not allowed.""")]
    [HResultConstant(0xC00D2767)]
    public static HResult NS_E_DRM_DEBUGGING_NOT_ALLOWED => new(0xC00D2767);

    /// <summary>The user canceled the DRM operation.</summary>
    [Description("""The user canceled the DRM operation.""")]
    [HResultConstant(0xC00D2768)]
    public static HResult NS_E_DRM_OPERATION_CANCELED => new(0xC00D2768);

    /// <summary>The license you are using has assocaited output restrictions. This license is unusable until these restrictions are queried.</summary>
    [Description("""The license you are using has assocaited output restrictions. This license is unusable until these restrictions are queried.""")]
    [HResultConstant(0xC00D2769)]
    public static HResult NS_E_DRM_RESTRICTIONS_NOT_RETRIEVED => new(0xC00D2769);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D276A)]
    public static HResult NS_E_DRM_UNABLE_TO_CREATE_PLAYLIST_OBJECT => new(0xC00D276A);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D276B)]
    public static HResult NS_E_DRM_UNABLE_TO_CREATE_PLAYLIST_BURN_OBJECT => new(0xC00D276B);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D276C)]
    public static HResult NS_E_DRM_UNABLE_TO_CREATE_DEVICE_REGISTRATION_OBJECT => new(0xC00D276C);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D276D)]
    public static HResult NS_E_DRM_UNABLE_TO_CREATE_METERING_OBJECT => new(0xC00D276D);

    /// <summary>The specified track has exceeded it's specified playlist burn limit in this playlist.</summary>
    [Description("""The specified track has exceeded it's specified playlist burn limit in this playlist.""")]
    [HResultConstant(0xC00D2770)]
    public static HResult NS_E_DRM_TRACK_EXCEEDED_PLAYLIST_RESTICTION => new(0xC00D2770);

    /// <summary>The specified track has exceeded it's track burn limit.</summary>
    [Description("""The specified track has exceeded it's track burn limit.""")]
    [HResultConstant(0xC00D2771)]
    public static HResult NS_E_DRM_TRACK_EXCEEDED_TRACKBURN_RESTRICTION => new(0xC00D2771);

    /// <summary>A problem has occurred in obtaining the device's certificate. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in obtaining the device's certificate. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2772)]
    public static HResult NS_E_DRM_UNABLE_TO_GET_DEVICE_CERT => new(0xC00D2772);

    /// <summary>A problem has occurred in obtaining the device's secure clock. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in obtaining the device's secure clock. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2773)]
    public static HResult NS_E_DRM_UNABLE_TO_GET_SECURE_CLOCK => new(0xC00D2773);

    /// <summary>A problem has occurred in setting the device's secure clock. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in setting the device's secure clock. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2774)]
    public static HResult NS_E_DRM_UNABLE_TO_SET_SECURE_CLOCK => new(0xC00D2774);

    /// <summary>A problem has occurred in obtaining the secure clock from server. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in obtaining the secure clock from server. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2775)]
    public static HResult NS_E_DRM_UNABLE_TO_GET_SECURE_CLOCK_FROM_SERVER => new(0xC00D2775);

    /// <summary>This content requires the metering policy to be enabled.</summary>
    [Description("""This content requires the metering policy to be enabled.""")]
    [HResultConstant(0xC00D2776)]
    public static HResult NS_E_DRM_POLICY_METERING_DISABLED => new(0xC00D2776);

    /// <summary>Transfer of chained licenses unsupported.</summary>
    [Description("""Transfer of chained licenses unsupported.""")]
    [HResultConstant(0xC00D2777)]
    public static HResult NS_E_DRM_TRANSFER_CHAINED_LICENSES_UNSUPPORTED => new(0xC00D2777);

    /// <summary>The Digital Rights Management component is not installed properly. Reinstall the Player.</summary>
    [Description("""The Digital Rights Management component is not installed properly. Reinstall the Player.""")]
    [HResultConstant(0xC00D2778)]
    public static HResult NS_E_DRM_SDK_VERSIONMISMATCH => new(0xC00D2778);

    /// <summary>The file could not be transferred because the device clock is not set.</summary>
    [Description("""The file could not be transferred because the device clock is not set.""")]
    [HResultConstant(0xC00D2779)]
    public static HResult NS_E_DRM_LIC_NEEDS_DEVICE_CLOCK_SET => new(0xC00D2779);

    /// <summary>The content header is missing an acquisition URL.</summary>
    [Description("""The content header is missing an acquisition URL.""")]
    [HResultConstant(0xC00D277A)]
    public static HResult NS_E_LICENSE_HEADER_MISSING_URL => new(0xC00D277A);

    /// <summary>The current attached device does not support WMDRM.</summary>
    [Description("""The current attached device does not support WMDRM.""")]
    [HResultConstant(0xC00D277B)]
    public static HResult NS_E_DEVICE_NOT_WMDRM_DEVICE => new(0xC00D277B);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D277C)]
    public static HResult NS_E_DRM_INVALID_APPCERT => new(0xC00D277C);

    /// <summary>The client application has been forcefully terminated during a DRM petition.</summary>
    [Description("""The client application has been forcefully terminated during a DRM petition.""")]
    [HResultConstant(0xC00D277D)]
    public static HResult NS_E_DRM_PROTOCOL_FORCEFUL_TERMINATION_ON_PETITION => new(0xC00D277D);

    /// <summary>The client application has been forcefully terminated during a DRM challenge.</summary>
    [Description("""The client application has been forcefully terminated during a DRM challenge.""")]
    [HResultConstant(0xC00D277E)]
    public static HResult NS_E_DRM_PROTOCOL_FORCEFUL_TERMINATION_ON_CHALLENGE => new(0xC00D277E);

    /// <summary>Secure storage protection error. Restore your licenses from a previous backup and try again.</summary>
    [Description("""Secure storage protection error. Restore your licenses from a previous backup and try again.""")]
    [HResultConstant(0xC00D277F)]
    public static HResult NS_E_DRM_CHECKPOINT_FAILED => new(0xC00D277F);

    /// <summary>A problem has occurred in the Digital Rights Management root of trust. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management root of trust. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2780)]
    public static HResult NS_E_DRM_BB_UNABLE_TO_INITIALIZE => new(0xC00D2780);

    /// <summary>A problem has occurred in retrieving the Digital Rights Management machine identification. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in retrieving the Digital Rights Management machine identification. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2781)]
    public static HResult NS_E_DRM_UNABLE_TO_LOAD_HARDWARE_ID => new(0xC00D2781);

    /// <summary>A problem has occurred in opening the Digital Rights Management data storage file. Contact Microsoft product.</summary>
    [Description("""A problem has occurred in opening the Digital Rights Management data storage file. Contact Microsoft product.""")]
    [HResultConstant(0xC00D2782)]
    public static HResult NS_E_DRM_UNABLE_TO_OPEN_DATA_STORE => new(0xC00D2782);

    /// <summary>The Digital Rights Management data storage is not functioning properly. Contact Microsoft product support.</summary>
    [Description("""The Digital Rights Management data storage is not functioning properly. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2783)]
    public static HResult NS_E_DRM_DATASTORE_CORRUPT => new(0xC00D2783);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2784)]
    public static HResult NS_E_DRM_UNABLE_TO_CREATE_INMEMORYSTORE_OBJECT => new(0xC00D2784);

    /// <summary>A secured library is required to access the requested functionality.</summary>
    [Description("""A secured library is required to access the requested functionality.""")]
    [HResultConstant(0xC00D2785)]
    public static HResult NS_E_DRM_STUBLIB_REQUIRED => new(0xC00D2785);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2786)]
    public static HResult NS_E_DRM_UNABLE_TO_CREATE_CERTIFICATE_OBJECT => new(0xC00D2786);

    /// <summary>A problem has occurred in the Digital Rights Management component during license migration. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component during license migration. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2787)]
    public static HResult NS_E_DRM_MIGRATION_TARGET_NOT_ONLINE => new(0xC00D2787);

    /// <summary>A problem has occurred in the Digital Rights Management component during license migration. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component during license migration. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2788)]
    public static HResult NS_E_DRM_INVALID_MIGRATION_IMAGE => new(0xC00D2788);

    /// <summary>A problem has occurred in the Digital Rights Management component during license migration. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component during license migration. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D2789)]
    public static HResult NS_E_DRM_MIGRATION_TARGET_STATES_CORRUPTED => new(0xC00D2789);

    /// <summary>A problem has occurred in the Digital Rights Management component during license migration. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component during license migration. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D278A)]
    public static HResult NS_E_DRM_MIGRATION_IMPORTER_NOT_AVAILABLE => new(0xC00D278A);

    /// <summary>A problem has occurred in the Digital Rights Management component during license migration. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component during license migration. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D278B)]
    public static HResult NS_DRM_E_MIGRATION_UPGRADE_WITH_DIFF_SID => new(0xC00D278B);

    /// <summary>The Digital Rights Management component is in use during license migration. Contact Microsoft product support.</summary>
    [Description("""The Digital Rights Management component is in use during license migration. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D278C)]
    public static HResult NS_DRM_E_MIGRATION_SOURCE_MACHINE_IN_USE => new(0xC00D278C);

    /// <summary>Licenses are being migrated to a machine running XP or downlevel OS. This operation can only be performed on Windows Vista or a later OS. Contact Microsoft product support.</summary>
    [Description("""Licenses are being migrated to a machine running XP or downlevel OS. This operation can only be performed on Windows Vista or a later OS. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D278D)]
    public static HResult NS_DRM_E_MIGRATION_TARGET_MACHINE_LESS_THAN_LH => new(0xC00D278D);

    /// <summary>Migration Image already exists. Contact Microsoft product support.</summary>
    [Description("""Migration Image already exists. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D278E)]
    public static HResult NS_DRM_E_MIGRATION_IMAGE_ALREADY_EXISTS => new(0xC00D278E);

    /// <summary>The requested action cannot be performed because a hardware configuration change has been detected by the Windows Media Digital Rights Management (DRM) components on your computer.</summary>
    [Description("""The requested action cannot be performed because a hardware configuration change has been detected by the Windows Media Digital Rights Management (DRM) components on your computer.""")]
    [HResultConstant(0xC00D278F)]
    public static HResult NS_E_DRM_HARDWAREID_MISMATCH => new(0xC00D278F);

    /// <summary>The wrong stublib has been linked to an application or DLL using drmv2clt.dll.</summary>
    [Description("""The wrong stublib has been linked to an application or DLL using drmv2clt.dll.""")]
    [HResultConstant(0xC00D2790)]
    public static HResult NS_E_INVALID_DRMV2CLT_STUBLIB => new(0xC00D2790);

    /// <summary>The legacy V2 data being imported is invalid.</summary>
    [Description("""The legacy V2 data being imported is invalid.""")]
    [HResultConstant(0xC00D2791)]
    public static HResult NS_E_DRM_MIGRATION_INVALID_LEGACYV2_DATA => new(0xC00D2791);

    /// <summary>The license being imported already exists.</summary>
    [Description("""The license being imported already exists.""")]
    [HResultConstant(0xC00D2792)]
    public static HResult NS_E_DRM_MIGRATION_LICENSE_ALREADY_EXISTS => new(0xC00D2792);

    /// <summary>The password of the Legacy V2 SST entry being imported is incorrect.</summary>
    [Description("""The password of the Legacy V2 SST entry being imported is incorrect.""")]
    [HResultConstant(0xC00D2793)]
    public static HResult NS_E_DRM_MIGRATION_INVALID_LEGACYV2_SST_PASSWORD => new(0xC00D2793);

    /// <summary>Migration is not supported by the plugin.</summary>
    [Description("""Migration is not supported by the plugin.""")]
    [HResultConstant(0xC00D2794)]
    public static HResult NS_E_DRM_MIGRATION_NOT_SUPPORTED => new(0xC00D2794);

    /// <summary>A migration importer cannot be created for this media file. Reinstall the application.</summary>
    [Description("""A migration importer cannot be created for this media file. Reinstall the application.""")]
    [HResultConstant(0xC00D2795)]
    public static HResult NS_E_DRM_UNABLE_TO_CREATE_MIGRATION_IMPORTER_OBJECT => new(0xC00D2795);

    /// <summary>The requested action cannot be performed because a problem occurred with the Windows Media Digital Rights Management (DRM) components on your computer.</summary>
    [Description("""The requested action cannot be performed because a problem occurred with the Windows Media Digital Rights Management (DRM) components on your computer.""")]
    [HResultConstant(0xC00D2796)]
    public static HResult NS_E_DRM_CHECKPOINT_MISMATCH => new(0xC00D2796);

    /// <summary>The requested action cannot be performed because a problem occurred with the Windows Media Digital Rights Management (DRM) components on your computer.</summary>
    [Description("""The requested action cannot be performed because a problem occurred with the Windows Media Digital Rights Management (DRM) components on your computer.""")]
    [HResultConstant(0xC00D2797)]
    public static HResult NS_E_DRM_CHECKPOINT_CORRUPT => new(0xC00D2797);

    /// <summary>The requested action cannot be performed because a problem occurred with the Windows Media Digital Rights Management (DRM) components on your computer.</summary>
    [Description("""The requested action cannot be performed because a problem occurred with the Windows Media Digital Rights Management (DRM) components on your computer.""")]
    [HResultConstant(0xC00D2798)]
    public static HResult NS_E_REG_FLUSH_FAILURE => new(0xC00D2798);

    /// <summary>The requested action cannot be performed because a problem occurred with the Windows Media Digital Rights Management (DRM) components on your computer.</summary>
    [Description("""The requested action cannot be performed because a problem occurred with the Windows Media Digital Rights Management (DRM) components on your computer.""")]
    [HResultConstant(0xC00D2799)]
    public static HResult NS_E_HDS_KEY_MISMATCH => new(0xC00D2799);

    /// <summary>Migration was canceled by the user.</summary>
    [Description("""Migration was canceled by the user.""")]
    [HResultConstant(0xC00D279A)]
    public static HResult NS_E_DRM_MIGRATION_OPERATION_CANCELLED => new(0xC00D279A);

    /// <summary>Migration object is already in use and cannot be called until the current operation completes.</summary>
    [Description("""Migration object is already in use and cannot be called until the current operation completes.""")]
    [HResultConstant(0xC00D279B)]
    public static HResult NS_E_DRM_MIGRATION_OBJECT_IN_USE => new(0xC00D279B);

    /// <summary>The content header does not comply with DRM requirements and cannot be used.</summary>
    [Description("""The content header does not comply with DRM requirements and cannot be used.""")]
    [HResultConstant(0xC00D279C)]
    public static HResult NS_E_DRM_MALFORMED_CONTENT_HEADER => new(0xC00D279C);

    /// <summary>The license for this file has expired and is no longer valid. Contact your content provider for further assistance.</summary>
    [Description("""The license for this file has expired and is no longer valid. Contact your content provider for further assistance.""")]
    [HResultConstant(0xC00D27D8)]
    public static HResult NS_E_DRM_LICENSE_EXPIRED => new(0xC00D27D8);

    /// <summary>The license for this file is not valid yet, but will be at a future date.</summary>
    [Description("""The license for this file is not valid yet, but will be at a future date.""")]
    [HResultConstant(0xC00D27D9)]
    public static HResult NS_E_DRM_LICENSE_NOTENABLED => new(0xC00D27D9);

    /// <summary>The license for this file requires a higher level of security than the player you are currently using has. Try using a different player or download a newer version of your current player.</summary>
    [Description("""The license for this file requires a higher level of security than the player you are currently using has. Try using a different player or download a newer version of your current player.""")]
    [HResultConstant(0xC00D27DA)]
    public static HResult NS_E_DRM_LICENSE_APPSECLOW => new(0xC00D27DA);

    /// <summary>The license cannot be stored as it requires security upgrade of Digital Rights Management component.</summary>
    [Description("""The license cannot be stored as it requires security upgrade of Digital Rights Management component.""")]
    [HResultConstant(0xC00D27DB)]
    public static HResult NS_E_DRM_STORE_NEEDINDI => new(0xC00D27DB);

    /// <summary>Your machine does not meet the requirements for storing the license.</summary>
    [Description("""Your machine does not meet the requirements for storing the license.""")]
    [HResultConstant(0xC00D27DC)]
    public static HResult NS_E_DRM_STORE_NOTALLOWED => new(0xC00D27DC);

    /// <summary>The license for this file requires an upgraded version of your player or a different player.</summary>
    [Description("""The license for this file requires an upgraded version of your player or a different player.""")]
    [HResultConstant(0xC00D27DD)]
    public static HResult NS_E_DRM_LICENSE_APP_NOTALLOWED => new(0xC00D27DD);

    /// <summary>The license server's certificate expired. Make sure your system clock is set correctly. Contact your content provider for further assistance.</summary>
    [Description("""The license server's certificate expired. Make sure your system clock is set correctly. Contact your content provider for further assistance.""")]
    [HResultConstant(0xC00D27DF)]
    public static HResult NS_E_DRM_LICENSE_CERT_EXPIRED => new(0xC00D27DF);

    /// <summary>The license for this file requires a higher level of security than the player you are currently using has. Try using a different player or download a newer version of your current player.</summary>
    [Description("""The license for this file requires a higher level of security than the player you are currently using has. Try using a different player or download a newer version of your current player.""")]
    [HResultConstant(0xC00D27E0)]
    public static HResult NS_E_DRM_LICENSE_SECLOW => new(0xC00D27E0);

    /// <summary>The content owner for the license you just acquired is no longer supporting their content. Contact the content owner for a newer version of the content.</summary>
    [Description("""The content owner for the license you just acquired is no longer supporting their content. Contact the content owner for a newer version of the content.""")]
    [HResultConstant(0xC00D27E1)]
    public static HResult NS_E_DRM_LICENSE_CONTENT_REVOKED => new(0xC00D27E1);

    /// <summary>The content owner for the license you just acquired requires your device to register to the current machine.</summary>
    [Description("""The content owner for the license you just acquired requires your device to register to the current machine.""")]
    [HResultConstant(0xC00D27E2)]
    public static HResult NS_E_DRM_DEVICE_NOT_REGISTERED => new(0xC00D27E2);

    /// <summary>The license for this file requires a feature that is not supported in your current player or operating system. You can try with newer version of your current player or contact your content provider for further assistance.</summary>
    [Description("""The license for this file requires a feature that is not supported in your current player or operating system. You can try with newer version of your current player or contact your content provider for further assistance.""")]
    [HResultConstant(0xC00D280A)]
    public static HResult NS_E_DRM_LICENSE_NOSAP => new(0xC00D280A);

    /// <summary>The license for this file requires a feature that is not supported in your current player or operating system. You can try with newer version of your current player or contact your content provider for further assistance.</summary>
    [Description("""The license for this file requires a feature that is not supported in your current player or operating system. You can try with newer version of your current player or contact your content provider for further assistance.""")]
    [HResultConstant(0xC00D280B)]
    public static HResult NS_E_DRM_LICENSE_NOSVP => new(0xC00D280B);

    /// <summary>The license for this file requires Windows Driver Model (WDM) audio drivers. Contact your sound card manufacturer for further assistance.</summary>
    [Description("""The license for this file requires Windows Driver Model (WDM) audio drivers. Contact your sound card manufacturer for further assistance.""")]
    [HResultConstant(0xC00D280C)]
    public static HResult NS_E_DRM_LICENSE_NOWDM => new(0xC00D280C);

    /// <summary>The license for this file requires a higher level of security than the player you are currently using has. Try using a different player or download a newer version of your current player.</summary>
    [Description("""The license for this file requires a higher level of security than the player you are currently using has. Try using a different player or download a newer version of your current player.""")]
    [HResultConstant(0xC00D280D)]
    public static HResult NS_E_DRM_LICENSE_NOTRUSTEDCODEC => new(0xC00D280D);

    /// <summary>The license for this file is not supported by your current player. You can try with newer version of your current player or contact your content provider for further assistance.</summary>
    [Description("""The license for this file is not supported by your current player. You can try with newer version of your current player or contact your content provider for further assistance.""")]
    [HResultConstant(0xC00D280E)]
    public static HResult NS_E_DRM_SOURCEID_NOT_SUPPORTED => new(0xC00D280E);

    /// <summary>An updated version of your media player is required to play the selected content.</summary>
    [Description("""An updated version of your media player is required to play the selected content.""")]
    [HResultConstant(0xC00D283D)]
    public static HResult NS_E_DRM_NEEDS_UPGRADE_TEMPFILE => new(0xC00D283D);

    /// <summary>A new version of the Digital Rights Management component is required. Contact product support for this application to get the latest version.</summary>
    [Description("""A new version of the Digital Rights Management component is required. Contact product support for this application to get the latest version.""")]
    [HResultConstant(0xC00D283E)]
    public static HResult NS_E_DRM_NEED_UPGRADE_PD => new(0xC00D283E);

    /// <summary>Failed to either create or verify the content header.</summary>
    [Description("""Failed to either create or verify the content header.""")]
    [HResultConstant(0xC00D283F)]
    public static HResult NS_E_DRM_SIGNATURE_FAILURE => new(0xC00D283F);

    /// <summary>Could not read the necessary information from the system registry.</summary>
    [Description("""Could not read the necessary information from the system registry.""")]
    [HResultConstant(0xC00D2840)]
    public static HResult NS_E_DRM_LICENSE_SERVER_INFO_MISSING => new(0xC00D2840);

    /// <summary>The DRM subsystem is currently locked by another application or user. Try again later.</summary>
    [Description("""The DRM subsystem is currently locked by another application or user. Try again later.""")]
    [HResultConstant(0xC00D2841)]
    public static HResult NS_E_DRM_BUSY => new(0xC00D2841);

    /// <summary>There are too many target devices registered on the portable media.</summary>
    [Description("""There are too many target devices registered on the portable media.""")]
    [HResultConstant(0xC00D2842)]
    public static HResult NS_E_DRM_PD_TOO_MANY_DEVICES => new(0xC00D2842);

    /// <summary>The security upgrade cannot be completed because the allowed number of daily upgrades has been exceeded. Try again tomorrow.</summary>
    [Description("""The security upgrade cannot be completed because the allowed number of daily upgrades has been exceeded. Try again tomorrow.""")]
    [HResultConstant(0xC00D2843)]
    public static HResult NS_E_DRM_INDIV_FRAUD => new(0xC00D2843);

    /// <summary>The security upgrade cannot be completed because the server is unable to perform the operation. Try again later.</summary>
    [Description("""The security upgrade cannot be completed because the server is unable to perform the operation. Try again later.""")]
    [HResultConstant(0xC00D2844)]
    public static HResult NS_E_DRM_INDIV_NO_CABS => new(0xC00D2844);

    /// <summary>The security upgrade cannot be performed because the server is not available. Try again later.</summary>
    [Description("""The security upgrade cannot be performed because the server is not available. Try again later.""")]
    [HResultConstant(0xC00D2845)]
    public static HResult NS_E_DRM_INDIV_SERVICE_UNAVAILABLE => new(0xC00D2845);

    /// <summary>Windows Media Player cannot restore your licenses because the server is not available. Try again later.</summary>
    [Description("""Windows Media Player cannot restore your licenses because the server is not available. Try again later.""")]
    [HResultConstant(0xC00D2846)]
    public static HResult NS_E_DRM_RESTORE_SERVICE_UNAVAILABLE => new(0xC00D2846);

    /// <summary>Windows Media Player cannot play the protected file. Verify that your computer's date is set correctly. If it is correct, on the Help menu, click Check for Player Updates to install the latest version of the Player.</summary>
    [Description("""Windows Media Player cannot play the protected file. Verify that your computer's date is set correctly. If it is correct, on the Help menu, click Check for Player Updates to install the latest version of the Player.""")]
    [HResultConstant(0xC00D2847)]
    public static HResult NS_E_DRM_CLIENT_CODE_EXPIRED => new(0xC00D2847);

    /// <summary>The chained license cannot be created because the referenced uplink license does not exist.</summary>
    [Description("""The chained license cannot be created because the referenced uplink license does not exist.""")]
    [HResultConstant(0xC00D2848)]
    public static HResult NS_E_DRM_NO_UPLINK_LICENSE => new(0xC00D2848);

    /// <summary>The specified KID is invalid.</summary>
    [Description("""The specified KID is invalid.""")]
    [HResultConstant(0xC00D2849)]
    public static HResult NS_E_DRM_INVALID_KID => new(0xC00D2849);

    /// <summary>License initialization did not work. Contact Microsoft product support.</summary>
    [Description("""License initialization did not work. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D284A)]
    public static HResult NS_E_DRM_LICENSE_INITIALIZATION_ERROR => new(0xC00D284A);

    /// <summary>The uplink license of a chained license cannot itself be a chained license.</summary>
    [Description("""The uplink license of a chained license cannot itself be a chained license.""")]
    [HResultConstant(0xC00D284C)]
    public static HResult NS_E_DRM_CHAIN_TOO_LONG => new(0xC00D284C);

    /// <summary>The specified encryption algorithm is unsupported.</summary>
    [Description("""The specified encryption algorithm is unsupported.""")]
    [HResultConstant(0xC00D284D)]
    public static HResult NS_E_DRM_UNSUPPORTED_ALGORITHM => new(0xC00D284D);

    /// <summary>License deletion did not work. Contact Microsoft product support.</summary>
    [Description("""License deletion did not work. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D284E)]
    public static HResult NS_E_DRM_LICENSE_DELETION_ERROR => new(0xC00D284E);

    /// <summary>The client's certificate is corrupted or the signature cannot be verified.</summary>
    [Description("""The client's certificate is corrupted or the signature cannot be verified.""")]
    [HResultConstant(0xC00D28A0)]
    public static HResult NS_E_DRM_INVALID_CERTIFICATE => new(0xC00D28A0);

    /// <summary>The client's certificate has been revoked.</summary>
    [Description("""The client's certificate has been revoked.""")]
    [HResultConstant(0xC00D28A1)]
    public static HResult NS_E_DRM_CERTIFICATE_REVOKED => new(0xC00D28A1);

    /// <summary>There is no license available for the requested action.</summary>
    [Description("""There is no license available for the requested action.""")]
    [HResultConstant(0xC00D28A2)]
    public static HResult NS_E_DRM_LICENSE_UNAVAILABLE => new(0xC00D28A2);

    /// <summary>The maximum number of devices in use has been reached. Unable to open additional devices.</summary>
    [Description("""The maximum number of devices in use has been reached. Unable to open additional devices.""")]
    [HResultConstant(0xC00D28A3)]
    public static HResult NS_E_DRM_DEVICE_LIMIT_REACHED => new(0xC00D28A3);

    /// <summary>The proximity detection procedure could not confirm that the receiver is near the transmitter in the network.</summary>
    [Description("""The proximity detection procedure could not confirm that the receiver is near the transmitter in the network.""")]
    [HResultConstant(0xC00D28A4)]
    public static HResult NS_E_DRM_UNABLE_TO_VERIFY_PROXIMITY => new(0xC00D28A4);

    /// <summary>The client must be registered before executing the intended operation.</summary>
    [Description("""The client must be registered before executing the intended operation.""")]
    [HResultConstant(0xC00D28A5)]
    public static HResult NS_E_DRM_MUST_REGISTER => new(0xC00D28A5);

    /// <summary>The client must be approved before executing the intended operation.</summary>
    [Description("""The client must be approved before executing the intended operation.""")]
    [HResultConstant(0xC00D28A6)]
    public static HResult NS_E_DRM_MUST_APPROVE => new(0xC00D28A6);

    /// <summary>The client must be revalidated before executing the intended operation.</summary>
    [Description("""The client must be revalidated before executing the intended operation.""")]
    [HResultConstant(0xC00D28A7)]
    public static HResult NS_E_DRM_MUST_REVALIDATE => new(0xC00D28A7);

    /// <summary>The response to the proximity detection challenge is invalid.</summary>
    [Description("""The response to the proximity detection challenge is invalid.""")]
    [HResultConstant(0xC00D28A8)]
    public static HResult NS_E_DRM_INVALID_PROXIMITY_RESPONSE => new(0xC00D28A8);

    /// <summary>The requested session is invalid.</summary>
    [Description("""The requested session is invalid.""")]
    [HResultConstant(0xC00D28A9)]
    public static HResult NS_E_DRM_INVALID_SESSION => new(0xC00D28A9);

    /// <summary>The device must be opened before it can be used to receive content.</summary>
    [Description("""The device must be opened before it can be used to receive content.""")]
    [HResultConstant(0xC00D28AA)]
    public static HResult NS_E_DRM_DEVICE_NOT_OPEN => new(0xC00D28AA);

    /// <summary>Device registration failed because the device is already registered.</summary>
    [Description("""Device registration failed because the device is already registered.""")]
    [HResultConstant(0xC00D28AB)]
    public static HResult NS_E_DRM_DEVICE_ALREADY_REGISTERED => new(0xC00D28AB);

    /// <summary>Unsupported WMDRM-ND protocol version.</summary>
    [Description("""Unsupported WMDRM-ND protocol version.""")]
    [HResultConstant(0xC00D28AC)]
    public static HResult NS_E_DRM_UNSUPPORTED_PROTOCOL_VERSION => new(0xC00D28AC);

    /// <summary>The requested action is not supported.</summary>
    [Description("""The requested action is not supported.""")]
    [HResultConstant(0xC00D28AD)]
    public static HResult NS_E_DRM_UNSUPPORTED_ACTION => new(0xC00D28AD);

    /// <summary>The certificate does not have an adequate security level for the requested action.</summary>
    [Description("""The certificate does not have an adequate security level for the requested action.""")]
    [HResultConstant(0xC00D28AE)]
    public static HResult NS_E_DRM_CERTIFICATE_SECURITY_LEVEL_INADEQUATE => new(0xC00D28AE);

    /// <summary>Unable to open the specified port for receiving Proximity messages.</summary>
    [Description("""Unable to open the specified port for receiving Proximity messages.""")]
    [HResultConstant(0xC00D28AF)]
    public static HResult NS_E_DRM_UNABLE_TO_OPEN_PORT => new(0xC00D28AF);

    /// <summary>The message format is invalid.</summary>
    [Description("""The message format is invalid.""")]
    [HResultConstant(0xC00D28B0)]
    public static HResult NS_E_DRM_BAD_REQUEST => new(0xC00D28B0);

    /// <summary>The Certificate Revocation List is invalid or corrupted.</summary>
    [Description("""The Certificate Revocation List is invalid or corrupted.""")]
    [HResultConstant(0xC00D28B1)]
    public static HResult NS_E_DRM_INVALID_CRL => new(0xC00D28B1);

    /// <summary>The length of the attribute name or value is too long.</summary>
    [Description("""The length of the attribute name or value is too long.""")]
    [HResultConstant(0xC00D28B2)]
    public static HResult NS_E_DRM_ATTRIBUTE_TOO_LONG => new(0xC00D28B2);

    /// <summary>The license blob passed in the cardea request is expired.</summary>
    [Description("""The license blob passed in the cardea request is expired.""")]
    [HResultConstant(0xC00D28B3)]
    public static HResult NS_E_DRM_EXPIRED_LICENSEBLOB => new(0xC00D28B3);

    /// <summary>The license blob passed in the cardea request is invalid. Contact Microsoft product support.</summary>
    [Description("""The license blob passed in the cardea request is invalid. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D28B4)]
    public static HResult NS_E_DRM_INVALID_LICENSEBLOB => new(0xC00D28B4);

    /// <summary>The requested operation cannot be performed because the license does not contain an inclusion list.</summary>
    [Description("""The requested operation cannot be performed because the license does not contain an inclusion list.""")]
    [HResultConstant(0xC00D28B5)]
    public static HResult NS_E_DRM_INCLUSION_LIST_REQUIRED => new(0xC00D28B5);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D28B6)]
    public static HResult NS_E_DRM_DRMV2CLT_REVOKED => new(0xC00D28B6);

    /// <summary>A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.</summary>
    [Description("""A problem has occurred in the Digital Rights Management component. Contact Microsoft product support.""")]
    [HResultConstant(0xC00D28B7)]
    public static HResult NS_E_DRM_RIV_TOO_SMALL => new(0xC00D28B7);

    /// <summary>Windows Media Player does not support the level of output protection required by the content.</summary>
    [Description("""Windows Media Player does not support the level of output protection required by the content.""")]
    [HResultConstant(0xC00D2904)]
    public static HResult NS_E_OUTPUT_PROTECTION_LEVEL_UNSUPPORTED => new(0xC00D2904);

    /// <summary>Windows Media Player does not support the level of protection required for compressed digital video.</summary>
    [Description("""Windows Media Player does not support the level of protection required for compressed digital video.""")]
    [HResultConstant(0xC00D2905)]
    public static HResult NS_E_COMPRESSED_DIGITAL_VIDEO_PROTECTION_LEVEL_UNSUPPORTED => new(0xC00D2905);

    /// <summary>Windows Media Player does not support the level of protection required for uncompressed digital video.</summary>
    [Description("""Windows Media Player does not support the level of protection required for uncompressed digital video.""")]
    [HResultConstant(0xC00D2906)]
    public static HResult NS_E_UNCOMPRESSED_DIGITAL_VIDEO_PROTECTION_LEVEL_UNSUPPORTED => new(0xC00D2906);

    /// <summary>Windows Media Player does not support the level of protection required for analog video.</summary>
    [Description("""Windows Media Player does not support the level of protection required for analog video.""")]
    [HResultConstant(0xC00D2907)]
    public static HResult NS_E_ANALOG_VIDEO_PROTECTION_LEVEL_UNSUPPORTED => new(0xC00D2907);

    /// <summary>Windows Media Player does not support the level of protection required for compressed digital audio.</summary>
    [Description("""Windows Media Player does not support the level of protection required for compressed digital audio.""")]
    [HResultConstant(0xC00D2908)]
    public static HResult NS_E_COMPRESSED_DIGITAL_AUDIO_PROTECTION_LEVEL_UNSUPPORTED => new(0xC00D2908);

    /// <summary>Windows Media Player does not support the level of protection required for uncompressed digital audio.</summary>
    [Description("""Windows Media Player does not support the level of protection required for uncompressed digital audio.""")]
    [HResultConstant(0xC00D2909)]
    public static HResult NS_E_UNCOMPRESSED_DIGITAL_AUDIO_PROTECTION_LEVEL_UNSUPPORTED => new(0xC00D2909);

    /// <summary>Windows Media Player does not support the scheme of output protection required by the content.</summary>
    [Description("""Windows Media Player does not support the scheme of output protection required by the content.""")]
    [HResultConstant(0xC00D290A)]
    public static HResult NS_E_OUTPUT_PROTECTION_SCHEME_UNSUPPORTED => new(0xC00D290A);

    /// <summary>Installation was not successful and some file cleanup is not complete. For best results, restart your computer.</summary>
    [Description("""Installation was not successful and some file cleanup is not complete. For best results, restart your computer.""")]
    [HResultConstant(0xC00D2AFA)]
    public static HResult NS_E_REBOOT_RECOMMENDED => new(0xC00D2AFA);

    /// <summary>Installation was not successful. To continue, you must restart your computer.</summary>
    [Description("""Installation was not successful. To continue, you must restart your computer.""")]
    [HResultConstant(0xC00D2AFB)]
    public static HResult NS_E_REBOOT_REQUIRED => new(0xC00D2AFB);

    /// <summary>Installation was not successful.</summary>
    [Description("""Installation was not successful.""")]
    [HResultConstant(0xC00D2AFC)]
    public static HResult NS_E_SETUP_INCOMPLETE => new(0xC00D2AFC);

    /// <summary>Setup cannot migrate the Windows Media Digital Rights Management (DRM) components.</summary>
    [Description("""Setup cannot migrate the Windows Media Digital Rights Management (DRM) components.""")]
    [HResultConstant(0xC00D2AFD)]
    public static HResult NS_E_SETUP_DRM_MIGRATION_FAILED => new(0xC00D2AFD);

    /// <summary>Some skin or playlist components cannot be installed.</summary>
    [Description("""Some skin or playlist components cannot be installed.""")]
    [HResultConstant(0xC00D2AFE)]
    public static HResult NS_E_SETUP_IGNORABLE_FAILURE => new(0xC00D2AFE);

    /// <summary>Setup cannot migrate the Windows Media Digital Rights Management (DRM) components. In addition, some skin or playlist components cannot be installed.</summary>
    [Description("""Setup cannot migrate the Windows Media Digital Rights Management (DRM) components. In addition, some skin or playlist components cannot be installed.""")]
    [HResultConstant(0xC00D2AFF)]
    public static HResult NS_E_SETUP_DRM_MIGRATION_FAILED_AND_IGNORABLE_FAILURE => new(0xC00D2AFF);

    /// <summary>Installation is blocked because your computer does not meet one or more of the setup requirements.</summary>
    [Description("""Installation is blocked because your computer does not meet one or more of the setup requirements.""")]
    [HResultConstant(0xC00D2B00)]
    public static HResult NS_E_SETUP_BLOCKED => new(0xC00D2B00);

    /// <summary>The specified protocol is not supported.</summary>
    [Description("""The specified protocol is not supported.""")]
    [HResultConstant(0xC00D2EE0)]
    public static HResult NS_E_UNKNOWN_PROTOCOL => new(0xC00D2EE0);

    /// <summary>The client is redirected to a proxy server.</summary>
    [Description("""The client is redirected to a proxy server.""")]
    [HResultConstant(0xC00D2EE1)]
    public static HResult NS_E_REDIRECT_TO_PROXY => new(0xC00D2EE1);

    /// <summary>The server encountered an unexpected condition which prevented it from fulfilling the request.</summary>
    [Description("""The server encountered an unexpected condition which prevented it from fulfilling the request.""")]
    [HResultConstant(0xC00D2EE2)]
    public static HResult NS_E_INTERNAL_SERVER_ERROR => new(0xC00D2EE2);

    /// <summary>The request could not be understood by the server.</summary>
    [Description("""The request could not be understood by the server.""")]
    [HResultConstant(0xC00D2EE3)]
    public static HResult NS_E_BAD_REQUEST => new(0xC00D2EE3);

    /// <summary>The proxy experienced an error while attempting to contact the media server.</summary>
    [Description("""The proxy experienced an error while attempting to contact the media server.""")]
    [HResultConstant(0xC00D2EE4)]
    public static HResult NS_E_ERROR_FROM_PROXY => new(0xC00D2EE4);

    /// <summary>The proxy did not receive a timely response while attempting to contact the media server.</summary>
    [Description("""The proxy did not receive a timely response while attempting to contact the media server.""")]
    [HResultConstant(0xC00D2EE5)]
    public static HResult NS_E_PROXY_TIMEOUT => new(0xC00D2EE5);

    /// <summary>The server is currently unable to handle the request due to a temporary overloading or maintenance of the server.</summary>
    [Description("""The server is currently unable to handle the request due to a temporary overloading or maintenance of the server.""")]
    [HResultConstant(0xC00D2EE6)]
    public static HResult NS_E_SERVER_UNAVAILABLE => new(0xC00D2EE6);

    /// <summary>The server is refusing to fulfill the requested operation.</summary>
    [Description("""The server is refusing to fulfill the requested operation.""")]
    [HResultConstant(0xC00D2EE7)]
    public static HResult NS_E_REFUSED_BY_SERVER => new(0xC00D2EE7);

    /// <summary>The server is not a compatible streaming media server.</summary>
    [Description("""The server is not a compatible streaming media server.""")]
    [HResultConstant(0xC00D2EE8)]
    public static HResult NS_E_INCOMPATIBLE_SERVER => new(0xC00D2EE8);

    /// <summary>The content cannot be streamed because the Multicast protocol has been disabled.</summary>
    [Description("""The content cannot be streamed because the Multicast protocol has been disabled.""")]
    [HResultConstant(0xC00D2EE9)]
    public static HResult NS_E_MULTICAST_DISABLED => new(0xC00D2EE9);

    /// <summary>The server redirected the player to an invalid location.</summary>
    [Description("""The server redirected the player to an invalid location.""")]
    [HResultConstant(0xC00D2EEA)]
    public static HResult NS_E_INVALID_REDIRECT => new(0xC00D2EEA);

    /// <summary>The content cannot be streamed because all protocols have been disabled.</summary>
    [Description("""The content cannot be streamed because all protocols have been disabled.""")]
    [HResultConstant(0xC00D2EEB)]
    public static HResult NS_E_ALL_PROTOCOLS_DISABLED => new(0xC00D2EEB);

    /// <summary>The MSBD protocol is no longer supported. Please use HTTP to connect to the Windows Media stream.</summary>
    [Description("""The MSBD protocol is no longer supported. Please use HTTP to connect to the Windows Media stream.""")]
    [HResultConstant(0xC00D2EEC)]
    public static HResult NS_E_MSBD_NO_LONGER_SUPPORTED => new(0xC00D2EEC);

    /// <summary>The proxy server could not be located. Please check your proxy server configuration.</summary>
    [Description("""The proxy server could not be located. Please check your proxy server configuration.""")]
    [HResultConstant(0xC00D2EED)]
    public static HResult NS_E_PROXY_NOT_FOUND => new(0xC00D2EED);

    /// <summary>Unable to establish a connection to the proxy server. Please check your proxy server configuration.</summary>
    [Description("""Unable to establish a connection to the proxy server. Please check your proxy server configuration.""")]
    [HResultConstant(0xC00D2EEE)]
    public static HResult NS_E_CANNOT_CONNECT_TO_PROXY => new(0xC00D2EEE);

    /// <summary>Unable to locate the media server. The operation timed out.</summary>
    [Description("""Unable to locate the media server. The operation timed out.""")]
    [HResultConstant(0xC00D2EEF)]
    public static HResult NS_E_SERVER_DNS_TIMEOUT => new(0xC00D2EEF);

    /// <summary>Unable to locate the proxy server. The operation timed out.</summary>
    [Description("""Unable to locate the proxy server. The operation timed out.""")]
    [HResultConstant(0xC00D2EF0)]
    public static HResult NS_E_PROXY_DNS_TIMEOUT => new(0xC00D2EF0);

    /// <summary>Media closed because Windows was shut down.</summary>
    [Description("""Media closed because Windows was shut down.""")]
    [HResultConstant(0xC00D2EF1)]
    public static HResult NS_E_CLOSED_ON_SUSPEND => new(0xC00D2EF1);

    /// <summary>Unable to read the contents of a playlist file from a media server.</summary>
    [Description("""Unable to read the contents of a playlist file from a media server.""")]
    [HResultConstant(0xC00D2EF2)]
    public static HResult NS_E_CANNOT_READ_PLAYLIST_FROM_MEDIASERVER => new(0xC00D2EF2);

    /// <summary>Session not found.</summary>
    [Description("""Session not found.""")]
    [HResultConstant(0xC00D2EF3)]
    public static HResult NS_E_SESSION_NOT_FOUND => new(0xC00D2EF3);

    /// <summary>Content requires a streaming media client.</summary>
    [Description("""Content requires a streaming media client.""")]
    [HResultConstant(0xC00D2EF4)]
    public static HResult NS_E_REQUIRE_STREAMING_CLIENT => new(0xC00D2EF4);

    /// <summary>A command applies to a previous playlist entry.</summary>
    [Description("""A command applies to a previous playlist entry.""")]
    [HResultConstant(0xC00D2EF5)]
    public static HResult NS_E_PLAYLIST_ENTRY_HAS_CHANGED => new(0xC00D2EF5);

    /// <summary>The proxy server is denying access. The username and/or password might be incorrect.</summary>
    [Description("""The proxy server is denying access. The username and/or password might be incorrect.""")]
    [HResultConstant(0xC00D2EF6)]
    public static HResult NS_E_PROXY_ACCESSDENIED => new(0xC00D2EF6);

    /// <summary>The proxy could not provide valid authentication credentials to the media server.</summary>
    [Description("""The proxy could not provide valid authentication credentials to the media server.""")]
    [HResultConstant(0xC00D2EF7)]
    public static HResult NS_E_PROXY_SOURCE_ACCESSDENIED => new(0xC00D2EF7);

    /// <summary>The network sink failed to write data to the network.</summary>
    [Description("""The network sink failed to write data to the network.""")]
    [HResultConstant(0xC00D2EF8)]
    public static HResult NS_E_NETWORK_SINK_WRITE => new(0xC00D2EF8);

    /// <summary>Packets are not being received from the server. The packets might be blocked by a filtering device, such as a network firewall.</summary>
    [Description("""Packets are not being received from the server. The packets might be blocked by a filtering device, such as a network firewall.""")]
    [HResultConstant(0xC00D2EF9)]
    public static HResult NS_E_FIREWALL => new(0xC00D2EF9);

    /// <summary>The MMS protocol is not supported. Please use HTTP or RTSP to connect to the Windows Media stream.</summary>
    [Description("""The MMS protocol is not supported. Please use HTTP or RTSP to connect to the Windows Media stream.""")]
    [HResultConstant(0xC00D2EFA)]
    public static HResult NS_E_MMS_NOT_SUPPORTED => new(0xC00D2EFA);

    /// <summary>The Windows Media server is denying access. The username and/or password might be incorrect.</summary>
    [Description("""The Windows Media server is denying access. The username and/or password might be incorrect.""")]
    [HResultConstant(0xC00D2EFB)]
    public static HResult NS_E_SERVER_ACCESSDENIED => new(0xC00D2EFB);

    /// <summary>The Publishing Point or file on the Windows Media Server is no longer available.</summary>
    [Description("""The Publishing Point or file on the Windows Media Server is no longer available.""")]
    [HResultConstant(0xC00D2EFC)]
    public static HResult NS_E_RESOURCE_GONE => new(0xC00D2EFC);

    /// <summary>There is no existing packetizer plugin for a stream.</summary>
    [Description("""There is no existing packetizer plugin for a stream.""")]
    [HResultConstant(0xC00D2EFD)]
    public static HResult NS_E_NO_EXISTING_PACKETIZER => new(0xC00D2EFD);

    /// <summary>The response from the media server could not be understood. This might be caused by an incompatible proxy server or media server.</summary>
    [Description("""The response from the media server could not be understood. This might be caused by an incompatible proxy server or media server.""")]
    [HResultConstant(0xC00D2EFE)]
    public static HResult NS_E_BAD_SYNTAX_IN_SERVER_RESPONSE => new(0xC00D2EFE);

    /// <summary>The Windows Media Server reset the network connection.</summary>
    [Description("""The Windows Media Server reset the network connection.""")]
    [HResultConstant(0xC00D2F00)]
    public static HResult NS_E_RESET_SOCKET_CONNECTION => new(0xC00D2F00);

    /// <summary>The request could not reach the media server (too many hops).</summary>
    [Description("""The request could not reach the media server (too many hops).""")]
    [HResultConstant(0xC00D2F02)]
    public static HResult NS_E_TOO_MANY_HOPS => new(0xC00D2F02);

    /// <summary>The server is sending too much data. The connection has been terminated.</summary>
    [Description("""The server is sending too much data. The connection has been terminated.""")]
    [HResultConstant(0xC00D2F05)]
    public static HResult NS_E_TOO_MUCH_DATA_FROM_SERVER => new(0xC00D2F05);

    /// <summary>It was not possible to establish a connection to the media server in a timely manner. The media server might be down for maintenance, or it might be necessary to use a proxy server to access this media server.</summary>
    [Description("""It was not possible to establish a connection to the media server in a timely manner. The media server might be down for maintenance, or it might be necessary to use a proxy server to access this media server.""")]
    [HResultConstant(0xC00D2F06)]
    public static HResult NS_E_CONNECT_TIMEOUT => new(0xC00D2F06);

    /// <summary>It was not possible to establish a connection to the proxy server in a timely manner. Please check your proxy server configuration.</summary>
    [Description("""It was not possible to establish a connection to the proxy server in a timely manner. Please check your proxy server configuration.""")]
    [HResultConstant(0xC00D2F07)]
    public static HResult NS_E_PROXY_CONNECT_TIMEOUT => new(0xC00D2F07);

    /// <summary>Session not found.</summary>
    [Description("""Session not found.""")]
    [HResultConstant(0xC00D2F08)]
    public static HResult NS_E_SESSION_INVALID => new(0xC00D2F08);

    /// <summary>Unknown packet sink stream.</summary>
    [Description("""Unknown packet sink stream.""")]
    [HResultConstant(0xC00D2F0A)]
    public static HResult NS_E_PACKETSINK_UNKNOWN_FEC_STREAM => new(0xC00D2F0A);

    /// <summary>Unable to establish a connection to the server. Ensure Windows Media Services is started and the HTTP Server control protocol is properly enabled.</summary>
    [Description("""Unable to establish a connection to the server. Ensure Windows Media Services is started and the HTTP Server control protocol is properly enabled.""")]
    [HResultConstant(0xC00D2F0B)]
    public static HResult NS_E_PUSH_CANNOTCONNECT => new(0xC00D2F0B);

    /// <summary>The Server service that received the HTTP push request is not a compatible version of Windows Media Services (WMS). This error might indicate the push request was received by IIS instead of WMS. Ensure WMS is started and has the HTTP Server control protocol properly enabled and try again.</summary>
    [Description("""The Server service that received the HTTP push request is not a compatible version of Windows Media Services (WMS). This error might indicate the push request was received by IIS instead of WMS. Ensure WMS is started and has the HTTP Server control protocol properly enabled and try again.""")]
    [HResultConstant(0xC00D2F0C)]
    public static HResult NS_E_INCOMPATIBLE_PUSH_SERVER => new(0xC00D2F0C);

    /// <summary>The playlist has reached its end.</summary>
    [Description("""The playlist has reached its end.""")]
    [HResultConstant(0xC00D32C8)]
    public static HResult NS_E_END_OF_PLAYLIST => new(0xC00D32C8);

    /// <summary>Use file source.</summary>
    [Description("""Use file source.""")]
    [HResultConstant(0xC00D32C9)]
    public static HResult NS_E_USE_FILE_SOURCE => new(0xC00D32C9);

    /// <summary>The property was not found.</summary>
    [Description("""The property was not found.""")]
    [HResultConstant(0xC00D32CA)]
    public static HResult NS_E_PROPERTY_NOT_FOUND => new(0xC00D32CA);

    /// <summary>The property is read only.</summary>
    [Description("""The property is read only.""")]
    [HResultConstant(0xC00D32CC)]
    public static HResult NS_E_PROPERTY_READ_ONLY => new(0xC00D32CC);

    /// <summary>The table key was not found.</summary>
    [Description("""The table key was not found.""")]
    [HResultConstant(0xC00D32CD)]
    public static HResult NS_E_TABLE_KEY_NOT_FOUND => new(0xC00D32CD);

    /// <summary>Invalid query operator.</summary>
    [Description("""Invalid query operator.""")]
    [HResultConstant(0xC00D32CF)]
    public static HResult NS_E_INVALID_QUERY_OPERATOR => new(0xC00D32CF);

    /// <summary>Invalid query property.</summary>
    [Description("""Invalid query property.""")]
    [HResultConstant(0xC00D32D0)]
    public static HResult NS_E_INVALID_QUERY_PROPERTY => new(0xC00D32D0);

    /// <summary>The property is not supported.</summary>
    [Description("""The property is not supported.""")]
    [HResultConstant(0xC00D32D2)]
    public static HResult NS_E_PROPERTY_NOT_SUPPORTED => new(0xC00D32D2);

    /// <summary>Schema classification failure.</summary>
    [Description("""Schema classification failure.""")]
    [HResultConstant(0xC00D32D4)]
    public static HResult NS_E_SCHEMA_CLASSIFY_FAILURE => new(0xC00D32D4);

    /// <summary>The metadata format is not supported.</summary>
    [Description("""The metadata format is not supported.""")]
    [HResultConstant(0xC00D32D5)]
    public static HResult NS_E_METADATA_FORMAT_NOT_SUPPORTED => new(0xC00D32D5);

    /// <summary>Cannot edit the metadata.</summary>
    [Description("""Cannot edit the metadata.""")]
    [HResultConstant(0xC00D32D6)]
    public static HResult NS_E_METADATA_NO_EDITING_CAPABILITY => new(0xC00D32D6);

    /// <summary>Cannot set the locale id.</summary>
    [Description("""Cannot set the locale id.""")]
    [HResultConstant(0xC00D32D7)]
    public static HResult NS_E_METADATA_CANNOT_SET_LOCALE => new(0xC00D32D7);

    /// <summary>The language is not supported in the format.</summary>
    [Description("""The language is not supported in the format.""")]
    [HResultConstant(0xC00D32D8)]
    public static HResult NS_E_METADATA_LANGUAGE_NOT_SUPORTED => new(0xC00D32D8);

    /// <summary>There is no RFC1766 name translation for the supplied locale id.</summary>
    [Description("""There is no RFC1766 name translation for the supplied locale id.""")]
    [HResultConstant(0xC00D32D9)]
    public static HResult NS_E_METADATA_NO_RFC1766_NAME_FOR_LOCALE => new(0xC00D32D9);

    /// <summary>The metadata (or metadata item) is not available.</summary>
    [Description("""The metadata (or metadata item) is not available.""")]
    [HResultConstant(0xC00D32DA)]
    public static HResult NS_E_METADATA_NOT_AVAILABLE => new(0xC00D32DA);

    /// <summary>The cached metadata (or metadata item) is not available.</summary>
    [Description("""The cached metadata (or metadata item) is not available.""")]
    [HResultConstant(0xC00D32DB)]
    public static HResult NS_E_METADATA_CACHE_DATA_NOT_AVAILABLE => new(0xC00D32DB);

    /// <summary>The metadata document is invalid.</summary>
    [Description("""The metadata document is invalid.""")]
    [HResultConstant(0xC00D32DC)]
    public static HResult NS_E_METADATA_INVALID_DOCUMENT_TYPE => new(0xC00D32DC);

    /// <summary>The metadata content identifier is not available.</summary>
    [Description("""The metadata content identifier is not available.""")]
    [HResultConstant(0xC00D32DD)]
    public static HResult NS_E_METADATA_IDENTIFIER_NOT_AVAILABLE => new(0xC00D32DD);

    /// <summary>Cannot retrieve metadata from the offline metadata cache.</summary>
    [Description("""Cannot retrieve metadata from the offline metadata cache.""")]
    [HResultConstant(0xC00D32DE)]
    public static HResult NS_E_METADATA_CANNOT_RETRIEVE_FROM_OFFLINE_CACHE => new(0xC00D32DE);

    /// <summary>Checksum of the obtained monitor descriptor is invalid.</summary>
    [Description("""Checksum of the obtained monitor descriptor is invalid.""")]
    [HResultConstant(0xC0261003)]
    public static HResult ERROR_MONITOR_INVALID_DESCRIPTOR_CHECKSUM => new(0xC0261003);

    /// <summary>Monitor descriptor contains an invalid standard timing block.</summary>
    [Description("""Monitor descriptor contains an invalid standard timing block.""")]
    [HResultConstant(0xC0261004)]
    public static HResult ERROR_MONITOR_INVALID_STANDARD_TIMING_BLOCK => new(0xC0261004);

    /// <summary>Windows Management Instrumentation (WMI) data block registration failed for one of the MSMonitorClass WMI subclasses.</summary>
    [Description("""Windows Management Instrumentation (WMI) data block registration failed for one of the MSMonitorClass WMI subclasses.""")]
    [HResultConstant(0xC0261005)]
    public static HResult ERROR_MONITOR_WMI_DATABLOCK_REGISTRATION_FAILED => new(0xC0261005);

    /// <summary>Provided monitor descriptor block is either corrupted or does not contain the monitor's detailed serial number.</summary>
    [Description("""Provided monitor descriptor block is either corrupted or does not contain the monitor's detailed serial number.""")]
    [HResultConstant(0xC0261006)]
    public static HResult ERROR_MONITOR_INVALID_SERIAL_NUMBER_MONDSC_BLOCK => new(0xC0261006);

    /// <summary>Provided monitor descriptor block is either corrupted or does not contain the monitor's user-friendly name.</summary>
    [Description("""Provided monitor descriptor block is either corrupted or does not contain the monitor's user-friendly name.""")]
    [HResultConstant(0xC0261007)]
    public static HResult ERROR_MONITOR_INVALID_USER_FRIENDLY_MONDSC_BLOCK => new(0xC0261007);

    /// <summary>There is no monitor descriptor data at the specified (offset, size) region.</summary>
    [Description("""There is no monitor descriptor data at the specified (offset, size) region.""")]
    [HResultConstant(0xC0261008)]
    public static HResult ERROR_MONITOR_NO_MORE_DESCRIPTOR_DATA => new(0xC0261008);

    /// <summary>Monitor descriptor contains an invalid detailed timing block.</summary>
    [Description("""Monitor descriptor contains an invalid detailed timing block.""")]
    [HResultConstant(0xC0261009)]
    public static HResult ERROR_MONITOR_INVALID_DETAILED_TIMING_BLOCK => new(0xC0261009);

    /// <summary>Exclusive mode ownership is needed to create unmanaged primary allocation.</summary>
    [Description("""Exclusive mode ownership is needed to create unmanaged primary allocation.""")]
    [HResultConstant(0xC0262000)]
    public static HResult ERROR_GRAPHICS_NOT_EXCLUSIVE_MODE_OWNER => new(0xC0262000);

    /// <summary>The driver needs more direct memory access (DMA) buffer space to complete the requested operation.</summary>
    [Description("""The driver needs more direct memory access (DMA) buffer space to complete the requested operation.""")]
    [HResultConstant(0xC0262001)]
    public static HResult ERROR_GRAPHICS_INSUFFICIENT_DMA_BUFFER => new(0xC0262001);

    /// <summary>Specified display adapter handle is invalid.</summary>
    [Description("""Specified display adapter handle is invalid.""")]
    [HResultConstant(0xC0262002)]
    public static HResult ERROR_GRAPHICS_INVALID_DISPLAY_ADAPTER => new(0xC0262002);

    /// <summary>Specified display adapter and all of its state has been reset.</summary>
    [Description("""Specified display adapter and all of its state has been reset.""")]
    [HResultConstant(0xC0262003)]
    public static HResult ERROR_GRAPHICS_ADAPTER_WAS_RESET => new(0xC0262003);

    /// <summary>The driver stack does not match the expected driver model.</summary>
    [Description("""The driver stack does not match the expected driver model.""")]
    [HResultConstant(0xC0262004)]
    public static HResult ERROR_GRAPHICS_INVALID_DRIVER_MODEL => new(0xC0262004);

    /// <summary>Present happened but ended up into the changed desktop mode.</summary>
    [Description("""Present happened but ended up into the changed desktop mode.""")]
    [HResultConstant(0xC0262005)]
    public static HResult ERROR_GRAPHICS_PRESENT_MODE_CHANGED => new(0xC0262005);

    /// <summary>Nothing to present due to desktop occlusion.</summary>
    [Description("""Nothing to present due to desktop occlusion.""")]
    [HResultConstant(0xC0262006)]
    public static HResult ERROR_GRAPHICS_PRESENT_OCCLUDED => new(0xC0262006);

    /// <summary>Not able to present due to denial of desktop access.</summary>
    [Description("""Not able to present due to denial of desktop access.""")]
    [HResultConstant(0xC0262007)]
    public static HResult ERROR_GRAPHICS_PRESENT_DENIED => new(0xC0262007);

    /// <summary>Not able to present with color conversion.</summary>
    [Description("""Not able to present with color conversion.""")]
    [HResultConstant(0xC0262008)]
    public static HResult ERROR_GRAPHICS_CANNOTCOLORCONVERT => new(0xC0262008);

    /// <summary>Not enough video memory available to complete the operation.</summary>
    [Description("""Not enough video memory available to complete the operation.""")]
    [HResultConstant(0xC0262100)]
    public static HResult ERROR_GRAPHICS_NO_VIDEO_MEMORY => new(0xC0262100);

    /// <summary>Could not probe and lock the underlying memory of an allocation.</summary>
    [Description("""Could not probe and lock the underlying memory of an allocation.""")]
    [HResultConstant(0xC0262101)]
    public static HResult ERROR_GRAPHICS_CANT_LOCK_MEMORY => new(0xC0262101);

    /// <summary>The allocation is currently busy.</summary>
    [Description("""The allocation is currently busy.""")]
    [HResultConstant(0xC0262102)]
    public static HResult ERROR_GRAPHICS_ALLOCATION_BUSY => new(0xC0262102);

    /// <summary>An object being referenced has reach the maximum reference count already and cannot be referenced further.</summary>
    [Description("""An object being referenced has reach the maximum reference count already and cannot be referenced further.""")]
    [HResultConstant(0xC0262103)]
    public static HResult ERROR_GRAPHICS_TOO_MANY_REFERENCES => new(0xC0262103);

    /// <summary>A problem could not be solved due to some currently existing condition. The problem should be tried again later.</summary>
    [Description("""A problem could not be solved due to some currently existing condition. The problem should be tried again later.""")]
    [HResultConstant(0xC0262104)]
    public static HResult ERROR_GRAPHICS_TRY_AGAIN_LATER => new(0xC0262104);

    /// <summary>A problem could not be solved due to some currently existing condition. The problem should be tried again immediately.</summary>
    [Description("""A problem could not be solved due to some currently existing condition. The problem should be tried again immediately.""")]
    [HResultConstant(0xC0262105)]
    public static HResult ERROR_GRAPHICS_TRY_AGAIN_NOW => new(0xC0262105);

    /// <summary>The allocation is invalid.</summary>
    [Description("""The allocation is invalid.""")]
    [HResultConstant(0xC0262106)]
    public static HResult ERROR_GRAPHICS_ALLOCATION_INVALID => new(0xC0262106);

    /// <summary>No more unswizzling apertures are currently available.</summary>
    [Description("""No more unswizzling apertures are currently available.""")]
    [HResultConstant(0xC0262107)]
    public static HResult ERROR_GRAPHICS_UNSWIZZLING_APERTURE_UNAVAILABLE => new(0xC0262107);

    /// <summary>The current allocation cannot be unswizzled by an aperture.</summary>
    [Description("""The current allocation cannot be unswizzled by an aperture.""")]
    [HResultConstant(0xC0262108)]
    public static HResult ERROR_GRAPHICS_UNSWIZZLING_APERTURE_UNSUPPORTED => new(0xC0262108);

    /// <summary>The request failed because a pinned allocation cannot be evicted.</summary>
    [Description("""The request failed because a pinned allocation cannot be evicted.""")]
    [HResultConstant(0xC0262109)]
    public static HResult ERROR_GRAPHICS_CANT_EVICT_PINNED_ALLOCATION => new(0xC0262109);

    /// <summary>The allocation cannot be used from its current segment location for the specified operation.</summary>
    [Description("""The allocation cannot be used from its current segment location for the specified operation.""")]
    [HResultConstant(0xC0262110)]
    public static HResult ERROR_GRAPHICS_INVALID_ALLOCATION_USAGE => new(0xC0262110);

    /// <summary>A locked allocation cannot be used in the current command buffer.</summary>
    [Description("""A locked allocation cannot be used in the current command buffer.""")]
    [HResultConstant(0xC0262111)]
    public static HResult ERROR_GRAPHICS_CANT_RENDER_LOCKED_ALLOCATION => new(0xC0262111);

    /// <summary>The allocation being referenced has been closed permanently.</summary>
    [Description("""The allocation being referenced has been closed permanently.""")]
    [HResultConstant(0xC0262112)]
    public static HResult ERROR_GRAPHICS_ALLOCATION_CLOSED => new(0xC0262112);

    /// <summary>An invalid allocation instance is being referenced.</summary>
    [Description("""An invalid allocation instance is being referenced.""")]
    [HResultConstant(0xC0262113)]
    public static HResult ERROR_GRAPHICS_INVALID_ALLOCATION_INSTANCE => new(0xC0262113);

    /// <summary>An invalid allocation handle is being referenced.</summary>
    [Description("""An invalid allocation handle is being referenced.""")]
    [HResultConstant(0xC0262114)]
    public static HResult ERROR_GRAPHICS_INVALID_ALLOCATION_HANDLE => new(0xC0262114);

    /// <summary>The allocation being referenced does not belong to the current device.</summary>
    [Description("""The allocation being referenced does not belong to the current device.""")]
    [HResultConstant(0xC0262115)]
    public static HResult ERROR_GRAPHICS_WRONG_ALLOCATION_DEVICE => new(0xC0262115);

    /// <summary>The specified allocation lost its content.</summary>
    [Description("""The specified allocation lost its content.""")]
    [HResultConstant(0xC0262116)]
    public static HResult ERROR_GRAPHICS_ALLOCATION_CONTENT_LOST => new(0xC0262116);

    /// <summary>Graphics processing unit (GPU) exception is detected on the given device. The device is not able to be scheduled.</summary>
    [Description("""Graphics processing unit (GPU) exception is detected on the given device. The device is not able to be scheduled.""")]
    [HResultConstant(0xC0262200)]
    public static HResult ERROR_GRAPHICS_GPU_EXCEPTION_ON_DEVICE => new(0xC0262200);

    /// <summary>Specified video present network (VidPN) topology is invalid.</summary>
    [Description("""Specified video present network (VidPN) topology is invalid.""")]
    [HResultConstant(0xC0262300)]
    public static HResult ERROR_GRAPHICS_INVALID_VIDPN_TOPOLOGY => new(0xC0262300);

    /// <summary>Specified VidPN topology is valid but is not supported by this model of the display adapter.</summary>
    [Description("""Specified VidPN topology is valid but is not supported by this model of the display adapter.""")]
    [HResultConstant(0xC0262301)]
    public static HResult ERROR_GRAPHICS_VIDPN_TOPOLOGY_NOT_SUPPORTED => new(0xC0262301);

    /// <summary>Specified VidPN topology is valid but is not supported by the display adapter at this time, due to current allocation of its resources.</summary>
    [Description("""Specified VidPN topology is valid but is not supported by the display adapter at this time, due to current allocation of its resources.""")]
    [HResultConstant(0xC0262302)]
    public static HResult ERROR_GRAPHICS_VIDPN_TOPOLOGY_CURRENTLY_NOT_SUPPORTED => new(0xC0262302);

    /// <summary>Specified VidPN handle is invalid.</summary>
    [Description("""Specified VidPN handle is invalid.""")]
    [HResultConstant(0xC0262303)]
    public static HResult ERROR_GRAPHICS_INVALID_VIDPN => new(0xC0262303);

    /// <summary>Specified video present source is invalid.</summary>
    [Description("""Specified video present source is invalid.""")]
    [HResultConstant(0xC0262304)]
    public static HResult ERROR_GRAPHICS_INVALID_VIDEO_PRESENT_SOURCE => new(0xC0262304);

    /// <summary>Specified video present target is invalid.</summary>
    [Description("""Specified video present target is invalid.""")]
    [HResultConstant(0xC0262305)]
    public static HResult ERROR_GRAPHICS_INVALID_VIDEO_PRESENT_TARGET => new(0xC0262305);

    /// <summary>Specified VidPN modality is not supported (for example, at least two of the pinned modes are not cofunctional).</summary>
    [Description("""Specified VidPN modality is not supported (for example, at least two of the pinned modes are not cofunctional).""")]
    [HResultConstant(0xC0262306)]
    public static HResult ERROR_GRAPHICS_VIDPN_MODALITY_NOT_SUPPORTED => new(0xC0262306);

    /// <summary>Specified VidPN source mode set is invalid.</summary>
    [Description("""Specified VidPN source mode set is invalid.""")]
    [HResultConstant(0xC0262308)]
    public static HResult ERROR_GRAPHICS_INVALID_VIDPN_SOURCEMODESET => new(0xC0262308);

    /// <summary>Specified VidPN target mode set is invalid.</summary>
    [Description("""Specified VidPN target mode set is invalid.""")]
    [HResultConstant(0xC0262309)]
    public static HResult ERROR_GRAPHICS_INVALID_VIDPN_TARGETMODESET => new(0xC0262309);

    /// <summary>Specified video signal frequency is invalid.</summary>
    [Description("""Specified video signal frequency is invalid.""")]
    [HResultConstant(0xC026230A)]
    public static HResult ERROR_GRAPHICS_INVALID_FREQUENCY => new(0xC026230A);

    /// <summary>Specified video signal active region is invalid.</summary>
    [Description("""Specified video signal active region is invalid.""")]
    [HResultConstant(0xC026230B)]
    public static HResult ERROR_GRAPHICS_INVALID_ACTIVE_REGION => new(0xC026230B);

    /// <summary>Specified video signal total region is invalid.</summary>
    [Description("""Specified video signal total region is invalid.""")]
    [HResultConstant(0xC026230C)]
    public static HResult ERROR_GRAPHICS_INVALID_TOTAL_REGION => new(0xC026230C);

    /// <summary>Specified video present source mode is invalid.</summary>
    [Description("""Specified video present source mode is invalid.""")]
    [HResultConstant(0xC0262310)]
    public static HResult ERROR_GRAPHICS_INVALID_VIDEO_PRESENT_SOURCE_MODE => new(0xC0262310);

    /// <summary>Specified video present target mode is invalid.</summary>
    [Description("""Specified video present target mode is invalid.""")]
    [HResultConstant(0xC0262311)]
    public static HResult ERROR_GRAPHICS_INVALID_VIDEO_PRESENT_TARGET_MODE => new(0xC0262311);

    /// <summary>Pinned mode must remain in the set on VidPN's cofunctional modality enumeration.</summary>
    [Description("""Pinned mode must remain in the set on VidPN's cofunctional modality enumeration.""")]
    [HResultConstant(0xC0262312)]
    public static HResult ERROR_GRAPHICS_PINNED_MODE_MUST_REMAIN_IN_SET => new(0xC0262312);

    /// <summary>Specified video present path is already in the VidPN topology.</summary>
    [Description("""Specified video present path is already in the VidPN topology.""")]
    [HResultConstant(0xC0262313)]
    public static HResult ERROR_GRAPHICS_PATH_ALREADY_IN_TOPOLOGY => new(0xC0262313);

    /// <summary>Specified mode is already in the mode set.</summary>
    [Description("""Specified mode is already in the mode set.""")]
    [HResultConstant(0xC0262314)]
    public static HResult ERROR_GRAPHICS_MODE_ALREADY_IN_MODESET => new(0xC0262314);

    /// <summary>Specified video present source set is invalid.</summary>
    [Description("""Specified video present source set is invalid.""")]
    [HResultConstant(0xC0262315)]
    public static HResult ERROR_GRAPHICS_INVALID_VIDEOPRESENTSOURCESET => new(0xC0262315);

    /// <summary>Specified video present target set is invalid.</summary>
    [Description("""Specified video present target set is invalid.""")]
    [HResultConstant(0xC0262316)]
    public static HResult ERROR_GRAPHICS_INVALID_VIDEOPRESENTTARGETSET => new(0xC0262316);

    /// <summary>Specified video present source is already in the video present source set.</summary>
    [Description("""Specified video present source is already in the video present source set.""")]
    [HResultConstant(0xC0262317)]
    public static HResult ERROR_GRAPHICS_SOURCE_ALREADY_IN_SET => new(0xC0262317);

    /// <summary>Specified video present target is already in the video present target set.</summary>
    [Description("""Specified video present target is already in the video present target set.""")]
    [HResultConstant(0xC0262318)]
    public static HResult ERROR_GRAPHICS_TARGET_ALREADY_IN_SET => new(0xC0262318);

    /// <summary>Specified VidPN present path is invalid.</summary>
    [Description("""Specified VidPN present path is invalid.""")]
    [HResultConstant(0xC0262319)]
    public static HResult ERROR_GRAPHICS_INVALID_VIDPN_PRESENT_PATH => new(0xC0262319);

    /// <summary>Miniport has no recommendation for augmentation of the specified VidPN topology.</summary>
    [Description("""Miniport has no recommendation for augmentation of the specified VidPN topology.""")]
    [HResultConstant(0xC026231A)]
    public static HResult ERROR_GRAPHICS_NO_RECOMMENDED_VIDPN_TOPOLOGY => new(0xC026231A);

    /// <summary>Specified monitor frequency range set is invalid.</summary>
    [Description("""Specified monitor frequency range set is invalid.""")]
    [HResultConstant(0xC026231B)]
    public static HResult ERROR_GRAPHICS_INVALID_MONITOR_FREQUENCYRANGESET => new(0xC026231B);

    /// <summary>Specified monitor frequency range is invalid.</summary>
    [Description("""Specified monitor frequency range is invalid.""")]
    [HResultConstant(0xC026231C)]
    public static HResult ERROR_GRAPHICS_INVALID_MONITOR_FREQUENCYRANGE => new(0xC026231C);

    /// <summary>Specified frequency range is not in the specified monitor frequency range set.</summary>
    [Description("""Specified frequency range is not in the specified monitor frequency range set.""")]
    [HResultConstant(0xC026231D)]
    public static HResult ERROR_GRAPHICS_FREQUENCYRANGE_NOT_IN_SET => new(0xC026231D);

    /// <summary>Specified frequency range is already in the specified monitor frequency range set.</summary>
    [Description("""Specified frequency range is already in the specified monitor frequency range set.""")]
    [HResultConstant(0xC026231F)]
    public static HResult ERROR_GRAPHICS_FREQUENCYRANGE_ALREADY_IN_SET => new(0xC026231F);

    /// <summary>Specified mode set is stale. Reacquire the new mode set.</summary>
    [Description("""Specified mode set is stale. Reacquire the new mode set.""")]
    [HResultConstant(0xC0262320)]
    public static HResult ERROR_GRAPHICS_STALE_MODESET => new(0xC0262320);

    /// <summary>Specified monitor source mode set is invalid.</summary>
    [Description("""Specified monitor source mode set is invalid.""")]
    [HResultConstant(0xC0262321)]
    public static HResult ERROR_GRAPHICS_INVALID_MONITOR_SOURCEMODESET => new(0xC0262321);

    /// <summary>Specified monitor source mode is invalid.</summary>
    [Description("""Specified monitor source mode is invalid.""")]
    [HResultConstant(0xC0262322)]
    public static HResult ERROR_GRAPHICS_INVALID_MONITOR_SOURCE_MODE => new(0xC0262322);

    /// <summary>Miniport does not have any recommendation regarding the request to provide a functional VidPN given the current display adapter configuration.</summary>
    [Description("""Miniport does not have any recommendation regarding the request to provide a functional VidPN given the current display adapter configuration.""")]
    [HResultConstant(0xC0262323)]
    public static HResult ERROR_GRAPHICS_NO_RECOMMENDED_FUNCTIONAL_VIDPN => new(0xC0262323);

    /// <summary>ID of the specified mode is already used by another mode in the set.</summary>
    [Description("""ID of the specified mode is already used by another mode in the set.""")]
    [HResultConstant(0xC0262324)]
    public static HResult ERROR_GRAPHICS_MODE_ID_MUST_BE_UNIQUE => new(0xC0262324);

    /// <summary>System failed to determine a mode that is supported by both the display adapter and the monitor connected to it.</summary>
    [Description("""System failed to determine a mode that is supported by both the display adapter and the monitor connected to it.""")]
    [HResultConstant(0xC0262325)]
    public static HResult ERROR_GRAPHICS_EMPTY_ADAPTER_MONITOR_MODE_SUPPORT_INTERSECTION => new(0xC0262325);

    /// <summary>Number of video present targets must be greater than or equal to the number of video present sources.</summary>
    [Description("""Number of video present targets must be greater than or equal to the number of video present sources.""")]
    [HResultConstant(0xC0262326)]
    public static HResult ERROR_GRAPHICS_VIDEO_PRESENT_TARGETS_LESS_THAN_SOURCES => new(0xC0262326);

    /// <summary>Specified present path is not in the VidPN topology.</summary>
    [Description("""Specified present path is not in the VidPN topology.""")]
    [HResultConstant(0xC0262327)]
    public static HResult ERROR_GRAPHICS_PATH_NOT_IN_TOPOLOGY => new(0xC0262327);

    /// <summary>Display adapter must have at least one video present source.</summary>
    [Description("""Display adapter must have at least one video present source.""")]
    [HResultConstant(0xC0262328)]
    public static HResult ERROR_GRAPHICS_ADAPTER_MUST_HAVE_AT_LEAST_ONE_SOURCE => new(0xC0262328);

    /// <summary>Display adapter must have at least one video present target.</summary>
    [Description("""Display adapter must have at least one video present target.""")]
    [HResultConstant(0xC0262329)]
    public static HResult ERROR_GRAPHICS_ADAPTER_MUST_HAVE_AT_LEAST_ONE_TARGET => new(0xC0262329);

    /// <summary>Specified monitor descriptor set is invalid.</summary>
    [Description("""Specified monitor descriptor set is invalid.""")]
    [HResultConstant(0xC026232A)]
    public static HResult ERROR_GRAPHICS_INVALID_MONITORDESCRIPTORSET => new(0xC026232A);

    /// <summary>Specified monitor descriptor is invalid.</summary>
    [Description("""Specified monitor descriptor is invalid.""")]
    [HResultConstant(0xC026232B)]
    public static HResult ERROR_GRAPHICS_INVALID_MONITORDESCRIPTOR => new(0xC026232B);

    /// <summary>Specified descriptor is not in the specified monitor descriptor set.</summary>
    [Description("""Specified descriptor is not in the specified monitor descriptor set.""")]
    [HResultConstant(0xC026232C)]
    public static HResult ERROR_GRAPHICS_MONITORDESCRIPTOR_NOT_IN_SET => new(0xC026232C);

    /// <summary>Specified descriptor is already in the specified monitor descriptor set.</summary>
    [Description("""Specified descriptor is already in the specified monitor descriptor set.""")]
    [HResultConstant(0xC026232D)]
    public static HResult ERROR_GRAPHICS_MONITORDESCRIPTOR_ALREADY_IN_SET => new(0xC026232D);

    /// <summary>ID of the specified monitor descriptor is already used by another descriptor in the set.</summary>
    [Description("""ID of the specified monitor descriptor is already used by another descriptor in the set.""")]
    [HResultConstant(0xC026232E)]
    public static HResult ERROR_GRAPHICS_MONITORDESCRIPTOR_ID_MUST_BE_UNIQUE => new(0xC026232E);

    /// <summary>Specified video present target subset type is invalid.</summary>
    [Description("""Specified video present target subset type is invalid.""")]
    [HResultConstant(0xC026232F)]
    public static HResult ERROR_GRAPHICS_INVALID_VIDPN_TARGET_SUBSET_TYPE => new(0xC026232F);

    /// <summary>Two or more of the specified resources are not related to each other, as defined by the interface semantics.</summary>
    [Description("""Two or more of the specified resources are not related to each other, as defined by the interface semantics.""")]
    [HResultConstant(0xC0262330)]
    public static HResult ERROR_GRAPHICS_RESOURCES_NOT_RELATED => new(0xC0262330);

    /// <summary>ID of the specified video present source is already used by another source in the set.</summary>
    [Description("""ID of the specified video present source is already used by another source in the set.""")]
    [HResultConstant(0xC0262331)]
    public static HResult ERROR_GRAPHICS_SOURCE_ID_MUST_BE_UNIQUE => new(0xC0262331);

    /// <summary>ID of the specified video present target is already used by another target in the set.</summary>
    [Description("""ID of the specified video present target is already used by another target in the set.""")]
    [HResultConstant(0xC0262332)]
    public static HResult ERROR_GRAPHICS_TARGET_ID_MUST_BE_UNIQUE => new(0xC0262332);

    /// <summary>Specified VidPN source cannot be used because there is no available VidPN target to connect it to.</summary>
    [Description("""Specified VidPN source cannot be used because there is no available VidPN target to connect it to.""")]
    [HResultConstant(0xC0262333)]
    public static HResult ERROR_GRAPHICS_NO_AVAILABLE_VIDPN_TARGET => new(0xC0262333);

    /// <summary>Newly arrived monitor could not be associated with a display adapter.</summary>
    [Description("""Newly arrived monitor could not be associated with a display adapter.""")]
    [HResultConstant(0xC0262334)]
    public static HResult ERROR_GRAPHICS_MONITOR_COULD_NOT_BE_ASSOCIATED_WITH_ADAPTER => new(0xC0262334);

    /// <summary>Display adapter in question does not have an associated VidPN manager.</summary>
    [Description("""Display adapter in question does not have an associated VidPN manager.""")]
    [HResultConstant(0xC0262335)]
    public static HResult ERROR_GRAPHICS_NO_VIDPNMGR => new(0xC0262335);

    /// <summary>VidPN manager of the display adapter in question does not have an active VidPN.</summary>
    [Description("""VidPN manager of the display adapter in question does not have an active VidPN.""")]
    [HResultConstant(0xC0262336)]
    public static HResult ERROR_GRAPHICS_NO_ACTIVE_VIDPN => new(0xC0262336);

    /// <summary>Specified VidPN topology is stale. Re-acquire the new topology.</summary>
    [Description("""Specified VidPN topology is stale. Re-acquire the new topology.""")]
    [HResultConstant(0xC0262337)]
    public static HResult ERROR_GRAPHICS_STALE_VIDPN_TOPOLOGY => new(0xC0262337);

    /// <summary>There is no monitor connected on the specified video present target.</summary>
    [Description("""There is no monitor connected on the specified video present target.""")]
    [HResultConstant(0xC0262338)]
    public static HResult ERROR_GRAPHICS_MONITOR_NOT_CONNECTED => new(0xC0262338);

    /// <summary>Specified source is not part of the specified VidPN topology.</summary>
    [Description("""Specified source is not part of the specified VidPN topology.""")]
    [HResultConstant(0xC0262339)]
    public static HResult ERROR_GRAPHICS_SOURCE_NOT_IN_TOPOLOGY => new(0xC0262339);

    /// <summary>Specified primary surface size is invalid.</summary>
    [Description("""Specified primary surface size is invalid.""")]
    [HResultConstant(0xC026233A)]
    public static HResult ERROR_GRAPHICS_INVALID_PRIMARYSURFACE_SIZE => new(0xC026233A);

    /// <summary>Specified visible region size is invalid.</summary>
    [Description("""Specified visible region size is invalid.""")]
    [HResultConstant(0xC026233B)]
    public static HResult ERROR_GRAPHICS_INVALID_VISIBLEREGION_SIZE => new(0xC026233B);

    /// <summary>Specified stride is invalid.</summary>
    [Description("""Specified stride is invalid.""")]
    [HResultConstant(0xC026233C)]
    public static HResult ERROR_GRAPHICS_INVALID_STRIDE => new(0xC026233C);

    /// <summary>Specified pixel format is invalid.</summary>
    [Description("""Specified pixel format is invalid.""")]
    [HResultConstant(0xC026233D)]
    public static HResult ERROR_GRAPHICS_INVALID_PIXELFORMAT => new(0xC026233D);

    /// <summary>Specified color basis is invalid.</summary>
    [Description("""Specified color basis is invalid.""")]
    [HResultConstant(0xC026233E)]
    public static HResult ERROR_GRAPHICS_INVALID_COLORBASIS => new(0xC026233E);

    /// <summary>Specified pixel value access mode is invalid.</summary>
    [Description("""Specified pixel value access mode is invalid.""")]
    [HResultConstant(0xC026233F)]
    public static HResult ERROR_GRAPHICS_INVALID_PIXELVALUEACCESSMODE => new(0xC026233F);

    /// <summary>Specified target is not part of the specified VidPN topology.</summary>
    [Description("""Specified target is not part of the specified VidPN topology.""")]
    [HResultConstant(0xC0262340)]
    public static HResult ERROR_GRAPHICS_TARGET_NOT_IN_TOPOLOGY => new(0xC0262340);

    /// <summary>Failed to acquire display mode management interface.</summary>
    [Description("""Failed to acquire display mode management interface.""")]
    [HResultConstant(0xC0262341)]
    public static HResult ERROR_GRAPHICS_NO_DISPLAY_MODE_MANAGEMENT_SUPPORT => new(0xC0262341);

    /// <summary>Specified VidPN source is already owned by a display mode manager (DMM) client and cannot be used until that client releases it.</summary>
    [Description("""Specified VidPN source is already owned by a display mode manager (DMM) client and cannot be used until that client releases it.""")]
    [HResultConstant(0xC0262342)]
    public static HResult ERROR_GRAPHICS_VIDPN_SOURCE_IN_USE => new(0xC0262342);

    /// <summary>Specified VidPN is active and cannot be accessed.</summary>
    [Description("""Specified VidPN is active and cannot be accessed.""")]
    [HResultConstant(0xC0262343)]
    public static HResult ERROR_GRAPHICS_CANT_ACCESS_ACTIVE_VIDPN => new(0xC0262343);

    /// <summary>Specified VidPN present path importance ordinal is invalid.</summary>
    [Description("""Specified VidPN present path importance ordinal is invalid.""")]
    [HResultConstant(0xC0262344)]
    public static HResult ERROR_GRAPHICS_INVALID_PATH_IMPORTANCE_ORDINAL => new(0xC0262344);

    /// <summary>Specified VidPN present path content geometry transformation is invalid.</summary>
    [Description("""Specified VidPN present path content geometry transformation is invalid.""")]
    [HResultConstant(0xC0262345)]
    public static HResult ERROR_GRAPHICS_INVALID_PATH_CONTENT_GEOMETRY_TRANSFORMATION => new(0xC0262345);

    /// <summary>Specified content geometry transformation is not supported on the respective VidPN present path.</summary>
    [Description("""Specified content geometry transformation is not supported on the respective VidPN present path.""")]
    [HResultConstant(0xC0262346)]
    public static HResult ERROR_GRAPHICS_PATH_CONTENT_GEOMETRY_TRANSFORMATION_NOT_SUPPORTED => new(0xC0262346);

    /// <summary>Specified gamma ramp is invalid.</summary>
    [Description("""Specified gamma ramp is invalid.""")]
    [HResultConstant(0xC0262347)]
    public static HResult ERROR_GRAPHICS_INVALID_GAMMA_RAMP => new(0xC0262347);

    /// <summary>Specified gamma ramp is not supported on the respective VidPN present path.</summary>
    [Description("""Specified gamma ramp is not supported on the respective VidPN present path.""")]
    [HResultConstant(0xC0262348)]
    public static HResult ERROR_GRAPHICS_GAMMA_RAMP_NOT_SUPPORTED => new(0xC0262348);

    /// <summary>Multisampling is not supported on the respective VidPN present path.</summary>
    [Description("""Multisampling is not supported on the respective VidPN present path.""")]
    [HResultConstant(0xC0262349)]
    public static HResult ERROR_GRAPHICS_MULTISAMPLING_NOT_SUPPORTED => new(0xC0262349);

    /// <summary>Specified mode is not in the specified mode set.</summary>
    [Description("""Specified mode is not in the specified mode set.""")]
    [HResultConstant(0xC026234A)]
    public static HResult ERROR_GRAPHICS_MODE_NOT_IN_MODESET => new(0xC026234A);

    /// <summary>Specified VidPN topology recommendation reason is invalid.</summary>
    [Description("""Specified VidPN topology recommendation reason is invalid.""")]
    [HResultConstant(0xC026234D)]
    public static HResult ERROR_GRAPHICS_INVALID_VIDPN_TOPOLOGY_RECOMMENDATION_REASON => new(0xC026234D);

    /// <summary>Specified VidPN present path content type is invalid.</summary>
    [Description("""Specified VidPN present path content type is invalid.""")]
    [HResultConstant(0xC026234E)]
    public static HResult ERROR_GRAPHICS_INVALID_PATH_CONTENT_TYPE => new(0xC026234E);

    /// <summary>Specified VidPN present path copy protection type is invalid.</summary>
    [Description("""Specified VidPN present path copy protection type is invalid.""")]
    [HResultConstant(0xC026234F)]
    public static HResult ERROR_GRAPHICS_INVALID_COPYPROTECTION_TYPE => new(0xC026234F);

    /// <summary>No more than one unassigned mode set can exist at any given time for a given VidPN source or target.</summary>
    [Description("""No more than one unassigned mode set can exist at any given time for a given VidPN source or target.""")]
    [HResultConstant(0xC0262350)]
    public static HResult ERROR_GRAPHICS_UNASSIGNED_MODESET_ALREADY_EXISTS => new(0xC0262350);

    /// <summary>The specified scan line ordering type is invalid.</summary>
    [Description("""The specified scan line ordering type is invalid.""")]
    [HResultConstant(0xC0262352)]
    public static HResult ERROR_GRAPHICS_INVALID_SCANLINE_ORDERING => new(0xC0262352);

    /// <summary>Topology changes are not allowed for the specified VidPN.</summary>
    [Description("""Topology changes are not allowed for the specified VidPN.""")]
    [HResultConstant(0xC0262353)]
    public static HResult ERROR_GRAPHICS_TOPOLOGY_CHANGES_NOT_ALLOWED => new(0xC0262353);

    /// <summary>All available importance ordinals are already used in the specified topology.</summary>
    [Description("""All available importance ordinals are already used in the specified topology.""")]
    [HResultConstant(0xC0262354)]
    public static HResult ERROR_GRAPHICS_NO_AVAILABLE_IMPORTANCE_ORDINALS => new(0xC0262354);

    /// <summary>Specified primary surface has a different private format attribute than the current primary surface.</summary>
    [Description("""Specified primary surface has a different private format attribute than the current primary surface.""")]
    [HResultConstant(0xC0262355)]
    public static HResult ERROR_GRAPHICS_INCOMPATIBLE_PRIVATE_FORMAT => new(0xC0262355);

    /// <summary>Specified mode pruning algorithm is invalid.</summary>
    [Description("""Specified mode pruning algorithm is invalid.""")]
    [HResultConstant(0xC0262356)]
    public static HResult ERROR_GRAPHICS_INVALID_MODE_PRUNING_ALGORITHM => new(0xC0262356);

    /// <summary>Specified display adapter child device already has an external device connected to it.</summary>
    [Description("""Specified display adapter child device already has an external device connected to it.""")]
    [HResultConstant(0xC0262400)]
    public static HResult ERROR_GRAPHICS_SPECIFIED_CHILD_ALREADY_CONNECTED => new(0xC0262400);

    /// <summary>The display adapter child device does not support reporting a descriptor.</summary>
    [Description("""The display adapter child device does not support reporting a descriptor.""")]
    [HResultConstant(0xC0262401)]
    public static HResult ERROR_GRAPHICS_CHILD_DESCRIPTOR_NOT_SUPPORTED => new(0xC0262401);

    /// <summary>The display adapter is not linked to any other adapters.</summary>
    [Description("""The display adapter is not linked to any other adapters.""")]
    [HResultConstant(0xC0262430)]
    public static HResult ERROR_GRAPHICS_NOT_A_LINKED_ADAPTER => new(0xC0262430);

    /// <summary>Lead adapter in a linked configuration was not enumerated yet.</summary>
    [Description("""Lead adapter in a linked configuration was not enumerated yet.""")]
    [HResultConstant(0xC0262431)]
    public static HResult ERROR_GRAPHICS_LEADLINK_NOT_ENUMERATED => new(0xC0262431);

    /// <summary>Some chain adapters in a linked configuration were not enumerated yet.</summary>
    [Description("""Some chain adapters in a linked configuration were not enumerated yet.""")]
    [HResultConstant(0xC0262432)]
    public static HResult ERROR_GRAPHICS_CHAINLINKS_NOT_ENUMERATED => new(0xC0262432);

    /// <summary>The chain of linked adapters is not ready to start because of an unknown failure.</summary>
    [Description("""The chain of linked adapters is not ready to start because of an unknown failure.""")]
    [HResultConstant(0xC0262433)]
    public static HResult ERROR_GRAPHICS_ADAPTER_CHAIN_NOT_READY => new(0xC0262433);

    /// <summary>An attempt was made to start a lead link display adapter when the chain links were not started yet.</summary>
    [Description("""An attempt was made to start a lead link display adapter when the chain links were not started yet.""")]
    [HResultConstant(0xC0262434)]
    public static HResult ERROR_GRAPHICS_CHAINLINKS_NOT_STARTED => new(0xC0262434);

    /// <summary>An attempt was made to turn on a lead link display adapter when the chain links were turned off.</summary>
    [Description("""An attempt was made to turn on a lead link display adapter when the chain links were turned off.""")]
    [HResultConstant(0xC0262435)]
    public static HResult ERROR_GRAPHICS_CHAINLINKS_NOT_POWERED_ON => new(0xC0262435);

    /// <summary>The adapter link was found to be in an inconsistent state. Not all adapters are in an expected PNP or power state.</summary>
    [Description("""The adapter link was found to be in an inconsistent state. Not all adapters are in an expected PNP or power state.""")]
    [HResultConstant(0xC0262436)]
    public static HResult ERROR_GRAPHICS_INCONSISTENT_DEVICE_LINK_STATE => new(0xC0262436);

    /// <summary>The driver trying to start is not the same as the driver for the posted display adapter.</summary>
    [Description("""The driver trying to start is not the same as the driver for the posted display adapter.""")]
    [HResultConstant(0xC0262438)]
    public static HResult ERROR_GRAPHICS_NOT_POST_DEVICE_DRIVER => new(0xC0262438);

    /// <summary>The driver does not support Output Protection Manager (OPM).</summary>
    [Description("""The driver does not support Output Protection Manager (OPM).""")]
    [HResultConstant(0xC0262500)]
    public static HResult ERROR_GRAPHICS_OPM_NOT_SUPPORTED => new(0xC0262500);

    /// <summary>The driver does not support Certified Output Protection Protocol (COPP).</summary>
    [Description("""The driver does not support Certified Output Protection Protocol (COPP).""")]
    [HResultConstant(0xC0262501)]
    public static HResult ERROR_GRAPHICS_COPP_NOT_SUPPORTED => new(0xC0262501);

    /// <summary>The driver does not support a user-accessible bus (UAB).</summary>
    [Description("""The driver does not support a user-accessible bus (UAB).""")]
    [HResultConstant(0xC0262502)]
    public static HResult ERROR_GRAPHICS_UAB_NOT_SUPPORTED => new(0xC0262502);

    /// <summary>The specified encrypted parameters are invalid.</summary>
    [Description("""The specified encrypted parameters are invalid.""")]
    [HResultConstant(0xC0262503)]
    public static HResult ERROR_GRAPHICS_OPM_INVALID_ENCRYPTED_PARAMETERS => new(0xC0262503);

    /// <summary>An array passed to a function cannot hold all of the data that the function wants to put in it.</summary>
    [Description("""An array passed to a function cannot hold all of the data that the function wants to put in it.""")]
    [HResultConstant(0xC0262504)]
    public static HResult ERROR_GRAPHICS_OPM_PARAMETER_ARRAY_TOO_SMALL => new(0xC0262504);

    /// <summary>The GDI display device passed to this function does not have any active video outputs.</summary>
    [Description("""The GDI display device passed to this function does not have any active video outputs.""")]
    [HResultConstant(0xC0262505)]
    public static HResult ERROR_GRAPHICS_OPM_NO_VIDEO_OUTPUTS_EXIST => new(0xC0262505);

    /// <summary>The protected video path (PVP) cannot find an actual GDI display device that corresponds to the passed-in GDI display device name.</summary>
    [Description("""The protected video path (PVP) cannot find an actual GDI display device that corresponds to the passed-in GDI display device name.""")]
    [HResultConstant(0xC0262506)]
    public static HResult ERROR_GRAPHICS_PVP_NO_DISPLAY_DEVICE_CORRESPONDS_TO_NAME => new(0xC0262506);

    /// <summary>This function failed because the GDI display device passed to it was not attached to the Windows desktop.</summary>
    [Description("""This function failed because the GDI display device passed to it was not attached to the Windows desktop.""")]
    [HResultConstant(0xC0262507)]
    public static HResult ERROR_GRAPHICS_PVP_DISPLAY_DEVICE_NOT_ATTACHED_TO_DESKTOP => new(0xC0262507);

    /// <summary>The PVP does not support mirroring display devices because they do not have video outputs.</summary>
    [Description("""The PVP does not support mirroring display devices because they do not have video outputs.""")]
    [HResultConstant(0xC0262508)]
    public static HResult ERROR_GRAPHICS_PVP_MIRRORING_DEVICES_NOT_SUPPORTED => new(0xC0262508);

    /// <summary>The function failed because an invalid pointer parameter was passed to it. A pointer parameter is invalid if it is null, it points to an invalid address, it points to a kernel mode address, or it is not correctly aligned.</summary>
    [Description("""The function failed because an invalid pointer parameter was passed to it. A pointer parameter is invalid if it is null, it points to an invalid address, it points to a kernel mode address, or it is not correctly aligned.""")]
    [HResultConstant(0xC026250A)]
    public static HResult ERROR_GRAPHICS_OPM_INVALID_POINTER => new(0xC026250A);

    /// <summary>An internal error caused this operation to fail.</summary>
    [Description("""An internal error caused this operation to fail.""")]
    [HResultConstant(0xC026250B)]
    public static HResult ERROR_GRAPHICS_OPM_INTERNAL_ERROR => new(0xC026250B);

    /// <summary>The function failed because the caller passed in an invalid OPM user mode handle.</summary>
    [Description("""The function failed because the caller passed in an invalid OPM user mode handle.""")]
    [HResultConstant(0xC026250C)]
    public static HResult ERROR_GRAPHICS_OPM_INVALID_HANDLE => new(0xC026250C);

    /// <summary>This function failed because the GDI device passed to it did not have any monitors associated with it.</summary>
    [Description("""This function failed because the GDI device passed to it did not have any monitors associated with it.""")]
    [HResultConstant(0xC026250D)]
    public static HResult ERROR_GRAPHICS_PVP_NO_MONITORS_CORRESPOND_TO_DISPLAY_DEVICE => new(0xC026250D);

    /// <summary>A certificate could not be returned because the certificate buffer passed to the function was too small.</summary>
    [Description("""A certificate could not be returned because the certificate buffer passed to the function was too small.""")]
    [HResultConstant(0xC026250E)]
    public static HResult ERROR_GRAPHICS_PVP_INVALID_CERTIFICATE_LENGTH => new(0xC026250E);

    /// <summary>A video output could not be created because the frame buffer is in spanning mode.</summary>
    [Description("""A video output could not be created because the frame buffer is in spanning mode.""")]
    [HResultConstant(0xC026250F)]
    public static HResult ERROR_GRAPHICS_OPM_SPANNING_MODE_ENABLED => new(0xC026250F);

    /// <summary>A video output could not be created because the frame buffer is in theater mode.</summary>
    [Description("""A video output could not be created because the frame buffer is in theater mode.""")]
    [HResultConstant(0xC0262510)]
    public static HResult ERROR_GRAPHICS_OPM_THEATER_MODE_ENABLED => new(0xC0262510);

    /// <summary>The function call failed because the display adapter's hardware functionality scan failed to validate the graphics hardware.</summary>
    [Description("""The function call failed because the display adapter's hardware functionality scan failed to validate the graphics hardware.""")]
    [HResultConstant(0xC0262511)]
    public static HResult ERROR_GRAPHICS_PVP_HFS_FAILED => new(0xC0262511);

    /// <summary>The High-Bandwidth Digital Content Protection (HDCP) System Renewability Message (SRM) passed to this function did not comply with section 5 of the HDCP 1.1 specification.</summary>
    [Description("""The High-Bandwidth Digital Content Protection (HDCP) System Renewability Message (SRM) passed to this function did not comply with section 5 of the HDCP 1.1 specification.""")]
    [HResultConstant(0xC0262512)]
    public static HResult ERROR_GRAPHICS_OPM_INVALID_SRM => new(0xC0262512);

    /// <summary>The video output cannot enable the HDCP system because it does not support it.</summary>
    [Description("""The video output cannot enable the HDCP system because it does not support it.""")]
    [HResultConstant(0xC0262513)]
    public static HResult ERROR_GRAPHICS_OPM_OUTPUT_DOES_NOT_SUPPORT_HDCP => new(0xC0262513);

    /// <summary>The video output cannot enable analog copy protection because it does not support it.</summary>
    [Description("""The video output cannot enable analog copy protection because it does not support it.""")]
    [HResultConstant(0xC0262514)]
    public static HResult ERROR_GRAPHICS_OPM_OUTPUT_DOES_NOT_SUPPORT_ACP => new(0xC0262514);

    /// <summary>The video output cannot enable the Content Generation Management System Analog (CGMS-A) protection technology because it does not support it.</summary>
    [Description("""The video output cannot enable the Content Generation Management System Analog (CGMS-A) protection technology because it does not support it.""")]
    [HResultConstant(0xC0262515)]
    public static HResult ERROR_GRAPHICS_OPM_OUTPUT_DOES_NOT_SUPPORT_CGMSA => new(0xC0262515);

    /// <summary>IOPMVideoOutput's GetInformation() method cannot return the version of the SRM being used because the application never successfully passed an SRM to the video output.</summary>
    [Description("""IOPMVideoOutput's GetInformation() method cannot return the version of the SRM being used because the application never successfully passed an SRM to the video output.""")]
    [HResultConstant(0xC0262516)]
    public static HResult ERROR_GRAPHICS_OPM_HDCP_SRM_NEVER_SET => new(0xC0262516);

    /// <summary>IOPMVideoOutput's Configure() method cannot enable the specified output protection technology because the output's screen resolution is too high.</summary>
    [Description("""IOPMVideoOutput's Configure() method cannot enable the specified output protection technology because the output's screen resolution is too high.""")]
    [HResultConstant(0xC0262517)]
    public static HResult ERROR_GRAPHICS_OPM_RESOLUTION_TOO_HIGH => new(0xC0262517);

    /// <summary>IOPMVideoOutput's Configure() method cannot enable HDCP because the display adapter's HDCP hardware is already being used by other physical outputs.</summary>
    [Description("""IOPMVideoOutput's Configure() method cannot enable HDCP because the display adapter's HDCP hardware is already being used by other physical outputs.""")]
    [HResultConstant(0xC0262518)]
    public static HResult ERROR_GRAPHICS_OPM_ALL_HDCP_HARDWARE_ALREADY_IN_USE => new(0xC0262518);

    /// <summary>The operating system asynchronously destroyed this OPM video output because the operating system's state changed. This error typically occurs because the monitor physical device object (PDO) associated with this video output was removed, the monitor PDO associated with this video output was stopped, the video output's session became a nonconsole session or the video output's desktop became an inactive desktop.</summary>
    [Description("""The operating system asynchronously destroyed this OPM video output because the operating system's state changed. This error typically occurs because the monitor physical device object (PDO) associated with this video output was removed, the monitor PDO associated with this video output was stopped, the video output's session became a nonconsole session or the video output's desktop became an inactive desktop.""")]
    [HResultConstant(0xC0262519)]
    public static HResult ERROR_GRAPHICS_OPM_VIDEO_OUTPUT_NO_LONGER_EXISTS => new(0xC0262519);

    /// <summary>IOPMVideoOutput's methods cannot be called when a session is changing its type. There are currently three types of sessions: console, disconnected and remote (remote desktop protocol [RDP] or Independent Computing Architecture [ICA]).</summary>
    [Description("""IOPMVideoOutput's methods cannot be called when a session is changing its type. There are currently three types of sessions: console, disconnected and remote (remote desktop protocol [RDP] or Independent Computing Architecture [ICA]).""")]
    [HResultConstant(0xC026251A)]
    public static HResult ERROR_GRAPHICS_OPM_SESSION_TYPE_CHANGE_IN_PROGRESS => new(0xC026251A);

    /// <summary>The monitor connected to the specified video output does not have an I2C bus.</summary>
    [Description("""The monitor connected to the specified video output does not have an I2C bus.""")]
    [HResultConstant(0xC0262580)]
    public static HResult ERROR_GRAPHICS_I2C_NOT_SUPPORTED => new(0xC0262580);

    /// <summary>No device on the I2C bus has the specified address.</summary>
    [Description("""No device on the I2C bus has the specified address.""")]
    [HResultConstant(0xC0262581)]
    public static HResult ERROR_GRAPHICS_I2C_DEVICE_DOES_NOT_EXIST => new(0xC0262581);

    /// <summary>An error occurred while transmitting data to the device on the I2C bus.</summary>
    [Description("""An error occurred while transmitting data to the device on the I2C bus.""")]
    [HResultConstant(0xC0262582)]
    public static HResult ERROR_GRAPHICS_I2C_ERROR_TRANSMITTING_DATA => new(0xC0262582);

    /// <summary>An error occurred while receiving data from the device on the I2C bus.</summary>
    [Description("""An error occurred while receiving data from the device on the I2C bus.""")]
    [HResultConstant(0xC0262583)]
    public static HResult ERROR_GRAPHICS_I2C_ERROR_RECEIVING_DATA => new(0xC0262583);

    /// <summary>The monitor does not support the specified Virtual Control Panel (VCP) code.</summary>
    [Description("""The monitor does not support the specified Virtual Control Panel (VCP) code.""")]
    [HResultConstant(0xC0262584)]
    public static HResult ERROR_GRAPHICS_DDCCI_VCP_NOT_SUPPORTED => new(0xC0262584);

    /// <summary>The data received from the monitor is invalid.</summary>
    [Description("""The data received from the monitor is invalid.""")]
    [HResultConstant(0xC0262585)]
    public static HResult ERROR_GRAPHICS_DDCCI_INVALID_DATA => new(0xC0262585);

    /// <summary>A function call failed because a monitor returned an invalid Timing Status byte when the operating system used the Display Data Channel Command Interface (DDC/CI) Get Timing Report and Timing Message command to get a timing report from a monitor.</summary>
    [Description("""A function call failed because a monitor returned an invalid Timing Status byte when the operating system used the Display Data Channel Command Interface (DDC/CI) Get Timing Report and Timing Message command to get a timing report from a monitor.""")]
    [HResultConstant(0xC0262586)]
    public static HResult ERROR_GRAPHICS_DDCCI_MONITOR_RETURNED_INVALID_TIMING_STATUS_BYTE => new(0xC0262586);

    /// <summary>The monitor returned a DDC/CI capabilities string that did not comply with the ACCESS.bus 3.0, DDC/CI 1.1 or MCCS 2 Revision 1 specification.</summary>
    [Description("""The monitor returned a DDC/CI capabilities string that did not comply with the ACCESS.bus 3.0, DDC/CI 1.1 or MCCS 2 Revision 1 specification.""")]
    [HResultConstant(0xC0262587)]
    public static HResult ERROR_GRAPHICS_MCA_INVALID_CAPABILITIES_STRING => new(0xC0262587);

    /// <summary>An internal Monitor Configuration API error occurred.</summary>
    [Description("""An internal Monitor Configuration API error occurred.""")]
    [HResultConstant(0xC0262588)]
    public static HResult ERROR_GRAPHICS_MCA_INTERNAL_ERROR => new(0xC0262588);

    /// <summary>An operation failed because a DDC/CI message had an invalid value in its command field.</summary>
    [Description("""An operation failed because a DDC/CI message had an invalid value in its command field.""")]
    [HResultConstant(0xC0262589)]
    public static HResult ERROR_GRAPHICS_DDCCI_INVALID_MESSAGE_COMMAND => new(0xC0262589);

    /// <summary>This error occurred because a DDC/CI message length field contained an invalid value.</summary>
    [Description("""This error occurred because a DDC/CI message length field contained an invalid value.""")]
    [HResultConstant(0xC026258A)]
    public static HResult ERROR_GRAPHICS_DDCCI_INVALID_MESSAGE_LENGTH => new(0xC026258A);

    /// <summary>This error occurred because the value in a DDC/CI message checksum field did not match the message's computed checksum value. This error implies that the data was corrupted while it was being transmitted from a monitor to a computer.</summary>
    [Description("""This error occurred because the value in a DDC/CI message checksum field did not match the message's computed checksum value. This error implies that the data was corrupted while it was being transmitted from a monitor to a computer.""")]
    [HResultConstant(0xC026258B)]
    public static HResult ERROR_GRAPHICS_DDCCI_INVALID_MESSAGE_CHECKSUM => new(0xC026258B);

    /// <summary>The HMONITOR no longer exists, is not attached to the desktop, or corresponds to a mirroring device.</summary>
    [Description("""The HMONITOR no longer exists, is not attached to the desktop, or corresponds to a mirroring device.""")]
    [HResultConstant(0xC02625D6)]
    public static HResult ERROR_GRAPHICS_PMEA_INVALID_MONITOR => new(0xC02625D6);

    /// <summary>The Direct3D (D3D) device's GDI display device no longer exists, is not attached to the desktop, or is a mirroring display device.</summary>
    [Description("""The Direct3D (D3D) device's GDI display device no longer exists, is not attached to the desktop, or is a mirroring display device.""")]
    [HResultConstant(0xC02625D7)]
    public static HResult ERROR_GRAPHICS_PMEA_INVALID_D3D_DEVICE => new(0xC02625D7);

    /// <summary>A continuous VCP code's current value is greater than its maximum value. This error code indicates that a monitor returned an invalid value.</summary>
    [Description("""A continuous VCP code's current value is greater than its maximum value. This error code indicates that a monitor returned an invalid value.""")]
    [HResultConstant(0xC02625D8)]
    public static HResult ERROR_GRAPHICS_DDCCI_CURRENT_CURRENT_VALUE_GREATER_THAN_MAXIMUM_VALUE => new(0xC02625D8);

    /// <summary>The monitor's VCP Version (0xDF) VCP code returned an invalid version value.</summary>
    [Description("""The monitor's VCP Version (0xDF) VCP code returned an invalid version value.""")]
    [HResultConstant(0xC02625D9)]
    public static HResult ERROR_GRAPHICS_MCA_INVALID_VCP_VERSION => new(0xC02625D9);

    /// <summary>The monitor does not comply with the Monitor Control Command Set (MCCS) specification it claims to support.</summary>
    [Description("""The monitor does not comply with the Monitor Control Command Set (MCCS) specification it claims to support.""")]
    [HResultConstant(0xC02625DA)]
    public static HResult ERROR_GRAPHICS_MCA_MONITOR_VIOLATES_MCCS_SPECIFICATION => new(0xC02625DA);

    /// <summary>The MCCS version in a monitor's mccs_ver capability does not match the MCCS version the monitor reports when the VCP Version (0xDF) VCP code is used.</summary>
    [Description("""The MCCS version in a monitor's mccs_ver capability does not match the MCCS version the monitor reports when the VCP Version (0xDF) VCP code is used.""")]
    [HResultConstant(0xC02625DB)]
    public static HResult ERROR_GRAPHICS_MCA_MCCS_VERSION_MISMATCH => new(0xC02625DB);

    /// <summary>The Monitor Configuration API only works with monitors that support the MCCS 1.0 specification, the MCCS 2.0 specification, or the MCCS 2.0 Revision 1 specification.</summary>
    [Description("""The Monitor Configuration API only works with monitors that support the MCCS 1.0 specification, the MCCS 2.0 specification, or the MCCS 2.0 Revision 1 specification.""")]
    [HResultConstant(0xC02625DC)]
    public static HResult ERROR_GRAPHICS_MCA_UNSUPPORTED_MCCS_VERSION => new(0xC02625DC);

    /// <summary>The monitor returned an invalid monitor technology type. CRT, plasma, and LCD (TFT) are examples of monitor technology types. This error implies that the monitor violated the MCCS 2.0 or MCCS 2.0 Revision 1 specification.</summary>
    [Description("""The monitor returned an invalid monitor technology type. CRT, plasma, and LCD (TFT) are examples of monitor technology types. This error implies that the monitor violated the MCCS 2.0 or MCCS 2.0 Revision 1 specification.""")]
    [HResultConstant(0xC02625DE)]
    public static HResult ERROR_GRAPHICS_MCA_INVALID_TECHNOLOGY_TYPE_RETURNED => new(0xC02625DE);

    /// <summary>The SetMonitorColorTemperature() caller passed a color temperature to it that the current monitor did not support. CRT, plasma, and LCD (TFT) are examples of monitor technology types. This error implies that the monitor violated the MCCS 2.0 or MCCS 2.0 Revision 1 specification.</summary>
    [Description("""The SetMonitorColorTemperature() caller passed a color temperature to it that the current monitor did not support. CRT, plasma, and LCD (TFT) are examples of monitor technology types. This error implies that the monitor violated the MCCS 2.0 or MCCS 2.0 Revision 1 specification.""")]
    [HResultConstant(0xC02625DF)]
    public static HResult ERROR_GRAPHICS_MCA_UNSUPPORTED_COLOR_TEMPERATURE => new(0xC02625DF);

    /// <summary>This function can be used only if a program is running in the local console session. It cannot be used if the program is running on a remote desktop session or on a terminal server session.</summary>
    [Description("""This function can be used only if a program is running in the local console session. It cannot be used if the program is running on a remote desktop session or on a terminal server session.""")]
    [HResultConstant(0xC02625E0)]
    public static HResult ERROR_GRAPHICS_ONLY_CONSOLE_SESSION_SUPPORTED => new(0xC02625E0);
    #endregion

    #region Values from https://github.com/dotnet/runtime/blob/v8.0.8/src/libraries/Common/src/System/HResults.cs
    [HResultConstant(0x1)]
    public static HResult S_FALSE => new(0x1);

    [HResultConstant(0x8013152D)]
    public static HResult COR_E_ABANDONEDMUTEX => new(0x8013152D);

    [HResultConstant(0x8013106A)]
    public static HResult COR_E_AMBIGUOUSIMPLEMENTATION => new(0x8013106A);

    [HResultConstant(0x8000211D)]
    public static HResult COR_E_AMBIGUOUSMATCH => new(0x8000211D);

    [HResultConstant(0x80131014)]
    public static HResult COR_E_APPDOMAINUNLOADED => new(0x80131014);

    [HResultConstant(0x80131600)]
    public static HResult COR_E_APPLICATION => new(0x80131600);

    [HResultConstant(0x80070057)]
    public static HResult COR_E_ARGUMENT => new(0x80070057);

    [HResultConstant(0x80131502)]
    public static HResult COR_E_ARGUMENTOUTOFRANGE => new(0x80131502);

    [HResultConstant(0x80070216)]
    public static HResult COR_E_ARITHMETIC => new(0x80070216);

    [HResultConstant(0x80131503)]
    public static HResult COR_E_ARRAYTYPEMISMATCH => new(0x80131503);

    [HResultConstant(0x800700C1)]
    public static HResult COR_E_BADEXEFORMAT => new(0x800700C1);

    [HResultConstant(0x8007000B)]
    public static HResult COR_E_BADIMAGEFORMAT => new(0x8007000B);

    [HResultConstant(0x80131015)]
    public static HResult COR_E_CANNOTUNLOADAPPDOMAIN => new(0x80131015);

    [HResultConstant(0x80131542)]
    public static HResult COR_E_CODECONTRACTFAILED => new(0x80131542);

    [HResultConstant(0x80131504)]
    public static HResult COR_E_CONTEXTMARSHAL => new(0x80131504);

    [HResultConstant(0x80131605)]
    public static HResult COR_E_CUSTOMATTRIBUTEFORMAT => new(0x80131605);

    [HResultConstant(0x80131541)]
    public static HResult COR_E_DATAMISALIGNED => new(0x80131541);

    [HResultConstant(0x80070003)]
    public static HResult COR_E_DIRECTORYNOTFOUND => new(0x80070003);

    [HResultConstant(0x80020012)]
    public static HResult COR_E_DIVIDEBYZERO => new(0x80020012); // DISP_E_DIVBYZERO

    [HResultConstant(0x80131524)]
    public static HResult COR_E_DLLNOTFOUND => new(0x80131524);

    [HResultConstant(0x80131529)]
    public static HResult COR_E_DUPLICATEWAITOBJECT => new(0x80131529);

    [HResultConstant(0x80070026)]
    public static HResult COR_E_ENDOFSTREAM => new(0x80070026);

    [HResultConstant(0x80131523)]
    public static HResult COR_E_ENTRYPOINTNOTFOUND => new(0x80131523);

    [HResultConstant(0x80131500)]
    public static HResult COR_E_EXCEPTION => new(0x80131500);

    [HResultConstant(0x80131506)]
    public static HResult COR_E_EXECUTIONENGINE => new(0x80131506);

    [HResultConstant(0x80131623)]
    public static HResult COR_E_FAILFAST => new(0x80131623);

    [HResultConstant(0x80131507)]
    public static HResult COR_E_FIELDACCESS => new(0x80131507);

    [HResultConstant(0x80131621)]
    public static HResult COR_E_FILELOAD => new(0x80131621);

    [HResultConstant(0x80070002)]
    public static HResult COR_E_FILENOTFOUND => new(0x80070002);

    [HResultConstant(0x80131537)]
    public static HResult COR_E_FORMAT => new(0x80131537);

    [HResultConstant(0x80131508)]
    public static HResult COR_E_INDEXOUTOFRANGE => new(0x80131508);

    [HResultConstant(0x80131578)]
    public static HResult COR_E_INSUFFICIENTEXECUTIONSTACK => new(0x80131578);

    [HResultConstant(0x8013153D)]
    public static HResult COR_E_INSUFFICIENTMEMORY => new(0x8013153D);

    [HResultConstant(0x80004002)]
    public static HResult COR_E_INVALIDCAST => new(0x80004002);

    [HResultConstant(0x80131527)]
    public static HResult COR_E_INVALIDCOMOBJECT => new(0x80131527);

    [HResultConstant(0x80131601)]
    public static HResult COR_E_INVALIDFILTERCRITERIA => new(0x80131601);

    [HResultConstant(0x80131531)]
    public static HResult COR_E_INVALIDOLEVARIANTTYPE => new(0x80131531);

    [HResultConstant(0x80131509)]
    public static HResult COR_E_INVALIDOPERATION => new(0x80131509);

    [HResultConstant(0x8013153A)]
    public static HResult COR_E_INVALIDPROGRAM => new(0x8013153A);

    [HResultConstant(0x80131620)]
    public static HResult COR_E_IO => new(0x80131620);

    [HResultConstant(0x80131577)]
    public static HResult COR_E_KEYNOTFOUND => new(0x80131577);

    [HResultConstant(0x80131535)]
    public static HResult COR_E_MARSHALDIRECTIVE => new(0x80131535);

    [HResultConstant(0x8013151A)]
    public static HResult COR_E_MEMBERACCESS => new(0x8013151A);

    [HResultConstant(0x80131510)]
    public static HResult COR_E_METHODACCESS => new(0x80131510);

    [HResultConstant(0x80131511)]
    public static HResult COR_E_MISSINGFIELD => new(0x80131511);

    [HResultConstant(0x80131532)]
    public static HResult COR_E_MISSINGMANIFESTRESOURCE => new(0x80131532);

    [HResultConstant(0x80131512)]
    public static HResult COR_E_MISSINGMEMBER => new(0x80131512);

    [HResultConstant(0x80131513)]
    public static HResult COR_E_MISSINGMETHOD => new(0x80131513);

    [HResultConstant(0x80131536)]
    public static HResult COR_E_MISSINGSATELLITEASSEMBLY => new(0x80131536);

    [HResultConstant(0x80131514)]
    public static HResult COR_E_MULTICASTNOTSUPPORTED => new(0x80131514);

    [HResultConstant(0x80131528)]
    public static HResult COR_E_NOTFINITENUMBER => new(0x80131528);

    [HResultConstant(0x80131515)]
    public static HResult COR_E_NOTSUPPORTED => new(0x80131515);

    [HResultConstant(0x80131622)]
    public static HResult COR_E_OBJECTDISPOSED => new(0x80131622);

    [HResultConstant(0x8013153B)]
    public static HResult COR_E_OPERATIONCANCELED => new(0x8013153B);

    [HResultConstant(0x8007000E)]
    public static HResult COR_E_OUTOFMEMORY => new(0x8007000E);

    [HResultConstant(0x80131516)]
    public static HResult COR_E_OVERFLOW => new(0x80131516);

    [HResultConstant(0x800700CE)]
    public static HResult COR_E_PATHTOOLONG => new(0x800700CE);

    [HResultConstant(0x80131539)]
    public static HResult COR_E_PLATFORMNOTSUPPORTED => new(0x80131539);

    [HResultConstant(0x80131517)]
    public static HResult COR_E_RANK => new(0x80131517);

    [HResultConstant(0x80131602)]
    public static HResult COR_E_REFLECTIONTYPELOAD => new(0x80131602);

    [HResultConstant(0x8013153E)]
    public static HResult COR_E_RUNTIMEWRAPPED => new(0x8013153E);

    [HResultConstant(0x80131538)]
    public static HResult COR_E_SAFEARRAYRANKMISMATCH => new(0x80131538);

    [HResultConstant(0x80131533)]
    public static HResult COR_E_SAFEARRAYTYPEMISMATCH => new(0x80131533);

    [HResultConstant(0x8013150A)]
    public static HResult COR_E_SECURITY => new(0x8013150A);

    [HResultConstant(0x8013150C)]
    public static HResult COR_E_SERIALIZATION => new(0x8013150C);

    [HResultConstant(0x800703E9)]
    public static HResult COR_E_STACKOVERFLOW => new(0x800703E9);

    [HResultConstant(0x80131518)]
    public static HResult COR_E_SYNCHRONIZATIONLOCK => new(0x80131518);

    [HResultConstant(0x80131501)]
    public static HResult COR_E_SYSTEM => new(0x80131501);

    [HResultConstant(0x80131603)]
    public static HResult COR_E_TARGET => new(0x80131603);

    [HResultConstant(0x80131604)]
    public static HResult COR_E_TARGETINVOCATION => new(0x80131604);

    [HResultConstant(0x8002000E)]
    public static HResult COR_E_TARGETPARAMCOUNT => new(0x8002000E);

    [HResultConstant(0x80131530)]
    public static HResult COR_E_THREADABORTED => new(0x80131530);

    [HResultConstant(0x80131519)]
    public static HResult COR_E_THREADINTERRUPTED => new(0x80131519);

    [HResultConstant(0x80131525)]
    public static HResult COR_E_THREADSTART => new(0x80131525);

    [HResultConstant(0x80131520)]
    public static HResult COR_E_THREADSTATE => new(0x80131520);

    [HResultConstant(0x80131505)]
    public static HResult COR_E_TIMEOUT => new(0x80131505);

    [HResultConstant(0x80131543)]
    public static HResult COR_E_TYPEACCESS => new(0x80131543);

    [HResultConstant(0x80131534)]
    public static HResult COR_E_TYPEINITIALIZATION => new(0x80131534);

    [HResultConstant(0x80131522)]
    public static HResult COR_E_TYPELOAD => new(0x80131522);

    [HResultConstant(0x80131013)]
    public static HResult COR_E_TYPEUNLOADED => new(0x80131013);

    [HResultConstant(0x80070005)]
    public static HResult COR_E_UNAUTHORIZEDACCESS => new(0x80070005);

    [HResultConstant(0x8013150D)]
    public static HResult COR_E_VERIFICATION => new(0x8013150D);

    [HResultConstant(0x8013152C)]
    public static HResult COR_E_WAITHANDLECANNOTBEOPENED => new(0x8013152C);

    [HResultConstant(0x8000000B)]
    public static HResult E_BOUNDS => new(0x8000000B);

    [HResultConstant(0x8000000C)]
    public static HResult E_CHANGED_STATE => new(0x8000000C);

    [HResultConstant(0x80070002)]
    public static HResult E_FILENOTFOUND => new(0x80070002);

    [HResultConstant(0x80073B1F)]
    public static HResult ERROR_MRM_MAP_NOT_FOUND => new(0x80073B1F);

    [HResultConstant(0x800705B4)]
    public static HResult ERROR_TIMEOUT => new(0x800705B4);

    [HResultConstant(0x80000013)]
    public static HResult RO_E_CLOSED => new(0x80000013);

    [HResultConstant(0x800A004C)]
    public static HResult CTL_E_PATHNOTFOUND => new(0x800A004C);

    [HResultConstant(0x800A0035)]
    public static HResult CTL_E_FILENOTFOUND => new(0x800A0035);

    [HResultConstant(0x80131047)]
    public static HResult FUSION_E_INVALID_NAME => new(0x80131047);

    [HResultConstant(0x80131044)]
    public static HResult FUSION_E_PRIVATE_ASM_DISALLOWED => new(0x80131044);

    [HResultConstant(0x80131040)]
    public static HResult FUSION_E_REF_DEF_MISMATCH => new(0x80131040);

    [HResultConstant(0x80070004)]
    public static HResult ERROR_TOO_MANY_OPEN_FILES => new(0x80070004);

    [HResultConstant(0x80070020)]
    public static HResult ERROR_SHARING_VIOLATION => new(0x80070020);

    [HResultConstant(0x80070021)]
    public static HResult ERROR_LOCK_VIOLATION => new(0x80070021);

    [HResultConstant(0x8007006E)]
    public static HResult ERROR_OPEN_FAILED => new(0x8007006E);

    [HResultConstant(0x80070571)]
    public static HResult ERROR_DISK_CORRUPT => new(0x80070571);

    [HResultConstant(0x800703ED)]
    public static HResult ERROR_UNRECOGNIZED_VOLUME => new(0x800703ED);

    [HResultConstant(0x8007045A)]
    public static HResult ERROR_DLL_INIT_FAILED => new(0x8007045A);

    [HResultConstant(0x80131016)]
    public static HResult MSEE_E_ASSEMBLYLOADINPROGRESS => new(0x80131016);

    [HResultConstant(0x800703EE)]
    public static HResult ERROR_FILE_INVALID => new(0x800703EE);

    #endregion

#pragma warning restore CA1707 // Identifiers should not contain underscores
}
