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

namespace DemoLib {
	public class Camera {
		Vertex position;
		Vector viewdir;
		Vector right;
		Vector up;
		
		// Default Constructor
		public Camera () {
			position = new Vertex (0.0f, 0.0f, 0.0f);
			viewdir = new Vector (0.0f, 0.0f, -1.0f);
			right = new Vector (1.0f, 0.0f, 0.0f);
			up = new Vector (0.0f, 1.0f, 0.0f);
		}
		
		// Positional Constructors
		public Camera (Vertex position) {
			this.position = position;
			
			viewdir = new Vector (0.0f, 0.0f, -1.0f);
			right = new Vector (1.0f, 0.0f, 0.0f);
			up = new Vector (0.0f, 1.0f, 0.0f);
		}
		
		public Camera (float x, float y, float z) {
			position = new Vertex (x, y, z);
			
			viewdir = new Vector (0.0f, 0.0f, -1.0f);
			right = new Vector (1.0f, 0.0f, 0.0f);
			up = new Vector (0.0f, 1.0f, 0.0f);
		}
		
		// Positional + LookAt Constructors
		public Camera (Vertex position, Vertex target) {
			this.position = position;
			
			LookAt (target);
		}
		
		public Camera (Vertex position, Vertex target, Vector upVector) {
			this.position = position;
			
			LookAt (target, upVector);
		}
		
		// Properties
		public Vertex Position {
			get { return position; }
			set { position = value; }
		}
		
		public Vector ViewDirection {
			get { return viewdir; }
		}
		
		// FIXME: better name for this?
		public Vector RightVector {
			get { return right; }
		}
		
		// FIXME: better name for this?
		public Vector UpVector {
			get { return up; }
		}
		
		// Methods
		
		// Direct the camera toward a specific point in space
		public void LookAt (float x, float y, float z) {
			Vertex point = new Vertex (x, y, z);
			LookAt (point);
		}
		
		public void LookAt (Vertex point) {
			Vector delta = point - position;
			
			viewdir = delta.Normal;
			
			if (Math.Abs (delta.x) < 0.00001f && Math.Abs (delta.z) < 0.00001f) {
				delta.x = 0.0f;
				delta.Normalize ();
				
				right = new Vector (1.0f, 0.0f, 0.0f);
				up = delta.Cross (right);
				
				right = viewdir.Cross (up) * -1.0f;
			} else {
				delta.y = 0.0f;
				delta.Normalize ();
				
				up = new Vector (0.0f, 1.0f, 0.0f);
				right = delta.Cross (up) * -1.0f;
				
				up = viewdir.Cross (right);
			}
			
			right.Normalize ();
			up.Normalize ();
		}
		
		// Direct the camera toward a specific point in space using the specified upward orientation
		public void LookAt (Vertex point, Vector upVector) {
			viewdir = point - position;
			viewdir.Normalize ();
			
			up = upVector.Normal;
			
			right = viewdir.Cross (up);
			right.Normalize ();
		}
		
		// Move the camera relative to world axis
		public void Move (Vector amount) {
			position += amount;
		}
		
		// Move the camera relative to its own axis/orientation
		public void MoveRelative (Vector amount) {
			Elevate (amount.y);
			Strafe (amount.x);
			Zoom (amount.z);
		}
		
		// Move the camera to a specific position
		public void MoveTo (Vertex point) {
			position = point;
		}
		
		// Move the camera to a specific position
		public void MoveTo (float x, float y, float z) {
			position = new Vertex (x, y, z);
		}
		
		// Rotate camera up or down (aka Tilt)
		public void Pitch (float angle) {
			// rotate around the right vector (normally the x-axis)
			viewdir = (viewdir * (float) Math.Cos (angle)) + (up * (float) Math.Sin (angle));
			viewdir.Normalize ();
			
			up = viewdir.Cross (right);
			up.Normalize ();
		}
		
		// Rotate camera left or right (aka Pan)
		public void Yaw (float angle) {
			// rotate around the up vector (normally the y-axis)
			viewdir = (viewdir * (float) Math.Cos (angle)) - (right * (float) Math.Sin (angle));
			viewdir.Normalize ();
			
			right = viewdir.Cross (up);
			right.Normalize ();
		}
		
		// Roll the camera left or right
		public void Roll (float angle) {
			// rotate around the view-direction vector (normally the z-axis)
			right = (right * (float) Math.Cos (angle)) + (up * (float) Math.Sin (angle));
			right.Normalize ();
			
			up = viewdir.Cross (right) * -1.0f;
			up.Normalize ();
		}
		
		// Change the elevation of the camera
		public void Elevate (float amount) {
			position += (up * amount);
		}
		
		// Strafe the camera left or right
		public void Strafe (float amount) {
			position += (right * amount);
		}
		
		// Zoom camera in or out
		public void Zoom (float amount) {
			position += (viewdir * amount);
		}
	}
}
