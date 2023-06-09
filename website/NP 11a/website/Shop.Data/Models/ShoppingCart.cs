﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shop.Data.Models
{
	public class ShoppingCart
	{
		private readonly ApplicationDbContext _context;

		public ShoppingCart(ApplicationDbContext context)
		{
			_context = context;
		}

		public string Id { get; set; }
		public IEnumerable<ShoppingCartItem> ShoppingCartItems { get; set; }

		public static ShoppingCart GetCart(IServiceProvider services)
		{
			ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
			var context = services.GetService<ApplicationDbContext>();
			string cartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();

			session.SetString("CartId", cartId);
			return new ShoppingCart(context) { Id = cartId };
		}

		public bool AddToCart(GraphicPhoto GraphicPhoto, int amount)
		{
			if (GraphicPhoto.InStock == 0 || amount == 0)
			{
				return false;
			}

			var shoppingCartItem = _context.ShoppingCartItems.SingleOrDefault(
				s => s.GraphicPhoto.Id == GraphicPhoto.Id && s.ShoppingCartId == Id);
			var isValidAmount = true;
			if (shoppingCartItem == null)
			{
				if (amount > GraphicPhoto.InStock)
				{
					isValidAmount = false;
				}
				shoppingCartItem = new ShoppingCartItem
				{
					ShoppingCartId = Id,
					GraphicPhoto = GraphicPhoto,
					Amount = Math.Min(GraphicPhoto.InStock, amount)
				};
				_context.ShoppingCartItems.Add(shoppingCartItem);
			}
			else
			{
				if (GraphicPhoto.InStock - shoppingCartItem.Amount - amount >= 0)
				{
					shoppingCartItem.Amount += amount;
				}
				else
				{
					shoppingCartItem.Amount += (GraphicPhoto.InStock - shoppingCartItem.Amount);
					isValidAmount = false;
				}
			}


			_context.SaveChanges();
			return isValidAmount;
		}

		public int RemoveFromCart(GraphicPhoto GraphicPhoto)
		{
			var shoppingCartItem = _context.ShoppingCartItems.SingleOrDefault(
				s => s.GraphicPhoto.Id == GraphicPhoto.Id && s.ShoppingCartId == Id);
			int localAmount = 0;
			if (shoppingCartItem != null)
			{
				if (shoppingCartItem.Amount > 1)
				{
					shoppingCartItem.Amount--;
					localAmount = shoppingCartItem.Amount;
				}
				else
				{
					_context.ShoppingCartItems.Remove(shoppingCartItem);
				}
			}

			_context.SaveChanges();
			return localAmount;
		}

		public IEnumerable<ShoppingCartItem> GetShoppingCartItems()
		{
			return ShoppingCartItems ??
				   (ShoppingCartItems = _context.ShoppingCartItems.Where(c => c.ShoppingCartId == Id)
					   .Include(s => s.GraphicPhoto));
		}

		public void ClearCart()
		{
			var cartItems = _context
				.ShoppingCartItems
				.Where(cart => cart.ShoppingCartId == Id);

			_context.ShoppingCartItems.RemoveRange(cartItems);
			_context.SaveChanges();
		}

		public decimal GetShoppingCartTotal()
		{
			return _context.ShoppingCartItems.Where(c => c.ShoppingCartId == Id)
				.Select(c => c.GraphicPhoto.Price * c.Amount).Sum();
		}

	}
}