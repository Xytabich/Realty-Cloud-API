using System;
using System.Collections.Generic;

namespace RealtyCloudAPI
{
	/// <summary>
	/// Информация о статусе заказа.
	/// </summary>
	public struct OrderStatusInfo
	{
		/// <summary>
		/// Идентификатор заказа.
		/// </summary>
		public string OrderID;
		/// <summary>
		/// Идентификатор владельца заказа.
		/// </summary>
		public string OwnerID;
		/// <summary>
		/// Откуда был произведен заказ (ссылка на источник или название).
		/// </summary>
		public string Source;
		/// <summary>
		/// Был ли уплачен заказ.
		/// </summary>
		public bool Paid;
		/// <summary>
		/// Время создания заказа.
		/// </summary>
		public DateTime CreationTime;
		/// <summary>
		/// Список продуктов в заказе.
		/// </summary>
		public OrderItem[] OrderItems;
		/// <summary>
		/// Идентификатор транзакции.
		/// </summary>
		public string TransactionID;

		internal static bool TryBuild(IDictionary<string, object> data, out OrderStatusInfo value)
		{
			value = new OrderStatusInfo();

			if(!data.TryGetString("OrderID", out value.OrderID))
			{
				return false;
			}

			if(!data.TryGetString("OwnerID", out value.OwnerID))
			{
				return false;
			}

			if(!data.TryGetBool("Paid", out value.Paid))
			{
				return false;
			}

			if(!data.TryGetDateTime("CreatedAt", out value.CreationTime))
			{
				return false;
			}

			if(!data.TryGetString("source", out value.Source))
			{
				value.Source = "";
			}

			if(!data.TryGetObjArray("OrderItem", OrderItem.TryBuild, out value.OrderItems))
			{
				return false;
			}

			if(!data.TryGetString("transaction_id", out value.TransactionID))
			{
				return false;
			}

			return true;
		}
	}

	/// <summary>
	/// Информация о статусе заказанного продукта.
	/// </summary>
	public struct OrderItem
	{
		/// <summary>
		/// Идентификатор заказанного продукта.
		/// </summary>
		public string OrderItemID;
		/// <summary>
		/// Идентификатор заказа.
		/// </summary>
		public string OrderID;
		/// <summary>
		/// Идентификатор продукта.
		/// </summary>
		public string ProductID;

		/// <summary>
		/// Был ли уплачен продукт.
		/// </summary>
		public bool Paid;
		/// <summary>
		/// Стоимость продукта, с учетом скидки.
		/// </summary>
		public decimal Price;
		/// <summary>
		/// Скидка при заказе.
		/// </summary>
		public decimal Discount;
		/// <summary>
		/// Количество продука.
		/// </summary>
		public decimal Quantity;

		/// <summary>
		/// Статус продукта.
		/// </summary>
		public string Status;
		/// <summary>
		/// Идентификатор запроса в Росреестр.
		/// </summary>
		public string RosreestrRequestID;
		/// <summary>
		/// Имя (ключ) продукта.
		/// </summary>
		public string ProductName;
		/// <summary>
		/// Метаданные продукта.
		/// </summary>
		public ItemMetadata Metadata;
		/// <summary>
		/// Ссылка на zip-архив, устанавливается если Status="done".
		/// </summary>
		public string ZipLink;
		/// <summary>
		/// Ссылка на pdf-документ устанавливается если Status="done".
		/// </summary>
		public string PdfLink;
		/// <summary>
		/// Список статусов продукта.
		/// </summary>
		public OrderItemStatus[] StatusList;

		internal static bool TryBuild(IDictionary<string, object> data, out OrderItem value)
		{
			value = new OrderItem();

			if(!data.TryGetString("OrderItemID", out value.OrderItemID))
			{
				return false;
			}

			if(!data.TryGetString("OrderID", out value.OrderID))
			{
				return false;
			}

			if(!data.TryGetString("ProductID", out value.ProductID))
			{
				return false;
			}

			if(!data.TryGetDecimal("Price", out value.Price))
			{
				return false;
			}

			if(!data.TryGetDecimal("Discount", out value.Discount))
			{
				return false;
			}

			if(!data.TryGetDecimal("Amount", out value.Quantity))
			{
				return false;
			}

			if(!data.TryGetString("status", out value.Status))
			{
				return false;
			}

			if(!data.TryGetString("ProductName", out value.ProductName))
			{
				return false;
			}

			if(!data.TryGetObj("ItemMetadata", ItemMetadata.TryBuild, out value.Metadata))
			{
				return false;
			}

			if(!data.TryGetBool("paid", out value.Paid))
			{
				return false;
			}

			if(!data.TryGetObjArray("item_statuses", OrderItemStatus.TryBuild, out value.StatusList))
			{
				return false;
			}

			data.TryGetString("rosreestr_request_id", out value.RosreestrRequestID);
			data.TryGetString("download_link_zip", out value.ZipLink);
			data.TryGetString("download_link_pdf", out value.PdfLink);

			return true;
		}
	}

	/// <summary>
	/// Метаданные продукта.
	/// </summary>
	public struct ItemMetadata
	{
		/// <summary>
		/// Идентификатор заказанного продукта.
		/// </summary>
		public string OrderItemID;
		/// <summary>
		/// Кадастровый номер целевого объекта.
		/// </summary>
		public string CadastralNumber;
		/// <summary>
		/// Адрес номер целевого объекта.
		/// </summary>
		public string Address;
		/// <summary>
		/// Время запроса.
		/// </summary>
		public DateTime RequestTime;

		internal static bool TryBuild(IDictionary<string, object> data, out ItemMetadata value)
		{
			value = new ItemMetadata();

			if(!data.TryGetString("order_item_id", out value.OrderItemID))
			{
				return false;
			}

			if(!data.TryGetString("kadastr_number", out value.CadastralNumber))
			{
				return false;
			}

			data.TryGetString("address", out value.Address);
			data.TryGetDateTime("requested_at", out value.RequestTime);

			return true;
		}
	}

	/// <summary>
	/// Статус заказанного продукта.
	/// </summary>
	public struct OrderItemStatus
	{
		/// <summary>
		/// Идентификатор статуса.
		/// </summary>
		public string StatusID;
		/// <summary>
		/// Идентификатор заказанного продукта.
		/// </summary>
		public string OrderItemID;
		/// <summary>
		/// Имя статуса.
		/// </summary>
		public string StatusName;
		/// <summary>
		/// Время создания статуса.
		/// </summary>
		public DateTime CreationTime;

		internal static bool TryBuild(IDictionary<string, object> data, out OrderItemStatus value)
		{
			value = new OrderItemStatus();

			if(!data.TryGetString("ID", out value.StatusID))
			{
				return false;
			}

			if(!data.TryGetString("OrderItemID", out value.OrderItemID))
			{
				return false;
			}

			if(!data.TryGetString("Status", out value.StatusName))
			{
				return false;
			}

			if(!data.TryGetDateTime("CreatedAt", out value.CreationTime))
			{
				return false;
			}

			return true;
		}
	}
}