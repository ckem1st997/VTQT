namespace VTQT.Core.Domain
{
    /// <summary>
    /// Represents an type of reCAPTCHA
    /// </summary>
    public enum CaptchaType
    {
        /// <summary>
        /// Use reCAPTCHA v2 check box
        /// </summary>
        CheckBoxReCaptchaV2 = 10,

        /// <summary>
        /// Use reCAPTCHA v3
        /// </summary>
        ReCaptchaV3 = 20,
    }

    /// <summary>
    /// Trạng thái kích hoạt
    /// </summary>
    public enum ActiveStatus
    {
        /// <summary>
        /// Kích hoạt
        /// </summary>
        Activated = 1,
        /// <summary>
        /// Ngừng kích hoạt
        /// </summary>
        Deactivated = 2
    }

    /// <summary>
    /// Prepare for Model CRUD
    /// </summary>
    public enum PrepareModelFor
    {
        Create = 1,
        Details = 2,
        Edit = 3
    }

    public enum CrudType
    {
        Create = 1,
        Read = 2,
        Update = 3,
        Delete = 4
    }

    /// <summary>
    /// Represents a log level
    /// </summary>
    public enum LogLevel
    {
        Debug = 10,
        Information = 20,
        Warning = 30,
        Error = 40,
        Fatal = 50
    }

    public enum DataType
    {
        DateTime = 1,
        Decimal = 2,
        Integer = 3,
        Boolean = 4,
        String = 5,
        Character = 6
    }
}
