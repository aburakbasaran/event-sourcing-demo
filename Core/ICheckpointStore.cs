using System.Threading.Tasks;

namespace Core
{
    public interface ICheckpointStore
    {
        T GetLastCheckpoint<T>(string projection);

        void SetCheckpoint<T>(T checkpoint, string projection);
    }
}