using ArdNet.Client.DependencyInjection;
using System.Diagnostics;
using System.Windows;
using TIPC.Core.Channels;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ArdClientExtensions
    {
        /// <summary>
        /// Add ArdNet logger hooks if compiled for DEBUG target.
        /// Prints errors to messagebox
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ArdNetInjectionClientBuilder AddDebugLogger(this ArdNetInjectionClientBuilder config)
        {
            AddDebugLoggerHelper(config);
            return config;
        }

        [Conditional("DEBUG")]
        private static void AddDebugLoggerHelper(this ArdNetInjectionClientBuilder config)
        {
            _ = config.AddSystemModifier((sp, system) =>
            {
                var msgHub = system.MessageHub;

                static void HandleInfo(object sender, IChannelEventArgs log)
                {
                    _ = MessageBox.Show(log.ToString(), "INFO");
                }
                static void HandleLog(object sender, ChannelLoggingArgs log)
                {
                    switch (log.Severity)
                    {
                        case LogSeverity.Warning:
                            {
                                var msg = log.Message.ToString();
                                _ = MessageBox.Show(msg, "Warning." + log.Severity);
                                break;
                            }
                        case LogSeverity.Error:
                        case LogSeverity.Failure:
                            {
                                var msg = log.Message.ToString();
                                _ = MessageBox.Show(msg, "Error." + log.Severity);
                                break;
                            }
                        default:
                            {
                                var msg = log.Message.ToString();
                                _ = MessageBox.Show(msg, "Info." + log.Severity);
                                break;
                            }
                    };
                }

                MessageCategoryTypes LoggingCaptureTypes =
                    MessageCategoryTypes.ExceptionMessages
                    | MessageCategoryTypes.LoggingMessages
                    | MessageCategoryTypes.InfoMessages;

                var LogClient = new LoggingMessageHubClient(msgHub, LoggingCaptureTypes);
                LogClient.MessagePushed += HandleInfo;
                LogClient.LogPushed += HandleLog;
                LogClient.ExceptionPushed += (sender, log) =>
                {
                    if (log.Severity == ExceptionSeverity.Expected)
                    {
                        return;
                    }
                    var msg = log.ToString();
                    _ = MessageBox.Show(msg, "Error." + log.Severity);
                };
                LogClient.InformationPushed += HandleInfo;
                LogClient.Start();

            });
        }

        /// <summary>
        /// Causes ArdNet internal messages to crash the application
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static ArdNetInjectionClientBuilder AddReleaseCrasher(this ArdNetInjectionClientBuilder config)
        {
            AddReleaseCrasherHelper(config);
            return config;
        }

        [Conditional("RELEASE")]
        private static void AddReleaseCrasherHelper(this ArdNetInjectionClientBuilder config)
        {
            _ = config.AddSystemModifier((sp, system) =>
            {
                var msgHub = system.MessageHub;

                static void HandleLog(object sender, ChannelLoggingArgs log)
                {
                    switch (log.Severity)
                    {
                        case LogSeverity.Error:
                        case LogSeverity.Failure:
                            {
                                Application.Current.Shutdown();
                                break;
                            }
                    };
                }

                MessageCategoryTypes LoggingCaptureTypes =
                    MessageCategoryTypes.ExceptionMessages
                    | MessageCategoryTypes.LoggingMessages
                    | MessageCategoryTypes.InfoMessages;

                var LogClient = new LoggingMessageHubClient(msgHub, LoggingCaptureTypes);
                LogClient.LogPushed += HandleLog;
                LogClient.ExceptionPushed += (sender, log) =>
                {
                    Application.Current.Shutdown();
                };
                LogClient.Start();

            });
        }
    }
}
