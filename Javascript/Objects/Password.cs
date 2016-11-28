using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Security;
using System.Reflection;

using Zorbo;
using Zorbo.Interface;

using Jurassic;
using Jurassic.Library;

using JScript = Javascript.Script;

namespace Javascript.Objects
{
    public class Password : ObjectInstance
    {
        IPassword password;

        [JSProperty(Name = "level")]
        public int Level {
            get { return (int)password.Level; }
            set {
                value = (value < 0) ? 0 : value;
                value = (value > (int)AdminLevel.Host) ? (int)AdminLevel.Host : value;

                password.Level = (AdminLevel)value;
            }
        }

        [JSProperty(Name = "password")]
        public string Sha1Text {
            get { return password.Sha1Text; }
        }


        protected override string InternalClassName {
            get { return "Password"; }
        }


        public Password(JScript script, IPassword password)
            : base(script.Engine) {

            this.password = password;
            this.PopulateFunctions();
        }
    }
}
