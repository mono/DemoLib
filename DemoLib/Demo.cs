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
using System.Collections;
using Tao.OpenGl;
using Tao.FreeGlut;

namespace DemoLib {
	public class Demo {
		static string title = "DemoLib Demo";
		static int displayFlags = 0;
		static int height = 560;
		static int width = 704;
		static bool fullscreen;
		static bool loop;
		
		static ArrayList scenes;
		static int scene = 0;
		
		static int lastFrame = 0;
		
		// FPS vars
		static int fpsTarget = 80;
		static int fpsTimerStart = 0;
		static int vfpsNumFrames = 0;
		static int fpsNumFrames = 0;
		
		// Constructors
		public Demo () {
			Glut.glutInit ();
			
			SetGLDefaults ();
		}
		
		static void SetGLDefaults () {
			Gl.glClearColor (0.0f, 0.0f, 0.0f, 0.0f);
			
			Gl.glDisable (Gl.GL_BLEND);
			
			Gl.glDisable (Gl.GL_CULL_FACE);
			Gl.glDisable (Gl.GL_DEPTH_TEST);
			
			Gl.glDisable (Gl.GL_FOG);
			Gl.glDisable (Gl.GL_DITHER);
			Gl.glDisable (Gl.GL_LINE_STIPPLE);
			
			Gl.glDisable (Gl.GL_LIGHT0);
			Gl.glDisable (Gl.GL_LIGHTING);
			Gl.glDisable (Gl.GL_NORMALIZE);
			
			Gl.glDisable (Gl.GL_TEXTURE_1D);
			Gl.glDisable (Gl.GL_TEXTURE_2D);
			Gl.glDisable (Gl.GL_TEXTURE_3D);
		}
		
		static void SetGLCallbacks () {
			Glut.glutDisplayFunc (new Glut.DisplayCallback (Display));
			Glut.glutReshapeFunc (new Glut.ReshapeCallback (Reshape));
			Glut.glutIdleFunc (new Glut.IdleCallback (Idle));
			
			Glut.glutKeyboardFunc (new Glut.KeyboardCallback (Keyboard));
			Glut.glutSpecialFunc (new Glut.SpecialCallback (Special));
		}
		
		// Properties
		public int DesiredFPS {
			get { return fpsTarget; }
			set { fpsTarget = value; }
		}
		
		// Methods
		public void Init (string title) {
			Init (title, 704, 560, false);
		}
		
		public void Init (string title, int width, int height) {
			Init (title, width, height, false);
		}
		
		public void Init (string title, int width, int height, bool fullscreen) {
			Demo.fullscreen = fullscreen;
			Demo.height = height;
			Demo.width = width;
			Demo.title = title;
			
			scenes = new ArrayList ();
		}
		
		public void AddScene (Scene scene) {
			if (scene == null)
				throw new ArgumentNullException ();
			
			/* Collect other glutInitDisplayMode flags: some scenes may need the 
			 * stencil buffer, others might need the accum buffer, still others 
			 * might need the depth buffer, etc. */
			displayFlags |= scene.DisplayFlags;
			
			scenes.Add (scene);
		}
		
		public void Run () {
			Run (false);
		}
		
		public void Run (bool loop) {
			if (scenes.Count < 1)
				return;
			
			Glut.glutInitDisplayMode (Glut.GLUT_RGBA | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
			Glut.glutInitWindowSize (width, height);
			
			Glut.glutCreateWindow (title);
			
			if (fullscreen)
				Glut.glutFullScreen ();
			
			Demo.loop = loop;
			
			((Scene) scenes[0]).Reset (width, height);
			
			SetGLCallbacks ();
			
			Glut.glutMainLoop ();
		}
		
		static void RenderFPS () {
			int now = Environment.TickCount;
			
			if (fpsTimerStart == 0) {
				fpsTimerStart = now;
				return;
			}
			
			if ((now - fpsTimerStart) > 5000) {
				//float secs = (now - fpsTimerStart) / 1000;
				//System.Console.WriteLine ("{0} frames ({1} virtual) in {2:f1} seconds = {3:f3} FPS ({4:f3} virtual)",
				//                          fpsNumFrames, vfpsNumFrames, secs, fpsNumFrames / secs, vfpsNumFrames / secs);
				fpsTimerStart = now;
				vfpsNumFrames = 0;
				fpsNumFrames = 0;
			}
		}
		
		// OpenGL Event callbacks
		static void Display () {
			((Scene) scenes[scene]).RenderFrame ();
			//fpsNumFrames++;
			RenderFPS ();
			
			Glut.glutSwapBuffers ();
		}
		
		static void Reshape (int width, int height) {
			Demo.height = height;
			Demo.width = width;
			
			((Scene) scenes[scene]).Resize (width, height);
		}
		
		static int FramesElapsed () {
			int now = Environment.TickCount;
			int n;
			
			if (lastFrame == 0) {
				lastFrame = now;
				return 1;
			}
			
			if ((n = ((now - lastFrame) / (1000 / fpsTarget))) > 0) {
				lastFrame = now;
				return n;
			}
			
			return 0;
		}
		
		static void Idle () {
			int nFrames = FramesElapsed ();
			int i = 0;
			
			if (nFrames == 0) {
				//Glut.glutPostRedisplay ();
				return;
			}
			
			while (i < nFrames) {
				if (!((Scene) scenes[scene]).AdvanceFrame ()) {
					scene++;
					
					if (scene == scenes.Count) {
						if (!loop)
							Environment.Exit (0);
						
						scene = 0;
					}
					
					((Scene) scenes[scene]).Reset (width, height);
				}
				
				i++;
			}
			
			fpsNumFrames++;
			vfpsNumFrames += nFrames;
			Glut.glutPostRedisplay ();
		}
		
		static void Keyboard (byte c, int x, int y) {
			switch ((char) c) {
				case 'q':
				case 'Q':
					Environment.Exit (0);
					break;
			}
		}
		
		static void Special (int c, int x, int y) {
			switch (c) {
				case 27: // Escape
					Environment.Exit (0);
					break;
			}
		}
	}
}
