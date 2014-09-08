using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using iclickpro.AccessCommon;
using RabbitMQ.Client;
using TaoBaoDataServer.WinClientData.Model;
using RabbitMQ.Client.Events;

namespace TaoBaoDataServer.WinClientData.BusinessLayer
{
    /// <summary>
    /// 消息队列的操作类
    /// </summary>
    public class BusinessMQ
    {

        private static ILog logAX = log4net.LogManager.GetLogger("Logger");

        private static readonly string Const_MQ_HostName = CommonFunction.GetAppSetting("MQ_HostName");
        private static readonly string Const_MQ_Port = CommonFunction.GetAppSetting("MQ_Port");
        private static readonly string Const_MQ_UserName = CommonFunction.GetAppSetting("MQ_UserName");
        private static readonly string Const_MQ_Password = CommonFunction.GetAppSetting("MQ_Password");
        private static readonly string Const_MQ_VirtualHost = CommonFunction.GetAppSetting("MQ_VirtualHost");

        /// <summary>
        /// 消息队列工厂，创建连接
        /// </summary>
        private static ConnectionFactory factoryMQ = new ConnectionFactory() { HostName = Const_MQ_HostName, Port = Convert.ToInt32(Const_MQ_Port), UserName = Const_MQ_UserName, Password = Const_MQ_Password, VirtualHost = Const_MQ_VirtualHost };

        /// <summary>
        /// 向exchange，发送消息
        /// </summary>
        public static void SendMsgToExchange(TopSession user, string exchangeName, string msg)
        {
            try
            {
                using (var connection = factoryMQ.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare(exchangeName, "fanout", true);

                        string message = msg;
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchangeName, "", null, body);
                        if (user != null)
                            logAX.InfoFormat(string.Format("发送用户:{0}，消息:{1}，成功", user.UserName, msg));
                        else
                            logAX.InfoFormat(string.Format("发送消息:{0}，成功", msg));
                    };
                };
            }
            catch (Exception se)
            {
                logAX.Error(string.Format("发送用户:{0}，消息:{1}，失败", user.UserName, msg), se);
            }

        }

        /// <summary>
        /// 向消息队列，发送消息
        /// </summary>
        public static void SendMsgToQueue(TopSession user, string queueName, string msg)
        {
            try
            {
                using (var connection = factoryMQ.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queueName, false, false, false, null);

                        string message = msg;
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish("", queueName, null, body);
                        logAX.InfoFormat(string.Format("发送用户:{0}，消息:{1}，成功", user.UserName, msg));
                    };
                };
            }
            catch (Exception se)
            {
                logAX.Error(string.Format("发送用户:{0}，消息:{1}，失败", user.UserName, msg), se);
            }

        }

        /// <summary>
        /// 从消息队列中接收消息，基于订阅模式
        /// </summary>
        public static string ReceiveOneMsg(string queueName)
        {
            try
            {
                using (var connection = factoryMQ.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        //删除队列
                        //var result = channel.QueueDelete(queueName);

                        //消费队列
                        channel.QueueDeclare(queueName, false, false, false, null);

                        var consumer = new QueueingBasicConsumer(channel);
                        channel.BasicConsume(queueName, true, consumer);

                        var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        string message = Encoding.UTF8.GetString(body);
                        return message;
                    }
                }
            }
            catch (Exception se)
            {
                logAX.Error("接受消息失败，订阅模式", se);
                return null;
            }
        }

        /// <summary>
        /// 从消息队列中接收消息，主动获取模式
        /// </summary>
        public static List<string> ReceiveLstMsg(string queueName)
        {
            List<string> lstMsg = new List<string>();
            try
            {
                using (var connection = factoryMQ.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queueName, false, false, false, null);
                        while (true)
                        {
                            BasicGetResult res = channel.BasicGet(queueName, true);
                            if (res != null)
                            {
                                string message = System.Text.UTF8Encoding.UTF8.GetString(res.Body);
                                lstMsg.Add(message);
                            }
                            else
                                break;
                        }
                    }
                }
            }
            catch (Exception se)
            {
                logAX.Error("接受消息失败，主动获取模式", se);
                return null;
            }
            return lstMsg;
        }
    }
}
