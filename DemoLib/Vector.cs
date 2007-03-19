/*
 *  Copyright (C) 2007 Jeffrey Stedfast
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Text;

namespace DemoLib {
	public class Vector : ICloneable {
#region Special Vectors
		public static readonly Vector Backward = new Vector (0.0f, 0.0f, -1.0f);
		public static readonly Vector Forward = new Vector (0.0f, 0.0f, 1.0f);
		public static readonly Vector Right = new Vector (1.0f, 0.0f, 0.0f);
		public static readonly Vector Left = new Vector (-1.0f, 0.0f, 0.0f);
		public static readonly Vector Down = new Vector (0.0f, -1.0f, 0.0f);
		public static readonly Vector Zero = new Vector (0.0f, 0.0f, 0.0f);
		public static readonly Vector Up = new Vector (0.0f, 1.0f, 0.0f);
#endregion
		
		public float[] v;
		
		// Constructors
		public Vector () 
			: this (0.0f, 0.0f, 0.0f, 1.0f) {
		}
		
		public Vector (float x)
			: this (x, 0.0f, 0.0f, 1.0f) {
		}
		
		public Vector (float x, float y)
			: this (x, y, 0.0f, 1.0f) {
		}
		
		public Vector (float x, float y, float z)
			: this (x, y, z, 1.0f) {
		}
		
		public Vector (float x, float y, float z, float w) {
			v = new float[4];
			v[0] = x;
			v[1] = y;
			v[2] = z;
			v[3] = w;
		}
		
		// Properties
		public float x {
			get { return v[0]; }
			set { v[0] = value; }
		}
		
		public float y {
			get { return v[1]; }
			set { v[1] = value; }
		}
		
		public float z {
			get { return v[2]; }
			set { v[2] = value; }
		}
		
		public float w {
			get { return v[3]; }
			set { v[3] = value; }
		}
		
		float LengthSquared {
			get { return (x * x) + (y * y) + (z * z); }
		}
		
		// Length of this Vector
		public float Length {
			get { return (float) Math.Sqrt (LengthSquared); }
		}
		
		// A Normal Vector representative of this Vector
		public Vector Normal {
			get {
				float len = Length;
				
				if (len == 0.0f || len == 1.0f)
					return this;
				
				return new Vector (x / len, y / len, z / len, w);
			}
		}
		
		// Angle between this Vector and the X-Axis
		public float AngleX {
			get {
				float n = LengthSquared;
				
				return (float) Math.Acos (x / Math.Sqrt ((double) (n * n)));
			}
		}
		
		// Angle between this Vector and the Y-Axis
		public float AngleY {
			get {
				float n = LengthSquared;
				
				return (float) Math.Acos (y / Math.Sqrt ((double) (n * n)));
			}
		}
		
		// Angle between this Vector and the Z-Axis
		public float AngleZ {
			get {
				float n = LengthSquared;
				
				return (float) Math.Acos (z / Math.Sqrt ((double) (n * n)));
			}
		}
		
		// Methods
		
		public object Clone () {
			return new Vector (x, y, z, w);
		}
		
		// Normalize this Vector
		public void Normalize () {
			float len = Length;
			
			if (len == 0.0f || len == 1.0f)
				return;
			
			x /= len;
			y /= len;
			z /= len;
		}
		
		// Angle between this Vector and another Vector
		public float Angle (Vector vector) {
			float dot = this.Dot (vector);
			float n1 = vector.LengthSquared;
			float n0 = LengthSquared;
			
			return (float) Math.Acos (dot / Math.Sqrt ((double) (n0 * n1)));
		}
		
		// Angle between 2 vectors
		public static float Angle (Vector v0, Vector v1) {
			float n0 = v0.LengthSquared;
			float n1 = v1.LengthSquared;
			float dot = v0.Dot (v1);
			
			return (float) Math.Acos (dot / Math.Sqrt ((double) (n0 * n1)));
		}
		
		/* Matrix math:
		 * 1         0         0         0
		 * 0         cos (a)   -sin (a)  0
		 * 0         sin (a)   cos (a)   0
		 * 0         0         0         1
		 **/
		public void RotateX (float angle) {
			y = (y * (float) Math.Cos (angle)) - (z * (float) Math.Sin (angle));
			z = (y * (float) Math.Sin (angle)) + (z * (float) Math.Cos (angle));
		}
		
		/* Matrix math:
		 * cos (a)   0         sin (a)   0
		 * 0         1         0         0
		 * -sin (a)  0         cos (a)   0
		 * 0         0         0         1
		 **/
		public void RotateY (float angle) {
			x = (x * (float) Math.Cos (angle)) + (z * (float) Math.Sin (angle));
			z = -(x * (float) Math.Sin (angle)) + (z * (float) Math.Cos (angle));
		}
		
		/* Matrix math:
		 * cos (a)   -sin (a)  0         0
		 * sin (a)   cos (a)   0         0
		 * 0         0         1         0
		 * 0         0         0         1
		 **/
		public void RotateZ (float angle) {
			x = (x * (float) Math.Cos (angle)) - (y * (float) Math.Sin (angle));
			y = (x * (float) Math.Sin (angle)) + (y * (float) Math.Cos (angle));
		}
		
		// Dot product of 2 Vectors
		public float Dot (Vector vector) {
			return (x * vector.x) + (y * vector.y) + (z * vector.z);
		}
		
		// Cross product of 2 Vectors
		public Vector Cross (Vector vector) {
			return new Vector ((y * vector.z) - (z * vector.y),
			                   (z * vector.x) - (x * vector.z),
			                   (x * vector.y) - (y * vector.x));
		}
		
		// Dot product of 2 Vectors
		public static float Dot (Vector v0, Vector v1) {
			return (v0.x * v1.x) + (v0.y * v1.y) + (v0.z * v1.z);
		}
		
		// Cross product of 2 Vectors
		public static Vector Cross (Vector v0, Vector v1) {
			return new Vector ((v0.y * v1.z) - (v0.z * v1.y),
			                   (v0.z * v1.x) - (v0.x * v1.z),
			                   (v0.x * v1.y) - (v0.y * v1.x));
		}
		
		// Overloaded Operators
		
		public static explicit operator Vertex (Vector v) {
			return new Vertex (v.x, v.y, v.z);
		}
		
		// Multiplication by a scalar value
		public static Vector operator * (Vector v, float scalar) {
			return new Vector (v.x * scalar, v.y * scalar, v.z * scalar);
		}
		
		// Division by a scalar value
		public static Vector operator / (Vector v, float scalar) {
			if (scalar == 0.0f)
				throw new DivideByZeroException ();
			
			return new Vector (v.x / scalar, v.y / scalar, v.z / scalar);
		}
		
		// Vector addition
		public static Vector operator + (Vector v0, Vector v1) {
			return new Vector (v0.x + v1.x, v0.y + v1.y, v0.z + v1.z);
		}
		
		// Vector subtraction
		public static Vector operator - (Vector v0, Vector v1) {
			return new Vector (v0.x - v1.x, v0.y - v1.y, v0.z - v1.z);
		}
		
		public static bool operator == (Vector v0, Vector v1) {
			return ((Math.Abs (v1.x - v0.x) < 0.00001f) &&
			        (Math.Abs (v1.y - v0.y) < 0.00001f) &&
			        (Math.Abs (v1.z - v0.z) < 0.00001f));
		}
		
		public static bool operator != (Vector v0, Vector v1) {
			return ((Math.Abs (v1.x - v0.x) >= 0.00001f) ||
			        (Math.Abs (v1.y - v0.y) >= 0.00001f) ||
			        (Math.Abs (v1.z - v0.z) >= 0.00001f));
		}
		
		// tolerance comparisons
		public static bool operator < (Vector v, float tolerance) {
			tolerance = Math.Abs (tolerance);
			return ((Math.Abs (v.x) < tolerance) &&
			        (Math.Abs (v.y) < tolerance) &&
			        (Math.Abs (v.z) < tolerance));
		}
		
		public static bool operator <= (Vector v, float tolerance) {
			tolerance = Math.Abs (tolerance);
			return ((Math.Abs (v.x) <= tolerance) &&
			        (Math.Abs (v.y) <= tolerance) &&
			        (Math.Abs (v.z) <= tolerance));
		}
		
		public static bool operator > (Vector v, float tolerance) {
			tolerance = Math.Abs (tolerance);
			return ((Math.Abs (v.x) > tolerance) &&
			        (Math.Abs (v.y) > tolerance) &&
			        (Math.Abs (v.z) > tolerance));
		}
		
		public static bool operator >= (Vector v, float tolerance) {
			tolerance = Math.Abs (tolerance);
			return ((Math.Abs (v.x) >= tolerance) &&
			        (Math.Abs (v.y) >= tolerance) &&
			        (Math.Abs (v.z) >= tolerance));
		}
		
		public override string ToString () {
			StringBuilder str = new StringBuilder ("Vector: (");
			str.Append (x);
			str.Append (", ");
			str.Append (y);
			str.Append (", ");
			str.Append (z);
			str.Append (')');
			
			return str.ToString ();
		}
		
		public override bool Equals (object obj) {
			return base.Equals (obj);
		}
		
		public override int GetHashCode () {
			return base.GetHashCode ();
		}
	}
}
