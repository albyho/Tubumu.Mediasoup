using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;

namespace Tubumu.Utils.FastLambda
{
    /// <summary>
    /// HashedListCache
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HashedListCache<T> : IDisposable, IExpressionCache<T> where T : class
    {
        private readonly Dictionary<int, SortedList<Expression, T>> _storage = new();
        private readonly ReaderWriterLockSlim _rwLock = new();

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="key"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        public T Get(Expression key, Func<Expression, T> creator)
        {
            SortedList<Expression, T>? sortedList;
            T? value;

            int hash = new Hasher().Hash(key);
            _rwLock.EnterReadLock();
            try
            {
                if (_storage.TryGetValue(hash, out sortedList) &&
                    sortedList.TryGetValue(key, out value))
                {
                    return value;
                }
            }
            finally
            {
                _rwLock.ExitReadLock();
            }

            _rwLock.EnterWriteLock();
            try
            {
                if (!_storage.TryGetValue(hash, out sortedList))
                {
                    sortedList = new SortedList<Expression, T>(new Comparer());
                    _storage.Add(hash, sortedList);
                }

                if (!sortedList.TryGetValue(key, out value))
                {
                    value = creator(key);
                    sortedList.Add(key, value);
                }

                return value;
            }
            finally
            {
                _rwLock.ExitWriteLock();
            }
        }

        private class Hasher : ExpressionHasher
        {
            protected override Expression VisitConstant(ConstantExpression c)
            {
                return c;
            }
        }

        internal class Comparer : ExpressionComparer
        {
            protected override int CompareConstant(ConstantExpression x, ConstantExpression y)
            {
                return 0;
            }
        }

        #region IDisposable Support

        private bool _disposedValue; // 要检测冗余调用

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // 1: 释放托管状态(托管对象)。
                    _rwLock.Dispose();
                }

                // 2: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // 3: 将大型字段设置为 null。

                _disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        // 4: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~HashedListCache() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // 5: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
