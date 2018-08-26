# base64pdf
Simple command line program to convert base64 string to a PDF.

# Running program
After compiling program, go to the terminal and navigate to the compiled executable.  This program requires two parameters and accepts 3 total parameters. Attempting to run the program without any parameters will cause it to fail; however, it will spit out the parameters it needs to function.

- dump parameter, verbosely prints out things as the program executes.
- inputpath, the file path to where the input file is. This file should contain only a base64 string representing the PDF. Example, “\~/Desktop/testexample.txt”
- outfilename, the filename to be assigned to the file. This is not a path, just the name of the file itself, example “testexample.pdf”. The program will place the file in the current working directory where it is running from. So, if one is on the desktop and executes program from there, file will output to desktop.

## Example
./base64pdf --dump -t --inputpath /Users/dmason/Desktop/testexample.txt --outfilename testexample.pdf