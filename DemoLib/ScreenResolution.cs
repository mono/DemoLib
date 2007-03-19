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
	public partial class ScreenResolution : Gtk.Dialog {
		bool exitClicked;
		
		void PlayClicked (object button, System.EventArgs args) {
			exitClicked = false;
			this.Hide ();
			Gtk.Main.Quit ();
		}
		
		void ExitClicked (object button, System.EventArgs args) {
			exitClicked = true;
			this.Hide ();
			Gtk.Main.Quit ();
		}
		
		// Constructors
		public ScreenResolution () {
			Build ();
			
			button_play.Clicked += PlayClicked;
			button_exit.Clicked += ExitClicked;
		}
		
		public ScreenResolution (int width, int height) 
			: this () {
			Gtk.ToggleButton radio;
			string label;
			
			label = width.ToString () + "x" + height.ToString ();
			switch (label) {
			case "320x240":
				radio = radio_320x240;
				break;
			case "640x480":
				radio = radio_640x480;
				break;
			case "800x600":
				radio = radio_800x600;
				break;
			case "1024x768":
				radio = radio_1024x768;
				break;
			case "1280x1024":
				radio = radio_1280x1024;
				break;
			case "1400x1280":
				radio = radio_1400x1280;
				break;
			default:
				throw new ArgumentOutOfRangeException ();
			}
				
			radio.Active = true;
		}
		
		public ScreenResolution (int width, int height, bool fullscreen)
			: this (width, height) {
			checkbox_fullscreen.Active = fullscreen;
		}
		
		public ScreenResolution (int width, int height, bool fullscreen, int colourDepth)
			: this (width, height, fullscreen) {
			if (colourDepth != 16 && colourDepth != 32)
				throw new ArgumentOutOfRangeException ("colourDepth");
			
			if (colourDepth == 16)
				radio_16bit.Active = true;
			else if (colourDepth == 32)
				radio_32bit.Active = true;
		}
		
		// Properties
		public bool Exit {
			get { return exitClicked; }
		}
		
		public int Width {
			get {
				if (radio_320x240.Active)
					return 320;
				if (radio_640x480.Active)
					return 640;
				if (radio_800x600.Active)
					return 800;
				if (radio_1024x768.Active)
					return 1024;
				if (radio_1280x1024.Active)
					return 1280;
				if (radio_1400x1280.Active)
					return 1400;
				
				return -1;
			}
		}
		
		public int Height {
			get {
				if (radio_320x240.Active)
					return 240;
				if (radio_640x480.Active)
					return 480;
				if (radio_800x600.Active)
					return 600;
				if (radio_1024x768.Active)
					return 768;
				if (radio_1280x1024.Active)
					return 1024;
				if (radio_1400x1280.Active)
					return 1280;
				
				return -1;
			}
		}
		
		public bool FullScreen {
			get { return checkbox_fullscreen.Active; }
		}
		
		public int ColourDepth {
			get {
				if (radio_16bit.Active)
					return 16;
				if (radio_32bit.Active)
					return 32;
				
				return -1;
			}
		}
	}
}
