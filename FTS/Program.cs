﻿using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace FTS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var context = new FullTextSearchContext();

            var results = context.Posts.Where(p => EF.Functions.ToTsVector("english", p.Title + " " + p.Body)
                                                        .Matches(EF.Functions.ToTsQuery("Npgsql"))).ToList();

            Console.WriteLine($"Found {results.Count} matches");
            
            results = context.Posts.Where(p => EF.Functions.ToTsVector("english", p.Title).SetWeight(NpgsqlTsVector.Lexeme.Weight.A)
                                                        .Concat(EF.Functions.ToTsVector("english", p.Body).SetWeight(NpgsqlTsVector.Lexeme.Weight.B))
                                                        .Concat(EF.Functions.ToTsVector("simple", p.Author).SetWeight(NpgsqlTsVector.Lexeme.Weight.C))
                                                        .Concat(EF.Functions.ToTsVector("simple", p.Tags).SetWeight(NpgsqlTsVector.Lexeme.Weight.B))
                .Matches(EF.Functions.ToTsQuery("sql"))).ToList();

            Console.WriteLine($"Found {results.Count} matches");
        }
    }
}