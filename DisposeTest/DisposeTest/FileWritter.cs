using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DisposeTest
{
    /// <summary>
    /// Contais the logic for playing audio files.
    /// </summary>
    public class FileWritter : IDisposable
    {

        private IntPtr fileHandle;
        private string filepath;

        [DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        [DllImport("kernel32", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateFile(string filePath, DesiredAccess lpFileMappingAttributes, uint shareMode, uint securityAttributes,
            CreationDisposition creationDisposition, uint flagsAndAttributes, int hTemplateFile);

        public void Dispose()
        {
            CloseHandle(fileHandle);
        }

        public FileWritter(string filePath)
        {
            this.filepath = filePath;
        }

        private IntPtr Open(string filePath)
        {
            IntPtr fileHandle = FileWritter.CreateFile(filePath, DesiredAccess.GENERIC_WRITE, 0, 0, CreationDisposition.OPEN_ALWAYS, 0, 0);
            if (fileHandle == IntPtr.Zero)
                throw new Exception($"Cannot creat {filePath}");

            return fileHandle;
        }

        public uint Write(string textToWrite)
        {
            var bytesToWrite = Encoding.Default.GetBytes(textToWrite);
            return this.Write(bytesToWrite, (uint)bytesToWrite.Length);
        }

        public uint Write(byte[] bytesToWrite, uint bytesToWriteCount)
        {
            if (this.fileHandle == IntPtr.Zero)
                this.fileHandle = this.Open(this.filepath);

            uint writtenBytesCount = 0;
            if (!FileWritter.WriteFile(this.fileHandle, bytesToWrite, bytesToWriteCount, out writtenBytesCount, IntPtr.Zero))
                throw new Exception($"Could not wirte to file: {this.filepath}");
            return writtenBytesCount;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteFile(IntPtr hFile, Byte[] aBuffer, UInt32 NumberOfBytesToWrite, out UInt32 lpNumberOfBytesWritten, IntPtr pOverlapped);

        [Flags]
        private enum FileMapAccess : uint
        {
            FileMapCopy = 0x0001,
            FileMapWrite = 0x0002,
            FileMapRead = 0x0004,
            FileMapAllAccess = 0x001f,
            fileMapExecute = 0x0020,
        }

        [Flags]
        public enum DesiredAccess : uint
        {
            GENERIC_READ = 0x80000000,
            GENERIC_WRITE = 0x40000000
        }

        public enum CreationDisposition : uint
        {
            CREATE_NEW = 1,
            CREATE_ALWAYS = 2,
            OPEN_EXISTING = 3,
            OPEN_ALWAYS = 4,
            TRUNCATE_EXSTING = 5
        }
    }
}
