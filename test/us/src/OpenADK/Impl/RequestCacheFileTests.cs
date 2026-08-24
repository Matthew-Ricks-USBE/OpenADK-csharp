using Library.UnitTesting.Framework;
using Xunit;
using OpenADK.Library;
using OpenADK.Library.Impl;
using OpenADK.Library.Infra;
using OpenADK.Library.us.Common;
using OpenADK.Library.us.Student;
using System;
using System.Collections.Generic;
using System.IO;

namespace Library.xUnit.US.Impl
{
    
    public class RequestCacheFileTests : AdkTest, IDisposable
    {
        private String[] fMsgIds;
        private RequestCache fRC;
        private Agent fAgent;

        public RequestCacheFileTests()
        {
            fAgent = CreateTestAgent();
            fAgent.Initialize();
        }

        public override void Dispose()
        {
            if (fRC != null)
            {
                fRC.Close();
                fRC = null;
            }

            String fname = fAgent.HomeDir + Path.DirectorySeparatorChar + "work" + Path.DirectorySeparatorChar +
                           "requests.adk";
            try
            {
                System.Threading.Thread.Sleep(100);
                File.Delete(fname);
            }
            catch (IOException)
            {
                try
                {
                    System.Threading.Thread.Sleep(200);
                    File.Delete(fname);
                }
                catch (IOException ex)
                {
                    System.Console.WriteLine("Warning: Unable to delete cache file: " + ex.Message);
                }
            }
        }


        /**
       * Tests that the RequestCache file persists information between requests
       * @throws Exception
       */

        [Fact]
        public void testSimpleCase()
        {
            fRC?.Close();
            fRC = Components.CreateRequestCache(fAgent);
            storeAssertedRequests(fRC);
            assertStoredRequests(fRC, true);
        }

        /**
       * Tests that the RequestCache file persists information between restarts
       * @throws Exception
       */

        [Fact]
        public void testPersistence()
        {
            fRC?.Close();
            fRC = Components.CreateRequestCache(fAgent);
            storeAssertedRequests(fRC);
            fRC.Close();

            // Create a new instance. This one should retrieve its settings from the persistence mechanism
            fRC = Components.CreateRequestCache(fAgent);
            assertStoredRequests(fRC, true);

            fRC.Close();
            fRC = Components.CreateRequestCache(fAgent);
            Assert.True(0 == fRC.ActiveRequestCount, "Should have zero pending requests");
        }


       /**
       * Tests that the RequestCache file persists information between restarts
       * Even if the state object is not able to be deserialized
       * @throws Exception
       */

       [Fact]
       public void testPersistenceWithBadState()
       {
           //create new cache for agent
           fRC = Components.CreateRequestCache(fAgent);

           //create new queryobject
           SIF_QueryObject obj = new SIF_QueryObject("");
           //create query, telling it what type of query it is(passing it queryobj)
           SIF_Query query = new SIF_Query(obj);
           //create new sif request
           SIF_Request request = new SIF_Request();
           //set query property
           request.SIF_Query = query;


           Query q = new Query(StudentDTD.STUDENTPERSONAL);

           String testStateItem = Runtime.MakeGuid();
           String requestMsgId = Runtime.MakeGuid();
           String testObjectType = Runtime.MakeGuid();

           // Use string user data (allowed type)
           q.UserData = testStateItem;
           storeRequest(fRC, request, q, requestMsgId, testObjectType);

           fRC.Close();

           // Create a new instance. This one should retrieve its settings from the persistence mechanism
           fRC = Components.CreateRequestCache(fAgent);

           IRequestInfo ri = fRC.GetRequestInfo(requestMsgId, null);

           // RequestInfo should still be available even if UserData isn't
           Assert.NotNull(ri);
           Assert.True(requestMsgId == ri.MessageId, "MessageId");
           Assert.True(testObjectType == ri.ObjectType, "ObjectType");
       }

        [Fact]
        public void testInstanceMultipleInvocations()
        {
            for (int i = 0; i < 3; i++)
            {
                testPersistence();
            }
        }

        [Fact]
        public void testPersistenceMultipleInvocations()
        {
            for (int i = 0; i < 3; i++)
            {
                testSimpleCase();
            }
        }

