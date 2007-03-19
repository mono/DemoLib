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
using Gtk;

namespace DemoLib {
	class Program {
		public static void Main (string[] args) {
			Application.Init ();
			ScreenResolution res = new ScreenResolution ();
			res.Show ();
			Application.Run ();
			
			if (res.Exit) {
				System.Console.WriteLine ("User clicked Exit");
				return;
			}
			
			System.Console.WriteLine ("Resolution: {0}x{1}:{2} FullScreen={3}",
									  res.Width, res.Height, res.ColourDepth, res.FullScreen);
			
			Demo demo = new Demo ();
			demo.Init ("OpenGL#", res.Width, res.Height, res.FullScreen);
			
			Scene scene = new VectorCubesScene (false);
			demo.AddScene (scene);
			
			scene = new FloatingCageScene ();
			demo.AddScene (scene);
			
			demo.Run (true);
			
			return;
		}
	}
}