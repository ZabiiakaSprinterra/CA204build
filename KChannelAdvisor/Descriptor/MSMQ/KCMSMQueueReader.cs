using KChannelAdvisor.Descriptor.MSMQ.Models;
using Newtonsoft.Json;
using PX.PushNotifications.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Messaging;

namespace KChannelAdvisor.Descriptor.MSMQ
{
    public class KCMSMQueueReader : IDisposable
    {
        private volatile bool _disposed;
        public string Address { get; }
        public bool IsQueueExists => MessageQueue.Exists(Address);
        public bool IsQueueAvailable => msQueue != null && msQueue.CanRead && msQueue.CanWrite;

        private MessageQueue msQueue;

        public KCMSMQueueReader(string address)
        {
            this.Address = address;

            if (IsQueueExists)
            {
                msQueue = new MessageQueue(Address)
                {
                    Formatter = new StringMessageFormatter(),
                    DefaultPropertiesToSend = new DefaultPropertiesToSend
                    {
                        Recoverable = true,
                        UseDeadLetterQueue = true,
                        TimeToReachQueue = TimeSpan.FromMinutes(1.0)
                    }
                };
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void DeleteQueue()
        {
            MessageQueue.Delete(Address);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            msQueue?.Dispose();

            _disposed = true;
        }

        public void CreateQueue()
        {
            msQueue = MessageQueue.Create(Address);
            msQueue.Formatter = new StringMessageFormatter();
            msQueue.DefaultPropertiesToSend = new DefaultPropertiesToSend
            {
                Recoverable = true,
                UseDeadLetterQueue = true,
                TimeToReachQueue = TimeSpan.FromMinutes(1.0)
            };
            msQueue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
        }

        public List<KCMSMQMessage> PeekAllMessages()
        {
            if (msQueue == null)
            {
                throw new InvalidOperationException(KCMSMQConstants.MSMQNotInitialized);
            }

            int count = GetMessagesCount();
            List<KCMSMQMessage> availableMsgs = new List<KCMSMQMessage>(count);
            if (count > 0)
            {
                using (Cursor cursor = msQueue.CreateCursor())
                {
                    Message msg = msQueue.Peek(TimeSpan.FromSeconds(3), cursor, PeekAction.Current);
                    KCMSMQMessage result;

                    using (var reader = new StreamReader(msg.BodyStream))
                    {
                        var json = reader.ReadToEnd();
                        result = JsonConvert.DeserializeObject<KCMSMQMessage>(json);
                        result.Id = msg.Id;
                    }

                    availableMsgs.Add(result);
                    count--;

                    while (count > 0)
                    {
                        msg = msQueue.Peek(TimeSpan.FromSeconds(3), cursor, PeekAction.Next);
                        using (var reader = new StreamReader(msg.BodyStream))
                        {
                            var json = reader.ReadToEnd();
                            result = JsonConvert.DeserializeObject<KCMSMQMessage>(json);
                            result.Id = msg.Id;
                        }

                        availableMsgs.Add(result);
                        count--;
                    }
                }
            }

            return availableMsgs;
        }

        public bool TryReceiveMessage(string msgID, out KCMSMQMessage message)
        {
            if (msQueue == null)
            {
                throw new InvalidOperationException(KCMSMQConstants.MSMQNotInitialized);
            }

            bool result = false;
            message = null;
            //try
            //{
            Message msg = msQueue.ReceiveById(msgID);
            if (msg != null)
            {
                using (var reader = new StreamReader(msg.BodyStream))
                {
                    var json = reader.ReadToEnd();
                    message = JsonConvert.DeserializeObject<KCMSMQMessage>(json);
                    message.Id = msg.Id;

                    result = true;
                }
            }

            return result;
            //}
            //catch (InvalidOperationException ex)
            //{
            //    return null;
            //}
        }

        public void CleanEmptyMessages()
        {
            if (msQueue == null)
            {
                throw new InvalidOperationException(KCMSMQConstants.MSMQNotInitialized);
            }

            List<KCMSMQMessage> msges = PeekAllMessages();
            foreach (KCMSMQMessage msg in msges)
            {
                if (msg.Inserted.Count == 0)
                {
                    msQueue.ReceiveById(msg.Id);
                }
            }
        }

        public int GetMessagesCount()
        {
            return GetMessagesCount(msQueue);
        }

        public static int GetMessagesCount(MessageQueue msQueue)
        {
            if (msQueue == null)
            {
                throw new InvalidOperationException(KCMSMQConstants.MSMQNotInitialized);
            }

            int count = 0;
            using (var enumerator = msQueue.GetMessageEnumerator2())
            {
                while (enumerator.MoveNext())
                {
                    count++;
                }
            }

            return count;
        }

    }
}
