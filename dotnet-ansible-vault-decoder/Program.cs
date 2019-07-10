using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using McMaster.Extensions.CommandLineUtils;

[assembly: InternalsVisibleTo("DotNet.Ansible.Vault.Decoder.Test")]

namespace dotnet_ansible_vault_decoder
{
    [Subcommand(typeof(EncodeCommand), typeof(DecodeCommand))]
    [Command("ansible-vault-decoder")]
    [VersionOptionFromMember(MemberName = "VersionString")]
    class MainCommand
    {
        public string VersionString => $"dotnet-anv {typeof(Program).Assembly.GetName().Version.ToString()}";
    }
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            return await CommandLineApplication.ExecuteAsync<MainCommand>(args).ConfigureAwait(false);
        }
    }

    // public class EncodeVault : BatchBase
    // {
    //     [Command("decode", "decoding encrypted vault data")]
    //     public void Decode(string filePath = null, string password = null, string output = null)
    //     {
    //         var codec = new AnsibleVaultCodec();
    //         using (var stm = GetInputStream(filePath))
    //         using (var ostm = GetOutputStream(output, true))
    //         {
    //             using (var sr = new StreamReader(stm, Encoding.UTF8))
    //             {
    //                 codec.Decode(sr, GetPassword(password), ostm);
    //             }
    //         }
    //     }
    //     [Command("encode")]
    //     public void Encode(string filePath = null, string password = null, string output = null, string eol = null, string label = null)
    //     {
    //         var codec = new AnsibleVaultCodec();
    //         using (var stm = GetInputStream(filePath))
    //         using (var ostm = GetOutputStream(output, true))
    //         {
    //             using (var sw = new StreamWriter(ostm, new UTF8Encoding(false)))
    //             {
    //                 var lst = new List<byte>();
    //                 var buf = new byte[4096];
    //                 while (true)
    //                 {
    //                     var bytesread = stm.Read(buf, 0, 4096);
    //                     if (bytesread <= 0)
    //                     {
    //                         break;
    //                     }
    //                     lst.AddRange(buf.Take(bytesread));
    //                 }
    //                 var eolString = EolOptionToEolString(eol);
    //                 if (eolString != null)
    //                 {
    //                     sw.NewLine = eolString;
    //                 }
    //                 codec.Encode(lst.ToArray(), GetPassword(password), CreateSalt(), sw, label, 80);
    //             }
    //         }
    //     }
    // }
}
