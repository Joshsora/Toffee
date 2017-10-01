namespace Toffee.Core
{
    public enum ToffeeErrorCodes : byte
    {
        None,
        ClientHelloApplicationInvalid,
        ClientHelloApplicationOutOfDate,
        ClientHelloHashInvalid,
        ClientHelloEncryptionInvalid
    }
}
