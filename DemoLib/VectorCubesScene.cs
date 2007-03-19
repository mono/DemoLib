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
using DemoLib;
using Tao.OpenGl;
using Tao.FreeGlut;

namespace DemoLib {
	public partial class VectorCubesScene : Scene {
		enum State {
			FlatPlaneFormation  = 0,
			SnowflakeFormation  = 2,
			CrossPlaneFormation = 4,
			TunnelFormation     = 6,
			HollowFormation     = 8,
			DiceFormation       = 10,
			NestedFormation     = 12,
			Complete
		}
		
		struct vector_t {
			public float x;
			public float y;
			public float z;
		}
		
		struct camera_t {
			public vector_t pos;
			public vector_t rot;
		}
		
		static Vector[][] formations = null;
		
		camera_t camera;
		Vector[] cubes;
		State state;
		
		int cubeId;
		
		int nFrames;
		int start;
		
		bool lighting;
		
		public VectorCubesScene () : this (false) {
		}
		
		public VectorCubesScene (bool enableLighting) : base () {
			int i;
			
			lighting = enableLighting;
			
			if (formations == null) {
				//float[][,] floats = new float[7][,];
				//floats[0] = flat_plane_floats;
				//floats[1] = snowflake_floats;
				//floats[2] = cross_plane_floats;
				//floats[3] = tunnel_floats;
				//floats[4] = hollow_cube_floats;
				//floats[5] = dice_floats;
				//floats[6] = nested_floats;
				//
				//formations = new Vector [7][];
				//
				//for (i = 0; i < 7; i++) {
				//	formations[i] = new Vector [144];
				//	for (int j = 0; i < 144; i++) {
				//		float x = floats[i][j, 0];
				//		float y = floats[i][j, 1];
				//		float z = floats[i][j, 2];
				//		
				//		formations[i][j] = new Vector (x, y, z);
				//	}
				//}
				
				float x, y, z;
				
				formations = new Vector [7][];
				for (i = 0; i < 7; i++)
					formations[i] = new Vector [144];
				
				// flat plane formation
				for (i = 0; i < 144; i++) {
					x = flat_plane_floats[i, 0];
					y = flat_plane_floats[i, 1];
					z = flat_plane_floats[i, 2];
					
					formations[0][i] = new Vector (x, y, z);
				}
				
				// snowflake formation
				for (i = 0; i < 144; i++) {
					x = snowflake_floats[i, 0];
					y = snowflake_floats[i, 1];
					z = snowflake_floats[i, 2];
					
					formations[1][i] = new Vector (x, y, z);
				}
				
				// cross-plane formation
				for (i = 0; i < 144; i++) {
					x = cross_plane_floats[i, 0];
					y = cross_plane_floats[i, 1];
					z = cross_plane_floats[i, 2];
					
					formations[2][i] = new Vector (x, y, z);
				}
				
				// tunnel formation
				for (i = 0; i < 144; i++) {
					x = tunnel_floats[i, 0];
					y = tunnel_floats[i, 1];
					z = tunnel_floats[i, 2];
					
					formations[3][i] = new Vector (x, y, z);
				}
				
				// hollow cube formation
				for (i = 0; i < 144; i++) {
					x = hollow_cube_floats[i, 0];
					y = hollow_cube_floats[i, 1];
					z = hollow_cube_floats[i, 2];
					
					formations[4][i] = new Vector (x, y, z);
				}
				
				// dice formation
				for (i = 0; i < 144; i++) {
					x = dice_floats[i, 0];
					y = dice_floats[i, 1];
					z = dice_floats[i, 2];
					
					formations[5][i] = new Vector (x, y, z);
				}
				
				// nested cubes formation
				for (i = 0; i < 144; i++) {
					x = nested_floats[i, 0];
					y = nested_floats[i, 1];
					z = nested_floats[i, 2];
					
					formations[6][i] = new Vector (x, y, z);
				}
			}
			
			cubes = new Vector [144];
			for (i = 0; i < 144; i++)
				cubes[i] = new Vector ();
		}
		
