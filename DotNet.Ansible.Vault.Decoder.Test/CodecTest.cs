using System;
using System.Linq;
using Xunit;
using dotnet_ansible_vault_decoder;
using System.Text;
using System.IO;

namespace DotNet.Ansible.Vault.Decoder.Test
{
    public class CodecTest
    {
        [Fact]
        public void EncodeTest()
        {
            var codec = new AnsibleVaultCodec();
            var data = new byte[128];
            var salt = Enumerable.Range(0, 32).Select(i => (byte)((i * 2) & 0xff)).ToArray();
            var pass = Enumerable.Range(0, 16).Select(i => (byte)(i & 0xff)).ToArray();
            var sb = new StringBuilder();
            using(var sw = new StringWriter(sb))
            {
                codec.Encode(data, pass, salt, sw, "moge", 80);
            }
            using(var sr = new StringReader(sb.ToString()))
            using(var mstm = new MemoryStream())
            {
                codec.Decode(sr, pass, mstm);
                Assert.Equal(data, mstm.ToArray());
            }
        }
    }
}
