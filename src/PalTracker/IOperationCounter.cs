using System.Collections.Generic;

namespace PalTracker
{
    public enum TrackedOperation
    {
        Create,
        Read,
        List,
        Update,
        Delete
    }

    public interface IOperationCounter<T>
    {
        void Increment(TrackedOperation operation);

        IDictionary<TrackedOperation, int> GetCounts();

        string Name { get; }
    }
}