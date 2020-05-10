﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpCompress.Common;

namespace Microsoft.CST.OpenSource.MultiExtractor
{
    /**
     * Very simple implementation of an .Deb format parser, needed for Debian .deb archives.
     * See: https://en.wikipedia.org/wiki/Deb_(file_format)#/media/File:Deb_File_Structure.svg
     */
    public static class DebArchiveFile
    {
        public static IEnumerable<FileEntry> GetFileEntries(FileEntry fileEntry)
        {
            if (fileEntry == null)
            {
                yield break;
            }

            // First, cut out the file signature (8 bytes) and global header (64 bytes)
            fileEntry.Content.Position = 72;
            var headerBytes = new byte[60];

            while (true)
            {
                if (fileEntry.Content.Length - fileEntry.Content.Position < 60)  // The header for each file is 60 bytes
                {
                    break;
                }
                fileEntry.Content.Read(headerBytes, 0, 60);
                var filename = Encoding.ASCII.GetString(headerBytes[0..16]).Trim();  // filename is 16 bytes
                var fileSizeBytes = headerBytes[48..58]; // File size is decimal-encoded, 10 bytes long
                if (int.TryParse(Encoding.ASCII.GetString(fileSizeBytes).Trim(), out int fileSize))
                {
                    var entryContent = new byte[fileSize];
                    fileEntry.Content.Write(entryContent, 0, fileSize);
                    using var fs = new FileStream(Path.GetTempFileName(), FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.DeleteOnClose);
                    fs.Write(entryContent, 0, entryContent.Length);
                    yield return new FileEntry(filename, fileEntry.FullPath, fs, passthroughStream: true);
                }
                else
                {
                    break;
                }                
            }
        }
    }
}
