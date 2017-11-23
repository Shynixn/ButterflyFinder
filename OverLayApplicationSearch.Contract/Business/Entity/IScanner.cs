using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverLayApplicationSearch.Contract.Business.Entity
{
    public delegate void ScannerDelegate(long amount);

    public interface IScanner : IDisposable
    {
        /// <summary>
        /// Gets called when the amount of items scanned changed
        /// </summary>
        event ScannerDelegate AmountScannedChangeEvent;

        /// <summary>
        /// Scans and indexes the given folder and subfiles
        /// </summary>
        /// <param name="fileName">path</param>
        void Scan(string fileName);

        /// <summary>
        /// Returns the amount of files and subfiles in the given folder
        /// </summary>
        /// <param name="fileName">path</param>
        /// <returns>amount</returns>
        long GetAmountOfFiles(string fileName);

        /// <summary>
        /// Returns the amount of items indexed by Scan
        /// </summary>
        long Count { get; }

        /// <summary>
        /// Returns the amount of items counted by last getAmountOfFiles
        /// </summary>
        long MaxCount { get; }

        /// <summary>
        /// Cancels the last scan
        /// </summary>
        void Cancel();

        /// <summary>
        /// returns if the task was cancelled
        /// </summary>
        bool IsCancelled { get; }
    }
}