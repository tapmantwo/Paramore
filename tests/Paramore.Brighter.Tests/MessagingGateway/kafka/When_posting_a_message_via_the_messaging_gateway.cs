﻿#region Licence
/* The MIT License (MIT)
Copyright © 2014 Wayne Hunsley <whunsley@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the “Software”), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

#endregion

using System;
using FluentAssertions;
using Paramore.Brighter.Tests.CommandProcessors.TestDoubles;
using Xunit;

namespace Paramore.Brighter.Tests.MessagingGateway.Kafka
{
    [Collection("Kafka")]
    [Trait("Category", "Kafka")]
    public class KafkaMessageProducerSendTests : KafkaIntegrationTestBase
    {
        private const string Topic = "test";

        private string QueueName { get; set; }

        public KafkaMessageProducerSendTests()
        {
            QueueName = Guid.NewGuid().ToString();
        }
        
        [Theory, MemberData(nameof(ServerParameters))]
        public void When_posting_a_message_via_the_messaging_gateway(string bootStrapServer)
        {
            using (var consumer = this.CreateMessageConsumer<MyCommand>("TestConsumer", bootStrapServer, QueueName, Topic))
            using (var producer = CreateMessageProducer("TestProducer", bootStrapServer))
            {
                var message = CreateMessage(Topic, $"test content [{QueueName}]");
                consumer.Receive(30000); 
                producer.Send(message);
                var receivedMessage = consumer.Receive(30000);
                var receivedMessageData = receivedMessage.Body.Value;

                consumer.Acknowledge(receivedMessage);

                receivedMessageData.Should().Be(message.Body.Value);
            }
        }
    }
}
