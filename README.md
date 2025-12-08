# SmartAccountant.Client

# Development Guidelines

* There is no need to handle Operation/TaskCancelledExceptions, as they are handled by the global exception handler.

## Emulator notes

Make sure to enable Windows Hypervisor Platform

adb reverse tcp:7130 tcp:7130

//TODO: converters to lib
