# CodesysV3Driver

A .NET Framework communication driver for CODESYS V3-based PLCs over TCP/UDP, shipped as a prebuilt library with a console demo and a WinForms demo.

- Target framework: .NET Framework 4.0 (buildable with VS2017+)
- Namespace: `CodesysV3Driver`
- Default ports: TCP 11740 / UDP 1740

## Package Layout

```
CodesysV3Driver.sln                       Solution file
├─ Lib\                                   Prebuilt assemblies
│  ├─ CodesysV3Driver.dll                 Driver library (namespace: CodesysV3Driver)
│  └─ Net4x.Extensions.Logging.Abstractions.dll   ILogger abstraction dependency
├─ TestCodesysV3Driver.Console\           Console demo
└─ TestCodesysV3Driver.GUI\               WinForms demo
```

## Key Features

- **Transport choice**: TCP (reliable delivery) or UDP (lower latency, no delivery guarantee). The caller selects explicitly; UDP failure does not fall back to TCP.
- **Device-adaptive login**: the login form is auto-detected per device (newer firmware two-step, older firmware single-step). Credentials are passed as `Connect()` parameters.
- **Symbol tree**: online browsing from the PLC, or offline loading of a CODESYS symbol-export XML; lazy expansion keeps initial load fast.
- **Batched read/write**: variables addressed by full symbolic path; typed values via `PValue`.
- **Auto-reconnect**: built-in, disabled by default (`Reconnect.Enabled = true`); raises `ConnectionLost` / `Reconnecting` / `Reconnected`. Works with both transports.
- **Per-instance error ledger**: read/write return `false` with `LastErrorCode` / `LastErrorText` instead of throwing.
- **Logging via ILogger injection**: assign the public `Logger` property; when unset, logging is silently discarded (NullLogger).
- **IDisposable**: `Dispose()` aliases `Disconnect()` — releases the link, keeps the symbol tree, and the instance can be reconnected.

## Quick Start

```csharp
using CodesysV3Driver;

var driver = new CodeSysController();
// driver.Logger = new MyLogger();        // optional: inject Microsoft.Extensions.Logging.ILogger

// TCP (default port 11740). For UDP: Connect(host, 1740, TransportLayer.Udp, user, password)
bool ok = driver.Connect("192.168.0.1", 11740, TransportLayer.Tcp, "user", "password");

// Offline symbol-export XML, or online from the PLC
ok = driver.LoadSymbolsFromFile("symbols.xml");   // or: driver.LoadSymbolsFromPLC()

// Browse the tree: null = top level; pass a node's SymbolPath to expand it
List<SymbolNode> groups = driver.Browse(null);
List<SymbolNode> leaves = driver.LoadAllSymbolicInfo();   // all leaf symbols

// Batch read
var names = new List<string> { "Application.GVL_Example.Start", "Application.GVL_Example.Speed" };
List<PValue> values;
ok = driver.Read(names, out values);       // values[i].ToString() shows the value

// Write
ok = driver.Write("Application.GVL_Example.Start", PValue.Create(VarType.BOOL, true));
ok = driver.Write("Application.GVL_Example.Speed", PValue.Create(VarType.REAL, 12.5));

driver.Disconnect();                       // idempotent; Dispose() is equivalent
```

## Building

Open `CodesysV3Driver.sln` in Visual Studio 2017+ and build; or from the command line:

```
MSBuild CodesysV3Driver.sln /p:Configuration=Debug
```

Both demo projects reference the prebuilt assemblies in `Lib\`.

## Test Demos

**Console** (`TestCodesysV3Driver.Console.exe`):

```
TestCodesysV3Driver.Console.exe <host> [port=11740] [user] [password] [symbolFile]
```

Connects to the PLC, loads the symbol table (offline CODESYS symbol-export XML when `symbolFile` is given, otherwise online from the PLC), prints the top-level symbol groups, sample-reads up to 8 readable leaf variables, then disconnects. Read-only demo — write examples are shown as comments at the end of `Program.cs`. Transport defaults to TCP; for UDP see the `Connect` comment in `Program.cs`.

```
# Connect via TCP (default port 11740)
TestCodesysV3Driver.Console.exe 192.168.0.1

# With credentials and an offline symbol file
TestCodesysV3Driver.Console.exe 192.168.0.1 11740 admin password symbols.xml
```

**GUI** (`TestCodesysV3Driver.GUI.exe`): enter host/port/credentials in the connection area -> load a symbol-export XML or fetch the online tree -> pick variables in the tree (lazy expansion) -> single-point read/write. Driver logs are written to the log area through the injected ILogger.

## Development Status

⚠️ **Important Notice**
This project is currently in **active development and testing phase**.
**Do not deploy in production environments.**

## Source Access

_Research authorization available upon request._