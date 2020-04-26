using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Checker
{
    [Serializable]
    [DataContract]
    public abstract class ClassChecker
    {
        [DataMember]
        public string TakeCondition { get; set; }
        [DataMember]
        public float RequiredAmount { get; set; }
        [DataMember]
        public string Name { get; set; }
        public ClassChecker(string takeCondition, float requiredAmount, string name)
        {
            this.TakeCondition = takeCondition;
            this.RequiredAmount = requiredAmount;
            this.Name = name;
        }

        public abstract bool IsFulfiled();

        public abstract bool AddClass(ClassDefinition cDef);

        public abstract void Clear();

        public abstract float GetUnitAmount();
    }
}
