# dotnet-ansible-vault-decoder

This is dotnet global tool for decoding/encoding [Ansible's vault data](https://docs.ansible.com/ansible/latest/user_guide/vault.html)

* [NuGet Page](https://www.nuget.org/packages/dotnet-ansible-vault-decoder/)

# Installation

## Prerequires

* [dotnet-sdk-2.1 or later](https://dotnet.microsoft.com/download)

## install via nuget

1. run `dotnet tool install -g dotnet-ansible-vault-decoder`
2. add `$HOME/.dotnet/tools` to your PATH env value


# Usage

you can execute by `dotnet-anv [encode/decode] [options]` command.
here is the `--help` output;

```
dotnet-anv 0.1.0.0

Usage: ansible-vault-decoder [options] [command]

Options:
  --version     Show version information
  -?|-h|--help  Show help information

Commands:
  decode        
  encode        

Run 'ansible-vault-decoder [command] --help' for more information about a command.
```

## encode

if you want to create ansible vault file, you can do it by `dotnet-anv encode`.
here is `dotnet-anv encode --help` output;

```
Usage: ansible-vault-decoder encode [options]

Options:
  -i|--input     input data file(default stdin)
  -o|--output    output file path(default stdout)
  -e|--eol       output end of line(cr, crlf, lf, if not set, using system default value)
  -p|--password  encryption password, if not set, input via prompt
  --passfile     password file(using first line, trailing whitespace will be trimmed
  --passenv      get password from environment variable
  -l|--label     vault label
  -?|-h|--help   Show help information

```

### Examples

* basic usage: `dotnet-anv encode -i plaindata.txt -o encrypted.yml -p password`
* input from stdin: `echo abcde|dotnet-anv encode -o encrypted.yml -p password`
* output to stdout: `dotnet-anv encode -i plaintdata.txt -p password`
* password by env: `export ANV_PASS=password;dotnet-anv encode --passenv ANV_PASS -i plaindata.txt`
* password by file: `dotnet-anv encode --passfile password.txt -i plaindata.txt`

## decode

if you want to decrypting ansible vault file, you can do it by `dotnet-anv decode`
here is `dotnet-anv decode --help` output;

```
Usage: ansible-vault-decoder decode [options]

Options:
  -i|--input     input data file(default stdin)
  -o|--output    output file path(default stdout)
  -p|--password  encryption password, if not set, input via prompt
  --passfile     password file(using first line, trailing whitespace will be trimmed
  --passenv      get password from environment variable
  -?|-h|--help   Show help information
```

### Examples

* basic usage: `dotnet-anv decode -i encrypted.yml -o plaindata.txt -p password`

## password

Password for encryption/decryption is specified by three ways

1. pass by argument: using argument value, by `-p [plaintext password]`, most simple way
2. pass by environment variable: using environment value, by `--passenv [environment name]`
3. pass by plaintext file: using text file value, by `--passfile [path to password file]`

