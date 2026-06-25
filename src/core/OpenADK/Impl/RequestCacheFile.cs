//
// Copyright (c)1998-2011 Pearson Education, Inc. or its affiliate(s). 
// All rights reserved.
//

using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using OpenADK.Library.Infra;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace OpenADK.Library.Impl
{
    /// <summary>  A RequestCache implementation that stores SIF_Request information to a file
    /// in the agent work directory.
    /// 
    /// </summary>
    /// <author>  Eric Petersen
    /// </author>
    /// <version>  1.0
    /// </version>
    internal class RequestCacheFile : RequestCache
    {
        private static readonly AdkSerializerOptions s_messagePackOptions = new();

        private Hashtable fCache = new Hashtable();
        private FileStream fFile;

        /// <summary>
        /// The Entry class is serialized to the Request cache file
        /// </summary>
        /// <remarks>
        /// The format of the RandomAccess file is arranged as follows:
        /// <code>
        /// 0x00 A single byte with the values:
        ///         '1' indicating an active record
        ///         '0' indicating a deleted record
        /// 0x01 8 bytes indicating the length of the serialized Entry as
        ///      an unsigned long
        /// 0x09 The start of the serialized entry object.  
        /// </code>
        /// </remarks>
        protected internal override void Initialize(Agent agent)
        {
            Initialize(agent, false);
        }

        /// <summary>  Initialize the RequestCache</summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Initialize(Agent agent,
                                 bool isRetry)
        {
            //  Ensure the requests.adk file exists in the work directory
            String fileName = agent.WorkDir + Path.DirectorySeparatorChar + "requests.adk";
            FileInfo currentCacheFile = new FileInfo(fileName);
            if (!currentCacheFile.Exists)
            {
                if ((Adk.Debug & AdkDebugFlags.Lifecycle) != 0)
                {
                    Agent.Log.Debug("Creating SIF_Request ID cache: " + fileName);
                }
            }

            try
            {
                fFile = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read);
            }
            catch (Exception ioe)
            {
                throw new AdkException
                    ("Error opening or creating SIF_Request ID cache: " + ioe.Message, null, ioe);
            }

            //  Read the file contents into memory
            if ((Adk.Debug & AdkDebugFlags.Lifecycle) != 0)
            {
                Agent.Log.Debug("Reading SIF_Request ID cache: " + fileName);
            }

            // At startup, we pack the file by writing all of the active entries into
            // a second file and then replacing the original file with this copy
            String tmpName = null;
            FileStream tmp = null;

            try
            {
                tmpName = agent.WorkDir + Path.DirectorySeparatorChar + "requests.$dk";
                tmp = new FileStream(tmpName, FileMode.OpenOrCreate, FileAccess.Write);
                tmp.SetLength(0);

                int days = 90;
                String str = agent.Properties["adkglobal.requestCache.age"];
                if (str != null)
                {
                    try
                    {
                        days = Int32.Parse(str);
                    }
                    catch (Exception ex)
                    {
                        Agent.Log.Warn
                            (
                            "Error parsing property 'adkglobal.requestCache.age', default of 90 days will be used: " +
                            ex.Message, ex);
                    }
                }

                DateTime maxAge = DateTime.Now.Subtract(TimeSpan.FromDays(days));
                RequestCacheFileEntry next = null;
                while ((next = Read(fFile, false)) != null)
                {
                    if (next.IsActive && next.RequestTime > maxAge)
                    {
                        Store(tmp, next);
                    }
                }

                tmp.Close();
                fFile.Close();

                //
                //  Overwrite the requests.adk file with the temporary, then
                //  delete the temporary.
                //
                //if ((currentCacheFile.Attributes & FileAttributes.ReadOnly) != 0)
                //{
                //   currentCacheFile.Attributes = FileAttributes.Normal;
                //}
                currentCacheFile.Delete();
                File.Move(tmpName, fileName);

                try
                {
                    fFile = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
                }
                catch (Exception fnfe)
                {
                    throw new AdkException
                        ("Error opening or creating SIF_Request ID cache: " + fnfe, null, fnfe);
                }

                if ((Adk.Debug & AdkDebugFlags.Lifecycle) != 0)
                {
                    Agent.Log.Debug
                        ("Read " + fCache.Count + " pending SIF_Request IDs from cache");
                }
            }
            catch (Exception ioe)
            {
                //  Make sure the files are closed
                if (tmp != null)
                {
                    try
                    {
                        tmp.Close();
                    }
                    catch (Exception ex)
                    {
                        Agent.Log.WarnFormat("Exception thrown while closing FileStream: {0}", ex);
                    }

                }
                if (fFile != null)
                {
                    try
                    {
                        fFile.Close();
                    }
                    catch (Exception ex)
                    {
                        Agent.Log.WarnFormat("Exception thrown while closing file: {0}", ex);
                    }

                }
                if ((currentCacheFile.Attributes & FileAttributes.ReadOnly) != 0)
                {
                    throw new AdkException
                          ("Error opening or creating SIF_Request ID cache: " + ioe, null, ioe);

                }

                if (isRetry)
                {
                    // We've already tried reinitializing. rethrow
                    if (ioe is AdkException)
                    {
                        throw;
                    }
                    else
                    {
                        throw new AdkException
                            ("Error opening or creating SIF_Request ID cache: " + ioe, null, ioe);
                    }
                }
                else
                {
                    Agent.Log.Warn
                        ("Could not read SIF_Request ID cache (will start with fresh cache): " +
                          ioe);

                    //
                    //  Delete the files and re-initialize from scratch. We don't
                    //  want a file error here to prevent the agent from running, so
                    //  no exception is thrown to the caller.
                    //
                    File.Delete(fileName);
                    File.Delete(tmpName);
                    Initialize(agent, true);
                }
            }
        }

        /// <summary>  Closes the RequestCache</summary>
        public override void Close()
        {
            lock (this)
            {
                base.Close();
                try
                {
                    if (fFile != null)
                    {
                        fFile.Close();
                    }
                }
                catch (Exception ioe)
                {
                    throw new AdkException
                        ("Error closing SIF_Request ID cache: " + ioe, null, ioe);
                }
            }
        }
        /// <summary>  Return the count of active requests in the cache</summary>
        public override int ActiveRequestCount
        {
            get { return fCache.Count; }
        }

        /// <summary>  Store the request MsgId and associated SIF Data Object type in the cache</summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override IRequestInfo StoreRequestInfo(SIF_Request request,
                                                       Query q,
                                                       IZone zone)
        {
            try
            {
                RequestCacheFileEntry entry = new RequestCacheFileEntry(true);
                entry.SetObjectType(request.SIF_Query.SIF_QueryObject.ObjectName);
                entry.SetMessageId(request.MsgId);
                entry.SetUserData(q.UserData);
                Store(fFile, entry);
                return entry;
            }
            catch (Exception thr)
            {
                throw new AdkException
                    ("Error writing to SIF_Request ID cache (MsgId: " + request.MsgId + ") " + thr,
                      zone, thr);
            }
        }

        private void Store(FileStream outStream,
                            RequestCacheFileEntry entry)
        {
            Boolean success;
            //cStore serialized Request in 2 parts
            //.NET framework returns null if Error occurs during
            //deserialization. 
            //In our case, when State error occurs, we'd still 
            //like to get the requestCacheFileEntry back
            //so I'm splitting the serialization into 2 parts:
            //State  and  requestCacheFileEntry
            //string l
            //
            entry.SetRequestTime(DateTime.Now);



            //store RequestFileCacheEntry to memory in HashTable
            fCache[entry.MessageId] = entry;


            // ******************************************************************
            //                                                          *********
            //             Serialize RequestCacheFile                   *********
            //                                                          *********
            outStream.Seek(0, SeekOrigin.End);
            entry.Location = outStream.Position;
            byte isActive = entry.IsActive ? (byte)1 : (byte)0;
            outStream.WriteByte(isActive);
            outStream.Flush();


            success = WriteNext(outStream, entry);
            if (success == false)
            {
                outStream.SetLength(entry.Location);
                return;
            }

            // ******************************************************************
            //                                                          *********
            //             Serialize State Information                  *********
            //                                                          *********
            // object l_objState = entry.State;
            WriteNext(outStream, entry.State);

        }

        /// <summary>
        /// Reads the next RequestCachFile Request from the FileStream
        /// and advances the Position of the filestream .
        /// The first 4 bytes return the length of the serialized object
        /// The remaining Bytes holds the serialized object bytes.
        /// </summary>
        /// <param name="outStream">The stream to read objects out of</param>
        private object ReadNext(FileStream outStream)
        {
            byte[] rawLength = new byte[4];
            outStream.ReadExactly(rawLength, 0, 4);

            int objectLength = BitConverter.ToInt32(rawLength, 0);
            if (objectLength > 0)
            {
                byte[] serializedObject = new byte[objectLength];
                outStream.ReadExactly(serializedObject, 0, objectLength);
                return MessagePackSerializer.Typeless.Deserialize(serializedObject, s_messagePackOptions);
            }
            return null;
        }


        /// <summary>
        /// Writes the next RequestCachFile Request to the FileStream
        /// and advances the Position of the filestream .
        /// The first 4 bytes return the length of the serialized object
        /// The remaining Bytes holds the object bytes.
        /// </summary>
        /// <param name="outStream">The stream being serialized</param>
        /// <param name="serializedObject">The object being serialized</param>
        private bool WriteNext(FileStream outStream, object serializedObject)
        {

            byte[] objectRawLength = new byte[4];
            outStream.Seek(0, SeekOrigin.End);

            //Make placeholder for serialized object length, write to fs
            outStream.Write(objectRawLength, 0, 4);
            
            if (serializedObject == null)
            {
                return true;
            }
            else
            {
                
                //Record starting position of stream where serialized object begins
                long startPosition = outStream.Position;

                try
                {
                    //serialize and write object to fs
                    byte[] messagePackBytes = MessagePackSerializer.Typeless.Serialize(serializedObject, s_messagePackOptions);
                    outStream.Write(messagePackBytes, 0, messagePackBytes.Length);
                }
                catch (Exception ex)
                {
                    Agent.Log.Error
                        ("Exception occurred while Writing RequestInfo State object to RequestCacheFile: " +
                         ex.Message, ex);
                    outStream.Seek(startPosition, SeekOrigin.Begin);
                    outStream.SetLength( startPosition );
                    return false;

                }
                outStream.Flush();

                // Write the serialized object's length to fs at placeholder position from above
                int objLength = (int) (outStream.Position - startPosition);
                objectRawLength = BitConverter.GetBytes(objLength);
                outStream.Seek(startPosition - 4, SeekOrigin.Begin);
                outStream.Write(objectRawLength, 0, 4);
                outStream.Flush();
                // Advance to the end of the stream
                outStream.Seek(0, SeekOrigin.End);
                return true;
            }
        }


        /// <summary>
        /// Reads the next RequestCacheFileEntry from the FileStream
        /// </summary>
        /// <param name="outStream">The filestream to read the entry from</param>
        /// <param name="readInactiveData">If TRUE, all Entries properties will be deserialized, even if inactive. If
        /// False, only active Entries will be deserialized and inactive Entries will be returned as an Entry
        /// with the "IsActive" property set to false </param>
        /// <returns>The next entry or null if at the end of the stream</returns>
        private RequestCacheFileEntry Read(FileStream outStream,
                                            bool readInactiveData)
        {

            int active = outStream.ReadByte();
            if (active == -1)
            {
                // We are at the end of the FileStream
                return null;
            }
            else
            {

                if (active == 0 && !readInactiveData)
                {
                    // Move the stream forward and skip over the serialized entries
                    ReadNext(outStream);
                    ReadNext(outStream);
                    // above objects are throw-away variables.
                    //They are only used to advance the filestream
                    return new RequestCacheFileEntry(false);
                }
                else
                {
                    RequestCacheFileEntry returnValue;
                    try
                    {
                        //************************************************************
                        //      deserialize RequestCacheFileEntry                *****
                        returnValue = (RequestCacheFileEntry)ReadNext(outStream);
                    }
                    catch (Exception ex)
                    {
                        Agent.Log.Warn
                            ("Error Deserializing Request Cache Info: " + ex.Message, ex);
                        return new RequestCacheFileEntry(false);
                    }

                    //************************************************************
                    //      deserialize the State object                     *****
                    try
                    {
                        Object state = ReadNext(outStream);
                        returnValue.State = state;
                    }
                    catch (Exception ex)
                    {
                        Agent.Log.Warn
                        ("Error Deserializing Request Cache State Info: " + ex.Message, ex);

                    }
                    returnValue.SetIsActive(active == 1);
                    return returnValue;


                }
            }
        }

        /// <summary>  Lookup the SIF Data Object type of a pending request given its MsgId,
        /// then remove the entry from the cache. To lookup an entry without removing
        /// it, call the lookupRequestObjectType method.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override IRequestInfo GetRequestInfo(string msgId,
                                                     IZone zone)
        {
            return Lookup(msgId, zone, true);
        }


        /// <summary>
        ///  Lookup the SIF Data Object type of a pending request given its MsgId
        /// </summary>
        /// <param name="msgId">The msgId of the request</param>
        /// <param name="zone">The zone associated with the request</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override IRequestInfo LookupRequestInfo(string msgId,
                                                        IZone zone)
        {
            return Lookup(msgId, zone, false);
        }

        /// <summary>
        /// Looks up the specified entry in the cache.
        /// </summary>
        /// <param name="msgId">The message id to lookup</param>
        /// <param name="zone">The zone associated with the message</param>
        /// <param name="remove">If TRUE, the entry will be removed from the cache</param>
        /// <returns>The entry associated with the specified message ID or NULL if no entry was found</returns>
        private IRequestInfo Lookup(string msgId,
                                     IZone zone,
                                       bool remove)
        {
            RequestCacheFileEntry e = (RequestCacheFileEntry)fCache[msgId];
            if (e == null)
            {
                return null;
            }

            if (remove)
            {
                fCache.Remove(msgId);
                try
                {
                    fFile.Seek(e.Location, SeekOrigin.Begin);
                    fFile.WriteByte(0);
                }
                catch (Exception ioe)
                {
                    throw new AdkException
                        ("Error removing entry from SIF_Request ID cache: " + ioe.Message, zone,
                          null);
                }
            }

            return e;
        }

        /// <summary>
        /// <see cref="MessagePackSerializerOptions"/> designed for <see cref="RequestCacheFile"/>.
        /// </summary>
        internal class AdkSerializerOptions : MessagePackSerializerOptions
        {
            /// <summary>
            /// ADK default options based on <see cref="MessagePackSerializerOptions.Standard"/>
            /// with the following changes:
            /// <list type="bullet">
            ///     <item>
            ///         <term>Security</term>
            ///         <description><see cref="MessagePackSecurity.UntrustedData"/></description>
            ///     </item>
            ///     <item>
            ///         <term>Resolver</term>
            ///         <description>
            ///             <see cref="CompositeResolver"/> of
            ///             <see cref="AdkResolver"/> and
            ///             <see cref="TypelessContractlessStandardResolver"/>
            ///         </description>
            ///     </item>
            /// </list>
            /// </summary>
            internal AdkSerializerOptions() : base(Standard
                .WithSecurity(MessagePackSecurity.UntrustedData)
                .WithResolver(CompositeResolver.Create(
                    AdkResolver.Instance,
                    TypelessContractlessStandardResolver.Instance))) { }

            /// <summary>
            /// Clone constructor.
            /// </summary>
            /// <param name="options">Options to clone.</param>
            protected AdkSerializerOptions(AdkSerializerOptions options) : base(options) { }

            /// <inheritdoc />
            public override Type LoadType(string typeName)
            {
                var type = base.LoadType(typeName);
                if (type.Namespace.StartsWith("OpenADK.")) return type;
                return type.Namespace switch
                {
                    "System" => type,
                    "System.Collections" => type,
                    "System.Collections.Generic" => type,
                    _ => null, // Unknown, potentially unsafe type. Refuse to load.
                };
            }

            /// <inheritdoc />
            protected override MessagePackSerializerOptions Clone()
            {
                if (this.GetType() != typeof(AdkSerializerOptions))
                {
                    throw new NotSupportedException($"The derived type {this.GetType().FullName} did not override the {nameof(Clone)} method as required.");
                }
                return new AdkSerializerOptions(this);
            }
        }

        /// <summary>
        /// Wrapper for serializing <see cref="SifElement"/> instances via their XML representation.
        /// </summary>
        [MessagePackObject]
        public class SifElementWrapper
        {
            [Key(0)]
            public string TypeName { get; set; }

            [Key(1)]
            public string XmlContent { get; set; }
        }

        /// <summary>
        /// Custom formatter helper for <see cref="SifElement"/> subclasses. Serializes to XML via
        /// <see cref="SifWriter"/> and restores via <see cref="SifParser"/>.
        /// Not registered directly as an <c>IMessagePackFormatter&lt;object&gt;</c> to avoid
        /// conflicts; used only through <see cref="SifElementFormatterWrapper{T}"/>.
        /// </summary>
        internal class SifElementFormatter
        {
            public static readonly SifElementFormatter Instance = new();

            public void Serialize(ref MessagePackWriter writer, object value, MessagePackSerializerOptions options)
            {
                if (value == null)
                {
                    writer.WriteNil();
                    return;
                }

                var element = (SifElement)value;

                // SifWriter skips elements not marked as changed (dirty). Ensure the element
                // is marked changed so all its content is written to the XML representation.
                element.SetChanged(true);

                using var ms = new MemoryStream();
                var sifWriter = new SifWriter(ms);
                sifWriter.Write(element);
                sifWriter.Flush();
                string xmlContent = System.Text.Encoding.UTF8.GetString(ms.ToArray());

                var wrapper = new SifElementWrapper
                {
                    TypeName = value.GetType().AssemblyQualifiedName,
                    XmlContent = xmlContent
                };

                var wrapperBytes = MessagePackSerializer.Serialize(wrapper, options);
                writer.WriteRaw(wrapperBytes);
            }

            public object Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
            {
                if (reader.IsNil)
                {
                    reader.ReadNil();
                    return null;
                }

                var wrapperBytes = reader.ReadRaw();
                var wrapper = MessagePackSerializer.Deserialize<SifElementWrapper>(wrapperBytes, options);

                if (wrapper?.XmlContent == null)
                    return null;

                var element = SifParser.NewInstance().Parse(wrapper.XmlContent);

                // SifParser returns null for empty XML elements (no fields/children).
                // Fall back to creating a new instance via the stored type name.
                if (element == null && wrapper.TypeName != null)
                {
                    var type = Type.GetType(wrapper.TypeName);
                    if (type != null && typeof(SifElement).IsAssignableFrom(type))
                    {
                        element = (SifElement)Activator.CreateInstance(type);
                    }
                }

                return element;
            }
        }

        /// <summary>
        /// Custom formatter for <see cref="SifElementWrapper"/> to avoid MessagePack's dynamic
        /// code generation failing on the type nested inside an internal class.
        /// </summary>
        internal class SifElementWrapperFormatter : IMessagePackFormatter<SifElementWrapper>
        {
            public static readonly SifElementWrapperFormatter Instance = new();

            public void Serialize(ref MessagePackWriter writer, SifElementWrapper value, MessagePackSerializerOptions options)
            {
                if (value == null) { writer.WriteNil(); return; }
                writer.WriteArrayHeader(2);
                writer.Write(value.TypeName);
                writer.Write(value.XmlContent);
            }

            public SifElementWrapper Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
            {
                if (reader.IsNil) { reader.ReadNil(); return null; }
                int count = reader.ReadArrayHeader();
                var wrapper = new SifElementWrapper();
                if (count > 0) wrapper.TypeName = reader.ReadString();
                if (count > 1) wrapper.XmlContent = reader.ReadString();
                return wrapper;
            }
        }

        /// <summary>
        /// Generic typed wrapper for SifElement formatters.
        /// </summary>
        internal class SifElementFormatterWrapper<T> : IMessagePackFormatter<T> where T : SifElement
        {
            public void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options)
            {
                SifElementFormatter.Instance.Serialize(ref writer, (object)value, options);
            }

            public T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
            {
                return (T)SifElementFormatter.Instance.Deserialize(ref reader, options);
            }
        }

        /// <summary>
        /// Wrapper for serializing SifSimpleType instances that don't have parameterless constructors.
        /// </summary>
        [MessagePackObject]
        public class SifSimpleTypeWrapper
        {
            [Key(0)]
            public string TypeName { get; set; }

            [Key(1)]
            public object Value { get; set; }
        }

        /// <summary>
        /// Custom formatter for <see cref="SifSimpleType"/> subclasses that don't have parameterless constructors.
        /// Serializes to a wrapper object, then reconstructs during deserialization.
        /// </summary>
        internal class SifSimpleTypeFormatter : IMessagePackFormatter<object>
        {
            public static readonly SifSimpleTypeFormatter Instance = new();

            public void Serialize(ref MessagePackWriter writer, object value, MessagePackSerializerOptions options)
            {
                if (value == null)
                {
                    writer.WriteNil();
                    return;
                }

                var sifType = value as SifSimpleType;
                if (sifType == null)
                {
                    // Not a SifSimpleType, shouldn't happen if resolver is correct
                    throw new MessagePackSerializationException($"Expected SifSimpleType, got {value.GetType().Name}");
                }

                // Serialize as a wrapper with type name and raw value
                var wrapper = new SifSimpleTypeWrapper
                {
                    TypeName = value.GetType().FullName,
                    Value = sifType.RawValue
                };

                // Use MessagePackSerializer to serialize the wrapper
                var wrapperBytes = MessagePackSerializer.Serialize(wrapper, options);
                writer.WriteRaw(wrapperBytes);
            }

            public object Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
            {
                if (reader.IsNil)
                {
                    reader.ReadNil();
                    return null;
                }

                // Deserialize the wrapper
                var wrapperBytes = reader.ReadRaw();
                var wrapper = MessagePackSerializer.Deserialize<SifSimpleTypeWrapper>(wrapperBytes, options);

                if (wrapper == null)
                {
                    return null;
                }

                var type = Type.GetType(wrapper.TypeName);
                if (type == null || !typeof(SifSimpleType).IsAssignableFrom(type))
                {
                    throw new MessagePackSerializationException($"Unknown or invalid SifSimpleType: {wrapper.TypeName}");
                }

                // Find the best constructor for this type
                var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
                if (constructors.Length == 0)
                {
                    throw new MessagePackSerializationException($"No public constructors found for {type.FullName}");
                }

                // Prefer non-obsolete constructors
                ConstructorInfo targetConstructor = null;
                foreach (var ctor in constructors)
                {
                    var obsoleteAttr = ctor.GetCustomAttribute<ObsoleteAttribute>();
                    if (obsoleteAttr == null)
                    {
                        targetConstructor = ctor;
                        break;
                    }
                    if (targetConstructor == null)
                    {
                        targetConstructor = ctor;
                    }
                }

                if (targetConstructor == null)
                {
                    throw new MessagePackSerializationException($"No suitable constructor found for {type.FullName}");
                }

                try
                {
                    return targetConstructor.Invoke(new[] { wrapper.Value });
                }
                catch (Exception ex) when (!(ex is MessagePackSerializationException))
                {
                    throw new MessagePackSerializationException($"Failed to instantiate {type.FullName}: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Composite resolver supporting SifSimpleType subclasses.
        /// </summary>
        /// <summary>
        /// Custom formatter for <see cref="RequestCacheFileEntry"/> instances.
        /// Serializes only the fields that should be persisted to the cache file.
        /// </summary>
        internal class RequestCacheFileEntryFormatter : IMessagePackFormatter<RequestCacheFileEntry>
        {
            public static readonly RequestCacheFileEntryFormatter Instance = new();

            public void Serialize(ref MessagePackWriter writer, RequestCacheFileEntry value, MessagePackSerializerOptions options)
            {
                if (value == null)
                {
                    writer.WriteNil();
                    return;
                }
                writer.WriteArrayHeader(3);
                writer.Write(value.ObjectType);
                writer.Write(value.MessageId);
                options.Resolver.GetFormatterWithVerify<DateTime>().Serialize(ref writer, value.RequestTime, options);
            }

            public RequestCacheFileEntry Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
            {
                if (reader.IsNil)
                {
                    reader.ReadNil();
                    return null;
                }
                int count = reader.ReadArrayHeader();
                var entry = new RequestCacheFileEntry(true);
                if (count > 0) entry.SetObjectType(reader.ReadString());
                if (count > 1) entry.SetMessageId(reader.ReadString());
                if (count > 2) entry.SetRequestTime(options.Resolver.GetFormatterWithVerify<DateTime>().Deserialize(ref reader, options));
                return entry;
            }
        }

        /// <summary>
        /// Custom formatter for <see cref="SifSimpleTypeWrapper"/> to avoid MessagePack's dynamic
        /// code generation failing on the type nested inside an internal class.
        /// </summary>
        internal class SifSimpleTypeWrapperFormatter : IMessagePackFormatter<SifSimpleTypeWrapper>
        {
            public static readonly SifSimpleTypeWrapperFormatter Instance = new();

            public void Serialize(ref MessagePackWriter writer, SifSimpleTypeWrapper value, MessagePackSerializerOptions options)
            {
                if (value == null)
                {
                    writer.WriteNil();
                    return;
                }
                writer.WriteArrayHeader(2);
                writer.Write(value.TypeName);
                TypelessFormatter.Instance.Serialize(ref writer, value.Value, options);
            }

            public SifSimpleTypeWrapper Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
            {
                if (reader.IsNil)
                {
                    reader.ReadNil();
                    return null;
                }
                int count = reader.ReadArrayHeader();
                var wrapper = new SifSimpleTypeWrapper();
                if (count > 0) wrapper.TypeName = reader.ReadString();
                if (count > 1) wrapper.Value = TypelessFormatter.Instance.Deserialize(ref reader, options);
                return wrapper;
            }
        }

        internal class AdkResolver : IFormatterResolver
        {
            public static readonly AdkResolver Instance = new();

            protected readonly ConcurrentDictionary<Type, IMessagePackFormatter> FormatterCache = new();

            public IMessagePackFormatter<T> GetFormatter<T>()
            {
                var type = typeof(T);

                if (type == typeof(RequestCacheFileEntry))
                {
                    var formatter = FormatterCache.GetOrAdd(type, RequestCacheFileEntryFormatter.Instance);
                    return (IMessagePackFormatter<T>)formatter;
                }

                if (type == typeof(SifSimpleTypeWrapper))
                {
                    var formatter = FormatterCache.GetOrAdd(type, SifSimpleTypeWrapperFormatter.Instance);
                    return (IMessagePackFormatter<T>)formatter;
                }

                if (type == typeof(SifElementWrapper))
                {
                    var formatter = FormatterCache.GetOrAdd(type, SifElementWrapperFormatter.Instance);
                    return (IMessagePackFormatter<T>)formatter;
                }

                // Check if T is a SifSimpleType subclass
                if (typeof(SifSimpleType).IsAssignableFrom(type))
                {
                    // Create a formatter for this specific type using dynamic wrapping
                    var formatter = FormatterCache.GetOrAdd(type, t =>
                    {
                        var formatterType = typeof(SifSimpleTypeFormatterWrapper<>).MakeGenericType(t);
                        return (IMessagePackFormatter)Activator.CreateInstance(formatterType);
                    });
                    return (IMessagePackFormatter<T>)formatter;
                }

                // Check if T is a SifElement subclass
                if (typeof(SifElement).IsAssignableFrom(type))
                {
                    var formatter = FormatterCache.GetOrAdd(type, t =>
                    {
                        var formatterType = typeof(SifElementFormatterWrapper<>).MakeGenericType(t);
                        return (IMessagePackFormatter)Activator.CreateInstance(formatterType);
                    });
                    return (IMessagePackFormatter<T>)formatter;
                }

                // Return null for fallback mechanism.
                return null;
            }
        }

        /// <summary>
        /// Generic wrapper for SifSimpleType formatters.
        /// </summary>
        internal class SifSimpleTypeFormatterWrapper<T> : IMessagePackFormatter<T> where T : SifSimpleType
        {
            public void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options)
            {
                SifSimpleTypeFormatter.Instance.Serialize(ref writer, (object)value, options);
            }

            public T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
            {
                var result = SifSimpleTypeFormatter.Instance.Deserialize(ref reader, options);
                return (T)result;
            }
        }
    }
}
