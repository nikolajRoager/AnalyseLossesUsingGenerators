using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeLossesUsingGenerators
{
    internal static class CSVReader
    {
        //Read all rows in this CSV file, and return them as pairs of headers and values (as a string)
        public static IEnumerable<Dictionary<string,string>> ReadRows(string inputFile)
        {
            //We are going to trim any excess spaces, i.e. ,  aircraft     , means aircraft, we are not going to remove empty entries!
            var options = StringSplitOptions.TrimEntries;
            //If an error is thrown (file not found, etc.) it simply falls through
            using (var reader = new StreamReader(inputFile))
            {
                //First get the header
                var headerList = reader.ReadLine()?.Split(',',options);//The ? means I only call this if it is not null, otherwise the headerList is null as well
                if (headerList == null || headerList.Length == 0)
                    throw new ArgumentException($"{inputFile} did not contain a header");

                //Loop through all lines, until the file is empty
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    var rowCols = line?.Split(',',options);

                    //Check that the columns in this row are decent
                    if (rowCols != null)
                    {
                        if (rowCols.Length != headerList.Length)
                            throw new ArgumentException($"The file {inputFile} had incosistent number of rows and columns, header has {headerList.Length} columns, but the follinw line does not match:\n{line}");
                        
                        var Out = new Dictionary<string, string>();

                        //Loop though all the columns in the header, and add them to the dictionary
                        for (int i = 0; i < headerList.Length; ++i)
                            Out.Add(headerList[i], rowCols[i]);

                        yield return Out;
                    }
                }
            }
        }
    }
}
