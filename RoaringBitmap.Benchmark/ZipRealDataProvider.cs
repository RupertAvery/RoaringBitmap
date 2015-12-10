﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace RoaringBitmap.Benchmark
{
    public class ZipRealDataProvider : IEnumerable<RoaringBitmap>, IDisposable
    {
        private readonly ZipArchive m_Archive;

        public ZipRealDataProvider(string path)
        {
            var fs = File.OpenRead(path);
            m_Archive = new ZipArchive(fs, ZipArchiveMode.Read);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IEnumerator<RoaringBitmap> GetEnumerator()
        {
            foreach (var zipArchiveEntry in m_Archive.Entries)
            {
                using (var stream = zipArchiveEntry.Open())
                {
                    using (var stringReader = new StreamReader(stream))
                    {
                        var split = stringReader.ReadLine().Split(',');
                        var values = split.Select(int.Parse).ToList();
                        yield return RoaringBitmap.Create(values);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        ~ZipRealDataProvider()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_Archive.Dispose();
            }
        }
    }
}