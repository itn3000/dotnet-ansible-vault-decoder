using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using MicroBatchFramework;

namespace dotnet_ansible_vault_decoder
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await BatchHost.CreateDefaultBuilder().RunBatchEngineAsync(args);
        }
    }

    class EncodeVault : BatchBase
    {
        Stream GetInputStream(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                return File.OpenRead(filePath);
            }
            else
            {
                return Console.OpenStandardInput();
            }
        }
        Stream GetOutputStream(string filePath, bool truncate)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                if (truncate)
                {
                    return File.Create(filePath);
                }
                else
                {
                    var ret = File.OpenWrite(filePath);
                    ret.Seek(0, SeekOrigin.End);
                    return ret;
                }
            }
            else
            {
                return Console.OpenStandardOutput();
            }
        }
        byte[] GetPassword(string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                return Encoding.UTF8.GetBytes(password);
            }
            else
            {
                password = Environment.GetEnvironmentVariable("ANSIBLE_VAULT_PASS");
                return Encoding.UTF8.GetBytes(password);
            }
        }
        public void Decode(string filePath = null, string password = null, string output = null)
        {
            var codec = new AnsibleVaultCodec();
            using (var stm = GetInputStream(filePath))
            using (var ostm = GetOutputStream(output, true))
            {
                using (var sr = new StreamReader(stm, Encoding.UTF8))
                {
                    codec.Decode(sr, GetPassword(password), ostm);
                }
            }
        }
        string EolOptionToEolString(string eol)
        {
            if (string.IsNullOrEmpty(eol))
            {
                return null;
            }
            if (eol.Equals("crlf", StringComparison.OrdinalIgnoreCase))
            {
                return "\r\n";
            }
            else if (eol.Equals("lf", StringComparison.OrdinalIgnoreCase))
            {
                return "\n";
            }
            else if (eol.Equals("cr", StringComparison.OrdinalIgnoreCase))
            {
                return "\r";
            }
            else
            {
                return null;
            }
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
        public void Encode(string filePath = null, string password = null, string output = null, string eol = null, string label = null)
        {
            var codec = new AnsibleVaultCodec();
            using (var stm = GetInputStream(filePath))
            using (var ostm = GetOutputStream(output, true))
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
                    var eolString = EolOptionToEolString(eol);
                    if (eolString != null)
                    {
                        sw.NewLine = eolString;
                    }
                    codec.Encode(lst.ToArray(), GetPassword(password), CreateSalt(), sw, label, 80);
                }
            }
        }
    }
}
