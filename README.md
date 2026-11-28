## Prerequisites
- **.NET Framework 4.0** must be installed to run this application

## Technical Stack
- Built on **.NET 4** framework

## Communication Protocol
- Implements **TCP and UDP** transport layers
- Functionally equivalent to CODESYS PLCHandler
- Default ports: **TCP 11740** / **UDP 1740**
- Transport layer selection:
  - **UDP**: 4-6x faster per operation (measured on AC802 real hardware), but no delivery guarantee
  - **TCP**: Reliable delivery with automatic retransmission, slower due to per-PDU round-trip overhead
  - UDP failure does NOT fall back to TCP (caller must explicitly choose)

## Authentication
- Supports both **encrypted two-step login** (RSA-OAEP with SHA-256) and **legacy single-step login**
- Login mode is automatically detected per device:
  - Newer firmware (e.g. 3.5.16+) requires encrypted two-step login
  - Older firmware (e.g. 3.5.11) accepts legacy single-step login
- Credentials are passed as `Connect()` parameters (username, password), not stored as public properties
- Challenge value (0x23 tag) is randomized per connection, aligned with official PLCHandler behavior

## Automatic Reconnection
- Built-in auto-reconnect with exponential backoff (default: disabled, set `Reconnect.Enabled = true`)
- Events: `ConnectionLost` → `Reconnecting` (with attempt count) → `Reconnected`
- Reconnection restores the session without requiring symbol table reload
- Works with both TCP and UDP transport layers
- UDP transport: detects connection loss via watchdog timer (silence timeout)
- TCP transport: detects connection loss via socket read failure

## Usage

### Console Demo
```
TestCodesysV3Driver.Console.exe <host> [port=11740] [user] [password] [symbolFile]
```
Connects to the PLC, loads the symbol table (offline CODESYS symbol-export XML when
`symbolFile` is given, otherwise online from the PLC), prints the top-level symbol
groups, sample-reads up to 8 readable leaf variables, then disconnects.
Read-only demo — write examples are shown as comments at the end of `Program.cs`.
Transport defaults to TCP; for UDP see the `Connect` comment in `Program.cs`.
Login form (encrypted two-step / legacy single-step) is auto-detected per device.

### Examples
```
# Connect via TCP (default port 11740)
TestCodesysV3Driver.Console.exe 192.168.0.1

# With credentials and an offline symbol file
TestCodesysV3Driver.Console.exe 192.168.0.1 11740 admin password symbols.xml
```

## Development Status
⚠️ **Important Notice**  
This project is currently in **active development and testing phase**.  
**Do not deploy in production environments.**

## Source Access
_Research authorization available upon request_  
(Includes full source code and protocol demonstration videos)
