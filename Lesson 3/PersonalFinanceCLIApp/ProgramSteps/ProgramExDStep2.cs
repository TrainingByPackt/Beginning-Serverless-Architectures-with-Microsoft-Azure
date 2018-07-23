﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using CsvHelper;
using System.Web;
using PersonalFinanceCLIApp.Models;

namespace PersonalFinanceCLIApp
{
    class Program
    {
        private static readonly string key = "Your Function Key";
        private static readonly HttpClient httpClient = new HttpClient();
        static void Main(string[] args)
        {
            var enterAnotherTransaction = true;
            while (enterAnotherTransaction)
            {
                Console.WriteLine("Enter in what you spent today!");
                var transactionAmount = decimal.Parse(Console.ReadLine());

                // Borrowing the Transaction definition from our other project
                var transaction = new Transaction { Amount = transactionAmount, ExecutionTime = DateTime.Now };

                // Check if the csv exists, and if not prepopulate it with headers.
                if (!File.Exists(@"transactions.csv"))
                {
                    using (StreamWriter writer = new StreamWriter(@"transactions.csv"))
                    {
                        var csvWriter = new CsvWriter(writer);
                        csvWriter.WriteHeader<Transaction>();
                        csvWriter.NextRecord();
                        csvWriter.Flush();
                    }
                }
                    

                // Add the transaction to the list and write it back into the csv
                using (StreamWriter writer = File.AppendText(@"transactions.csv"))
                {
                    var csvWriter = new CsvWriter(writer);
                    csvWriter.WriteRecord<Transaction>(transaction);
                    csvWriter.NextRecord();
                    csvWriter.Flush();
                    Console.WriteLine("Transaction successfully added");
                }

                // Read out what transactions are in the file
                List<Transaction> transactions;
                using (StreamReader reader = new StreamReader(@"transactions.csv"))
                {
                    var csvReader = new CsvReader(reader);
                    transactions = csvReader.GetRecords<Transaction>().ToList();
                }

                // Calculate some interesting facts about the users transaction history
                var totalTransactions = transactions.Sum(t => t.Amount);
                var meanTransactionSize = transactions.Average(t => t.Amount);
                var largestTransactionAmount = transactions.Max(t => t.Amount);

                Console.WriteLine($"In total you've logged transactions worth {totalTransactions} with me.\n" +
                    $"Your average transaction was {meanTransactionSize}.\n" +
                    $"Your largest transaction ever was {largestTransactionAmount}");

                Console.WriteLine("Do you want to add another transaction?");
                enterAnotherTransaction = bool.Parse(Console.ReadLine());
            }
            
        }
    }
}
