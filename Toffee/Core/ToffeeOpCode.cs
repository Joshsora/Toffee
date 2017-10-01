namespace Toffee.Core
{
    /// <summary>
    /// An enum of possible packet OpCodes.
    /// </summary>
    public enum ToffeeOpCode : ushort
    {
        #region Client Packets (1-1000)
        /// <summary>
        /// The first packet sent from the client to a client agent.
        /// <see cref="Packets.ClientHello"/>
        /// </summary>
        ClientHello = 1,

        /// <summary>
        /// The first packet sent from a client agent to a new client.
        /// <see cref="Packets.ClientHelloResponse"/>
        /// </summary>
        ClientHelloResponse = 2,

        /// <summary>
        /// Can either be sent from the client to a client agent to end your connection cleanly,
        /// or it can be sent from the client agent to the client to kick your connection.
        /// <see cref="Packets.ClientDisconnect"/>
        /// </summary>
        ClientDisconnect = 3,

        /// <summary>
        /// Sent from the client to the client agent every X seconds so that unexpected disconnections can be detected.
        /// </summary>
        ClientHeartbeat = 4,

        /// <summary>
        /// Can either be sent from the client to the server to update fields,
        /// or it can be sent from the server to tell a client that a field has been updated.
        /// <see cref="Packets.ObjectUpdateFields"/>
        /// </summary>
        ClientFieldUpdate = 5,
        #endregion
        #region Message Director Packets (1001-2000)
        /// <summary>
        /// The first packet sent from an internal participant to a message director.
        /// <see cref="Packets.InternalHello"/>
        /// </summary>
        InternalHello = 1001
        #endregion
    }
}
