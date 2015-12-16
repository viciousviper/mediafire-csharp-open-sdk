using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFireSDK.Model
{
    /// <summary>
    /// Represents the stage of a block operation.
    /// </summary>
    public class MediaFireOperationProgress
    {
        public MediaFireOperationProgress() { }

        /// <remarks>
        /// This property value is in bytes.
        /// </remarks>
        public long TotalSize { get;  set; }

        /// <remarks>
        /// This property value is in bytes.
        /// </remarks>
        public long CurrentSize { get;  set; }

        /// <remarks>
        /// A percentage value between 0 and 1
        /// </remarks>
        public double Percentage
        {
            get
            {
                if (TotalSize == 0)
                    return 1;

                return ((CurrentSize) / (double)TotalSize);
            }
        }

    }
}
