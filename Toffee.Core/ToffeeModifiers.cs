using System;

namespace Toffee.Core
{
    /// <summary>
    /// A bit-field of modifiers that affects the security and behavior of various Toffee objects.
    /// </summary>
    [Flags]
    public enum ToffeeModifiers
    {
        None = 0x00,

        // Sending restrictions
        AnonSend = 0x01,
        ClientSend = 0x02,
        OwnerSend = 0x04,
        FriendSend = 0x08,

        // Culling
        Broadcast = 0x10,
        GlobalFriends = 0x20,
        LocalFriends = 0x40,

        // Storage
        Ram = 0x80,
        Db = 0x100,

        // Etc
        Required = 0x200,

        // Security
        Encrypted = 0x400
    }
}
