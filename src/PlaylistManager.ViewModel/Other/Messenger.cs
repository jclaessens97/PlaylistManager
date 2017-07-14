using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;


namespace PlaylistManager.ViewModel.Other
{
    /// <summary>
    /// Class for communications between ViewModels
    /// Source: https://stackoverflow.com/a/23909882/5985593  && https://github.com/Yeah69/MessengerPattern/blob/master/Messenger.cs
    /// TODO: extend messenger key (second link)
    /// </summary>
    public class Messenger
    {
        protected class MessengerKey
        {
            public object Recipient { get; }
            public object Context { get; }
            public Type Type { get; }

            public MessengerKey(object _recipient, object _context, Type _type)
            {
                Recipient = _recipient;
                Context = _context;
                Type = _type;
            }

            /// <summary>
            /// Determines if the specified MessengerKey is the same to the current messengerkey
            /// </summary>
            /// <param name="_other"></param>
            /// <returns></returns>
            public bool Equals(MessengerKey _other)
            {
                if (ReferenceEquals(null, _other)) return false;
                if (ReferenceEquals(this, _other)) return true;


                return Equals(Recipient, _other.Recipient) && Equals(Context, _other.Context) && Equals(Context, _other.Type);
            }

            /// <summary>
            /// Determines if the specified MessengerKey is the same to the current messengerkey
            /// </summary>
            /// <param name="_obj"></param>
            /// <returns></returns>
            public override bool Equals(object _obj)
            {
                if (ReferenceEquals(null, _obj)) return false;
                if (ReferenceEquals(this, _obj)) return true;
                if (_obj.GetType() != GetType()) return false;

                return Equals((MessengerKey) _obj);
            }

            /// <summary>
            /// hash function
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = Recipient?.GetHashCode() ?? 0;
                    hashCode = (hashCode * 397) ^ (Context?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 397) ^ (Type?.GetHashCode() ?? 0);
                    return hashCode;
                }
            }

            public static bool operator ==(MessengerKey _left, MessengerKey _right)
            {
                return Equals(_left, _right);
            }

            public static bool operator !=(MessengerKey _left, MessengerKey _right)
            {
                return !Equals(_left, _right);
            }
        }

        #region Attributes

        private static Messenger instance;
        private static readonly object lockObject = new object();

        private static readonly ConcurrentDictionary<MessengerKey, object> dictionary = new ConcurrentDictionary<MessengerKey, object>();

        #endregion

        #region Properties

        public static Messenger Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new Messenger();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        private Messenger()
        {
        }

        #region Register methods

        /// <summary>
        /// Registers a recipient for a type of message T. 
        /// The action parameter will be executed when a corresponding message is send
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_recipient"></param>
        /// <param name="_action"></param>
        public void Register<T>(object _recipient, Action<T> _action)
        {
            Register(_recipient, _action, null);
        }

        /// <summary>
        /// Registers a recipient for a type of message T and a matching context. 
        /// The action parameter will be executed when a corresponding message is send
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_recipient"></param>
        /// <param name="_action"></param>
        /// <param name="_context"></param>
        public void Register<T>(object _recipient, Action<T> _action, object _context)
        {
            var key = new MessengerKey(_recipient, _context, typeof(T));
            dictionary.TryAdd(key, _action);
        }

        /// <summary>
        /// Unregisters a messenger recipient completely.
        /// After this method is executed, the recipient will no longer receive any messages.
        /// </summary>
        /// <param name="_recipient"></param>
        public void Unregister<T>(object _recipient)
        {
            Unregister<T>(_recipient, null);
        }

        /// <summary>
        /// Unregisters a messenger recipient with a matching context completely.
        /// After this method is executed, the recipient will no longer receive any messages
        /// </summary>
        /// <param name="_recipient"></param>
        /// <param name="_context"></param>
        public void Unregister<T>(object _recipient, object _context)
        {
            object action;
            var key = new MessengerKey(_recipient, _context, typeof(T));
            dictionary.TryRemove(key, out action);
        }

        #endregion

        #region Send methods

        /// <summary>
        /// Sends a message to registered recipients. 
        /// The message will reach all recipients that are registered for this message type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_message"></param>
        public void Send<T>(T _message)
        {
            Send(_message, null);
        }

        /// <summary>
        /// Sends a message to registered recipients. 
        /// The message will reach all recipients that are registered for this message type and matching context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_message"></param>
        /// <param name="_context"></param>
        public void Send<T>(T _message, object _context)
        {
            IEnumerable<KeyValuePair<MessengerKey, object>> result;

            if (_context == null)
            {
                result = from r in dictionary where r.Key.Context == null select r;
            }
            else
            {
                result = from r in dictionary where r.Key.Context != null && r.Key.Context.Equals(_context) select r;
            }

            foreach (var action in result.Select(x => x.Value).OfType<Action<T>>())
            {
                action(_message);
            }
        }

        #endregion
    }
}