		static int GetCubeId () {
			byte[] light = new byte[3] { 135, 206, 255 };
			byte[] dark = new byte[3] { 108, 166, 205 };
			int id;
			
			id = Gl.glGenLists (1);
			Gl.glNewList (id, Gl.GL_COMPILE);
			
			Gl.glColor3ubv (light);
			
			/* top panel (ccw) */
			Gl.glBegin (Gl.GL_TRIANGLE_STRIP);
			Gl.glNormal3f ( 0.0f, -1.0f,  0.0f);
			Gl.glVertex3f (-0.5f, -0.5f,  0.5f);
			Gl.glVertex3f (-0.5f, -0.5f, -0.5f);
			Gl.glVertex3f ( 0.5f, -0.5f,  0.5f);
			Gl.glVertex3f ( 0.5f, -0.5f, -0.5f);
			Gl.glEnd ();
			
			/* bottom panel (cw) */
			Gl.glBegin (Gl.GL_TRIANGLE_STRIP);
			Gl.glNormal3f ( 0.0f,  1.0f,  0.0f);
			Gl.glVertex3f ( 0.5f,  0.5f, -0.5f);
			Gl.glVertex3f (-0.5f,  0.5f, -0.5f);
			Gl.glVertex3f ( 0.5f,  0.5f,  0.5f);
			Gl.glVertex3f (-0.5f,  0.5f,  0.5f);
			Gl.glEnd ();
			
			/* dark colour for these */
			Gl.glColor3ubv (dark);
			
			/* right panel */
			Gl.glBegin (Gl.GL_TRIANGLE_STRIP);
			Gl.glNormal3f ( 1.0f,  0.0f,  0.0f);
			Gl.glVertex3f ( 0.5f, -0.5f,  0.5f);
			Gl.glVertex3f ( 0.5f, -0.5f, -0.5f);
			Gl.glVertex3f ( 0.5f,  0.5f,  0.5f);
			Gl.glVertex3f ( 0.5f,  0.5f, -0.5f);
			Gl.glEnd ();
			
			/* left panel */
			Gl.glBegin (Gl.GL_TRIANGLE_STRIP);
			Gl.glNormal3f (-1.0f,  0.0f,  0.0f);
			Gl.glVertex3f (-0.5f,  0.5f, -0.5f);
			Gl.glVertex3f (-0.5f, -0.5f, -0.5f);
			Gl.glVertex3f (-0.5f,  0.5f,  0.5f);
			Gl.glVertex3f (-0.5f, -0.5f,  0.5f);
			Gl.glEnd ();
			
			/* front panel */
			Gl.glBegin (Gl.GL_TRIANGLE_STRIP);
			Gl.glNormal3f ( 0.0f,  0.0f,  1.0f);
			Gl.glVertex3f (-0.5f,  0.5f,  0.5f);
			Gl.glVertex3f (-0.5f, -0.5f,  0.5f);
			Gl.glVertex3f ( 0.5f,  0.5f,  0.5f);
			Gl.glVertex3f ( 0.5f, -0.5f,  0.5f);
			Gl.glEnd ();
			
			/* back panel */
			Gl.glBegin (Gl.GL_TRIANGLE_STRIP);
			Gl.glNormal3f ( 0.0f,  0.0f, -1.0f);
			Gl.glVertex3f ( 0.5f, -0.5f, -0.5f);
			Gl.glVertex3f (-0.5f, -0.5f, -0.5f);
			Gl.glVertex3f ( 0.5f,  0.5f, -0.5f);
			Gl.glVertex3f (-0.5f,  0.5f, -0.5f);
			Gl.glEnd ();
			
			Gl.glEndList ();
			
			return id;
		}
		
		void InitCallLists () {
			if (cubeId == 0)
				cubeId = GetCubeId ();
		}
		
		// Interface Properties
		public override int DisplayFlags {
			get { return Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH; }
		}
		
		public override bool Complete {
			get { return state >= State.Complete; }
		}
		
