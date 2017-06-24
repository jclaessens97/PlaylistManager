using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistManager.Model
{
	public class Settings
	{
		public string Folder { get; private set; }
		public bool IncludeSubdirs { get; private set; }

		public Settings()
		{
			Folder = Globals.DEBUG_MUSIC_FOLDER_PATH; 
			IncludeSubdirs = Globals.DEBUG_INCLUDE_SUBDIRS;
		}


		public void ChangeFolder(string _path)
		{
			//TODO: change folder method
			throw new NotImplementedException();
		}
	}
}
