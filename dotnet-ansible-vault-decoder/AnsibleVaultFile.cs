using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace dotnet_ansible_vault_decoder
{
    class AnsibleVaultFile
    {
        const string AnsibleVaultSignature = "$ANSIBLE_VAULT";
        public string Version { get; set; }
        public string Algorithm { get; set; }
        public string Label { get; set; }
        public byte[] Salt { get; set; }
        public byte[] ExpectedHMac { get; set; }
        public byte[] EncryptedBytes { get; set; }
        public static AnsibleVaultFile Load(TextReader sr)
        {
            var ret = new AnsibleVaultFile();
            var headerString = sr.ReadLine();
            var header = headerString.Split(';');
            if (header[0] != AnsibleVaultSignature)
            {
                throw new ArgumentException($"invalid ansible vault header:{headerString}");
            }
            ret.Version = header[1];
            ret.Algorithm = header[2];
            if(header.Length > 3)
            {
                ret.Label = header[3];
            }
            var bodyBytes = new List<byte>();
            while (true)
            {
                var line = sr.ReadLine();
                if (line == null)
                {
                    break;
                }
                ByteUtil.ConvertToBytes(line, bodyBytes);
            }
            var (salt, expectedhmac, encrypted_bytes) = DecodeBody(bodyBytes.ToArray());
            ret.Salt = salt;
            ret.ExpectedHMac = expectedhmac;
            ret.EncryptedBytes = encrypted_bytes;
            return ret;
        }
        public static AnsibleVaultFile Load(string filePath)
        {
            using (var stm = File.OpenRead(filePath))
            using (var sr = new StreamReader(stm, Encoding.UTF8))
            {
                return Load(sr);
            }
        }
        public static void Save(AnsibleVaultFile f, TextWriter output, string label, int width = 80)
        {
            if(string.IsNullOrEmpty(f.Label))
            {
                output.WriteLine($"{AnsibleVaultSignature};{f.Version};{f.Algorithm}");
            }
            else
            {
                output.WriteLine($"{AnsibleVaultSignature};{f.Version};{f.Algorithm};{f.Label}");
            }
            var data = new byte[f.Salt.Length + f.ExpectedHMac.Length + f.EncryptedBytes.Length + 2];
            var dataSpan = data.AsSpan();
            f.Salt.AsSpan().CopyTo(dataSpan);
            dataSpan[f.Salt.Length] = 0x0a;
            f.ExpectedHMac.AsSpan().CopyTo(dataSpan.Slice(f.Salt.Length + 1));
            dataSpan[f.Salt.Length + 1 + f.ExpectedHMac.Length] = 0x0a;
            f.EncryptedBytes.AsSpan().CopyTo(dataSpan.Slice(f.Salt.Length + f.ExpectedHMac.Length + 2));
            Span<char> buffer = stackalloc char[width];
            while (!dataSpan.IsEmpty)
            {
                var len = Math.Min(40, dataSpan.Length);
                ByteUtil.ConvertToHexChars(dataSpan.Slice(0, len), buffer);
                output.WriteLine(buffer.Slice(0, len * 2));
                dataSpan = dataSpan.Slice(len);
            }
        }
        static (byte[] salt, byte[] expectedhmac, byte[] encryptedbytes) DecodeBody(byte[] body)
        {
            byte[] salt = null, expectedhmac = null, encrypted_bytes = null;
            var sp = body.AsSpan();
            for (int i = 0; i < 3; i++)
            {
                var idx = sp.IndexOf((byte)0x0a);
                if (idx < 0)
                {
                    encrypted_bytes = ByteUtil.ConvertToBytes(Encoding.ASCII.GetString(sp));
                    break;
                }
                switch (i)
                {
                    case 0:
                        salt = ByteUtil.ConvertToBytes(Encoding.ASCII.GetString(sp.Slice(0, idx)));
                        break;
                    case 1:
                        expectedhmac = ByteUtil.ConvertToBytes(Encoding.ASCII.GetString(sp.Slice(0, idx)));
                        break;
                    default:
                        throw new ArgumentException("invalid body format");
                }
                sp = sp.Slice(idx + 1);
            }
            return (salt, expectedhmac, encrypted_bytes);
        }
    }

}