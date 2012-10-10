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

namespace MP4ToSmoothStream
{
    class Program
    {
        const string configuration = 
@"<taskDefinition xmlns=""http://schemas.microsoft.com/iis/media/v4/TM/TaskDefinition#""> 
  <name>MP4 to Smooth Streams</name>
  <id>5e1e1a1c-bba6-11df-8991-0019d1916af0</id>
  <description xml:lang=""en"">Converts MP4 files encoded with H.264 (AVC) video and AAC-LC audio codecs to Smooth Streams.</description>
  <inputFolder />
  <properties namespace=""http://schemas.microsoft.com/iis/media/V4/TM/MP4ToSmooth#"" prefix=""mp4"">
    <property name=""keepSourceNames"" required=""false"" value=""true"" helpText=""This property tells the MP4 to Smooth task to keep the original file name rather than add the bitrate bitrate information."" />
  </properties>
  <taskCode>
    <type>Microsoft.Web.Media.TransformManager.MP4toSmooth.MP4toSmooth_Task, Microsoft.Web.Media.TransformManager.MP4toSmooth, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35</type>
  </taskCode>
</taskDefinition>";

        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("MP4ToSmoothStream <asset-id>");
                return -1;
            }

            string accountName = Environment.GetEnvironmentVariable("ACCOUNT_NAME");
            string accountKey = Environment.GetEnvironmentVariable("ACCOUNT_KEY");
            CloudMediaContext cloudMediaContext = new CloudMediaContext(accountName, accountKey);

            string assetId = args[0];
            IAsset asset = cloudMediaContext.GetAssetById(assetId);

            IJob job = cloudMediaContext.Jobs.Create(string.Format("Convert {0} to Smooth Stream", asset.Name));

            IMediaProcessor processor = cloudMediaContext.GetMediaProcessor("MP4 to Smooth Streams Task");

            ITask task = job.Tasks.AddNew("MP4 to Smooth Stream Conversion",
                processor,
                configuration,
                Microsoft.WindowsAzure.MediaServices.Client.TaskCreationOptions.ProtectedConfiguration);

            task.InputMediaAssets.Add(asset);

            task.OutputMediaAssets.AddNew(string.Format("Smooth Stream for {0}", asset.Name),
                true,
                AssetCreationOptions.None);

            job.Submit();

            Console.WriteLine(job.Id);
            return 0;
        }
    }
}