// See https://aka.ms/new-console-template for more information
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Identity;
using PasswordHash;

Console.WriteLine("Hello, World!");




var hasher = new PasswordHasher<UserAccount>();
var user = new UserAccount();


var hash = hasher.HashPassword(user, "AQAAAAIAAYagAAAAEN6CLlj/VLC/oN9fc4uuCdx3gezV4xcprkN/Td5X1WDNnmn0vPpYyawSYb3k6BfmGw==");
Console.WriteLine(hash);