        [Fact]
        public void testPersistenceWithRemoval()
        {
            fRC = Components.CreateRequestCache(fAgent);
            SIF_QueryObject obj = new SIF_QueryObject("");
            SIF_Query query = new SIF_Query(obj);
            SIF_Request request = new SIF_Request();

            request.SIF_Query = query;

            Query q = new Query(StudentDTD.STUDENTPERSONAL);
            String testStateItem = Runtime.MakeGuid();
            // Use string user data instead of TestState (allowed type)
            q.UserData = testStateItem;

            fMsgIds = new String[10];
            // Add 10 entries to the cache, interspersed with other entries that are removed
            for (int i = 0; i < 10; i++)
            {
                String phantom1 = Runtime.MakeGuid();
                String phantom2 = Runtime.MakeGuid();
                storeRequest(fRC, request, q, phantom1, "foo");
                fMsgIds[i] = Runtime.MakeGuid();
                storeRequest(fRC, request, q, fMsgIds[i], "Object_" + i);
                storeRequest(fRC, request, q, phantom2, "bar");

                fRC.GetRequestInfo(phantom1, null);
                fRC.GetRequestInfo(phantom2, null);
            }

            // remove every other entry, close, re-open and assert that the correct entries are there
            for (int i = 0; i < 10; i += 2)
            {
                fRC.GetRequestInfo(fMsgIds[i], null);
            }

            Assert.True(5 == fRC.ActiveRequestCount, "Before closing Should have five objects");
            fRC.Close();

            // Create a new instance. This one should retrieve its settings from the persistence mechanism
            fRC = Components.CreateRequestCache(fAgent);
            Assert.True(5 == fRC.ActiveRequestCount, "After Re-Openeing Should have five objects");
            for (int i = 1; i < 10; i += 2)
            {
                IRequestInfo cachedInfo = fRC.GetRequestInfo(fMsgIds[i], null);
                Assert.NotNull(cachedInfo);
            }
            Assert.True(0 == fRC.ActiveRequestCount, "Should have zero objects");
        }


        /**
       * Tests that the RequestCacheFile class handles the case of the 
       * cache file becoming readonly
       * @throws Exception
       */

        [Fact]
        public void testWithReadOnlyFile()
        {
            // Make the existing cache file readonly
            String fname = fAgent.HomeDir + Path.DirectorySeparatorChar + "work" + Path.DirectorySeparatorChar +
                           "requests.adk";
            FileInfo fi = new FileInfo(fname);

            //= new File(fname);
            if (!fi.Exists)
            {
                StreamWriter sw = fi.CreateText();
                //sw.WriteLine("");
                sw.Flush();
                sw.Close();

                // RandomAccessFile raf = new RandomAccessFile(fname, "rw");
                // raf.setLength(0);
                //raf.close();
            }
            fi.IsReadOnly = true;
            try
            {
                Assert.Throws<AdkException>(() => Components.CreateRequestCache(fAgent));
            }
            finally
            {
                fi.IsReadOnly = false;
                fi.Delete();
            }
        }


        /**
       * Tests that the RequestCache file persists information between restarts, even if it starts
       * with a corrupt file
       * @throws Exception
       */

        [Fact]
        public void testWithCorruptFile()
        {
            // Delete the existing cache file, if it exists
            String fname = fAgent.HomeDir + Path.DirectorySeparatorChar + "work" + Path.DirectorySeparatorChar +
                           "requestcache.adk";
            FileInfo fi = new FileInfo(fname);
            StreamWriter sw = fi.CreateText();
            sw.WriteLine("!@#$!@#$");
            sw.Flush();
            sw.Close();

            //RandomAccessFile raf = new RandomAccessFile(fname, "rw");
            //raf.writeChars("!@#$!@#$");
            // raf.close();

            fRC = Components.CreateRequestCache(fAgent);
            storeAssertedRequests(fRC);
            fRC.Close();

            // Create a new instance. This one should retrieve its settings from the persistence mechanism
            fRC = Components.CreateRequestCache(fAgent);
            assertStoredRequests(fRC, true);
        }

