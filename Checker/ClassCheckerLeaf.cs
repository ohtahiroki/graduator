using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace Checker
{
    [Serializable]
    [DataContract]
    public class ClassCheckerLeaf : ClassChecker
    {
        [NonSerialized]
        private List<ClassDefinition> classes;

        public ClassCheckerLeaf(string takeCondition, float requiredAmount, string name) : base(takeCondition, requiredAmount, name)
        {
            classes = new List<ClassDefinition>();
        }

        [DataMember]
        public List<ClassDefinition> Classes { get => classes; private set => classes = value; }

        override public bool IsFulfiled()
        {
            return GetUnitAmount() >= RequiredAmount;
        }

        public override bool AddClass(ClassDefinition cDef)
        {
            if (Regex.IsMatch(cDef.Id, TakeCondition) && RequiredAmount > GetUnitAmount())
            {
                Classes.Add(cDef);
                return true;
            }
            return false;
        }

        public override float GetUnitAmount()
        {
            return Classes.Sum(a => a.Unit);
        }

        public override void Clear()
        {
            Classes.Clear();
        }

        public override string ToString()
        {
            return TakeCondition + " " + GetUnitAmount() + "/" + RequiredAmount.ToString() + " " + Name;
        }
    }
}
