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
    public class ClassCheckerNode : ClassChecker
    {
        public ClassCheckerNode() : this("", 0.0f, "")
        {

        }
    
        public ClassCheckerNode(string takeCondition, float requiredAmount, string name) : base(takeCondition, requiredAmount, name)
        {
            this.Children = new List<ClassChecker>();
        }

        [DataMember]
        public List<ClassChecker> Children { get; private set; }

        public override bool AddClass(ClassDefinition cDef)
        {
            if (Regex.IsMatch(cDef.Id, TakeCondition) && GetUnitAmount() < RequiredAmount)
            {
                foreach (var item in Children)
                {
                    if (item.AddClass(cDef)) return true;
                }
            }
            return false;
        }

        public override void Clear()
        {
            foreach (var item in Children)
            {
                item.Clear();
            }
        }

        public override float GetUnitAmount()
        {
            float total = 0;
            foreach (var item in Children)
            {
                total += item.GetUnitAmount();
            }
            return total;
        }

        public override bool IsFulfiled()
        {
            foreach (var item in Children)
            {
                if (!item.IsFulfiled()) return false;
            }
            return true;
        }

        public override string ToString()
        {
            return TakeCondition + " " + GetUnitAmount() + "/" + RequiredAmount.ToString() + " " + Name;
        }
    }
}
