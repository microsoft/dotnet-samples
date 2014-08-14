using System;
using System.Numerics;

namespace Algorithms
{
    // I need a float implementation of the BCL Complex type, but this only contains the bare
    // essentials, plus a couple operations needed for efficient Mandelbrot calcuation.
    internal struct ComplexFloat
    {
        public ComplexFloat(float real, float imaginary)
        {
            Real = real; Imaginary = imaginary;
        }

        public float Real;
        public float Imaginary;

        public ComplexFloat square()
        {
            return new ComplexFloat(Real * Real - Imaginary * Imaginary, 2.0f * Real * Imaginary);
        }

        public float sqabs()
        {
            return Real * Real + Imaginary * Imaginary;
        }

        public override string ToString()
        {
            return String.Format("[{0} + {1}Imaginary]", Real, Imaginary);
        }

        public static ComplexFloat operator +(ComplexFloat a, ComplexFloat b)
        {
            return new ComplexFloat(a.Real + b.Real, a.Imaginary + b.Imaginary);
        }
    }

    // A couple extension methods that operate on BCL Complex types to help efficiently calculate
    // the Mandelbrot set (They're instance methods on the ComplexFloat custom type)
    public static partial class extensions
    {
        public static double sqabs(this Complex val)
        {
            return val.Real * val.Real + val.Imaginary * val.Imaginary;
        }

        public static Complex square(this Complex val)
        {
            return new Complex(val.Real * val.Real - val.Imaginary * val.Imaginary, 2.0 * val.Real * val.Imaginary);
        }
    }

    // This is an implementation of ComplexFloat that operates on Vector<float> at a time SIMD types
    internal struct ComplexVecFloat
    {
        // Goofy magic to get SIMD working with the CTP This will not be required once RyuJIT is
        // official. You must use the type in the class constructor. It will _not_ be accelerated
        // until after the class constructor, has been invoked though.
#pragma warning disable 414
        private static Vector<float> dummy = new Vector<float>();
#pragma warning restore

        public ComplexVecFloat(Vector<float> real, Vector<float> imaginary)
        {
            Real = real; Imaginary = imaginary;
        }

        public Vector<float> Real;
        public Vector<float> Imaginary;

        public ComplexVecFloat square()
        {
            return new ComplexVecFloat(Real * Real - Imaginary * Imaginary, Real * Imaginary + Real * Imaginary);
        }

        public Vector<float> sqabs()
        {
            return Real * Real + Imaginary * Imaginary;
        }

        public override string ToString()
        {
            return String.Format("[{0} + {1}Imaginary]", Real, Imaginary);
        }

        public static ComplexVecFloat operator +(ComplexVecFloat a, ComplexVecFloat b)
        {
            return new ComplexVecFloat(a.Real + b.Real, a.Imaginary + b.Imaginary);
        }
    }

    // This is an implementation of Complex that operates on Vector<double> at a time SIMD types
    internal struct ComplexVecDouble
    {
        // Goofy magic to get SIMD working with the CTP This will not be required once RyuJIT is
        // official. You must use the type in the class constructor. It will _not_ be accelerated
        // until after the class constructor, has been invoked though.
#pragma warning disable 414
        private static Vector<float> dummy = new Vector<float>();
#pragma warning restore

        public ComplexVecDouble(Vector<double> real, Vector<double> imaginary)
        {
            Real = real; Imaginary = imaginary;
        }

        public Vector<double> Real;
        public Vector<double> Imaginary;

        public ComplexVecDouble square()
        {
            return new ComplexVecDouble(Real * Real - Imaginary * Imaginary, Real * Imaginary + Real * Imaginary);
        }

        public Vector<double> sqabs()
        {
            return Real * Real + Imaginary * Imaginary;
        }

        public override string ToString()
        {
            return String.Format("[{0} + {1}Imaginary]", Real, Imaginary);
        }

        public static ComplexVecDouble operator +(ComplexVecDouble a, ComplexVecDouble b)
        {
            return new ComplexVecDouble(a.Real + b.Real, a.Imaginary + b.Imaginary);
        }
    }
}