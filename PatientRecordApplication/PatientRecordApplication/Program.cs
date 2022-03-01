using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


namespace ConsoleChap17FileIOApp
{
    /// <summary>
    /// A Patient Record System that allows for the editing and recalling of patient records
    /// </summary>
    /// <Student>Elise Ambriz</Student>
    /// <Class>CIS297</Class>
    /// <Semester>Winter 2022</Semester>
    class Program
    {
        [Serializable]
        class NegativeException : Exception
        {
            public NegativeException() { }

            public NegativeException(string name)
                : base(String.Format("Can't be a negative number", name))
            {

            }
        }
        static void Main(string[] args)
        {
            FileOperations();
            DirectoryOperations();
            FileStreamOperations();
            SequentialAccessWriteOperation();
            ReadSequentialAccessOperation();
            FindPatientsBalance();
            FindPatientsID();
            //SerializableDemonstration();

        }

        //File operations                                                               b
        static void FileOperations()
        {
            string fileName;
            Write("Enter a filename >> ");
            try
            {
                fileName = ReadLine();
                if (File.Exists(fileName))
                {
                    WriteLine("File exists");
                    WriteLine("File was created " + File.GetCreationTime(fileName));
                    WriteLine("File was last written to " + File.GetLastWriteTime(fileName));
                }
                else
                {
                    throw new FileNotFoundException("File not found");
                }
            }
            catch (FileNotFoundException noFileExc)
            {
                WriteLine($"{noFileExc.Message}");
                WriteLine("File does not exist");
            }
        }

        //Directory Operations
        static void DirectoryOperations()
        {
            //Directory operations
            string directoryName;
            string[] listOfFiles;
            Write("Enter a folder >> ");
            directoryName = ReadLine();
            if (Directory.Exists(directoryName))
            {
                WriteLine("Directory exists, and it contains the following:");
                listOfFiles = Directory.GetFiles(directoryName);
                for (int x = 0; x < listOfFiles.Length; ++x)
                    WriteLine("   {0}", listOfFiles[x]);
            }
            else
            {
                WriteLine("Directory does not exist");
            }
        }

