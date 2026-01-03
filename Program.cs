using System;
using System.Globalization;

namespace Task08.RectangleParallelepiped
{
    public readonly struct Point2D
    {
        public double X { get; }
        public double Y { get; }

        public Point2D(double x, double y) { X = x; Y = y; }
    }

    public readonly struct Point3D
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public Point3D(double x, double y, double z) { X = x; Y = y; Z = z; }
    }

    // Rectangle: b1 <= x1 <= a1, b2 <= x2 <= a2
    public class Rectangle
    {
        private double _b1, _a1, _b2, _a2;

        // Change from protected to public
        public double B1 => _b1;
        public double A1 => _a1;
        public double B2 => _b2;
        public double A2 => _a2;

        public Rectangle() { }

        public Rectangle(double b1, double a1, double b2, double a2)
        {
            SetCoefficients(b1, a1, b2, a2);
        }

        // validate numeric values
        protected static void ValidateNumber(double v, string name)
        {
            if (double.IsNaN(v) || double.IsInfinity(v))
                throw new ArgumentException($"{name} must be a finite number.", name);
        }

        // set coefficients (ensures b <= a by swapping if necessary)
        public virtual void SetCoefficients(double b1, double a1, double b2, double a2)
        {
            ValidateNumber(b1, nameof(b1));
            ValidateNumber(a1, nameof(a1));
            ValidateNumber(b2, nameof(b2));
            ValidateNumber(a2, nameof(a2));

            if (b1 <= a1) { _b1 = b1; _a1 = a1; }
            else { _b1 = a1; _a1 = b1; }

            if (b2 <= a2) { _b2 = b2; _a2 = a2; }
            else { _b2 = a2; _a2 = b2; }
        }

        public virtual void PrintCoefficients()
        {
            Console.WriteLine("Rectangle bounds:");
            Console.WriteLine($"  b1 <= x1 <= a1 : {B1} <= x1 <= {A1}");
            Console.WriteLine($"  b2 <= x2 <= a2 : {B2} <= x2 <= {A2}");
        }

        // virtual polymorphic containment check using coords array
        // Rectangle expects at least two coordinates [x1, x2]
        public virtual bool Contains(params double[] coords)
        {
            if (coords == null || coords.Length < 2) return false;
            var x1 = coords[0];
            var x2 = coords[1];
            return x1 >= B1 && x1 <= A1 && x2 >= B2 && x2 <= A2;
        }

        // convenience overloads
        public virtual bool Contains(Point2D p) => Contains(p.X, p.Y);
    }

    // Parallelepiped: extends Rectangle with b3 <= x3 <= a3.
    public class Parallelepiped : Rectangle
    {
        private double _b3, _a3;
        // expose third-dimension bounds publicly so external code/tests can read them
        public double B3 => _b3;
        public double A3 => _a3;

        public Parallelepiped() : base() { }

        public Parallelepiped(double b1, double a1, double b2, double a2, double b3, double a3)
            : base(b1, a1, b2, a2)
        {
            SetCoefficients(b1, a1, b2, a2, b3, a3);
        }

        // overloaded: set 3D coefficients
        public void SetCoefficients(double b1, double a1, double b2, double a2, double b3, double a3)
        {
            // reuse base validation and normalization for first two dimensions
            base.SetCoefficients(b1, a1, b2, a2);

            ValidateNumber(b3, nameof(b3));
            ValidateNumber(a3, nameof(a3));

            if (b3 <= a3) { _b3 = b3; _a3 = a3; }
            else { _b3 = a3; _a3 = b3; }
        }

        public override void PrintCoefficients()
        {
            Console.WriteLine("Parallelepiped bounds:");
            Console.WriteLine($"  b1 <= x1 <= a1 : {B1} <= x1 <= {A1}");
            Console.WriteLine($"  b2 <= x2 <= a2 : {B2} <= x2 <= {A2}");
            Console.WriteLine($"  b3 <= x3 <= a3 : {B3} <= x3 <= {A3}");
        }

        // override polymorphic containment: if only 2 coordinates provided, check base projection
        public override bool Contains(params double[] coords)
        {
            if (coords == null) return false;
            if (coords.Length < 2) return false;
            if (coords.Length == 2) return base.Contains(coords); // projection check
            // expect at least 3 coords for full 3D check
            var x1 = coords[0];
            var x2 = coords[1];
            var x3 = coords[2];
            return base.Contains(x1, x2) && x3 >= B3 && x3 <= A3;
        }

        // convenience overloads
        public bool Contains(Point3D p) => Contains(p.X, p.Y, p.Z);
        public override bool Contains(Point2D p) => Contains(p.X, p.Y);
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Rectangle and Parallelepiped demo with virtual methods\n");

            Console.WriteLine("Choose mode: enter '1' to work with a Rectangle, anything else for Parallelepiped:");
            var choiceLine = Console.ReadLine()?.Trim() ?? "";
            bool chooseRectangle = choiceLine == "1";

            // Declare base-class reference (acts as "pointer" to an instance of unknown compile-time type)
            Rectangle baseRef;

            if (chooseRectangle)
            {
                Console.WriteLine("Enter rectangle bounds (b1 a1 b2 a2) separated by spaces:");
                var rectVals = ReadDoubles(4);
                // dynamic creation: object created depending on runtime choice
                baseRef = new Rectangle(rectVals[0], rectVals[1], rectVals[2], rectVals[3]);
            }
            else
            {
                Console.WriteLine("Enter parallelepiped bounds (b1 a1 b2 a2 b3 a3) separated by spaces:");
                var parVals = ReadDoubles(6);
                // dynamic creation of derived type, but stored in base-class reference
                baseRef = new Parallelepiped(parVals[0], parVals[1], parVals[2], parVals[3], parVals[4], parVals[5]);
            }

            // Call virtual method through base-class reference (demonstrates polymorphism)
            Console.WriteLine();
            Console.WriteLine("Calling PrintCoefficients() via base-class reference:");
            baseRef.PrintCoefficients();

            Console.WriteLine();

            // Call Contains via base-class reference with appropriate number of coords
            if (baseRef is Parallelepiped) // runtime type check
            {
                Console.WriteLine("Enter a 3D point (x1 x2 x3) to check for the parallelepiped:");
                var p3 = ReadDoubles(3);
                Console.WriteLine(baseRef.Contains(p3[0], p3[1], p3[2])
                    ? "Point belongs to the parallelepiped."
                    : "Point does NOT belong to the parallelepiped");
            }
            else
            {
                Console.WriteLine("Enter a 2D point (x1 x2) to check for the rectangle:");
                var p2 = ReadDoubles(2);
                Console.WriteLine(baseRef.Contains(p2[0], p2[1])
                    ? "Point belongs to the rectangle."
                    : "Point does NOT belong to the rectangle");
            }

            // Additional demonstration: treat baseRef as a "pointer" again and call Contains(Point2D)
            Console.WriteLine();
            Console.WriteLine("Demonstrating call to Contains(Point2D) through the same base-class reference:");
            var sample = new Point2D((baseRef is Parallelepiped) ? ((Parallelepiped)baseRef).B1 : 0, 0);
            Console.WriteLine($"Sample point ({sample.X}, {sample.Y}) belongs: {baseRef.Contains(sample)}");
        }

        // helper to read exactly n doubles from one line or multiple lines; uses InvariantCulture
        private static double[] ReadDoubles(int count)
        {
            var list = new double[count];
            int read = 0;
            while (read < count)
            {
                string line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    Console.WriteLine("Input empty. Please enter numbers:");
                    continue;
                }

                var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var p in parts)
                {
                    if (read >= count) break;
                    if (double.TryParse(p, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double v))
                    {
                        list[read++] = v;
                    }
                    else if (double.TryParse(p, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out v))
                    {
                        list[read++] = v;
                    }
                    else
                    {
                        Console.WriteLine($"Could not parse '{p}'. Enter a valid number.");
                    }
                }

                if (read < count)
                {
                    Console.WriteLine($"Need {count - read} more number(s)...");
                }
            }
            return list;
        }
    }
}
