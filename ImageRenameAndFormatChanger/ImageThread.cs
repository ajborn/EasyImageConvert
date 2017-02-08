using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.IO;

namespace ImageRenameAndFormatChanger {
	// The ThreadWithState class contains the information needed for
	// a task, and the method that executes the task.
	//
	public class ImageThread {
        // State information used in the task.
        private int BeginningIndex;
        private string[] ImageNames;
        private string StartingIndex;
        private string ThreadName;
        private UserInput ThreadUserInput;
        private int EndingIndex;

		// The constructor obtains the state information.
		public ImageThread(string[] imageNames, UserInput threadUserInput, string startingIndex, int beginningIndex, int endingIndex, string threadName) {
			ImageNames = imageNames;
			StartingIndex = startingIndex;
			ThreadUserInput = threadUserInput;
            BeginningIndex = beginningIndex;
            EndingIndex = imageNames.Length - endingIndex;
            ThreadName = threadName;
        }
        // The thread procedure performs the task, such as formatting
        // and printing a document.
        public void ProcessFiles() {
            string[] log = new string[ImageNames.Length];
            var newDirectory = $"{ThreadUserInput.ImageDirectory}\\RenamedImages";
            var index = 0;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var outputIndex = 0;
            var outputForEach = ImageNames.Length > 10 ? true : false;
            Console.WriteLine($"BeginningIndex{BeginningIndex}  EndingIndex{(EndingIndex)}");
            for (var i = BeginningIndex; i < EndingIndex; i++) {
                Image newImage = null;
                //First operation of file processing
                var parsedStringIndex = FormatFileNameZeroIndex(ThreadUserInput, StartingIndex, i);
                //Second operation of file processing
                try {
                    newImage = Image.FromFile(ImageNames[i]);
                } catch (Exception e) {
                    Console.WriteLine($"\tSkipped {ImageNames[i]}\n\n\tThis file is not an Image file = {ImageNames[i]}\n\n");
                    File.AppendAllText("ErrorLog.txt", e.ToString());
                    continue;
                }
                //Third operation Generate new filename
                var newFileName = $"{ThreadUserInput.FileGroupDescription}_{parsedStringIndex}";
                var fileInputFormatString = ThreadUserInput.FileFormatInput == "2" ? "jpg" : ThreadUserInput.ParsedFileFormat?.ToString().ToLower();
                try {
                    //Fourth operation output new file name and append to log
                    log[i] = $"{DateTime.Now.ToString()} Elapsed time: {watch.Elapsed} Renamed file {ImageNames[i]} to {newFileName}.{fileInputFormatString}\n\n ";

                    ///Fifth operation to complete//Save the image
                    newImage.Save(Path.Combine(newDirectory, newFileName + "." + (fileInputFormatString)), ThreadUserInput.ParsedFileFormat);
                    newImage.Dispose();

                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                }
                outputIndex++;

                if (outputForEach) {
                    outputIndex = OutputElapsedTime(outputIndex, watch.Elapsed, $"{i}/{EndingIndex}");
                } else { Console.WriteLine($"Elaped Time: {watch.Elapsed}  Files Complete:  {i}/{EndingIndex}\n  "); }
                index = i+1;
            }
            watch.Stop();
            Console.WriteLine($"\n\nElaped Time: {watch.Elapsed}  Files Complete:  {index}/{EndingIndex}, Thread Name: {ThreadName}\n\n");
            foreach (var logstring in log) {
                File.AppendAllText($"Log{ThreadName}.txt", logstring);
            }

        }

        private int OutputElapsedTime(int outputIndex, TimeSpan elapsed, string complete) {
            if (outputIndex == 10) {
                Console.WriteLine($"Elaped Time: {elapsed} Files Complete:  {complete}\n  {ThreadName}");
                return 0;
            }
            return outputIndex;
        }

        public static string FormatFileNameZeroIndex(UserInput ThreadUserInput, string startingIndex, int index) {

			var parsedStringIndex = startingIndex == null && index == 0 ? startingIndex : index.ToString();

			if (!string.IsNullOrEmpty(startingIndex) && ThreadUserInput.FileGroupNumberOption == "0" && index > 0)
				parsedStringIndex = startingIndex.Substring(startingIndex.Length - (startingIndex.Length - parsedStringIndex.Length)) + index.ToString();

			return parsedStringIndex;
		}
	}
}
