using SQLite;

namespace OpenFun_Core.Models
{
    /// <summary>
    /// Base class for all database table models.
    /// All derived classes will automatically have an auto-incrementing integer primary key.
    /// Derived classes should define their own additional properties and use sqlite-net-pcl attributes
    /// like [Indexed], [NotNull], etc. to define the table schema.
    /// 
    /// Example:
    /// public class PangramData : DatabaseTable
    /// {
    ///     [Indexed]
    ///     public DateTime Date { get; set; }
    ///     public string LetterSequence { get; set; }
    ///     public string Word { get; set; }
    /// }
    /// </summary>
    public abstract class DatabaseTable
    {
        /// <summary>
        /// Auto-incrementing primary key shared by all derived tables.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Gets the name of the table in the database.
        /// Can be overridden in derived classes to customize the table name using the [Table("TableName")] attribute.
        /// </summary>
        public virtual string TableName => GetType().Name;
    }
}
