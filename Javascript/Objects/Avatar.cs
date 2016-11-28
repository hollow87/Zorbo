using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Zorbo.Interface;

using Jurassic;
using Jurassic.Library;

using JScript = Javascript.Script;

namespace Javascript.Objects
{
    public class Avatar : ObjectInstance, IAvatar
    {
        ArrayInstance smallbytes;
        ArrayInstance largebytes;

        [JSProperty(Name = "smallBytes")]
        public ArrayInstance SmallBytes {
            get { return smallbytes; }
        }

        [JSProperty(Name = "largeBytes")]
        public ArrayInstance LargeBytes {
            get { return largebytes; }
        }

        byte[] IAvatar.SmallBytes {
            get { return smallbytes.ToArray<byte>(); }
        }

        byte[] IAvatar.LargeBytes {
            get { return largebytes.ToArray<byte>(); }
        }

        protected override string InternalClassName {
            get { return "Avatar"; }
        }

        #region " Constructor "

        public class Constructor : ClrFunction
        {
            JScript script = null;

            public Constructor(JScript script)
                : base(script.Engine.Function.InstancePrototype, "Avatar", new Avatar(script)) {

                this.script = script;
            }

            [JSCallFunction]
            public Avatar Call(ArrayInstance smallbytes, object largebytes) {

                if (!(largebytes is ArrayInstance))
                    return new Avatar(script, this.InstancePrototype, smallbytes);

                return new Avatar(script, this.InstancePrototype, smallbytes, (ArrayInstance)largebytes);    
            }

            [JSConstructorFunction]
            public Avatar Construct(ArrayInstance smallbytes, object largebytes) {
                return Call(smallbytes, largebytes);
            }
        }

        #endregion

        internal Avatar(JScript script)
            : base(script.Engine) {

            this.PopulateFunctions();
        }

        public Avatar(JScript script, ObjectInstance prototype, ArrayInstance bytes)
            : base(script.Engine, prototype) {

            this.PopulateFunctions();

            this.smallbytes = bytes;
            this.largebytes = bytes;
        }

        public Avatar(JScript script, ObjectInstance prototype, ArrayInstance smallbytes, ArrayInstance largebytes)
            : base(script.Engine, prototype) {

            this.PopulateFunctions();

            this.smallbytes = smallbytes;
            this.largebytes = largebytes;
        }

        public Avatar(JScript script, IAvatar avatar)
            : base(script.Engine, ((ClrFunction)script.Engine.Global["Avatar"]).InstancePrototype) {

            this.PopulateFunctions();

            if (avatar != null) {
                this.smallbytes = avatar.SmallBytes.ToJSArray(script.Engine);
                this.largebytes = avatar.LargeBytes.ToJSArray(script.Engine);
            }
        }


        public bool Equals(IAvatar other) {

            if (other == null ||
                smallbytes == null ||
                other.SmallBytes == null) 
                return Object.ReferenceEquals(this, other);

            return ((IAvatar)this).SmallBytes.SequenceEqual(other.SmallBytes);
        }

        public override bool Equals(object obj) {
            return Equals(obj as Avatar);
        }


        public static bool operator ==(Avatar a, IAvatar b) {

            if (Object.ReferenceEquals(a, b))
                return true;

            if ((object)a == null || (object)b == null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Avatar a, IAvatar b) {
            return !(a == b);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
