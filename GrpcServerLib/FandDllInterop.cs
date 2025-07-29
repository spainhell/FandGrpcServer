using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GrpcServerLib
{
    /// <summary>
    /// Interop class for calling C++ functions from FandLibs.dll
    /// </summary>
    public static class FandDllInterop
    {
        private const string DLL_NAME = "cppfand_dll.dll";

        /// <summary>
        /// Opens an RDB file and returns the number of records
        /// </summary>
        /// <param name="rdbName">Path to the RDB file</param>
        /// <returns>Number of records in the RDB file</returns>
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int OpenRDB([MarshalAs(UnmanagedType.LPStr)] string rdbName);

        /// <summary>
        /// Gets the total number of records in the opened RDB
        /// </summary>
        /// <returns>Number of records, or -1 if no RDB is open</returns>
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetRecordsCount();

        /// <summary>
        /// Loads a specific record by number
        /// </summary>
        /// <param name="recNr">Record number to load (1-based)</param>
        /// <returns>0 for success, -1 for error</returns>
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int LoadRecord(int recNr);

        /// <summary>
        /// Gets the chapter type of the currently loaded record
        /// </summary>
        /// <param name="chapterType">Buffer to receive the chapter type</param>
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetChapterType([MarshalAs(UnmanagedType.LPArray)] byte[] chapterType);

        /// <summary>
        /// Gets the chapter name of the currently loaded record
        /// </summary>
        /// <param name="chapterName">Buffer to receive the chapter name</param>
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetChapterName([MarshalAs(UnmanagedType.LPArray)] byte[] chapterName);

        /// <summary>
        /// Gets the length of the chapter code
        /// </summary>
        /// <returns>Length of the chapter code</returns>
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetChapterCodeLength();

        /// <summary>
        /// Gets the chapter code of the currently loaded record
        /// </summary>
        /// <param name="chapterCode">Buffer to receive the chapter code</param>
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetChapterCode([MarshalAs(UnmanagedType.LPArray)] byte[] chapterCode);

        /// <summary>
        /// Closes a specific record (placeholder function)
        /// </summary>
        /// <param name="recNr">Record number to close</param>
        /// <returns>Always returns 0</returns>
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CloseRecord(int recNr);

        /// <summary>
        /// Clears the RDB (sets record count to 0)
        /// </summary>
        /// <returns>Always returns 0</returns>
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ClearRdb();

        /// <summary>
        /// Saves a new chapter with type, name, and code
        /// </summary>
        /// <param name="chapterType">Chapter type</param>
        /// <param name="chapterName">Chapter name</param>
        /// <param name="chapterCode">Chapter code</param>
        /// <returns>New total number of records</returns>
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public static extern int SaveChapter(
            [MarshalAs(UnmanagedType.LPWStr)] string chapterType,
            [MarshalAs(UnmanagedType.LPWStr)] string chapterName,
            [MarshalAs(UnmanagedType.LPWStr)] string chapterCode);

        /// <summary>
        /// Closes the RDB and saves the file
        /// </summary>
        /// <returns>Final number of records</returns>
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CloseRdb();

        // High-level wrapper methods for string operations

        /// <summary>
        /// Gets the chapter type as a managed string
        /// </summary>
        /// <returns>Chapter type string</returns>
        public static string GetChapterTypeString()
        {
            byte[] buffer = new byte[1024]; // Allocate sufficient buffer
            GetChapterType(buffer);
            return Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        }

        /// <summary>
        /// Gets the chapter name as a managed string
        /// </summary>
        /// <returns>Chapter name string</returns>
        public static string GetChapterNameString()
        {
            byte[] buffer = new byte[1024]; // Allocate sufficient buffer
            GetChapterName(buffer);
            return Encoding.Unicode.GetString(buffer).TrimEnd('\0');
        }

        /// <summary>
        /// Gets the chapter code as a managed string
        /// </summary>
        /// <returns>Chapter code string</returns>
        public static string GetChapterCodeString()
        {
            int length = GetChapterCodeLength();
            if (length <= 0) return string.Empty;

            byte[] buffer = new byte[length];
            GetChapterCode(buffer);
            return Encoding.Unicode.GetString(buffer);
        }
    }
}