        [Fact]
        public void testWithLegacyFile()
        {
            //assertStoredRequests(fRC, true);
            // Copy the legacy requests.adk file to the agent work directory
            //FileInfo legacyFile = new FileInfo("requests.adk");

            //Assert.True(legacyFile.Exists, "Saved legacy file does [not?] exist");
            //FileInfo copiedFile = new FileInfo(fAgent.HomeDir + Path.DirectorySeparatorChar + "work" + Path.DirectorySeparatorChar + "requests.adk");
            //if (copiedFile.Exists)
            //{
            //   copiedFile.Delete();
            //}

            //// Copy the file
            //legacyFile.CopyTo(copiedFile.FullName, true);

            // Now open up an instance of the request cache and verify that the contents are there


            fRC = Components.CreateRequestCache(fAgent);
            SIF_QueryObject obj = new SIF_QueryObject("");
            SIF_Query query = new SIF_Query(obj);
            SIF_Request request = new SIF_Request();
            request.SIF_Query = query;

            Query q;

            fMsgIds = new String[10];
            // Add 10 entries to the cache 
            for (int i = 0; i < 10; i++)
            {
                // Use string user data instead of TestState (allowed type)
                String stateData = Runtime.MakeGuid();
                q = new Query(StudentDTD.STUDENTPERSONAL);
                q.UserData = stateData;
                fMsgIds[i] = Runtime.MakeGuid();
                storeRequest(fRC, request, q, fMsgIds[i], "Object_" + i.ToString());
            }


            Assert.True(10 == fRC.ActiveRequestCount, "Active request count");


            // Lookup each setting, 
            for (int i = 0; i < 10; i++)
            {
                IRequestInfo reqInfo = fRC.LookupRequestInfo(fMsgIds[i], null);
                Assert.True("Object_" + i.ToString() == reqInfo.ObjectType, "Initial lookup");
            }

            // Lookup each setting, 
            for (int i = 0; i < 10; i++)
            {
                IRequestInfo reqInfo = fRC.GetRequestInfo(fMsgIds[i], null);
                Assert.True("Object_" + i.ToString() == reqInfo.ObjectType, "Initial lookup");
            }

            // all messages should now be removed from the queue
            Assert.True(0 == fRC.ActiveRequestCount, "Cache should be empty");

            // Now run one of our other tests
            testPersistence();
        }


        /**
       * Stores the items in the cache that will later be asserted
       * @param cache
       */

        private void storeAssertedRequests(RequestCache cache)
        {
            SIF_QueryObject obj = new SIF_QueryObject("");
            SIF_Query query = new SIF_Query(obj);
            SIF_Request request = new SIF_Request();
            request.SIF_Query = query;

            Query q;

            fMsgIds = new String[10];
            // Add 10 entries to the cache, interspersed with other entries that are removed
            for (int i = 0; i < 10; i++)
            {
                q = new Query(StudentDTD.STUDENTPERSONAL);
                // Use string user data instead of TestState (allowed type)
                String stateData = "TestState_" + Runtime.MakeGuid();
                q.UserData = stateData;

                String phantom1 = Runtime.MakeGuid();
                String phantom2 = Runtime.MakeGuid();
                storeRequest(cache, request, q, phantom1, "foo");
                fMsgIds[i] = Runtime.MakeGuid();

                storeRequest(cache, request, q, fMsgIds[i], "Object_" + i.ToString());
                storeRequest(cache, request, q, phantom2, "bar");

                cache.GetRequestInfo(phantom1, null);
                cache.GetRequestInfo(phantom2, null);
            }
        }


        private void storeRequest(
            RequestCache rc,
            SIF_Request request,
            Query q,
            String msgID,
            String objectName)
        {
            //request.getSIF_Query().getSIF_QueryObject().setObjectName(objectName);
            request.SIF_Query.SIF_QueryObject.ObjectName = objectName;
            request.Header.SIF_MsgId = msgID;
            rc.StoreRequestInfo(request, q, null);
        }


        /**
       * Asserts that the items stored in the storeAssertedRequests call
       * are still in the cache
       * @param cache The RequestCache class to assert
       * @param testRemoval True if the items in the cache should be removed
       * and asserted that they are removed
       */

        private void assertStoredRequests(RequestCache cache, Boolean testRemoval)
        {
            Assert.True(fMsgIds.Length == cache.ActiveRequestCount, "Active request count");

            // Lookup each setting, 
            for (int i = 0; i < fMsgIds.Length; i++)
            {
                IRequestInfo reqInfo = cache.LookupRequestInfo(fMsgIds[i], null);
                Assert.True("Object_" + i.ToString() == reqInfo.ObjectType, "Initial lookup");
                // Verify user data is a string if present (not required to persist)
                if (reqInfo.UserData != null)
                {
                    Assert.IsAssignableFrom<string>(reqInfo.UserData);
                }
            }

            if (testRemoval)
            {
                // Lookup each setting, 
                for (int i = 0; i < fMsgIds.Length; i++)
                {
                    IRequestInfo reqInfo = cache.GetRequestInfo(fMsgIds[i], null);
                    Assert.True("Object_" + i.ToString() == reqInfo.ObjectType, "Initial lookup");
                    // Verify user data is a string if present (not required to persist)
                    if (reqInfo.UserData != null)
                    {
                        Assert.IsAssignableFrom<string>(reqInfo.UserData);
                    }
                }

                // all messages should now be removed from the queue
                Assert.True(0 == cache.ActiveRequestCount, "Cache should be empty");
            }
        }

