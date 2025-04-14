﻿/*/
 *
 * this tool was made for T6_greenlight_mp, but it *should* support all versions of T5 and T6.
 * keyword: should
 *
 * it also only support's xbox 360 (ps3 and wii u are untested but should work for them)
 * pc is unsupported however you *can* unlink pc wad's, you just have to swap around the magic's bytes
 * i will need to get my hands on a ps3 and wii u wad to properly confirm... this likely won't happen though
 *
/*/

using System;
using System.IO;
using System.Collections.Generic;

using static Utils.Structs;
using static Utils.Reader;
using static Utils.Writer;

namespace Wad
{
    class Wad
    {
        static void UnlinkWAD(string FileName)
        {
            // tell the user what .wad we are unlinking
            Console.WriteLine($"Unlinking: {FileName}..\n");

            try
            {
                List<WADEntry> Entries = ProcessOnlineWAD(File.ReadAllBytes(FileName));
                if (Entries != null)
                {
                    string OutputDirectory = Path.Combine(
                        ".", Path.GetFileNameWithoutExtension(GetFilename(FileName)));
                    // check if the output directory exists
                    CreateOutputDirectory(OutputDirectory);

                    UnlinkEntries(Entries, File.ReadAllBytes(FileName), OutputDirectory);

                    //all good!
                    Console.WriteLine("\nDone!");
                }
            }
            catch (Exception MSG)
            {
                Console.WriteLine($"Failed to unlink: {FileName}!");
                Console.WriteLine($"Reason: {MSG.Message}");
                return;
            }
        }

        static void LinkWAD(string FolderName)
        {
            // tell the user what .wad we are linking
            Console.WriteLine($"Linking: {FolderName}..\n");

            try
            {
                using (var Stream = new MemoryStream())
                using (var Writer = new BinaryWriter(Stream))
                {
                    WAD WADFile = WriteOnlineWAD(Directory.GetFiles(FolderName));
                    WriteWADHeader(Writer, WADFile.header);
                    WriteWADEntries(Writer, WADFile.entries);
                    WriteCompressedData(Writer, WADFile.entries);

                    // okay let's save the file now
                    File.WriteAllBytes(
                        Path.GetFileName(FolderName) + ".wad", Stream.ToArray());

                    //all good!
                    Console.WriteLine("\nDone!");
                }
            }
            catch (Exception MSG)
            {
                Console.WriteLine($"Failed to link: {FolderName}!");
                Console.WriteLine($"Reason: {MSG.Message}");
                return;
            }
        }

        static void Unlink(string[] Parameters)
        {
            // first check if there are any parameters
            if (Parameters.Length < 2)
            {
                Console.WriteLine("USAGE :: --unlink <input.wad>");
                return;
            }

            // small check to see if the .wad is already unlinked
            if (!Parameters[1].EndsWith(".wad", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(
                    "WARNING :: trying to unlink a non .wad file!");
                Console.WriteLine(
                    "        :: you might be trying to unlink an already unlinked .wad.");
                Console.WriteLine(
                    "        :: if not, add .wad extension to your command or check your file name.");
                return;
            }

            // okay all good, unlink it now
            UnlinkWAD(Parameters[1]);
        }

        static void Link(string[] Parameters)
        {
            // first check if there are any parameters
            if (Parameters.Length < 2)
            {
                Console.WriteLine("USAGE :: --link <input folder>");
                return;
            }

            // small check to see if the .wad is already linked
            if (Parameters[1].EndsWith(".wad", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(
                    "WARNING :: trying to link an already linked .wad file!");
                Console.WriteLine(
                    "        :: if not, remove the .wad extension from your command or check your file name.");
                return;
            }

            // okay all good, link it now
            LinkWAD(Parameters[1]);
        }

        static void Help()
        {
            // just general help for the tool

            Console.WriteLine("command usages:");
            Console.WriteLine("--unlink   <input .wad>   ::  unlinks the inputted .wad file.");
            Console.WriteLine("  shortcut                :: -u");
            Console.WriteLine("--link     <input folder> ::  links the inputted folder into a .wad file.");
            Console.WriteLine("  shortcut                :: -l");
            Console.WriteLine("--help                    ::  displays help for various commands.");
            Console.WriteLine("  shortcut                :: -h");
            Console.WriteLine("--about                   ::  displays information about this tool.");
            Console.WriteLine("  shortcut                :: -a");
        }

        static void About()
        {
            Console.WriteLine("tool information:");
            Console.WriteLine("wad.exe :: a linker / unlinker tool for 3arc's .wad file type");
            Console.WriteLine("        :: made by ymes_zzz");
        }

        /*/
         * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
        /*/

        static void Main(string[] Parameters)
        {
            // first check if there are any parameters
            if (Parameters.Length < 1)
            {
                Console.WriteLine("USAGE :: wad.exe <command>");
                return;
            }

            // now check what the user wants to do

            switch (Parameters[0])
            {
                case "-u":
                case "--unlink":
                    Unlink(Parameters);
                    break;
                case "-l":
                case "--link":
                    Link(Parameters);
                    break;
                case "-h":
                case "--help":
                    Help();
                    break;
                case "-a":
                case "--about":
                    About();
                    break;
                default:
                    Console.WriteLine("ERROR :: unknown command!");
                    return;
            }
        }
    }
}