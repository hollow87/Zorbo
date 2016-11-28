using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Zorbo;
using Zorbo.Interface;

using Jurassic;
using Jurassic.Library;

using JScript = Javascript.Script;

namespace Javascript.Objects
{
    public class Admin : ReadOnlyList
    {
        IAdmins admin;

        [JSProperty(Name = "count", IsConfigurable = true)]
        public override int Count {
            get { return admin.Count; }
        }


        public Admin(JScript script, IAdmins admin)
            : base(script) {

            this.admin = admin;
            this.PopulateFunctions();
        }

        protected override string InternalClassName {
            get { return "Admin"; }
        }


        public override IEnumerable<PropertyNameAndValue> Properties {
            get {
                int i = -1;
                foreach (var admin in this.admin) {

                    var user = script.Room.Users.Items.Find((s) => ((User)s).Id == admin.Id);
                    yield return new PropertyNameAndValue((++i).ToString(), new PropertyDescriptor(user, PropertyAttributes.FullAccess));
                }
            }
        }

        public override PropertyDescriptor GetOwnPropertyDescriptor(uint index) {

            if (index < this.admin.Count) {
                var admin = this.admin[(int)index];
                var user = script.Room.Users.Items.Find((s) => ((User)s).Id == admin.Id);

                return new PropertyDescriptor(user, PropertyAttributes.FullAccess);
            }
            return new PropertyDescriptor(null, PropertyAttributes.Sealed);
        }
    }
}
