using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using VTQT.Core;
using VTQT.Core.Domain.Localization;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Linq;
using LinqToDB.Tools;
using VTQT.Core.Infrastructure;

namespace VTQT.Data
{
    /// <summary>
    /// Represents the Entity repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public partial class EntityRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        #region Fields

        private readonly DataConnection _dataConnection;
        private ITable<TEntity> _entities;

        #endregion

        #region Ctor

        public EntityRepository(DataConnection dataConnection)
        {
            _dataConnection = dataConnection;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get entity entries by identifiers
        /// </summary>
        /// <param name="ids">Entity entry identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        public virtual IList<TEntity> GetByIds(IEnumerable<string> ids)
        {
            if (!ids?.Any() ?? true)
                return new List<TEntity>();

            return Entities.Where(w => ids.Contains(w.Id)).ToList();
        }

        /// <summary>
        /// Get entity entries by identifiers
        /// </summary>
        /// <param name="ids">Entity entry identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        public virtual async Task<IList<TEntity>> GetByIdsAsync(IEnumerable<string> ids)
        {
            if (!ids?.Any() ?? true)
                return new List<TEntity>();

            return await Entities.Where(w => ids.Contains(w.Id)).ToListAsync();
        }

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual TEntity GetById(object id)
        {
            return Entities.FirstOrDefault(e => e.Id == (string)id);
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            return await Entities.FirstOrDefaultAsync(e => e.Id == (string)id);
        }

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual int Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return _dataConnection.Insert(entity);
        }

