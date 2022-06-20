using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using VTQT.Core.Localization;

namespace VTQT.Core.Logging
{
    public interface IMvcNotifier
    {
        void Add(MvcNotifyType type, LocalizedString message, bool durable = true);
        ICollection<MvcNotifyEntry> Entries { get; }
    }

    public class MvcNotifier : IMvcNotifier
    {
        private readonly HashSet<MvcNotifyEntry> _entries = new HashSet<MvcNotifyEntry>();

        public void Add(MvcNotifyType type, LocalizedString message, bool durable = true)
        {
            _entries.Add(new MvcNotifyEntry { Type = type, Message = message, Durable = durable });
        }

        public ICollection<MvcNotifyEntry> Entries => _entries;
    }

    public static class IMvcNotifierExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Information(this IMvcNotifier notifier, LocalizedString message, bool durable = true)
        {
            notifier.Add(MvcNotifyType.Info, message, durable);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Success(this IMvcNotifier notifier, LocalizedString message, bool durable = true)
        {
            notifier.Add(MvcNotifyType.Success, message, durable);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warning(this IMvcNotifier notifier, LocalizedString message, bool durable = true)
        {
            notifier.Add(MvcNotifyType.Warning, message, durable);
        }

        public static void Error(this IMvcNotifier notifier, Exception exception, bool durable = true)
        {
            if (exception == null)
                return;

            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            notifier.Add(MvcNotifyType.Error, exception.Message, durable);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(this IMvcNotifier notifier, LocalizedString message, bool durable = true)
        {
            notifier.Add(MvcNotifyType.Error, message, durable);
        }
    }
}
