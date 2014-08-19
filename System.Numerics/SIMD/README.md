# SIMD Sample

## Introduction

This sample contains two applications that demonstrate how to leverage SIMD
from C# with, and without, explicit vectorization.

## Prerequisites

In order to use SIMD, you need to perform the following steps:

* **Download and install the latest preview of RyuJIT**. You can get it from 
  <http://aka.ms/RyuJIT>

* **Enable SIMD for the current user**. This will make it easy for you to run
  and debug the sample apps. In order to enable SIMD, run the `enable-jit.cmd` 
  batch file.

* **Don’t forget to disable the JIT when you’re done**. Otherwise all managed
  64 bit apps will use the new JIT, which may not work for all apps. For 
  example, the JIT is currently known to not being able to handle PowerShell.
  exe correctly. You can disable the SIMD support by running the
  `disable-jit.cmd` batch file.

## Debugging

In order to ensure a great debugging experience the JIT will normally suppress 
all optimizations when a debugger is attached, even for binaries compiled in 
release mode. This also includes the support for SIMD.

If you want to debug your binary with SIMD intrinsics enabled, you need to 
disable the setting that causes the JIT to suppress optimizations:

1. Go to **Tools | Options | Debugging | General**
2. Uncheck the checkbox named **Suppress JIT optimization** on module load

## Description

The JIT support for SIMD is provided by RyuJIT. The SIMD APIs are exposed via 
the `Microsoft.Bcl.Simd` NuGet package.

The NuGet package exposes two programming models, each of which is covered by
a single sample application:

* Vectors with a fixed size (*Ray Tracer*)
* Vectors with a hardware dependent size (*Mandelbrot*)

### Using vectors with a fixed size

The NuGet package provides the following fixed size vectors:

* `System.Numerics.Vector2f`
* `System.Numerics.Vector3f`
* `System.Numerics.Vector4f`

They are modeled after the most common vector types that frequently occur in 
games and graphics programing. They usual represent points and vectors in 3D
space. The operations on those types, for example, addition and 
multiplication, are accelerated using SIMD.

The types are explicitly designed to be drop-in replacements for the data 
types that these kind of apps already use.

The Ray Tracer sample demonstrates this capability. Imagine you would have 
defined a 3-dimensional vector type to represent the rays and points of the 
scene that is rendered. The idea is that you simply add a reference to the 
NuGet package and delete your type. After fixing all call sites to refer to 
`System.Numerics.Vector3f`, your application will use SIMD instructions for 
all vector operations. Good examples of these basic vector operations include
all the scene objects such as [Disk.cs](RayTracer/Objects/Disc.cs) and 
[Plane.cs](RayTracer/Objects/Plane.cs).

Except for porting the usages from your vector type to `Vector3f` no major 
algorithmic changes are required. In particular it doesn’t require explicit 
vectorization.

### Using vectors with a hardware dependent size

While the fixed size vector types are convenient to use, their maximum degree 
of parallelization is limited by the number of components. For example, an 
application that uses `Vector2f` can get a speed-up of at most a factor of
two – even if the hardware would be capable of performing operations on eight 
elements at a time.

In order for an application to scale with the hardware capabilities, you have 
to vectorize the algorithm. Vectorizing an algorithm means that you need to 
break the input into a set of vectors whose size is hardware-dependent. On a 
machine with SSE2, this means the app could operate over vectors of four
32-bit floating point values. This allows later versions of the JIT to 
leverage newer SIMD implementations, such as AVX (please note that AVX support
isn’t included with our preview yet).

The hardware-dependent vector type is:

* `System.Numerics.Vector<T>`

The Mandelbrot sample demonstrates how this approach would look. In 
particular, compare the scalar version against the vectorized version:

* **Scalar version**. [ScalarFloat.cs](Mandelbrot/ScalarFloat.cs#L17), method 
  `RenderSingleThreadedWithADT`. 
  Note how the algorithm is defined using simple for loops and scalar floats.

* **Vectorized version**. [VectorFloat.cs](Mandelbrot/VectorFloat.cs#L29),
   method `RenderSingleThreadedWithADT`. Note how the same same algorithm is 
   now vectorized. It still uses for loops but it uses `Vector<float>` instead
   of scalar floats.

## Known Issues

Because the JIT is still in preview you need to expect some rough corners. For 
example, PowerShell is known to not be able to successfully run on the new 
JIT. If you experience any issues, make sure to disable the JIT. You can do 
this via the `disable-jit.cmd` batch file.