using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace BIM.Lmv.Revit.Batch
{
    internal class Router : IDisposable
    {
        public static readonly Router Instance = new Router();
        private volatile bool _IsRunning;
        private readonly Queue<string> _SendMessages = new Queue<string>(0x100);

        static Router()
        {
            Instance.Start();
        }

        void IDisposable.Dispose()
        {
            _IsRunning = false;
        }

        public event Action<DateTime, string> OnMessageReceived;

        public event Action<string> OnMessageSendFailure;

        private void Excute(object state)
        {
            var id = Process.GetCurrentProcess().Id;
            try
            {
                using (var file = state as MemoryMappedFile)
                {
                    using (var mutex = Mutex.OpenExisting("RevitRouter.Mutex"))
                    {
                        using (var stream = file.CreateViewStream())
                        {
                            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
                            {
                                using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
                                {
                                    var flag = false;
                                    while (_IsRunning)
                                    {
                                        if (flag)
                                        {
                                            flag = false;
                                            Thread.Sleep(100);
                                        }
                                        else
                                        {
                                            Thread.Sleep(0);
                                        }
                                        if (mutex.WaitOne(300))
                                        {
                                            try
                                            {
                                                bool flag2;
                                                var queue = new Queue<string>();
                                                stream.Seek(0L, SeekOrigin.Begin);
                                                var num2 = reader.ReadByte();
                                                switch (num2)
                                                {
                                                    case 0:
                                                        flag = true;
                                                        goto Label_016B;

                                                    case 1:
                                                    {
                                                        var time = new DateTime(reader.ReadInt64());
                                                        var num3 = reader.ReadInt32();
                                                        if (id != num3)
                                                        {
                                                            break;
                                                        }
                                                        var str = reader.ReadString();
                                                        if (OnMessageReceived != null)
                                                        {
                                                            OnMessageReceived(time, str);
                                                        }
                                                        goto Label_016B;
                                                    }
                                                    case 2:
                                                    {
                                                        var time2 = new DateTime(reader.ReadInt64());
                                                        var span = DateTime.Now - time2;
                                                        if (Math.Abs(span.TotalSeconds) <= 5.0)
                                                        {
                                                            continue;
                                                        }
                                                        var num4 = reader.ReadInt32();
                                                        var str2 = reader.ReadString();
                                                        if ((OnMessageSendFailure != null) && (num4 == id))
                                                        {
                                                            OnMessageSendFailure(str2);
                                                        }
                                                        goto Label_016B;
                                                    }
                                                    default:
                                                        throw new NotSupportedException("MessageFlag=" + num2);
                                                }
                                                flag = true;
                                                Label_016B:
                                                flag2 = false;
                                                try
                                                {
                                                    Monitor.Enter(queue = _SendMessages, ref flag2);
                                                    stream.Seek(0L, SeekOrigin.Begin);
                                                    if (_SendMessages.Count == 0)
                                                    {
                                                        writer.Write((byte) 0);
                                                    }
                                                    else
                                                    {
                                                        var str3 = _SendMessages.Dequeue();
                                                        var now = DateTime.Now;
                                                        writer.Write((byte) 2);
                                                        writer.Write(now.Ticks);
                                                        writer.Write(id);
                                                        writer.Write(str3);
                                                    }
                                                }
                                                finally
                                                {
                                                    if (flag2)
                                                    {
                                                        Monitor.Exit(queue);
                                                    }
                                                }
                                            }
                                            finally
                                            {
                                                mutex.ReleaseMutex();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                _IsRunning = false;
                lock (_SendMessages)
                {
                    while (_SendMessages.Count > 0)
                    {
                        var str4 = _SendMessages.Dequeue();
                        if (OnMessageSendFailure != null)
                        {
                            OnMessageSendFailure(str4);
                        }
                    }
                }
            }
        }

        public bool IsRunning()
        {
            return _IsRunning;
        }

        public bool SendMessage(MessageObj msg)
        {
            if (!_IsRunning)
            {
                return false;
            }
            var item = JsonConvert.SerializeObject(msg, Formatting.None);
            lock (_SendMessages)
            {
                _SendMessages.Enqueue(item);
                return true;
            }
        }

        public bool Start()
        {
            if (_IsRunning)
            {
                throw new InvalidOperationException("Router is running!");
            }
            try
            {
                var state = MemoryMappedFile.OpenExisting("RevitRouter", MemoryMappedFileRights.ReadWrite);
                _IsRunning = true;
                return ThreadPool.QueueUserWorkItem(Excute, state);
            }
            catch (FileNotFoundException)
            {
                _IsRunning = false;
                return false;
            }
        }

        public void Stop()
        {
            _IsRunning = false;
        }
    }
}