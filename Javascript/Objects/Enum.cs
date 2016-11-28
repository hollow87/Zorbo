using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jurassic;
using Jurassic.Library;

using JScript = Javascript.Script;
using Zorbo;

namespace Javascript.Objects
{
    public class Enum : ObjectInstance
    {
        JScript script;
        System.Type enumType;

        protected override string InternalClassName {
            get { return enumType.Name; }
        }

        public override IEnumerable<PropertyNameAndValue> Properties {
            get {
                string[] names = System.Enum.GetNames(enumType);
                Array values = System.Enum.GetValues(enumType);

                for (int i = 0; i < names.Length; i++)
                    yield return new PropertyNameAndValue(names[i], new PropertyDescriptor(
                        values.GetValue(i),
                        PropertyAttributes.FullAccess));
            }
        }

        public Enum(JScript script, Type enumType)
            : base(script.Engine) {
            this.script = script;
            this.enumType = enumType;
        }

        private object ToPrimitive(object value) {
            try {
                value = Convert.ChangeType(value, typeof(int));
            }
            catch {
                value = value.ToString();
            }
            return value;
        }

        protected override object GetMissingPropertyValue(string propertyName) {
            string[] names = System.Enum.GetNames(enumType);
            Array values = System.Enum.GetValues(enumType);

            int index = names.FindIndex((s) => s == propertyName);
            if (index >= 0) return ToPrimitive(values.GetValue(index));

            return base.GetMissingPropertyValue(propertyName);
        }

        public override PropertyDescriptor GetOwnPropertyDescriptor(uint index) {

            Array values = System.Enum.GetValues(enumType);

            if (index < values.Length)
                return new PropertyDescriptor(ToPrimitive(values.GetValue(index)), PropertyAttributes.FullAccess);
            
            return new PropertyDescriptor(null, PropertyAttributes.Sealed);
        }
    }
}
