using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VTQT.Core;
using LinqToDB.Data;

namespace VTQT.Data
{
    /// <summary>
    /// Represents an entity repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public partial interface IRepository<TEntity> where TEntity : BaseEntity
    {
        #region Methods

        /// <summary>
        /// Get entity entries by identifiers
        /// </summary>
        /// <param name="ids">Entity entry identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        IList<TEntity> GetByIds(IEnumerable<string> ids);

        /// <summary>
        /// Get entity entries by identifiers
        /// </summary>
        /// <param name="ids">Entity entry identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        Task<IList<TEntity>> GetByIdsAsync(IEnumerable<string> ids);

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        TEntity GetById(object id);

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entry
        /// </returns>
        Task<TEntity> GetByIdAsync(object id);

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        int Insert(TEntity entity);

        Task<int> InsertAsync(TEntity entity);

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        long Insert(IEnumerable<TEntity> entities);

        Task<long> InsertAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Loads the original copy of the entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Copy of the passed entity</returns>
        TEntity LoadOriginalCopy(TEntity entity);

        Task<TEntity> LoadOriginalCopyAsync(TEntity entity);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        int Update(TEntity entity);

        Task<int> UpdateAsync(TEntity entity);

        int Update<TResult>(TEntity entity, Expression<Func<TEntity, TResult>> selector);

        Task<int> UpdateAsync<TResult>(TEntity entity, Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        int Update(IEnumerable<TEntity> entities);

        Task<int> UpdateAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        int Delete(TEntity entity);

        Task<int> DeleteAsync(TEntity entity);

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        int Delete(IEnumerable<TEntity> entities);

        Task<int> DeleteAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        int Delete(Expression<Func<TEntity, bool>> predicate);

        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        int Delete(IEnumerable<object> ids);

        Task<int> DeleteAsync(IEnumerable<object> ids);

        /// <summary>
        /// Executes command using System.Data.CommandType.StoredProcedure command type
        /// and returns results as collection of values of specified type
        /// </summary>
        /// <param name="storeProcedureName">Store procedure name</param>
        /// <param name="dataParameters">Command parameters</param>
        /// <returns>Collection of query result records</returns>
        IList<TEntity> EntityFromSql(string storeProcedureName, params DataParameter[] dataParameters);

        Task<IList<TEntity>> EntityFromSqlAsync(string storeProcedureName, params DataParameter[] dataParameters);

        /// <summary>
        /// Truncates database table
        /// </summary>
        /// <param name="resetIdentity">Performs reset identity column</param>
        int Truncate(bool resetIdentity = false);

        Task<int> TruncateAsync(bool resetIdentity = false);

        #endregion

        #region Properties

        DataConnection DataConnection { get; }

        /// <summary>
        /// Gets a table
        /// </summary>
        IQueryable<TEntity> Table { get; }

        #endregion
    }

    #region Fix Id (DataType, Lowercase) cho các DB khác

    /// <summary>
    /// Represents an entity repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public partial interface IIntRepository<TEntity> where TEntity : BaseIntEntity
    {
        #region Methods

        /// <summary>
        /// Get entity entries by identifiers
        /// </summary>
        /// <param name="ids">Entity entry identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        IList<TEntity> GetByIds(IEnumerable<int> ids);

        /// <summary>
        /// Get entity entries by identifiers
        /// </summary>
        /// <param name="ids">Entity entry identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        Task<IList<TEntity>> GetByIdsAsync(IEnumerable<int> ids);

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        TEntity GetById(object id);

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entry
        /// </returns>
        Task<TEntity> GetByIdAsync(object id);

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        int Insert(TEntity entity);

        Task<int> InsertAsync(TEntity entity);

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        long Insert(IEnumerable<TEntity> entities);

        Task<long> InsertAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Loads the original copy of the entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Copy of the passed entity</returns>
        TEntity LoadOriginalCopy(TEntity entity);

        Task<TEntity> LoadOriginalCopyAsync(TEntity entity);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        int Update(TEntity entity);

        Task<int> UpdateAsync(TEntity entity);

        int Update<TResult>(TEntity entity, Expression<Func<TEntity, TResult>> selector);

        Task<int> UpdateAsync<TResult>(TEntity entity, Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        int Update(IEnumerable<TEntity> entities);

        Task<int> UpdateAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        int Delete(TEntity entity);

        Task<int> DeleteAsync(TEntity entity);

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        int Delete(IEnumerable<TEntity> entities);

        Task<int> DeleteAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        int Delete(Expression<Func<TEntity, bool>> predicate);

        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        int Delete(IEnumerable<object> ids);

        Task<int> DeleteAsync(IEnumerable<object> ids);

        /// <summary>
        /// Executes command using System.Data.CommandType.StoredProcedure command type
        /// and returns results as collection of values of specified type
        /// </summary>
        /// <param name="storeProcedureName">Store procedure name</param>
        /// <param name="dataParameters">Command parameters</param>
        /// <returns>Collection of query result records</returns>
        IList<TEntity> EntityFromSql(string storeProcedureName, params DataParameter[] dataParameters);

        Task<IList<TEntity>> EntityFromSqlAsync(string storeProcedureName, params DataParameter[] dataParameters);

        /// <summary>
        /// Truncates database table
        /// </summary>
        /// <param name="resetIdentity">Performs reset identity column</param>
        int Truncate(bool resetIdentity = false);

        Task<int> TruncateAsync(bool resetIdentity = false);

        #endregion

        #region Properties

        DataConnection DataConnection { get; }

        /// <summary>
        /// Gets a table
        /// </summary>
        IQueryable<TEntity> Table { get; }

        #endregion
    }

    /// <summary>
    /// Represents an entity repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public partial interface IIntLowercaseRepository<TEntity> where TEntity : BaseIntLowercaseEntity
    {
        #region Methods

        /// <summary>
        /// Get entity entries by identifiers
        /// </summary>
        /// <param name="ids">Entity entry identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        IList<TEntity> GetByIds(IEnumerable<int> ids);

        /// <summary>
        /// Get entity entries by identifiers
        /// </summary>
        /// <param name="ids">Entity entry identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        Task<IList<TEntity>> GetByIdsAsync(IEnumerable<int> ids);

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        TEntity GetById(object id);

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entry
        /// </returns>
        Task<TEntity> GetByIdAsync(object id);

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        int Insert(TEntity entity);

        Task<int> InsertAsync(TEntity entity);

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        long Insert(IEnumerable<TEntity> entities);

        Task<long> InsertAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Loads the original copy of the entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Copy of the passed entity</returns>
        TEntity LoadOriginalCopy(TEntity entity);

        Task<TEntity> LoadOriginalCopyAsync(TEntity entity);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        int Update(TEntity entity);

        Task<int> UpdateAsync(TEntity entity);

        int Update<TResult>(TEntity entity, Expression<Func<TEntity, TResult>> selector);

        Task<int> UpdateAsync<TResult>(TEntity entity, Expression<Func<TEntity, TResult>> selector);

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        int Update(IEnumerable<TEntity> entities);

        Task<int> UpdateAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        int Delete(TEntity entity);

        Task<int> DeleteAsync(TEntity entity);

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        int Delete(IEnumerable<TEntity> entities);

        Task<int> DeleteAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        int Delete(Expression<Func<TEntity, bool>> predicate);

        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        int Delete(IEnumerable<object> ids);

        Task<int> DeleteAsync(IEnumerable<object> ids);

        /// <summary>
        /// Executes command using System.Data.CommandType.StoredProcedure command type
        /// and returns results as collection of values of specified type
        /// </summary>
        /// <param name="storeProcedureName">Store procedure name</param>
        /// <param name="dataParameters">Command parameters</param>
        /// <returns>Collection of query result records</returns>
        IList<TEntity> EntityFromSql(string storeProcedureName, params DataParameter[] dataParameters);

        Task<IList<TEntity>> EntityFromSqlAsync(string storeProcedureName, params DataParameter[] dataParameters);

        /// <summary>
        /// Truncates database table
        /// </summary>
        /// <param name="resetIdentity">Performs reset identity column</param>
        int Truncate(bool resetIdentity = false);

        Task<int> TruncateAsync(bool resetIdentity = false);

        #endregion

        #region Properties

        DataConnection DataConnection { get; }

        /// <summary>
        /// Gets a table
        /// </summary>
        IQueryable<TEntity> Table { get; }

        #endregion
    }

    #endregion
}
