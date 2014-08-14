@echo off
setlocal

:: This will enable RyuJIT

reg add HKCU\SOFTWARE\Microsoft\.NETFramework /v AltJit /t REG_SZ /d "*" /f /reg:64 > NUL

:: This will enable SIMD

reg add HKCU\SOFTWARE\Microsoft\.NETFramework /v FeatureSIMD /t REG_DWORD /d 1 /f /reg:64 > NUL