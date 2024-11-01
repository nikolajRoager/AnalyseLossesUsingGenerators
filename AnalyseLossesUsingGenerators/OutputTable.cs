using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeLossesUsingGenerators
{
    internal class OutputTable
    {
        private List<Column> columns;

        private int Total;
        public OutputTable()
        {
            columns = new List<Column>();
            Total = 0;
        }

        public void AddColumn(string name, bool ukrainian, bool russian)
        {
            columns.Add(new Column(name,ukrainian,russian));
        }

        public void AddColumnTag(string tag,bool inverse)
        {

            columns[columns.Count() - 1].Tags[tag]=inverse;
        }

        public void Count (string model,Taglist tags, bool ukrainian=false/*In this case, anything which is not identified as Ukrainian is Russian*/)
        {
            bool added_to_any = false;
            foreach (Column c in columns)
            {
                //For every column, check if the nation matches
                if (c.Ukrainian && ukrainian ||c.Russian && !ukrainian)
                {
                    //Using bitwise | supresses short circuit logic, this means we evaluate the column c, even if added_to_any is always true
                    added_to_any |= c.Add(model, tags)  ;
                }
            }

            //We count how many pieces of equipment are in any column, so if the same thing shows up both in the fighter and Cas common, we count it once
            if (added_to_any)
                ++Total;
        }
        
        public int NColumns () { return columns.Count(); }
        
        //Exceptions fall through if something goes wrong
        public void Save(string output)
        {
            //Both save to the file AND print to the console

            //We are going to print in the format:
            /*
             * column 0, column 1, column 2, total
             * 2       , 4       , 3       , 4
             */
            //We will try to keep the lines the same length
            string header_line="";
            string count_line ="";
            
            foreach (Column c in columns)
            {
                header_line += c.Name;
                count_line += $"{c.Count}";
                if (header_line.Length>count_line.Length)
                    count_line += new string(' ',header_line.Length-count_line.Length);
                else
                    header_line += new string(' ',count_line.Length-header_line.Length);
                //Separate with commas
                header_line += ",";
                count_line  += ",";

            }
            header_line += "Total";
            count_line += $"{Total}";
            
            Console.WriteLine("Saving the following to "+output+":");
            Console.WriteLine(header_line);
            Console.WriteLine(count_line);
            
            //Let errors fall through
            using (StreamWriter sw = new StreamWriter(output,false/*Do not append (the file may exist from previous runs, overwrite it)*/))
            {
                sw.WriteLine(header_line + "\n"+count_line+"\n");
            }
        }
    }
}
