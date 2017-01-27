using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionSignerMocker.Repositories
{
    public class Wallet : IWallet
    {
        public string PrivateKey
        {
            get;
            set;
        }

        public string PubKey
        {
            get
            {
                if(PrivateKey == null)
                {
                    return null;
                }
                else
                {
                    var key = new Key(StringToByteArray(PrivateKey));
                    var secret = new BitcoinSecret(key, Network.Main);
                    return secret.PubKey.ToHex();
                }
            }
        }

        // From: http://stackoverflow.com/questions/311165/how-do-you-convert-byte-array-to-hexadecimal-string-and-vice-versa
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }


    }
}
