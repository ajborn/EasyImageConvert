using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace ImageRenameAndFormatChanger {
	public class Program {

		public const string TOO_MANY_FILES = "Too many files in your directory!!\n\n";
		public static void Main(string[] args) {
			Console.WriteLine("\nHello! Enjoy renaming your images.\n\n");

			MainUserInput.ImageDirectory = EnterDirectory();
			MainUserInput.FileGroupDescription = EnterFileGroupDescription();
			MainUserInput.FileGroupNumberOption = EnterFileGroupNumberOption();
			MainUserInput.FileFormatInput = EnterFileFormat();

			Console.WriteLine("\nStarting Image rename....\n\n");
			RenameFiles(MainUserInput);
            
            
		}

		private static UserInput MainUserInput = new UserInput();

		public static string EnterFileFormat(int numberOfFilesToProcess = 0) {
			Console.WriteLine("[[Enter the number for your file format.]]\n\n {{\tFile Extensions: (1) .jpeg  (2) .jpg (3) .png (4) .gif}} \n\n");

			var getInput = Console.ReadLine();
			if (string.IsNullOrEmpty(getInput)) {
				Console.Write("\tNothing was entered!");
				return EnterFileFormat();
			}

			return getInput;
		}

		public static string EnterDirectory() {
			Console.WriteLine("Enter the directory where you want to rename a directory of Images....\n\n");

			var getInput = Console.ReadLine().Trim();
			if (string.IsNullOrEmpty(getInput) || !Directory.Exists(getInput)) {
				Console.WriteLine("Directory does not exist.");
				return EnterDirectory();
			}

			return getInput;
		}

		public static string EnterFileGroupNumberOption(string message = null) {
			if (string.IsNullOrEmpty(message)) {
				Console.WriteLine("\n\nEnter a file group numbering option:\n\n\t (0) Trailing Zeros - DecemberTrip_[00].jpeg\n\n\t (1) Without Trailing Zeros - DecebmerTrip_123.\n\n\t ***Your format will change what is in the square brackets.***");
				Console.WriteLine("\n\nNumbers will add automatically be added to the end of the file name. \n\n");
			} else
				Console.WriteLine(message);

			var getInput = Console.ReadLine().Trim();

			if (string.IsNullOrEmpty(getInput)) {
				Console.WriteLine("You did not enter anything, please enter a file name format....\n\n");
				return EnterFileGroupNumberOption();
			}

			switch (getInput) {
				case "0":
					return getInput = "0";
				case "1":
					return getInput = "1";
				default:
					return EnterFileGroupNumberOption("You did not enter a valid option. Please enter 0 for leading zeros and 1 for no leading zeros.");
			}
		}

		public static string EnterFileGroupDescription(string message = null) {

			if (string.IsNullOrEmpty(message)) Console.WriteLine("\n\nEnter a file group description:\n\n");
			else Console.WriteLine(message);

			var getInput = Console.ReadLine().Trim();

			if (string.IsNullOrEmpty(getInput)) {
				Console.WriteLine("You did not enter anything, please enter a file group description....\n\n");
				return EnterFileGroupDescription();
			}
			return getInput;
		}

		public static string DetermineStartingStringOfZeros(int numberOfFiles) {
			if (numberOfFiles < 98)
				return "00";
			else if (numberOfFiles < 998)
				return "000";
			else if (numberOfFiles < 9998)
				return "0000";
			return TOO_MANY_FILES;
		}

		public static void RenameFiles(UserInput userInput) {
			//Get All the image Names to generate an Image Object
			var imageNames = Directory.GetFiles(userInput.ImageDirectory);
			//Get the number of files in the directory for setting are naming format
			var numberOfFiles = imageNames.Length;
			//Holds leading zero string
			string startingIndex = string.IsNullOrEmpty(userInput.FileGroupDescription) && userInput.FileGroupNumberOption == "1" ? null : DetermineStartingStringOfZeros(numberOfFiles);

			if (startingIndex != null && startingIndex == TOO_MANY_FILES) {
				Console.WriteLine("\n\n\tYour directory has too many images. Restart the Application.  Press any key to exit...");
				Console.ReadKey();
			}

            var newDirectory = $"{userInput.ImageDirectory}\\RenamedImages";
            if (!Directory.Exists(newDirectory))
                Directory.CreateDirectory(newDirectory);
            int beginningIndex = imageNames.Length / 2;
            ImageThread imageThread1 = new ImageThread(imageNames, userInput, startingIndex, 0, beginningIndex + 1, "thread1");
            ImageThread imageThread2 = new ImageThread(imageNames, userInput, startingIndex, beginningIndex, 0, "thread2");
            Thread thread1 = new Thread(new ThreadStart(imageThread1.ProcessFiles));
            Thread thread2 = new Thread(new ThreadStart(imageThread2.ProcessFiles));

            thread1.Priority = ThreadPriority.Highest;
            thread2.Priority = ThreadPriority.Highest;

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();
            
            Console.WriteLine("Do you want to delete your old images? y/n");
            var delete = Console.ReadLine();
            if (!string.IsNullOrEmpty(delete) && delete?.ToLower() == "y") {
               foreach (var oldImageName in imageNames) {
                    File.Delete(Path.Combine(MainUserInput.ImageDirectory, oldImageName));
                }
            }

        }

        //public static void ProcessFiles(UserInput userInput, string[] imageNames, string startingIndex) {
        //    var index = 0;
        //    string[] log = new string[imageNames.Length];
            
        //    var newDirectory = $"{userInput.ImageDirectory}\\RenamedImages";
        //    if (!Directory.Exists(newDirectory))
        //        Directory.CreateDirectory(newDirectory);
        //    var watch = System.Diagnostics.Stopwatch.StartNew();

        //    var outputIndex = 0;
        //    var outputForEach = imageNames.Length > 10 ? true : false;
        //    foreach (var oldImageName in imageNames) {

        //        Image newImage = null;
        //        //First operation of file processing
        //        var parsedStringIndex = FormatFileNameZeroIndex(userInput, startingIndex, index);
        //        //Second operation of file processing
        //        try {
        //            newImage = Image.FromFile(oldImageName);
        //        } catch (Exception e) {
        //            Console.WriteLine($"\tSkipped {oldImageName}\n\n\tThis file is not an Image file = {oldImageName}\n\n");
        //            File.AppendAllText("ErrorLog.txt", e.ToString());
        //            continue;
        //        }
        //        //Third operation Generate new filename
        //        var newFileName = $"{userInput.FileGroupDescription}_{parsedStringIndex}";
        //        var fileInputFormatString = userInput.FileFormatInput == "2" ? "jpg" : userInput.ParsedFileFormat?.ToString().ToLower();
        //        try {
        //            //Fourth operation output new file name and append to log
        //            log[index] = $"{DateTime.Now.ToString()} Elapsed time: {watch.Elapsed} Renamed file {oldImageName} to {newFileName}.{fileInputFormatString}\n";

        //            ///Fifth operation to complete//Save the image
        //            newImage.Save(Path.Combine(newDirectory, newFileName + "." + (fileInputFormatString)), userInput.ParsedFileFormat);
        //            newImage.Dispose();

        //        } catch (Exception e) {
        //            Console.WriteLine(e.ToString());
        //        }
        //        index++;
        //        outputIndex++;

        //        if (outputForEach) {
        //            outputIndex = OutputElapsedTime(outputIndex, watch.Elapsed, $"{index}/{imageNames.Length}");
        //        } else { Console.WriteLine($"Elaped Time: {watch.Elapsed}  Files Complete:  {index}/{imageNames.Length}\n  "); }
        //    }
        //    watch.Stop();
        //    Console.WriteLine($"\n\nElaped Time: {watch.Elapsed}  Files Complete:  {index}/{imageNames.Length}\n\n");
        //    foreach (var logstring in log) {
        //        File.AppendAllText("Log.txt", logstring);
        //    }
            
        //}

        //private static int OutputElapsedTime(int outputIndex, TimeSpan elapsed, string complete) {
        //    if (outputIndex == 10) {
        //        Console.WriteLine($"Elaped Time: {elapsed} Files Complete:  {complete}\n  ");
        //        return 0;
        //    }
        //    return outputIndex;
        //}

        //public static string FormatFileNameZeroIndex(UserInput userInput, string startingIndex, int index) {

        //    var parsedStringIndex = startingIndex == null && index == 0 ? startingIndex : index.ToString();

        //    if (!string.IsNullOrEmpty(startingIndex) && userInput.FileGroupNumberOption == "0" && index > 0)
        //        parsedStringIndex = startingIndex.Substring(startingIndex.Length - (startingIndex.Length - parsedStringIndex.Length)) + index.ToString();

        //    return parsedStringIndex;
        //}
    }
}

