using System;
using System.Collections.Generic;

namespace Doji.PackageAuthoring.Editor {
    internal interface IPackageSearchSource : IDisposable {
        event Action Changed;

        bool IsLoading { get; }
        string StatusMessage { get; }
        IReadOnlyList<PackageSearchEntry> Entries { get; }

        void Refresh();
    }
}
