using System.Collections.Generic;

namespace RealtyCloudAPI
{
	/// <summary>
	/// Информация о заказе.
	/// </summary>
	public struct OrderInfo
	{
		/// <summary>
		/// Идентификатор заказа.
		/// </summary>
		public string ID;
		/// <summary>
		/// Список продуктов в заказе.
		/// </summary>
		public OrderInfoItem[] Items;

		/// <summary>
		/// Итоговая стоимость всех продуктов в заказе.
		/// </summary>
		public decimal TotalPrice;
		/// <summary>
		/// Информация о счете.
		/// </summary>
		public AccountInfo AccountInfo;
		/// <summary>
		/// Была ли снята сумма со счета при заказе.
		/// </summary>
		public bool IsAccountBalanceUsed;

		internal static bool TryBuild(IDictionary<string, object> data, out OrderInfo value)
		{
			value = new OrderInfo();

			if(!data.TryGetString("id", out value.ID))
			{
				return false;
			}

			if(!data.TryGetObjArray("order_items", OrderInfoItem.TryBuild, out value.Items))
			{
				return false;
			}

			if(!data.TryGetDecimal("total_amount", out value.TotalPrice))
			{
				return false;
			}

			if(!data.TryGetBool("use_account_balance", out value.IsAccountBalanceUsed))
			{
				return false;
			}

			data.TryGetObj("account_info", AccountInfo.TryBuild, out value.AccountInfo);

			return true;
		}
	}

	/// <summary>
	/// Информация о заказанном продукте.
	/// </summary>
	public struct OrderInfoItem
	{
		/// <summary>
		/// Идентификатор заказанного продукта.
		/// </summary>
		public string OrderItemID;
		/// <summary>
		/// Имя (ключ) продукта.
		/// </summary>
		public string ProductName;
		/// <summary>
		/// Целевой объект.
		/// </summary>
		public string ObjectKey;
		/// <summary>
		/// Купон, если был использован при заказе.
		/// </summary>
		public string CouponID;
		/// <summary>
		/// Стоимость продукта.
		/// </summary>
		public decimal Price;

		internal static bool TryBuild(IDictionary<string, object> data, out OrderInfoItem value)
		{
			value = new OrderInfoItem();
			
			if(!data.TryGetString("order_item_id", out value.OrderItemID))
			{
				return false;
			}

			if(!data.TryGetString("product_name", out value.ProductName))
			{
				return false;
			}

			if(!data.TryGetString("object_key", out value.ObjectKey))
			{
				return false;
			}

			if(!data.TryGetString("coupon_id", out value.CouponID))
			{
				return false;
			}

			if(!data.TryGetDecimal("price", out value.Price))
			{
				return false;
			}

			return true;
		}
	}

	/// <summary>
	/// Информация о счете.
	/// </summary>
	public struct AccountInfo
	{
		/// <summary>
		/// True если на выполнение операции недостаточно средств.
		/// </summary>
		public bool NotEnoughMoney;
		/// <summary>
		/// Текущий баланс счета.
		/// </summary>
		public decimal CurrentBalance;
		/// <summary>
		/// Баланс счета до выполнения операции.
		/// </summary>
		public decimal PreviousBalance;
		/// <summary>
		/// Сумма, снятая со счета при выполнении операции.
		/// </summary>
		public decimal PaidAmount;

		internal static bool TryBuild(IDictionary<string, object> data, out AccountInfo value)
		{
			value = new AccountInfo();

			if(!data.TryGetBool("not_enough_money", out value.NotEnoughMoney))
			{
				return false;
			}

			if(!data.TryGetDecimal("balance_current", out value.CurrentBalance))
			{
				return false;
			}

			if(!data.TryGetDecimal("balance_before", out value.PreviousBalance))
			{
				return false;
			}

			if(!data.TryGetDecimal("amount", out value.PaidAmount))
			{
				return false;
			}

			return true;
		}
	}
}