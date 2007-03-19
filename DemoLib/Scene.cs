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
	public class Scene : IScene {
		// Constructors
		public Scene () {
		}
		
		// Interface Properties
		public virtual int DisplayFlags {
			get { return 0; }
		}
		
		public virtual bool Complete {
			get { return false; }
		}
		
		// Interface Methods
		public virtual void Reset (int width, int height) {
			
		}
		
		public virtual void Resize (int width, int height) {
		
		}
		
		public virtual bool AdvanceFrame () {
			return true;
		}
		
		public virtual void RenderFrame () {
		
		}
	}
}
