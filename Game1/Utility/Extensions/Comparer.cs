﻿using System;
using System.Collections.Generic;

namespace Omniplatformer.Utility.Extensions
{
    public class DuplicateKeyComparer<TKey>
                :
             IComparer<TKey> where TKey : IComparable
    {
        #region IComparer<TKey> Members

        public int Compare(TKey x, TKey y)
        {
            int result = x.CompareTo(y);

            if (result == 0)
                return 1;   // Handle equality as being greater
            else
                return result;
        }

        #endregion
    }
}
