using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeLossesUsingGenerators
{
    //The entire list of all tags
    internal class Taglist
    {
        public SortedDictionary<string,Tag> tagSet {get; set;}
        
        //For debug purposes, if we want to print all the tags, we need to store the longest number of chars in the number of elements in the tag
        private int maxTagCountSize=0;

        //File errors fall through
        public Taglist(string tag_definitions)
        {
            tagSet = new SortedDictionary<string, Tag>();

            //I will not go out of my way to use generators to load definitions.txt, as it is only ~800 models
            using (var reader = new StreamReader(tag_definitions))
            {

                //The definition is in the format: modelname tag tag tag tag ...
                //We want all the tags with the associated modelnames in a list
                for (string? list; (list = reader.ReadLine()) != null;)
                {
                    string[] words = list.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                    //Throw a warning if I messed up when manually assigning tags
                    if (words.Length == 1)
                        Console.WriteLine($"WARNING {tag_definitions} contained a name {words[0]} with no tags");
                    else if (words.Length >= 2)
                    {
                        //Loop through all tags of this name, and add this name to each tag, if the tag doesn't exist, add the tag
                        for (int i = 1; i < words.Length; i++)
                        {
                            if (tagSet.ContainsKey(words[i].ToLower()))
                                tagSet[words[i].ToLower()].Add(words[0]);
                            else
                                tagSet.Add(words[i].ToLower(), new Tag(words[i].ToLower(), [words[0]]));


                        }
                    }
                }
            }
            maxTagCountSize = 0;
            foreach (var tag in tagSet.Values)
                maxTagCountSize = Math.Max(maxTagCountSize, (int)Math.Log10(tag.getModels()));
        }
        
        public bool Contains(string tag)
        {
            return tagSet.ContainsKey(tag);
        }

        public bool ModelIs(string model,string tag)
        {
            return tagSet.ContainsKey(tag) && tagSet[tag].Contains(model);
        }

        public void debugPrint()
        {
            foreach (var tag in tagSet.Values)
                Console.WriteLine($"* There are {(new string(' ',maxTagCountSize-(int)Math.Log10(tag.getModels())))+tag.getModels()} unique models with the tag {tag.TagName}");
        }
    }
}
