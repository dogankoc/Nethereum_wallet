using System;
using static System.Console;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.HdWallet;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using NBitcoin;
//using Rijndael256;
using System.Threading;

namespace Wallets
{
    class EthereumWallet
    {
        const string network = "https://eth-mainnet.g.alchemy.com/v2/tHJ00oeslB7iQgburf-xwMMg_DH9ompj";
        static int z = 0;
        static bool foundit = false;
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }
        

        static async Task MainAsync(string[] args)
        {
            Web3 web3 = new Web3(network);

            do
            {
                await CreateWallets(web3, z);
                z++;

            } while (!foundit);
        }

        private static async Task CreateWallets(Web3 web3, int count)
        {
            try
            {
                Random random = new Random();
                DateTime customDateTime = new DateTime(random.Next(2018, 2023), random.Next(1, 13), random.Next(1, 29), random.Next(0, 24), random.Next(0, 60), random.Next(0, 60));
                string customSeed = customDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                Console.WriteLine("Oluşturulan Tarih: " + customDateTime.ToString("yyyy-MM-dd HH:mm:ss"));                
                Wallet wallet = new Wallet(Wordlist.English, WordCount.Twelve, customSeed);
                //Wallet wallet = new Wallet(Wordlist.English, WordCount.Twelve);
                string words = string.Join(" ", wallet.Words);
                string privatekey = wallet.GetAccount(0).PrivateKey.ToString();
                
                Console.WriteLine(count + ") ---------------------------------------------------------------------");
                Console.WriteLine("Seeds: " + words);
                Console.WriteLine("Private Key: " + privatekey);
                Console.WriteLine();
                Console.WriteLine("Addresses: ");
                for (int i = 0; i < 3; i++)
                {
                    string address = wallet.GetAccount(i).Address;
                    var balance = await web3.Eth.GetBalance.SendRequestAsync(address);
                    var etherAmount = Web3.Convert.FromWei(balance.Value);

                    Console.WriteLine(i + ")" + "\t" + address + "\t" + etherAmount + " ETH");

                    if (etherAmount > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        foundit = true;
                        await SaveAccountToFile(address, etherAmount);
                    }
                }
                Console.WriteLine();
                Console.WriteLine();
            }
            catch (Exception)
            {
                Console.WriteLine("ERROR!");
                throw;
            }
        }

        private static async Task SaveAccountToFile(string address, decimal etherAmount)
        {
            try
            {
                string filePath = "accounts.txt";
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    await writer.WriteLineAsync($"{address}\t{etherAmount} ETH");
                }

                Console.WriteLine($"Account with non-zero balance saved: {address}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
