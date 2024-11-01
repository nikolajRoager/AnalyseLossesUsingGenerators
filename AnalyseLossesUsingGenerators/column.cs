using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeLossesUsingGenerators
{
    //A column with a count of how much equipment
    internal class Column
    {
        public string Name { get; set; }
        public int Count {get; set; }

        public Dictionary<string, bool> Tags;//Tags, and whether or not they are inversed

        public bool Ukrainian;
        public bool Russian;

        public Column(string name, bool ukrainian, bool russian)
        {
            Tags = new Dictionary<string, bool>();
            Name = name;
            Count = 0;
            Ukrainian=ukrainian;
            Russian  =russian;
        }
        
        //Count this thing, if it matches, return true if it matched
        public bool Add(string model, Taglist tags)
        {
            //This model should be added if it fulfills ALL tags
            bool doAdd = true;

            foreach (var t in Tags)
            {
                //It is ok, if this model IS literally equal to one of of our Tags (which we should not invert)
                if (t.Key.Equals(model) && Tags[model] == false)
                    continue;
                //But if this model IS literally equal to one of of our Tags which we should invert, then DON'T COUNT
                else if (t.Key.Equals(model) && Tags[model] == true)
                {
                    doAdd = false;
                    break;
                }
                //Alternatively, if this current tag is in the global list of tags
                else if (tags.ModelIs(model,t.Key) && t.Value==false)
                {
                    continue;
                }
                //Or it it explicitly was not found, and we didn't want to find it
                else if (!tags.ModelIs(model,t.Key) && t.Value==true)
                {
                    continue;
                }
                else
                {
                    //This didn't work
                    doAdd = false;
                    break;

                }
            }

            if (doAdd)
            {
                ++Count;
                //Console.WriteLine(model); uncomment this line for debug printing of all models included (makes the program slow)

            }
            return doAdd;
        }
    }
}
