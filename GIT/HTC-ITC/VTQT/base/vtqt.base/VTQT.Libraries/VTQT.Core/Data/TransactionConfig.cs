using System.Transactions;

namespace VTQT.Core.Data
{
    public class TransactionConfig
    {
        // TODO-XBase-Data: Check nếu DB hỗ trợ Snapshot thì dùng Snapshot, không thì dùng Serializable
        // Nếu DB "SET ALLOW_SNAPSHOT_ISOLATION ON" thì dùng Snapshot, nếu không sẽ báo lỗi
        //new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot },
        //public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.Snapshot;
        public static IsolationLevel IsolationLevel { get; } = IsolationLevel.Serializable;
    }
}
