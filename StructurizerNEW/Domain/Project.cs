﻿using System;
using System.IO;
using System.Linq;
using StructurizerNEW.Templating;

namespace StructurizerNEW.Domain
{
    public class Project : FolderEntity
    {
        public const bool OpenAfterManualBuildTrigger = true;

        public Project(DirectoryInfo path) : base(path, "project")
        {
            foreach (var chapterDir in SubDirectories.OrderBy(k => k.Name, new MixedNumbersAndStringsComparer()))
            {
                var chapter = new Chapter(chapterDir, 1, Path + MetaData.OutputDir);
                chapter.Parent = this;
                Children.Add(chapter);
            }
        }

        public override void Process()
        {
            var templater = new HtmlTemplater();

            //new VisioToPngProcessor().FindAndFlattenVSDs(Path.FullName);

            foreach (var chapter in Children)
            {
                chapter.Process();
            }

            var result = templater.Process(this);

            if (OpenAfterManualBuildTrigger)
            {
                if (File.Exists(result.GeneratedFilename))
                {
                    System.Diagnostics.Process.Start(result.GeneratedFilename);
                }
                else
                {
                    Console.WriteLine("Failed to find generated file");
                }                
            }
        }
    }
}