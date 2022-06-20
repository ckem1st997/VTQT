using VTQT.Caching;
using System;
using LinqToDB.Mapping;

namespace VTQT.Core
{
    /// <summary>
    /// Base class for entities
    /// </summary>
    [Serializable]
    public abstract partial class BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        [PrimaryKey, NotNull]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Get key for caching the entity
        /// </summary>
        public string EntityCacheKey => GetEntityCacheKey(GetType(), Id);

        /// <summary>
        /// Get key for caching the entity
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="id">Entity id</param>
        /// <returns>Key for caching the entity</returns>
        public static string GetEntityCacheKey(Type entityType, object id)
        {
            return string.Format(CachingDefaults.EntityByIdCacheKey, entityType.Name.ToLower(), id);
        }

        public static string GetEntityPrefix(Type entityType)
        {
            return string.Format(CachingDefaults.EntityPrefix, entityType.Name.ToLower());
        }
    }

    #region Fix Id (DataType, Lowercase) cho các DB khác

    /// <summary>
    /// Base class for entities
    /// </summary>
    [Serializable]
    public abstract partial class BaseIntEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        [PrimaryKey, NotNull]
        public int Id { get; set; }

        /// <summary>
        /// Get key for caching the entity
        /// </summary>
        public string EntityCacheKey => GetEntityCacheKey(GetType(), Id);

        /// <summary>
        /// Get key for caching the entity
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="id">Entity id</param>
        /// <returns>Key for caching the entity</returns>
        public static string GetEntityCacheKey(Type entityType, object id)
        {
            return string.Format(CachingDefaults.EntityByIdCacheKey, entityType.Name.ToLower(), id);
        }

        public static string GetEntityPrefix(Type entityType)
        {
            return string.Format(CachingDefaults.EntityPrefix, entityType.Name.ToLower());
        }
    }

    /// <summary>
    /// Base class for entities
    /// </summary>
    [Serializable]
    public abstract partial class BaseIntLowercaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        [PrimaryKey, NotNull]
        public int id { get; set; }

        /// <summary>
        /// Get key for caching the entity
        /// </summary>
        public string EntityCacheKey => GetEntityCacheKey(GetType(), id);

        /// <summary>
        /// Get key for caching the entity
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="id">Entity id</param>
        /// <returns>Key for caching the entity</returns>
        public static string GetEntityCacheKey(Type entityType, object id)
        {
            return string.Format(CachingDefaults.EntityByIdCacheKey, entityType.Name.ToLower(), id);
        }

        public static string GetEntityPrefix(Type entityType)
        {
            return string.Format(CachingDefaults.EntityPrefix, entityType.Name.ToLower());
        }
    }

    #endregion
}
