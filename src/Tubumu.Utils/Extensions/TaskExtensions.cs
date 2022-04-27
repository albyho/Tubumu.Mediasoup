using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Tubumu.Utils.Extensions
{
    public static class TaskExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // 造成编译器优化调用
        public static void NoWarning(this Task _)
        {
        }

        /// <summary>
        /// Returns a task that completes as the original task completes or when a timeout expires,
        /// whichever happens first.
        /// </summary>
        /// <param name="task">The task to wait for.</param>
        /// <param name="timeout">The maximum time to wait.</param>
        /// <returns>
        /// A task that completes with the result of the specified <paramref name="task"/> or
        /// faults with a <see cref="TimeoutException"/> if <paramref name="timeout"/> elapses first.
        /// </returns>
        public static async Task WithTimeout(this Task task, TimeSpan timeout, Action? cancelled = null)
        {
            using var timerCancellation = new CancellationTokenSource();
            var timeoutTask = Task.Delay(timeout, timerCancellation.Token);
            var firstCompletedTask = await Task.WhenAny(task, timeoutTask).ConfigureAwait(false);
            if (firstCompletedTask == timeoutTask)
            {
                if (cancelled == null)
                {
                    throw new TimeoutException();
                }
                cancelled();
            }

            // The timeout did not elapse, so cancel the timer to recover system resources.
            timerCancellation.Cancel();

            // re-throw any exceptions from the completed task.
            await task.ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a task that completes as the original task completes or when a timeout expires,
        /// whichever happens first.
        /// </summary>
        /// <typeparam name="T">The type of value returned by the original task.</typeparam>
        /// <param name="task">The task to wait for.</param>
        /// <param name="timeout">The maximum time to wait.</param>
        /// <returns>
        /// A task that completes with the result of the specified <paramref name="task"/> or
        /// faults with a <see cref="TimeoutException"/> if <paramref name="timeout"/> elapses first.
        /// </returns>
        public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout, Action? cancelled = null)
        {
            await WithTimeout((Task)task, timeout, cancelled).ConfigureAwait(false);
            return task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// Creates a new Task that mirrors the supplied task but that will be canceled after the specified timeout.
        /// </summary>
        /// <typeparam name="TResult">Specifies the type of data contained in the task.</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>The new Task that may time out.</returns>
        public static Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            var result = new TaskCompletionSource<TResult>(task.AsyncState);
            var timer = new Timer(state => ((TaskCompletionSource<TResult>)state!).TrySetCanceled(), result, timeout, TimeSpan.FromMilliseconds(-1));
            task.ContinueWith(t =>
            {
                timer.Dispose();
                result.TrySetFromTask(t);
            }, TaskContinuationOptions.ExecuteSynchronously);
            return result.Task;
        }

        /// <summary>
        /// Attempts to transfer the result of a Task to the TaskCompletionSource.
        /// </summary>
        /// <typeparam name="TResult">Specifies the type of the result.</typeparam>
        /// <param name="resultSetter">The TaskCompletionSource.</param>
        /// <param name="task">The task whose completion results should be transfered.</param>
        /// <returns>Whether the transfer could be completed.</returns>
        public static bool TrySetFromTask<TResult>(this TaskCompletionSource<TResult> resultSetter, Task task)
        {
            switch (task.Status)
            {
                case TaskStatus.RanToCompletion:
#pragma warning disable CS8604 // Possible null reference argument.
                    return resultSetter.TrySetResult(task is Task<TResult> taskLocal ? taskLocal.Result : default);
#pragma warning restore CS8604 // Possible null reference argument.
                case TaskStatus.Faulted:
                    return resultSetter.TrySetException(task.Exception!.InnerExceptions);
                case TaskStatus.Canceled:
                    return resultSetter.TrySetCanceled();
                case TaskStatus.Created:
                    break;
                case TaskStatus.WaitingForActivation:
                    break;
                case TaskStatus.WaitingToRun:
                    break;
                case TaskStatus.Running:
                    break;
                case TaskStatus.WaitingForChildrenToComplete:
                    break;
            }

            throw new InvalidOperationException("The task was not completed.");
        }

        /// <summary>
        /// Attempts to transfer the result of a Task to the TaskCompletionSource.
        /// </summary>
        /// <typeparam name="TResult">Specifies the type of the result.</typeparam>
        /// <param name="resultSetter">The TaskCompletionSource.</param>
        /// <param name="task">The task whose completion results should be transfered.</param>
        /// <returns>Whether the transfer could be completed.</returns>
        public static bool TrySetFromTask<TResult>(this TaskCompletionSource<TResult> resultSetter, Task<TResult> task)
        {
            return TrySetFromTask(resultSetter, (Task)task);
        }

        public static Task<T> FromResult<T>(T value)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(value);
            return tcs.Task;
        }

        public static TaskCompletionSource<TResult> WithTimeout<TResult>(this TaskCompletionSource<TResult> taskCompletionSource, TimeSpan timeout)
        {
            return WithTimeout(taskCompletionSource, timeout, null);
        }

        public static TaskCompletionSource<TResult> WithTimeout<TResult>(this TaskCompletionSource<TResult> taskCompletionSource, TimeSpan timeout, Action? cancelled)
        {
            Timer? timer = null;
            timer = new Timer(state =>
            {
                timer?.Dispose();
                if (taskCompletionSource.Task.Status != TaskStatus.RanToCompletion)
                {
                    taskCompletionSource.TrySetCanceled();
                    cancelled?.Invoke();
                }
            }, null, timeout, TimeSpan.FromMilliseconds(-1));

            return taskCompletionSource;
        }

        /// <summary>
        /// Set the TaskCompletionSource in an async fashion. This prevents the Task Continuation being executed sync on the same thread
        /// This is required otherwise contintinuations will happen on CEF UI threads
        /// </summary>
        /// <typeparam name="TResult">Generic param</typeparam>
        /// <param name="taskCompletionSource">tcs</param>
        /// <param name="result">result</param>
        public static void TrySetResultAsync<TResult>(this TaskCompletionSource<TResult> taskCompletionSource, TResult result)
        {
            Task.Factory.StartNew(() => taskCompletionSource.TrySetResult(result), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
        }

        #region Synchronously await

        /// <summary>
        /// Synchronously await the results of an asynchronous operation without deadlocking; ignoring cancellation.
        /// </summary>
        /// <param name="task">
        /// The <see cref="Task"/> representing the pending operation.
        /// </param>
        public static void AwaitCompletion(this ValueTask task)
        {
            new SynchronousAwaiter(task, true).GetResult();
        }

        /// <summary>
        /// Synchronously await the results of an asynchronous operation without deadlocking; ignoring cancellation.
        /// </summary>
        /// <param name="task">
        /// The <see cref="Task"/> representing the pending operation.
        /// </param>
        public static void AwaitCompletion(this Task task)
        {
            new SynchronousAwaiter(task, true).GetResult();
        }

        /// <summary>
        /// Synchronously await the results of an asynchronous operation without deadlocking.
        /// </summary>
        /// <param name="task">
        /// The <see cref="Task"/> representing the pending operation.
        /// </param>
        /// <typeparam name="T">
        /// The result type of the operation.
        /// </typeparam>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static T? AwaitResult<T>(this Task<T> task)
        {
            return new SynchronousAwaiter<T>(task).GetResult();
        }

        /// <summary>
        /// Synchronously await the results of an asynchronous operation without deadlocking.
        /// </summary>
        /// <param name="task">
        /// The <see cref="Task"/> representing the pending operation.
        /// </param>
        public static void AwaitResult(this Task task)
        {
            new SynchronousAwaiter(task).GetResult();
        }

        /// <summary>
        /// Synchronously await the results of an asynchronous operation without deadlocking.
        /// </summary>
        /// <param name="task">
        /// The <see cref="ValueTask"/> representing the pending operation.
        /// </param>
        /// <typeparam name="T">
        /// The result type of the operation.
        /// </typeparam>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static T? AwaitResult<T>(this ValueTask<T> task)
        {
            return new SynchronousAwaiter<T>(task).GetResult();
        }

        /// <summary>
        /// Synchronously await the results of an asynchronous operation without deadlocking.
        /// </summary>
        /// <param name="task">
        /// The <see cref="ValueTask"/> representing the pending operation.
        /// </param>
        public static void AwaitResult(this ValueTask task)
        {
            new SynchronousAwaiter(task).GetResult();
        }

        /// <summary>
        /// Ignore the <see cref="OperationCanceledException"/> if the operation is cancelled.
        /// </summary>
        /// <param name="task">
        /// The <see cref="Task"/> representing the asynchronous operation whose cancellation is to be ignored.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> representing the asynchronous operation whose cancellation is ignored.
        /// </returns>
        public static async Task IgnoreCancellationResult(this Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary>
        /// Ignore the <see cref="OperationCanceledException"/> if the operation is cancelled.
        /// </summary>
        /// <param name="task">
        /// The <see cref="ValueTask"/> representing the asynchronous operation whose cancellation is to be ignored.
        /// </param>
        /// <returns>
        /// The <see cref="ValueTask"/> representing the asynchronous operation whose cancellation is ignored.
        /// </returns>
        public static async ValueTask IgnoreCancellationResult(this ValueTask task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary>
        /// Ignore the results of an asynchronous operation allowing it to run and die silently in the background.
        /// </summary>
        /// <param name="task">
        /// The <see cref="Task"/> representing the asynchronous operation whose results are to be ignored.
        /// </param>
        public static async void IgnoreResult(this Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch
            {
                // ignore exceptions
            }
        }

        /// <summary>
        /// Ignore the results of an asynchronous operation allowing it to run and die silently in the background.
        /// </summary>
        /// <param name="task">
        /// The <see cref="ValueTask"/> representing the asynchronous operation whose results are to be ignored.
        /// </param>
        public static async void IgnoreResult(this ValueTask task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch
            {
                // ignore exceptions
            }
        }

        #endregion
    }

    /// <summary>
    /// Internal class for waiting for asynchronous operations that have a result.
    /// </summary>
    /// <typeparam name="TResult">
    /// The result type.
    /// </typeparam>
    public class SynchronousAwaiter<TResult>
    {
        /// <summary>
        /// The manual reset event signaling completion.
        /// </summary>
        private readonly ManualResetEvent _manualResetEvent;

        /// <summary>
        /// The exception thrown by the asynchronous operation.
        /// </summary>
        private Exception? _exception;

        /// <summary>
        /// The result of the asynchronous operation.
        /// </summary>
        private TResult? _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronousAwaiter{TResult}"/> class.
        /// </summary>
        /// <param name="task">
        /// The task representing an asynchronous operation.
        /// </param>
        public SynchronousAwaiter(Task<TResult> task)
        {
            _manualResetEvent = new ManualResetEvent(false);
            WaitFor(task);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronousAwaiter{TResult}"/> class.
        /// </summary>
        /// <param name="task">
        /// The task representing an asynchronous operation.
        /// </param>
        public SynchronousAwaiter(ValueTask<TResult> task)
        {
            _manualResetEvent = new ManualResetEvent(false);
            WaitFor(task);
        }

        /// <summary>
        /// Gets a value indicating whether the operation is complete.
        /// </summary>
        public bool IsComplete => _manualResetEvent.WaitOne(0);

        /// <summary>
        /// Synchronously get the result of an asynchronous operation.
        /// </summary>
        /// <returns>
        /// The result of the asynchronous operation.
        /// </returns>
        public TResult? GetResult()
        {
            _manualResetEvent.WaitOne();
            return _exception != null ? throw _exception : _result;
        }

        /// <summary>
        /// Tries to synchronously get the result of an asynchronous operation.
        /// </summary>
        /// <param name="operationResult">
        /// The result of the operation.
        /// </param>
        /// <returns>
        /// The result of the asynchronous operation.
        /// </returns>
        public bool TryGetResult(out TResult? operationResult)
        {
            if (IsComplete)
            {
                operationResult = _exception != null ? throw _exception : _result;
                return true;
            }

            operationResult = default;
            return false;
        }

        /// <summary>
        /// Background "thread" which waits for the specified asynchronous operation to complete.
        /// </summary>
        /// <param name="task">
        /// The task.
        /// </param>
        private async void WaitFor(Task<TResult> task)
        {
            try
            {
                _result = await task.ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _exception = exception;
            }
            finally
            {
                _manualResetEvent.Set();
            }
        }

        /// <summary>
        /// Background "thread" which waits for the specified asynchronous operation to complete.
        /// </summary>
        /// <param name="task">
        /// The task.
        /// </param>
        private async void WaitFor(ValueTask<TResult> task)
        {
            try
            {
                _result = await task.ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _exception = exception;
            }
            finally
            {
                _manualResetEvent.Set();
            }
        }
    }

    /// <summary>
    /// Internal class for  waiting for  asynchronous operations that have no result.
    /// </summary>
    public class SynchronousAwaiter
    {
        /// <summary>
        /// The manual reset event signaling completion.
        /// </summary>
        private readonly ManualResetEvent _manualResetEvent;

        /// <summary>
        /// The exception thrown by the asynchronous operation.
        /// </summary>
        private Exception? _exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronousAwaiter{TResult}"/> class.
        /// </summary>
        /// <param name="task">
        /// The task representing an asynchronous operation.
        /// </param>
        /// <param name="ignoreCancellation">
        /// Indicates whether to ignore cancellation. Default is false.
        /// </param>
        public SynchronousAwaiter(Task task, bool ignoreCancellation = false)
        {
            _manualResetEvent = new ManualResetEvent(false);
            WaitFor(task, ignoreCancellation);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronousAwaiter{TResult}"/> class.
        /// </summary>
        /// <param name="task">
        /// The task representing an asynchronous operation.
        /// </param>
        /// <param name="ignoreCancellation">
        /// Indicates whether to ignore cancellation. Default is false.
        /// </param>
        public SynchronousAwaiter(ValueTask task, bool ignoreCancellation = false)
        {
            _manualResetEvent = new ManualResetEvent(false);
            WaitFor(task, ignoreCancellation);
        }

        /// <summary>
        /// Gets a value indicating whether the operation is complete.
        /// </summary>
        public bool IsComplete => _manualResetEvent.WaitOne(0);

        /// <summary>
        /// Synchronously get the result of an asynchronous operation.
        /// </summary>
        public void GetResult()
        {
            _manualResetEvent.WaitOne();
            if (_exception != null)
            {
                throw _exception;
            }
        }

        /// <summary>
        /// Background "thread" which waits for the specified asynchronous operation to complete.
        /// </summary>
        /// <param name="task">
        /// The task.
        /// </param>
        /// <param name="ignoreCancellation">
        /// Indicates whether to ignore cancellation. Default is false.
        /// </param>
        private async void WaitFor(Task task, bool ignoreCancellation)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                _exception = exception;
            }
            finally
            {
                _manualResetEvent.Set();
            }
        }

        /// <summary>
        /// Background "thread" which waits for the specified asynchronous operation to complete.
        /// </summary>
        /// <param name="task">
        ///     The task.
        /// </param>
        /// <param name="ignoreCancellation">
        /// Indicates whether to ignore cancellation. Default is false.
        /// </param>
        private async void WaitFor(ValueTask task, bool ignoreCancellation)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception exception)
            {
                _exception = exception;
            }
            finally
            {
                _manualResetEvent.Set();
            }
        }
    }
}
