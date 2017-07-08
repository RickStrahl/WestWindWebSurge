using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebSurge;

namespace SimpleStressTester.Tests
{
    [TestClass]
    public class UserManagementTests
    {
        [TestMethod]
        public void CreateUserEntries()
        {

            var userList = new List<UserEntry>();

            var user = new UserEntry();            
            var login = new LoginFormEntry
            {
                Url = "http://west-wind.com/wconnect/login.wwd",
                HttpVerb = "POST",
                ContentType = "application/x-www-form-urlencoded",
                FormVariables =
                {
                    new HttpFormVariable
                    {
                        Key = "txtUsername",
                        Value = "rstrahl@west-wind.com",
                    },
                    new HttpFormVariable
                    {
                        Key = "txtPassword",
                        Value = "seekrit",
                    }
                }
            };
            user.LoginUrls.Add(login);
            userList.Add(user);

            user = new UserEntry();
            login = new LoginFormEntry
            {
                Url = "http://west-wind.com/wconnect/login.wwd",
                HttpVerb = "POST",
                ContentType = "application/x-www-form-urlencoded",
                FormVariables =
                {
                    new HttpFormVariable
                    {
                        Key = "txtUsername",
                        Value = "megger@eps-software.com",
                    },
                    new HttpFormVariable
                    {
                        Key = "txtPassword",
                        Value = "seekriteps",
                    }
                }
            };
            user.LoginUrls.Add(login);
            userList.Add(user);

            string file = Path.GetTempFileName();
            UserManager.SaveUsersToJsonFile(userList, file);

            Assert.IsTrue(File.Exists(file));

            string json = File.ReadAllText(file);
            Console.WriteLine(json);

            Assert.IsNotNull(file);

            File.Delete(file);

        }
    }
}
