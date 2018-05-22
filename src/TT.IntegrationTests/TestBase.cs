using Highway.Data;
using NUnit.Framework;
using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using TT.Domain;

namespace TT.IntegrationTests
{
    [TestFixture]
    public abstract class TestBase
    {
        public string ConnectionString { get; private set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var dbContext = new DomainContext();
            DomainRegistry.Repository = new DomainRepository(dbContext);

            ConnectionString = dbContext.Database.Connection.ConnectionString;
        }

        [SetUp]
        public abstract void SetUp();

        [TearDown]
        public abstract void TearDown();

        /// <summary>
        /// Loads a CSV file into the database given a table name which will be used to search for the file by name.
        /// The directory searched would be the location of the testing class given its namespace.
        /// </summary>
        /// <param name="table">The table to look for and to upload the data into.</param>
        /// <param name="subDir">Any additional sub directories to seach into.</param>
        protected void LoadTableFromCSV(string table, string subDir = "")
        {
            using (ICSVLoader loader = new CSVLoader(ConnectionString))
            {
                loader.LoadCSVFromTableAndNamespace(this, table, subDir);
            }
        }

        /// <summary>
        /// Asynchronous loads a CSV file into the database given a table name which will be used to search for the file by name.
        /// The directory searched would be the location of the testing class given its namespace.
        /// </summary>
        /// <param name="table">The table to look for and to upload the data into.</param>
        /// <param name="subDir">Any additional sub directories to seach into.</param>
        protected async Task LoadTableFromCSVAsync(string table, string subDir = "")
        {
            using (ICSVLoader loader = new CSVLoader(ConnectionString))
            {
                await loader.LoadCSVFromTableAndNamespaceAsync(this, table, subDir);
            }
        }

        /// <summary>
        /// Loads a CSV file into the database with the table name being the file name.
        /// </summary>
        /// <param name="path">The absolute path of the file.</param>
        protected void LoadTableFromAbsoluteCSVPath(string path)
        {
            using (ICSVLoader loader = new CSVLoader(ConnectionString))
            {
                loader.LoadCSVFromAbsolutePath(path);
            }
        }

        /// <summary>
        /// Asynchronous loads a CSV file into the database with the table name being the file name.
        /// </summary>
        /// <param name="path">The absolute path of the file.</param>
        protected async Task LoadTableFromAbsoluteCSVPathAsync(string path)
        {
            using (ICSVLoader loader = new CSVLoader(ConnectionString))
            {
                await loader.LoadCSVFromAbsolutePathAsync(path);
            }
        }
    }
}
