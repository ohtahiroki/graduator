using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checker
{
    public class ClassDefinition
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public float Unit { get; set; }

        public override string ToString()
        {
            return $"{Id} {Unit} {Name}";
        }
    }
}