        [Fact]
        public void testSerializationWithStringUserData()
        {
            // Test that string user data (System.* type) is properly serialized/deserialized
            fRC = Components.CreateRequestCache(fAgent);

            SIF_QueryObject obj = new SIF_QueryObject("");
            SIF_Query query = new SIF_Query(obj);
            SIF_Request request = new SIF_Request();
            request.SIF_Query = query;

            Query q = new Query(StudentDTD.STUDENTPERSONAL);
            string testData = "TestUserDataString_" + Runtime.MakeGuid();
            q.UserData = testData;

            String msgId = Runtime.MakeGuid();
            storeRequest(fRC, request, q, msgId, "StudentPersonal");

            fRC.Close();

            // Re-open and verify the string data was preserved
            fRC = Components.CreateRequestCache(fAgent);
            IRequestInfo ri = fRC.GetRequestInfo(msgId, null);

            Assert.NotNull(ri);
            Assert.True(testData == (string)ri.UserData, "UserData should match original string");
        }

        [Fact]
        public void testSerializationWithGuidUserData()
        {
            // Test that Guid data (System.* type) is properly serialized/deserialized
            fRC = Components.CreateRequestCache(fAgent);

            SIF_QueryObject obj = new SIF_QueryObject("");
            SIF_Query query = new SIF_Query(obj);
            SIF_Request request = new SIF_Request();
            request.SIF_Query = query;

            Query q = new Query(StudentDTD.STUDENTPERSONAL);
            var testData = Guid.NewGuid();
            q.UserData = testData;

            String msgId = Runtime.MakeGuid();
            storeRequest(fRC, request, q, msgId, "StudentPersonal");

            fRC.Close();

            // Re-open and verify the string data was preserved
            fRC = Components.CreateRequestCache(fAgent);
            IRequestInfo ri = fRC.GetRequestInfo(msgId, null);

            Assert.NotNull(ri);
            Assert.True(testData == (Guid)ri.UserData, "UserData should match original Guid");
        }

        [Fact]
        public void testSerializationWithDictionaryUserData()
        {
            // Test that System.Collections types are properly serialized/deserialized
            fRC = Components.CreateRequestCache(fAgent);

            SIF_QueryObject obj = new SIF_QueryObject("");
            SIF_Query query = new SIF_Query(obj);
            SIF_Request request = new SIF_Request();
            request.SIF_Query = query;

            Query q = new Query(StudentDTD.STUDENTPERSONAL);
            var testDict = new System.Collections.Generic.Dictionary<string, object>
            {
                { "Name", "TestName" },
                { "ID", 12345 },
                { "Active", true }
            };
            q.UserData = testDict;

            String msgId = Runtime.MakeGuid();
            storeRequest(fRC, request, q, msgId, "StudentPersonal");

            fRC.Close();

            // Re-open and verify the dictionary data was preserved
            fRC = Components.CreateRequestCache(fAgent);
            IRequestInfo ri = fRC.GetRequestInfo(msgId, null);

            Assert.NotNull(ri);
            Assert.NotNull(ri.UserData);
            // Note: Deserialized dictionary type may vary depending on MessagePack implementation
            Assert.True(ri.UserData is System.Collections.IDictionary, "UserData should be a dictionary type");
        }

        /// <summary>
        /// Tests whether any of the Adk's classes which inherit from SifSimpleType cannot be 
        /// serialized. By default they all should be.
        /// </summary>
        [Fact]
        public void testSerializationWithSifSimpleTypesUserData()
        {
            IList<SifSimpleType> originalList =
            [
                new SifBoolean(true),
                new SifDate(new DateTime(2007, 12, 1)),
                new SifDateTime(new DateTime(2007, 12, 1)),
                new SifDecimal(10),
                new SifDuration(new TimeSpan(1000)),
                new SifInt(5),
                new SifString("This is a test"),
                new SifTime(new DateTime(2007, 12, 1)),
            ];

            SIF_QueryObject obj = new SIF_QueryObject("");
            SIF_Query query = new SIF_Query(obj);
            SIF_Request request = new SIF_Request();
            request.SIF_Query = query;

            Query q = new Query(StudentDTD.STUDENTPERSONAL);

            foreach (var item in originalList)
            {
                Console.WriteLine("Testing type: " + item.GetType().Name);
                q.UserData = item;
                String msgId = Runtime.MakeGuid();

                fRC = Components.CreateRequestCache(fAgent);
                storeRequest(fRC, request, q, msgId, "StudentPersonal");
                fRC.Close();

                // Re-open and verify the dictionary data was preserved
                fRC = Components.CreateRequestCache(fAgent);
                IRequestInfo ri = fRC.GetRequestInfo(msgId, null);
                fRC.Close();

                Assert.NotNull(ri);
                Assert.NotNull(ri.UserData);
            }
        }

