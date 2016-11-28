using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zorbo.Interface
{
    [Flags]
    public enum ClientFeatures : byte
    {
        NONE = 0,
        VOICE = 1,
        PRIVATE_VOICE = 2,
        OPUS_VOICE = 4,
        PRIVATE_OPUS_VOICE = 8,
        HTML = 16,
    }

    [Flags]
    public enum ServerFeatures : byte
    {
        NONE = 0,
        PRIVATE = 1,
        SHARING = 2,
        COMPRESSION = 4,
        VOICE = 8,
        OPUS_VOICE = 16,
        ROOM_SCRIBBLES = 32,
        PRIVATE_SCRIBBLES = 64,
        HTML = 128
    }
}
