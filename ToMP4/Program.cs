﻿#region Copyright (c) 2012 Two10degrees and Active Web Solutions Ltd
//
// (C) Copyright 2012 Two10degrees and Active Web Solutions Ltd
//      All rights reserved.
//
// This software is provided "as is" without warranty of any kind,
// express or implied, including but not limited to warranties as to
// quality and fitness for a particular purpose. Active Web Solutions Ltd
// does not support the Software, nor does it warrant that the Software
// will meet your requirements or that the operation of the Software will
// be uninterrupted or error free or that any defects will be
// corrected. Nothing in this statement is intended to limit or exclude
// any liability for personal injury or death caused by the negligence of
// Active Web Solutions Ltd, its employees, contractors or agents.
//
#endregion

using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using Two10.MediaServices;

namespace ToMP4
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("ToMP4 <asset-id>");
                return -1;
            }

            string accountName = Environment.GetEnvironmentVariable("ACCOUNT_NAME");
            string accountKey = Environment.GetEnvironmentVariable("ACCOUNT_KEY");
            CloudMediaContext cloudMediaContext = new CloudMediaContext(accountName, accountKey);

            string assetId = args[0];
            IAsset asset = cloudMediaContext.GetAssetById(assetId);

            IJob job = cloudMediaContext.Jobs.Create(string.Format("Convert {0} to MP4", asset.Name));

            IMediaProcessor processor = cloudMediaContext.GetMediaProcessor("Windows Azure Media Encoder");

            ITask task = job.Tasks.AddNew("MP4 Conversion",
                processor,
                "H.264 256k DSL CBR",
                Microsoft.WindowsAzure.MediaServices.Client.TaskCreationOptions.None);

            task.InputMediaAssets.Add(asset);

            // This name doesnt seem to get used .. bug?

            task.OutputMediaAssets.AddNew(string.Format("MP4 for {0}", asset.Name),
                true,
                AssetCreationOptions.None);
 
            job.Submit();

            Console.WriteLine(job.Id);

            return 0;

        }
    }
}