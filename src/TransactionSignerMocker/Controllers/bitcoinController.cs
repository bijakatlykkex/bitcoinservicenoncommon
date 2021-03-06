﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using TransactionSignerMocker.Repositories;
using TransactionSignerMocker.AzureRepositories;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TransactionSignerMocker.Controllers
{
    [Route("api/[controller]")]
    public class bitcoinController : Controller
    {
        private readonly IBitcoinServiceWallet _bitcoinServiceWalletRepository;

        public bitcoinController(IBitcoinServiceWallet bitcoinServiceWalletRepository)
        {
            _bitcoinServiceWalletRepository = bitcoinServiceWalletRepository;
        }

        [HttpGet("sayhello")]
        public string Sayhello()
        {
            return "Hello";
        }

        [HttpGet("key")]
        public async Task<string> Key()
        {
            var key = new NBitcoin.Key();
            await _bitcoinServiceWalletRepository.InsertWallet(new Wallet
            {
                PrivateKey =
                BitConverter.ToString(key.ToBytes()).Replace("-", "")
            });
            return key.PubKey.ToHex();
        }

        [HttpPost("sign")]
        public string Sign([FromBody]string Transaction)
        {
            return Transaction;
        }

    }
}
