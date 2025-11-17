using OpenFun_Core.Models;
using SQLite;
using System.Linq.Expressions;

namespace OpenFun_Core.Services
{
    /// <summary>
    /// SQLite-backed data service using sqlite-net-pcl that works with DatabaseTable-derived objects.
    /// The service creates tables dynamically based on the provided table type and performs CRUD operations
    /// using the TableName property. Sub-applications should create specific DatabaseTable-derived classes
    /// to define their data schema (e.g., PangramData : DatabaseTable with Date, LetterSequence, Word properties).
    /// </summary>
    public class DatabaseService
    {
        private readonly string _databasePath;
        private readonly SQLiteAsyncConnection _db;

        public DatabaseService(string databaseName = "OpenFun.db")
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenFun");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            _databasePath = Path.Combine(folder, databaseName);
            _db = new SQLiteAsyncConnection(_databasePath);
        }

        /// <summary>
        /// Initializes a table for the specified DatabaseTable type.
        /// Creates the table schema based on the type definition.
        /// </summary>
        /// <typeparam name="T">Type derived from DatabaseTable.</typeparam>
        public async Task InitializeTableAsync<T>() where T : class, new()
        {
            await _db.CreateTableAsync<T>();
        }

        /// <summary>
        /// Saves the provided DatabaseTable object to the database.
        /// Inserts or replaces the object in the corresponding table.
        /// </summary>
        /// <typeparam name="T">Type derived from DatabaseTable.</typeparam>
        /// <param name="data">The DatabaseTable object to save.</param>
        public async Task SaveAsync<T>(T data) where T : class, new()
        {
            await InitializeTableAsync<T>();
            await _db.InsertOrReplaceAsync(data);
        }

        /// <summary>
        /// Adds a new record or updates an existing one based on the provided predicate.
        /// If an existing row matches the predicate, the Id from that row will be copied to the provided
        /// object and the object will be updated (insert or replace). If no match is found, the object will be inserted.
        /// </summary>
        /// <typeparam name="T">Type derived from DatabaseTable.</typeparam>
        /// <param name="data">Object to add or update.</param>
        /// <param name="predicate">Predicate to identify existing record(s).</param>
        public async Task<T> AddOrUpdateAsync<T>(T data, Expression<Func<T, bool>> predicate) where T : DatabaseTable, new()
        {
            await InitializeTableAsync<T>();

            T? existing = await _db.Table<T>().Where(predicate).FirstOrDefaultAsync();
            if (existing != null)
            {
                // Copy Id from existing to data so InsertOrReplace will update the row
                data.Id = existing.Id;
                await _db.UpdateAsync(data);
            }
            else
            {
                await _db.InsertAsync(data);
            }

            return data;
        }

        /// <summary>
        /// Retrieves a single object from the database by primary key.
        /// </summary>
        /// <typeparam name="T">Type derived from DatabaseTable.</typeparam>
        /// <param name="primaryKey">The primary key value to retrieve.</param>
        /// <returns>The retrieved object or null if not found.</returns>
        public async Task<T?> RetrieveAsync<T>(object primaryKey) where T : class, new()
        {
            await InitializeTableAsync<T>();
            return await _db.FindAsync<T>(primaryKey);
        }

        /// <summary>
        /// Retrieves the first object matching the provided predicate using server-side translation where possible.
        /// </summary>
        public async Task<T?> RetrieveFirstAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            await InitializeTableAsync<T>();
            // sqlite-net supports expression-based Where which gets translated to SQL where possible
            return await _db.Table<T>().Where(predicate).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves objects from the table matching the given expression predicate (translated to SQL when supported).
        /// </summary>
        /// <typeparam name="T">Type derived from DatabaseTable.</typeparam>
        /// <param name="predicate">Expression predicate to filter results.</param>
        /// <returns>Collection of objects matching the predicate.</returns>
        public async Task<IEnumerable<T>> RetrieveAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            await InitializeTableAsync<T>();
            return await _db.Table<T>().Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Executes a raw SQL query and maps the results to the specified type.
        /// Use this for complex queries that cannot be expressed with expressions.
        /// </summary>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, params object[] args) where T : class, new()
        {
            await InitializeTableAsync<T>();
            return await _db.QueryAsync<T>(sql, args);
        }

        /// <summary>
        /// Retrieves all objects from the table of type T.
        /// </summary>
        /// <typeparam name="T">Type derived from DatabaseTable.</typeparam>
        /// <returns>Collection of all objects in the table.</returns>
        public async Task<IEnumerable<T>> RetrieveAllAsync<T>() where T : class, new()
        {
            await InitializeTableAsync<T>();
            return await _db.Table<T>().ToListAsync();
        }

        /// <summary>
        /// Deletes a single object from the database.
        /// </summary>
        /// <typeparam name="T">Type derived from DatabaseTable.</typeparam>
        /// <param name="data">The object to delete.</param>
        public async Task DeleteAsync<T>(T data) where T : class, new()
        {
            await InitializeTableAsync<T>();
            await _db.DeleteAsync(data);
        }

        /// <summary>
        /// Deletes an object by primary key.
        /// </summary>
        /// <typeparam name="T">Type derived from DatabaseTable.</typeparam>
        /// <param name="primaryKey">The primary key value to delete.</param>
        public async Task DeleteAsync<T>(object primaryKey) where T : class, new()
        {
            await InitializeTableAsync<T>();
            await _db.DeleteAsync<T>(primaryKey);
        }

        /// <summary>
        /// Deletes all objects from the table.
        /// </summary>
        /// <typeparam name="T">Type derived from DatabaseTable.</typeparam>
        public async Task DeleteAllAsync<T>() where T : class, new()
        {
            await InitializeTableAsync<T>();
            await _db.DeleteAllAsync<T>();
        }

        /// <summary>
        /// Gets the count of records in the table.
        /// </summary>
        /// <typeparam name="T">Type derived from DatabaseTable.</typeparam>
        /// <returns>The number of records in the table.</returns>
        public async Task<int> GetCountAsync<T>() where T : class, new()
        {
            await InitializeTableAsync<T>();
            return await _db.Table<T>().CountAsync();
        }

        /// <summary>
        /// Gets the count of records matching the given predicate.
        /// </summary>
        public async Task<int> GetCountAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            await InitializeTableAsync<T>();
            return await _db.Table<T>().Where(predicate).CountAsync();
        }
    }
}