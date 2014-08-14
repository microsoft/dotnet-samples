@echo off
setlocal

:: This will remove the markers that enabled RyuJIT & SIMD 
reg delete HKCU\SOFTWARE\Microsoft\.NETFramework /v AltJit /f /reg:64 > NUL
reg delete HKCU\SOFTWARE\Microsoft\.NETFramework /v FeatureSIMD /f /reg:64 > NUL