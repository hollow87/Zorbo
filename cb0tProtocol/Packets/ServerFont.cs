using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;
using Zorbo.Packets;

namespace cb0tProtocol.Packets
{
    class ServerFont : AresPacket
    {
        byte size = 11;
        string name = "Verdana";
        ColorCode namecolor = ColorCode.None;
        ColorCode textcolor = ColorCode.None;
        string namecolor2 = string.Empty;
        string textcolor2 = string.Empty;

        public override byte Id {
            get { return (byte)AdvancedId.MSG_CHAT_SERVER_CUSTOM_FONT; }
        }

        [PacketItem(0)]
        public string Username { get; set; }

        [PacketItem(1)]
        public byte Size {
            get { return size; }
            set {
                if (value < 8)
                    size = 8;
                else if (value > 18)
                    size = 18;
                else
                    size = value;
            }
        }

        [PacketItem(2)]
        public String Name {
            get { return name; }
            set { name = value; }
        }

        [PacketItem(3, Optional = true, OptionalValue = (byte)255)]
        public ColorCode NameColor {
            get { return namecolor; }
            set { namecolor = value; }
        }

        [PacketItem(4, Optional = true, OptionalValue = (byte)255)]
        public ColorCode TextColor {
            get { return textcolor; }
            set { textcolor = value; }
        }

        [PacketItem(5, Optional = true)]
        public string NameColor2 {
            get { return namecolor2; }
            set { namecolor2 = value; }
        }

        [PacketItem(6, Optional = true)]
        public string TextColor2 {
            get { return textcolor2; }
            set { textcolor2 = value; }
        }

        public ServerFont() { }

        public ServerFont(IClient user) {
            Username = user.Name;
        }
    }
}
