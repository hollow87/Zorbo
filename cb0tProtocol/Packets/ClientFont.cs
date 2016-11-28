using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;
using Zorbo.Packets;

namespace cb0tProtocol.Packets
{
    public class ClientFont : AresPacket
    {
        byte size = 11;
        string name = "Verdana";
        ColorCode namecolor = ColorCode.None;
        ColorCode textcolor = ColorCode.None;
        string namecolor2 = string.Empty;
        string textcolor2 = string.Empty;

        public override byte Id {
            get { return (byte)AdvancedId.MSG_CHAT_CLIENT_CUSTOM_FONT; }
        }

        [PacketItem(0)]
        public byte Size {
            get { return size; }
            set { size = value; }
        }

        [PacketItem(1)]
        public String Name {
            get { return name; }
            set { name = value; }
        }

        [PacketItem(2, Optional = true, OptionalValue = (byte)255)]
        public ColorCode NameColor {
            get { return namecolor; }
            set { namecolor = value; }
        }

        [PacketItem(3, Optional = true, OptionalValue = (byte)255)]
        public ColorCode TextColor {
            get { return textcolor; }
            set { textcolor = value; }
        }

        [PacketItem(4, Optional = true)]
        public string NameColor2 {
            get { return namecolor2; }
            set { namecolor2 = value; }
        }

        [PacketItem(5, Optional = true)]
        public string TextColor2 {
            get { return textcolor2; }
            set { textcolor2 = value; }
        }
    }
}
