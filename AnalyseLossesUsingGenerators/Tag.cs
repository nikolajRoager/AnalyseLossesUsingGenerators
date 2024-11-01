using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzeLossesUsingGenerators
{
    //A tag for a weapon system
    internal class Tag
    {
        public string TagName {  get; set; }

        private SortedSet<string> ModelNames { get; set; }

        public Tag(string tagName, string[] Models)
        {
            TagName = tagName;
            ModelNames = new SortedSet<string>(Models);
        }
        
        //Add a model name to this tag
        public void Add(string model)
        {
            ModelNames.Add(model);
        }

        public int getModels() { return ModelNames.Count; }
        public bool Contains(string model) { return ModelNames.Contains(model); }
    }
}
