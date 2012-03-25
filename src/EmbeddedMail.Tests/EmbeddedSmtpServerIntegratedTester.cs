﻿using System.Linq;
using System.Net.Mail;
using FubuTestingSupport;
using NUnit.Framework;

namespace EmbeddedMail.Tests
{
    [TestFixture]
    public class EmbeddedSmtpServerIntegratedTester
    {
        private EmbeddedSmtpServer theServer;
        private MailMessage theMessage;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            var port = PortFinder.FindPort(8181);
            theServer = new EmbeddedSmtpServer(port);
            theMessage = new MailMessage("x@domain.com", "y@domain.com", "Hello there", "O hai");
            theMessage.CC.Add("copy@domain.com");
            theMessage.Bcc.Add("blind@domain.com");
            theServer.Start();

            using (var client = new SmtpClient("localhost", 8181))
            {
                client.Send(theMessage);
                client.Send(theMessage);
            }

            theServer.Stop();
        }

        [Test]
        public void receives_the_message()
        {
            var message = theServer.ReceivedMessages().First();
            message.From.ShouldEqual(theMessage.From);
            message.To.First().ShouldEqual(theMessage.To.First());
            message.Subject.ShouldEqual(theMessage.Subject);
            message.Body.ShouldEqual(theMessage.Body);

            message.CC.Single().ShouldEqual(theMessage.CC.Single());
            message.Bcc.Single().ShouldEqual(theMessage.Bcc.Single());
        }

        [Test]
        public void receives_multiple_messages()
        {
            theServer.ReceivedMessages().ShouldHaveCount(2);
        }
    }
}