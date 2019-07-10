using System;
using System.IO;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace dotnet_ansible_vault_decoder
{
    [Command("encode", "encoding to encrypted ansible vault")]
    class EncodeCommand
    {
        [Option("-i|--input", "input data file(default stdin)", CommandOptionType.SingleValue)]
        public string FilePath { get; set; }
        [Option("-o|--output", "output file path(default stdout)", CommandOptionType.SingleValue)]
        public string OutputPath { get; set; }
        [Option("-e|--eol", "output end of line(cr, crlf, lf, if not set, using system default value)", CommandOptionType.SingleValue)]
        public string EndOfLine { get; set; }
        [Option("-p|--password", "encryption password, if not set, input via prompt", CommandOptionType.SingleValue)]
        public string Password { get; set; }
        [Option("--passfile", "password file(using first line, trailing whitespace will be trimmed", CommandOptionType.SingleValue)]
        public string Passfile { get; set; }
        [Option("--passenv", "get password from environment variable", CommandOptionType.SingleValue)]
        public string PassEnv { get; set; }
        [Option("-l|--label", "vault label", CommandOptionType.SingleValue)]
        public string Label { get; set; }
        public int OnExecute()
        {
            try
            {
                var codec = new AnsibleVaultCodec();
                using (var stm = ArgumentUtil.GetInputStream(FilePath))
                using (var ostm = ArgumentUtil.GetOutputStream(OutputPath, true))
                {
                    using (var sw = new StreamWriter(ostm, new UTF8Encoding(false)))
                    {
                        var lst = new List<byte>();
                        var buf = new byte[4096];
                        while (true)
                        {
                            var bytesread = stm.Read(buf, 0, 4096);
                            if (bytesread <= 0)
                            {
                                break;
                            }
                            lst.AddRange(buf.Take(bytesread));
                        }
                        var eolString = ArgumentUtil.EolOptionToEolString(EndOfLine);
                        if (eolString != null)
                        {
                            sw.NewLine = eolString;
                        }
                        codec.Encode(lst.ToArray(), ArgumentUtil.GetPassword(Password, Passfile, PassEnv), CreateSalt(), sw, Label, 80);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"error in encoding: {e}");
                return 1;
            }
            return 0;
        }
        byte[] CreateSalt()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var data = new byte[32];
                rng.GetBytes(data);
                return data;
            }
        }
    }

}