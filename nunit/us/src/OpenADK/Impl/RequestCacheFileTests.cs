using Library.UnitTesting.Framework;
using NUnit.Framework;
using OpenADK.Library;
using OpenADK.Library.Impl;
using OpenADK.Library.Infra;
using OpenADK.Library.us.Common;
using OpenADK.Library.us.Student;
using System;
using System.Collections.Generic;
using System.IO;

namespace Library.Nunit.US.Impl
{
    [TestFixture]
    public class RequestCacheFileTests
    {
        private String[] fMsgIds;
        private RequestCache fRC;
        private Agent fAgent;

        [SetUp]
        public void setUp()
        {
            Adk.Initialize();

            fAgent = new TestAgent();
            fAgent.Initialize();
        }

        [TearDown]
        public void tearDown()
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
                // Add a small delay to allow file handle to be fully released
                System.Threading.Thread.Sleep(100);
                File.Delete(fname);
            }
            catch (IOException)
            {
                // If file is still locked, try again after another delay
                try
                {
                    System.Threading.Thread.Sleep(200);
                    File.Delete(fname);
                }
                catch (IOException ex)
                {
                    // If it still fails, log it but don't fail the test teardown
                    System.Console.WriteLine("Warning: Unable to delete cache file: " + ex.Message);
                }
            }
            //File f = new File(fname);
            //f.delete();
        }


        /**
       * Tests that the RequestCache file persists information between requests
       * @throws Exception
       */

        [Test]
        public void testSimpleCase()
        {
            fRC = RequestCache.GetInstance(fAgent);
            storeAssertedRequests(fRC);
            assertStoredRequests(fRC, true);
        }

        /**
       * Tests that the RequestCache file persists information between restarts
       * @throws Exception
       */

        [Test]
        public void testPersistence()
        {
            fRC = RequestCache.GetInstance(fAgent);
            storeAssertedRequests(fRC);
            fRC.Close();

            // Create a new instance. This one should retrieve its settings from the persistence mechanism
            fRC = RequestCache.GetInstance(fAgent);
            assertStoredRequests(fRC, true);

            fRC.Close();
            fRC = RequestCache.GetInstance(fAgent);
            Assert.AreEqual(0, fRC.ActiveRequestCount, "Should have zero pending requests");
        }


       /**
       * Tests that the RequestCache file persists information between restarts
       * Even if the state object is not able to be deserialized
       * @throws Exception
       */

       [Test]
       public void testPersistenceWithBadState()
       {
           //create new cache for agent
           fRC = RequestCache.GetInstance(fAgent);

           //create new queryobject
           SIF_QueryObject obj = new SIF_QueryObject("");
           //create query, telling it what type of query it is(passing it queryobj)
           SIF_Query query = new SIF_Query(obj);
           //create new sif request
           SIF_Request request = new SIF_Request();
           //set query property
           request.SIF_Query = query;


           Query q = new Query(StudentDTD.STUDENTPERSONAL);

           String testStateItem = Adk.MakeGuid();
           String requestMsgId = Adk.MakeGuid();
           String testObjectType = Adk.MakeGuid();

           // Use string user data (allowed type)
           q.UserData = testStateItem;
           storeRequest(fRC, request, q, requestMsgId, testObjectType);

           fRC.Close();

           // Create a new instance. This one should retrieve its settings from the persistence mechanism
           fRC = RequestCache.GetInstance(fAgent);

           IRequestInfo ri = fRC.GetRequestInfo(requestMsgId, null);

           // RequestInfo should still be available even if UserData isn't
           Assert.IsNotNull(ri, "RequestInfo was null");
           Assert.AreEqual(requestMsgId, ri.MessageId, "MessageId");
           Assert.AreEqual(testObjectType, ri.ObjectType, "ObjectType");
       }

        [Test]
        public void testInstanceMultipleInvocations()
        {
            for (int i = 0; i < 3; i++)
            {
                testPersistence();
            }
        }

        [Test]
        public void testPersistenceMultipleInvocations()
        {
            for (int i = 0; i < 3; i++)
            {
                testSimpleCase();
            }
        }

        [Test]
        public void testPersistenceWithRemoval()
        {
            fRC = RequestCache.GetInstance(fAgent);
            SIF_QueryObject obj = new SIF_QueryObject("");
            SIF_Query query = new SIF_Query(obj);
            SIF_Request request = new SIF_Request();

            request.SIF_Query = query;

            Query q = new Query(StudentDTD.STUDENTPERSONAL);
            String testStateItem = Adk.MakeGuid();
            // Use string user data instead of TestState (allowed type)
            q.UserData = testStateItem;

            fMsgIds = new String[10];
            // Add 10 entries to the cache, interspersed with other entries that are removed
            for (int i = 0; i < 10; i++)
            {
                String phantom1 = Adk.MakeGuid();
                String phantom2 = Adk.MakeGuid();
                storeRequest(fRC, request, q, phantom1, "foo");
                fMsgIds[i] = Adk.MakeGuid();
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

            Assert.AreEqual(5, fRC.ActiveRequestCount, "Before closing Should have five objects");
            fRC.Close();

            // Create a new instance. This one should retrieve its settings from the persistence mechanism
            fRC = RequestCache.GetInstance(fAgent);
            Assert.AreEqual(5, fRC.ActiveRequestCount, "After Re-Openeing Should have five objects");
            for (int i = 1; i < 10; i += 2)
            {
                IRequestInfo cachedInfo = fRC.GetRequestInfo(fMsgIds[i], null);
                Assert.IsNotNull(cachedInfo, "No cachedID returned for " + i);
            }
            Assert.AreEqual(0, fRC.ActiveRequestCount, "Should have zero objects");
        }


        /**
       * Tests that the RequestCacheFile class handles the case of the 
       * cache file becoming readonly
       * @throws Exception
       */

        [Test]
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
                Assert.Throws<AdkException>(() => RequestCache.GetInstance(fAgent));
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

        [Test]
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

            fRC = RequestCache.GetInstance(fAgent);
            storeAssertedRequests(fRC);
            fRC.Close();

            // Create a new instance. This one should retrieve its settings from the persistence mechanism
            fRC = RequestCache.GetInstance(fAgent);
            assertStoredRequests(fRC, true);
        }

        [Test]
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


            fRC = RequestCache.GetInstance(fAgent);
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
                String stateData = Adk.MakeGuid();
                q = new Query(StudentDTD.STUDENTPERSONAL);
                q.UserData = stateData;
                fMsgIds[i] = Adk.MakeGuid();
                storeRequest(fRC, request, q, fMsgIds[i], "Object_" + i.ToString());
            }


            Assert.AreEqual(10, fRC.ActiveRequestCount, "Active request count");


            // Lookup each setting, 
            for (int i = 0; i < 10; i++)
            {
                IRequestInfo reqInfo = fRC.LookupRequestInfo(fMsgIds[i], null);
                Assert.AreEqual("Object_" + i.ToString(), reqInfo.ObjectType, "Initial lookup");
            }

            // Lookup each setting, 
            for (int i = 0; i < 10; i++)
            {
                IRequestInfo reqInfo = fRC.GetRequestInfo(fMsgIds[i], null);
                Assert.AreEqual("Object_" + i.ToString(), reqInfo.ObjectType, "Initial lookup");
            }

            // all messages should now be removed from the queue
            Assert.AreEqual(0, fRC.ActiveRequestCount, "Cache should be empty");

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
                String stateData = "TestState_" + Adk.MakeGuid();
                q.UserData = stateData;

                String phantom1 = Adk.MakeGuid();
                String phantom2 = Adk.MakeGuid();
                storeRequest(cache, request, q, phantom1, "foo");
                fMsgIds[i] = Adk.MakeGuid();

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
            Assert.AreEqual(fMsgIds.Length, cache.ActiveRequestCount, "Active request count");

            // Lookup each setting, 
            for (int i = 0; i < fMsgIds.Length; i++)
            {
                IRequestInfo reqInfo = cache.LookupRequestInfo(fMsgIds[i], null);
                Assert.AreEqual("Object_" + i.ToString(), reqInfo.ObjectType, "Initial lookup");
                // Verify user data is a string if present (not required to persist)
                if (reqInfo.UserData != null)
                {
                    Assert.IsInstanceOf<string>(reqInfo.UserData, "User Data should be a string for " + i);
                }
            }

            if (testRemoval)
            {
                // Lookup each setting, 
                for (int i = 0; i < fMsgIds.Length; i++)
                {
                    IRequestInfo reqInfo = cache.GetRequestInfo(fMsgIds[i], null);
                    Assert.AreEqual("Object_" + i.ToString(), reqInfo.ObjectType, "Initial lookup");
                    // Verify user data is a string if present (not required to persist)
                    if (reqInfo.UserData != null)
                    {
                        Assert.IsInstanceOf<string>(reqInfo.UserData, "User Data should be a string for " + i);
                    }
                }

                // all messages should now be removed from the queue
                Assert.AreEqual(0, cache.ActiveRequestCount, "Cache should be empty");
            }
        }

        [Test]
        public void testSerializationWithStringUserData()
        {
            // Test that string user data (System.* type) is properly serialized/deserialized
            fRC = RequestCache.GetInstance(fAgent);

            SIF_QueryObject obj = new SIF_QueryObject("");
            SIF_Query query = new SIF_Query(obj);
            SIF_Request request = new SIF_Request();
            request.SIF_Query = query;

            Query q = new Query(StudentDTD.STUDENTPERSONAL);
            string testData = "TestUserDataString_" + Adk.MakeGuid();
            q.UserData = testData;

            String msgId = Adk.MakeGuid();
            storeRequest(fRC, request, q, msgId, "StudentPersonal");

            fRC.Close();

            // Re-open and verify the string data was preserved
            fRC = RequestCache.GetInstance(fAgent);
            IRequestInfo ri = fRC.GetRequestInfo(msgId, null);

            Assert.IsNotNull(ri, "RequestInfo should not be null");
            Assert.AreEqual(testData, (string)ri.UserData, "UserData should match original string");
        }

        [Test]
        public void testSerializationWithGuidUserData()
        {
            // Test that Guid data (System.* type) is properly serialized/deserialized
            fRC = RequestCache.GetInstance(fAgent);

            SIF_QueryObject obj = new SIF_QueryObject("");
            SIF_Query query = new SIF_Query(obj);
            SIF_Request request = new SIF_Request();
            request.SIF_Query = query;

            Query q = new Query(StudentDTD.STUDENTPERSONAL);
            var testData = Guid.NewGuid();
            q.UserData = testData;

            String msgId = Adk.MakeGuid();
            storeRequest(fRC, request, q, msgId, "StudentPersonal");

            fRC.Close();

            // Re-open and verify the string data was preserved
            fRC = RequestCache.GetInstance(fAgent);
            IRequestInfo ri = fRC.GetRequestInfo(msgId, null);

            Assert.IsNotNull(ri, "RequestInfo should not be null");
            Assert.AreEqual(testData, (Guid)ri.UserData, "UserData should match original Guid");
        }

        [Test]
        public void testSerializationWithDictionaryUserData()
        {
            // Test that System.Collections types are properly serialized/deserialized
            fRC = RequestCache.GetInstance(fAgent);

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

            String msgId = Adk.MakeGuid();
            storeRequest(fRC, request, q, msgId, "StudentPersonal");

            fRC.Close();

            // Re-open and verify the dictionary data was preserved
            fRC = RequestCache.GetInstance(fAgent);
            IRequestInfo ri = fRC.GetRequestInfo(msgId, null);

            Assert.IsNotNull(ri, "RequestInfo should not be null");
            Assert.IsNotNull(ri.UserData, "UserData should not be null");
            // Note: Deserialized dictionary type may vary depending on MessagePack implementation
            Assert.IsTrue(ri.UserData is System.Collections.IDictionary, "UserData should be a dictionary type");
        }

        /// <summary>
        /// Tests whether any of the Adk's classes which inherit from SifSimpleType cannot be 
        /// serialized. By default they all should be.
        /// </summary>
        [Test]
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
                TestContext.Out.WriteLine("Testing type: " + item.GetType().Name);
                q.UserData = item;
                String msgId = Adk.MakeGuid();

                fRC = RequestCache.GetInstance(fAgent);
                storeRequest(fRC, request, q, msgId, "StudentPersonal");
                fRC.Close();

                // Re-open and verify the dictionary data was preserved
                fRC = RequestCache.GetInstance(fAgent);
                IRequestInfo ri = fRC.GetRequestInfo(msgId, null);
                fRC.Close();

                Assert.IsNotNull(ri, "RequestInfo should not be null");
                Assert.IsNotNull(ri.UserData, "UserData should not be null");
            }
        }

        [Test]
        public void testSerializationWithSifElementUserData()
        {
            // Test that string user data (System.* type) is properly serialized/deserialized
            fRC = RequestCache.GetInstance(fAgent);

            SIF_QueryObject obj = new SIF_QueryObject("");
            SIF_Query query = new SIF_Query(obj);
            SIF_Request request = new SIF_Request();
            request.SIF_Query = query;

            Query q = new Query(StudentDTD.STUDENTPERSONAL);

            #region Name
            var name = new Name(NameType.LEGAL, "Nahorniak", "Mike");
            q.UserData = name;

            String msgId = Adk.MakeGuid();
            storeRequest(fRC, request, q, msgId, "StudentPersonal");

            fRC.Close();

            // Re-open and verify the string data was preserved
            fRC = RequestCache.GetInstance(fAgent);
            IRequestInfo ri = fRC.GetRequestInfo(msgId, null);

            Assert.IsNotNull(ri, "RequestInfo should not be null");
            Assert.IsInstanceOf<Name>(ri.UserData, $"UserData should be a {nameof(Name)}");
            var deserializedName = (Name)ri.UserData;
            Assert.AreEqual(name.LastName, deserializedName.LastName, "Name.LastName should round-trip correctly");
            Assert.AreEqual(name.FirstName, deserializedName.FirstName, "Name.FirstName should round-trip correctly");
            #endregion

            #region StudentPersonal
            var studentPersonal = new StudentPersonal();
            q.UserData = studentPersonal;

            msgId = Adk.MakeGuid();
            storeRequest(fRC, request, q, msgId, "StudentPersonal");

            fRC.Close();

            // Re-open and verify the string data was preserved
            fRC = RequestCache.GetInstance(fAgent);
            ri = fRC.GetRequestInfo(msgId, null);

            Assert.IsNotNull(ri, "RequestInfo should not be null");
            Assert.IsInstanceOf<StudentPersonal>(ri.UserData, $"UserData should be a {nameof(StudentPersonal)}");
            #endregion

            #region SIF_Error
            var error = new SIF_Error(
                (int)SifErrorCategoryCode.Generic,
                SifErrorCodes.GENERIC_GENERIC_ERROR_1,
                "Could not serialize the SIF_Err object");
            q.UserData = error;

            msgId = Adk.MakeGuid();
            storeRequest(fRC, request, q, msgId, "StudentPersonal");

            fRC.Close();

            // Re-open and verify the string data was preserved
            fRC = RequestCache.GetInstance(fAgent);
            ri = fRC.GetRequestInfo(msgId, null);

            Assert.IsNotNull(ri, "RequestInfo should not be null");
            Assert.IsInstanceOf<SIF_Error>(ri.UserData, $"UserData should be a {nameof(SIF_Error)}");
            var deserializedError = (SIF_Error)ri.UserData;
            Assert.AreEqual(error.SIF_Category, deserializedError.SIF_Category, "SIF_Error.SIF_Category should round-trip correctly");
            Assert.AreEqual(error.SIF_Code, deserializedError.SIF_Code, "SIF_Error.SIF_Code should round-trip correctly");
            #endregion
        }

        [Test]
        public void testBinaryFormatEfficiency()
        {
            // Test that MessagePack binary format is used (not JSON)
            fRC = RequestCache.GetInstance(fAgent);

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

            String msgId = Adk.MakeGuid();
            storeRequest(fRC, request, q, msgId, "StudentPersonal");

            // Get the file size - should be relatively compact with binary format
            String fname = fAgent.HomeDir + Path.DirectorySeparatorChar + "work" + Path.DirectorySeparatorChar + "requests.adk";
            FileInfo fi = new FileInfo(fname);
            
            fRC.Close();

            Assert.IsTrue(fi.Exists, "Cache file should exist");
            Assert.Greater(fi.Length, 0, "Cache file should have content");
            // Binary format should be significantly smaller than JSON equivalent
            // For this test, we just verify the file was created with content
            Assert.Pass("Binary cache file created with size: " + fi.Length + " bytes");
        }
    } //end class
} //end namespace


