// first LRS Filter program for Address labels
// Basic function to cover printing the labels 
// this was done quickly to cover CBS problem discovered after linux conversion go live



using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LabelFilter
{
    class Program
    {
        private static bool overwrite;

        static int Main(string[] args)
        {

            //Declare any variables in use
            int counter = 0;

            String InputFileName;
            String OutputFileName;
            //String AttrFileName;
            String line;
            String TempFile;
            String term = "\r\n";

            string[] LabelLine = new string[14];
            string ZPLString = "";

            //The filter definition should look like this
            //Datatype all
            //Command - filter.exe (this program)
            //Arguments: &infile &outfile &attrfile

            InputFileName = args[0];
            OutputFileName = args[1];
            //AttrFileName = args[2];


            //InputFileName = @"C:\test\Read4.txt";
            //OutputFileName = @"C:\test\WriteText1.txt";
            //AttrFileName = @"C:\test\AttrFileText.txt";

            Console.WriteLine("<!VPSX-MsgType>DEBUG");
            Console.WriteLine("Converting Schedule Label TXT to ZPL");

            Console.WriteLine("<!VPSX-MsgType>INFO");
            Console.WriteLine("The Input Filename is {0}", InputFileName);
            Console.WriteLine("<!VPSX-MsgType>INFO");
            Console.WriteLine("The Output Filename is {0} ", OutputFileName);

            // Create a file to write the processed results to.
            // In this case Data will be formatted into a ZPL Label

            using (StreamWriter sw = File.CreateText(OutputFileName))
            {
                sw.WriteLine(term);

                Console.WriteLine("<!VPSX-MsgType>DEBUG");
                Console.WriteLine("Create Output File");
            }

            // Need to create temp file to work from 
            TempFile = string.Format(@"D:\temp\{0}.TXT", Guid.NewGuid()); // create temp file name using GUID
            File.Copy(InputFileName, TempFile);          // Copy InputFile to Temp File

            //VPSX expects all filters to create an altered output file.  

            // Now start Processing the File contents  
            System.IO.StreamReader file =
                new System.IO.StreamReader(TempFile);

            Console.WriteLine("<!VPSX-MsgType>DEBUG");
            Console.WriteLine("Open File for Read");

            while ((line = file.ReadLine()) != null)
            {
                char[] charArr = line.ToCharArray();

                if (line == "")
                {
                    int a;// System.Console.WriteLine(" BLANK Line ");

                    counter++;
                    Console.WriteLine("<!VPSX-MsgType>DEBUG");
                    Console.WriteLine("BLANK Count ++");
                }
                else
                {
                    if (line.Length < 2)
                    {
                        // Skip but count
                        counter++;
                        Console.WriteLine("<!VPSX-MsgType>DEBUG");
                        Console.WriteLine("Short Line Count ++");
                    }
                    else
                    {
                        //System.Console.WriteLine(line);
                        LabelLine[counter] = line;  // will cause error for wrong data  ie laser job
                        counter++;

                        Console.WriteLine("<!VPSX-MsgType>DEBUG");
                        Console.WriteLine("Line of Data counter ++");

                    }

                    if (counter >= 7) //7
                    {//ERROR
                        Console.WriteLine("<!VPSX-MsgType>ERROR");
                        Console.WriteLine("More than 6 lines with no FF");
                        break;
                    }

                }


                foreach (char ch in charArr)
                {
                    // if (ch == 0x00)
                    {
                        // Console.WriteLine("<!VPSX-MsgType>DEBUG");
                        // Console.WriteLine(" NULL ");
                    }


                    // if (ch == 0x0A)
                    {
                        // Console.WriteLine("<!VPSX-MsgType>DEBUG");
                        // Console.WriteLine(" LF ");
                    }


                    if ((ch == 0x0C) || (counter == 6)) // Form Feed found  6
                    {

                        Console.WriteLine("<!VPSX-MsgType>DEBUG");
                        Console.WriteLine("6 lines Found Write One ZPL Label to Output File");


                        // System.Console.WriteLine(" Form Feed ");
                        // NewLabel = 1;  // FF end of label 
                        //counter = 0;


                        // Data checks

                        // Start Build ZPL Code for Schedule Label from data


                        // WriteLabel();

                        ZPLString =
                        "^XA" + term +
                        "^LH10,0" + term +
                        "^FT20,38" + term +
                        "^A0N,25,34^FD" + LabelLine[0] + "^FS" + term +
                        "^FT20,68" +
                        "^A0N,25,34^FD" + LabelLine[1] + "^FS" + term +
                       "^FT20,98" +
                        "^A0N,25,34^FD" + LabelLine[2] + "^FS" + term +
                        "^FT20,128" +
                        "^A0N,25,34^FD" + LabelLine[3] + "^FS" + term +
                        "^FT20,158" +
                        "^A0N,25,34^FD" + LabelLine[4] + "^FS" + term +
                        "^FT20,188" +
                        "^A0N,25,34^FD" + LabelLine[5] + "^FS" + term +
                        "^FT20,218" +
                        "^A0N,25,34^FD" + LabelLine[6] + "^FS" + term +
                        "^FT20,248" +
                        "^A0N,25,34^FD" + LabelLine[7] + "^FS" + term +
                        "^FT20,278" +
                        "^A0N,25,34^FD" + LabelLine[8] + "^FS" + term +
                        "^FT20,308" +
                        "^A0N,25,34^FD" + LabelLine[9] + "^FS" + term +
                        "^FT20,338" +
                        "^A0N,25,34^FD" + LabelLine[10] + "^FS" + term +
                        "^FT20,368" +                                       // Added for data error in some formats extra line
                        "^A0N,25,34^FD" + LabelLine[11] + "^FS" + term +    // extra line
                        "^PQ1" +
                        "^XZ" + term + term;
                        // End Schedule Label


                        using (StreamWriter sw = File.AppendText(OutputFileName))
                        {
                            sw.WriteLine(ZPLString);

                        }

                        // zero out label space

                        for (int i = 0; i <= 13; i++)
                        {
                            LabelLine[i] = "";
                        }
                    }

                    // if (ch == 0x0D)
                    //     System.Console.WriteLine(" CR ");

                }



            }

            // close the file?
            file.Close();

            File.Delete(TempFile);

            Console.WriteLine("<!VPSX-MsgType>DEBUG");
            Console.WriteLine("Done Close File and Delete:");

            Console.WriteLine("<!VPSX-MsgType>DEBUG");
            Console.WriteLine(TempFile);

            // System.Console.WriteLine("There were {0} lines.", counter);
            // Suspend the screen.  






            //(StreamWriter w = File.AppendText("log.txt"))

            //VPSX also has Filter Feedback commands which can affect the behavior
            //of VPSX.  I will use those here

            // Console.WriteLine("<!VPSX-DoNotPrint>");  //We don't really want anything printed here
            //Console.WriteLine("<!VPSX-NoOutputFile>"); //Tell VPSX that there is no output file

            return 0;



        }
        /*
        public static void WriteLabel()
        {
            int i;

            ZPLString =
            "^XA" +
            "^LH10,0" +
            "^FT20,38" +
            "^A0N,25,34^FD" + Program.LabelLine[0] + "^FS" +
            "^FT20,68" +
            "^A0N,25,34^FD" + LabelLine[1] + "^FS" +
            "^FT20,98" +
            "^A0N,25,34^FD" + LabelLine[2] + "^FS" +
            "^FT20,128" +
            "^A0N,25,34^FD" + LabelLine[3] + "^FS" +
            "^FT20,158" +
            "^A0N,25,34^FD" + LabelLine[4] + "^FS" +
            "^FT20,188" +
            "^A0N,25,34^FD" + LabelLine[5] + "^FS" +
            "^FT20,218" +
            "^A0N,25,34^FD" + LabelLine[6] + "^FS" +
            "^FT20,248" +
            "^A0N,25,34^FD" + LabelLine[7] + "^FS" +
            "^FT20,278" +
            "^A0N,25,34^FD" + LabelLine[8] + "^FS" +
            "^FT20,308" +
            "^A0N,25,34^FD" + LabelLine[9] + "^FS" +
            "^FT20,338" +
            "^A0N,25,34^FD" + LabelLine[10] + "^FS" +
            "^PQ1" +
            "^XZ" + 0x0C;

           // using (StreamWriter w = File.AppendText(OutputFileName))
          //  {
          //      w.WriteLine(ZPLString);
         //   }

          //  System.IO.File.AppendText


        }
        */
    }



}
