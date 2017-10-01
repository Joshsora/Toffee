using Toffee.Logging;

namespace Toffee.Server
{
    public interface IRole
    {
        bool Configure(object config, Logger logger = null);
        void Start();
        void Tick();
        void Stop();
    }
}
