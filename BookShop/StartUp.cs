using BookShop.Data;

using BookShop.Initializer;
using BookShop.Models;
using Microsoft.EntityFrameworkCore;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BookShop
{
    public class StartUp
    {
        static void Main(string[] args)
        {

            var context = new BookShopContext();
            //string input = Console.ReadLine();
            //DbInitializer.ResetDatabase(context);
            // Console.WriteLine(GetBooksByAgeRestriction(context, "miNor"));
            // Console.WriteLine(GetGoldenBooks(context));
            //  Console.WriteLine(GetBooksByPrice(context));
            // Console.WriteLine(GetBooksNotReleasedIn(context,1998));
            //Console.WriteLine(GetBooksByCategory(context,input));
            //Console.WriteLine(GetBooksReleasedBefore(context,"12-04-1992").Length);
            //Console.WriteLine(GetAuthorNamesEndingIn(context,input));
            // Console.WriteLine(GetBookTitlesContaining(context,input));
            //Console.WriteLine(GetBooksByAuthor(context,input));
            // Console.WriteLine(CountBooks(context,12));
            // Console.WriteLine(CountCopiesByAuthor(context));
            //Console.WriteLine(GetTotalProfitByCategory(context));
            //Console.WriteLine(GetMostRecentBooks(context));
            IncreasePrices(context);
            //Console.WriteLine(RemoveBooks(context));
        }
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            if (command == null)
            {
                command = Console.ReadLine();
            }

            return string.Join(Environment.NewLine, context.Books.ToList()
                .Where(b => b.AgeRestriction.ToString().Equals(command, StringComparison.OrdinalIgnoreCase))
                .Select(b => b.Title)
                .OrderBy(t => t));
        }
        public static string GetGoldenBooks(BookShopContext context)
        {

            return string.Join(Environment.NewLine, context.Books.ToList().OrderBy(b => b.BookId)
                .Where(b => b.Copies < 5000 && b.EditionType == Models.Enums.EditionType.Gold)
                .Select(b => b.Title)
                );
        }
        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var books = context.Books.Where(b => b.Price > 40)
                .Select(a => new
                {
                    BookTitle = a.Title,
                    BookPrice = a.Price
                }
                ).ToList();
            foreach (var b in books.OrderByDescending(p => p.BookPrice))
            {
                sb.AppendLine($"{b.BookTitle} - ${b.BookPrice:f2}");
            }
            return sb.ToString().TrimEnd();
        }
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books.Where(b => b.ReleaseDate.Value.Year != year && b.ReleaseDate != null)
                .Select(a => new
                {
                    BookTitle=a.Title
                }
                ).ToList();
            foreach (var b in books)
            {
                sb.AppendLine($"{b.BookTitle}");
            }
            return sb.ToString().TrimEnd();
        }
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            List<string> categories = input
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower())
                .ToList();

            List<string> bookTitles = new List<string>();

            foreach (var category in categories)
            {

                List<string> currentCategoryBooks = context.
                    Books
                    .Where(b => b.BookCategories.Any(bc => bc.Category.Name.ToLower() == category))
                    .Select(b => b.Title)
                    .ToList();
                bookTitles.AddRange(currentCategoryBooks);
            }

            return string.Join(Environment.NewLine, bookTitles.OrderBy(b => b));
        }
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder sb = new StringBuilder();
            DateTime dateTime = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var books = context.Books.OrderByDescending(d => d.ReleaseDate).Where(b => b.ReleaseDate < dateTime)
                .Select(a =>
                new
                {
                    BookTitle = a.Title,
                    BookEditionType=a.EditionType,
                    BookPrice=a.Price
                }
                )
            .ToList();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.BookTitle} - {b.BookEditionType} - ${b.BookPrice:f2}");
            }
            return sb.ToString().Trim();
        }
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();
            var authors = context.Authors.Where(a => a.FirstName.EndsWith(input))
                .Select(
                b => new
                {
                    AutherName = b.FirstName + " " + b.LastName
                }) .ToList();
            foreach (var a in authors.OrderBy(a=>a.AutherName))
            {
                sb.AppendLine(a.AutherName);
            }
            return sb.ToString().Trim();
        }
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();
            var books = context.Books.OrderBy(n => n.Title).Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(a => new
                { 
                    Title=a.Title
                }).ToList();
            foreach (var b in books)
            {
                sb.AppendLine(b.Title);
            }
            return sb.ToString().Trim();
        }
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();
            var searchBooks=context.Authors.Where(a => a.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(
                b => new
                {
                    AutherName = b.FirstName + " " + b.LastName,
                    AuthorBooks=b.Books.OrderBy(n => n.BookId)
                    .Select(x=>new
                    {
                        BookTitel=x.Title
                    }).ToList()
                }).ToList();
            foreach (var author in searchBooks)
            {
                foreach (var b in author.AuthorBooks)
                {
                    sb.AppendLine($"{ b.BookTitel} ({author.AutherName})");
                }
            }
            return sb.ToString().Trim();
          
        }
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books.Where(b => b.Title.Length > lengthCheck).Select(x=>x.Title).ToList().Count();
        }
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var authors = context.Authors
                .Select(b => new
                {
                    AutherName = b.FirstName + " " + b.LastName,
                    AuthorBooks = b.Books
                    .Select(x => new
                    {
                        BookTitel = x.Title,
                        BookCopies=x.Copies
                    }).ToList()
                }).ToList();
            foreach (var au in authors.OrderByDescending(a => a.AuthorBooks.Sum(b=>b.BookCopies)))
            {
                sb.AppendLine($"{au.AutherName} - {au.AuthorBooks.Sum(b => b.BookCopies)}");
            }
            return sb.ToString().Trim();
        }
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var categories = context.Categories
                .Select(b => new
                {
                    CategoryName=b.Name,
                    BooksCategory=b.CategoryBooks
                    .Select(c => new
                    {
                        BookCopies=c.Book.Copies,
                        BookPrice=c.Book.Price,
                        BookProfits= c.Book.Copies* c.Book.Price
                    }).ToList(),
                }).ToList();
            foreach (var cat in categories.OrderByDescending(a => a.BooksCategory.Sum(b => b.BookProfits)))
            {
                sb.AppendLine($"{cat.CategoryName} ${cat.BooksCategory.Sum(b=>b.BookProfits):f2}");
            }

              return sb.ToString().Trim();
        }
        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categories = context.Categories.OrderBy(a=>a.Name)
                .Select(b => new
                {
                    CategoryName = b.Name,
                    BooksCategory = b.CategoryBooks
                    .Select(c => new
                    {
                        BookName = c.Book.Title,
                        RealaseYear = c.Book.ReleaseDate.Value.Year,
                        RealaseDate=c.Book.ReleaseDate
                    }).ToList(),
                }).ToList();
            foreach (var c in categories)
            {
                sb.AppendLine($"--{c.CategoryName}");
                foreach (var b in c.BooksCategory.OrderByDescending(y => y.RealaseDate).Take(3))
                {
                    sb.AppendLine($"{b.BookName} ({b.RealaseYear})");
                }
            }  
            return sb.ToString().Trim();
        }
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books.Where(b => b.ReleaseDate.Value.Year < 2010);
            foreach (var b in  books)
            {
                b.Price += 5;
            }

            context.SaveChanges();
        }
        public static int RemoveBooks(BookShopContext context)
        {
            int count = context.Books.Where(b => b.Copies < 4200).Count();
            var books = context.Books.Where(b => b.Copies < 4200).ToList();
            context.RemoveRange(books);
            context.SaveChanges();
            return count;
        }
    }
}