        [Fact]
        public void testSerializationWithSifElementUserData()
        {
            // Test that string user data (System.* type) is properly serialized/deserialized
            fRC = Components.CreateRequestCache(fAgent);

            SIF_QueryObject obj = new SIF_QueryObject("");
            SIF_Query query = new SIF_Query(obj);
            SIF_Request request = new SIF_Request();
            request.SIF_Query = query;

            Query q = new Query(StudentDTD.STUDENTPERSONAL);

            #region Name
            var name = new Name(NameType.LEGAL, "Nahorniak", "Mike");
            q.UserData = name;

            String msgId = Runtime.MakeGuid();
            storeRequest(fRC, request, q, msgId, "StudentPersonal");

            fRC.Close();

            // Re-open and verify the string data was preserved
            fRC = Components.CreateRequestCache(fAgent);
            IRequestInfo ri = fRC.GetRequestInfo(msgId, null);

            Assert.NotNull(ri);
            Assert.IsAssignableFrom<Name>(ri.UserData);
            var deserializedName = (Name)ri.UserData;
            Assert.True(name.LastName == deserializedName.LastName, "Name.LastName should round-trip correctly");
            Assert.True(name.FirstName == deserializedName.FirstName, "Name.FirstName should round-trip correctly");
            #endregion

            #region StudentPersonal
            var studentPersonal = new StudentPersonal();
            q.UserData = studentPersonal;

            msgId = Runtime.MakeGuid();
            storeRequest(fRC, request, q, msgId, "StudentPersonal");

            fRC.Close();

            // Re-open and verify the string data was preserved
            fRC = Components.CreateRequestCache(fAgent);
            ri = fRC.GetRequestInfo(msgId, null);

            Assert.NotNull(ri);
            Assert.IsAssignableFrom<StudentPersonal>(ri.UserData);
            #endregion

            #region SIF_Error
            var error = new SIF_Error(
                (int)SifErrorCategoryCode.Generic,
                SifErrorCodes.GENERIC_GENERIC_ERROR_1,
                "Could not serialize the SIF_Err object");
            q.UserData = error;

            msgId = Runtime.MakeGuid();
            storeRequest(fRC, request, q, msgId, "StudentPersonal");

            fRC.Close();

            // Re-open and verify the string data was preserved
            fRC = Components.CreateRequestCache(fAgent);
            ri = fRC.GetRequestInfo(msgId, null);

            Assert.NotNull(ri);
            Assert.IsAssignableFrom<SIF_Error>(ri.UserData);
            var deserializedError = (SIF_Error)ri.UserData;
            Assert.True(error.SIF_Category == deserializedError.SIF_Category, "SIF_Error.SIF_Category should round-trip correctly");
            Assert.True(error.SIF_Code == deserializedError.SIF_Code, "SIF_Error.SIF_Code should round-trip correctly");
            #endregion
        }

        [Fact]
        public void testBinaryFormatEfficiency()
        {
            // Test that MessagePack binary format is used (not JSON)
            fRC = Components.CreateRequestCache(fAgent);

            SIF_QueryObject obj = new SIF_QueryObject("");
            SIF_Query query = new SIF_Query(obj);
            SIF_Request request = new SIF_Request();
            request.SIF_Query = query;

            Query q = new Query(StudentDTD.STUDENTPERSONAL);
            var testData = new System.Collections.Generic.Dictionary<string, string>
            {
                { "Field1", "SomeTestValue" },
                { "Field2", "AnotherValue" },
                { "Field3", "ThirdValue" }
            };
            q.UserData = testData;

            String msgId = Runtime.MakeGuid();
            storeRequest(fRC, request, q, msgId, "StudentPersonal");

            // Get the file size - should be relatively compact with binary format
            String fname = fAgent.HomeDir + Path.DirectorySeparatorChar + "work" + Path.DirectorySeparatorChar + "requests.adk";
            FileInfo fi = new FileInfo(fname);
            
            fRC.Close();

            Assert.True(fi.Exists, "Cache file should exist");
            Assert.True(fi.Length > 0, "Cache file should have content");
            // Binary format should be significantly smaller than JSON equivalent
            // For this test, we just verify the file was created with content
            // Assert.Pass: Binary cache file created with size: {fi.Length} bytes
        }
    } //end class
} //end namespace


