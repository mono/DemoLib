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
	public class FloatingCageScene : Scene {
		static Vector CameraZoom = new Vector (0.0f, 0.0f, -0.08f);
		static Vector InnerRotation = new Vector (0.7f, 0.8f, 0.9f);
		static Vector OuterRotation = new Vector (-0.7f, -0.8f, -0.9f);
		
		static readonly float[,,] innerVerts = new float[4,4,3] {
			/* normal:  0.0f,  1.0f,  0.0f */
			{ { -4.0f, -3.0f,  3.0f },
			  { -4.0f, -3.0f, -3.0f },
			  { -3.0f, -3.0f,  3.0f },
			  { -3.0f, -3.0f, -3.0f },
			},
			{ {  3.0f, -3.0f,  3.0f },
			  {  3.0f, -3.0f, -3.0f },
			  {  4.0f, -3.0f,  3.0f },
			  {  4.0f, -3.0f, -3.0f },
			},
			{ { -3.0f, -3.0f,  4.0f },
			  { -3.0f, -3.0f,  3.0f },
			  {  3.0f, -3.0f,  4.0f },
			  {  3.0f, -3.0f,  3.0f },
			},
			{ { -3.0f, -3.0f, -3.0f },
			  { -3.0f, -3.0f, -4.0f },
			  {  3.0f, -3.0f, -3.0f },
			  {  3.0f, -3.0f, -4.0f },
			}
		};
		
		static readonly float[,,] outerVerts = new float[4,4,3] {
			/* normal:  0.0f,  1.0f,  0.0f */
			{ { -4.0f, -4.0f,  4.0f },
			  { -4.0f, -4.0f, -4.0f },
			  { -3.0f, -4.0f,  4.0f },
			  { -3.0f, -4.0f, -4.0f },
			},
			{ {  3.0f, -4.0f,  4.0f },
			  {  3.0f, -4.0f, -4.0f },
			  {  4.0f, -4.0f,  4.0f },
			  {  4.0f, -4.0f, -4.0f },
			},
			{ { -4.0f, -4.0f,  4.0f },
			  { -4.0f, -4.0f,  3.0f },
			  {  4.0f, -4.0f,  4.0f },
			  {  4.0f, -4.0f,  3.0f },
			},
			{ { -4.0f, -4.0f, -3.0f },
			  { -4.0f, -4.0f, -4.0f },
			  {  4.0f, -4.0f, -3.0f },
			  {  4.0f, -4.0f, -4.0f },
			}
		};
		
		enum State {
			Start       = 0,
			ZoomOut     = 1,
			ZoomedOut   = 2,
			ZoomIn      = 3,
			Finish      = 4,
			FadeOut     = 5,
			Complete
		}
		
		State state;
		int nFrames;
		
		Vertex innerCage;
		int innerCageId;
		
		Vertex outerCage;
		int outerCageId;
		
		Vertex camera;
		
		// Constructors
		public FloatingCageScene () : base () {
			innerCage = new Vertex ();
			outerCage = new Vertex ();
			camera = new Vertex ();
		}
		
		// Private initialization
		static void SwapVerts (ref float[,,] verts, int i, int v0, int v1) {
			float[] tmp = new float [3];
			int k;
			
			for (k = 0; k < 3; k++)
				tmp[k] = verts[i,v0,k];
			
			for (k = 0; k < 3; k++)
				verts[i,v0,k] = verts[i,v1,k];
			
			for (k = 0; k < 3; k++)
				verts[i,v1,k] = tmp[k];
		}
		
		static void DrawCage (byte[] colour, float[,,] verts, float normv, bool swap) {
			int norm = 1;
			int i, j;
			
			do {
				float normx, normy, normz;
				
				Gl.glColor3ubv (colour);
				
				switch (norm) {
				case 0:
					normx = normv;
					normy = 0.0f;
					normz = 0.0f;
					break;
				case 1:
				default:
					normx = 0.0f;
					normy = normv;
					normz = 0.0f;
					break;
				case 2:
					normx = 0.0f;
					normy = 0.0f;
					normz = normv;
					break;
				}
				
				for (i = 0; i < 4; i++) {
					if (swap)
						SwapVerts (ref verts, i, 0, 3);
					
					/* draw 4 faces */
					Gl.glBegin (Gl.GL_TRIANGLE_STRIP);
					Gl.glNormal3f (normx, normy, normz);
					for (j = 0; j < 4; j++) {
						Gl.glVertex3fv (ref verts[i,j,0]);
						verts[i,j,norm] = -verts[i,j,norm];
					}
					Gl.glEnd ();
					
					SwapVerts (ref verts, i, 0, 3);
					
					/* draw the opposite 4 faces */
					Gl.glBegin (Gl.GL_TRIANGLE_STRIP);
					Gl.glNormal3f (-normx, -normy, -normz);
					for (j = 0; j < 4; j++) {
						float tmp;
						
						Gl.glVertex3fv (ref verts[i,j,0]);
						verts[i,j,norm] = -verts[i,j,norm];
						
						/* shift all co-ords one axis */
						if (norm == 1) {
							tmp = verts[i,j,0];
							verts[i,j,0] = verts[i,j,1];
							verts[i,j,1] = tmp;
						} else {
							tmp = verts[i,j,0];
							verts[i,j,0] = verts[i,j,2];
							verts[i,j,2] = tmp;
						}
					}
					Gl.glEnd ();
					
					if (swap)
						SwapVerts (ref verts, i, 0, 3);
				}
				
				/* darken the colour */
				if (colour[0] > 0)
					colour[0] -= 64;
				if (colour[1] > 0)
					colour[1] -= 64;
				if (colour[2] > 0)
					colour[2] -= 64;
				
				norm = (norm + 2) % 3;
			} while (norm != 1);
		}
		
		static int GetCageId (byte[] colour) {
			byte[] red = new byte[3] { 226, 0, 0 };
			float[,,] verts = new float[4,4,3];
			int id;
			
			id = Gl.glGenLists (1);
			Gl.glNewList (id, Gl.GL_COMPILE);
			
			Array.Copy (outerVerts, verts, 48);
			DrawCage (red, verts, 1.0f, false);
			
			Array.Copy (innerVerts, verts, 48);
			DrawCage (colour, verts, 1.0f, true);
			
			Gl.glEndList ();
			
			return id;
		}
		
		void InitCallLists () {
			if (innerCageId != 0)
				return;
			
			byte[] colour = new byte[3] { 0, 0, 0 };
			
			colour[1] = 226;
			innerCageId = GetCageId (colour);
			colour[1] = 0;
			
			colour[2] = 226;
			outerCageId = GetCageId (colour);
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
			
			state = State.Start;
			nFrames = 0;
			
			innerCage.x = 0.0f;
			innerCage.y = 0.0f;
			innerCage.z = 0.0f;
			
			outerCage.x = 0.0f;
			outerCage.y = 0.0f;
			outerCage.z = 0.0f;
			
			camera.x = 0.0f;
			camera.y = 0.0f;
			camera.z = -1.3f;
			
			InitCallLists ();
			
			Resize (width, height);
		}
		
		public override void Resize (int width, int height) {
			Gl.glViewport (0, 0, width, height);
			Gl.glMatrixMode (Gl.GL_PROJECTION);
			Gl.glLoadIdentity ();
			Glu.gluPerspective (45.0f, (double) width / (double) height, 0.1f, 100.0f);
			Gl.glMatrixMode (Gl.GL_MODELVIEW);
			Gl.glLoadIdentity ();
		}
		
		public override bool AdvanceFrame () {
			bool zoom = false;
			
			innerCage += InnerRotation;
			outerCage += OuterRotation;
			
			if (state == State.ZoomIn) {
				camera -= CameraZoom;
				zoom = true;
			} else if (state == State.ZoomOut) {
				camera += CameraZoom;
				zoom = true;
			} else if (state == State.FadeOut) {
				// this is a short-lived state as well
				zoom = true;
			}
			
			if ((zoom && nFrames == 175) || nFrames == 800) {
				nFrames = 0;
				state++;
			} else
				nFrames++;
			
			return state != State.Complete;
		}
		
		public override void RenderFrame () {
			byte[] fade = new byte [4] { 0, 0, 0, 0 };
			
			Gl.glClear (Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			
			Gl.glPolygonMode (Gl.GL_FRONT_AND_BACK, Gl.GL_FILL);
			
			Gl.glLoadIdentity ();
			
			Gl.glTranslatef (camera.x, camera.y, camera.z);
			
			Gl.glPushMatrix ();
			Gl.glScalef (0.3f, 0.3f, 0.3f);
			Gl.glRotatef (innerCage.x, 1.0f, 0.0f, 0.0f);
			Gl.glRotatef (innerCage.y, 0.0f, 1.0f, 0.0f);
			Gl.glRotatef (innerCage.z, 0.0f, 0.0f, 1.0f);
			Gl.glCallList (innerCageId);
			Gl.glPopMatrix ();
			
			Gl.glPushMatrix ();
			Gl.glScalef (0.6f, 0.6f, 0.6f);
			Gl.glRotatef (outerCage.x, 1.0f, 0.0f, 0.0f);
			Gl.glRotatef (outerCage.y, 0.0f, 1.0f, 0.0f);
			Gl.glRotatef (outerCage.z, 0.0f, 0.0f, 1.0f);
			Gl.glCallList (outerCageId);
			Gl.glPopMatrix ();
			
			if (state == State.Start && nFrames < 128) {
				Gl.glEnable (Gl.GL_BLEND);
				Gl.glBlendFunc (Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
				
				Gl.glDisable (Gl.GL_DEPTH_TEST);
				
				fade[3] = (byte) (255 - (nFrames * 2));
				
				Gl.glColor4ubv (fade);
				
				Gl.glBegin (Gl.GL_TRIANGLE_STRIP);
				Gl.glNormal3f ( 0.0f,  0.0f,  1.0f);
				Gl.glVertex3f (-1.0f,  1.0f,  0.0f);
				Gl.glVertex3f (-1.0f, -1.0f,  0.0f);
				Gl.glVertex3f ( 1.0f,  1.0f,  0.0f);
				Gl.glVertex3f ( 1.0f, -1.0f,  0.0f);
				Gl.glEnd ();
				
				Gl.glDisable (Gl.GL_BLEND);
				
				Gl.glEnable (Gl.GL_DEPTH_TEST);
				Gl.glDepthFunc (Gl.GL_LEQUAL);
			} else if (state == State.FadeOut) {
				Gl.glEnable (Gl.GL_BLEND);
				Gl.glBlendFunc (Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
				
				Gl.glDisable (Gl.GL_DEPTH_TEST);
				
				fade[3] = (byte) (nFrames * (255.0f / 175.0f));
				
				Gl.glColor4ubv (fade);
				
				Gl.glBegin (Gl.GL_TRIANGLE_STRIP);
				Gl.glNormal3f ( 0.0f,  0.0f,  1.0f);
				Gl.glVertex3f (-1.0f,  1.0f,  0.0f);
				Gl.glVertex3f (-1.0f, -1.0f,  0.0f);
				Gl.glVertex3f ( 1.0f,  1.0f,  0.0f);
				Gl.glVertex3f ( 1.0f, -1.0f,  0.0f);
				Gl.glEnd ();
				
				Gl.glDisable (Gl.GL_BLEND);
				
				Gl.glEnable (Gl.GL_DEPTH_TEST);
				Gl.glDepthFunc (Gl.GL_LEQUAL);
			}
		}
	}
}
