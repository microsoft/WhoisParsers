// -----------------------------------------------------------------------
// <copyright file="RWhoisConsumer.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.RWhois.Crawler
{
    using System;
    using System.IO;
    using System.Text;
    using Microsoft.Geolocation.Whois.Parsers;
    using NetTools;

    public class RWhoisConsumer : IObserver<RawWhoisSection>
    {
        private IDisposable unsubscriber;

        private StreamWriter outFile;

        public RWhoisConsumer(string outFilePath)
        {
            this.outFile = new StreamWriter(outFilePath);
        }

        public virtual void Subscribe(IObservable<RawWhoisSection> provider)
        {
            if (provider != null)
            {
                this.unsubscriber = provider.Subscribe(this);
            }
        }

        public void OnCompleted()
        {
            this.outFile.Close();
            ////Console.WriteLine("Done receiving data!");
            this.Unsubscribe();
        }

        public void OnError(Exception error)
        {
            this.outFile.Close();
        }

        public void OnNext(RawWhoisSection section)
        {
            if (section.Records != null)
            {
                StringBuilder rawNetwork;

                if (section.Records.TryGetValue("IP-Network", out rawNetwork))
                {
                    this.outFile.Write(section);
                    this.outFile.WriteLine();
                    this.outFile.Flush();
                    ////Console.WriteLine("Got one: " + IPAddressRange.Parse(rawNetwork.ToString()));
                }
                else
                {
                    ////Console.WriteLine("Got one but could not find IP-Network record");
                }
            }
            else
            {
                ////Console.WriteLine("Got one! No records?!");
            }
        }

        public virtual void Unsubscribe()
        {
            this.unsubscriber.Dispose();
        }
    }
}
