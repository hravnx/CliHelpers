using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