        //Using FileStream to create and write some text into it
        static void FileStreamOperations()
        {
            FileStream outFile = new
            FileStream("SomeText.txt", FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(outFile);
            Write("Enter some text >> ");
            string text = ReadLine();
            writer.WriteLine(text);
            // Error occurs if the next two statements are reversed
            writer.Close();
            outFile.Close();
        }

        //Writing data to a Sequential Access text file
        static void SequentialAccessWriteOperation()
        {
            const int END = 999;
            const string DELIM = ",";
            const string FILENAME = "PatientData.txt";
            Patient pat = new Patient();
            FileStream outFile = new FileStream(FILENAME, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(outFile);
            Write("Enter ID number or " + END + " to quit >> ");
            
            try
            {
                pat.IDNum = Convert.ToInt32(ReadLine());
            }
            //Checks whether the user input ID number was a number or not
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
            }

            while (pat.IDNum != END)
            {
                try
                {
                    Write("Enter last name >> ");
                    pat.Name = ReadLine();
                    Write("Enter balance owed >> ");
                    pat.BalanceOwed = Convert.ToDouble(ReadLine());
                    writer.WriteLine(pat.IDNum + DELIM + pat.Name + DELIM + pat.BalanceOwed);
                    Write("Enter next ID number or " + END + " to quit >> ");
                    pat.IDNum = Convert.ToInt32(ReadLine());
                    //throws exception is user input ID number is negative
                    if (pat.IDNum < 0)
                    {
                        throw new NegativeException("Negative Error");
                    }
                }
                //Checks whether the user input was formatted correctly
                catch (FormatException ex)
                {
                    Console.Write(ex.Message);
                }
                //Checks whether the user input ID number is negative or positive
                catch (NegativeException ex)
                {
                    Console.Write(ex.Message);
                }
            }
            writer.Close();
            outFile.Close();
        }

        //Read data from a Sequential Access File                                               b
        static void ReadSequentialAccessOperation()
        {
            const char DELIM = ',';
            const string FILENAME = "PatientData.txt";
            Patient pat = new Patient();
            FileStream inFile = new FileStream(FILENAME, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(inFile);
            string recordIn;
            string[] fields;
            WriteLine("\n{0,-5}{1,-12}{2,8}\n", "Num", "Name", "Balance Owed");
            recordIn = reader.ReadLine();
            while (recordIn != null)
            {
                    fields = recordIn.Split(DELIM);
                    pat.IDNum = Convert.ToInt32(fields[0]);
                    pat.Name = fields[1];
                    pat.BalanceOwed = Convert.ToDouble(fields[2]);
                    WriteLine("{0,-5}{1,-12}{2,8}", pat.IDNum, pat.Name, pat.BalanceOwed.ToString("C"));
                    recordIn = reader.ReadLine();
            }
            reader.Close();
            inFile.Close();
        }

        //repeatedly searches a file to produce                                         d
        //lists of patients who meet a minimum balance owed requirement
        static void FindPatientsBalance()
        {
            const char DELIM = ',';
            const int END = 999;
            const string FILENAME = "PatientData.txt";
            Patient pat = new Patient();
            FileStream inFile = new FileStream(FILENAME, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(inFile);
            string recordIn;
            string[] fields;
            double minBalanceOwed;
            Write("Enter minimum balance owed to find or " + END + " to quit >> ");
            minBalanceOwed = Convert.ToDouble(Console.ReadLine());
            while (minBalanceOwed != END)
            {
                WriteLine("\n{0,-5}{1,-12}{2,8}\n",
                   "Num", "Name", "Balance Owed");
                inFile.Seek(0, SeekOrigin.Begin);
                recordIn = reader.ReadLine();
                while (recordIn != null)
                {
                    fields = recordIn.Split(DELIM);
                    pat.IDNum = Convert.ToInt32(fields[0]);
                    pat.Name = fields[1];
                    pat.BalanceOwed = Convert.ToDouble(fields[2]);
                    if (pat.BalanceOwed >= minBalanceOwed)
                        WriteLine("{0,-5}{1,-12}{2,8}", pat.IDNum, pat.Name, pat.BalanceOwed.ToString("C"));
                    recordIn = reader.ReadLine();
                }
                Write("\nEnter minimum balance owed to find or " + END + " to quit >> ");
                minBalanceOwed = Convert.ToDouble(Console.ReadLine());
            }
            reader.Close();  // Error occurs if
            inFile.Close(); //these two statements are reversed
        }

        
        static void FindPatientsID()
        {
            const char DELIM = ',';
            const int END = 999;
            const string FILENAME = "PatientData.txt";
            Patient pat = new Patient();
            FileStream inFile = new FileStream(FILENAME, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(inFile);
            string recordIn;
            string[] fields;
            double ID;
            Write("Enter ID number to find or " + END + " to quit >> ");
            ID = Convert.ToDouble(Console.ReadLine());
            while (ID != END)
            {
                WriteLine("\n{0,-5}{1,-12}{2,8}\n",
                   "Num", "Name", "Balance Owed");
                inFile.Seek(0, SeekOrigin.Begin);
                recordIn = reader.ReadLine();
                while (recordIn != null)
                {
                    fields = recordIn.Split(DELIM);
                    pat.IDNum = Convert.ToInt32(fields[0]);
                    pat.Name = fields[1];
                    pat.BalanceOwed = Convert.ToDouble(fields[2]);
                    if (pat.IDNum == ID)
                    {
                        WriteLine("{0,-5}{1,-12}{2,8}", pat.IDNum, pat.Name, pat.BalanceOwed.ToString("C"));
                    }
                    recordIn = reader.ReadLine();
                }
                Write("\nEnter ID number to find or " + END + " to quit >> ");
                ID = Convert.ToDouble(Console.ReadLine());
            }
            reader.Close();  // Error occurs if
            inFile.Close(); //these two statements are reversed
        }


        /*
        //Serializable Demonstration
        /// <summary>
        /// writes Person class objects to a file and later reads them                      a (enter & save to file) and 
        /// from the file into the program
        /// </summary>
        static void SerializableDemonstration()
        {
            const int END = 999;
            const string FILENAME = "Data.ser";
            Person pat = new Person();
            FileStream outFile = new FileStream(FILENAME, FileMode.Create, FileAccess.Write);
            BinaryFormatter bFormatter = new BinaryFormatter();
            Write("Enter ID number or " + END + " to quit >> ");
            pat.IDNum = Convert.ToInt32(ReadLine());
            while (pat.IDNum != END)
            {
                Write("Enter last name >> ");
                pat.Name = ReadLine();
                Write("Enter balance owed >> ");
                pat.BalanceOwed = Convert.ToDouble(ReadLine());
                bFormatter.Serialize(outFile, pat);                         //Exception Handle here. Mad that you can imput employees after checking min salary
                Write("Enter ID number or " + END + " to quit >> ");
                pat.IDNum = Convert.ToInt32(ReadLine());
            }
            outFile.Close();
            FileStream inFile = new FileStream(FILENAME, FileMode.Open, FileAccess.Read);
            WriteLine("\n{0,-5}{1,-12}{2,8}\n", "Num", "Name", "Balance Owed");
            while (inFile.Position < inFile.Length)
            {
                pat = (Person)bFormatter.Deserialize(inFile);
                WriteLine("{0,-5}{1,-12}{2,8}", pat.IDNum, pat.Name, pat.BalanceOwed.ToString("C"));
            }
            inFile.Close();
        }
        */

    }

    class Patient
    {
        public int IDNum { get; set; }
        public string Name { get; set; }
        public double BalanceOwed { get; set; }


    }



    class Person
    {
        public int IDNum { get; set; }
        public string Name { get; set; }
        public double BalanceOwed { get; set; }
    }

}



/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientRecordApplication
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
*/