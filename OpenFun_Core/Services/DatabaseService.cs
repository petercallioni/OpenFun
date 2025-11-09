using SQLite;
using System.Text.Json;

namespace OpenFun_Core.Services
{
    /// <summary>
    /// Simple SQLite-backed generic data store using sqlite-net-pcl.
    /// Note: when calling SaveAsync/RetrieveAsync/DeleteAsync use the calling sub-application's
    /// name (for example "Pangram") as the id parameter so each game can store and retrieve
    /// its own data independently.
    /// </summary>
    public class DatabaseService
    {
        private readonly string _databasePath;
        private readonly SQLiteAsyncConnection _db;

        public DatabaseService(string databaseName = "OpenFun.db")
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _databasePath = Path.Combine(folder, databaseName);
            _db = new SQLiteAsyncConnection(_databasePath);
            InitializeDatabaseAsync().ContinueWith(t => { if (t.Exception != null) System.Diagnostics.Debug.WriteLine(t.Exception); });
        }

        private async Task InitializeDatabaseAsync()
        {
            await _db.CreateTableAsync<DataStoreEntry>();
        }

        /// <summary>
        /// Saves the provided data under the specified id.
        /// The id should be the calling sub-application's name (e.g. "Pangram") so each game
        /// can save its own state independently.
        /// </summary>
        /// <typeparam name="T">Type of the data to save.</typeparam>
        /// <param name="id">Identifier for the stored data. Use the sub-application name (e.g. "Pangram").</param>
        /// <param name="data">The data to save.</param>
        public async Task SaveAsync<T>(string id, T data)
        {
            var jsonData = JsonSerializer.Serialize(data);
            var entry = new DataStoreEntry { Id = id, Data = jsonData };

            // Insert or replace
            await _db.InsertOrReplaceAsync(entry);
        }

        /// <summary>
        /// Retrieves data previously saved under the specified id.
        /// The id should be the calling sub-application's name (e.g. "Pangram").
        /// </summary>
        /// <typeparam name="T">Type to deserialize the stored data into.</typeparam>
        /// <param name="id">Identifier used when saving the data. Use the sub-application name (e.g. "Pangram").</param>
        /// <returns>The deserialized data or default(T) if not found.</returns>
        public async Task<T?> RetrieveAsync<T>(string id)
        {
            var entry = await _db.FindAsync<DataStoreEntry>(id);
            if (entry == null || string.IsNullOrWhiteSpace(entry.Data))
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(entry.Data);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Deletes the entry with the given id. Use the sub-application name (e.g. "Pangram")
        /// to remove that application's saved data.
        /// </summary>
        /// <param name="id">Identifier used when saving the data. Use the sub-application name (e.g. "Pangram").</param>
        public async Task DeleteAsync(string id)
        {
            var entry = await _db.FindAsync<DataStoreEntry>(id);
            if (entry != null)
                await _db.DeleteAsync(entry);
        }

        /// <summary>
        /// Returns all stored ids. Typically these will correspond to sub-application names
        /// that have saved data (e.g. "Pangram").
        /// </summary>
        /// <returns>Collection of stored ids.</returns>
        public async Task<IEnumerable<string>> GetAllIdsAsync()
        {
            var entries = await _db.Table<DataStoreEntry>().ToListAsync();
            return entries.Select(e => e.Id);
        }

        [Table("DataStore")]
        public class DataStoreEntry
        {
            [PrimaryKey]
            public string Id { get; set; } = string.Empty;

            public string Data { get; set; } = "{}";
        }
    }
}