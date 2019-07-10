using System;
using System.IO;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace dotnet_ansible_vault_decoder
{
    [Command("decode", "decoding ansible vault data")]
    class DecodeCommand
    {
        [Option("-i|--input", "input data file(default stdin)", CommandOptionType.SingleValue)]
        public string FilePath { get; set; }
        [Option("-o|--output", "output file path(default stdout)", CommandOptionType.SingleValue)]
        public string OutputPath { get; set; }
        [Option("-p|--password", "encryption password, if not set, input via prompt", CommandOptionType.SingleValue)]
        public string Password { get; set; }
        [Option("--passfile", "password file(using first line, trailing whitespace will be trimmed", CommandOptionType.SingleValue)]
        public string Passfile { get; set; }
        [Option("--passenv", "get password from environment variable", CommandOptionType.SingleValue)]
        public string PassEnv { get; set; }
        public int OnExecute()
        {
            try
            {
                var codec = new AnsibleVaultCodec();
                using (var stm = ArgumentUtil.GetInputStream(FilePath))
                using (var ostm = ArgumentUtil.GetOutputStream(OutputPath, true))
                {
                    using (var sr = new StreamReader(stm, Encoding.UTF8))
                    {
                        codec.Decode(sr, ArgumentUtil.GetPassword(Password, Passfile, PassEnv), ostm);
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"decoding error: {e}");
                return 1;
            }
            return 0;
        }

    }
}