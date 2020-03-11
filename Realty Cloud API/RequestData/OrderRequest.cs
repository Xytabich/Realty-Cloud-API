using System;
using System.Collections.Generic;

namespace RealtyCloudAPI
{
	/// <summary>
	/// Запрос на продукт(ы).
	/// </summary>
	public struct OrderRequest
	{
		/// <summary>
		/// Запрашиваемые продукты.
		/// </summary>
		public OrderRequestItem[] Items;
		/// <summary>
		/// Использовать баланс аккаунта или создать только заявку.
		/// </summary>
		public bool UseAccountBalance;

		/// <summary>
		/// Конструктор запроса.
		/// </summary>
		/// <param name="useAccountBalance">Использовать баланс аккаунта или создать только заявку</param>
		/// <param name="items">Запрашиваемые продукты</param>
		public OrderRequest(bool useAccountBalance, params OrderRequestItem[] items)
		{
			this.UseAccountBalance = useAccountBalance;
			this.Items = items;
		}

		internal IDictionary<string, object> ToObject()
		{
			return new Dictionary<string, object>() {
				{ "order_items", new List<object>(Array.ConvertAll(Items, Converter)) },
				{ "use_account_balance", UseAccountBalance }
			};
		}

		internal static object Converter(OrderRequestItem item) { return item.ToObject(); }
	}

	/// <summary>
	/// Запрашиваемый продукт.
	/// </summary>
	public struct OrderRequestItem
	{
		/// <summary>
		/// Имя (ключ) запрашиваемого продукта.
		/// </summary>
		public string ProductName;
		/// <summary>
		/// Целевой объект.
		/// </summary>
		public string ObjectKey;
		/// <summary>
		/// Купон, используемый при заказе (если есть).
		/// </summary>
		public string CouponID;

		/// <summary>
		/// Конструктор запрашиваемого продукта.
		/// </summary>
		/// <param name="productName">Имя запрашиваемого продукта</param>
		/// <param name="objectKey">Целевой объект</param>
		public OrderRequestItem(string productName, string objectKey)
		{
			this.ProductName = productName;
			this.ObjectKey = objectKey;
			this.CouponID = string.Empty;
		}

		/// <summary>
		/// Конструктор запрашиваемого продукта.
		/// </summary>
		/// <param name="productName">Имя запрашиваемого продукта</param>
		/// <param name="objectKey">Целевой объект</param>
		/// <param name="couponID">Купон, используемый при заказе</param>
		public OrderRequestItem(string productName, string objectKey, string couponID)
		{
			this.ProductName = productName;
			this.ObjectKey = objectKey;
			this.CouponID = couponID;
		}

		internal IDictionary<string, object> ToObject()
		{
			var obj = new Dictionary<string, object>() {
				{ "product_name", ProductName },
				{ "object_key", ObjectKey }
			};

			if(!string.IsNullOrEmpty(CouponID)) obj.Add("coupon_id", CouponID);

			return obj;
		}
	}
}