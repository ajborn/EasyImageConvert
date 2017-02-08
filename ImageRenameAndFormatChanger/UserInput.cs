using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageRenameAndFormatChanger {
	public class UserInput {
		public string ImageDirectory { get; set; }
		public string FileGroupNumberOption { get; set; }
		public string FileGroupDescription { get; set; }

		private string _fileFormatInput = string.Empty;
		public string FileFormatInput
		{
			get { return _fileFormatInput; }
			set { _fileFormatInput = value; }
		}
		public ImageFormat ParsedFileFormat
		{
			get { return DetermineImageFormatFromString(FileFormatInput); }
		}

		public static ImageFormat DetermineImageFormatFromString(string formatFromUserInput = null) {
			var returnedImageFormat = ImageFormat.Jpeg;
			switch (formatFromUserInput) {
				case "1":
					return returnedImageFormat = ImageFormat.Jpeg;
				case "2":
					return returnedImageFormat = ImageFormat.Jpeg;
				case "3":
					return returnedImageFormat = ImageFormat.Png;
				case "4":
					return returnedImageFormat = ImageFormat.Gif;
				default:
					return returnedImageFormat = ImageFormat.Jpeg;

			}
		}
	}
}
