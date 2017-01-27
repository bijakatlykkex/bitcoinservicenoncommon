using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionSignerMocker.Repositories
{
    public interface IWallet
    {
        string PubKey { get; }
        string PrivateKey { get; set; }
    }

    public interface IBitcoinServiceWallet
    {
        Task InsertWallet(IWallet wallet);

        Task<IWallet> GetWallet(IWallet wallet);
    }
}