        public virtual async Task<int> InsertAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return await _dataConnection.InsertAsync(entity);
        }

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual long Insert(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = 0L;
            using (var transaction = new TransactionScope())
            {
                var status = _dataConnection.BulkCopy(new BulkCopyOptions(), entities);
                result = status.RowsCopied;
                transaction.Complete();
            }

            return result;
        }

        public virtual async Task<long> InsertAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = 0L;
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var status = await _dataConnection.BulkCopyAsync(new BulkCopyOptions(), entities);
                result = status.RowsCopied;
                transaction.Complete();
            }

            return result;
        }

        /// <summary>
        /// Loads the original copy of the entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Copy of the passed entity</returns>
        public virtual TEntity LoadOriginalCopy(TEntity entity)
        {
            return _dataConnection.GetTable<TEntity>()
                .FirstOrDefault(e => e.Id == entity.Id);
        }

        public virtual async Task<TEntity> LoadOriginalCopyAsync(TEntity entity)
        {
            return await _dataConnection.GetTable<TEntity>()
                .FirstOrDefaultAsync(e => e.Id == entity.Id);
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual int Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return _dataConnection.Update(entity);
        }

        public virtual async Task<int> UpdateAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return await _dataConnection.UpdateAsync(entity);
        }

        public virtual int Update<TResult>(TEntity entity, Expression<Func<TEntity, TResult>> selector)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var query = Entities.Where(w => w.Id == entity.Id);
            IUpdatable<TEntity> cmd = null;
            foreach (var p in selector.ReturnType.GetProperties())
            {
                var propValue = p.GetValue(entity, null);
                cmd = cmd == null
                    ? query.Set(x => Sql.Property<object>(x, p.Name), propValue)
                    : cmd.Set(x => Sql.Property<object>(x, p.Name), propValue);
            }

            return cmd?.Update() ?? 0;
        }

        public virtual async Task<int> UpdateAsync<TResult>(TEntity entity, Expression<Func<TEntity, TResult>> selector)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var query = Entities.Where(w => w.Id == entity.Id);
            IUpdatable<TEntity> cmd = null;
            foreach (var p in selector.ReturnType.GetProperties())
            {
                var propValue = p.GetValue(entity, null);
                cmd = cmd == null
                    ? query.Set(x => Sql.Property<object>(x, p.Name), propValue)
                    : cmd.Set(x => Sql.Property<object>(x, p.Name), propValue);
            }

            return cmd != null ? await cmd.UpdateAsync() : 0;
        }

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual int Update(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = 0;
            foreach (var entity in entities)
            {
                result += Update(entity);
            }

            return result;
        }

        public virtual async Task<int> UpdateAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = 0;
            foreach (var entity in entities)
            {
                result += await UpdateAsync(entity);
            }

            return result;
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual int Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var result = _dataConnection.Delete(entity);

            if (typeof(ILocalizedEntity).IsAssignableFrom(typeof(TEntity)))
                DeleteLocalizedEntities(new[] { entity.Id });

            return result;
        }

        public virtual async Task<int> DeleteAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var result = await _dataConnection.DeleteAsync(entity);

            if (typeof(ILocalizedEntity).IsAssignableFrom(typeof(TEntity)))
                await DeleteLocalizedEntitiesAsync(new[] { entity.Id });

            return result;
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual int Delete(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = _dataConnection.GetTable<TEntity>()
                .Where(e => e.Id.In(entities.Select(x => x.Id)))
                .Delete();

            if (typeof(ILocalizedEntity).IsAssignableFrom(typeof(TEntity)))
                DeleteLocalizedEntities(entities.Select(s => s.Id));

            return result;
        }

        public virtual async Task<int> DeleteAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = await _dataConnection.GetTable<TEntity>()
                .Where(e => e.Id.In(entities.Select(x => x.Id)))
                .DeleteAsync();

            if (typeof(ILocalizedEntity).IsAssignableFrom(typeof(TEntity)))
                await DeleteLocalizedEntitiesAsync(entities.Select(s => s.Id));

            return result;
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        public virtual int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return _dataConnection.GetTable<TEntity>()
                .Where(predicate)
                .Delete();

            // DeleteLocalizedEntities manually by LocalizedEntityService
        }

        public virtual async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _dataConnection.GetTable<TEntity>()
                .Where(predicate)
                .DeleteAsync();

            // DeleteLocalizedEntities manually by LocalizedEntityService
        }

        public virtual int Delete(IEnumerable<object> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = _dataConnection.GetTable<TEntity>()
                .Where(e => e.Id.In(ids))
                .Delete();

            if (typeof(ILocalizedEntity).IsAssignableFrom(typeof(TEntity)))
                DeleteLocalizedEntities(ids);

            return result;
        }

        public virtual async Task<int> DeleteAsync(IEnumerable<object> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _dataConnection.GetTable<TEntity>()
                .Where(e => e.Id.In(ids))
                .DeleteAsync();

            if (typeof(ILocalizedEntity).IsAssignableFrom(typeof(TEntity)))
                await DeleteLocalizedEntitiesAsync(ids);

            return result;
        }

        /// <summary>
        /// Executes command using System.Data.CommandType.StoredProcedure command type
        /// and returns results as collection of values of specified type
        /// </summary>
        /// <param name="storeProcedureName">Store procedure name</param>
        /// <param name="dataParameters">Command parameters</param>
        /// <returns>Collection of query result records</returns>
        public virtual IList<TEntity> EntityFromSql(string storeProcedureName, params DataParameter[] dataParameters)
        {
            var command = new CommandInfo(_dataConnection, storeProcedureName, dataParameters?.ToArray() ?? Array.Empty<DataParameter>());
            var rez = command.QueryProc<TEntity>()?.ToList();
            UpdateOutputParameters(_dataConnection, dataParameters);
            return rez ?? new List<TEntity>();
        }

        public virtual async Task<IList<TEntity>> EntityFromSqlAsync(string storeProcedureName, params DataParameter[] dataParameters)
        {
            var command = new CommandInfo(_dataConnection, storeProcedureName, dataParameters?.ToArray() ?? Array.Empty<DataParameter>());
            var rez = (await command.QueryProcAsync<TEntity>())?.ToList();
            UpdateOutputParameters(_dataConnection, dataParameters);
            return rez ?? new List<TEntity>();
        }

        /// <summary>
        /// Truncates database table
        /// </summary>
        /// <param name="resetIdentity">Performs reset identity column</param>
        public virtual int Truncate(bool resetIdentity = false)
        {
            return _dataConnection.GetTable<TEntity>().Truncate(resetIdentity);
        }

        public virtual async Task<int> TruncateAsync(bool resetIdentity = false)
        {
            return await _dataConnection.GetTable<TEntity>().TruncateAsync(resetIdentity);
        }

        public virtual int DeleteLocalizedEntities(IEnumerable<object> entityIds)
        {
            var entityType = typeof(TEntity);
            var entityName = entityType.Name;

            var masterConn = EngineContext.Current.Resolve<DataConnection>(DataConnectionHelper.ConnectionStringNames.Master);

            //var result = _dataConnection.GetTable<LocalizedProperty>()
            var result = masterConn.GetTable<BaseEntity>().TableName("LocalizedProperty")
                //.Where(w => w.LocaleKeyGroup == entityName && w.EntityId.In(entityIds))
                .Where(w => Sql.Property<string>(w, "LocaleKeyGroup") == entityName
                    && Sql.Property<string>(w, "EntityId").In(entityIds))
                .Delete();

            return result;
        }

        public virtual async Task<int> DeleteLocalizedEntitiesAsync(IEnumerable<object> entityIds)
        {
            var entityType = typeof(TEntity);
            var entityName = entityType.Name;

            var masterConn = EngineContext.Current.Resolve<DataConnection>(DataConnectionHelper.ConnectionStringNames.Master);

            //var result = await _dataConnection.GetTable<LocalizedProperty>()
            var result = await masterConn.GetTable<BaseEntity>().TableName("LocalizedProperty")
                //.Where(w => w.LocaleKeyGroup == entityName && w.EntityId.In(entityIds))
                .Where(w => Sql.Property<string>(w, "LocaleKeyGroup") == entityName
                    && Sql.Property<string>(w, "EntityId").In(entityIds))
                .DeleteAsync();

            return result;
        }

        #endregion

        #region Properties

        public virtual DataConnection DataConnection => _dataConnection;

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<TEntity> Table => Entities;

        /// <summary>
        /// Gets an entity set
        /// </summary>
        protected virtual ITable<TEntity> Entities => _entities ?? (_entities = _dataConnection.GetTable<TEntity>());

        #endregion

        #region Utils

        private void UpdateOutputParameters(DataConnection dataConnection, DataParameter[] dataParameters)
        {
            if (dataParameters is null || dataParameters.Length == 0)
                return;

            foreach (var dataParam in dataParameters.Where(p => p.Direction == ParameterDirection.Output))
            {
                UpdateParameterValue(dataConnection, dataParam);
            }
        }

        private void UpdateParameterValue(DataConnection dataConnection, DataParameter parameter)
        {
            if (dataConnection is null)
                throw new ArgumentNullException(nameof(dataConnection));

            if (parameter is null)
                throw new ArgumentNullException(nameof(parameter));

            if (dataConnection.Command is IDbCommand command &&
                command.Parameters.Count > 0 &&
                command.Parameters.Contains(parameter.Name) &&
                command.Parameters[parameter.Name] is IDbDataParameter param)
            {
                parameter.Value = param.Value;
            }
        }

        #endregion
    }

    #region Fix Id (DataType, Lowercase) cho các DB khác

    /// <summary>
    /// Represents the Entity repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public partial class IntEntityRepository<TEntity> : IIntRepository<TEntity> where TEntity : BaseIntEntity
    {
        #region Fields

        private readonly DataConnection _dataConnection;
        private ITable<TEntity> _entities;

        #endregion

        #region Ctor

        public IntEntityRepository(DataConnection dataConnection)
        {
            _dataConnection = dataConnection;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get entity entries by identifiers
        /// </summary>
        /// <param name="ids">Entity entry identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        public virtual IList<TEntity> GetByIds(IEnumerable<int> ids)
        {
            if (!ids?.Any() ?? true)
                return new List<TEntity>();

            return Entities.Where(w => ids.Contains(w.Id)).ToList();
        }

        /// <summary>
        /// Get entity entries by identifiers
        /// </summary>
        /// <param name="ids">Entity entry identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        public virtual async Task<IList<TEntity>> GetByIdsAsync(IEnumerable<int> ids)
        {
            if (!ids?.Any() ?? true)
                return new List<TEntity>();

            return await Entities.Where(w => ids.Contains(w.Id)).ToListAsync();
        }

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual TEntity GetById(object id)
        {
            return Entities.FirstOrDefault(e => e.Id == (int)id);
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            return await Entities.FirstOrDefaultAsync(e => e.Id == (int)id);
        }

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual int Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return _dataConnection.Insert(entity);
        }

        public virtual async Task<int> InsertAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return await _dataConnection.InsertAsync(entity);
        }

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual long Insert(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = 0L;
            using (var transaction = new TransactionScope())
            {
                var status = _dataConnection.BulkCopy(new BulkCopyOptions(), entities);
                result = status.RowsCopied;
                transaction.Complete();
            }

            return result;
        }

        public virtual async Task<long> InsertAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = 0L;
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var status = await _dataConnection.BulkCopyAsync(new BulkCopyOptions(), entities);
                result = status.RowsCopied;
                transaction.Complete();
            }

            return result;
        }

        /// <summary>
        /// Loads the original copy of the entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Copy of the passed entity</returns>
        public virtual TEntity LoadOriginalCopy(TEntity entity)
        {
            return _dataConnection.GetTable<TEntity>()
                .FirstOrDefault(e => e.Id == entity.Id);
        }

        public virtual async Task<TEntity> LoadOriginalCopyAsync(TEntity entity)
        {
            return await _dataConnection.GetTable<TEntity>()
                .FirstOrDefaultAsync(e => e.Id == entity.Id);
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual int Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return _dataConnection.Update(entity);
        }

        public virtual async Task<int> UpdateAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return await _dataConnection.UpdateAsync(entity);
        }

        public virtual int Update<TResult>(TEntity entity, Expression<Func<TEntity, TResult>> selector)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var query = Entities.Where(w => w.Id == entity.Id);
            IUpdatable<TEntity> cmd = null;
            foreach (var p in selector.ReturnType.GetProperties())
            {
                var propValue = p.GetValue(entity, null);
                cmd = cmd == null
                    ? query.Set(x => Sql.Property<object>(x, p.Name), propValue)
                    : cmd.Set(x => Sql.Property<object>(x, p.Name), propValue);
            }

            return cmd?.Update() ?? 0;
        }

        public virtual async Task<int> UpdateAsync<TResult>(TEntity entity, Expression<Func<TEntity, TResult>> selector)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var query = Entities.Where(w => w.Id == entity.Id);
            IUpdatable<TEntity> cmd = null;
            foreach (var p in selector.ReturnType.GetProperties())
            {
                var propValue = p.GetValue(entity, null);
                cmd = cmd == null
                    ? query.Set(x => Sql.Property<object>(x, p.Name), propValue)
                    : cmd.Set(x => Sql.Property<object>(x, p.Name), propValue);
            }

            return cmd != null ? await cmd.UpdateAsync() : 0;
        }

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual int Update(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = 0;
            foreach (var entity in entities)
            {
                result += Update(entity);
            }

            return result;
        }

        public virtual async Task<int> UpdateAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = 0;
            foreach (var entity in entities)
            {
                result += await UpdateAsync(entity);
            }

            return result;
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual int Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var result = _dataConnection.Delete(entity);

            if (typeof(ILocalizedEntity).IsAssignableFrom(typeof(TEntity)))
                DeleteLocalizedEntities(new[] { entity.Id.ToString() });

            return result;
        }

        public virtual async Task<int> DeleteAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var result = await _dataConnection.DeleteAsync(entity);

            if (typeof(ILocalizedEntity).IsAssignableFrom(typeof(TEntity)))
                await DeleteLocalizedEntitiesAsync(new[] { entity.Id.ToString() });

            return result;
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual int Delete(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = _dataConnection.GetTable<TEntity>()
                .Where(e => e.Id.In(entities.Select(x => x.Id)))
                .Delete();

            if (typeof(ILocalizedEntity).IsAssignableFrom(typeof(TEntity)))
                DeleteLocalizedEntities(entities.Select(s => s.Id.ToString()));

            return result;
        }

        public virtual async Task<int> DeleteAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = await _dataConnection.GetTable<TEntity>()
                .Where(e => e.Id.In(entities.Select(x => x.Id)))
                .DeleteAsync();

            if (typeof(ILocalizedEntity).IsAssignableFrom(typeof(TEntity)))
                await DeleteLocalizedEntitiesAsync(entities.Select(s => s.Id.ToString()));

            return result;
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        public virtual int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return _dataConnection.GetTable<TEntity>()
                .Where(predicate)
                .Delete();

            // DeleteLocalizedEntities manually by LocalizedEntityService
        }

        public virtual async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _dataConnection.GetTable<TEntity>()
                .Where(predicate)
                .DeleteAsync();

            // DeleteLocalizedEntities manually by LocalizedEntityService
        }

        public virtual int Delete(IEnumerable<object> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = _dataConnection.GetTable<TEntity>()
                .Where(e => e.Id.In(ids))
                .Delete();

            if (typeof(ILocalizedEntity).IsAssignableFrom(typeof(TEntity)))
                DeleteLocalizedEntities(ids);

            return result;
        }

        public virtual async Task<int> DeleteAsync(IEnumerable<object> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _dataConnection.GetTable<TEntity>()
                .Where(e => e.Id.In(ids))
                .DeleteAsync();

            if (typeof(ILocalizedEntity).IsAssignableFrom(typeof(TEntity)))
                await DeleteLocalizedEntitiesAsync(ids);

            return result;
        }

        /// <summary>
        /// Executes command using System.Data.CommandType.StoredProcedure command type
        /// and returns results as collection of values of specified type
        /// </summary>
        /// <param name="storeProcedureName">Store procedure name</param>
        /// <param name="dataParameters">Command parameters</param>
        /// <returns>Collection of query result records</returns>
        public virtual IList<TEntity> EntityFromSql(string storeProcedureName, params DataParameter[] dataParameters)
        {
            var command = new CommandInfo(_dataConnection, storeProcedureName, dataParameters?.ToArray() ?? Array.Empty<DataParameter>());
            var rez = command.QueryProc<TEntity>()?.ToList();
            UpdateOutputParameters(_dataConnection, dataParameters);
            return rez ?? new List<TEntity>();
        }

        public virtual async Task<IList<TEntity>> EntityFromSqlAsync(string storeProcedureName, params DataParameter[] dataParameters)
        {
            var command = new CommandInfo(_dataConnection, storeProcedureName, dataParameters?.ToArray() ?? Array.Empty<DataParameter>());
            var rez = (await command.QueryProcAsync<TEntity>())?.ToList();
            UpdateOutputParameters(_dataConnection, dataParameters);
            return rez ?? new List<TEntity>();
        }

        /// <summary>
        /// Truncates database table
        /// </summary>
        /// <param name="resetIdentity">Performs reset identity column</param>
        public virtual int Truncate(bool resetIdentity = false)
        {
            return _dataConnection.GetTable<TEntity>().Truncate(resetIdentity);
        }

        public virtual async Task<int> TruncateAsync(bool resetIdentity = false)
        {
            return await _dataConnection.GetTable<TEntity>().TruncateAsync(resetIdentity);
        }

        public virtual int DeleteLocalizedEntities(IEnumerable<object> entityIds)
        {
            var entityType = typeof(TEntity);
            var entityName = entityType.Name;

            var masterConn = EngineContext.Current.Resolve<DataConnection>(DataConnectionHelper.ConnectionStringNames.Master);

            //var result = _dataConnection.GetTable<LocalizedProperty>()
            var result = masterConn.GetTable<BaseEntity>().TableName("LocalizedProperty")
                //.Where(w => w.LocaleKeyGroup == entityName && w.EntityId.In(entityIds))
                .Where(w => Sql.Property<string>(w, "LocaleKeyGroup") == entityName
                    && Sql.Property<string>(w, "EntityId").In(entityIds))
                .Delete();

            return result;
        }

        public virtual async Task<int> DeleteLocalizedEntitiesAsync(IEnumerable<object> entityIds)
        {
            var entityType = typeof(TEntity);
            var entityName = entityType.Name;

            var masterConn = EngineContext.Current.Resolve<DataConnection>(DataConnectionHelper.ConnectionStringNames.Master);

            //var result = await _dataConnection.GetTable<LocalizedProperty>()
            var result = await masterConn.GetTable<BaseEntity>().TableName("LocalizedProperty")
                //.Where(w => w.LocaleKeyGroup == entityName && w.EntityId.In(entityIds))
                .Where(w => Sql.Property<string>(w, "LocaleKeyGroup") == entityName
                    && Sql.Property<string>(w, "EntityId").In(entityIds))
                .DeleteAsync();

            return result;
        }

        #endregion

        #region Properties

        public virtual DataConnection DataConnection => _dataConnection;

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<TEntity> Table => Entities;

        /// <summary>
        /// Gets an entity set
        /// </summary>
        protected virtual ITable<TEntity> Entities => _entities ?? (_entities = _dataConnection.GetTable<TEntity>());

        #endregion

        #region Utils

        private void UpdateOutputParameters(DataConnection dataConnection, DataParameter[] dataParameters)
        {
            if (dataParameters is null || dataParameters.Length == 0)
                return;

            foreach (var dataParam in dataParameters.Where(p => p.Direction == ParameterDirection.Output))
            {
                UpdateParameterValue(dataConnection, dataParam);
            }
        }

        private void UpdateParameterValue(DataConnection dataConnection, DataParameter parameter)
        {
            if (dataConnection is null)
                throw new ArgumentNullException(nameof(dataConnection));

            if (parameter is null)
                throw new ArgumentNullException(nameof(parameter));

            if (dataConnection.Command is IDbCommand command &&
                command.Parameters.Count > 0 &&
                command.Parameters.Contains(parameter.Name) &&
                command.Parameters[parameter.Name] is IDbDataParameter param)
            {
                parameter.Value = param.Value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents the Entity repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public partial class IntLowercaseEntityRepository<TEntity> : IIntLowercaseRepository<TEntity> where TEntity : BaseIntLowercaseEntity
    {
        #region Fields

        private readonly DataConnection _dataConnection;
        private ITable<TEntity> _entities;

        #endregion

        #region Ctor

        public IntLowercaseEntityRepository(DataConnection dataConnection)
        {
            _dataConnection = dataConnection;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get entity entries by identifiers
        /// </summary>
        /// <param name="ids">Entity entry identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        public virtual IList<TEntity> GetByIds(IEnumerable<int> ids)
        {
            if (!ids?.Any() ?? true)
                return new List<TEntity>();

            return Entities.Where(w => ids.Contains(w.id)).ToList();
        }

        /// <summary>
        /// Get entity entries by identifiers
        /// </summary>
        /// <param name="ids">Entity entry identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        public virtual async Task<IList<TEntity>> GetByIdsAsync(IEnumerable<int> ids)
        {
            if (!ids?.Any() ?? true)
                return new List<TEntity>();

            return await Entities.Where(w => ids.Contains(w.id)).ToListAsync();
        }

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual TEntity GetById(object id)
        {
            return Entities.FirstOrDefault(e => e.id == (int)id);
        }

        public virtual async Task<TEntity> GetByIdAsync(object id)
        {
            return await Entities.FirstOrDefaultAsync(e => e.id == (int)id);
        }

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual int Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return _dataConnection.Insert(entity);
        }

        public virtual async Task<int> InsertAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return await _dataConnection.InsertAsync(entity);
        }

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual long Insert(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = 0L;
            using (var transaction = new TransactionScope())
            {
                var status = _dataConnection.BulkCopy(new BulkCopyOptions(), entities);
                result = status.RowsCopied;
                transaction.Complete();
            }

            return result;
        }

        public virtual async Task<long> InsertAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = 0L;
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var status = await _dataConnection.BulkCopyAsync(new BulkCopyOptions(), entities);
                result = status.RowsCopied;
                transaction.Complete();
            }

            return result;
        }

        /// <summary>
        /// Loads the original copy of the entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Copy of the passed entity</returns>
        public virtual TEntity LoadOriginalCopy(TEntity entity)
        {
            return _dataConnection.GetTable<TEntity>()
                .FirstOrDefault(e => e.id == entity.id);
        }

        public virtual async Task<TEntity> LoadOriginalCopyAsync(TEntity entity)
        {
            return await _dataConnection.GetTable<TEntity>()
                .FirstOrDefaultAsync(e => e.id == entity.id);
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual int Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return _dataConnection.Update(entity);
        }

        public virtual async Task<int> UpdateAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return await _dataConnection.UpdateAsync(entity);
        }

        public virtual int Update<TResult>(TEntity entity, Expression<Func<TEntity, TResult>> selector)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var query = Entities.Where(w => w.id == entity.id);
            IUpdatable<TEntity> cmd = null;
            foreach (var p in selector.ReturnType.GetProperties())
            {
                var propValue = p.GetValue(entity, null);
                cmd = cmd == null
                    ? query.Set(x => Sql.Property<object>(x, p.Name), propValue)
                    : cmd.Set(x => Sql.Property<object>(x, p.Name), propValue);
            }

            return cmd?.Update() ?? 0;
        }

        public virtual async Task<int> UpdateAsync<TResult>(TEntity entity, Expression<Func<TEntity, TResult>> selector)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var query = Entities.Where(w => w.id == entity.id);
            IUpdatable<TEntity> cmd = null;
            foreach (var p in selector.ReturnType.GetProperties())
            {
                var propValue = p.GetValue(entity, null);
                cmd = cmd == null
                    ? query.Set(x => Sql.Property<object>(x, p.Name), propValue)
                    : cmd.Set(x => Sql.Property<object>(x, p.Name), propValue);
            }

            return cmd != null ? await cmd.UpdateAsync() : 0;
        }

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual int Update(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = 0;
            foreach (var entity in entities)
            {
                result += Update(entity);
            }

            return result;
        }

        public virtual async Task<int> UpdateAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = 0;
            foreach (var entity in entities)
            {
                result += await UpdateAsync(entity);
            }

            return result;
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual int Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var result = _dataConnection.Delete(entity);

            if (typeof(ILocalizedEntity).IsAssignableFrom(typeof(TEntity)))
                DeleteLocalizedEntities(new[] { entity.id.ToString() });

            return result;
        }

        public virtual async Task<int> DeleteAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var result = await _dataConnection.DeleteAsync(entity);

            if (typeof(ILocalizedEntity).IsAssignableFrom(typeof(TEntity)))
                await DeleteLocalizedEntitiesAsync(new[] { entity.id.ToString() });

            return result;
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual int Delete(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = _dataConnection.GetTable<TEntity>()
                .Where(e => e.id.In(entities.Select(x => x.id)))
                .Delete();

            if (typeof(ILocalizedEntity).IsAssignableFrom(typeof(TEntity)))
                DeleteLocalizedEntities(entities.Select(s => s.id.ToString()));

            return result;
        }

        public virtual async Task<int> DeleteAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var result = await _dataConnection.GetTable<TEntity>()
                .Where(e => e.id.In(entities.Select(x => x.id)))
                .DeleteAsync();

            if (typeof(ILocalizedEntity).IsAssignableFrom(typeof(TEntity)))
                await DeleteLocalizedEntitiesAsync(entities.Select(s => s.id.ToString()));

            return result;
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        public virtual int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return _dataConnection.GetTable<TEntity>()
                .Where(predicate)
                .Delete();

            // DeleteLocalizedEntities manually by LocalizedEntityService
        }

        public virtual async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _dataConnection.GetTable<TEntity>()
                .Where(predicate)
                .DeleteAsync();

            // DeleteLocalizedEntities manually by LocalizedEntityService
        }

        public virtual int Delete(IEnumerable<object> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = _dataConnection.GetTable<TEntity>()
                .Where(e => e.id.In(ids))
                .Delete();

            if (typeof(ILocalizedEntity).IsAssignableFrom(typeof(TEntity)))
                DeleteLocalizedEntities(ids);

            return result;
        }

        public virtual async Task<int> DeleteAsync(IEnumerable<object> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            var result = await _dataConnection.GetTable<TEntity>()
                .Where(e => e.id.In(ids))
                .DeleteAsync();

            if (typeof(ILocalizedEntity).IsAssignableFrom(typeof(TEntity)))
                await DeleteLocalizedEntitiesAsync(ids);

            return result;
        }

        /// <summary>
        /// Executes command using System.Data.CommandType.StoredProcedure command type
        /// and returns results as collection of values of specified type
        /// </summary>
        /// <param name="storeProcedureName">Store procedure name</param>
        /// <param name="dataParameters">Command parameters</param>
        /// <returns>Collection of query result records</returns>
        public virtual IList<TEntity> EntityFromSql(string storeProcedureName, params DataParameter[] dataParameters)
        {
            var command = new CommandInfo(_dataConnection, storeProcedureName, dataParameters?.ToArray() ?? Array.Empty<DataParameter>());
            var rez = command.QueryProc<TEntity>()?.ToList();
            UpdateOutputParameters(_dataConnection, dataParameters);
            return rez ?? new List<TEntity>();
        }

        public virtual async Task<IList<TEntity>> EntityFromSqlAsync(string storeProcedureName, params DataParameter[] dataParameters)
        {
            var command = new CommandInfo(_dataConnection, storeProcedureName, dataParameters?.ToArray() ?? Array.Empty<DataParameter>());
            var rez = (await command.QueryProcAsync<TEntity>())?.ToList();
            UpdateOutputParameters(_dataConnection, dataParameters);
            return rez ?? new List<TEntity>();
        }

        /// <summary>
        /// Truncates database table
        /// </summary>
        /// <param name="resetIdentity">Performs reset identity column</param>
        public virtual int Truncate(bool resetIdentity = false)
        {
            return _dataConnection.GetTable<TEntity>().Truncate(resetIdentity);
        }

        public virtual async Task<int> TruncateAsync(bool resetIdentity = false)
        {
            return await _dataConnection.GetTable<TEntity>().TruncateAsync(resetIdentity);
        }

        public virtual int DeleteLocalizedEntities(IEnumerable<object> entityIds)
        {
            var entityType = typeof(TEntity);
            var entityName = entityType.Name;

            var masterConn = EngineContext.Current.Resolve<DataConnection>(DataConnectionHelper.ConnectionStringNames.Master);

            //var result = _dataConnection.GetTable<LocalizedProperty>()
            var result = masterConn.GetTable<BaseEntity>().TableName("LocalizedProperty")
                //.Where(w => w.LocaleKeyGroup == entityName && w.EntityId.In(entityIds))
                .Where(w => Sql.Property<string>(w, "LocaleKeyGroup") == entityName
                    && Sql.Property<string>(w, "EntityId").In(entityIds))
                .Delete();

            return result;
        }

        public virtual async Task<int> DeleteLocalizedEntitiesAsync(IEnumerable<object> entityIds)
        {
            var entityType = typeof(TEntity);
            var entityName = entityType.Name;

            var masterConn = EngineContext.Current.Resolve<DataConnection>(DataConnectionHelper.ConnectionStringNames.Master);

            //var result = await _dataConnection.GetTable<LocalizedProperty>()
            var result = await masterConn.GetTable<BaseEntity>().TableName("LocalizedProperty")
                //.Where(w => w.LocaleKeyGroup == entityName && w.EntityId.In(entityIds))
                .Where(w => Sql.Property<string>(w, "LocaleKeyGroup") == entityName
                    && Sql.Property<string>(w, "EntityId").In(entityIds))
                .DeleteAsync();

            return result;
        }

        #endregion

        #region Properties

        public virtual DataConnection DataConnection => _dataConnection;

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<TEntity> Table => Entities;

        /// <summary>
        /// Gets an entity set
        /// </summary>
        protected virtual ITable<TEntity> Entities => _entities ?? (_entities = _dataConnection.GetTable<TEntity>());

        #endregion

        #region Utils

        private void UpdateOutputParameters(DataConnection dataConnection, DataParameter[] dataParameters)
        {
            if (dataParameters is null || dataParameters.Length == 0)
                return;

            foreach (var dataParam in dataParameters.Where(p => p.Direction == ParameterDirection.Output))
            {
                UpdateParameterValue(dataConnection, dataParam);
            }
        }

        private void UpdateParameterValue(DataConnection dataConnection, DataParameter parameter)
        {
            if (dataConnection is null)
                throw new ArgumentNullException(nameof(dataConnection));

            if (parameter is null)
                throw new ArgumentNullException(nameof(parameter));

            if (dataConnection.Command is IDbCommand command &&
                command.Parameters.Count > 0 &&
                command.Parameters.Contains(parameter.Name) &&
                command.Parameters[parameter.Name] is IDbDataParameter param)
            {
                parameter.Value = param.Value;
            }
        }

        #endregion
    }

    #endregion
}
