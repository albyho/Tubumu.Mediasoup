using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tubumu.Mediasoup
{
    public abstract class ChannelBase : IChannel
    {
        #region Constants

        protected const int MessageMaxLen = 4194308;

        protected const int PayloadMaxLen = 4194304;

        #endregion Constants

        #region Private Fields

        /// <summary>
        /// Logger
        /// </summary>
        protected readonly ILogger<Channel> _logger;

        // TODO: (alby) _closed 的使用及线程安全。
        /// <summary>
        /// Closed flag.
        /// </summary>
        protected bool _closed;

        /// <summary>
        /// Worker id.
        /// </summary>
        protected readonly int _workerId;

        /// <summary>
        /// Next id for messages sent to the worker process.
        /// </summary>
        protected uint _nextId = 0;

        #endregion Private Fields

        #region Events

        public abstract event Action<string, string, string?>? MessageEvent;

        #endregion Events

        public ChannelBase(ILogger<Channel> logger, int workerId)
        {
            _logger = logger;
            _workerId = workerId;
        }

        public abstract void Close();

        public abstract Task<string?> RequestAsync(MethodId methodId, object? @internal = null, object? data = null);

        #region Event handles

        protected virtual void ConsumerSocketOnData(string payloadString)
        {
            try
            {
                // We can receive JSON messages (Channel messages) or log strings.
                var message = $"ConsumerSocketOnData() | Worker [pid:{_workerId}] payload: {payloadString}";
                switch (payloadString[0])
                {
                    // 123 = '{' (a Channel JSON messsage).
                    case '{':
                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            ProcessMessage(payloadString);
                        });
                        break;

                    // 68 = 'D' (a debug log).
                    case 'D':
                        if (!payloadString.Contains("(trace)"))
                        {
                            _logger.LogDebug(message);
                        }

                        break;

                    // 87 = 'W' (a warn log).
                    case 'W':
                        if (!payloadString.Contains("no suitable Producer"))
                        {
                            _logger.LogWarning(message);
                        }

                        break;

                    // 69 = 'E' (an error log).
                    case 'E':
                        _logger.LogError(message);
                        break;

                    // 88 = 'X' (a dump log).
                    case 'X':
                        _logger.LogDebug(message);
                        break;

                    default:
                        _logger.LogWarning($"ConsumerSocketOnData() | Worker [pid:{_workerId}] unexpected data, payload: {payloadString}");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ConsumerSocketOnData() | Worker [pid:{_workerId}] Received invalid message from the worker process, payload: {payloadString}");
                return;
            }
        }

        protected abstract void ProcessPayload(string payload);

        #endregion Event handles
    }
}