		// Interface Methods
		public override void Reset (int width, int height) {
			Gl.glClearColor (0.0f, 0.0f, 0.0f, 0.0f);
			Gl.glClearDepth (1.0f);
			
			Gl.glShadeModel (Gl.GL_FLAT);
			
			Gl.glEnable (Gl.GL_CULL_FACE);
			Gl.glCullFace (Gl.GL_BACK);
			
			Gl.glEnable (Gl.GL_DEPTH_TEST);
			Gl.glDepthFunc (Gl.GL_LEQUAL);
			
			Gl.glDisable (Gl.GL_BLEND);
			
			Gl.glHint (Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
			
			if (lighting) {
				float[] light = new float[4] { 1.0f, 1.0f, 4.0f, 0.0f };
				float[] specular = new float[4] { 1.0f, 0.7f, 0.1f, 0.5f };
				float[] ambient = new float[4] { 0.529f, 0.8f, 1.0f, 0.5f };
				float[] shininess = new float[1] { 20.0f };
				
				Gl.glEnable (Gl.GL_LIGHT0);
				Gl.glEnable (Gl.GL_LIGHTING);
				Gl.glEnable (Gl.GL_NORMALIZE);
				
				Gl.glLightfv (Gl.GL_LIGHT0, Gl.GL_POSITION, light);
				
				Gl.glMaterialfv (Gl.GL_FRONT, Gl.GL_AMBIENT_AND_DIFFUSE, ambient);
				Gl.glMaterialfv (Gl.GL_FRONT, Gl.GL_SPECULAR, specular);
				Gl.glMaterialfv (Gl.GL_FRONT, Gl.GL_SHININESS, shininess);
				
				Gl.glDrawBuffer (Gl.GL_BACK);
			} else {
				Gl.glDisable (Gl.GL_LIGHT0);
				Gl.glDisable (Gl.GL_LIGHTING);
				Gl.glDisable (Gl.GL_NORMALIZE);
			}
			
			state = 0;
			
			start = 0;
			nFrames = 0;
			
			camera.pos.x = 20.0f;
			camera.pos.y = 400.0f;
			camera.pos.z = 20.0f;
			camera.rot.x = 0.0f;
			camera.rot.y = 0.0f;
			camera.rot.z = 0.0f;
			
			for (int i = 0; i < 144; i++) {
				cubes[i].x = formations[0][i].x;
				cubes[i].y = formations[0][i].y;
				cubes[i].z = formations[0][i].z;
			}
			
			InitCallLists ();
			
			Resize (width, height);
		}
		
		public override void Resize (int width, int height) {
			Gl.glViewport (0, 0, width, height);
			Gl.glMatrixMode (Gl.GL_PROJECTION);
			Gl.glLoadIdentity ();
			Glu.gluPerspective (45.0f, (double) width / (double) height, 0.1f, 400.0f);
			Gl.glMatrixMode (Gl.GL_MODELVIEW);
			Gl.glLoadIdentity ();
		}
		
		static int MoveCube (ref Vector cube, Vector start, Vector finish) {
			Vector delta = finish - cube;
			
			if (delta <= 0.001f) {
				cube.x = finish.x;
				cube.y = finish.y;
				cube.z = finish.z;
				return 0;
			}
			
			delta = finish - start;
			delta *= 0.005f;
			
			cube += delta;
			
			return 1;
		}
		
		public override bool AdvanceFrame () {
			int prev, next, moved = 0;
			int i;
			
			if ((((int) state) & 0x1) != 0) {
				/* we are between states... move each cube closer to
				 * where it is supposed to be for the next state */
				
				// next formation...
				prev = (((int) state) >> 1);
				next = prev + 1;
				
				for (i = 0; i < 144; i++)
					moved += MoveCube (ref cubes[i], formations[prev][i], formations[next][i]);
				
				if (moved == 0) {
					start = nFrames;
					state++;
				}
			} else {
				// last for 600 frames for each state
				if ((nFrames - start) >= 600)
					state++;
			}
			
			// rotate the camera
			camera.rot.x += 0.7f;
			camera.rot.y += 0.8f;
			camera.rot.z += 0.9f;
			
			// zoom the camera
			camera.pos.y -= (camera.pos.y / 75.0f);
			if (camera.pos.y < 0.01f)
				camera.pos.y = 0.0f;
			
			nFrames++;
			
			return state != State.Complete;
		}
		
		public override void RenderFrame () {
			Gl.glClear (Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			Gl.glPolygonMode (Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
			Gl.glLoadIdentity ();
			
			Glu.gluLookAt (camera.pos.x,
			               camera.pos.y,
			               camera.pos.z,
			               0.0f, 0.0f, 0.0f,
			               0.0f, 0.1f, 0.0f);
			
			Gl.glPushMatrix ();
			Gl.glRotatef (camera.rot.x, 1.0f, 0.0f, 0.0f);
			Gl.glRotatef (camera.rot.y, 0.0f, 1.0f, 0.0f);
			Gl.glRotatef (camera.rot.z, 0.0f, 0.0f, 1.0f);
			
			for (int i = 0; i < 144; i++) {
				Vector delta;
				
				if (i == 0) {
					delta = cubes[0];
				} else {
					delta = cubes[i] - cubes[i - 1];
				}
				
				Gl.glTranslatef (delta.x, delta.y, delta.z);
				Gl.glCallList (cubeId);
			}
			
			Gl.glPopMatrix ();
		}
	}
}
