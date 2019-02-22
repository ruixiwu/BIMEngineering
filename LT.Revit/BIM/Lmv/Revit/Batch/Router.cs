using System.Xml;

namespace BIM.Lmv.Revit.Batch
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    internal class Router : IDisposable
    {
        private volatile bool _IsRunning;
        private Queue<string> _SendMessages = new Queue<string>(0x100);
        public static readonly Router Instance = new Router();

        public event Action<DateTime, string> OnMessageReceived;

        public event Action<string> OnMessageSendFailure;

        static Router()
        {
            Instance.Start();
        }

        private void Excute(object state)
        {
            int id = Process.GetCurrentProcess().Id;
            try
            {
                using (MemoryMappedFile file = state as MemoryMappedFile)
                {
                    using (Mutex mutex = Mutex.OpenExisting("RevitRouter.Mutex"))
                    {
                        using (MemoryMappedViewStream stream = file.CreateViewStream())
                        {
                            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, true))
                            {
                                using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, true))
                                {
                                    bool flag = false;
                                    while (this._IsRunning)
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
                                               // Queue<string> queue;
                                                var queue = new Queue<string>();
                                                stream.Seek(0L, SeekOrigin.Begin);
                                                byte num2 = reader.ReadByte();
                                                switch (num2)
                                                {
                                                    case 0:
                                                        flag = true;
                                                        goto Label_016B;

                                                    case 1:
                                                    {
                                                        DateTime time = new DateTime(reader.ReadInt64());
                                                        int num3 = reader.ReadInt32();
                                                        if (id != num3)
                                                        {
                                                            break;
                                                        }
                                                        string str = reader.ReadString();
                                                        if (this.OnMessageReceived != null)
                                                        {
                                                            this.OnMessageReceived(time, str);
                                                        }
                                                        goto Label_016B;
                                                    }
                                                    case 2:
                                                    {
                                                        DateTime time2 = new DateTime(reader.ReadInt64());
                                                        TimeSpan span = (TimeSpan) (DateTime.Now - time2);
                                                        if (Math.Abs(span.TotalSeconds) <= 5.0)
                                                        {
                                                            continue;
                                                        }
                                                        int num4 = reader.ReadInt32();
                                                        string str2 = reader.ReadString();
                                                        if ((this.OnMessageSendFailure != null) && (num4 == id))
                                                        {
                                                            this.OnMessageSendFailure(str2);
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
                                                    Monitor.Enter(queue = this._SendMessages, ref flag2);
                                                    stream.Seek(0L, SeekOrigin.Begin);
                                                    if (this._SendMessages.Count == 0)
                                                    {
                                                        writer.Write((byte) 0);
                                                    }
                                                    else
                                                    {
                                                        string str3 = this._SendMessages.Dequeue();
                                                        DateTime now = DateTime.Now;
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
                                                continue;
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
                this._IsRunning = false;
                lock (this._SendMessages)
                {
                    while (this._SendMessages.Count > 0)
                    {
                        string str4 = this._SendMessages.Dequeue();
                        if (this.OnMessageSendFailure != null)
                        {
                            this.OnMessageSendFailure(str4);
                        }
                    }
                }
            }
        }

        public bool IsRunning() => 
            this._IsRunning;

        public bool SendMessage(MessageObj msg)
        {
            if (!this._IsRunning)
            {
                return false;
            }
            string item = JsonConvert.SerializeObject(msg, Formatting.None);
            lock (this._SendMessages)
            {
                this._SendMessages.Enqueue(item);
                return true;
            }
        }

        public bool Start()
        {
            if (this._IsRunning)
            {
                throw new InvalidOperationException("Router is running!");
            }
            try
            {
                MemoryMappedFile state = MemoryMappedFile.OpenExisting("RevitRouter", MemoryMappedFileRights.ReadWrite);
                this._IsRunning = true;
                return ThreadPool.QueueUserWorkItem(new WaitCallback(this.Excute), state);
            }
            catch (FileNotFoundException)
            {
                this._IsRunning = false;
                return false;
            }
        }

        public void Stop()
        {
            this._IsRunning = false;
        }

        void IDisposable.Dispose()
        {
            this._IsRunning = false;
        }
    }
}

