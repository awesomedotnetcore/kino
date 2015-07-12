using System.Collections.Generic;
using System.Linq;

namespace rawf.Actors
{
    //TODO: Add TTL for registrations, so that never consumed handlers are not staying forever
    internal class MessageHandlerStack
    {
        private readonly IDictionary<MessageHandlerIdentifier, HashSet<SocketIdentifier>> storage;

        internal MessageHandlerStack()
        {
            storage = new Dictionary<MessageHandlerIdentifier, HashSet<SocketIdentifier>>();
        }

        internal void Push(MessageHandlerIdentifier messageHandlerIdentifier, SocketIdentifier socketIdentifier)
        {
            HashSet<SocketIdentifier> collection;
            if (!storage.TryGetValue(messageHandlerIdentifier, out collection))
            {
                collection = new HashSet<SocketIdentifier>();
                storage[messageHandlerIdentifier] = collection;
            }
            Push(collection, socketIdentifier);
        }


        internal SocketIdentifier Pop(MessageHandlerIdentifier messageHandlerIdentifier)
        {
            //TODO: Implement round robin
            HashSet<SocketIdentifier> collection;
            return storage.TryGetValue(messageHandlerIdentifier, out collection)
                       ? Get(collection)
                       //? (IdentifyByMessage(messageHandlerIdentifier)) ? Pop(collection) : Get(collection)
                       : null;
        }

        private bool IdentifyByMessage(MessageHandlerIdentifier callbackIdentifier)
        {
            return callbackIdentifier is ActorIdentifier;
        }

        private static void Push<T>(ISet<T> hashSet, T element)
        {
            hashSet.Add(element);
        }

        private static T Pop<T>(ICollection<T> hashSet)
        {
            if (hashSet.Any())
            {
                var el = hashSet.First();
                hashSet.Remove(el);

                return el;
            }

            return default(T);
        }

        private static T Get<T>(ICollection<T> hashSet)
        {
            return hashSet.Any()
                       ? hashSet.First()
                       : default(T);
        }
    }
}