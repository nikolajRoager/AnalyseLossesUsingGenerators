using AnalyzeLossesUsingGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to this analysis program");
        Console.WriteLine("This program will analyze publicly available photographically documented loss data, from the Russian-Ukrainian war, from February 2022 to July 2024; the data is from the Oryx Database, for more details (and links to photographic evidence of all losses, please read \"readme.md\")");
        Console.WriteLine();

        string source;
        if (args.Length == 0)
        {
            Console.WriteLine("The data need to exist in the folder data\\ in the current executing directory, if you have stored the data elsewhere use the commandline argument \"path\\to\\data\\folder\\\"");
            source = "Data\\";
        }
        else
            source = args[0];

        //Now get the files we are going to open
        Console.WriteLine($"Loading files in folder {source}");

        //The data we will be reading from
        string Russian_losses_source = Path.Combine(source, "img_russia_losses_metadata.csv");
        string Ukrainian_losses_source = Path.Combine(source, "img_ukraine_losses_metadata.csv");

        string tag_definitions = Path.Combine(source, "definitions.txt");

        Console.WriteLine($"Loading {tag_definitions} ...");
        Taglist tags;

        try
        {
            tags = new Taglist(tag_definitions);
        }
        catch (Exception ex)
        {
            Console.WriteLine("There was a problem loading tag definition");
            Console.WriteLine(ex.Message);
            return;
        }
        

        Console.WriteLine("Done");

        Console.WriteLine("Time to set up the analysis!");
        bool execute = false;

        //Currently selected output table, and the total list
        string Outfile = "default.csv";
        Dictionary<string, OutputTable> OutputTables = new Dictionary<string, OutputTable>();
        OutputTables.Add(Outfile, new OutputTable());

        //Read commands from a file
        Queue<string> FileCommands=new Queue<string>();

        while (!execute)
        {
            //Print all table outputs
            Console.WriteLine("Outputs:");
            foreach (var  v in OutputTables)
            {
                string text = v.Key+$" {v.Value.NColumns()} columns";
                if (text.Equals(Outfile))
                    text = "Current output > " + text;
                Console.WriteLine((new string(' ',Console.WindowWidth-text.Length))+text);
            }


            string? input;

            if (FileCommands.Count() > 0)
            {
                input = FileCommands.Dequeue();
                Console.WriteLine($"Executing command from file: {input}");
            }
            else
            {
                Console.WriteLine("You have 6 commands you can write now:");
                Console.WriteLine(":quit                               : end program without printing");
                Console.WriteLine(":execute                            : starts the analysis, saves all output files, quits");
                Console.WriteLine(":load FILENAME                      : apply all commands in a text file");
                Console.WriteLine("    Hint write \":load example.txt\" to load all my examples");
                Console.WriteLine(":output Name                        : Set Output file to name");
                Console.WriteLine(":Showtags                           : Print all currently available Tags");
                Console.WriteLine("    The tags are also shown in the file readme.md with explanation");
                Console.WriteLine(":Column COUNTRY TAGS ... : NAME...  : Add Column of the list of tags to the current output file");
                Console.WriteLine("    COUTRY must be either U or R or A (for all)");
                Console.WriteLine("    The TAGS after Country is any list of tags (or specific model names) (which you can see using showtags) or see in the file readme.md");
                Console.WriteLine("    any words after colon ':' are used as column name (if there is no colon ':', the column has default name)");
                Console.WriteLine("       The Column will count any equipment which has ALL the tags in the command");
                Console.WriteLine("       If you write the word \"not\" in front of a tag, the column will exclude anything with that tag");

                Console.Write(":");
                input = Console.ReadLine();
            }

            if (input != null)
            {
                List<string> outputs = new List<string>( input.Split(' ',StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries));
                if (outputs.Count()==0)
                    Console.Clear();
                else
                    switch (outputs[0].ToLower())
                    {
                        case "load":
                            if (FileCommands.Count()>0)
                            {
                               
                                Console.WriteLine("File loading commands CAN NOT be nested");
                                break;
                            }
                            if (outputs.Count()<=1)
                            {
                                Console.Clear();
                                Console.WriteLine("filename not found");
                                break;
                            }
                            try
                            {
                                using (StreamReader Sr = new StreamReader(outputs[1]))
                                {
                                    string? line;
                                    FileCommands=new Queue<string>();
                                    while ((line = Sr.ReadLine()) != null)
                                    {
                                        if (line.Length>0)
                                            FileCommands.Enqueue(line);
                                    }
                                }
                            }
                            catch(Exception ex)
                            {
                                //If we are playing back errors from a file, we want to be able to see all previous commands
                                if (FileCommands.Count() == 0)
                                    Console.Clear();
                                Console.WriteLine("Could not read file, error:");
                                Console.WriteLine(ex.Message);
                            }
                            break;
                        case "column":
                            if (FileCommands.Count() == 0)
                                Console.Clear();
                            if (outputs.Count() < 3)
                            {
                                Console.WriteLine("Not enough arguments, need (at least) country");
                            }

                            bool ukrainian = false;
                            bool russian = false;
                            switch (outputs[1].ToLower())
                            {
                                case "u":
                                   ukrainian= true;
                                    
                                    break;
                                case "r":
                                    russian= true;
                                    break;
                                default:
                                    Console.WriteLine("WARNING, no valid country, using BOTH");
                                    russian = true;
                                    ukrainian = true;
                                    break;
                                case "a":
                                    russian = true;
                                    ukrainian = true;
                                    break;
                            }

                            string colName;
                            //First read the name of this column
                            int name_start_i = outputs.FindIndex(1,outputs.Count()-1,x => x.Equals(":"));

                            //Returned if : was not found
                            if (name_start_i<0)
                            {
                                //Use default name
                                colName = $"Column {OutputTables[Outfile].NColumns()}";
                                Console.WriteLine("Using default name " + colName);
                                outputs = outputs.Skip(2).ToList();
                            }
                            else
                            {
                                //The name is after the :
                                colName=string.Join(" ",outputs.Skip(name_start_i+1));
                                outputs = outputs.GetRange(2, name_start_i - 2);
                            }
                            OutputTables[Outfile].AddColumn(colName,ukrainian,russian);

                            for (int i = 0; i < outputs.Count(); ++i)
                            {
                                bool invert = false;
                                if (outputs[i].ToLower().Equals("not") && i < outputs.Count())
                                {
                                    invert = true;
                                    ++i;
                                }
                                //If the tag is not in the list of tag, then it is a model name, which is valid too
                                OutputTables[Outfile].AddColumnTag(outputs[i].ToLower(), invert);
                            }
                            break;
                        case "quit":
                            return;
                        case "output":
                            if (FileCommands.Count() == 0)
                                Console.Clear();
                            if (outputs.Count() < 2)
                                Console.WriteLine("Not enough arguments, need name of output table");
                            else
                            {
                                string newOutfile=string.Join("_", outputs.Skip(1));
                                newOutfile =Path.ChangeExtension(newOutfile, ".csv");
                                
                                //If we haven't saved anything there, drop the table (it already exists if we got here, so no need to check
                                if (OutputTables[Outfile].NColumns()==0)
                                    OutputTables.Remove(Outfile);

                                //If it isn't there already, add it
                                if (!OutputTables.ContainsKey(newOutfile))
                                    OutputTables.Add(newOutfile,new OutputTable());

                                Outfile = newOutfile;
                            }
                            break;
                        case "execute":
                            execute = true;

                            break;
                        case "showtags":
                            tags.debugPrint();//No clear, leave it here
                            break;
                        default:
                            if (FileCommands.Count() == 0)
                                Console.Clear();
                            Console.WriteLine("Invalid command");
                            break;
                    }
            }
        }
        //If we get here, we are asking to analyze all the data
        Console.WriteLine("Running data analysis ... this might take a few seconds");

        try
        {
            Console.WriteLine("Analyzing Ukrainian loos data");

            foreach (var row in CSVReader.ReadRows(Ukrainian_losses_source))
            {
                //Check if this table has columns containing this model
                foreach (var table in OutputTables.Values)
                {
                    //Model header must exist
                    table.Count(row["model"],tags,true);
                }
            }
            Console.WriteLine("Analyzing Russian loos data");
            foreach (var row in CSVReader.ReadRows(Russian_losses_source))
            {
                //Check if this table has columns containing this model
                foreach (var table in OutputTables.Values)
                {
                    //Model header must exist
                    table.Count(row["model"],tags,false);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("There was an error loading and analyzing the data");
            Console.WriteLine(ex.ToString());
            return;
        }

        Console.WriteLine("Done, now saving");
        foreach (var table in OutputTables)
        {
            Console.WriteLine("");
            try
            {
                table.Value.Save(table.Key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an error saving the data to {table.Key}");
                Console.WriteLine(ex.ToString());
                //We will try to save, even if one fail (though it will most likely not work)
            }
        }
        Console.WriteLine("Done, you need to plot the data using other programs, I have made some python programs to plot my examples");
    }
}