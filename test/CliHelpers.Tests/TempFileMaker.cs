using System;
using System.IO;

namespace CliHelpers.Tests
{
    public class TempFileMaker : IDisposable
    {
        public string TempFilePath { get; }

        public TempFileMaker()
        {
            TempFilePath = Path.GetTempFileName();
        }

        public void Dispose()
        {
            File.Delete(TempFilePath);
        }
    }
}
