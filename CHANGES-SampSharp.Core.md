### 0.8.0-alpha5
- Added a separate buffer for native results so the native arguments don't get overwritten by the native when it is writing the output (plugin)
- Changed natives to provide all results in array references instead of just the written values. The unwritten values are 0 by default
- Fixed deadlocks when calling natives which call callbacks which call natives
- Fixed a crash caused by a recursive non-recursive-mutex locking issue (plugin) 

### 0.8.0-alpha4
- Fixed array arguments in natives not working

### 0.8.0-alpha3
- Fixed a threading issue (#220)
- Fixed a communication buffer overflow when pausing the server using a debugger. Set `com_debug 1` in server.cfg to avoid these overflows.

### 0.8.0-alpha2
- Fixed FakeGMX not reconnecting players (#221)

### 0.8.0-alpha1
- Initial version
