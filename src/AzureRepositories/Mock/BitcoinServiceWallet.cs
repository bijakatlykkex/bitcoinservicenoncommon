using AzureStorage;
using Core.Exceptions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransactionSignerMocker.Repositories;
using NBitcoin;
using Core.Settings;

namespace TransactionSignerMocker.AzureRepositories
{
    public class BitcoinServiceWalletEntity : TableEntity, IWallet
    {
        // From: http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string GeneratePartitionKey()
        {
            return "BitcoinServiceWallet";
        }

        public static string GenerateRowKey(string privateKey)
        {
            var key = new Key(StringToByteArray(privateKey));
            var secret = new BitcoinSecret(key, Network.Main);
            var pubKey = secret.PubKey.ToHex();
            return $"{pubKey}";
        }

        public string PrivateKey
        {
            get;
            set;
        }

        public string PubKey
        {
            get;
            set;
        }
        public static BitcoinServiceWalletEntity Create(IWallet wallet)
        {
            return new BitcoinServiceWalletEntity
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(wallet.PrivateKey),
                PrivateKey = wallet.PrivateKey,
                PubKey = wallet.PubKey
            };
        }
    }

    public class BitcoinServiceWallet : IBitcoinServiceWallet
    {
        private const int EntityExistsHttpStatusCode = 409;

        private readonly INoSQLTableStorage<BitcoinServiceWalletEntity> _storage;

        private readonly BaseSettings _baseSettings;

        public BitcoinServiceWallet(INoSQLTableStorage<BitcoinServiceWalletEntity> storage,
            BaseSettings baseSettings)
        {
            _storage = storage;
            _baseSettings = baseSettings;
        }

        public Task InsertWallet(IWallet wallet)
        {
            Action<StorageException> throwIfBackend = (exception) =>
            {
                if (exception != null && exception.RequestInformation.HttpStatusCode == EntityExistsHttpStatusCode)
                    throw new BackendException("entity already exists", ErrorCode.TransactionConcurrentInputsProblem);
            };

            try
            {
                return _storage.InsertAsync(BitcoinServiceWalletEntity.Create(wallet));
            }
            catch (AggregateException e)
            {
                var exception = e.InnerExceptions[0] as StorageException;
                throwIfBackend(exception);
                throw;
            }
            catch (StorageException e)
            {
                throwIfBackend(e);
                throw;
            }
        }

        public async Task<IWallet> GetWallet(IWallet wallet)
        {
            return await _storage.GetDataAsync(BitcoinServiceWalletEntity.GeneratePartitionKey(),
                wallet.PubKey);
        }
    }
}
