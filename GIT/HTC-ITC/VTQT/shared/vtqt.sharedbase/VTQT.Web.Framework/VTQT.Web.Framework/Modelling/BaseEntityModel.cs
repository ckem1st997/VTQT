using System;

namespace VTQT.Web.Framework.Modelling
{
    public partial class BaseEntityModel : BaseModel
    {
        /// <summary>
        /// Gets or sets model identifier
        /// </summary>
        public virtual string Id { get; set; } = Guid.NewGuid().ToString();
    }

    #region Fix Id (DataType, Lowercase) cho các DB khác

    public partial class BaseIntEntity : BaseModel
    {
        /// <summary>
        /// Gets or sets model identifier
        /// </summary>
        public virtual int Id { get; set; }
    }

    public partial class BaseIntLowercaseEntity : BaseModel
    {
        /// <summary>
        /// Gets or sets model identifier
        /// </summary>
        public virtual int id { get; set; }
    }

    #endregion
}
