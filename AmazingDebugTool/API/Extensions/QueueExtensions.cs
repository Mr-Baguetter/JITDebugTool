using System.Collections.Generic;

namespace JITDebugTool.API.Extensions
{
    public static class QueueExtensions
    {
        public static bool TryDequeue<T>(this Queue<T> queue, out T element)
        {
            if (queue.Count > 0)
            {
                element = queue.Dequeue();
                return true;
            }

            element = default(T);
            return false;
        }
    }
}
