using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Force.DeepCloner;

namespace Tubumu.Mediasoup
{
    public class MediasoupServer : EventEmitter
    {
        private readonly List<IWorker> _workers = new();

        private int _nextMediasoupWorkerIndex = 0;

        private readonly ReaderWriterLockSlim _workersLock = new();

        public EventEmitter Observer { get; } = new EventEmitter();

        /// <summary>
        /// Get a cloned copy of the mediasoup supported RTP capabilities.
        /// </summary>
        /// <returns></returns>
        public static RtpCapabilities GetSupportedRtpCapabilities()
        {
            return RtpCapabilities.SupportedRtpCapabilities.DeepClone();
        }

        /// <summary>
        /// Get next mediasoup Worker.
        /// </summary>
        /// <returns></returns>
        public IWorker GetWorker()
        {
            _workersLock.EnterReadLock();
            try
            {
                if (_nextMediasoupWorkerIndex > _workers.Count - 1)
                {
                    throw new Exception("none worker");
                }

                if (++_nextMediasoupWorkerIndex == _workers.Count)
                {
                    _nextMediasoupWorkerIndex = 0;
                }

                return _workers[_nextMediasoupWorkerIndex];
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Get worker failure: {ex.Message}");
                throw;
            }
            finally
            {
                _workersLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Add worker.
        /// </summary>
        /// <param name="worker"></param>
        public void AddWorker(IWorker worker)
        {
            if (worker == null)
            {
                throw new ArgumentNullException(nameof(worker));
            }

            _workersLock.EnterWriteLock();
            try
            {
                _workers.Add(worker);
                Observer.Emit("newworker", worker);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Add worker failure: {ex.Message}");
            }
            finally
            {
                _workersLock.ExitWriteLock();
            }
        }
    }
}
