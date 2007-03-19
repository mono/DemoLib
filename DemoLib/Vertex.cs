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
	public class Vertex : ICloneable {
		public static readonly Vertex Origin = new Vertex (0.0f, 0.0f, 0.0f);
		public float[] v;
		
		// Constructors
		public Vertex () 
			: this (0.0f, 0.0f, 0.0f, 1.0f) {
		}
		
		public Vertex (float x)
			: this (x, 0.0f, 0.0f, 1.0f) {
		}
		
		public Vertex (float x, float y)
			: this (x, y, 0.0f, 1.0f) {
		}
		
		public Vertex (float x, float y, float z)
			: this (x, y, z, 1.0f) {
		}
		
		public Vertex (float x, float y, float z, float w) {
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
		
		// Methods
		
		public object Clone () {
			return new Vertex (x, y, z, w);
		}
		
		// Overloaded Operators
		
		public static explicit operator Vector (Vertex v) {
			return new Vector (v.x, v.y, v.z);
		}
		
		// Multiplication by a scalar value
		public static Vertex operator * (Vertex v, float scalar) {
			return new Vertex (v.x * scalar, v.y * scalar, v.z * scalar, v.w);
		}
		
		// Division by a scalar value
		public static Vertex operator / (Vertex v, float scalar) {
			if (scalar == 0.0f)
				throw new DivideByZeroException ();
			
			return new Vertex (v.x / scalar, v.y / scalar, v.z / scalar);
		}
		
		// Vertex addition (results in a Vector)
		public static Vector operator + (Vertex v0, Vertex v1) {
			return new Vector (v0.x + v1.x, v0.y + v1.y, v0.z + v1.z);
		}
		
		// Vertex subtraction (results in a Vector)
		public static Vector operator - (Vertex v0, Vertex v1) {
			return new Vector (v0.x - v1.x, v0.y - v1.y, v0.z - v1.z);
		}
		
		public static Vertex operator + (Vertex vertex, Vector vector) {
			return (Vertex) (((Vector) vertex) + vector);
		}
		
		public static Vertex operator - (Vertex vertex, Vector vector) {
			return (Vertex) (((Vector) vertex) - vector);
		}
		
		public static bool operator == (Vertex v0, Vertex v1) {
			return ((Math.Abs (v1.x - v0.x) < 0.00001f) &&
			        (Math.Abs (v1.y - v0.y) < 0.00001f) &&
			        (Math.Abs (v1.z - v0.z) < 0.00001f));
		}
		
		public static bool operator != (Vertex v0, Vertex v1) {
			return ((Math.Abs (v1.x - v0.x) >= 0.00001f) ||
			        (Math.Abs (v1.y - v0.y) >= 0.00001f) ||
			        (Math.Abs (v1.z - v0.z) >= 0.00001f));
		}
		
		public override string ToString () {
			StringBuilder str = new StringBuilder ("Vertex: (");
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
